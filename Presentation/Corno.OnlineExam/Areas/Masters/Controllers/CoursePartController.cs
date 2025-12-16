using System;
using System.Net;
using System.Web.Mvc;
using Corno.Data.Corno.Masters;
using Corno.Logger;
using Corno.OnlineExam.Controllers;
using Corno.Services.Corno.Masters.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Corno.OnlineExam.Areas.Masters.Controllers;

public class CoursePartController : CornoController
{
    #region -- Constructors --
    public CoursePartController(ICoursePartService coursePartService)
    {
        _coursePartService = coursePartService;

        const string viewPath = "~/Areas/Masters/views/CoursePart/";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly ICoursePartService _coursePartService;

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

    // GET: /CoursePart/Create

    public ActionResult Create()
    {
        return View(_createPath, new CoursePart());
    }

    // POST: /CoursePart/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(CoursePart model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_createPath, model);

            _coursePartService.AddAndSave(model);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, model);
    }

    // GET: /CoursePart/Edit/5
    public ActionResult Edit(int? id)
    {
        if (null == id)
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        // Get CoursePart
        var coursePart = _coursePartService.GetById((int)id);

        return View(_editPath, coursePart);
    }

    // POST: /CoursePart/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(CoursePart model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_editPath, model);

            _coursePartService.UpdateAndSave(model);

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
            var coursePartQuery = _coursePartService.GetQuery();
            var result = coursePartQuery.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(new { Success = false, Message = exception.Message }, JsonRequestBehavior.AllowGet);
        }
    }

    #endregion
}