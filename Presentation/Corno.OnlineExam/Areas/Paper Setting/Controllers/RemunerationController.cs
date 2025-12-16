using System;
using System.Web.Mvc;
using Corno.Data.Corno;
using Corno.Data.Corno.Paper_Setting.Models;
using Corno.Data.Corno.Question_Bank;
using Corno.Globals;
using Corno.OnlineExam.Attributes;
using Corno.OnlineExam.Controllers;
using Corno.Services.Corno.Paper_Setting.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Corno.OnlineExam.Areas.Paper_Setting.Controllers;

[Authorize]
public class RemunerationController : CornoController
{
    #region -- Constructors --
    public RemunerationController(IRemunerationService remunerationService) 
    {
        _remunerationService = remunerationService;

        _viewPath = "~/Areas/paper setting/views/Remuneration/Create.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly IRemunerationService _remunerationService;

    private readonly string _viewPath;
    #endregion

    #region -- Private Methods --
        
    #endregion

    #region -- Actions --
    [Authorize]
    public ActionResult Create()
    {
        var sessionData = Session[User.Identity.Name] as SessionData;
        var remuneration = new Remuneration
        {
            InstanceId = sessionData?.InstanceId ?? 0,
            CollegeId = sessionData?.CollegeId,
        };

        return View(remuneration);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [MultipleButton(Name = "action", Argument = "Save")]
    public ActionResult Create(Remuneration remuneration)
    {
        if (!ModelState.IsValid)
            return View(_viewPath, remuneration);

        try
        {
            // Validate fields
            _remunerationService.ValidateFields(remuneration);

            // Add or update Remuneration
            _remunerationService.Save(remuneration);

            TempData["Success"] = "Saved successfully.";

            ModelState.Clear();
            return RedirectToAction("Create", new {  });
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_viewPath, remuneration);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "GetAllCourseParts")]
    public ActionResult GetAllCourseParts(Remuneration remuneration)
    {
        if (!ModelState.IsValid || null == remuneration)
            return View(_viewPath, remuneration);

        try
        {
            // Get all Remuneration subjects
            remuneration = _remunerationService.GetCourseParts(remuneration);

            return View(_viewPath, remuneration);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_viewPath, remuneration);
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