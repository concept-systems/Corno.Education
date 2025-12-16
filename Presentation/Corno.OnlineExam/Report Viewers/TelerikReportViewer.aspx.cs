using Corno.Globals.Constants;
using Corno.Reports.Convocation;
using Corno.Reports.Environment;
using Corno.Reports.Exam;
using Corno.Reports.Registration;
using Corno.Reports.Revaluation;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;
using Corno.Globals;
using Corno.Reports.Accounts;
using Corno.Reports.AnswerSheet;
using Corno.Reports.Mobile_Update;
using Corno.Reports.Time_Table;
using Corno.Services.Bootstrapper;
using Telerik.Reporting;

namespace Corno.OnlineExam.Report_Viewers;

[Authorize]
public partial class TelerikReportViewer : Page
{
    #region -- Private Methods --
    private Report GetReport()
    {
        var reportName = HttpContext.Current.Session["ReportName"]?.ToString();
        Report report = null;

        var sessionData = Session[User.Identity.Name] as SessionData;

        var instanceId = sessionData?.InstanceId ?? 0;//(int)HttpContext.Current.Session[ModelConstants.InstanceId];
        var collegeId = 0;

        if (User.IsInRole(ModelConstants.College) &&
            sessionData?.CollegeId > 0) //HttpContext.Current.Session[ModelConstants.CollegeId].ToInt() > 0)
            collegeId = sessionData?.CollegeId ?? 0;//Convert.ToInt32(HttpContext.Current.Session[ModelConstants.CollegeId]);

        switch (reportName)
        {
            //Registration
            case "CollegewiseStudent":
                report = new CollegewiseStudent(instanceId, collegeId);
                break;
            case "CollegewiseRegistration":
                report = new CollegeWiseRegistration(instanceId, collegeId);
                break;
            case "RegistrationStudentSummary":
                report = new Reports.Registration.StudentSummaryRpt(instanceId);
                break;
            case ReportName.CategoryClassStudentsRpt:
                report = new CategoryClassStudentsRpt(instanceId, collegeId);
                break;
            case ReportName.CategoryStudentsRpt:
                report = new CategoryStudentsRpt(instanceId, collegeId);
                break;
            case ReportName.CountryClassStudentsRpt:
                report = new CountryClassStudentsRpt(instanceId, collegeId);
                break;
            case ReportName.CountryStateClassStudentsRpt:
                report = new CountryStateClassStudentsRpt(instanceId, collegeId);
                break;
            case ReportName.CountryStudentsRpt:
                report = new CountryStudentsRpt(instanceId, collegeId);
                break;
            case ReportName.MinorityClassStudentsRpt:
                report = new MinorityClassStudentsRpt(instanceId, collegeId);
                break;
            case ReportName.MinorityStudentsRpt:
                report = new MinorityStudentsRpt(instanceId, collegeId);
                break;
            case ReportName.OtherStateStudentsRpt:
                report = new OtherStateStudentsRpt(instanceId, collegeId);
                break;
            case ReportName.StateClassStudentsRpt:
                report = new StateClassStudentsRpt(instanceId, collegeId);
                break;
            case ReportName.StudentRollListRpt:
                report = new StudentRollListRpt(instanceId, collegeId);
                break;

            // Revaluation
            case "RevaluationRpt":
                report = new RevaluationRpt(instanceId, collegeId);
                break;
            case "RevaluationChallanRpt":
                {
                    var prnNo = string.Empty;
                    if (null != HttpContext.Current.Session[ModelConstants.PrnNo])
                        prnNo = HttpContext.Current.Session[ModelConstants.PrnNo].ToString();
                    report = new RevaluationChallanRpt(instanceId, prnNo);
                }
                break;

            //Exam
            case ReportName.ExamNameListRpt:
                report = new ExamNameListRpt(instanceId);
                break;
            case ReportName.SubjectSummaryRpt:
                report = new SubjectSummaryRpt(instanceId);
                break;
            case ReportName.SubjectSummary1Rpt:
                report = new SubjectSummary1Rpt(instanceId);
                break;
            case ReportName.ExamChallanRpt:
                {
                    var prnNo = string.Empty;
                    //if (null != HttpContext.Current.Session[ModelConstants.PrnNo])
                    if (null != sessionData && !string.IsNullOrEmpty(sessionData.Prn))
                        prnNo = sessionData.Prn;// HttpContext.Current.Session[ModelConstants.PrnNo].ToString();
                    report = new ExamChallanRpt(instanceId, prnNo);
                }
                break;
            case ReportName.ExamFeeSummaryRpt:
                report = new ExamFeeSummaryRpt(instanceId, collegeId);
                break;
            case ReportName.ExamStudentSummary:
                report = new Reports.Exam.StudentSummaryRpt(instanceId);
                break;
            case ReportName.ExamReceiptRpt:
                report = new Reports.Exam.ReceiptRpt(instanceId, sessionData?.Prn);
                break;

            // Convocation
            case "ConvocationRpt":
                report = new ConvocationRpt(instanceId);
                break;
            case "ConvocationChallanRpt":
                {
                    var prnNo = string.Empty;
                    if (null != HttpContext.Current.Session[ModelConstants.PrnNo])
                        prnNo = HttpContext.Current.Session[ModelConstants.PrnNo].ToString();
                    report = new ConvocationChallanRpt(instanceId, prnNo);
                }
                break;
            case "ConvocationFeeSummaryRpt":
                report = new ConvocationFeeSummaryRpt(instanceId, collegeId);
                break;
            case "ConvocationStudentSummary":
                report = new Reports.Convocation.StudentSummaryRpt(instanceId);
                break;
            case ReportName.ConvocationStudentsRpt:
                report = new ConvocationStudentsRpt();
                break;
            case "ConvocationStudentResults":
                report = new Reports.Convocation.StudentResultRpt();
                break;

            // Environment Studies
            case "EnvNameListRpt":
                report = new EnvNameListRpt(instanceId, collegeId);
                break;
            case "EnvironmentCAPRpt":
                report = new EnvironmentCapRpt(instanceId, collegeId);
                break;
            case "EnvironmentChallanRpt":
                {
                    var prnNo = string.Empty;
                    if (null != sessionData && !string.IsNullOrEmpty(sessionData.Prn))
                        prnNo = sessionData.Prn;// HttpContext.Current.Session[ModelConstants.PrnNo].ToString();
                    report = new EnvironmentChallanRpt(instanceId, prnNo);
                }
                break;
            case "EnvironmentFeeSummaryRpt":
                report = new EnvironmentFeeSummaryRpt(instanceId, collegeId);
                break;
            case "EnvironmentStudentSummary":
                report = new Reports.Environment.StudentSummaryRpt(instanceId);
                break;
            case "EnvironmentStudentResults":
                report = new Reports.Environment.StudentResultRpt();
                break;

            // Time Table
            case ReportName.TimeTableTheoryRpt:
                report = Bootstrapper.GetReport(typeof(TimeTableTheoryRpt));
                break;
            case ReportName.TimeTablePracticalRpt:
                report = Bootstrapper.GetReport(typeof(TimeTablePracticalRpt));
                break;
            case ReportName.TimeTableCoursePartRpt:
                report = Bootstrapper.GetReport(typeof(TimeTableCoursePartRpt));
                break;
            case ReportName.TimeTableStartDatesRpt:
                report = Bootstrapper.GetReport(typeof(TimeTableStartDatesRpt));
                break;
            case ReportName.Schedule1Rpt:
                report = new ScheduleExaminationRpt(instanceId, collegeId);
                break;
            case ReportName.Schedule2Rpt:
                report = new ScheduleCAPRpt(instanceId);
                break;
            case ReportName.Schedule3Rpt:
                report = new ScheduleExaminationMinRpt(instanceId, collegeId);
                break;
            case ReportName.ClashesRpt:
                report = Bootstrapper.GetReport(typeof(ClashesRpt));
                break;
            case ReportName.ResultDeclarationRpt:
                report = new ResultDeclarationRpt(instanceId, collegeId);
                break;
            case ReportName.CapRecordsRpt:
                report = new CapRecordsRpt(instanceId, collegeId);
                break;
            case ReportName.TimeTableTheoryDateWiseRpt:
                report = Bootstrapper.GetReport(typeof(TimeTableTheoryDateWiseRpt));
                break;

            // Answer Sheet
            case ReportName.AnswerSheetChallanRpt:
                {
                    var prnNo = string.Empty;
                    if (null != HttpContext.Current.Session[ModelConstants.PrnNo])
                        prnNo = HttpContext.Current.Session[ModelConstants.PrnNo].ToString();
                    report = new AnswerSheetChallanRpt(instanceId, prnNo);
                }
                break;
            case ReportName.AnswerSheetRpt:
                report = new AnswerSheetRpt(instanceId, collegeId);
                break;

            // Mobile Update
            case ReportName.MobileUpdateRpt:
                report = new MobileUpdateRpt(instanceId, collegeId);
                break;
            case ReportName.AbcRpt:
                report = new AbcRpt(instanceId, collegeId);
                break;

            // Accounts
            case ReportName.PayoutSummaryRpt:
                report = new PayoutSummaryRpt();
                break;
            case ReportName.PayoutExamRpt:
                report = new PayoutExamRpt();
                break;
            case ReportName.PayoutMissingRpt:
                report = new PayoutMissingRpt();
                break;
        }

        return report;
    }
    #endregion

    #region -- Events --
    [Authorize]
    protected void Page_Load(object sender, EventArgs e)
    {
        if (!(HttpContext.Current.Session[ModelConstants.Report] is Report report))
            report = GetReport();

        if (null == report) return;

        ReportViewer1.ReportSource = report;
        ReportViewer1.ShowPrintButton = true;
        ReportViewer1.RefreshReport();

        Session[ModelConstants.Report] = null;
    }


    #endregion
}