using System;
using System.Collections.Generic;
using System.Linq;
using Corno.Data.Corno;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Helper;
using Telerik.Reporting;

namespace Corno.Reports.Time_Table;

public partial class ScheduleCAPRpt : Report
{
    #region -- Connstructors --
    public ScheduleCAPRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    public ScheduleCAPRpt(int instanceId)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        sdsCollege.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCourse.ConnectionString = GlobalVariables.ConnectionStringExamServer;

        _instanceId = instanceId;
    }
    #endregion

    #region -- Data Members --

    private readonly int _instanceId;
    #endregion

    #region -- Methods --

    public static string GetDateString(DateTime? date, int days)
    {
        return null == date ? "-" : date.Value.AddDays(days).ToString("dd/MM/yyyy");
    }

    public static void HandleReport(object sender, int instanceId)
    {
        if (!(sender is Telerik.Reporting.Processing.Report report)) return;

        var collegeId = null != report.Parameters[ModelConstants.College].Value ?
            Convert.ToInt32(((object[])report.Parameters[ModelConstants.College].Value)[0]) : 0;
        var courseIds = new List<int>();
        var courseParameters = (object[])report.Parameters[ModelConstants.Course]?.Value;
        if (null != courseParameters)
        {
            foreach (var courseId in courseParameters)
                courseIds.Add(Convert.ToInt32(courseId));
        }

        var examService = Bootstrapper.Get<ICoreService>();
        var timeTables = examService.Tbl_EXAM_SCHEDULE_MSTR_Repository.Get(t => t.Num_FK_INST_NO == instanceId &&
                courseIds.Contains(t.Num_FK_COURSE_NO))
            .ToList();

        if (timeTables.Count <= 0)
            return;
            
        var dataSource = new List<TimeTableViewModel>();
        foreach (var timeTable in timeTables)
        {
            var feeStructure = ExamServerHelper.GetFeeStructureForReport(instanceId, collegeId, timeTable.Num_FK_COURSE_NO, timeTable.Num_FK_COPRT_NO, false, examService);
            var record = new TimeTableViewModel
            {
                InstanceId = instanceId,
                InstanceName = ExamServerHelper.GetInstanceName(instanceId, examService),
                CollegeId = collegeId,
                CollegeName = ExamServerHelper.GetCollegeName(collegeId, examService),
                CourseId = timeTable.Num_FK_COURSE_NO,
                CourseName =
                    ExamServerHelper.GetCourseNameFromCoursePartId(timeTable.Num_FK_COPRT_NO, examService),
                CoursePartId = timeTable.Num_FK_COPRT_NO,
                CoursePartName = ExamServerHelper.GetCoursePartName(timeTable.Num_FK_COPRT_NO, examService),
                BranchId = timeTable.Num_FK_BR_NO,
                BranchName = ExamServerHelper.GetBranchShortName(timeTable.Num_FK_BR_NO, examService),

                ExamStartDate = timeTable.Dtm_Commencement_DateOfExam,
                ExamEndDate = timeTable.Dtm_ConclusionofExam_Date,

                ExamFee = feeStructure.ExamFee,
                StatementMarksAndCapFee = feeStructure.StatementOfMarksFee + feeStructure.CapFee,
                //ExamServerHelper.GetExamFee(instanceId, timeTable.Num_FK_COURSE_NO, timeTable.Num_FK_COPRT_NO, examService)
            };

            dataSource.Add(record);
        }

        if (dataSource.Count <= 0)
            return;

        report.DataSource = dataSource;
    }
    #endregion

    #region -- Events --
    private void ScheduleRpt_NeedDataSource(object sender, EventArgs e)
    {
        HandleReport(sender, _instanceId);
    }
    #endregion
}