using System;
using System.Linq;
using Corno.Data.Core;
using Corno.Data.Helpers;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Reports.Base;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Helper;
using Telerik.Reporting;
using Telerik.Reporting.Processing;

namespace Corno.Reports.Convocation;

public partial class StudentResultRpt : BaseReport
{
    #region -- Constructors --
    public StudentResultRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        sdsCollege.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCourse.ConnectionString = GlobalVariables.ConnectionStringExamServer;
    }
    #endregion

    #region -- Methods --

    private static string GetResult(Tbl_STUDENT_COURSE studentCourse, Tbl_STUDENT_CONVO convocation)
    {
        if (null == convocation)
            return "Not Applied";
        if (convocation.Num_ST_CONVO_NO > 0)
            return "Received";
        if (convocation.Num_FK_CONVO_NO?.ToInt() > 0 && convocation.Num_ST_CONVO_NO?.ToInt() <= 0)
            return "Applied";
        return string.Empty;
    }

    private void HandleReport(object sender)
    {
        if (sender is not Telerik.Reporting.Processing.Report report) return;

        var collegeId = null != report.Parameters[ModelConstants.College].Value ?
            Convert.ToInt32(((object[])report.Parameters[ModelConstants.College].Value)[0]) : 0;
        var courseId = null != report.Parameters[ModelConstants.Course].Value ?
            Convert.ToInt32(((object[])report.Parameters[ModelConstants.Course].Value)[0]) : 0;
        var registrationYear = report.Parameters["RegistrationYear"].Value?.ToString();

        var examService = Bootstrapper.Get<ICoreService>();
        var collegeName = ExamServerHelper.GetCollegeName(collegeId, examService);
        var courseName = ExamServerHelper.GetCourseName(courseId, examService);

        var studentCourses = examService.Tbl_STUDENT_COURSE_Repository
            .Get(s => s.Num_ST_COLLEGE_CD == collegeId && s.Num_FK_CO_CD == courseId &&
                      s.Chr_ST_REG_YEAR == registrationYear).ToList();
        var branchIds = studentCourses.Select(s => s.Num_FK_BR_CD).Distinct();
        var branches = examService.Tbl_BRANCH_MSTR_Repository.Get(b => branchIds.Contains(b.Num_PK_BR_CD)).ToList();

        var prnList = studentCourses.Select(s => s.Chr_FK_PRN_NO).Distinct().ToList();
        var studentInfoAddresses = examService.Tbl_STUDENT_INFO_ADR_Repository.Get(s => prnList.Contains(s.Chr_FK_PRN_NO)).ToList();
        var studentInfoList = examService.Tbl_STUDENT_INFO_Repository.Get(s => prnList.Contains(s.Chr_PK_PRN_NO)).ToList();
        var convoMasters = examService.Tbl_CONVO_MSTR_Repository.Get().ToList();
        var convocations = examService.Tbl_STUDENT_CONVO_Repository.Get(s => prnList.Contains(s.Chr_FK_PRN_NO)).ToList();
        var dataSource = (from studentCourse in studentCourses
                          join studentInfoAddress in studentInfoAddresses on studentCourse.Chr_FK_PRN_NO equals studentInfoAddress.Chr_FK_PRN_NO
                          join studentInfo in studentInfoList on studentCourse.Chr_FK_PRN_NO equals studentInfo.Chr_PK_PRN_NO
                          join branch in branches on (studentCourse.Num_FK_BR_CD ?? 0) equals (branch.Num_PK_BR_CD) into defaultBranch
                          from branch in defaultBranch.DefaultIfEmpty()
                          join convocation in convocations
                              on studentCourse.Chr_FK_PRN_NO equals convocation.Chr_FK_PRN_NO into defaultConvocation
                          from convocation in defaultConvocation.DefaultIfEmpty()
                          /*join convoMaster in convoMasters
                              on convocation?.Num_FK_CONVO_NO equals convoMaster.Num_PK_CONVO_NO into defaultConvoMaster
                          from convoMaster in defaultConvoMaster.DefaultIfEmpty()*/
                          select new
                          {
                              CollegeId = collegeId,
                              CollegeName = collegeName,
                              CourseId = courseId,
                              CourseName = courseName,
                              Prn = studentCourse.Chr_FK_PRN_NO,
                              StudentName = studentInfo.Var_ST_NM,
                              Email = studentInfoAddress.Chr_Student_Email,
                              Mobile = studentInfoAddress.Num_MOBILE,
                              BranchId = studentCourse.Num_FK_BR_CD,
                              BranchName = branch?.Var_BR_SHRT_NM,
                              Status = GetResult(studentCourse, convocation),
                              ConvocationNo = convocation?.Num_FK_CONVO_NO,
                              StudentConvocationNo = convocation?.Num_ST_CONVO_NO,
                              YearOfIssue = convoMasters.FirstOrDefault(p => p.Num_PK_CONVO_NO == convocation?.Num_FK_CONVO_NO)?.Chr_CONVO_YEAR
                          }).ToList();

        if (dataSource.Count > 0)
            report.DataSource = dataSource;
        //LogHandler.LogInfo("Record Count : " + dataSource.Count());
    }
    #endregion

    #region -- Events --
    // Don't Delete. This event is specifically for picture box null event.
    private void MobileUpdateRpt_Error(object sender, ErrorEventArgs eventArgs)
    {
        var procEl = (ProcessingElement)sender;
        procEl.Exception = null;
    }

    private void MobileUpdateRpt_NeedDataSource(object sender, EventArgs e)
    {
        HandleReport(sender);
    }
    #endregion
}