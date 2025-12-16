using Corno.OnlineExam.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using Corno.Data.Corno;
using Corno.Globals.Constants;
using Corno.Logger;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Helper;
using System.IO;
using CsvHelper.Configuration;
using CsvHelper;
using System.Text;
using System.Globalization;
using Corno.Data.Common;
using Corno.Data.Core;
using MoreLinq;

namespace Corno.OnlineExam.Areas.Transactions.Controllers
{
    [Authorize]
    public class AbcController : BaseController
    {
        #region -- Data Members --
        private readonly ICornoService _cornoService;
        private readonly ICoreService _coreService;
        #endregion

        #region -- Constructors --
        public AbcController(ICornoService registrationService,
            ICoreService examService)
        {
            _cornoService = registrationService;
            _coreService = examService;
        }
        #endregion

        #region -- Methods --
        private static string GetClassCode(Tbl_CLASS_MSTR studentClass, string yearlyResultFlag, Tbl_COURSE_PART_MSTR coursePart)
        {
            if (null != studentClass)
                return studentClass.Var_CLS_NM;
            var resultFlag = string.Empty;
            if (coursePart?.Chr_YEARLY_RES_FLG == "Y")
                resultFlag = yearlyResultFlag;
            var result = string.Empty;
            switch (resultFlag)
            {
                case "F": result = "Fail"; break;
                case "T": result = "ATKT"; break;
                case "P": result = "Pass"; break;
                case "N": result = "Absent"; break;
                case "O": result = "Lower Exam Fail"; break;
            }

            return result;
        }

        /*private static string GetCgpa(TBL_STUDENT_CGPA studentClass, Tbl_COURSE_PART_MSTR coursePart)
        {
            if (null == studentClass.cgpa)
                return coursePart.Chr_COPRT_CLASS_FLG == "Y" ? "Fail" : string.Empty;
            return "Pass";
        }*/
        #endregion

        #region -- Actions --
        // GET: /MobileUpdate/Create
        [Authorize]
        public ActionResult Create()
        {
            return View(new UniversityBaseModel());
        }

        // POST: /MobileUpdate/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(UniversityBaseModel viewModel)
        {
            try
            {
                if (!ModelState.IsValid)
                    View(viewModel);

                LogHandler.LogInfo("Updating mobile");

                var instanceId = HttpContext.Session[ModelConstants.InstanceId].ToInt();
                var instance = _coreService.Tbl_SYS_INST_Repository.Get(i => i.Num_PK_INST_SRNO == instanceId)
                    .FirstOrDefault();
                var college = _coreService.TBL_COLLEGE_MSTRRepository.Get(c => c.Num_PK_COLLEGE_CD == viewModel.CollegeId).FirstOrDefault();
                var course = _coreService.Tbl_COURSE_MSTR_Repository.Get(c => c.Num_PK_CO_CD == viewModel.CourseId).FirstOrDefault();
                var coursePart = _coreService.Tbl_COURSE_PART_MSTR_Repository.Get(c => c.Num_PK_COPRT_NO == viewModel.CoursePartId).FirstOrDefault();
                //var branch = _coreService.Tbl_BRANCH_MSTR_Repository.Get(c => c.Num_PK_BR_CD == viewModel.BranchId).FirstOrDefault();
                var schedule = _coreService.Tbl_EXAM_SCHEDULE_MSTR_Repository.Get(c =>
                    c.Num_FK_INST_NO == instanceId && c.Num_FK_COPRT_NO == viewModel.CoursePartId).FirstOrDefault();
                var prn = "2003100275";
                var exams = (from exam in _coreService.TBL_STUDENT_EXAMS_Repository.Get(e =>
                        e.Num_FK_INST_NO == instanceId && e.Num_FK_CO_CD == viewModel.CourseId &&
                        e.Num_FK_COPRT_NO == viewModel.CoursePartId && e.Chr_FK_PRN_NO == prn/*&& e.Num_FK_BR_CD == viewModel.BranchId*/)
                             join subject in _coreService.Tbl_STUDENT_SUBJECT_Repository.Get(s => s.Num_FK_INST_NO == instanceId &&
                                     s.Num_FK_COPRT_NO == viewModel.CoursePartId && s.Chr_ST_SUB_CAN != "Y") on exam.Chr_FK_PRN_NO equals subject.Chr_FK_PRN_NO
                             join grade in _coreService.Tbl_GRADE_MSTR_Repository.Get()
                                 on new { Course = exam.Num_FK_CO_CD ?? 0, Grade = subject.Num_ST_GRD_NO } equals new { Course = grade.Num_FK_CO_CD, Grade = grade.Num_PK_GRD_CD } into defaultGrade
                             from grade in defaultGrade.DefaultIfEmpty()
                                 //join cgpa in _coreService.TBL_STUDENT_CGPA_Repository.Get()
                                 //    on new { Instance = exam.Num_FK_INST_NO, CoursePart = exam.Num_COPRT_PART_NO, Prn = exam.Chr_FK_PRN_NO } equals new { Instance = cgpa.Num_FK_INST_NO ?? 0, CoursePart = cgpa.Num_FK_COPRT_NO ?? 0, Prn = cgpa.Chr_FK_PRN_NO } into defaultCgpa
                                 //from cgpa in defaultCgpa.DefaultIfEmpty()
                             select new
                             {
                                 exam.Num_FK_INST_NO,
                                 exam.Chr_FK_PRN_NO,
                                 exam.Num_ST_SEAT_NO,
                                 exam.Num_FK_CO_CD,
                                 exam.Num_FK_COPRT_NO,
                                 exam.Num_FK_BR_CD,
                                 exam.Num_FK_CLASS_CD,
                                 exam.Chr_ST_YR_RES,
                                 exam.Num_COPRT_SEMI_NO,
                                 subject.Num_FK_SUB_CD,
                                 grade?.Num_PK_GRD_CD,
                                 grade?.Var_GRD_NM,
                                 grade?.Num_GRD_POINTS,
                                 //cgpa.Num_CGPA, cgpa?.Num_CGPA_AVG, cgpa?.Num_CGPA_GRACE, cgpa?.Num_TOTAL_CREDITS, cgpa?.Num_GRADE_POINTS
                             }).ToList();

                var prns = exams.Select(s => s.Chr_FK_PRN_NO).Distinct();
                var studentInfos = _coreService.Tbl_STUDENT_INFO_Repository.Get(c =>
                    prns.Contains(c.Chr_PK_PRN_NO)).ToList();
                var studentInfoAdrs = _coreService.Tbl_STUDENT_INFO_ADR_Repository.Get(c =>
                    prns.Contains(c.Chr_FK_PRN_NO)).ToList();

                var branchIds = exams.Select(s =>
                    s.Num_FK_BR_CD).Distinct();
                var branches = _coreService.Tbl_BRANCH_MSTR_Repository.Get(c =>
                    branchIds.Contains(c.Num_PK_BR_CD)).ToList();
                var classIds = exams.Select(s =>
                    s.Num_FK_CLASS_CD).Distinct();
                var classes = _coreService.Tbl_CLASS_MSTR_Repository.Get(c =>
                    classIds.Contains(c.Num_PK_CLS_CD)).ToList();
                /*var gradeIds = exams.Select(s => s.gra).Distinct();
                var classes = _coreService.Tbl_CLASS_MSTR_Repository.Get(c =>
                    classIds.Contains(c.Num_PK_CLS_CD)).ToList();*/
                var cgpas = _coreService.TBL_STUDENT_CGPA_Repository.Get(c =>
                    c.Num_FK_INST_NO == instanceId && c.Num_FK_COPRT_NO == viewModel.CoursePartId &&
                    prns.Contains(c.Chr_FK_PRN_NO)).ToList();
                var subjectIds = exams.Select(s => s.Num_FK_SUB_CD).Distinct();
                var subjects = _coreService.Tbl_SUBJECT_MSTR_Repository.Get(c =>
                    subjectIds.Contains(c.Num_PK_SUB_CD)).ToList();

                var groups = exams.GroupBy(g => new { g.Num_FK_COPRT_NO, g.Chr_FK_PRN_NO });

                var models = new List<AbcExportModel>();
                foreach (var group in groups)
                {
                    var exam = group.FirstOrDefault();
                    var studentInfo = studentInfos.FirstOrDefault(s => s.Chr_PK_PRN_NO == exam.Chr_FK_PRN_NO);
                    var studentInfoAdr = studentInfoAdrs.FirstOrDefault(s => s.Chr_FK_PRN_NO == exam.Chr_FK_PRN_NO);
                    var studentClass = classes.FirstOrDefault(s => s.Num_PK_CLS_CD == exam.Num_FK_CLASS_CD);
                    var cgpa = cgpas.FirstOrDefault(c => c.Chr_FK_PRN_NO == exam.Chr_FK_PRN_NO);
                    var groupSubjects = exams.Where(e => e.Num_FK_COPRT_NO == group.Key.Num_FK_COPRT_NO &&
                                                    e.Chr_FK_PRN_NO == group.Key.Chr_FK_PRN_NO).ToList();
                    var branch = branches.FirstOrDefault(b => b.Num_PK_BR_CD == exam.Num_FK_BR_CD);
                    foreach (var groupSubject in groupSubjects)
                    {
                        var subject = subjects.FirstOrDefault(s => s.Num_PK_SUB_CD == groupSubject.Num_FK_SUB_CD);
                        var model = new AbcExportModel
                        {
                            CollegeCode = college?.Num_PK_COLLEGE_CD ?? 0,
                            CollegeName = college?.Var_CL_COLLEGE_NM1,
                            CourseCode = course.Num_PK_CO_CD,
                            CourseName = course.Var_CO_NM,
                            Branch = branch?.Var_BR_NM,
                            Instance = instance?.Var_INST_REM,

                            Prn = exam.Chr_FK_PRN_NO,
                            SeatNo = exam.Num_ST_SEAT_NO,

                            StudentName = studentInfo?.Var_ST_NM,
                            Gender = studentInfo?.Chr_ST_SEX_CD,
                            DateOfBirth = studentInfo?.Dtm_ST_DOB_DT.ToInt() <= 0
                            ? $"{new DateTime(1900, 1, 1):dd/MMM/yyyy}"
                            : $"{new DateTime(studentInfo.Dtm_ST_DOB_YEAR.ToInt(), studentInfo.Dtm_ST_DOB_MONTH.ToInt(), studentInfo.Dtm_ST_DOB_DT.ToInt()):dd/MMM/yyyy}",
                            //: $@"{studentInfo?.Dtm_ST_DOB_DT}/{studentInfo?.Dtm_ST_DOB_MONTH}/{studentInfo?.Dtm_ST_DOB_YEAR}",
                            MarksRecordStatus = "O",
                            CoursePartResult = GetClassCode(studentClass, groupSubject.Chr_ST_YR_RES, coursePart),

                            Year = instance?.Num_INST_YEAR,
                            Month = instance?.Num_INST_MONTH,

                            ResultDate = schedule?.Dtm_Result_Date,

                            SemesterNo = exam.Num_COPRT_SEMI_NO,

                            Cgpa = coursePart.Chr_COPRT_CLASS_FLG == "Y" ? cgpa?.Num_CGPA_AVG + cgpa.Num_CGPA_GRACE : null,
                            Sgpa = cgpa?.Num_CGPA,

                            AbcAccountId = studentInfoAdr?.Chr_Abc,

                            TotalGradePoints = group.Sum(g => g.Num_GRD_POINTS),

                            TotalCredit = cgpa?.Num_TOTAL_CREDITS,
                            TotalCreditPoints = cgpa?.Num_GRADE_POINTS,

                            SubjectCode = subject.Num_PK_SUB_CD,
                            SubjectName = subject.Var_SUBJECT_NM,
                            SubjectGrade = groupSubject.Var_GRD_NM,
                            SubjectGradePoints = groupSubject.Num_GRD_POINTS,

                            SubjectCredit = subject.Num_SUBJECT_CREDIT,
                            SubjectCreditPoints = groupSubject.Num_GRD_POINTS * subject.Num_SUBJECT_CREDIT
                        };
                        models.Add(model);
                    }
                }


                using (var stream = new MemoryStream())
                {
                    // Create a stream writer with UTF-8 encoding
                    using (var writer = new StreamWriter(stream, Encoding.UTF8))
                    {
                        // Create a CSV writer
                        var csvConfiguration = new CsvConfiguration(CultureInfo.InvariantCulture) { HasHeaderRecord = true, };
                        using (var csvWriter = new CsvWriter(writer, csvConfiguration))
                        {
                            // Write the data
                            csvWriter.WriteRecords(models);
                        }
                    }

                    // Prepare the CSV file for download
                    var fileBytes = stream.ToArray();

                    // Set the content type and file name
                    var fileName = "data.csv";
                    var contentType = "text/csv";

                    // Return the CSV file as a downloadable attachment
                    return File(fileBytes, contentType, fileName);
                }
            }
            catch (Exception exception)
            {
                HandleControllerException(exception);
                LogHandler.LogInfo(LogHandler.GetDetailException(exception));
            }

            return View(viewModel);
        }

        #endregion
    }
}