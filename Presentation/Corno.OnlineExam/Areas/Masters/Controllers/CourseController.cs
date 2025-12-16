using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Corno.Data.Corno.Masters;
using Corno.Globals.Constants;
using Corno.Logger;
using Corno.OnlineExam.Controllers;
using Corno.Services.Corno.Masters.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Exception = System.Exception;

namespace Corno.OnlineExam.Areas.Masters.Controllers;

public class CourseController : CornoController
{
    #region -- Constructors --
    public CourseController(ICourseService courseService, ICategoryService categoryService)
    {
        _courseService = courseService;
        _categoryService = categoryService;

        const string viewPath = "~/Areas/Masters/views/Course/";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly ICourseService _courseService;
    private readonly ICategoryService _categoryService;

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

    // GET: /Course/Create

    public ActionResult Create()
    {
        return View(_createPath, new Course());
    }

    // POST: /Course/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Course model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_createPath, model);

            //Course model = Mapper.Map<Course>(viewModel);
            _courseService.AddAndSave(model);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, model);
    }

    // GET: /Course/Edit/5
    public ActionResult Edit(int? id)
    {
        if (null == id)
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        // Get Course
        var course = _courseService.GetById((int)id);
        course.CourseCategoryDetails = course.CourseCategoryDetails
            .Select((d, index) => { d.SerialNo = index + 1; return d; })
            .ToList();

        // Update category names
        var categoryIds = course.CourseCategoryDetails.Select(d => d.CategoryId).Distinct().ToList();
        var rootCategoryIds = course.CourseCategoryDetails.Select(d => d.RootCategoryId).Distinct().ToList();
        var categories = _categoryService.GetViewModelList(p => categoryIds.Contains(p.Id ?? 0) || rootCategoryIds.Contains(p.Id ?? 0)).ToList();
        course.CourseCategoryDetails.ForEach(d => d.CategoryName = 
            categories.FirstOrDefault(x => x.Id == d.CategoryId)?.NameWithId);
        course.CourseCategoryDetails.ForEach(d => d.RootCategoryName = 
            categories.FirstOrDefault(x => x.Id == d.RootCategoryId)?.NameWithId);

        return View(_editPath, course);
    }

    // POST: /Course/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(Course model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_editPath, model);

            _courseService.UpdateAndSave(model);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_editPath, model);
    }

        
    public ActionResult GetIndexCourses([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var courseQuery = _courseService.GetQuery();
            var result = courseQuery.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogInfo(exception);
        }
        return Json(new {Success = "false"}, JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Inline_Create_Update_Destroy([DataSourceRequest] DataSourceRequest request, CourseCategoryDetail model)
    {
        model.Id ??= 0;

        // Update category names
        var categories = _categoryService.GetViewModelList(p => 
            p.Id == model.CategoryId || p.Id == model.RootCategoryId).ToList();
        model.CategoryName = categories.FirstOrDefault(x => x.Id == model.CategoryId)?.NameWithId;
        model.RootCategoryName = categories.FirstOrDefault(x => x.Id == model.RootCategoryId)?.NameWithId;

        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    }
    #endregion
}