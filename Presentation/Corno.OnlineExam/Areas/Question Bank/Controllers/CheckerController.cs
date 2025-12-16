using Corno.Data.Corno;
using Corno.Data.Corno.Masters;
using Corno.Data.Corno.Question_Bank.Dtos;
using Corno.Data.Corno.Question_Bank.Models;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Globals.Enums;
using Corno.OnlineExam.Controllers;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web.Mvc;

namespace Corno.OnlineExam.Areas.Question_Bank.Controllers;

[Authorize]
public class CheckerController : CornoController
{
    #region -- Constructors --
    public CheckerController(IQuestionService questionService, 
        IQuestionAppointmentService questionAppointmentService,
        IFacultyService facultyService, ICourseService courseService,
        IMiscMasterService miscMasterService,
        ISubjectService subjectService, IStaffService staffService)
    {
        _miscMasterService = miscMasterService;
        _subjectService = subjectService;
        _staffService = staffService;
        _questionService = questionService;
        _questionAppointmentService = questionAppointmentService;
        _facultyService = facultyService;
        _courseService = courseService;

        var viewPath = "~/Areas/Question bank/views/Checker";
        _viewPath = $"{viewPath}/View.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly IQuestionService _questionService;
    private readonly IQuestionAppointmentService _questionAppointmentService;
    private readonly IFacultyService _facultyService;
    private readonly ICourseService _courseService;
    private readonly IMiscMasterService _miscMasterService;
    private readonly ISubjectService _subjectService;
    private readonly IStaffService _staffService;

    private readonly string _viewPath;
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
    public ActionResult View(int subjectId, int paperCategoryId)
    {
        try
        {
            /*var staffId = GetStaffId();
            if (HttpContext.Session[User.Identity.Name] is not SessionData sessionData)
                throw new Exception("Invalid session data");
            var appointment = _questionAppointmentService.FirstOrDefault(p => p.InstanceId == sessionData.InstanceId &&
                                                                               p.SubjectId == subjectId &&
                                                                               p.QuestionAppointmentDetails.Any(d => d.StaffId == staffId),
                p => p);
            var appointmentDetail = appointment?.QuestionAppointmentDetails.FirstOrDefault(d => d.StaffId == staffId);
            if (null == appointmentDetail)
                throw new Exception($"No appointment found for subjectId: {subjectId} and staffId: {staffId}");
            var statusList = new List<string> { StatusConstants.Active };

            var query = _questionService.Get(p => 
                statusList.Contains(p.Status), p => p);
            var paperCategories = _miscMasterService.GetQuery();
            var subjects = _subjectService.GetQuery().Include(nameof(Subject.SubjectChapterDetails));
            var questions = from question in query.Where(q =>
                    q.SubjectId == subjectId/* && staffId == q.StaffId#1#)
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
                            Date = question.Date,
                            QuestionTypeName = ((QuestionType)(question.QuestionTypeId ?? 0)).ToString(),
                            Marks = question.Marks,
                            DifficultyLevelName = ((DifficultyLevel)(question.DifficultyLevel ?? 0)).ToString(),
                            PaperCategoryName = paperCategory.Name,
                            CoNo = question.CoNo,
                            Status = question.Status,
                            SubjectName = subject.Name,
                            ChapterName = subject.SubjectChapterDetails.Where(d => d.Id == question.ChapterId).Select(d => d.Name).FirstOrDefault()
                        };
            var questionList = questions.ToList();
            return View(_viewPath, questions.FirstOrDefault());*/
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_viewPath, new Question
        {
            SubjectId = subjectId,
            PaperCategoryId = paperCategoryId
        });
    }

    [HttpPost]
    /*[ValidateAntiForgeryToken]
    [Authorize]
    [ValidateInput(false)]
    [MultipleButton(Name = "action", Argument = "Accept")]*/
    public ActionResult Accept(int id)
    {
        try
        {
            var question = _questionService.GetById(id);
            _questionService.Accept(question);

            return Json(new { Success = true, Message = "Question accepted successfully." },
                JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return Json(new { Success = false, Message = (object)exception.Message },
                JsonRequestBehavior.AllowGet);
        }
    }

    [HttpPost]
    /*[ValidateAntiForgeryToken]
    [Authorize]
    [ValidateInput(false)]
    [MultipleButton(Name = "action", Argument = "Reject")]*/
    public ActionResult Reject(int id)
    {
        try
        {
            var question = _questionService.GetById(id);
            _questionService.Reject(question);

            return Json(new { Success = true, Message = "Question rejected successfully." },
                JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return Json(new { Success = false, Message = (object)exception.Message },
                JsonRequestBehavior.AllowGet);
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
                                                                               p.SubjectId == subjectId &&
                                                                               p.QuestionAppointmentDetails.Any(d => d.StaffId == staffId),
                p => p);
            var appointmentDetail = appointment?.QuestionAppointmentDetails.FirstOrDefault(d => d.StaffId == staffId);
            if (null == appointmentDetail)
                throw new Exception($"No appointment found for subjectId: {subjectId} and staffId: {staffId}");
            var statusList = new List<string> { StatusConstants.Active };

            var query = _questionService.Get(p =>
                statusList.Contains(p.Status), p => p).AsEnumerable();
            var paperCategories = _miscMasterService.GetQuery().AsEnumerable();
            var subjects = _subjectService.GetQuery().Include(nameof(Subject.SubjectChapterDetails))
                .AsEnumerable();
            var questions = from question in query.Where(q =>
                    q.SubjectId == subjectId/* && staffId == q.StaffId*/)
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
                                Date = question.Date,
                                QuestionTypeName = ((QuestionType)(question.QuestionTypeId ?? 0)).ToString(),
                                Marks = question.Marks,
                                DifficultyLevelName = ((DifficultyLevel)(question.DifficultyLevel ?? 0)).ToString(),
                                PaperCategoryName = paperCategory.Name,
                                CoNo = question.CoNo,
                                Status = question.Status,
                                SubjectName = subject.Name,
                                ChapterName = subject.SubjectChapterDetails.Where(d => d.Id == question.ChapterId).Select(d => d.Name).FirstOrDefault()
                            };
            var result = questions.ToDataSourceResult(request);

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

    public ActionResult GetIndexDtos([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            if (HttpContext.Session[User.Identity.Name] is not SessionData sessionData)
                throw new Exception("Invalid session data");
            var appointments = _questionAppointmentService.GetQuery()
                .Where(p => p.InstanceId == sessionData.InstanceId).AsEnumerable();
            var structures = Bootstrapper.Get<IStructureService>();

            var query = from appointment in appointments
                        join structure in structures.GetQuery() on
                            new { appointment.SubjectId } equals new { structure.SubjectId }
                        join faculty in _facultyService.GetQuery().AsEnumerable() 
                            on appointment.FacultyId equals faculty.Id into defaultFaculty
                        from faculty in defaultFaculty.DefaultIfEmpty()
                        join course in _courseService.GetQuery().AsEnumerable() 
                            on appointment.CourseId equals course.Id into defaultCourse
                        from course in defaultCourse.DefaultIfEmpty()
                        join subject in _subjectService.GetQuery().AsEnumerable() 
                            on appointment.SubjectId equals subject.Id into defaultSubject
                        from subject in defaultSubject.DefaultIfEmpty()
                        join paperCategory in _miscMasterService.GetQuery().AsEnumerable() 
                            on structure.PaperCategoryId equals paperCategory.Id into defaultPaperCategory
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

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Inline_Create_Update_Destroy([DataSourceRequest] DataSourceRequest request, LinkDetail model)
    {
        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    }

    #endregion
}