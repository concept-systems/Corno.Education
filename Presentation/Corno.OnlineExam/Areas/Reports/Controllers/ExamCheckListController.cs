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

public class ExamCheckListController : BaseController
{
    #region -- Data Members --

    private readonly ICoreService _examService;
    #endregion

    #region -- Constructors --
    public ExamCheckListController(ICoreService examService)
    {
        _examService = examService;
    }
    #endregion

    [Authorize]
    public ActionResult ExamCheckList()
    {
        StoreCollegesInViewBag();

        var viewModel = new ExamCheckListViewModel();
        return View(viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public ActionResult ExamCheckList(ExamCheckListViewModel viewModel)
    {
        try
        {
            if (ModelState.IsValid)
            {
                var report = new ExamAppForm();

                int instanceId = Convert.ToInt16(HttpContext.Session[ModelConstants.InstanceId].ToString());
                if (viewModel.CoursePartId != null)
                {
                    var dsExamForm = ExamFormAppCheckList((int) viewModel.CoursePartId, instanceId, viewModel.Bundle);

                    // If Branch is selected, delete all records other than Branch.
                    if (viewModel.BranchId > 0)
                    {
                        dsExamForm.Tables["ExamForm"].Rows.OfType<DataRow>()
                            .Where(r => r.Field<int>("Num_FK_BR_CD") != viewModel.BranchId)
                            .ToList()
                            .ForEach(r => r.Delete());
                    }

                    // If PRN is selected, delete all records other than prn no.
                    if (!string.IsNullOrEmpty(viewModel.PrnNo))
                    {
                        dsExamForm.Tables["ExamForm"].Rows.OfType<DataRow>()
                            .Where(r => r.Field<string>("Chr_APP_PRN_NO") != viewModel.PrnNo)
                            .ToList()
                            .ForEach(r => r.Delete());
                    }

                    if (viewModel.CentreId > 0)
                    {
                        dsExamForm.Tables["ExamForm"].Rows.OfType<DataRow>()
                            .Where(r => r.Field<string>("Num_FK_DistCenter_ID") != viewModel.CentreId.ToString())
                            .ToList()
                            .ForEach(r => r.Delete());
                    }

                    if (null != dsExamForm)
                    {
                        report.Refresh();

                        report.SetDataSource(dsExamForm);

                        report.SetParameterValue(0, "BHARATI VIDYAPEETH DEEMED UNIVERSITY, PUNE");
                        report.SetParameterValue(1, HttpContext.Session[ModelConstants.InstanceName].ToString());
                        report.SetParameterValue(2, viewModel.CourseId);
                        report.SetParameterValue(3, ExamServerHelper.GetCourseNameFromCoursePartId(viewModel.CoursePartId, _examService));
                        report.SetParameterValue(4, viewModel.CoursePartId);
                        report.SetParameterValue(5, ExamServerHelper.GetCoursePartName(viewModel.CoursePartId, _examService));
                        report.SetParameterValue(6, viewModel.Bundle);
                        //report.SetParameterValue(7, ExamServerHelper.GetBundle(viewModel.PRNNo, _examService));


                        ViewData["ReportName"] = "CheckList";

                        var existingReport = HttpContext.Session["CrystalReport"] as ReportDocument;
                        if (existingReport != null)
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
            int? collegeId = (int) HttpContext.Session[ModelConstants.CollegeId];
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

    public static Ds_ExamForm ExamFormAppCheckList(int coursePartId, int instanceId, string bundle)
    {
        using (var dsExamForm = new SqlConnection(GlobalVariables.ConnectionStringExamServer))
        {

            dsExamForm.Open();

            using (var cmd = new SqlCommand())
            {
                cmd.Connection = dsExamForm;

                cmd.CommandType = CommandType.StoredProcedure;
                cmd.CommandText = "Sp_ExamAppFormCheckList";
                cmd.CommandTimeout = 600;

                cmd.Parameters.AddWithValue("@Num_FK_COPRT_NO", coursePartId);
                cmd.Parameters.AddWithValue("@Num_FK_INST_NO", instanceId);
                cmd.Parameters.AddWithValue("@BUNDAL", bundle);

                using (var sqlDataAdapter = new SqlDataAdapter(cmd))
                {
                    // Fill the DataSet using default values for DataTable names, etc
                    var dataset = new Ds_ExamForm();
                    sqlDataAdapter.Fill(dataset, "ExamForm");

                    return dataset;
                }
            }
        }
    }

    #endregion
}