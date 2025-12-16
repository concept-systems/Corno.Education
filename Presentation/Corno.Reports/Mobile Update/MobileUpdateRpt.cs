using System;
using System.Linq;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Logger;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Helper;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Report = Telerik.Reporting.Report;

namespace Corno.Reports.Mobile_Update;

public partial class MobileUpdateRpt : Report
{
    #region -- Constructors --
    public MobileUpdateRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    public MobileUpdateRpt(int instanceId, int collegeId)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        sdsCollege.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        if (collegeId > 0)
            sdsCollege.SelectCommand += " And Num_PK_COLLEGE_CD = " + collegeId;
        sdsCourse.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCoursePart.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsBranch.ConnectionString = GlobalVariables.ConnectionStringExamServer;

        sdsMain.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsMain.SelectCommand = sdsMain.SelectCommand.Replace("@InstanceID",
            instanceId.ToString());

        _instanceId = instanceId;
    }
    #endregion

    #region -- Data Members --

    private readonly int _instanceId;
    #endregion

    #region -- Methods --
    public void HandleReport(object sender, int instanceId)
    {
        if (!(sender is Telerik.Reporting.Processing.Report report)) return;

        var collegeId = null != report.Parameters[ModelConstants.College].Value ?
            Convert.ToInt32(((object[])report.Parameters[ModelConstants.College].Value)[0]) : 0;
        var courseId = null != report.Parameters[ModelConstants.Course].Value ?
            Convert.ToInt32(((object[])report.Parameters[ModelConstants.Course].Value)[0]) : 0;
        var coursePartId = null != report.Parameters[ModelConstants.CoursePart].Value ?
            Convert.ToInt32(((object[])report.Parameters[ModelConstants.CoursePart].Value)[0]) : 0;
           
        //var courseIds = new List<int>();
        //var courseParameters = (object[])report.Parameters[ModelConstants.CoursePart]?.Value;
        //if (null != courseParameters)
        //{
        //    foreach (var courseId in courseParameters)
        //        courseIds.Add(Convert.ToInt32(courseId));
        //}

        LogHandler.LogInfo("InstanceId : " + instanceId +
                           ", CollegeId : " + collegeId + ", CoursePartId : " + coursePartId);

        var examService = Bootstrapper.Get<ICoreService>();
        var collegeName = ExamServerHelper.GetCollegeName(collegeId, examService);
        var courseName = ExamServerHelper.GetCourseName(courseId, examService);
        var coursePartName = ExamServerHelper.GetCoursePartName(coursePartId, examService);

        var appTemps = examService.Tbl_APP_TEMP_Repository.Get(y =>
                y.Num_FK_INST_NO == instanceId &&
                y.Num_FK_COLLEGE_CD == collegeId && y.Num_FK_COPRT_NO == coursePartId)
            .ToList();
        var distinctPrn = appTemps.Distinct().Select(d => d.Chr_APP_PRN_NO);
        var studentInfos = examService.Tbl_STUDENT_INFO_Repository.Get(s =>
            distinctPrn.Contains(s.Chr_PK_PRN_NO));
        var studentInfoAddrs = examService.Tbl_STUDENT_INFO_ADR_Repository.Get(s =>
            distinctPrn.Contains(s.Chr_FK_PRN_NO));

        var yrChanges = examService.Tbl_STUDENT_YR_CHNG_Repository.Get(y =>
                y.Num_FK_INST_NO == instanceId &&
                y.Num_FK_COL_CD == collegeId && y.Num_FK_COPRT_NO == coursePartId)
            .ToList();

        var dataSource = from appTemp in appTemps
            join studentInfo in studentInfos
                on appTemp.Chr_APP_PRN_NO equals studentInfo.Chr_PK_PRN_NO
            join studentInfoAddress in studentInfoAddrs
                on appTemp.Chr_APP_PRN_NO equals studentInfoAddress.Chr_FK_PRN_NO
            select new
            {
                Prn = appTemp.Chr_APP_PRN_NO,
                StudentName =  studentInfo.Var_ST_NM,
                SeatNo = yrChanges.FirstOrDefault(c => 
                    c.Chr_FK_PRN_NO == appTemp.Chr_APP_PRN_NO)?.Num_ST_SEAT_NO,
                Email = studentInfoAddress.Chr_Student_Email,
                Mobile = studentInfoAddress.Num_MOBILE,
                Photo = studentInfoAddress.Ima_ST_PHOTO,

                CollegeId = collegeId,
                CollegeName = collegeName,
                CourseId = courseId,
                CourseName = courseName,
                CoursePartId = coursePartId,
                CoursePartName = coursePartName,
                BranchId = appTemp.Num_FK_BR_CD,
                BranchName = ExamServerHelper.GetBranchName(appTemp.Num_FK_BR_CD, examService)
            };

        report.DataSource = dataSource;

        LogHandler.LogInfo("Record Count : " + dataSource.Count());
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
        HandleReport(sender, _instanceId);
    }
    #endregion
}