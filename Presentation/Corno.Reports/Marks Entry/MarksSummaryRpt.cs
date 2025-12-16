using System;
using System.Collections.Generic;
using System.Linq;
using Corno.Data.Core;
using Corno.Data.Helpers;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Reports.Exam;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Helper;
using MoreLinq;
using Telerik.Reporting;

namespace Corno.Reports.Marks_Entry;

public partial class MarksSummaryRpt : Report
{
    #region -- Connstructors --
    public MarksSummaryRpt(int instanceId)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        sdsCollege.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCenters.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCourse.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCoursePart.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsBranch.ConnectionString = GlobalVariables.ConnectionStringExamServer;

        _instanceId = instanceId;
    }
    #endregion

    #region -- Data Members --

    private readonly int _instanceId;
    #endregion

    #region -- Methods --
    private IEnumerable<ExamSubjectViewModel> GetSubjects(int collegeId, int coursePartId, int centerId, List<int> branchIds)
    {
        var coreService = Bootstrapper.Get<ICoreService>();
        if (null == coreService) return null;

        /*var studentSubjects = coreService.Tbl_STUDENT_SUBJECT_Repository.Get(e =>
            e.Num_FK_INST_NO == _instanceId && e.Num_FK_COPRT_NO == coursePartId && e.Chr_ST_SUB_STS != "Z" &&
            e.Chr_ST_SUB_CAN != "Y" && e.Chr_ST_SUB_STS != "D" && e.Chr_ST_SUB_CAN != "Y").ToList();*/

        var studentSubjects = coreService.Tbl_STUDENT_SUBJECT_Repository.Get(e =>
            e.Num_FK_INST_NO == _instanceId && e.Num_FK_COPRT_NO == coursePartId && e.Chr_ST_SUB_STS != "Z" &&
            e.Chr_ST_SUB_STS != "E" && e.Chr_ST_SUB_STS != "D" && e.Chr_ST_SUB_CAN != "Y").ToList();

        /*var isBranchApplicable = coreService.Tbl_COURSE_PART_MSTR_Repository.GetById(coursePartId)
            ?.Chr_COPRT_BRANCH_APP_FLG;*/
        var isBranchApplicable = coreService.Tbl_COURSE_PART_MSTR_Repository.FirstOrDefault(p => 
                p.Num_PK_COPRT_NO == coursePartId, p => p)?.Chr_COPRT_BRANCH_APP_FLG;
        List<Tbl_STUDENT_YR_CHNG> yrChanges;
        if (isBranchApplicable == "Y")
        {
            yrChanges = coreService.Tbl_STUDENT_YR_CHNG_Repository.Get(y => y.Num_FK_INST_NO == _instanceId &&
                                                                            y.Num_FK_COL_CD == collegeId && (y.Num_FK_DistCenter_ID ?? 0) == centerId && branchIds.Contains(y.Num_FK_BR_CD)).ToList();
        }
        else
            yrChanges = coreService.Tbl_STUDENT_YR_CHNG_Repository.Get(y => y.Num_FK_INST_NO == _instanceId &&
                                                                            y.Num_FK_COL_CD == collegeId && (y.Num_FK_DistCenter_ID ?? 0) == centerId).ToList();

        var subjects = (from studentSubject in studentSubjects
                        join categoryMarks in coreService.Tbl_STUDENT_CAT_MARKS_Repository.Get(c =>
                                c.Num_FK_INST_NO == _instanceId && c.Var_ST_PH_STS == "A")
                            on new { A = studentSubject.Chr_FK_PRN_NO, B = studentSubject.Num_FK_SUB_CD } equals
                            new { A = categoryMarks.Var_FK_PRN_NO, B = categoryMarks.Num_FK_SUB_CD }
                        join paperMarks in coreService.Tbl_STUDENT_PAP_MARKS_Repository.Get(c =>
                                c.Num_FK_INST_NO == (short)_instanceId && c.Var_ST_PH_STS == "A") on
                            new { A = studentSubject.Chr_FK_PRN_NO, B = studentSubject.Num_FK_SUB_CD, C = categoryMarks.Num_FK_CAT_CD } equals
                            new { A = paperMarks.Var_FK_PRN_NO, B = paperMarks.Num_FK_SUB_CD, C = paperMarks.Num_FK_CAT_CD } into paperMarksDefault
                        from paperMarks in paperMarksDefault.DefaultIfEmpty()
                        join yrChange in yrChanges
                            on studentSubject.Chr_FK_PRN_NO equals yrChange?.Chr_FK_PRN_NO /*into defaultYrChanges
                            from yrChange in defaultYrChanges.DefaultIfEmpty()*/
                        join studentInfo in coreService.Tbl_STUDENT_INFO_Repository.Get() on
                            yrChange?.Chr_FK_PRN_NO equals studentInfo.Chr_PK_PRN_NO into defaultStudentInfo
                        from studentInfo in defaultStudentInfo.DefaultIfEmpty()
                        join evalCatMaster in coreService.Tbl_EVALCAT_MSTR_Repository.Get() on
                            categoryMarks?.Num_FK_CAT_CD equals evalCatMaster.Num_PK_CAT_CD into defaultEvalCatMaster
                        from evalCatMaster in defaultEvalCatMaster.DefaultIfEmpty()
                            //join paperMaster in coreService.Tbl_SUB_CATPAP_MSTR_Repository.Get() 
                            //    on paperMarks?.Num_FK_PAP_CD equals paperMaster.Num_PK_PAP_CD
                        select new ExamSubjectViewModel
                        {
                            SubjectId = studentSubject.Num_FK_SUB_CD,
                            SeatNo = yrChange?.Num_ST_SEAT_NO ?? 0,
                            Prn = yrChange?.Chr_FK_PRN_NO,
                            Gender = studentInfo?.Chr_ST_SEX_CD,
                            CategoryCode = categoryMarks.Num_FK_CAT_CD,
                            CategoryName = evalCatMaster?.Var_CAT_NM,
                            PaperCode = paperMarks?.Num_FK_PAP_CD ?? 0,
                            //PaperName = paperMaster?.Var_PAP_NM 
                        }).OrderBy(y => y.SubjectId).ThenBy(y => y.SeatNo).ToList();

        subjects = subjects.GroupBy(s => new {s.CategoryCode, s.PaperCode, s.SubjectId, s.SeatNo }).Select(g => g.First()).ToList();

        var paperCodes = subjects.Select(s => s.PaperCode).Distinct();
        var paperMasters = coreService.Tbl_SUB_CATPAP_MSTR_Repository.Get(p =>
                paperCodes.Contains(p.Num_PK_PAP_CD))
            .ToList();
        foreach (var subject in subjects)
            subject.PaperName = paperMasters.FirstOrDefault(m => m.Num_FK_SUB_CD == subject.SubjectId &&
                                                                 m.Num_PK_PAP_CD == subject.PaperCode)?.Var_PAP_NM;

        return subjects;
    }
    #endregion

    #region -- Events --
    private void MarksSummaryRpt_NeedDataSource(object sender, EventArgs e)
    {
        if (sender is not Telerik.Reporting.Processing.Report report) return;

        var collegeId = null != report.Parameters[ModelConstants.College].Value ?
            Convert.ToInt32(((object[])report.Parameters[ModelConstants.College].Value)[0]) : 0;
        var centerId = null != report.Parameters[ModelConstants.Center].Value ?
            Convert.ToInt32(report.Parameters[ModelConstants.Center].Value) : 0;
        var courseId = null != report.Parameters[ModelConstants.Course].Value ?
            Convert.ToInt32(((object[])report.Parameters[ModelConstants.Course].Value)[0]) : 0;
        var coursePartId = null != report.Parameters[ModelConstants.CoursePart].Value ?
            Convert.ToInt32(((object[])report.Parameters[ModelConstants.CoursePart].Value)[0]) : 0;
        var branchIds = null != report.Parameters[ModelConstants.Branch].Value ?
            ((object[])report.Parameters[ModelConstants.Branch].Value).ToList().Select(s => int.Parse(s.ToString())).ToList() : new List<int> { 0 };

        var examService = Bootstrapper.Get<ICoreService>();
        if (null == examService) return;

        var instanceName = ExamServerHelper.GetInstanceName(_instanceId, examService);
        var collegeName = ExamServerHelper.GetCollegeName(collegeId, examService);
        var centerName = ExamServerHelper.GetCentreName(centerId, examService);
        var courseName = ExamServerHelper.GetCourseName(courseId, examService);
        var coursePartName = ExamServerHelper.GetCoursePartName(coursePartId, examService);

        var examSubjectViewModels = GetSubjects(collegeId, coursePartId, centerId, branchIds).ToList();

        var subjectIds = examSubjectViewModels.Select(s => s.SubjectId).Distinct().ToList();
        var subjects = examService.Tbl_SUBJECT_MSTR_Repository.Get(s => subjectIds.Contains(s.Num_PK_SUB_CD)).ToList();

        var enteredPrnList = examService.Tbl_MARKS_TMP_Repository.Get(s => s.Num_FK_INST_NO == _instanceId &&
                                                                           (s.Num_FK_DISTCOL_CD ?? 0) == centerId &&
                                                                           s.Num_FK_COL_CD == collegeId &&
                                                                           s.Chr_FK_COPRT_NO == coursePartId.ToString())
            .ToList();

        //LogHandler.LogInfo($"Prn Count : {enteredPrnList.Count}, Subjects : {string.Join(",", enteredPrnList.Select(p => p.Chr_FK_SUB_CD).Distinct())}");

        var dataSource = examSubjectViewModels
            .GroupBy(y => new { y.SubjectId, y.CategoryCode, y.PaperCode })
            .Select(g =>
            {
                var enteredCount = enteredPrnList.Where(p => p.Chr_FK_SUB_CD.Trim() == g.Key.SubjectId.ToString() &&
                                                             p.Chr_FK_CAT_CD.ToInt() == g.Key.CategoryCode &&
                                                             p.Chr_FK_PAP_CD.ToInt() == g.Key.PaperCode)
                    .DistinctBy(s => s.Chr_CODE_SEAT_NO)
                    .Count();
                var wrongGenders = g.Where(x => x.Gender != "M" && x.Gender != "F").Select(x => x.Prn);
                return new
                {
                    InstanceId = _instanceId,
                    InstanceName = instanceName,
                    CollegeId = collegeId,
                    CollegeName = collegeName,
                    CenterId = centerId,
                    CenterName = centerName,
                    CourseId = courseId,
                    CourseName = courseName,
                    CoursePartId = coursePartId,
                    CoursePartName = coursePartName,
                    g.Key.SubjectId,
                    SubjectName = subjects.FirstOrDefault(s => s.Num_PK_SUB_CD == g.Key.SubjectId)?.Var_SUBJECT_NM,
                    g.Key.CategoryCode,
                    g.FirstOrDefault()?.CategoryName,
                    g.FirstOrDefault()?.PaperCode,
                    g.FirstOrDefault()?.PaperName,

                    M = g.Count(i => i.Gender == "M"),
                    F = g.Count(i => i.Gender == "F"),
                    WrongGenders = string.Join(",", wrongGenders),

                    Total = g.Count(),

                    EnteredCount = enteredCount,

                    SeatNos = string.Join(", ", g.Select(s => s.SeatNo))
            };
            }).ToList();

        report.DataSource = dataSource;
    }
    #endregion
}