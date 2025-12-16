using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Corno.Data.Dtos.Report;
using Corno.Data.Helpers;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Helper;
using MoreLinq;
using Telerik.Reporting;

namespace Corno.Reports.Time_Table;

public partial class TimeTableTheoryRpt : Report
{
    #region -- Connstructors --
    public TimeTableTheoryRpt(ICornoService transactionService)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        sdsCollege.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCourse.ConnectionString = GlobalVariables.ConnectionStringExamServer;

        _transactionService = transactionService;
        _instanceId = (int)HttpContext.Current.Session[ModelConstants.InstanceId];
    }

    #endregion

    #region -- Data Members --
    private readonly ICornoService _transactionService;
    private readonly int _instanceId;
    private void TimeTableTheoryRpt_NeedDataSource(object sender, EventArgs e)
    {
        if (!(sender is Telerik.Reporting.Processing.Report report)) return;

        var collegeId = null != report.Parameters[ModelConstants.College].Value ?
            Convert.ToInt32(((object[])report.Parameters[ModelConstants.College].Value)[0]) : 0;
        var courseIds = new List<int>();
        var courseParameters = (object[])report.Parameters[ModelConstants.Course]?.Value;
        if (null != courseParameters)
            courseIds.AddRange(courseParameters.Select(courseId => courseId.ToInt()));

        var theoryCategories = ExamServerHelper.GetTheoryCategories();
        var query = _transactionService.TimeTableRepository
            .Get(t => t.InstanceId == _instanceId && courseIds.Contains(t.CourseId ?? 0),
                null, "TimeTableTheoryDetails");

        if (!query.Any())
            return;

        var coreService = Bootstrapper.Get<ICoreService>();
        var timeTables = (from timeTable in query
                from timeTableDetail in timeTable.TimeTableTheoryDetails
                where theoryCategories.Contains(timeTableDetail.CategoryCode ?? 0) && timeTableDetail.StartDate != null
                select new TimeTableTheoryReportDto
                {
                    InstanceId = _instanceId,
                    CollegeId = collegeId,
                    CenterId = timeTable.CentreId,
                    CourseId = timeTable.CourseId,
                    CoursePartId = timeTable.CoursePartId,
                    BranchId = timeTable.BranchId,
                    SubjectId = timeTableDetail.SubjectId,
                    SubjectType = timeTableDetail.SubjectType,
                    StartDate = timeTableDetail.StartDate,
                    StartTime = timeTableDetail.StartTime,
                    EndTime = timeTableDetail.EndTime,
                    SubjectDivisionCode = timeTableDetail.SubjectDivisionCode
                }).AsEnumerable()
            .DistinctBy(p => new
            {
                p.InstanceId, p.CollegeId, p.CourseId,
                p.CoursePartId, p.BranchId, p.SubjectId, p.SubjectDivisionCode
            })
            .ToList();

        var dataSource = from timeTable in timeTables
                         join college in coreService.TBL_COLLEGE_MSTRRepository.Get()
                             on timeTable.CollegeId equals college.Num_PK_COLLEGE_CD into defaultCollege
                         from college in defaultCollege.DefaultIfEmpty()
                         join center in coreService.TBL_DISTANCE_CENTERS_Repository.Get()
                             on timeTable.CenterId equals center.Num_PK_DistCenter_ID into defaultCenter
                         from center in defaultCenter.DefaultIfEmpty()
                         join coursePart in coreService.Tbl_COURSE_PART_MSTR_Repository.Get()
                             on timeTable.CoursePartId equals coursePart.Num_PK_COPRT_NO into defaultCoursePart
                         from coursePart in defaultCoursePart
                         join course in coreService.Tbl_COURSE_MSTR_Repository.Get()
                             on coursePart.Num_FK_CO_CD equals course.Num_PK_CO_CD into defaultCourse
                         from course in defaultCourse.DefaultIfEmpty()
                         join branch in coreService.Tbl_BRANCH_MSTR_Repository.Get()
                             on timeTable.BranchId equals branch.Num_PK_BR_CD into defaultBranch
                         from branch in defaultBranch.DefaultIfEmpty()
                         join subject in coreService.Tbl_SUBJECT_MSTR_Repository.Get()
                             on timeTable.SubjectId equals subject.Num_PK_SUB_CD into defaultSubject
                         from subject in defaultSubject.DefaultIfEmpty()
                         join instance in coreService.Tbl_SYS_INST_Repository.Get()
                             on timeTable.InstanceId equals instance.Num_PK_INST_SRNO into defaultInstance
                         from instance in defaultInstance.DefaultIfEmpty()
                         select new TimeTableTheoryReportDto
                         {
                             InstanceId = _instanceId,
                             CollegeId = collegeId,
                             CenterId = timeTable.CenterId,
                             CourseId = timeTable.CourseId,
                             CoursePartId = timeTable.CoursePartId,
                             BranchId = timeTable.BranchId,
                             SubjectId = timeTable.SubjectId,
                             SubjectType = timeTable.SubjectType,
                             StartDate = timeTable.StartDate,
                             StartTime = timeTable.StartTime,
                             EndTime = timeTable.EndTime,
                             SubjectDivisionCode = timeTable.SubjectDivisionCode,

                             InstanceName = instance?.Var_INST_REM,
                             CollegeName = college?.Var_CL_COLLEGE_NM1,
                             CenterName = center?.DIST_CENT_NAME,
                             CourseName = course?.Var_CO_NM,
                             CoursePartName = coursePart?.Var_COPRT_DESC,
                             BranchName = branch?.Var_BR_NM,
                             SubjectName = subject?.Var_SUBJECT_NM,
                             CommonType = subject?.Var_CommonSubject,
                         };

        report.DataSource = dataSource.ToList();
    }

    /*private void TimeTableTheoryRpt_NeedDataSource(object sender, EventArgs e)
    {
        if (!(sender is Telerik.Reporting.Processing.Report report)) return;

        var collegeId = null != report.Parameters[ModelConstants.College].Value ?
            Convert.ToInt32(((object[])report.Parameters[ModelConstants.College].Value)[0]) : 0;
        //var courseId = null != report.Parameters[ModelConstants.Course].Value ?
        //    Convert.ToInt32(((object[])report.Parameters[ModelConstants.Course].Value)[0]) : 0;
        var courseIds = new List<int>();
        var courseParameters = (object[])report.Parameters[ModelConstants.Course]?.Value;
        if (null != courseParameters)
        {
            foreach (var courseId in courseParameters)
                courseIds.Add(Convert.ToInt32(courseId));
        }

        var theoryCategories = ExamServerHelper.GetTheoryCategories();
        var timeTables = _transactionService.TimeTableRepository.Get(t => t.InstanceId == _instanceId &&
                                                                          //(t.CollegeId ?? 0) == collegeId &&
                                                                          courseIds.Contains(t.CourseId ?? 0),
                null, "TimeTableTheoryDetails")
            .AsQueryable()
            .AsNoTracking()
            .ToList();


        if (timeTables.Count <= 0)
            return;

        var examService = Bootstrapper.Get<ICoreService>();
        var dataSource = new List<object>();
        foreach (var timeTable in timeTables)
        {
            dataSource.AddRange(timeTable.TimeTableTheoryDetails
                .Where(t => theoryCategories.Contains(t.CategoryCode ?? 0) && t.StartDate != null).Select(t =>
                    new
                    {
                        InstanceId = _instanceId,
                        InstanceName = ExamServerHelper.GetInstanceName(_instanceId, examService),
                        CollegeId = collegeId,
                        CollegeName = ExamServerHelper.GetCollegeName(collegeId, examService),
                        CenterId = timeTable.CentreId,
                        CenterName = ExamServerHelper.GetCentreName(timeTable.CentreId, examService),
                        timeTable.CourseId,
                        CourseName = ExamServerHelper.GetCourseNameFromCoursePartId(timeTable.CoursePartId, examService),
                        timeTable.CoursePartId,
                        CoursePartName = ExamServerHelper.GetCoursePartName(timeTable.CoursePartId, examService),
                        timeTable.BranchId,
                        BranchName = ExamServerHelper.GetBranchName(timeTable.BranchId, examService),

                        t.SubjectId,
                        SubjectName = ExamServerHelper.GetSubjectName(t.SubjectId, examService),
                        CommonType = ExamServerHelper.SubjectCommonType(t.SubjectId, examService),
                        t.SubjectType,
                        t.StartDate,
                        t.StartTime,
                        t.EndTime,
                        t.SubjectDivisionCode
                    }).ToList());

        }

        report.DataSource = dataSource;
    }*/
    #endregion
}

// DTO for TimeTableTheory report
public class TimeTableTheoryReportDto : BaseReportDto
{
    public string CommonType { get; set; }
    public string SubjectType { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? SubjectDivisionCode { get; set; }
}
