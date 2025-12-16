using Corno.Data.Core;
using Corno.Data.Corno;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Helper;
using Mapster;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Reporting;

namespace Corno.Reports.Time_Table;

public partial class ScheduleExaminationMinRpt : Report
{
    #region -- Connstructors --
    public ScheduleExaminationMinRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    public ScheduleExaminationMinRpt(int instanceId, int collegeId)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        sdsCollege.ConnectionString = GlobalVariables.ConnectionStringExamServer;

        _instanceId = instanceId;
    }
    #endregion

    #region -- Data Members --

    private readonly int _instanceId;
    #endregion

    #region -- Methods --
    public static void HandleReport(object sender, int instanceId)
    {
        if (sender is not Telerik.Reporting.Processing.Report report) return;

        var collegeId = null != report.Parameters[ModelConstants.College].Value ?
            Convert.ToInt32(((object[])report.Parameters[ModelConstants.College].Value)[0]) : 0;

        var examService = Bootstrapper.Get<ICoreService>();
        var courseIds = examService.Tbl_COLLEGE_COURSE_MSTRRepository.Get(t =>
            t.NUM_FK_COLLEGE_CD == collegeId).Select(d => d.NUM_FK_CO_CD).Distinct()
            .ToList();
        var timeTables = examService.Tbl_EXAM_SCHEDULE_MSTR_Repository.Get(t =>
                t.Num_FK_INST_NO == instanceId && courseIds.Contains(t.Num_FK_COURSE_NO))
            .ToList();

        if (timeTables.Count <= 0)
            return;
        var groups = timeTables.GroupBy(t => t.Num_FK_COURSE_NO).ToList();
        var dataSource = new List<TimeTableViewModel>();
        // Update to database in Schedule course 
        var timeTableCourseList = new List<Tbl_EXAM_SCHEDULE_COURSE>();
        foreach (var group in groups)
        {
            var timeTable = group.ToList().OrderBy(x => x.Dtm_Commencement_DateOfExam).FirstOrDefault();
            if (null == timeTable) return;
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
                /*CoursePartId = timeTable.Num_FK_COPRT_NO,
                CoursePartName = ExamServerHelper.GetCoursePartName(timeTable.Num_FK_COPRT_NO, examService),
                BranchId = timeTable.Num_FK_BR_NO,
                BranchName = ExamServerHelper.GetBranchShortName(timeTable.Num_FK_BR_NO, examService),*/

                ExamStartDate = timeTable.Dtm_Commencement_DateOfExam,// group.Min(g => g.Dtm_Commencement_DateOfExam),
                ExamEndDate = timeTable.Dtm_ConclusionofExam_Date, //group.Min(g => g.Dtm_ConclusionofExam_Date),

                ExamFee = feeStructure.ExamFee,
                StatementMarksAndCapFee = feeStructure.StatementOfMarksFee + feeStructure.CapFee,
            };

            dataSource.Add(record);

            if (dataSource.Count <= 0)
                return;

            var timeTableCourse = new Tbl_EXAM_SCHEDULE_COURSE();
            timeTable.Adapt(timeTableCourse);
            timeTableCourseList.Add(timeTableCourse);
        }

        
        /*foreach (var timeTable in timeTables)
        {
            var existing = examService.Tbl_EXAM_SCHEDULE_COURSE_Repository.Get(t =>
                    t.Num_FK_INST_NO == instanceId && t.Num_FK_COURSE_NO == timeTable.Num_FK_COURSE_NO)
                .FirstOrDefault();
            if (null != existing)
                continue;
            var timeTableCourse = new Tbl_EXAM_SCHEDULE_COURSE();
            timeTable.Adapt(timeTableCourse);
            //examService.Tbl_EXAM_SCHEDULE_COURSE_Repository.Add(timeTableCourse);
            timeTableCourseList.Add(timeTableCourse);
        }*/

        if (timeTableCourseList.Count > 0)
        {
            var timeTableCourseIds = timeTableCourseList.Select(t => t.Num_FK_COURSE_NO)
                .Distinct().ToList();
            var existingList = examService.Tbl_EXAM_SCHEDULE_COURSE_Repository.Get(t =>
                    t.Num_FK_INST_NO == instanceId && timeTableCourseIds.Contains(t.Num_FK_COURSE_NO)).ToList();
            var resultList = timeTableCourseList
                .Where(item1 => !existingList.Any(item2 => 
                    item2.Num_FK_INST_NO == item1.Num_FK_INST_NO && item2.Num_FK_COURSE_NO == item1.Num_FK_COURSE_NO))
                .ToList();
            //timeTableCourseList = timeTableCourseList.Except(existingList).ToList();
            if (resultList.Count > 0)
            {
                examService.Tbl_EXAM_SCHEDULE_COURSE_Repository.AddRange(resultList);
                examService.Save();
            }
        }

        report.DataSource = dataSource;
    }
    #endregion

    #region -- Events --
    private void Schedule1Rpt_NeedDataSource(object sender, EventArgs e)
    {
        HandleReport(sender, _instanceId);
    }
    #endregion
}