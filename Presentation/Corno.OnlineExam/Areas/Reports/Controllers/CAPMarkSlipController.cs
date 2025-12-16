using Corno.Data.Reports;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.OnlineExam.Controllers;
using Corno.Reports.CrystalReports;
using Corno.Reports.DataSets;
using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Data;
using System.Data.SqlClient;
using System.Linq;
using System.Web.Mvc;
using Corno.Services.Core.Interfaces;

namespace Corno.OnlineExam.Areas.Reports.Controllers;

public class CapMarkSlipController : BaseController
{
    #region -- Data Members --

    private readonly ICoreService _examService;
    #endregion

    #region -- Constructors --
    public CapMarkSlipController(ICoreService examService)
    {
        _examService = examService;
    }
    #endregion

    [Authorize]
    public ActionResult CapMarkSlip()
    {
        StoreCollegesInViewBag();

        var viewModel = new MarkSlipViewModel();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public ActionResult CapMarkSlip(MarkSlipViewModel viewModel)
    {
        try
        {
            if (ModelState.IsValid)
            {
                int instanceId = Convert.ToInt16(HttpContext.Session[ModelConstants.InstanceId].ToString());
                var dsBlankfromCap = GetCapMarkSlipDataSet(viewModel, instanceId);

                if (viewModel.SubjectId > 0)
                {
                    dsBlankfromCap.Tables["StudentSeatNo"].Rows.OfType<DataRow>()
                        .Where(r => r.Field<int>("Subject_Cd") != viewModel.SubjectId)
                        .ToList()
                        .ForEach(r => r.Delete());
                }

                if (null != viewModel.CentreId && viewModel.CentreId > 0)
                {
                    // Get Center name from Center Id
                    //var centerName = ExamServerHelper.GetCentreName(viewModel.CentreId, _examService);
                    dsBlankfromCap.Tables["StudentSeatNo"].Rows.OfType<DataRow>()
                        .Where(r => r.Field<int>("DistCenter") != viewModel.CentreId)
                        //.Where(r => r.Field<string>("DistCenterName").EndsWith(centerName) == false)
                        .ToList()
                        .ForEach(r => r.Delete());
                }

                if (null != dsBlankfromCap)
                {
                    ReportClass report;
                    if (viewModel.IsMedical)
                        report = new MarkSlip_BlankAll_Medical();
                    else
                        report = new MarkSlip_BlankAll();

                    report.SetDataSource(dsBlankfromCap.Tables["StudentSeatNo"]);

                    report.SetParameterValue(0, @"BHARATI VIDYAPEETH (DEEMED TO BE UNIVERSITY), PUNE, INDIA");

                    report.SetParameterValue(1, "(" + viewModel.CourseId + ")" + " ( " + ExamServerHelper.GetCourseNameFromCoursePartId(viewModel.CoursePartId, _examService) + " ) ");
                    report.SetParameterValue(2, "(" + viewModel.CoursePartId + ")" + " ( " + ExamServerHelper.GetCoursePartName(viewModel.CoursePartId, _examService) + " ) " + HttpContext.Session[ModelConstants.InstanceName]);
                    report.SetParameterValue(3, "(" + viewModel.CollegeId + ")" + " ( " + ExamServerHelper.GetCollegeShortName(viewModel.CollegeId, _examService) + " ) ");
                    report.SetParameterValue(4, string.Empty);

                    ViewData["ReportName"] = "CAP Mark Slip";
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
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        StoreCollegesInViewBag();

        return View(viewModel);
    }

    #region -- Methods --
    private void StoreCollegesInViewBag()
    {
        if (User.IsInRole("College") && null != HttpContext.Session[ModelConstants.CollegeId])
        {
            //Nullable<int> centreID = _masterService.CollegeRepository.GetByID((int)HttpContext.Session[ModelConstants.CollegeID]).CentreID;
            //viewModel.Centres = _masterService.CentreRepository.Get(c => c.ID == centreID).ToList();
            var collegeId = (int) HttpContext.Session[ModelConstants.CollegeId];
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

    public static DSBlankfromCAP GetCapMarkSlipDataSet(MarkSlipViewModel viewModel, int? instanceId)
    {
        using (var dsMarkSlip = new SqlConnection(GlobalVariables.ConnectionStringExamServer))
        {

            dsMarkSlip.Open();

            using (var cmd = new SqlCommand())
            {
                cmd.Connection = dsMarkSlip;

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "SP_StudentSeatNo_Sel_All";
                cmd.CommandTimeout = 600;

                cmd.Parameters.AddWithValue("@Num_FK_INST_NO", instanceId);
                if (null == viewModel.BranchId || viewModel.BranchId < 0)
                    viewModel.BranchId = 0;
                cmd.Parameters.AddWithValue("@Num_FK_BR_CD", (int) viewModel.BranchId);
                if (viewModel.CoursePartId != null)
                    cmd.Parameters.AddWithValue("@Num_FK_COPRT_NO", (int) viewModel.CoursePartId);
                if (viewModel.CollegeId != null)
                    cmd.Parameters.AddWithValue("@Num_FK_COL_CD", (int) viewModel.CollegeId);
                //cmd.Parameters.AddWithValue("@Subject_Cd", Convert.ToInt32(viewModel.SubjectID));
                //cmd.Parameters.AddWithValue("@Cat_cd", Convert.ToInt32(viewModel.CategoryID));
                //cmd.Parameters.AddWithValue("@Pap_Cd", Convert.ToInt32(viewModel.PaperNo));
                //cmd.Parameters.AddWithValue("@Sec_Cd", Convert.ToInt32(viewModel.SectionNo));

                using (var da = new SqlDataAdapter(cmd))
                {
                    // Fill the DataSet using default values for DataTable names, etc
                    var dataset = new DSBlankfromCAP();
                    da.Fill(dataset, "StudentSeatNo");

                    return dataset;
                }
                //dsMarkSlip.Close();
            }
        }
    }

    #endregion
}