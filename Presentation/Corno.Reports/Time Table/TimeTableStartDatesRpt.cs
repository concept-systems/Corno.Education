using System;
using System.Linq;
using System.Web;
using Corno.Data.Corno;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;
using Telerik.Reporting;

namespace Corno.Reports.Time_Table;

public partial class TimeTableStartDatesRpt : Report
{
    #region -- Connstructors --
    public TimeTableStartDatesRpt(ICornoService cornoService)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        sdsCollege.ConnectionString = GlobalVariables.ConnectionStringExamServer;

        _cornoService = cornoService;
        _instanceId = (int)HttpContext.Current.Session[ModelConstants.InstanceId];
    }

    #endregion

    #region -- Data Members --
    private readonly ICornoService _cornoService;
    private readonly int _instanceId;
    #endregion

    #region -- Events --
    private void TimeTableStartDatesRpt_NeedDataSource(object sender, EventArgs e)
    {
        if (sender is not Telerik.Reporting.Processing.Report report) return;

        var collegeId = null != report.Parameters[ModelConstants.College].Value ?
            Convert.ToInt32(((object[])report.Parameters[ModelConstants.College].Value)[0]) : 0;

        var timeTables = _cornoService.TimeTableRepository.Get(t => 
                t.InstanceId == _instanceId && (t.CollegeId ?? 0) == collegeId, p => p, 
                null, $"{nameof(TimeTable.TimeTableTheoryDetails)},{nameof(TimeTable.TimeTablePracticalDetails)}")
            .ToList();

        if (timeTables.Count <= 0)
            return;
        /*timeTables = timeTables.Where(p => p.CollegeId == 7 && p.CourseId == 13010)
            .ToList();*/
        var coreService = Bootstrapper.Get<ICoreService>();
        var dataSource = from timeTable in timeTables
                         join college in coreService.TBL_COLLEGE_MSTRRepository.Get()
                                      on timeTable.CollegeId equals college.Num_PK_COLLEGE_CD into defaultCollege
                         from college in defaultCollege.DefaultIfEmpty()
                         join center in coreService.TBL_DISTANCE_CENTERS_Repository.Get()
                             on timeTable.CentreId equals center.Num_PK_DistCenter_ID into defaultCenter
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
                         join instance in coreService.Tbl_SYS_INST_Repository.Get()
                             on timeTable.InstanceId equals instance.Num_PK_INST_SRNO into defaultInstance
                         from instance in defaultInstance.DefaultIfEmpty()
                         select new
                         {
                             InstanceId = _instanceId,
                             InstanceName = instance.Var_INST_REM,
                             CollegeId = collegeId,
                             CollegeName = college?.Var_CL_COLLEGE_NM1,
                             CenterId = timeTable.CentreId ?? 0,
                             CenterName = center?.DIST_CENT_NAME,
                             timeTable.CourseId,
                             CourseName = $"({timeTable.CourseId}) {course.Var_CO_NM}",

                             timeTable.CoursePartId,
                             CoursePartName = $"{coursePart.Var_COPRT_DESC} {((timeTable.BranchId ?? 0) > 0 ? branch.Var_BR_SHRT_NM : string.Empty)}",

                             ThoeryStartDate = timeTable.TimeTableTheoryDetails
                                 .Where(d => d.StartDate is not null)
                                 .OrderBy(d => d.StartDate)
                                 .FirstOrDefault()?.StartDate,
                             PracticalStartDate = timeTable.TimeTablePracticalDetails
                                 .Where(d => d.StartDate is not null)
                                 .OrderBy(d => d.StartDate)
                                 .FirstOrDefault()?.StartDate,
                         };

        report.DataSource = dataSource.ToList();
    }
    #endregion
}