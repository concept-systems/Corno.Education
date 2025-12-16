using Corno.Data.Helpers;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Helper;
using System;
using System.Linq;
using Telerik.Reporting;

namespace Corno.Reports.Environment;

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
    private void StudentSummaryRpt_NeedDataSource(object sender, EventArgs e)
    {
        if (!(sender is Telerik.Reporting.Processing.Report report)) return;

        var collegeId = null != report.Parameters[ModelConstants.College].Value ?
            Convert.ToInt32(((object[])report.Parameters[ModelConstants.College].Value)[0]) : 0;

        var examService = Bootstrapper.Get<ICoreService>();
        if (null == examService) return;

        var instanceName = examService.Tbl_SYS_INST_Repository.FirstOrDefault(p => p.Num_PK_INST_SRNO == _instanceId, p => p.Var_INST_REM);
        var collegeName = ExamServerHelper.GetCollegeName(collegeId, examService);

        var environmentStudies = (from environmentStudy in examService.TBl_STUDENT_ENV_STUDIES_Repository.Get(r => r.Num_FK_INST_NO == _instanceId &&
                r.Num_FK_COL_CD == collegeId)
            join studentInfo in examService.Tbl_STUDENT_INFO_Repository.Get()
                on environmentStudy.Chr_FK_PRN_NO equals studentInfo.Chr_PK_PRN_NO
            select new
            {
                environmentStudy.Num_FK_COPRT_NO,
                environmentStudy.Num_FK_CO_CD,
                studentInfo.Chr_ST_SEX_CD
            }).ToList();

        var groups = environmentStudies.GroupBy(g => g.Num_FK_COPRT_NO).ToList();

        var coursePartIds = groups.Select(r => (short)r.Key.ToUShort()).Distinct();
        var courseParts = examService.Tbl_COURSE_PART_MSTR_Repository
            .Get(c => coursePartIds.Contains(c.Num_PK_COPRT_NO))
            .ToList();
        var courseIds = environmentStudies.Select(c => c.Num_FK_CO_CD).Distinct().ToList();
        var courses = examService.Tbl_COURSE_MSTR_Repository.Get(c => courseIds.Contains(c.Num_PK_CO_CD)).ToList();

        var dataSource = groups
            .Select(g =>
            {
                var firstRecord = g.FirstOrDefault();
                var coursePart = courseParts.FirstOrDefault(c => c.Num_PK_COPRT_NO == firstRecord?.Num_FK_COPRT_NO.ToUShort());
                var course = courses.FirstOrDefault(c => c.Num_PK_CO_CD == coursePart?.Num_FK_CO_CD);
                return new
                {
                    InstanceName = instanceName,
                    CollegeId = collegeId,
                    CollegeName = collegeName,
                    CourseId = course?.Num_PK_CO_CD,
                    CourseName = course?.Var_CO_NM,
                    CoursePartId = g.Key.ToUShort(),
                    CoursePartName = coursePart?.Var_COPRT_DESC,

                    M = g.Count(x => x.Chr_ST_SEX_CD == "M"),
                    F = g.Count(x => x.Chr_ST_SEX_CD == "F"),
                    Total = g.Count()
                };
            });

        report.DataSource = dataSource;
    }
    #endregion
}