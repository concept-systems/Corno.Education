using Corno.Data.Core;
using Corno.Data.Corno;
using Corno.Data.Helpers;
using Corno.Globals.Constants;
using Corno.Logger;
using Corno.OnlineExam.Controllers;
using Corno.Services.Core.Interfaces;
using CsvHelper;
using CsvHelper.Configuration;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.Globalization;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Web.Mvc;

namespace Corno.OnlineExam.Areas.Transactions.Controllers;

[Authorize]
public class AbcController : BaseController
{
    #region -- Constructors --
    public AbcController(ICoreService coreService)
    {
        _coreService = coreService;
    }
    #endregion

    #region -- Data Members --
    private readonly ICoreService _coreService;
    #endregion

    #region -- Methods --
    private static string GetClassCode(Tbl_CLASS_MSTR studentClass, string yearlyResultFlag, Tbl_COURSE_PART_MSTR coursePart)
    {
        if (null != studentClass)
            return studentClass.Var_CLS_NM;
        var resultFlag = string.Empty;
        if (coursePart?.Chr_YEARLY_RES_FLG == "Y")
            resultFlag = yearlyResultFlag;
        var result = "-";
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
        return View(new AbcViewModel());
    }

    // POST: /MobileUpdate/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public ActionResult Create(AbcViewModel viewModel)
    {
        try
        {
            if (!ModelState.IsValid)
                View(viewModel);

            var instanceId = HttpContext.Session[ModelConstants.InstanceId].ToInt();

            // Check whether time table is freeze
            var timeTableFreeze = _coreService.Tbl_TimeTableINST_Repository.Get(t =>
                    t.Num_FK_INST_NO == instanceId && t.Num_PK_CO_CD == viewModel.CourseId)
                .FirstOrDefault();
            if (timeTableFreeze is { Chr_FreezeConvocation: "Y" })
                throw new Exception("Convocation is frozen. Not allowed to download data.");

            var instance = _coreService.Tbl_SYS_INST_Repository.Get(i => i.Num_PK_INST_SRNO == instanceId)
                .FirstOrDefault();
            var college = _coreService.TBL_COLLEGE_MSTRRepository.Get(c => c.Num_PK_COLLEGE_CD == viewModel.CollegeId).FirstOrDefault();
            var course = _coreService.Tbl_COURSE_MSTR_Repository.Get(c => c.Num_PK_CO_CD == viewModel.CourseId).FirstOrDefault();
            var coursePart = _coreService.Tbl_COURSE_PART_MSTR_Repository.Get(c => c.Num_PK_COPRT_NO == viewModel.CoursePartId).FirstOrDefault();
            var schedule = _coreService.Tbl_EXAM_SCHEDULE_MSTR_Repository.Get(c =>
                c.Num_FK_INST_NO == instanceId && c.Num_FK_COPRT_NO == viewModel.CoursePartId).FirstOrDefault();
            //var prn = "2401010072";
            var studentExams = _coreService.TBL_STUDENT_EXAMS_Repository.Get(e =>
                e.Num_FK_INST_NO == instanceId && (e.Num_FK_CO_CD ?? 0) == (viewModel.CourseId ?? 0) &&
                e.Num_FK_COPRT_NO == viewModel.CoursePartId).ToList();
            var studentSubjects = _coreService.Tbl_STUDENT_SUBJECT_Repository.Get(s => s.Num_FK_INST_NO == instanceId &&
                s.Num_FK_COPRT_NO == viewModel.CoursePartId && s.Chr_ST_SUB_CAN != "Y" /*&& s.Chr_FK_PRN_NO == prn*/).ToList();
            //LogHandler.LogInfo($"Student subjects : {studentSubjects.Count}");
            var examSubjects = (from exam in studentExams
                                join yrChange in _coreService.Tbl_STUDENT_YR_CHNG_Repository.Get(y => y.Num_FK_INST_NO == instanceId &&
                        y.Num_FK_COL_CD == viewModel.CollegeId)
                    on exam.Chr_FK_PRN_NO equals yrChange.Chr_FK_PRN_NO
                                join subject in studentSubjects on exam.Chr_FK_PRN_NO equals subject.Chr_FK_PRN_NO
                                join grade in _coreService.Tbl_GRADE_MSTR_Repository.Get()
                                    on new { Course = exam.Num_FK_CO_CD ?? 0, Grade = subject.Num_ST_GRD_NO } equals new { Course = grade.Num_FK_CO_CD, Grade = grade.Num_PK_GRD_CD } into defaultGrade
                                from grade in defaultGrade.DefaultIfEmpty()
                                select new StudentSubjectInfo
                                {
                                    Num_FK_INST_NO = exam.Num_FK_INST_NO,
                                    Chr_FK_PRN_NO = exam.Chr_FK_PRN_NO,
                                    Num_ST_SEAT_NO = exam.Num_ST_SEAT_NO,
                                    Num_FK_CO_CD = exam.Num_FK_CO_CD,
                                    Num_FK_COPRT_NO = exam.Num_FK_COPRT_NO,
                                    Num_FK_BR_CD = exam.Num_FK_BR_CD,
                                    Num_FK_CLASS_CD = exam.Num_FK_CLASS_CD,
                                    Chr_ST_YR_RES = exam.Chr_ST_YR_RES,
                                    Num_COPRT_SEMI_NO = exam.Num_COPRT_SEMI_NO,
                                    Num_FK_SUB_CD = subject.Num_FK_SUB_CD,
                                    Chr_ST_SUB_RES = subject.Chr_ST_SUB_RES,
                                    Num_PK_GRD_CD = grade.Num_PK_GRD_CD,
                                    Var_GRD_NM = grade.Var_GRD_NM,
                                    Num_GRD_POINTS = grade.Num_GRD_POINTS,
                                }).ToList();

            //LogHandler.LogInfo($"Exam subjects : {examSubjects.Count}");

            // Get Backlog subjects
            //var syllabusSubjectCodes = (from syllabus in _coreService.Tbl_COPART_SYLLABUS_Repository.Get(s => s.Num_FK_COPRT_NO == viewModel.CoursePartId)
            //    join subject in _coreService.Tbl_COPART_SYLLABUS_TRX_Repository.Get() on syllabus.Num_PK_SYL_NO equals subject.Num_FK_SYL_NO
            //    select subject.Num_FK_SUB_CD).ToList();

            var syllabusSubjectCodes = examSubjects.Select(s => s.Num_FK_SUB_CD).Distinct().ToList();

            var prns = examSubjects.Select(s => s.Chr_FK_PRN_NO).Distinct();
            var studentInfos = _coreService.Tbl_STUDENT_INFO_Repository.Get(c =>
                prns.Contains(c.Chr_PK_PRN_NO)).ToList();
            var studentInfoAdrs = _coreService.Tbl_STUDENT_INFO_ADR_Repository.Get(c =>
                prns.Contains(c.Chr_FK_PRN_NO)).ToList();
            var studentCourses = _coreService.Tbl_STUDENT_COURSE_Repository.Get(c =>
                prns.Contains(c.Chr_FK_PRN_NO)).ToList();

            var branchIds = examSubjects.Select(s =>
                s.Num_FK_BR_CD).Distinct();
            var branches = _coreService.Tbl_BRANCH_MSTR_Repository.Get(c =>
                branchIds.Contains(c.Num_PK_BR_CD)).ToList();
            var classIds = examSubjects.Select(s =>
                s.Num_FK_CLASS_CD).Distinct();
            var classes = _coreService.Tbl_CLASS_MSTR_Repository.Get(c =>
                classIds.Contains(c.Num_PK_CLS_CD)).ToList();
            var cgpaList = _coreService.TBL_STUDENT_CGPA_Repository.Get(c =>
                c.Num_FK_INST_NO == instanceId && c.Num_FK_COPRT_NO == viewModel.CoursePartId &&
                prns.Contains(c.Chr_FK_PRN_NO)).ToList();
            //var subjectIds = examSubjects.Select(s => s.Num_FK_SUB_CD).Distinct();
            var subjects = _coreService.Tbl_SUBJECT_MSTR_Repository.Get(c =>
                syllabusSubjectCodes.Contains(c.Num_PK_SUB_CD)).ToList();

            var groups = examSubjects.GroupBy(g =>
                new { g.Num_FK_COPRT_NO, g.Chr_FK_PRN_NO });

            // Create and get folder details
            var folderPath = GetFolderPath();

            // Define the data rows dynamically
            var rows = new List<Dictionary<string, object>>();
            foreach (var group in groups)
            {
                var exam = group.FirstOrDefault();
                var groupSubjects = group.ToList();
                /*// Check for backlog student
                if (!groupSubjects.Count.Equals(syllabusSubjectCodes.Count))
                {
                    var groupSubjectCodes = groupSubjects.Select(d => d.Num_FK_SUB_CD).Distinct();
                    var backLogSubjects = (from subject in _coreService.Tbl_STUDENT_SUBJECT_Repository.Get(s =>
                            s.Num_FK_COPRT_NO == viewModel.CoursePartId && s.Chr_FK_PRN_NO == exam.Chr_FK_PRN_NO && s.Num_FK_INST_NO != instanceId &&
                            !groupSubjectCodes.Contains(s.Num_FK_SUB_CD) && s.Chr_ST_SUB_RES == "P" && s.Chr_ST_SUB_CAN != "Y")
                        join grade in _coreService.Tbl_GRADE_MSTR_Repository.Get()
                            on new { Course = exam.Num_FK_CO_CD ?? 0, Grade = subject.Num_ST_GRD_NO } equals new { Course = grade.Num_FK_CO_CD, Grade = grade.Num_PK_GRD_CD } into defaultGrade
                        from grade in defaultGrade.DefaultIfEmpty()
                        select new StudentSubjectInfo
                        {
                            Num_FK_INST_NO = exam.Num_FK_INST_NO,
                            Chr_FK_PRN_NO = exam.Chr_FK_PRN_NO,
                            Num_ST_SEAT_NO = exam.Num_ST_SEAT_NO,
                            Num_FK_CO_CD = exam.Num_FK_CO_CD,
                            Num_FK_COPRT_NO = exam.Num_FK_COPRT_NO,
                            Num_FK_BR_CD = exam.Num_FK_BR_CD,
                            Num_FK_CLASS_CD = exam.Num_FK_CLASS_CD,
                            Chr_ST_YR_RES = exam.Chr_ST_YR_RES,
                            Num_COPRT_SEMI_NO = exam.Num_COPRT_SEMI_NO,
                            Num_FK_SUB_CD = subject.Num_FK_SUB_CD,
                            Chr_ST_SUB_RES = subject.Chr_ST_SUB_RES,
                            Num_PK_GRD_CD = grade.Num_PK_GRD_CD,
                            Var_GRD_NM = grade.Var_GRD_NM,
                            Num_GRD_POINTS = grade.Num_GRD_POINTS
                        }).ToList();
                    //backLogSubjects = backLogSubjects.ExceptBy(groupSubjects).ToList();
                    groupSubjects.AddRange(backLogSubjects);
                }*/

                groupSubjects = groupSubjects.OrderBy(g => g.Num_FK_SUB_CD).ToList();

                var studentInfo = studentInfos.FirstOrDefault(s => s.Chr_PK_PRN_NO == exam?.Chr_FK_PRN_NO);
                var studentInfoAdr = studentInfoAdrs.FirstOrDefault(s => s.Chr_FK_PRN_NO == exam?.Chr_FK_PRN_NO);
                var studentCourse = studentCourses.FirstOrDefault(s => s.Chr_FK_PRN_NO == exam?.Chr_FK_PRN_NO);
                var studentClass = classes.FirstOrDefault(s => s.Num_PK_CLS_CD == exam?.Num_FK_CLASS_CD);
                var cgpa = cgpaList.FirstOrDefault(c => c.Chr_FK_PRN_NO == exam?.Chr_FK_PRN_NO);
                var branch = branches.FirstOrDefault(b => b.Num_PK_BR_CD == exam.Num_FK_BR_CD);
                var isImageValid = IsValidImage(studentInfoAdr.Ima_ST_PHOTO, viewModel.IgnorePhotos); // null == studentInfoAdr.Ima_ST_PHOTO || studentInfoAdr.Ima_ST_PHOTO.Length > 0 || !IsValidImage(studentInfoAdr.Ima_ST_PHOTO);
                var photoFileName = isImageValid ? $"{exam.Chr_FK_PRN_NO}.jpg" : string.Empty;
                var row = new Dictionary<string, object>
                {
                    {"ORG_CODE", college?.Num_PK_COLLEGE_CD ?? 0},
                    {"ORG_NAME", college ?.Var_CL_COLLEGE_NM1 ?? string.Empty},
                    {"ACADEMIC_COURSE_ID", course.Num_PK_CO_CD},
                    {"COURSE_NAME" , course.Var_CO_NM ?? string.Empty},
                    {"STREAM",branch?.Var_BR_NM_ABC ?? string.Empty},
                    { "SESSION",  instance ?.Var_INST_REM ?? string.Empty},

                    {"REGN_NO", exam.Chr_FK_PRN_NO ?? string.Empty},
                    {"PHOTO", photoFileName},
                    {"RROLL", exam.Num_ST_SEAT_NO ?? 0},

                    {"CNAME", studentInfo ?.Var_ST_NM ?? string.Empty},
                    {"AADHAAR_NAME", studentInfoAdr ?.AADHAAR_NAME ?? string.Empty},
                    {"GENDER", studentInfo ?.Chr_ST_SEX_CD ?? string.Empty},
                    {"DOB", studentInfo?.Dtm_ST_DOB_DT.ToInt() <= 0 ? $"{new DateTime(1990, 1, 1):dd-MM-yyyy}"
                        : $"{new DateTime(studentInfo.Dtm_ST_DOB_YEAR.ToInt(), studentInfo.Dtm_ST_DOB_MONTH.ToInt(), studentInfo.Dtm_ST_DOB_DT.ToInt()):dd-MM-yyyy}"},
                    {"MRKS_REC_STATUS", "O"},
                    {"RESULT", GetClassCode(studentClass, group.FirstOrDefault()?.Chr_ST_YR_RES, coursePart)},

                    {"YEAR", instance ?.Num_INST_YEAR ?? 0},
                    {"MONTH", new DateTimeFormatInfo().GetMonthName(instance?.Num_INST_MONTH ?? 0)},

                    {"DOI", $"{schedule?.Dtm_Result_Date ?? new DateTime(1990, 1, 1):dd/MM/yyyy}"},

                    {"SEM", RomanNumerals.Convert.ToRomanNumerals(exam.Num_COPRT_SEMI_NO ?? 0)},

                    {"CGPA", coursePart.Chr_COPRT_CLASS_FLG == "Y" ? ((cgpa?.Num_CGPA_AVG ?? 0) + (cgpa.Num_CGPA_GRACE ?? 0)).ToString(CultureInfo.InvariantCulture) : string.Empty},
                    {"SGPA", cgpa?.Num_CGPA ?? 0},

                    {"ABC_ACCOUNT_ID",  studentInfoAdr?.Chr_Abc ?? string.Empty},

                    {"TOT_GRADE_POINTS", groupSubjects.Sum(g => g.Num_GRD_POINTS) ?? 0},

                    {"TOT_CREDIT", cgpa?.Num_TOTAL_CREDITS ?? 0},
                    {"TOT_CREDIT_POINTS", cgpa?.Num_GRADE_POINTS ?? 0},
                    {"ADMISSION_YEAR", studentCourse?.Chr_ST_REG_YEAR},
                    { "NCRF_LEVEL", coursePart.Chr_NCRFLevel ?? string.Empty},
                };

                /*if (exam.Chr_FK_PRN_NO == "2401020072")
                {
                    int i = 0;
                }*/

                var index = 1;
                double totalSubjectCredits = 0;
                double totalCreditPoints = 0;
                foreach (var groupSubject in groupSubjects)
                {
                    var subject = subjects.FirstOrDefault(s => s.Num_PK_SUB_CD == groupSubject.Num_FK_SUB_CD);
                    if (null == subject) continue;

                    var subjectCredits = groupSubject.Chr_ST_SUB_RES != "P" ? 0 : subject.Num_SUBJECT_CREDIT ?? 0;
                    var subjectCreditPoints = (groupSubject?.Num_GRD_POINTS ?? 0) * subjectCredits;
                    var subjectEligibility = groupSubject.Chr_ST_SUB_RES == "P" ? "Y" : "N";
                    totalSubjectCredits += subjectCredits;
                    totalCreditPoints += subjectCreditPoints.ToDouble();

                    row.Add($"SUB{index}", subject?.Num_PK_SUB_CD.ToString());
                    row?.Add($"SUB{index}NM", subject?.Var_SUBJECT_NM ?? string.Empty);
                    row?.Add($"SUB{index}_GRADE", groupSubject?.Var_GRD_NM ?? string.Empty);
                    row.Add($"SUB{index}_GRADE_POINTS", groupSubject?.Num_GRD_POINTS.ToString());
                    row.Add($"SUB{index}_CREDIT", subjectCredits.ToString(CultureInfo.InvariantCulture));
                    row.Add($"SUB{index}_CREDIT_ELIGIBILITY", subjectEligibility);
                    row.Add($"SUB{index}_CREDIT_POINTS", subjectCreditPoints);

                    index++;
                }

                if (isImageValid)
                    WriteImage($"{folderPath}/{photoFileName}", studentInfoAdr.Ima_ST_PHOTO);

                // Update data 
                row["TOT_CREDIT"] = totalSubjectCredits;
                row["TOT_CREDIT_POINTS"] = totalCreditPoints;

                rows.Add(row);
            }

            // Write Csv File
            CreateCsvFile(viewModel, rows, instanceId, folderPath);

            var zipFilePath = Path.Combine(Server.MapPath("~/Zip"), "Files.zip");
            if (System.IO.File.Exists(zipFilePath))
                System.IO.File.Delete(zipFilePath);
            ZipFile.CreateFromDirectory(folderPath, zipFilePath);
            var bytes = System.IO.File.ReadAllBytes(zipFilePath);

            //Send the File to Download.
            return File(bytes, "application/octet-stream", "Files.zip");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            LogHandler.LogInfo(LogHandler.GetDetailException(exception));
        }

        return View(viewModel);
    }

    private string CreateCsvFile(AbcViewModel viewModel, List<Dictionary<string, object>> rows, int instanceId, string folderPath)
    {
        if (rows.Count <= 0)
            throw new Exception("No students found for export");
        var dataFileName = $"{instanceId}_{viewModel.CollegeId}_{viewModel.CourseId}_{viewModel.CoursePartId}.csv";
        var dataFilePath = Path.Combine(folderPath, dataFileName);

        // Write csv file
        WriteCsvFile(dataFilePath, rows);

        return dataFilePath;
    }

    private static bool IsValidImage(byte[] bytes, bool ignorePhotos)
    {
        if (ignorePhotos || bytes is not { Length: > 0 })
            return false;

        try
        {
            using var ms = new MemoryStream(bytes);
            Image.FromStream(ms);
        }
        catch (ArgumentException)
        {
            return false;
        }

        return true;
    }

    private string GetFolderPath()
    {
        var folderPath = Server.MapPath("~/Temp");
        var directory = new DirectoryInfo(folderPath);
        if (!Directory.Exists(folderPath))
            Directory.CreateDirectory(folderPath);
        foreach (var file in directory.GetFiles())
            file.Delete();

        return folderPath;
    }

    private void WriteCsvFile(string filePath, List<Dictionary<string, object>> rows)
    {
        if (rows.Count <= 0) return;

        // Create a stream writer with UTF-8 encoding
        // Write the data to a CSV file
        using var writer = new StreamWriter(filePath);
        using var csv = new CsvWriter(writer, new CsvConfiguration(CultureInfo.InvariantCulture));
        // Write the header row dynamically
        var headers = (rows.OrderBy(r => r.Values.Count)
                .LastOrDefault() ?? throw new Exception("No Data"))
            .Select(r => r.Key).ToList();
        csv.WriteField(headers);
        csv.NextRecord();
        // Write the data rows dynamically
        foreach (var row in rows)
        {
            var values = new List<object>(row.Values);
            csv.WriteField(values);
            csv.NextRecord();
        }
    }

    private void WriteImages(string folderPath, List<Dictionary<string, object>> rows)
    {
        if (rows.Count <= 0) return;

        foreach (var row in rows)
        {
            /*var prn = row["PHOTO"] as string;
            var byteArray = row["PHOTO"] as byte[];*/
            var filePath = $"{folderPath}/{row["REGN_NO"] as string}.jpg";
            WriteImage(filePath, row["PHOTO"] as byte[]);
        }
    }

    private void WriteImage(string filePath, byte[] imageBytes = null)
    {
        if (null == imageBytes) return;

        // Convert byte array to image
        using (var ms = new MemoryStream(imageBytes))
        {
            var image = Image.FromStream(ms);

            // Perform any necessary image processing or resizing here, if needed

            // Save the image to a file
            image.Save(filePath, ImageFormat.Jpeg);
        }
    }

    #endregion
}

public class StudentSubjectInfo
{
    public short Num_FK_INST_NO { get; set; }
    public string Chr_FK_PRN_NO { get; set; }
    public long? Num_ST_SEAT_NO { get; set; }
    public short? Num_FK_CO_CD { get; set; }
    public short Num_FK_COPRT_NO { get; set; }
    public short? Num_FK_BR_CD { get; set; }
    public short? Num_FK_CLASS_CD { get; set; }
    public string Chr_ST_YR_RES { get; set; }
    public short? Num_COPRT_SEMI_NO { get; set; }
    public int Num_FK_SUB_CD { get; set; }
    public string Chr_ST_SUB_RES { get; set; }
    public short? Num_PK_GRD_CD { get; set; }
    public string Var_GRD_NM { get; set; }
    public double? Num_GRD_POINTS { get; set; }
}
