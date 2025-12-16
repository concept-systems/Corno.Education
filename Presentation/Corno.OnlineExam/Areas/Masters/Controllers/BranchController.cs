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

public class BranchController : CornoController
{
    #region -- Constructors --
    public BranchController(IBranchService branchService, ICoursePartService coursePartService,
        ISubjectService subjectService)
    {
        _branchService = branchService;
        _coursePartService = coursePartService;
        _subjectService = subjectService;

        const string viewPath = "~/Areas/Masters/views/Branch/";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly IBranchService _branchService;
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

    // GET: /Branch/Create

    public ActionResult Create()
    {
        return View(_createPath, new Branch());
    }

    // POST: /Branch/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Branch model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_createPath, model);

            _branchService.AddAndSave(model);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, model);
    }

    // GET: /Branch/Edit/5
    public ActionResult Edit(int? id)
    {
        if (null == id)
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        // Get Branch
        var branch = _branchService.GetById((int)id);
        branch.BranchSubjectDetails = branch.BranchSubjectDetails
            .Select((d, index) => { d.SerialNo = index + 1; return d; })
            .ToList();

        // Update Course Part & Subject Names
        var coursePartIds = branch.BranchSubjectDetails.Select(d => d.CoursePartId).Distinct();
        var courseParts = _coursePartService.GetViewModelList(p => coursePartIds.Contains(p.Id ?? 0)).ToList();
        branch.BranchSubjectDetails.ForEach(d => d.CoursePartName = courseParts.FirstOrDefault(x => x.Id == d.CoursePartId)?.NameWithId);

        var subjectIds = branch.BranchSubjectDetails.Select(d => d.SubjectId).Distinct();
        var subjects = _subjectService.GetViewModelList(p => subjectIds.Contains(p.Id ?? 0)).ToList();
        branch.BranchSubjectDetails.ForEach(d => d.SubjectName = subjects.FirstOrDefault(x => x.Id == d.SubjectId)?.NameWithId);

        return View(_editPath, branch);
    }

    // POST: /Branch/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(Branch model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_editPath, model);

            _branchService.UpdateAndSave(model);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_editPath, model);
    }

        
    public ActionResult GetIndexBranches([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var query = _branchService.GetQuery();
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
    public ActionResult Inline_Create_Update_Destroy([DataSourceRequest] DataSourceRequest request, BranchSubjectDetail model)
    {
        model.Id ??= 0;
        model.BranchId ??= 0;

        var coursePart = _coursePartService.GetViewModel(model.CoursePartId);
        model.CoursePartName = coursePart?.NameWithId;

        var subject = _subjectService.GetViewModel( model.SubjectId);
        model.SubjectName = subject?.NameWithId;

        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    }
    #endregion
}