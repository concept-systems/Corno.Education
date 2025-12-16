using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Corno.Data.Corno.Masters;
using Corno.Logger;
using Corno.OnlineExam.Controllers;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Masters.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Corno.OnlineExam.Areas.Masters.Controllers;

public class FacultyController : CornoController
{
    #region -- Constructors --
    public FacultyController(IFacultyService facultyService, ICoursePartService coursePartService,
        ISubjectService subjectService)
    {
        _facultyService = facultyService;
        _coursePartService = coursePartService;
        _subjectService = subjectService;

        const string viewPath = "~/Areas/Masters/views/Faculty/";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly IFacultyService _facultyService;
    private readonly ICoursePartService _coursePartService;
    private readonly ISubjectService _subjectService;

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

    // GET: /Faculty/Create

    public ActionResult Create()
    {
        return View(_createPath, new Faculty());
    }

    // POST: /Faculty/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Faculty model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_createPath, model);

            _facultyService.AddAndSave(model);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, model);
    }

    // GET: /Faculty/Edit/5
    public ActionResult Edit(int? id)
    {
        if (null == id)
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        // Get Faculty
        var faculty = _facultyService.GetById((int)id);

        return View(_editPath, faculty);
    }

    // POST: /Faculty/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(Faculty model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_editPath, model);

            _facultyService.UpdateAndSave(model);

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
            var staffService = Bootstrapper.Get<IStaffService>();

            var query = from faculty in _facultyService.GetQuery() as IEnumerable<Faculty>
                        join staff in staffService.GetQuery() as IEnumerable<Staff>
                            on faculty.StaffId equals staff.Id into defaultStaff
                        from staff in defaultStaff.DefaultIfEmpty()
                        select new
                        {
                            faculty.Id,
                            faculty.SerialNo,
                            faculty.Name,
                            faculty.Description,
                            Dean = staff?.Name,
                            faculty.RecognitionDate,
                            faculty.Status
                        };
            var result = query.ToDataSourceResult(request);
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