using Corno.Data.Helpers;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Helper;
using System;
using System.Linq;
using Telerik.Reporting;

namespace Corno.Reports.Convocation;

public partial class StudentSummaryRpt : Report
{
    #region -- Connstructors --
    public StudentSummaryRpt(int instanceId)
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

    #region -- Events --
    private void SummaryRpt_NeedDataSource(object sender, EventArgs e)
    {
        if (!(sender is Telerik.Reporting.Processing.Report report)) return;

        var collegeId = null != report.Parameters[ModelConstants.College].Value ?
            Convert.ToInt32(((object[])report.Parameters[ModelConstants.College].Value)[0]) : 0;
        var convocationNo = report.Parameters[ModelConstants.ConvocationNo].Value.ToInt();

        var coreService = Bootstrapper.Get<ICoreService>();
        if (null == coreService) return;

        var instanceName = coreService.Tbl_SYS_INST_Repository.FirstOrDefault(p => p.Num_PK_INST_SRNO == _instanceId, p => p.Var_INST_REM);
        var collegeName = ExamServerHelper.GetCollegeName(collegeId, coreService);

        var convocations = (from convocation in coreService.Tbl_STUDENT_CONVO_Repository.Get(r => 
                r.Num_FK_CONVO_NO == convocationNo && r.Num_FK_COLLEGE_CD == collegeId)
            join studentInfo in coreService.Tbl_STUDENT_INFO_Repository.Get()
                on convocation.Chr_FK_PRN_NO equals studentInfo.Chr_PK_PRN_NO
            select new
            {
                convocation.Num_FK_CO_CD,
                studentInfo.Chr_ST_SEX_CD
            }).ToList();

        var groups = convocations.GroupBy(g => g.Num_FK_CO_CD).ToList();

        var courseIds = convocations.Select(c => c.Num_FK_CO_CD).Distinct().ToList();
        var courses = coreService.Tbl_COURSE_MSTR_Repository.Get(c => courseIds.Contains(c.Num_PK_CO_CD)).ToList();

        var dataSource = groups
            .Select(g =>
            {
                var course = courses.FirstOrDefault(c => c.Num_PK_CO_CD == g.Key);
                return new
                {
                    InstanceName = instanceName,
                    CollegeId = collegeId,
                    CollegeName = collegeName,
                    CourseId = course?.Num_PK_CO_CD,
                    CourseName = course?.Var_CO_NM,

                    M = g.Count(x => x.Chr_ST_SEX_CD == "M"),
                    F = g.Count(x => x.Chr_ST_SEX_CD == "F"),
                    Total = g.Count()
                };
            });

        report.DataSource = dataSource;
    }
    #endregion
}