using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Corno.Data.Corno;
using Corno.Data.Corno.Question_Bank;
using Corno.Data.Corno.Question_Bank.Models;
using Corno.Globals.Constants;
using Corno.Logger;
using Corno.OnlineExam.Attributes;
using Corno.OnlineExam.Controllers;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Masters;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Question_Bank;
using Corno.Services.Corno.Question_Bank.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Corno.OnlineExam.Areas.Question_Bank.Controllers;

[Authorize]
public class StructureController : CornoController
{
    #region -- Constructors --
    public StructureController()
    {
        _structureService = Bootstrapper.Get<IStructureService>();
        _subjectService = Bootstrapper.Get<ISubjectService>();

        _viewPath = "~/Areas/question bank/views/Structure/Create.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly IStructureService _structureService;
    private readonly ISubjectService _subjectService;

    private readonly string _viewPath;
    #endregion

    #region -- Private Methods --
        
    #endregion

    #region -- Actions --
    [Authorize]
    public ActionResult Create()
    {
        //var sessionData = Session[User.Identity.Name] as SessionData;
        var structure = new Structure { /*InstanceId = sessionData?.InstanceId ?? 0 */};

        return View(_viewPath, structure);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [MultipleButton(Name = "action", Argument = "Save")]
    public ActionResult Create(Structure structure)
    {
        if (!ModelState.IsValid)
            return View(_viewPath, structure);

        try
        {
            // Add or update Structure
            _structureService.Save(structure);

            TempData["Success"] = "Saved successfully.";

            ModelState.Clear();
            return RedirectToAction("Create", new {  });
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_viewPath, structure);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "GetQuestions")]
    public ActionResult GetQuestions(Structure structure)
    {
        structure?.StructureDetails.Clear();

        if (!ModelState.IsValid || null == structure)
            return View(_viewPath, structure);

        try
        {
            // Get all Structure subjects
            structure = _structureService.GetQuestions(structure);
            if (structure.StructureDetails.Count <= 0)
            {
                var subject = _subjectService.GetViewModel(structure.SubjectId);
                throw new Exception($"No questions found for selected subject : {subject.Code}");
            }

            structure.EnableHeader = false;

            ModelState.Clear();
            return View(_viewPath, structure);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        LogHandler.LogInfo("Logging error");

        structure.EnableHeader = true;
        return View(_viewPath, structure);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "ClearControls")]
    public ActionResult ClearControls(Structure structure)
    {
        /*if (!ModelState.IsValid || null == structure)
        {
            ModelState.Clear();
            return View(_viewPath, structure);
        }*/

        try
        {
            structure.StructureDetails.Clear();
            structure.NoOfSections = 1;
            structure.NoOfChapters = 0;
            structure.NoOfSections = 0;
            structure.NoOfQuestions = 0;
            structure.MaxMarks = 0;

            ModelState.Clear();
            return View(_viewPath, structure);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        ModelState.Clear();
        return View(_viewPath, structure);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "CopyStructure")]
    public ActionResult CopyStructure(Structure model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_viewPath, model);

            var existing = _structureService.GetExisting(model);
            if (null == existing)
                throw new Exception($"Structure is not crated for subject : {model.SubjectId}");

            var subjects = _subjectService.Get(p => p.CourseId == model.CourseId &&
                                                          p.Id != model.Id && p.Status != StatusConstants.Closed, p => p).ToList();
            var structureSubjectIds = _structureService.Get(p => p.CourseId == model.CourseId, p => 
                    p.SubjectId).ToList();
            var pendingSubjects = subjects.Where(s => !structureSubjectIds.Contains(s.Id)).ToList();

            var structures = new List<Structure>();
            foreach (var subject in pendingSubjects)
            {
                var newStructure = new Structure();
                newStructure.Copy(existing);
                newStructure.SubjectId = subject.Id;

                structures.Add(newStructure);
            }

            _structureService.AddRangeAndSave(structures);

            TempData["Success"] = "Copied successfully.";
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_viewPath, model);
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Inline_Create_Update_Destroy([DataSourceRequest] DataSourceRequest request, LinkDetail model)
    {
        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    }

    /*[HttpPost]*/
    public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
    {
        var fileContents = Convert.FromBase64String(base64);

        return File(fileContents, contentType, fileName);
    }
    #endregion
}