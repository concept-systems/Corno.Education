using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Helper;
using MoreLinq;
using Telerik.Reporting;

namespace Corno.Reports.Time_Table;

public partial class ClashesRpt : Report
{
    #region -- Connstructors --
    public ClashesRpt(ICornoService transactionService)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        sdsCollege.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCourse.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCoursePart.ConnectionString = GlobalVariables.ConnectionStringExamServer;

        _transactionService = transactionService;
        _instanceId = (int)HttpContext.Current.Session[ModelConstants.InstanceId];
    }

    #endregion

    #region -- Data Members --
    private readonly ICornoService _transactionService;
    private readonly int _instanceId;
    #endregion

    #region -- Events --
    private void ClashesRpt_NeedDataSource(object sender, EventArgs e)
    {
        var report = sender as Telerik.Reporting.Processing.Report;

        if (null == report) return;

        var collegeId = null != report.Parameters[ModelConstants.College].Value ?
            Convert.ToInt32(((object[])report.Parameters[ModelConstants.College].Value)[0]) : 0;
        var courseId = null != report.Parameters[ModelConstants.Course].Value ?
            Convert.ToInt32(((object[])report.Parameters[ModelConstants.Course].Value)[0]) : 0;
        var fromCoursePartId = null != report.Parameters[ModelConstants.FromCoursePart].Value ?
            Convert.ToInt32(report.Parameters[ModelConstants.FromCoursePart].Value) : 0;
        var toCoursePartId = null != report.Parameters[ModelConstants.ToCoursePart].Value ?
            Convert.ToInt32(report.Parameters[ModelConstants.ToCoursePart].Value) : 0;
        //var coursePartIds = new List<int>();
        //var coursePartParameters = (object[])report.Parameters[ModelConstants.CoursePart]?.Value;
        //if (null != coursePartParameters)
        //{
        //    foreach (var coursePartId in coursePartParameters)
        //        coursePartIds.Add(Convert.ToInt32(coursePartId));
        //}

        var theoryCategories = ExamServerHelper.GetTheoryCategories();
        var timeTables = _transactionService.TimeTableRepository.Get(t => t.InstanceId == _instanceId &&
                                                                          (t.CollegeId ?? 0) == collegeId &&
                                                                          (t.CourseId ?? 0) == courseId &&
                                                                          (t.CoursePartId ?? 0) >= fromCoursePartId &&
                                                                          (t.CoursePartId ?? 0) <= toCoursePartId,
                null, "TimeTableTheoryDetails")
            .ToList();


        if (timeTables.Count <= 0)
            return;

        var examService = Bootstrapper.Get<ICoreService>();
        var dataSource = new List<object>();
        foreach (var timeTable in timeTables)
        {
            dataSource.AddRange(timeTable.TimeTableTheoryDetails
                .Where(t => theoryCategories.Contains(t.CategoryCode ?? 0) && t.StartDate != null &&
                            t.StartTime != null)
                .Select(t => new
                {
                    InstanceId = _instanceId,
                    InstanceName = ExamServerHelper.GetInstanceName(_instanceId, examService),
                    CollegeId = collegeId,
                    CollegeName = ExamServerHelper.GetCollegeName(collegeId, examService),
                    CenterId = timeTable.CentreId,
                    CenterName = ExamServerHelper.GetCentreName(timeTable.CentreId, examService),
                    CourseId = courseId,
                    CourseName = ExamServerHelper.GetCourseNameFromCoursePartId(timeTable.CoursePartId, examService),
                    timeTable.CoursePartId,
                    CoursePartName = ExamServerHelper.GetCoursePartName(timeTable.CoursePartId, examService),
                    timeTable.BranchId,
                    BranchName = ExamServerHelper.GetBranchName(timeTable.BranchId, examService),

                    t.SubjectId,
                    SubjectName = ExamServerHelper.GetSubjectName(t.SubjectId, examService),
                    t.SubjectType,
                    t.StartDate,
                    t.StartTime,
                    t.EndTime
                })
                .DistinctBy(d => new { d.SubjectId, d.StartDate, d.StartTime?.Hour })
                .ToList());
        }

        var subjectWiseGroup = dataSource.GroupBy(d => new
            {
                ((dynamic)d).CoursePartId,
                ((dynamic)d).StartDate,
                ((dynamic)d).StartTime.Hour
            })
            .Select(g => g.First())
            .ToList();

        // Now find clashes
        var grouped = subjectWiseGroup.GroupBy(d => new { ((dynamic)d).StartDate, ((dynamic)d).StartTime.Hour })
            .Where(g => g.Count() > 1)
            //.DistinctBy(d => ((dynamic)d).CoursePartId)
            .SelectMany(g => g.DistinctBy(d => ((dynamic)d).CoursePartId))
            .ToList();


        report.DataSource = grouped;
    }
    #endregion
}