    using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Services.Bootstrapper;
using Corno.Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using Corno.Reports.Extensions;
    using Corno.Services.Core.Interfaces;
using Telerik.Reporting;

namespace Corno.Reports.Exam;

public partial class SubjectSummaryRpt : Report
{
    #region -- Connstructors --
    public SubjectSummaryRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    public SubjectSummaryRpt(int instanceId)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        sdsCollege.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCenters.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCourse.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCoursePart.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsBranch.ConnectionString = GlobalVariables.ConnectionStringExamServer;

        _instanceId = instanceId;

        _coreService = Bootstrapper.Get<ICoreService>();
    }
    #endregion

    #region -- Data Members --

    private readonly ICoreService _coreService;


    private readonly int _instanceId;
    #endregion

    #region -- Private Methods --

    public short[] GetCategories(string subjectType)
    {
        switch (subjectType)
        {
            case "Theory":
                return new short[] { 2, 10, 20, 26, 33, 39, 40, 48, 52, 53, 54, 55, 56 };
            case "Practical":
                return new short[] { 8, 25, 38, 41, 42, 43, 51 };
            case "IA":
                return new short[] { 1, 5, 11, 22, 29, 45, 49, 50 };
            case "Oral":
                return new short[] { 3, 4 };
            case "Viva":
                return new short[] { 6, 30, 31, 44 };
            case "Project":
                return new short[] { 7, 19, 23, 24 };
            case "TW":
                return new short[] { 9, 16 };
            case "EXT EVA":
                return new short[] { 12 };
            case "Dissertation":
                return new short[] { 13 };
            case "Industry Internship":
                return new short[] { 14 };
            case "Clinical":
                return new short[] { 15 };
            case "Sessional":
                return new short[] { 17, 18 };
            case "Panel":
                return new short[] { 21 };
            case "Departmental":
                return new short[] { 27 };
            case "Field Work":
                return new short[] { 28 };
            case "Grade":
                return new short[] { 32 };
            case "TW & P":
                return new short[] { 34 };
            case "TW & O":
                return new short[] { 35 };
            case "Internship":
                return new short[] { 36 };
            case "Seminar":
                return new short[] { 37 };
            case "Guru":
                return new short[] { 46 };
        }

        return null;
    }

    private List<ExamSubjectViewModel> GetSubjects(string subjectType, int collegeId, int coursePartId, int centerId, int branchId)
    {
        var categories = GetCategories(subjectType);
        if (null == categories)
            throw new Exception("Invalid subject type");

        var yrChanges = branchId > 0 ? _coreService.Tbl_STUDENT_YR_CHNG_Repository
                .Get(y => y.Num_FK_INST_NO == _instanceId &&
                          y.Num_FK_COL_CD == collegeId &&
                          y.Num_FK_DistCenter_ID == centerId &&
                          y.Num_FK_BR_CD == branchId)
            : _coreService.Tbl_STUDENT_YR_CHNG_Repository
                .Get(y => y.Num_FK_INST_NO == _instanceId &&
                          y.Num_FK_COL_CD == collegeId &&
                          y.Num_FK_DistCenter_ID == centerId);
            
        var subjects = (from yrChange in yrChanges
            join examSubject in
                _coreService.Tbl_STUDENT_SUBJECT_Repository.Get(s => s.Num_FK_INST_NO == _instanceId &&
                                                                     s.Num_FK_COPRT_NO == coursePartId && s.Chr_ST_SUB_STS != "Z" &&
                                                                     s.Chr_ST_SUB_CAN != "Y" && s.Chr_ST_SUB_STS != "D") on
                yrChange.Chr_FK_PRN_NO equals examSubject.Chr_FK_PRN_NO
            join studentCatMarks in _coreService.Tbl_STUDENT_CAT_MARKS_Repository.Get(
                    c => c.Num_FK_INST_NO == (short)_instanceId && categories.Contains(c.Num_FK_CAT_CD) &&
                         c.Var_ST_PH_STS == "A") on
                new { A = yrChange.Chr_FK_PRN_NO, B = examSubject.Num_FK_SUB_CD } equals
                new { A = studentCatMarks.Var_FK_PRN_NO, B = studentCatMarks.Num_FK_SUB_CD }
            join studentInfo in
                _coreService.Tbl_STUDENT_INFO_Repository.Get() on
                yrChange.Chr_FK_PRN_NO equals studentInfo.Chr_PK_PRN_NO
            select new ExamSubjectViewModel
            {
                SubjectId = examSubject.Num_FK_SUB_CD,
                SeatNo = yrChange.Num_ST_SEAT_NO ?? 0,
                Prn = yrChange.Chr_FK_PRN_NO,
                Gender = studentInfo.Chr_ST_SEX_CD,
                CategoryCode = studentCatMarks.Num_FK_CAT_CD,
                CategoryName = subjectType

            }).OrderBy(y => y.SubjectId).ThenBy(y => y.SeatNo).ToList();

        return subjects;
    }

        
    private void UpdateCategoryAsPerPapers(List<ExamSubjectViewModel> subjects,
        int coursePartId)
    {
        foreach (var subject in subjects)
        {
            var subjectCatMaster = _coreService.Tbl_SUBJECT_CAT_MSTR_Repository.Get(c =>
                c.Num_FK_COPRT_NO == coursePartId && c.Num_FK_SUB_CD == subject.SubjectId &&
                c.Num_FK_CAT_CD == subject.CategoryCode &&
                c.chr_CAT_SUBCAT_APL == "Y").FirstOrDefault();
            if (null != subjectCatMaster) continue;

            var subCatPapMaster =
                _coreService.Tbl_SUB_CATPAP_MSTR_Repository
                    .Get(c => c.Num_FK_COPRT_NO == coursePartId && 
                              c.Num_FK_SUB_CD == subject.SubjectId &&
                              c.CHR_WRT_UNI_APL == "Y").FirstOrDefault();
            if (null != subCatPapMaster) continue;

            subject.CategoryName = "IA";
        }
    }

    #endregion

    #region -- Events --
    private void SubjectSummaryRpt_NeedDataSource(object sender, EventArgs e)
    {
        var report = sender as Telerik.Reporting.Processing.Report;

        if (null == report) return;

        var collegeId = null != report.Parameters[ModelConstants.College].Value ?
            Convert.ToInt32(((object[])report.Parameters[ModelConstants.College].Value)[0]) : 0;
        var centerId = null != report.Parameters[ModelConstants.Center].Value ?
            Convert.ToInt32(report.Parameters[ModelConstants.Center].Value) : 0;
        var courseId = null != report.Parameters[ModelConstants.Course].Value ?
            Convert.ToInt32(((object[])report.Parameters[ModelConstants.Course].Value)[0]) : 0;
        var coursePartId = null != report.Parameters[ModelConstants.CoursePart].Value ?
            Convert.ToInt32(((object[])report.Parameters[ModelConstants.CoursePart].Value)[0]) : 0;
        var branchId = null != report.Parameters[ModelConstants.Branch].Value ?
            Convert.ToInt32(report.Parameters[ModelConstants.Branch].Value) : 0;

        var theorySubjects = GetSubjects("Theory", collegeId, coursePartId, centerId,
            branchId);
        UpdateCategoryAsPerPapers(theorySubjects, coursePartId);

        var practicalSubjects = GetSubjects("Practical", collegeId, coursePartId,
            centerId, branchId);
        var iaSubjects = GetSubjects("IA", collegeId, coursePartId, centerId, branchId);

        // Merge Theory & Practical
        theorySubjects.AddRange(practicalSubjects);
        theorySubjects.AddRange(iaSubjects);

        var instanceName = ExamServerHelper.GetInstanceName(_instanceId, _coreService);
        var collegeName = ExamServerHelper.GetCollegeName(collegeId, _coreService);
        var centerName = ExamServerHelper.GetCentreName(centerId, _coreService);
        var courseName = ExamServerHelper.GetCourseNameFromCoursePartId(coursePartId, _coreService);
        var coursePartName = ExamServerHelper.GetCoursePartName(coursePartId, _coreService);
        var branchName = ExamServerHelper.GetBranchName(branchId, _coreService);

        var dataSource = theorySubjects.GroupBy(y => new { y.SubjectId })
            .Select(g => new
            {
                g.FirstOrDefault()?.InstanceId,
                InstanceName = instanceName,
                CollegeId = collegeId,
                CollegeName = collegeName,
                CenterId = centerId,
                CenterName = centerName,
                CourseId = courseId,
                CourseName = courseName,
                CoursePartId = coursePartId,
                CoursePartName = coursePartName,
                BranchId = branchId,
                BranchName = branchName,

                g.Key.SubjectId,
                SubjectName = ExamServerHelper.GetSubjectName(g.Key.SubjectId, _coreService),

                SeatNosTheory = g.Where(s => s.CategoryName == "Theory")
                    .Select(d => d.SeatNo).OrderBy(d => d).Ranges()
                    .ToRangeString(),
                SeatNosPractical = g.Where(s => s.CategoryName == "Practical")
                    .Select(d => d.SeatNo).OrderBy(d => d).Ranges()
                    .ToRangeString(),
                SeatNosIa = g.Where(s => s.CategoryName == "IA")
                    .Select(d => d.SeatNo).OrderBy(d => d).Ranges()
                    .ToRangeString(),

                //SeatNosTheory = string.Join(",",
                //    g.Where(s => s.CategoryName == "Theory")
                //        .Select(d => d.SeatNo)),
                //SeatNosPractical = string.Join(",",
                //    g.Where(s => s.CategoryName == "Practical")
                //        .Select(d => d.SeatNo)),
                //SeatNosIa = string.Join(",",
                //    g.Where(s => s.CategoryName == "IA")
                //        .Select(d => d.SeatNo)),

                M = g.Count(s => s.Gender == "M"),
                F = g.Count(s => s.Gender != "M"),
                MTheory = g.Count(s => s.Gender == "M" && s.CategoryName == "Theory"),
                FTheory = g.Count(s => s.Gender != "M" && s.CategoryName == "Theory"),
                MPractical = g.Count(s => s.Gender == "M" && s.CategoryName == "Practical"),
                FPractical = g.Count(s => s.Gender != "M" && s.CategoryName == "Practical"),
                MIa = g.Count(s => s.Gender == "M" && s.CategoryName == "IA"),
                FIa = g.Count(s => s.Gender != "M" && s.CategoryName == "IA"),

                Total = g.Count()
            }).ToList();

        //CsvFileDescription outputFileDescription = new CsvFileDescription
        //{
        //    SeparatorChar = ',', // tab delimited
        //    FirstLineHasColumnNames = true, // no column names in first record
        //    IgnoreTrailingSeparatorChar = true,
        //};

        //var csvContext = new CsvContext();
        //csvContext.Write(dataSource, @"E:\Temp\SubjectSummary.csv");

        report.DataSource = dataSource;
    }
    #endregion
}