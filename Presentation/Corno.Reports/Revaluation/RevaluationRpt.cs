using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Services.Bootstrapper;
using Corno.Services.Core;
using Corno.Services.Core.Interfaces;
using Corno.Services.Helper;
using System;
using System.Linq;
using Telerik.Reporting;

namespace Corno.Reports.Revaluation;

// Summary description for CollegewiseRegistration.
public partial class RevaluationRpt : Report
{
    public RevaluationRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    #region -- Data Members --

    private readonly short _instanceId;
    #endregion

    public RevaluationRpt(int instanceId, int collegeId)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        sdsCollege.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        if (collegeId > 0)
            sdsCollege.SelectCommand += " And Num_PK_COLLEGE_CD = " + collegeId;
        sdsCourse.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCenter.ConnectionString = GlobalVariables.ConnectionStringExamServer;

        _instanceId = (short)instanceId;
    }

    //private void RevaluationRpt_Error(object sender, ErrorEventArgs eventArgs)
    //{
    //    var procEl = (ProcessingElement) sender;
    //    procEl.Exception = null;
    //}

    private void RevaluationRpt_NeedDataSource(object sender, EventArgs e)
    {
        var report = sender as Telerik.Reporting.Processing.Report;
        if (null == report) return;

        var collegeId = (short)(null != report.Parameters[ModelConstants.College].Value ?
            Convert.ToInt16(((object[])report.Parameters[ModelConstants.College].Value)[0]) : 0);
        var courseIds = ((object[])report.Parameters[ModelConstants.Course].Value).ToList().Select(s => int.Parse(s.ToString())).ToList();
        var centerId = null != report.Parameters[ModelConstants.Center].Value ?
            Convert.ToInt16(report.Parameters[ModelConstants.Center].Value) : 0;

        var coreService = Bootstrapper.Get<ICoreService>();
        if (coreService?.TBL_STUDENT_REVAL_CHILD_Repository == null) return;

        var instanceName = coreService.Tbl_SYS_INST_Repository.FirstOrDefault(p => p.Num_PK_INST_SRNO == _instanceId, p => p.Var_INST_REM);
        var collegeName = ExamServerHelper.GetCollegeName(collegeId, coreService);

        var dataSource = (from reval in coreService.TBL_STUDENT_REVALUATION_Repository
                .Get(r => r.Num_FK_INST_NO == _instanceId)
            join yrChange in coreService.Tbl_STUDENT_YR_CHNG_Repository
                    .Get(c => c.Num_FK_COL_CD == collegeId &&
                              c.Num_FK_INST_NO == _instanceId && (c.Num_FK_DistCenter_ID ?? 0) == centerId)
                on reval.NUM_FK_PRN_NO equals yrChange.Chr_FK_PRN_NO
            join studentInfo in coreService.Tbl_STUDENT_INFO_Repository.Get()
                on reval.NUM_FK_PRN_NO equals studentInfo.Chr_PK_PRN_NO into defaultStudentInfo
            from studentInfo in defaultStudentInfo.DefaultIfEmpty()
            join coursePart in coreService.Tbl_COURSE_PART_MSTR_Repository.Get()
                on reval.Num_FK_COPRT_NO equals coursePart.Num_PK_COPRT_NO into defaultCoursePart
            from coursePart in defaultCoursePart
            join course in coreService.Tbl_COURSE_MSTR_Repository.Get(p => courseIds.Contains(p.Num_PK_CO_CD))
                on coursePart.Num_FK_CO_CD equals course.Num_PK_CO_CD into defaultCourse
            from course in defaultCourse.DefaultIfEmpty()
            join branch in coreService.Tbl_BRANCH_MSTR_Repository.Get()
                on yrChange.Num_FK_BR_CD equals branch.Num_PK_BR_CD into defaultBranch
            from branch in defaultBranch.DefaultIfEmpty()
            join subject in coreService.Tbl_SUBJECT_MSTR_Repository.Get()
                on reval.NUM_FK_SUB_CD equals subject.Num_PK_SUB_CD into defaultSubject
            from subject in defaultSubject.DefaultIfEmpty()
            select new
            {
                InstanceName = instanceName,
                CollegeId = collegeId,
                CollegeName = collegeName,
                CourseId = coursePart.Num_FK_CO_CD,
                CourseName = course.Var_CO_SHRT_NM,
                CoursePartId = coursePart.Num_PK_COPRT_NO,
                CoursePartName = coursePart.Var_COPRT_SHRT_NM,
                BranchName = branch.Var_BR_SHRT_NM,

                StudentName = studentInfo.Var_ST_NM,
                SeatNo = yrChange.Num_ST_SEAT_NO,
                PrnNo = reval.NUM_FK_PRN_NO,
                SubjectId = reval.NUM_FK_SUB_CD,
                SubjectName = subject.Var_SUBJECT_NM,
                RevaluationFee = reval.NUM_REVALUATION_FEE,
                VerificationFee = reval.NUM_VERIFICATION_FEE
            }).Where(x => courseIds.Contains(x.CourseId)).ToList();

        var childData = (from revalChild in coreService.TBL_STUDENT_REVAL_CHILD_Repository.Get(r => r.Num_FK_INST_NO == _instanceId)
                         join yrChange in coreService.Tbl_STUDENT_YR_CHNG_Repository.Get(c => c.Num_FK_COL_CD == collegeId &&
                                 c.Num_FK_INST_NO == _instanceId && c.Num_FK_DistCenter_ID == centerId)
                             on revalChild.NUM_FK_PRN_NO equals yrChange.Chr_FK_PRN_NO
                         join studentInfo in coreService.Tbl_STUDENT_INFO_Repository.Get()
                             on yrChange.Chr_FK_PRN_NO equals studentInfo.Chr_PK_PRN_NO into defaultStudentInfo
                         from studentInfo in defaultStudentInfo.DefaultIfEmpty()
                         join coursePart in coreService.Tbl_COURSE_PART_MSTR_Repository.Get()
                             on revalChild.Num_FK_COPRT_NO equals coursePart.Num_PK_COPRT_NO into defaultCoursePart
                         from coursePart in defaultCoursePart
                         join course in coreService.Tbl_COURSE_MSTR_Repository.Get(p => courseIds.Contains(p.Num_PK_CO_CD))
                             on coursePart.Num_FK_CO_CD equals course.Num_PK_CO_CD into defaultCourse
                         from course in defaultCourse.DefaultIfEmpty()
                         join branch in coreService.Tbl_BRANCH_MSTR_Repository.Get()
                             on yrChange.Num_FK_BR_CD equals branch.Num_PK_BR_CD into defaultBranch
                         from branch in defaultBranch.DefaultIfEmpty()
                         join subject in coreService.Tbl_SUBJECT_MSTR_Repository.Get()
                             on revalChild.NUM_FK_SUB_CD equals subject.Num_PK_SUB_CD into defaultSubject
                         from subject in defaultSubject.DefaultIfEmpty()
                         select new
                         {
                             InstanceName = instanceName,
                             CollegeId = collegeId,
                             CollegeName = collegeName,
                             CourseId = coursePart.Num_FK_CO_CD,
                             CourseName = course.Var_CO_SHRT_NM,
                             CoursePartId = coursePart.Num_PK_COPRT_NO,
                             CoursePartName = coursePart.Var_COPRT_SHRT_NM,
                             BranchName = branch.Var_BR_SHRT_NM,

                             StudentName = studentInfo.Var_ST_NM,
                             SeatNo = yrChange.Num_ST_SEAT_NO,
                             PrnNo = revalChild.NUM_FK_PRN_NO,
                             SubjectId = revalChild.NUM_FK_SUB_CD,
                             SubjectName = subject.Var_SUBJECT_NM,
                             RevaluationFee = revalChild.NUM_REVALUATION_FEE,
                             VerificationFee = revalChild.NUM_VERIFICATION_FEE
                         }).Where(x => courseIds.Contains(x.CourseId)).ToList();

        dataSource.AddRange(childData);

        if (dataSource.Count <= 0)
            return;

        report.DataSource = dataSource;
    }
}