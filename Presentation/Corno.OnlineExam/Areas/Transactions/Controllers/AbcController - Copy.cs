using Corno.OnlineExam.Controllers;
using System;
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
        private static string GetClassCode(Tbl_CLASS_MSTR studentClass, Tbl_COURSE_PART_MSTR coursePart)
        {
            if (null == studentClass)
                return coursePart.Chr_COPRT_CLASS_FLG == "Y" ? "Fail" : string.Empty;
            return "Pass";
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
            return View(new AbcViewModel());
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
                var college = _coreService.TBL_COLLEGE_MSTRRepository.Get(c => c.Num_PK_COLLEGE_CD == viewModel.CollegeId).FirstOrDefault();)
                var studentExams = _coreService.TBL_STUDENT_EXAMS_Repository.Get(e =>
                        e.Num_FK_INST_NO == instanceId && e.Num_FK_CO_CD == viewModel.CourseId &&
                        e.Num_FK_COPRT_NO == viewModel.CoursePartId && e.Num_FK_BR_CD == viewModel.BranchId)
                    .Take(50).ToList();
                var courseIds = studentExams.Select(s => s.Num_FK_CO_CD).Distinct();
                var courses = _coreService.Tbl_COURSE_MSTR_Repository.Get(c =>
                    courseIds.Contains(c.Num_PK_CO_CD)).ToList();
                var coursePartIds = studentExams.Select(s => s.Num_FK_COPRT_NO).Distinct();
                var courseParts = _coreService.Tbl_COURSE_PART_MSTR_Repository.Get(c =>
                    coursePartIds.Contains(c.Num_PK_COPRT_NO)).ToList();
                /*var branchIds = studentExams.Select(s => s.Num_FK_BR_CD).Distinct();
                var branches = _coreService.Tbl_BRANCH_MSTR_Repository.Get(c =>
                    branchIds.Contains(c.Num_PK_BR_CD)).ToList();*/
                var prns = studentExams.Select(s => s.Chr_FK_PRN_NO).Distinct();
                var studentInfos = _coreService.Tbl_STUDENT_INFO_Repository.Get(c =>
                    prns.Contains(c.Chr_PK_PRN_NO)).ToList();

                var subjects =
                    from subject in _coreService.Tbl_STUDENT_SUBJECT_Repository.Get(s => s.Num_FK_INST_NO == instanceId)
                    join exam in studentExams
                        on new { Instance = subject.Num_FK_INST_NO, CoursePart = subject.Num_FK_COPRT_NO } equals new
                            { Instance = exam.Num_FK_INST_NO, CoursePart = exam.Num_FK_COPRT_NO }
                    join grade in _coreService.Tbl_GRADE_MSTR_Repository.Get()
                        on new { Course = exam.Num_FK_CO_CD ?? 0, Grade = subject.Num_ST_GRD_NO } equals new { Course = grade.Num_FK_CO_CD, Grade = grade.Num_PK_GRD_CD} into defaultGrade
                    from grade in defaultGrade.DefaultIfEmpty()
                    select new { subject.Num_ST_GRD_NO, grade.Num_GRD_POINTS };

                var dataSource = from exam in studentExams
                                 join course in courses on exam.Num_FK_CO_CD equals course.Num_PK_CO_CD
                                 join coursePart in courseParts on exam.Num_FK_COPRT_NO equals coursePart.Num_PK_COPRT_NO
                                 join studentInfo in studentInfos on exam.Chr_FK_PRN_NO equals studentInfo.Chr_PK_PRN_NO
                                 join schedule in _coreService.Tbl_CAP_SCHEDULE_MSTR_Repository.Get()
                                     on new { Instance = exam.Num_FK_INST_NO, CoursePart = exam.Num_FK_COPRT_NO } equals new { Instance = schedule.Num_FK_INST_NO ?? 0, CoursePart = schedule.Num_FK_COPRT_NO } into defaultSchedule
                                 from schedule in defaultSchedule.DefaultIfEmpty()
                                 join studentClass in _coreService.Tbl_CLASS_MSTR_Repository.Get()
                                     on exam.Num_FK_CLASS_CD equals studentClass.Num_PK_CLS_CD into defaultClass
                                 from studentClass in defaultClass.DefaultIfEmpty()
                                 join cgpa in _coreService.TBL_STUDENT_CGPA_Repository.Get()
                                     on new { Instance = exam.Num_FK_INST_NO, CoursePart = exam.Num_FK_COPRT_NO } equals new { Instance = cgpa.Num_FK_INST_NO ?? 0, CoursePart = cgpa.Num_FK_COPRT_NO ?? 0 } into defaultCgpa
                                 from cgpa in defaultCgpa.DefaultIfEmpty()
                                     //join branch in branches on exam.Num_FK_BR_CD equals branch.Num_PK_BR_CD
                                 select new AbcExportModel()
                                 {
                                     CollegeCode = college?.Num_PK_COLLEGE_CD ?? 0,
                                     CollegeName = college?.Var_CL_SHRT_NM,
                                     CourseCode = course.Num_PK_CO_CD,
                                     CourseName = course.Var_CO_NM,
                                     Branch = exam.Num_FK_BR_CD,
                                     Instance = instance?.Var_INST_REM,

                                     Prn = exam.Chr_FK_PRN_NO,
                                     SeatNo = exam.Num_ST_SEAT_NO,

                                     StudentName = studentInfo?.Var_ST_NM,
                                     Gender = studentInfo?.Chr_ST_SEX_CD,
                                     DateOfBirth = $@"{studentInfo?.Dtm_ST_DOB_DT}/{studentInfo?.Dtm_ST_DOB_MONTH}/{studentInfo?.Dtm_ST_DOB_YEAR}",
                                     MarksRecordStatus = "O",
                                     CoursePartResult = GetClassCode(studentClass, coursePart),

                                     Year = instance?.Num_INST_YEAR,
                                     Month = instance?.Num_INST_MONTH,

                                     ResultDate = schedule?.Dtm_Result_Date,

                                     SemesterNo = exam.Num_COPRT_SEMI_NO,

                                     Cgpa = coursePart.Chr_COPRT_CLASS_FLG == "Y" ? cgpa?.Num_CGPA_AVG + cgpa?.Num_CGPA_GRACE : null,
                                     Sgpa = cgpa?.Num_CGPA,

                                     AbcAccountId = studentInfo?.Chr_Abc,

                                     TotalGradePoints = string.Empty,

                                     TotalCredit = cgpa?.Num_TOTAL_CREDITS,
                                     TotalCreditPoints = cgpa?.Num_GRADE_POINTS,
                                 };


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
                            csvWriter.WriteRecords(dataSource);
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