using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.IO;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Corno.Data.Corno.Masters;
using Corno.Data.ViewModels;
using Corno.Globals.Constants;
using Corno.Logger;
using Corno.OnlineExam.Attributes;
using Corno.OnlineExam.Controllers;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Masters;
using Corno.Services.Corno.Masters.Interfaces;
using Ganss.Excel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;
using MoreLinq;

namespace Corno.OnlineExam.Areas.Masters.Controllers;

public class SubjectController : CornoController
{
    #region -- Constructors --
    public SubjectController()
    {
        _subjectService = Bootstrapper.Get<ISubjectService>();
        _categoryService = Bootstrapper.Get<ICategoryService>();
        _miscMasterService = Bootstrapper.Get<IMiscMasterService>();

        const string viewPath = "~/Areas/Masters/views/Subject/";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly ISubjectService _subjectService;
    private readonly ICategoryService _categoryService;
    private readonly IMiscMasterService _miscMasterService;

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

    // GET: /Subject/Create

    public ActionResult Create()
    {
        return View(_createPath, new Subject());
    }

    // POST: /Subject/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "Create")]
    public ActionResult Create(Subject model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_createPath, model);

            //Subject model = Mapper.Map<Subject>(viewModel);
            _subjectService.AddAndSave(model);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, model);
    }

    // GET: /Subject/Edit/5
    public ActionResult Edit(int? id)
    {
        if (null == id)
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        // Get subject
        var subject = _subjectService.GetById((int)id);
        subject.SubjectCategoryDetails = subject.SubjectCategoryDetails
            .Select((d, index) => { d.SerialNo = index + 1; return d; }).ToList();
        subject.SubjectChapterDetails = subject.SubjectChapterDetails
            .Select((d, index) => { d.SerialNo = index + 1; return d; }).ToList();
        subject.SubjectInstructionDetails = subject.SubjectInstructionDetails
            .Select((d, index) => { d.SerialNo = index + 1; return d; }).ToList();
        subject.SubjectSectionDetails = subject.SubjectSectionDetails
            .Select((d, index) => { d.SerialNo = index + 1; return d; }).ToList();

        // Update category names
        var categoryIds = subject.SubjectCategoryDetails.Select(d => d.CategoryId).Distinct();
        var categories = _categoryService.GetViewModelList(c => categoryIds.Contains(c.Id ?? 0));
        subject.SubjectCategoryDetails.ForEach(d => d.CategoryName = categories.FirstOrDefault(x => x.Id == d.CategoryId)?.NameWithId);
        // Update Paper Category names
        var paperCategoryIds = subject.SubjectCategoryDetails.Select(d => d.PaperCategoryId).Distinct();
        var paperCategories = _miscMasterService.GetViewModelList(c => paperCategoryIds.Contains(c.Id ?? 0));
        subject.SubjectCategoryDetails.ForEach(d => d.PaperCategoryName = paperCategories.FirstOrDefault(x => x.Id == d.PaperCategoryId)?.NameWithId);
        subject.SubjectInstructionDetails.ForEach(d => d.PaperCategoryName = paperCategories.FirstOrDefault(x => x.Id == d.PaperCategoryId)?.NameWithId);
        subject.SubjectSectionDetails.ForEach(d => d.PaperCategoryName = paperCategories.FirstOrDefault(x => x.Id == d.PaperCategoryId)?.NameWithId);

        return View(_editPath, subject);
    }

    // POST: /Subject/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "Edit")]
    public ActionResult Edit(Subject model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_editPath, model);

            _subjectService.UpdateAndSave(model);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_editPath, model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "CopyInstructions")]
    public ActionResult CopyInstructions(Subject model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_createPath, model);

            var courseSubjects = _subjectService.Get(p => p.CourseId == model.CourseId &&
               p.Id != model.Id && p.Status != StatusConstants.Closed, p => p).ToList();

            foreach (var subject in courseSubjects)
            {
                foreach (var newInstructionDetail in model.SubjectInstructionDetails)
                {
                    var instructionDetail = subject.SubjectInstructionDetails.FirstOrDefault(d =>
                        d.Description == newInstructionDetail.Description);
                    if (null != instructionDetail)
                        continue;

                    subject.SubjectInstructionDetails.Add(new SubjectInstructionDetail
                    {
                        SerialNo = subject.SubjectInstructionDetails.Count + 1,
                        PaperCategoryId = newInstructionDetail.PaperCategoryId,
                        Description = newInstructionDetail.Description,
                    });
                }
            }

            _subjectService.UpdateRangeAndSave(courseSubjects);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, model);
    }

    public ActionResult GetIndexSubjects([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var subjectQuery = _subjectService.GetQuery().AsNoTracking();
            var result = subjectQuery.ToDataSourceResult(request);

            var paperCategoryIds = (result.Data as List<Subject>)?.SelectMany(p => p.SubjectInstructionDetails.DistinctBy(d => d.PaperCategoryId),
                (_, d) => d.PaperCategoryId).ToList();
            var paperCategories = _miscMasterService.GetViewModelList(c => paperCategoryIds.Contains(c.Id ?? 0));

            result.Data = (result.Data as List<Subject>)?.Select(subject =>
            {
                subject.SubjectInstructionDetails.ForEach(d =>
                {
                    d.PaperCategoryName = paperCategories.FirstOrDefault(x => x.Id == d.PaperCategoryId)?.NameWithId;
                });
                return subject;
            }).ToList();

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(new { Success = false, Message = exception.Message }, JsonRequestBehavior.AllowGet);
        }
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Grid_Chapter_Categories([DataSourceRequest] DataSourceRequest request, SubjectCategoryDetail model)
    {
        model.Id ??= 0;

        var category = _categoryService.GetViewModel(model.CategoryId);
        var paperCategory = _miscMasterService.GetViewModel(model.PaperCategoryId);

        model.CategoryName = category?.NameWithId;
        model.PaperCategoryName = paperCategory?.NameWithId;

        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Grid_Chapter_Operations([DataSourceRequest] DataSourceRequest request, SubjectChapterDetail model)
    {
        model.Id ??= 0;

        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Grid_Instruction_Operations([DataSourceRequest] DataSourceRequest request, SubjectInstructionDetail model)
    {
        model.Id ??= 0;

        var paperCategory = _miscMasterService.GetViewModel(model.PaperCategoryId);
        model.PaperCategoryName = paperCategory?.NameWithId;

        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Grid_Instruction_Sections([DataSourceRequest] DataSourceRequest request, SubjectSectionDetail model)
    {
        model.Id ??= 0;

        var paperCategory = _miscMasterService.GetViewModel(model.PaperCategoryId);
        model.PaperCategoryName = paperCategory?.NameWithId;

        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    }

    public ActionResult Excel_Export_Subject(string contentType, string base64, string fileName)
    {
        var fileContents = Convert.FromBase64String(base64);
        return File(fileContents, contentType, fileName);

        /*var records = _subjectService.GetQuery()
            .ProjectToType<SubjectIndexViewModel>();

        using var stream = new MemoryStream();
        var excelMapper = new ExcelMapper();

        excelMapper.Save(stream, records);
        stream.Position = 0;

        return File(stream.ToArray(), contentType, fileName);*/
    }
    #endregion
}