using Corno.Data.Reports;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Reports.CrystalReports;
using Corno.Reports.DataSets;
using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Data;
using System.Data.Entity.Validation;
using System.Data.SqlClient;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Corno.Services.Core.Interfaces;

namespace Corno.OnlineExam.Areas.Reports.Controllers;

public class HallTicketController : Controller
{
    #region -- Data Members --

    private readonly ICoreService _examService;
    #endregion

    #region -- Constructors --
    public HallTicketController(ICoreService examService)
    {
        _examService = examService;
    }
    #endregion

    [Authorize]
    public ActionResult HallTicket()
    {
        StoreCollegesInViewBag();

        var viewModel = new HallTicketViewModel();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public ActionResult HallTicket(HallTicketViewModel viewModel)
    {
        try
        {
            if (ModelState.IsValid)
            {
                int instanceId = Convert.ToInt16(HttpContext.Session[ModelConstants.InstanceId].ToString());
                var dataHallTicket = GetHallTicketDataSet(viewModel, instanceId);

                //DataColumn column = dataHallTicket.Tables["Hall_Ticket"].Columns["Ima_ST_PHOTO"];
                var blankBitmap = new Bitmap(256, 256);
                using (var g = Graphics.FromImage(blankBitmap)) { g.Clear(Color.White); }

                foreach (DataTable table in dataHallTicket.Tables)
                {
                    foreach (DataRow row in table.Rows)
                    {
                        if (row["Ima_ST_PHOTO"] == DBNull.Value || row["Ima_Flg"] == DBNull.Value)
                        {
                            row["Ima_ST_PHOTO"] = ImageToByteArray(blankBitmap);
                            continue;
                        }

                        var data = (byte[]) row["Ima_ST_PHOTO"];
                        if (null == data || data.Length <= 0)
                            row["Ima_ST_PHOTO"] = ImageToByteArray(blankBitmap);
                    }
                }

                // If Center is selected, delete all records other than prn no.
                if (viewModel.CentreId is > 0)
                {
                    // Get Center name from Center Id
                    dataHallTicket.Tables["Hall_Ticket"].Rows.OfType<DataRow>()
                        .Where(r => r.Field<short>("Num_FK_DistCenter_ID") != viewModel.CentreId)
                        .ToList()
                        .ForEach(r => r.Delete());
                }

                // If PRN is selected, delete all records other than prn no.
                if (!string.IsNullOrEmpty(viewModel.PrnNo))
                {
                    dataHallTicket.Tables["Hall_Ticket"].Rows.OfType<DataRow>()
                        .Where(r => r.Field<string>("Chr_FK_PRN_NO") != viewModel.PrnNo)
                        .ToList()
                        .ForEach(r => r.Delete());
                }

                if (string.IsNullOrEmpty(viewModel.PrnNo))
                    viewModel.PrnNo = string.Empty;

                var report = new HallTicket();
                report.SetDataSource(dataHallTicket.Tables["Hall_Ticket"]);

                ViewData["ReportName"] = "Hall Ticket";
                if (HttpContext.Session["CrystalReport"] is ReportDocument existingReport)
                {
                    existingReport.Close();
                    existingReport.Dispose();
                }
                HttpContext.Session["CrystalReport"] = report;

                //return Redirect("/ReportViewer/CrystalReportViewer.aspx");
                return View(@"../Report/Details");
            }
        }
        catch (DbEntityValidationException dbEx)
        {
            // To Check in Watch List
            //((System.Data.Entity.Validation.DbEntityValidationException)$exception).EntityValidationErrors

            foreach (var validationErrors in dbEx.EntityValidationErrors)
            {
                foreach (var validationError in validationErrors.ValidationErrors)
                    ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
            }
        }
        catch (Exception exception)
        {
            var errors = ModelState.Values.SelectMany(v => v.Errors);
            foreach (var error in errors)
                ModelState.AddModelError(error.ErrorMessage, error.Exception);

            ModelState.AddModelError(string.Empty, exception.Message);
            if (exception.InnerException != null)
            {
                ModelState.AddModelError(string.Empty, exception.InnerException.Message);
                if (null != exception.InnerException.InnerException)
                    ModelState.AddModelError(string.Empty, exception.InnerException.InnerException.Message);
            }
        }

        StoreCollegesInViewBag();

        return View(viewModel);
    }

    #region -- Methods --
    public byte[] ImageToByteArray(Bitmap imageIn)
    {
        var ms = new MemoryStream();
        imageIn.Save(ms, ImageFormat.Png);
        return ms.ToArray();
    }

    private void StoreCollegesInViewBag()
    {
        if (User.IsInRole("College") && null != HttpContext.Session[ModelConstants.CollegeId])
        {
            int? collegeId = (int)HttpContext.Session[ModelConstants.CollegeId];
            ViewBag.Colleges = _examService.TBL_COLLEGE_MSTRRepository.Get(c => c.Num_PK_COLLEGE_CD == collegeId)
                .Select(c => new { ID = c.Num_PK_COLLEGE_CD, Name = c.Var_CL_COLLEGE_NM1, NameWithID = "(" + c.Num_PK_COLLEGE_CD.ToString() + ") " + c.Var_CL_COLLEGE_NM1 })
                .ToList().Distinct().OrderBy(c => c.ID);
        }
        else
        {
            ViewBag.Colleges = _examService.TBL_COLLEGE_MSTRRepository.Get().ToList()
                .Select(c => new { ID = c.Num_PK_COLLEGE_CD, Name = c.Var_CL_COLLEGE_NM1, NameWithID = "(" + c.Num_PK_COLLEGE_CD.ToString() + ") " + c.Var_CL_COLLEGE_NM1 })
                .ToList().Distinct().OrderBy(c => c.ID);
        }
    }

    public static Data_HallTicket GetHallTicketDataSet(HallTicketViewModel viewModel, int instanceId)
    {
        using var dsHallTicket = new SqlConnection(GlobalVariables.ConnectionStringExamServer);
        dsHallTicket.Open();

        using var cmd = new SqlCommand();
        cmd.Connection = dsHallTicket;

        cmd.CommandType = CommandType.StoredProcedure;
        cmd.CommandText = "sp_HallTicket_Show";
        cmd.CommandTimeout = 6000;

        cmd.Parameters.AddWithValue("@EXAMINST", instanceId);
        if (viewModel.CoursePartId != null)
            cmd.Parameters.AddWithValue("@Course_Part", (int)viewModel.CoursePartId);
        if (viewModel.CollegeId != null)
            cmd.Parameters.AddWithValue("@College", (int)viewModel.CollegeId);
        if (viewModel.BranchId is > 0)
            cmd.Parameters.AddWithValue("@Branch", (int)viewModel.BranchId);
        var seatNoFrom = Convert.ToInt64(viewModel.FromSeatNo);
        var seatNoTo = Convert.ToInt64(viewModel.ToSeatNo);
        cmd.Parameters.AddWithValue("@SeatNoFrom", seatNoFrom);
        cmd.Parameters.AddWithValue("@SeatNoTo", seatNoTo);

        var prnNo = viewModel.PrnNo ?? string.Empty;
        cmd.Parameters.AddWithValue("@PRN", prnNo);

        using var da = new SqlDataAdapter(cmd);
        // Fill the DataSet using default values for DataTable names, etc
        var dataSet = new Data_HallTicket();
        da.Fill(dataSet, "Hall_Ticket");

        return dataSet;
        //dsHallTicket.Close();
    }

    #endregion
}