using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Helper;
using Telerik.Reporting;

namespace Corno.Reports.Time_Table;

public partial class TimeTableCoursePartRpt : Report
{
    #region -- Connstructors --
    public TimeTableCoursePartRpt(ICornoService transactionService)
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
    #endregion

    #region -- Events --
    private void TimeTableCoursePartRpt_NeedDataSource(object sender, EventArgs e)
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

        var timeTables = _transactionService.TimeTableRepository.Get(t => t.InstanceId == _instanceId &&
                                                                          (t.CollegeId ?? 0) == collegeId &&
                                                                          courseIds.Contains(t.CourseId ?? 0),
                null, "TimeTableCoursePartDetails")
            .AsQueryable()
            .AsNoTracking()
            .ToList();

        if (timeTables.Count <= 0)
            return;

        var examService = Bootstrapper.Get<ICoreService>();
        var dataSource = new List<object>();
        foreach (var timeTable in timeTables)
        {
            dataSource.AddRange(timeTable.TimeTableCoursePartDetails
                .Where(t => t.StartDate != null)
                .Select(t => new
                {
                    InstanceId = _instanceId,
                    InstanceName = ExamServerHelper.GetInstanceName(_instanceId, examService),
                    CollegeId = collegeId,
                    CollegeName = ExamServerHelper.GetCollegeName(collegeId, examService),
                    CenterId = timeTable.CentreId,
                    CenterName = ExamServerHelper.GetCentreName(timeTable.CentreId, examService),
                    timeTable.CourseId,
                    CourseName = ExamServerHelper.GetCourseName(timeTable.CourseId, examService),

                    t.CoursePartId,
                    CoursePartName = ExamServerHelper.GetCoursePartName(t.CoursePartId, examService),

                    t.SubjectGroup,
                    t.StartDate,
                    t.EndDate,
                    t.StartTime,
                    t.EndTime
                }).ToList());

        }

        report.DataSource = dataSource;
    }
    #endregion
}