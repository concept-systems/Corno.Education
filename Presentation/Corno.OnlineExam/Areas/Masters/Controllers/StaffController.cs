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

public class StaffController : CornoController
{
    #region -- Constructors --
    public StaffController(IStaffService staffService, ICollegeService collegeService, 
        ICourseService courseService, ICoursePartService coursePartService,
        ISubjectService subjectService)
    {
        _staffService = staffService;
        _collegeService = collegeService;
        _courseService = courseService;
        _coursePartService = coursePartService;
        _subjectService = subjectService;

        const string viewPath = "~/Areas/Masters/views/Staff/";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly IStaffService _staffService;
    private readonly ICollegeService _collegeService;
    private readonly ICourseService _courseService;
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

    // GET: /Staff/Create

    public ActionResult Create()
    {
        return View(_createPath, new Staff
        {
            FirstName = "First Name"
        });
    }

    // POST: /Staff/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Staff model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_createPath, model);

            // Split name to first, middle and last name
            model.SplitFullName();

            _staffService.AddAndSave(model);

            model.Code = model.Id.ToString();
            _staffService.UpdateAndSave(model);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, model);
    }

    // GET: /Staff/Edit/5
    public ActionResult Edit(int? id)
    {
        if (null == id)
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        // Get Staff
        var model = _staffService.GetById((int)id);

        // Split name to first, middle and last name
        model.SplitFullName();

        model.StaffSubjectDetails = model.StaffSubjectDetails
            .Select((d, index) => { d.SerialNo = index + 1; return d; })
            .ToList();

        // Update Course Part & Subject Names
        var collegeIds = model.StaffSubjectDetails.Select(d => d.CollegeId).Distinct().ToList();
        var colleges = _collegeService.GetViewModelList(p => collegeIds.Contains(p.Id ?? 0));
        model.StaffSubjectDetails.ForEach(d => d.CollegeName = colleges.FirstOrDefault(x => x.Id == d.CollegeId)?.NameWithId);

        var courseIds = model.StaffSubjectDetails.Select(d => d.CourseId).Distinct().ToList();
        var courses = _courseService.GetViewModelList(c => courseIds.Contains(c.Id ?? 0));
        model.StaffSubjectDetails.ForEach(d => d.CourseName = courses.FirstOrDefault(x => x.Id == d.CourseId)?.NameWithId);

        var coursePartIds = model.StaffSubjectDetails.Select(d => d.CoursePartId).Distinct().ToList();
        var courseParts = _coursePartService.GetViewModelList(c => coursePartIds.Contains(c.Id ?? 0));
        model.StaffSubjectDetails.ForEach(d => d.CoursePartName = courseParts.FirstOrDefault(x => x.Id == d.CoursePartId)?.NameWithId);

        var subjectIds = model.StaffSubjectDetails.Select(d => d.SubjectId).Distinct().ToList();
        var subjects = _subjectService.GetViewModelList(c => subjectIds.Contains(c.Id ?? 0));
        model.StaffSubjectDetails.ForEach(d => d.SubjectName = subjects.FirstOrDefault(x => x.Id == d.SubjectId)?.NameWithCode);

        return View(_editPath, model);
    }

    // POST: /Staff/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(Staff model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_editPath, model);

            //model.StaffSubjectDetails.ForEach(d => d.StaffId = model.Id);

            // Split name to first, middle and last name
            //model.SplitFullName();

            _staffService.UpdateAndSave(model);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_editPath, model);
    }

    public ActionResult GetIndexStaffs([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var staffQuery = _staffService.GetQuery();
            var result = staffQuery.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(new { Success = false, Message = exception.Message }, JsonRequestBehavior.AllowGet);
        }
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Inline_Create_Update_Destroy([DataSourceRequest] DataSourceRequest request, StaffSubjectDetail model)
    {
        model.Id ??= 0;

        var college = _collegeService.GetViewModel(model.CollegeId);
        model.CollegeName = college?.NameWithId;
        var course = _courseService.GetViewModel(model.CourseId);
        model.CourseName = course?.NameWithId;
        var coursePart = _coursePartService.GetViewModel(model.CoursePartId);
        model.CoursePartName = coursePart?.NameWithId;
        var subject = _subjectService.GetViewModel(model.SubjectId);
        model.SubjectName = subject?.NameWithCode;

        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    }

    /*[AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Grid_Chapter_Operations([DataSourceRequest] DataSourceRequest request,  model)
    {
        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Grid_Instruction_Operations([DataSourceRequest] DataSourceRequest request, StaffSubjectDetail model)
    {
        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    }*/

    #endregion
}