using Corno.Data.Corno.Masters;
using Corno.Data.Helpers;
using Corno.Data.ViewModels;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Globals.Enums;
using Corno.Logger;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;
using Kendo.Mvc.Extensions;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Mvc;
using Corno.Data.Corno.Paper_Setting.Models;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Paper_Setting.Interfaces;

namespace Corno.OnlineExam.Controllers;

[Authorize]
public class CornoController : UniversityController
{
    #region -- Constructors --
    public CornoController()
    {
        //_masterService = Bootstrapper.Get<IMasterService>();
        _collegeService = Bootstrapper.Get<ICollegeService>();
        _courseService = Bootstrapper.Get<ICourseService>();
        _coursePartService = Bootstrapper.Get<ICoursePartService>();
        _branchService = Bootstrapper.Get<IBranchService>();
        _subjectService = Bootstrapper.Get<ISubjectService>();
        _categoryService = Bootstrapper.Get<ICategoryService>();
        _staffService = Bootstrapper.Get<IStaffService>();
        _miscMasterService = Bootstrapper.Get<IMiscMasterService>();

        _structureService = Bootstrapper.Get<IStructureService>();
    }
    #endregion

    #region -- Data Members --

    //private readonly IMasterService _masterService;
    private readonly ICollegeService _collegeService;
    private readonly ICourseService _courseService;
    private readonly ICoursePartService _coursePartService;
    private readonly IBranchService _branchService;
    private readonly ISubjectService _subjectService;
    private readonly ICategoryService _categoryService;
    private readonly IStaffService _staffService;
    private readonly IMiscMasterService _miscMasterService;

    private readonly IStructureService _structureService;

    #endregion

    #region -- Methods --
    public override JsonResult GetFaculties()
    {
        try
        {
            var facultyService = Bootstrapper.Get<IFacultyService>();
            /*var faculties = _masterService.FacultyRepository.Get()
                .Select(c => new { c.Id, c.Name, NameWithId = $"({c.Id}) {c.Name}" }).ToList();*/
            var faculties = facultyService.GetViewModelList();
            return Json(faculties, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    [AllowAnonymous]
    public override JsonResult GetColleges()
    {
        try
        {
            var colleges = _collegeService.GetViewModelList();
            return Json(colleges, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public override ActionResult GetCollegesByFaculty(int? facultyId, string filter = default)
    {
        try
        {
            var colleges = _collegeService.GetByFaculty(facultyId, filter);
            return Json(colleges, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public ActionResult GetResearchCenters()
    {
        try
        {
            var collegeService = Bootstrapper.Get<ICollegeService>();

            var colleges = collegeService.GetViewModelList(p =>
                p.CollegeTypeId == (int)CollegeType.ResearchCenter);

            return Json(colleges, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public override JsonResult GetCoursesByFacultyAndCollege(int? facultyId, int? collegeId, string filter = default)
    {
        if (collegeId == null)
            return Json(null, JsonRequestBehavior.AllowGet);

        try
        {
            var courses = _courseService.GetCourses(facultyId, collegeId, filter);
            return Json(courses, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public override JsonResult GetAllCoursesByCollege(int? collegeId)
    {
        if (collegeId == null)
            return Json(null, JsonRequestBehavior.AllowGet);

        try
        {
            var courses = _courseService.GetCourses(collegeId);

            return Json(courses, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public JsonResult GetCoursesByFaculty(int? facultyId, string filter = null)
    {
        if (facultyId == null)
            return Json(null, JsonRequestBehavior.AllowGet);

        try
        {
            facultyId = facultyId.ToInt();

            Expression<Func<Course, bool>> predicate = p => p.FacultyId == facultyId && p.Status == StatusConstants.Active;
            predicate = _courseService.AddContainsFilter(predicate, filter);
            
            var courses = _courseService.GetViewModelList(predicate);
            return Json(courses, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public JsonResult GetCourseParts()
    {
        try
        {
            var courseParts = _coursePartService.GetViewModelList(c => c.Status == StatusConstants.Active)
                .OrderByDescending(b => b.Id)
                .ToList();

            return Json(courseParts, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public override JsonResult GetCoursePartsByCourse(int? courseId, string filter = null, FormType formType = FormType.None)
    {
        if (courseId == null)
            return Json(null, JsonRequestBehavior.AllowGet);

        try
        {
            courseId = courseId.ToInt();

            Expression<Func<CoursePart, bool>> predicate = p => p.CourseId == courseId && p.Status == StatusConstants.Active;
            predicate = _coursePartService.AddContainsFilter(predicate, filter);
            var courseParts = _coursePartService.GetViewModelList(predicate);
            return Json(courseParts, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public override JsonResult GetBranchesByCoursePart(int? courseId, int? coursePartId, string filter = default)
    {
        if (null == courseId || null == coursePartId)
            return Json(null, JsonRequestBehavior.AllowGet);

        try
        {
            var branches = _branchService.GetBranches(courseId, coursePartId, filter);

            return Json(branches, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public override JsonResult GetSubjectsByCoursePart(int? coursePartId, int? branchId, bool onlyTheory = false, 
        string filter = default)
    {
        try
        {
            if (null == coursePartId)
                return Json(null, JsonRequestBehavior.AllowGet);

            Expression<Func<Subject, bool>> predicate = p => p.CoursePartId == coursePartId && 
                                                             p.Status == StatusConstants.Active;
            if (onlyTheory)
            {
                var theoryCategories = new[] { 2, 48 };
                predicate = predicate.And(p => p.SubjectCategoryDetails.Any(d =>
                    theoryCategories.Contains(d.CategoryId)));
            }
            // Filter
            predicate = _subjectService.AddContainsFilter(predicate, filter);
            var subjects = _subjectService.GetViewModelList(predicate).ToList();

            return Json(subjects, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public JsonResult GetSubjectsByCategory(int? courseId, int? coursePartId, int? branchId = default, int? categoryId = default, string filter = default)
    {
        try
        {
            if (null == coursePartId)
                return Json(null, JsonRequestBehavior.AllowGet);

            var subjects = _subjectService.GetSubjectsByCategory(courseId, coursePartId, branchId, categoryId, filter);
            return Json(subjects, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public override JsonResult GetAllCategories()
    {
        try
        {
            return Json(_categoryService.GetViewModelList(), JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public JsonResult GetCourseCategories(int? courseId, string filter = default)
    {
        try
        {
            var course = _courseService.GetById(courseId);

            var categoryIds = course.CourseCategoryDetails.Select(d => d.RootCategoryId).Distinct().ToList();
            var categories = _categoryService.GetViewModelList(p => categoryIds.Contains(p.Id ?? 0)).ToList();

            if (!string.IsNullOrEmpty(filter))
                categories = categories.Where(p => p.Id.ToString().ToLower().Contains(filter.ToLower()) ||
                                                   p.Name.ToLower().Contains(filter.ToLower())).ToList();

            return Json(categories, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public JsonResult GetQuestionNos(int? subjectId, int? chapterId)
    {
        if (null == subjectId)
            return Json(null, JsonRequestBehavior.AllowGet);

        try
        {
            var questions = _structureService.GetQuestionNos((int)subjectId, chapterId ?? 0);

            return Json(questions, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public JsonResult GetStaffs()
    {
        try
        {
            var staffs = _staffService.GetViewModelList();
            return Json(staffs, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public JsonResult GetAppointmentStaffs(Appointment appointment, int? collegeId)
    {
        try
        {
            appointment.InstanceId = (HttpContext.Session[User.Identity.Name] as SessionData)?.InstanceId;
            var appointmentService = Bootstrapper.Get<IAppointmentService>();
            appointment = appointmentService.GetStaffs(appointment);

            var staffs = appointment.AppointmentDetails
                .Where(a => a.IsPaperSetter &&
                            a.Status != StatusConstants.Closed)
                .Select(s =>
                {
                    var staffType = s.IsInternal.ToBoolean() ? "Internal" : "External";
                    return new MasterViewModel
                    {
                        Id = s.StaffId ?? 0,
                        Name = $"({s.StaffId}) {s.StaffName} - ({staffType})"
                    };
                }).ToList();

            return Json(staffs, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public JsonResult GetDeans()
    {
        try
        {
            // 1492 is Id for Dean in MiscMaster
            var staffs = _staffService.GetViewModelList(d => d.DesignationId == 1492);
            return Json(staffs, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public JsonResult GetAllQuestionTypes()
    {
        var questionTypes = GetQuestionTypeModels();
        return Json(questionTypes, JsonRequestBehavior.AllowGet);
    }

    public JsonResult GetDifficultyLevels()
    {
        var difficultyLevels = new List<MasterViewModel>
        {
            new() {Id = (int)DifficultyLevel.Difficult, Name = DifficultyLevel.Difficult.ToString().SplitPascalCase()},
            new() {Id = (int)DifficultyLevel.Moderate, Name = DifficultyLevel.Moderate.ToString().SplitPascalCase()},
            new() {Id = (int)DifficultyLevel.Easy, Name = DifficultyLevel.Easy.ToString().SplitPascalCase()},
        };
        return Json(difficultyLevels, JsonRequestBehavior.AllowGet);
    }

    public JsonResult GetMiscMasters(string miscType)
    {
        try
        {
            var miscMasters = _miscMasterService.GetViewModelList(p =>
                p.MiscType == miscType);
            return Json(miscMasters, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }
}

#endregion