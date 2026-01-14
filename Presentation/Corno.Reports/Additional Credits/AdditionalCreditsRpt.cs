using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Services.Core.Interfaces;
using System;
using System.Linq;
using Telerik.Reporting;

namespace Corno.Reports.Additional_Credits;

public partial class AdditionalCreditsRpt : Report
{
    #region -- Constructors --
    public AdditionalCreditsRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    public AdditionalCreditsRpt(ICoreService coreService)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        sdsCollege.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCourse.ConnectionString = GlobalVariables.ConnectionStringExamServer;

        _coreService = coreService;
    }

    #endregion

    #region -- Data Members --
    private readonly ICoreService _coreService;
    #endregion


    #region -- Events --
    private void AdditionalCreditsRpt_NeedDataSource(object sender, EventArgs e)
    {
        if (!(sender is Telerik.Reporting.Processing.Report report)) return;

        var collegeIds = ((object[])report.Parameters[ModelConstants.College].Value).ToList().Select(s => int.Parse(s.ToString())).ToList();
        var courseIds = ((object[])report.Parameters[ModelConstants.Course].Value).ToList().Select(s => int.Parse(s.ToString())).ToList();

        // Query Tbl_ADDITIONAL_CREDITS with joins to get related data
        var query = from credit in _coreService.Tbl_Additional_Credits_Repository.Get(c =>
                c.DELETE_FLG != "Y", p => p)
                    join college in _coreService.TBL_COLLEGE_MSTRRepository.Get() on credit.Num_FK_COLLEGE_CD equals college.Num_PK_COLLEGE_CD
                    join course in _coreService.Tbl_COURSE_MSTR_Repository.Get() on credit.Num_FK_COURCE_CD equals course.Num_PK_CO_CD
                    join subject in _coreService.Tbl_SUBJECT_MSTR_Repository.Get() on credit.Num_FK_SUB_CD equals subject.Num_PK_SUB_CD
                    join student in _coreService.Tbl_STUDENT_INFO_Repository.Get() on credit.Chr_ADD_PRN_NO equals student.Chr_PK_PRN_NO
                    join branch in _coreService.Tbl_BRANCH_MSTR_Repository.Get() on credit.Num_FK_BR_CD equals branch.Num_PK_BR_CD into branchGroup
                    from branch in branchGroup.DefaultIfEmpty()
                    where (collegeIds.Count == 0 || collegeIds.Contains(credit.Num_FK_COLLEGE_CD)) &&
                          (courseIds.Count == 0 || courseIds.Contains(credit.Num_FK_COURCE_CD))
                    select new
                    {
                        PRN = credit.Chr_ADD_PRN_NO,
                        Gender = student.Chr_ST_SEX_CD ?? string.Empty,
                        CollegeID = credit.Num_FK_COLLEGE_CD,
                        CollegeName = college.Var_CL_COLLEGE_NM1 ?? string.Empty,
                        CourseID = credit.Num_FK_COURCE_CD,
                        CourseName = course.Var_CO_NM ?? string.Empty,
                        BranchID = credit.Num_FK_BR_CD ?? 0,
                        BranchName = branch != null ? branch.Var_BR_NM : string.Empty,
                        SubjectID = credit.Num_FK_SUB_CD,
                        SubjectName = subject.Var_SUBJECT_NM ?? string.Empty,
                        MaxCredits = credit.Num_MAX_CREDITS,
                        CreditsEarned = credit.Chr_IS_COMPLETED == "Y" ? credit.Num_MAX_CREDITS : 0,
                        IsCompleted = credit.Chr_IS_COMPLETED == "Y" ? "Yes" : "No",
                        CompletedDate = credit.Dtm_COMPLETED,
                        UserName = credit.Var_USR_NM ?? string.Empty
                    };

        // Order data for proper grouping: by PRN, then by Subject
        var data = query
            .OrderBy(x => x.PRN)
            .ThenBy(x => x.SubjectID)
            .ToList();

        if (data.Count <= 0)
            return;
        report.DataSource = data;
    }
    #endregion

    #region -- Inner Classes --
    public class AnswerSheetViewModel
    {
        public int InstanceId { get; set; }
        public string InstanceName { get; set; }
        public int CollegeId { get; set; }
        public string CollegeName { get; set; }
        public int CenterId { get; set; }
        public string CenterName { get; set; }
        public int? CourseId { get; set; }
        public string CourseName { get; set; }
        public int? CoursePartId { get; set; }
        public string CoursePartName { get; set; }
        public string BranchName { get; set; }

        public string StudentName { get; set; }
        public string Prn { get; set; }
        public long? SeatNo { get; set; }
        public int? SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public double Fee { get; set; }
    }

    #endregion
}