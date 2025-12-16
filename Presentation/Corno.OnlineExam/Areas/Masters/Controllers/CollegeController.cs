using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Corno.Data.Corno.Masters;
using Corno.Logger;
using Corno.OnlineExam.Controllers;
using Corno.Services.Corno.Masters.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Corno.OnlineExam.Areas.Masters.Controllers;

public class CollegeController : CornoController
{
    #region -- Constructors --
    public CollegeController(ICollegeService collegeService, IFacultyService facultyService,
        ICourseService courseService)
    {
        _collegeService = collegeService;
        _facultyService = facultyService;
        _courseService = courseService;

        const string viewPath = "~/Areas/Masters/views/College/";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly ICollegeService _collegeService;
    private readonly IFacultyService _facultyService;
    private readonly ICourseService _courseService;

    private readonly string _indexPath;
    private readonly string _createPath;
    private readonly string _editPath;
    #endregion

    #region -- Actions --
    [Authorize]
    public ActionResult Index(int? page)
    {
        return View(_indexPath, null);
    }

    // GET: /College/Create

    public ActionResult Create()
    {
        return View(_createPath, new College());
    }

    // POST: /College/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(College model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_createPath, model);

            _collegeService.AddAndSave(model);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, model);
    }

    // GET: /College/Edit/5
    public ActionResult Edit(int? id)
    {
        if (null == id)
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        // Get College
        var college = _collegeService.GetById(id);
        college.CollegeCourseDetails = college.CollegeCourseDetails
            .Select((d, index) => { d.SerialNo = index + 1; return d; })
            .ToList();

        // Update Course Part & Subject Names
        var facultyIds = college.CollegeCourseDetails.Select(d => d.FacultyId).Distinct();
        var faculties = _facultyService.GetViewModelList(p => facultyIds.Contains(p.Id ?? 0));
        college.CollegeCourseDetails.ForEach(d => d.FacultyName = faculties.FirstOrDefault(x => 
            x.Id == d.FacultyId)?.NameWithId);

        var courseIds = college.CollegeCourseDetails.Select(d => d.CourseId).Distinct();
        var courses = _courseService.GetViewModelList(c => courseIds.Contains(c.Id ?? 0));
        college.CollegeCourseDetails.ForEach(d => d.CourseName = courses.FirstOrDefault(x => 
            x.Id == d.CourseId)?.NameWithId);

        return View(_editPath, college);
    }

    // POST: /College/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(College model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_editPath, model);

            _collegeService.UpdateAndSave(model);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_editPath, model);
    }

        
    public ActionResult GetIndexModels([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var query = _collegeService.GetQuery();
            var result = query.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(new { Success = false, Message = exception.Message }, JsonRequestBehavior.AllowGet);
        }
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Inline_Create_Update_Destroy([DataSourceRequest] DataSourceRequest request, CollegeCourseDetail model)
    {
        model.Id ??= 0;

        var faculty = _facultyService.GetViewModel(model.FacultyId);
        model.FacultyName = faculty?.NameWithId;

        var course = _courseService.GetViewModel(model.CourseId);
        model.CourseName = course?.NameWithId;

        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    }
    #endregion
}