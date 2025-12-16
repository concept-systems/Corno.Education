using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Services.Bootstrapper;
using Corno.Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using Corno.Data.Helpers;
using Corno.Logger;
using Corno.Reports.Extensions;
using Corno.Services.Core.Interfaces;
using Telerik.Reporting;

namespace Corno.Reports.Exam;

public partial class SubjectSummary1Rpt : Report
{
    #region -- Connstructors --
    public SubjectSummary1Rpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        _categoryNames = new[]{ "Theory","Practical","IA","Oral","Viva","Project","TW","EXT EVA","Dissertation","Industry Internship","Clinical","Sessional","Panel","Departmental","Field Work","Grade","TW & P","TW & O","Internship","Seminar","Guru"};
    }

    public SubjectSummary1Rpt(int instanceId)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        sdsCollege.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCenters.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCourse.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCoursePart.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsBranch.ConnectionString = GlobalVariables.ConnectionStringExamServer;

        _instanceId = instanceId;
        _categoryNames = new[] { "Theory", "Practical", "IA", "Oral", "Viva", "Project", "TW", "EXT EVA", "Dissertation", "Industry Internship", "Clinical", "Sessional", "Panel", "Departmental", "Field Work", "Grade", "TW & P", "TW & O", "Internship", "Seminar", "Guru" };
    }
    #endregion

    #region -- Data Members --

    private readonly int _instanceId;
    private readonly string[] _categoryNames;
    #endregion

    #region -- Methods --
    private IEnumerable<ExamSubjectViewModel> GetSubjects(int collegeId, int coursePartId, int centerId, int branchId)
    {
        var coreService = Bootstrapper.Get<ICoreService>();
        if (null == coreService) return null;

        var yrChanges = branchId > 0 ? coreService.Tbl_STUDENT_YR_CHNG_Repository
                .Get(y => y.Num_FK_INST_NO == _instanceId && y.Num_FK_COL_CD == collegeId &&
                          y.Num_FK_DistCenter_ID == centerId && y.Num_FK_BR_CD == branchId).ToList()
            : coreService.Tbl_STUDENT_YR_CHNG_Repository
                .Get(y => y.Num_FK_INST_NO == _instanceId && y.Num_FK_COL_CD == collegeId &&
                          y.Num_FK_DistCenter_ID == centerId).ToList();
        LogHandler.LogInfo($"GetSubjects: CollegeId: {collegeId}, CoursePartId: {coursePartId}, CenterId: {centerId}, BranchId: {branchId} - Found {yrChanges.Count} year changes.");
        var subjects = (from studentSubject in coreService.Tbl_STUDENT_SUBJECT_Repository.Get(e =>
                e.Num_FK_INST_NO == _instanceId && e.Num_FK_COPRT_NO == coursePartId && 
                e.Chr_ST_SUB_STS != "Z" &&
                e.Chr_ST_SUB_CAN != "Y" &&
                e.Chr_ST_SUB_STS != "D").ToList()
            join categoryMarks in coreService.Tbl_STUDENT_CAT_MARKS_Repository.Get(c =>
                    c.Num_FK_INST_NO == _instanceId && c.Var_ST_PH_STS == "A")
                on new { A = studentSubject.Chr_FK_PRN_NO, B = studentSubject.Num_FK_SUB_CD } equals
                new { A = categoryMarks.Var_FK_PRN_NO, B = categoryMarks.Num_FK_SUB_CD } into categoryMarksDefault
            from categoryMarks in categoryMarksDefault.DefaultIfEmpty()
            join paperMarks in coreService.Tbl_STUDENT_PAP_MARKS_Repository.Get(c =>
                    c.Num_FK_INST_NO == (short)_instanceId && c.Var_ST_PH_STS == "A") 
                on new { A = studentSubject.Chr_FK_PRN_NO, B = studentSubject.Num_FK_SUB_CD, C = categoryMarks.Num_FK_CAT_CD } equals
                new { A = paperMarks.Var_FK_PRN_NO, B = paperMarks.Num_FK_SUB_CD, C = paperMarks.Num_FK_CAT_CD } into paperMarksDefault
            from paperMarks in paperMarksDefault.DefaultIfEmpty()
            join yrChange in yrChanges/*coreService.Tbl_STUDENT_YR_CHNG_Repository.Get(y => y.Num_FK_INST_NO == _instanceId &&
                                    y.Num_FK_COL_CD == collegeId && (y.Num_FK_DistCenter_ID ?? 0) == centerId && y.Num_FK_BR_CD == branchId)*/
                on studentSubject.Chr_FK_PRN_NO equals yrChange.Chr_FK_PRN_NO
            join studentInfo in coreService.Tbl_STUDENT_INFO_Repository.Get() on
                yrChange.Chr_FK_PRN_NO equals studentInfo.Chr_PK_PRN_NO into studentInfoDefault
            from studentInfo in studentInfoDefault.DefaultIfEmpty()
            join evalCatMaster in coreService.Tbl_EVALCAT_MSTR_Repository.Get() 
                on categoryMarks.Num_FK_CAT_CD equals evalCatMaster.Num_PK_CAT_CD into evalCatMasterDefault
            from evalCatMaster in evalCatMasterDefault.DefaultIfEmpty()
                        select new ExamSubjectViewModel
            {
                SubjectId = studentSubject?.Num_FK_SUB_CD ?? 0,
                SeatNo = yrChange.Num_ST_SEAT_NO ?? 0,
                Prn = yrChange?.Chr_FK_PRN_NO,
                Gender = studentInfo?.Chr_ST_SEX_CD,
                CategoryCode = evalCatMaster?.Num_PK_CAT_CD ?? 0,
                CategoryName = evalCatMaster?.Var_CAT_NM,
                PaperCode = paperMarks?.Num_FK_PAP_CD ?? 0
            })
            .OrderBy(y => y.SubjectId)
            .ThenBy(y => y.SeatNo)
            .ToList();
        LogHandler.LogInfo($"GetSubjects: Found {subjects.Count} subjects for CollegeId: {collegeId}, CoursePartId: {coursePartId}, CenterId: {centerId}, BranchId: {branchId}");
        return subjects;
    }
    /*private IEnumerable<ExamSubjectViewModel> GetSubjects(int collegeId, int coursePartId, int centerId, int branchId)
    {
        var examService = (ICoreService)Bootstrapper.GetService(typeof(CoreService));
        if (null == examService) return null;

        var subjects = branchId > 0
            ? (from yrChange in examService.Tbl_STUDENT_YR_CHNG_Repository.Get(y => y.Num_FK_INST_NO == _instanceId &&
                                                                                    y.Num_FK_COL_CD == collegeId &&
                                                                                    y.Num_FK_DistCenter_ID == centerId &&
                                                                                    y.Num_FK_BR_CD == branchId)
               join examSubject in
                   examService.Tbl_STUDENT_SUBJECT_Repository.Get(s => s.Num_FK_INST_NO == _instanceId &&
                                                                       s.Num_FK_COPRT_NO == coursePartId &&
                                                                       s.Chr_ST_SUB_STS != "Z" &&
                                                                       s.Chr_ST_SUB_CAN != "Y" &&
                                                                       s.Chr_ST_SUB_STS != "D") on
                   yrChange.Chr_FK_PRN_NO equals examSubject.Chr_FK_PRN_NO
               join studentCatMarks in examService.Tbl_STUDENT_CAT_MARKS_Repository.Get(
                            c => c.Num_FK_INST_NO == (short)_instanceId && 
                                c.Var_ST_PH_STS == "A") on
                  new { A = yrChange.Chr_FK_PRN_NO, B = examSubject.Num_FK_SUB_CD } equals
                  new { A = studentCatMarks.Var_FK_PRN_NO, B = studentCatMarks.Num_FK_SUB_CD }
               join studentInfo in
                   examService.Tbl_STUDENT_INFO_Repository.Get() on
                   yrChange.Chr_FK_PRN_NO equals studentInfo.Chr_PK_PRN_NO
               join evalCatMaster in examService.Tbl_EVALCAT_MSTR_Repository.Get() on
                    studentCatMarks.Num_FK_CAT_CD equals evalCatMaster.Num_PK_CAT_CD
               select new ExamSubjectViewModel
               {
                   SubjectId = examSubject.Num_FK_SUB_CD,
                   SeatNo = yrChange.Num_ST_SEAT_NO ?? 0,
                   Prn = yrChange.Chr_FK_PRN_NO,
                   Gender = studentInfo.Chr_ST_SEX_CD,
                   CategoryName = evalCatMaster.Var_CAT_NM
               }).OrderBy(y => y.SubjectId).ThenBy(y => y.SeatNo).ToList()
            : (from yrChange in examService.Tbl_STUDENT_YR_CHNG_Repository.Get(y => y.Num_FK_INST_NO == _instanceId &&
                                                                                    y.Num_FK_COL_CD == collegeId &&
                                                                                    y.Num_FK_DistCenter_ID == centerId)
               join examSubject in
                   examService.Tbl_STUDENT_SUBJECT_Repository.Get(s => s.Num_FK_INST_NO == _instanceId &&
                                                                       s.Num_FK_COPRT_NO == coursePartId &&
                                                                       s.Chr_ST_SUB_STS != "Z" &&
                                                                       s.Chr_ST_SUB_CAN != "Y" &&
                                                                       s.Chr_ST_SUB_STS != "D") on
                   yrChange.Chr_FK_PRN_NO equals examSubject.Chr_FK_PRN_NO
               join studentCatMarks in examService.Tbl_STUDENT_CAT_MARKS_Repository.Get(
                            c => c.Num_FK_INST_NO == (short)_instanceId && 
                                c.Var_ST_PH_STS == "A") on
                  new { A = yrChange.Chr_FK_PRN_NO, B = examSubject.Num_FK_SUB_CD } equals
                  new { A = studentCatMarks.Var_FK_PRN_NO, B = studentCatMarks.Num_FK_SUB_CD }
               join studentInfo in
                   examService.Tbl_STUDENT_INFO_Repository.Get() on
                   yrChange.Chr_FK_PRN_NO equals studentInfo.Chr_PK_PRN_NO
               join evalCatMaster in examService.Tbl_EVALCAT_MSTR_Repository.Get() on
                    studentCatMarks.Num_FK_CAT_CD equals evalCatMaster.Num_PK_CAT_CD
               select new ExamSubjectViewModel
               {
                   SubjectId = examSubject.Num_FK_SUB_CD,
                   SeatNo = yrChange.Num_ST_SEAT_NO ?? 0,
                   Prn = yrChange.Chr_FK_PRN_NO,
                   Gender = studentInfo.Chr_ST_SEX_CD,
                   CategoryName = evalCatMaster.Var_CAT_NM
               }).OrderBy(y => y.SubjectId).ThenBy(y => y.SeatNo).ToList();

        return subjects;
    }*/
    #endregion

    #region -- Events --
    private void SubjectSummary1Rpt_NeedDataSource(object sender, EventArgs e)
    {
        if (!(sender is Telerik.Reporting.Processing.Report report)) return;

        var collegeId = null != report.Parameters[ModelConstants.College].Value ?
            ((object[])report.Parameters[ModelConstants.College].Value)[0]?.ToInt() ?? 0 : 0;
        var centerId = report.Parameters[ModelConstants.Center].Value?.ToInt() ?? 0;
        var courseId = null != report.Parameters[ModelConstants.Course].Value ?
            ((object[])report.Parameters[ModelConstants.Course].Value)[0]?.ToInt() ?? 0 : 0;
        var coursePartId = null != report.Parameters[ModelConstants.CoursePart].Value ?
            ((object[])report.Parameters[ModelConstants.CoursePart].Value)[0]?.ToInt() ?? 0 : 0;
        var branchId = null != report.Parameters[ModelConstants.Branch].Value ?
            report.Parameters[ModelConstants.Branch].Value?.ToInt() ?? 0 : 0;
        /*var branchIds = null != report.Parameters[ModelConstants.Branch].Value ?
            ((object[])report.Parameters[ModelConstants.Branch].Value).ToList().Select(s => int.Parse(s.ToString())).ToList() : new List<int> { 0 };*/

        var examService = Bootstrapper.Get<ICoreService>();
        if (null == examService) return;
        LogHandler.LogInfo($"SubjectSummary1Rpt_NeedDataSource: CollegeId: {collegeId}, CoursePartId: {coursePartId}, CenterId: {centerId}, BranchId: {branchId}");
        var examSubjectViewModels = GetSubjects(collegeId, coursePartId, centerId, branchId)
            .ToList();
        LogHandler.LogInfo($"SubjectSummary1Rpt_NeedDataSource: {examSubjectViewModels.Count} subjects found for CollegeId: {collegeId}, CoursePartId: {coursePartId}, CenterId: {centerId}, BranchId: {branchId}");
        var subjects = examSubjectViewModels
            .GroupBy(y => new { y.SubjectId, y.CategoryName})
            .Select(g => new
            {
                InstanceId = _instanceId,
                InstanceName = ExamServerHelper.GetInstanceName(_instanceId, examService),
                CollegeId = collegeId,
                CollegeName = ExamServerHelper.GetCollegeName(collegeId, examService),
                CenterId = centerId,
                CenterName = ExamServerHelper.GetCentreName(centerId, examService),
                CourseId = courseId,
                CourseName = ExamServerHelper.GetCourseNameFromCoursePartId(coursePartId, examService),
                CoursePartId = coursePartId,
                CoursePartName = ExamServerHelper.GetCoursePartName(coursePartId, examService),
                BranchId = branchId,
                BranchName = ExamServerHelper.GetBranchName(branchId, examService),
                g.Key.SubjectId,
                SubjectName = ExamServerHelper.GetSubjectName(g.Key.SubjectId, examService),
                //SeatNos = string.Join(",", g.Select(i => 
                //    i.SeatNo)),
                SeatNos = g.OrderBy(i => i.SeatNo)
                    .Select(i =>    i.SeatNo)
                    .Ranges().ToRangeString(),
                g.Key.CategoryName,

                M = g.Count(i => i.Gender == "M"),
                F = g.Count(i => i.Gender == "F"),

                Total = g.Count()
            }).ToList();

        report.DataSource = subjects;
    }
    #endregion
}