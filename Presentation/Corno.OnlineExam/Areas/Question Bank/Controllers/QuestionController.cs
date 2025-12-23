using Corno.Data.Corno;
using Corno.Data.Corno.Masters;
using Corno.Data.Corno.Question_Bank.Dtos;
using Corno.Data.Corno.Question_Bank.Models;
using Corno.Data.ViewModels;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Globals.Enums;
using Corno.Logger;
using Corno.OnlineExam.Attributes;
using Corno.OnlineExam.Controllers;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;
using DevExpress.XtraPrinting;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Corno.OnlineExam.Areas.Question_Bank.Controllers;

[Authorize]
public class QuestionController : CornoController
{
    #region -- Constructors --
    public QuestionController(IQuestionService questionService, IQuestionAppointmentService questionAppointmentService,
        IFacultyService facultyService, ICourseService courseService,
        IMiscMasterService miscMasterService, IPaperService paperService,
        ISubjectService subjectService, IStaffService staffService)
    {
        _miscMasterService = miscMasterService;
        _paperService = paperService;
        _subjectService = subjectService;
        _staffService = staffService;
        _questionService = questionService;
        _questionAppointmentService = questionAppointmentService;
        _facultyService = facultyService;
        _courseService = courseService;

        var viewPath = "~/Areas/question bank/views/Question";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";
        _templatePath = $"{viewPath}/View.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly IQuestionService _questionService;
    private readonly IQuestionAppointmentService _questionAppointmentService;
    private readonly IFacultyService _facultyService;
    private readonly ICourseService _courseService;
    private readonly IMiscMasterService _miscMasterService;
    private readonly IPaperService _paperService;
    private readonly ISubjectService _subjectService;
    private readonly IStaffService _staffService;

    private readonly string _createPath;
    private readonly string _editPath;
    private readonly string _templatePath;
    #endregion

    #region -- Private Methods --
    private Staff GetStaff()
    {
        var mobile = User.Identity.Name;
        var staff = _staffService.FirstOrDefault(p => p.Mobile == mobile, p => p);
        Session[ModelConstants.Staff] = staff ?? throw new Exception($"No staff found for mobile : {mobile}");
        Session[ModelConstants.StaffId] = staff.Id;
        return staff;
    }
    private int GetStaffId()
    {
        // Update staff id in session.
        if (Session[ModelConstants.StaffId] is int staffId && (int?)staffId > 0)
            return staffId;

        var staff = GetStaff();
        return staff.Id ?? 0;
    }

    private Subject GetSubject(int subjectId)
    {
        // Update staff id in session.
        if (Session[ModelConstants.Subject] is Subject subject && subject.Id == subjectId)
            return subject;
        subject = _subjectService.GetById(subjectId);
        Session[ModelConstants.Subject] = subject ?? throw new Exception($"No subject found for mobile : {subjectId}");
        Session[ModelConstants.SubjectId] = subject.Id;
        return subject;
    }
    #endregion

    #region -- Actions --
    [Authorize]
    public ActionResult Index()
    {
        return View(new Question());
    }

    [Authorize]
    public ActionResult Create(int? facultyId, int? courseId, int? coursePartId, int? subjectId,
        int? paperCategoryId)
    {
        try
        {
            var staff = GetStaff();
            var staffDetail = staff?.StaffSubjectDetails.FirstOrDefault(d => d.SubjectId == subjectId);
            var question = null == staffDetail ? new Question() : staffDetail.Adapt<Question>();
            question.FacultyId = facultyId;
            question.CourseId = courseId;
            question.CoursePartId = coursePartId;
            question.SubjectId = subjectId;
            question.PaperCategoryId = paperCategoryId;
            var subject = GetSubject(question.SubjectId ?? 0);
            question.SubjectName = $"({subject?.Code}) {subject?.Name}";

            return View(_createPath, question);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, new Question());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [ValidateInput(false)]
    [MultipleButton(Name = "action", Argument = "Create")]
    public ActionResult Create(Question question)
    {
        if (!ModelState.IsValid)
            return View(_createPath, question);

        try
        {
            var staffId = GetStaffId();
            if (HttpContext.Session[User.Identity.Name] is not SessionData sessionData)
                throw new Exception("Invalid session data");

            _questionService.SaveQuestion(question, staffId, sessionData.InstanceId, isEdit: false);

            TempData["Success"] = "Saved successfully.";

            ModelState.Clear();

            var newQuestion = new Question
            {
                FacultyId = question.FacultyId,
                CourseId = question.CourseId,
                CoursePartId = question.CoursePartId,
                SubjectId = question.SubjectId,
                PaperCategoryId = question.PaperCategoryId,
                ChapterId = question.ChapterId,
                QuestionTypeId = question.QuestionTypeId,
            };
            return View(_createPath, newQuestion);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, question);
    }

    [Authorize]
    public ActionResult Edit(int id)
    {
        var question = _questionService.GetById(id);
        var subject = GetSubject(question.SubjectId ?? 0);
        question.SubjectName = $"({subject?.Code}) {subject?.Name}";

        return View(_editPath, question);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [ValidateInput(false)]
    [MultipleButton(Name = "action", Argument = "Edit")]
    public ActionResult Edit(Question question)
    {
        if (!ModelState.IsValid)
            return View(_editPath, question);

        try
        {
            var staffId = GetStaffId();
            // InstanceId not used for edit path duplicate update, pass 0
            _questionService.SaveQuestion(question, staffId, 0, isEdit: true);

            TempData["Success"] = "Saved successfully.";

            ModelState.Clear();
            return RedirectToAction("Create", "Question", new
            {
                area = "Question Bank",
                facultyId = question.FacultyId,
                courseId = question.CourseId,
                coursePartId = question.CoursePartId,
                subjectId = question.SubjectId,
                paperCategoryId = question.PaperCategoryId
            });
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_editPath, question);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [ValidateInput(false)]
    [MultipleButton(Name = "action", Argument = "Accept")]
    public ActionResult Accept(Question question)
    {
        if (!ModelState.IsValid)
            return View(_editPath, question);

        try
        {
            // Add or update Structure
            _questionService.Accept(question);

            TempData["Success"] = "Accepted successfully.";

            ModelState.Clear();
            return RedirectToAction("Index", "Question", new { area = "Question Bank" });
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_editPath, question);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [ValidateInput(false)]
    [MultipleButton(Name = "action", Argument = "Reject")]
    public ActionResult Reject(Question question)
    {
        if (!ModelState.IsValid)
            return View(_editPath, question);

        try
        {
            // Add or update Structure
            _questionService.Reject(question);

            TempData["Success"] = "Rejected successfully.";

            ModelState.Clear();
            return RedirectToAction("Index", "Question", new { area = "Question Bank" });
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_editPath, question);
    }

    /*public ActionResult GetQuestions([DataSourceRequest] DataSourceRequest request, QuestionBankViewModel bankViewModel)
    {
        try
        {
            var questions = _questionService.GetQuestions(bankViewModel);
            questions.ForEach(q => q.QuestionTypeName = q.QuestionTypeName.SplitPascalCase());

            // Get all Structure subjects
            return Json(questions.ToDataSourceResult(request));
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return Json(null);
    }*/

    public ActionResult GetQuestionTypeDtos([DataSourceRequest] DataSourceRequest request,
        int? subjectId, int paperCategoryId)
    {
        try
        {
            var staffId = GetStaffId();
            if (HttpContext.Session[User.Identity.Name] is not SessionData sessionData)
            {
                throw new Exception("Invalid session data");
            }

            var appointments = _questionAppointmentService.Get(p => p.InstanceId == sessionData.InstanceId && p.SubjectId == subjectId &&
                            p.QuestionAppointmentDetails.Any(d => d.StaffId == staffId),
                     p => p)
                .AsEnumerable();

            var query = from appointment in appointments
                        from appointmentQuestionType in appointment.QuestionAppointmentTypeDetails
                        where appointmentQuestionType.PaperCategoryId == paperCategoryId
                        join paperCategory in _miscMasterService.GetQuery().AsEnumerable() on paperCategoryId equals paperCategory.Id into defaultPaperCategory
                        from paperCategory in defaultPaperCategory.DefaultIfEmpty()
                        select new QuestionAppointmentTypeDetailDto
                        {
                            Id = appointmentQuestionType.Id ?? 0,
                            PaperCategoryId = paperCategory.Id,
                            PaperCategoryName = paperCategory.Name,
                            QuestionTypeId = appointmentQuestionType.QuestionTypeId,
                            QuestionTypeName = ((QuestionType)(appointmentQuestionType.QuestionTypeId ?? 0)).ToString(),
                            QuestionCount = appointmentQuestionType.QuestionCount ?? 0,
                            CompletedCount = appointmentQuestionType.CompletedCount ?? 0
                        };

            var dataSource = query.ToDataSourceResult(request);

            // Convert to DataSourceResult and return as JSON
            return Json(dataSource, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
        }
    }

    public ActionResult GetQuestionDtos([DataSourceRequest] DataSourceRequest request, int? subjectId)
    {
        try
        {
            var staffId = GetStaffId();
            if (HttpContext.Session[User.Identity.Name] is not SessionData sessionData)
                throw new Exception("Invalid session data");
            var appointment = _questionAppointmentService.FirstOrDefault(p => p.InstanceId == sessionData.InstanceId && 
                                                                               p.SubjectId == subjectId && p.QuestionAppointmentDetails.Any(d => 
                                                                                   d.StaffId == staffId), 
                p => p);
            var appointmentDetail = appointment?.QuestionAppointmentDetails.FirstOrDefault(d => d.StaffId == staffId);
            if(null == appointmentDetail)
                throw new Exception($"No appointment found for subjectId: {subjectId} and staffId: {staffId}");
            var statusList = new List<string> { StatusConstants.Active };
            var staffIds = appointment.QuestionAppointmentDetails
                .Where(d => d.IsSetter || d.IsChecker)
                .Select(d => d.StaffId).Distinct().ToList();
            if(appointmentDetail.IsSetter)
                statusList = [StatusConstants.Active, StatusConstants.Approved, StatusConstants.Rejected];

            var questions = _questionService.Get(p => statusList.Contains(p.Status), p => p)
                .AsEnumerable();
            var paperCategories = _miscMasterService.GetQuery().AsEnumerable();
            var subjects = _subjectService.GetQuery().Include(nameof(Subject.SubjectChapterDetails))
                .AsEnumerable();
            var query = from question in questions.Where(q =>
                    q.SubjectId == subjectId && staffIds.Contains(q.StaffId))
                        join subject in subjects on question.SubjectId equals subject.Id into defaultSubject
                        from subject in defaultSubject.DefaultIfEmpty()
                        join paperCategory in paperCategories
                            on question.PaperCategoryId equals paperCategory.Id into defaultPaperCategory
                        from paperCategory in defaultPaperCategory.DefaultIfEmpty()
                        select new QuestionListDto
                        {
                            //SerialNo = request.Page * request.PageSize + serialNo++,
                            Id = question.Id,
                            Description = question.Description,
                            ModelAnswer = question.ModelAnswer,
                            Date = question.Date,
                            QuestionTypeName = ((QuestionType)(question.QuestionTypeId ?? 0)).ToString(),
                            Marks = question.Marks,
                            DifficultyLevelName = ((DifficultyLevel)(question.DifficultyLevel ?? 0)).ToString(),
                            PaperCategoryName = paperCategory.Name,
                            CoNo = question.CoNo,
                            TaxonomySerialNo = question.TaxonomySerialNo,
                            Status = question.Status,
                            SubjectName = subject.Name,
                            ChapterName = subject.SubjectChapterDetails.Where(d => d.Id == question.ChapterId).Select(d => d.Name).FirstOrDefault()
                        };
            var result = query.ToDataSourceResult(request);

            // Now assign serial numbers after the result is materialized
            var serialNo = (request.Page - 1) * request.PageSize + 1;
            foreach (var item in (IEnumerable<QuestionListDto>)result.Data)
                item.SerialNo = serialNo++;

            // Convert to DataSourceResult and return as JSON
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
        }
    }

    [HttpPost]
    public ActionResult UploadImage()
    {
        var file = Request.Files["upload"];
        if (file is not { ContentLength: > 0 })
            return Json(new { error = new { message = "No file uploaded." } });
        var fileName = Path.GetFileName(file.FileName);
        var uploadPath = Server.MapPath("~/App_Data/Question Images/");
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var filePath = Path.Combine(uploadPath, fileName);
        file.SaveAs(filePath);

        var imageUrl = Url.Content("~/App_Data/Question Images/" + fileName);
        return Json(new { url = imageUrl });
    }

    public ActionResult GetIndexDtos([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            if (HttpContext.Session[User.Identity.Name] is not SessionData sessionData)
                throw new Exception("Invalid session data");
            var staffId = GetStaffId();
            var appointments = _questionAppointmentService.Get(p => p.InstanceId == sessionData.InstanceId &&
                                                                               p.QuestionAppointmentDetails.Any(d =>
                                                                                  d.StaffId == staffId),
                p => p).AsEnumerable();
            /*var appointments = _questionAppointmentService.GetQuery()
                .Where(p => p.InstanceId == sessionData.InstanceId)
                .AsEnumerable();*/
            var structures = Bootstrapper.Get<IStructureService>();

            var query = from appointment in appointments
                        join structure in structures.GetQuery().AsEnumerable() on
                            new { appointment.SubjectId } equals new { structure.SubjectId }
                        join faculty in _facultyService.GetQuery().AsEnumerable() on appointment.FacultyId equals faculty.Id into defaultFaculty
                        from faculty in defaultFaculty.DefaultIfEmpty()
                        join course in _courseService.GetQuery().AsEnumerable() on appointment.CourseId equals course.Id into defaultCourse
                        from course in defaultCourse.DefaultIfEmpty()
                        join subject in _subjectService.GetQuery().AsEnumerable() on appointment.SubjectId equals subject.Id into defaultSubject
                        from subject in defaultSubject.DefaultIfEmpty()
                        join paperCategory in _miscMasterService.GetQuery().AsEnumerable() on structure.PaperCategoryId equals paperCategory.Id into defaultPaperCategory
                        from paperCategory in defaultPaperCategory.DefaultIfEmpty()
                        select new
                        {
                            subject.Id,
                            FacultyId = faculty.Id,
                            FacultyName = faculty.Name,
                            CourseId = course.Id,
                            CourseName = course.Name,
                            appointment.CoursePartId,
                            appointment.BranchId,
                            SubjectId = subject.Id,
                            SubjectCode = subject.Code,
                            SubjectName = subject.Name,
                            structure.PaperCategoryId,
                            PaperCategoryName = paperCategory.Name,
                        };

            var rawResult = query.ToDataSourceResult(request);

            // Now format strings and assign serial numbers
            var serialNo = (request.Page - 1) * request.PageSize + 1;
            var formattedData = ((IEnumerable<dynamic>)rawResult.Data).Select(item => new QuestionIndexDto
            {
                SerialNo = serialNo++,
                Id = item.Id,
                FacultyId = item.FacultyId,
                CourseId = item.CourseId,
                CoursePartId = item.CoursePartId,
                BranchId = item.BranchId,
                SubjectId = item.SubjectId,
                FacultyName = $"({item.FacultyId}) {item.FacultyName}",
                CourseName = $"({item.CourseId}) {item.CourseName}",
                SubjectName = $"({item.SubjectCode}) {item.SubjectName}",
                PaperCategoryId = item.PaperCategoryId,
                PaperCategoryName = $"({item.PaperCategoryId}) {item.PaperCategoryName}",
            }).ToList();

            rawResult.Data = formattedData;

            return Json(rawResult, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return Json(ModelState.ToDataSourceResult(), JsonRequestBehavior.AllowGet);
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ValidateInput(false)]
    [MultipleButton(Name = "action", Argument = "ViewTemplate")]
    public ActionResult ViewTemplate(Question question)
    {
        if (!ModelState.IsValid)
            return View(_editPath, question);

        try
        {
            var paper = question.Adapt<Paper>();
            paper = _paperService.GetQuestions(paper);

            var reichEditServer = Bootstrapper.Get<IRichEditDocumentService>();
            var documentServer = reichEditServer.CreateWordFile(paper);

            // Export to PDF in memory
            using var pdfStream = new MemoryStream();
            var options = new PdfExportOptions();
            documentServer.ExportToPdf(pdfStream, options);
            pdfStream.Seek(0, SeekOrigin.Begin);

            ViewBag.PdfBytes = pdfStream.ToArray();
            ViewBag.PdfBase64 = Convert.ToBase64String(pdfStream.ToArray());
            TempData["Success"] = null;
            return View(_templatePath);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_editPath, question);
    }

    public JsonResult GetChapters(int? subjectId, int paperCategoryId)
    {
        if (null == subjectId)
            return Json(null, JsonRequestBehavior.AllowGet);

        try
        {
            var structureService = Bootstrapper.Get<IStructureService>();
            var structure = structureService.FirstOrDefault(p => p.SubjectId == subjectId && p.PaperCategoryId == paperCategoryId,
            p => p);
            if (null == structure)
                throw new Exception($"No structure found for subjectId: {subjectId} and paperCategoryId: {paperCategoryId}");
            var chapterNos = structure.StructureDetails.Select(d => d.ChapterNos).ToList();
            var chapterSerialNos = chapterNos
                .SelectMany(s => s.Split(','))
                .Select(s => int.Parse(s.Trim()))
                .Distinct()
                .ToList();

            // Get chapters for the subject
            var subject = _subjectService.FirstOrDefault(p => p.Id == subjectId, p => p);
            var chapters = subject?.SubjectChapterDetails
                .Where(c => chapterSerialNos.Contains(c.SerialNo ?? 0))
                .Select(b => new MasterViewModel
                {
                    SerialNo = b.SerialNo,
                    Id = b.Id ?? 0,
                    Name = b.Name,
                    NameWithCode = $"({b.Code}) {b.Name}",
                    NameWithId = $"({b.Id}) {b.Name}"
                })
                .OrderBy(b => b.Id).ToList();
            return Json(chapters, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
        }
        return Json(null, JsonRequestBehavior.AllowGet);
    }

    public JsonResult GetQuestionTypes(int? subjectId, int? paperCategoryId, int? chapterSerialNo)
    {
        if (null == subjectId || null == paperCategoryId || null == chapterSerialNo)
            return Json(null, JsonRequestBehavior.AllowGet);

        try
        {
            var structureService = Bootstrapper.Get<IStructureService>();
            var structure = structureService.FirstOrDefault(p => p.SubjectId == subjectId && p.PaperCategoryId == paperCategoryId,
                p => p);
            if (null == structure)
                throw new Exception($"No structure found for subjectId: {subjectId} and paperCategoryId: {paperCategoryId}");
            var questionTypeIds = structure.StructureDetails
                .Where(d => d.ChapterNos.Contains(chapterSerialNo.ToString()))
                .Select(d => d.QuestionTypeId)
                .Distinct().ToList();

            var questionTypes = GetQuestionTypeModels()
                .Where(p => questionTypeIds.Contains(p.Id));
            return Json(questionTypes, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
        }
        return Json(null, JsonRequestBehavior.AllowGet);
    }

    public JsonResult GetQuestionTypeIds(int? subjectId, int paperCategoryId, int chapterSerialNo)
    {
        if (null == subjectId)
            return Json(null, JsonRequestBehavior.AllowGet);

        try
        {
            var structureService = Bootstrapper.Get<IStructureService>();
            var structure = structureService.FirstOrDefault(p => p.SubjectId == subjectId &&
                                                                 p.PaperCategoryId == paperCategoryId,
                p => p);
            if (null == structure)
                throw new Exception($"No structure found for subjectId: {subjectId} and paperCategoryId: {paperCategoryId}");
            var questionTypeIds = structure.StructureDetails
                .Where(d => d.ChapterNos.Contains(chapterSerialNo.ToString()))
                .Select(d => d.QuestionTypeId).ToList();

            var questionTypes = GetQuestionTypeModels()
                .Where(p => questionTypeIds.Contains(p.Id));

            return Json(questionTypes, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
        }
        return Json(null, JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Inline_Create_Update_Destroy([DataSourceRequest] DataSourceRequest request, LinkDetail model)
    {
        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    }

    #endregion
}