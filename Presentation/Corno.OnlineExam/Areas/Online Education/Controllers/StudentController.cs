using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using Corno.Data.Corno.Online_Education;
using Corno.Data.Corno.Question_Bank;
using Corno.Data.Corno.Question_Bank.Models;
using Corno.Data.Dtos.Import;
using Corno.Data.Dtos.Online_Education;
using Corno.Globals.Constants;
using Corno.Logger;
using Corno.OnlineExam.Attributes;
using Corno.OnlineExam.Controllers;
using Corno.OnlineExam.Hubs;
using Corno.Services.Corno.Online_Education.Interfaces;
using DevExpress.Web.Mvc;
using DevExpress.XtraRichEdit;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;
using Microsoft.AspNet.SignalR;

namespace Corno.OnlineExam.Areas.Online_Education.Controllers;

[System.Web.Mvc.Authorize]
public class StudentController : CornoController
{
    #region -- Constructors --
    public StudentController(IOnlineEducationStudentService studentService)
    {
        _studentService = studentService;

        const string viewPath = "~/Areas/Online Education/Views/Student/";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
        _previewPath = $"{viewPath}/View.cshtml";
    }
    #endregion

    #region -- Data Members --
    private readonly IOnlineEducationStudentService _studentService;

    private readonly string _indexPath;
    private readonly string _createPath;
    private readonly string _editPath;
    private readonly string _previewPath;
    #endregion

    #region -- Actions --
    [System.Web.Mvc.Authorize]
    public ActionResult Index(int? page)
    {
        return View(_indexPath);
    }

    [System.Web.Mvc.Authorize]
    public ActionResult Create()
    {
        return View(_createPath, new Paper { Code = "Test" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [System.Web.Mvc.Authorize]
    [ValidateInput(false)]
    [MultipleButton(Name = "action", Argument = "Create")]
    public ActionResult Create(Paper paper)
    {
        if (!ModelState.IsValid)
            return View(_createPath, paper);

        try
        {
            TempData["Success"] = "Saved successfully.";

            // Clear details so that user can add new question for other details
            ModelState.Clear();
            return RedirectToAction("Create");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_createPath, paper);
    }

    public ActionResult Edit(int? id)
    {
        return View(_editPath, null);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ValidateInput(false)]
    [MultipleButton(Name = "action", Argument = "Edit")]
    public ActionResult Edit(Paper model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_editPath, model);

            

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_editPath, model);
    }

    public ActionResult GetIndexDtos([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var query = _studentService.GetQuery();
            var result = query.ProjectToType<StudentIndexDto>().ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception); // Ensure this logs the error properly
            ModelState.AddModelError("", exception.Message); // Use an empty key for better serialization
            return Json(new DataSourceResult { Errors = ModelState });
        }
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Inline_Create_Update_Destroy([DataSourceRequest] DataSourceRequest request, PaperDetail paperDetail)
    {
        var docBytes = RichEditExtension.SaveCopy(ModelConstants.Description,
            DocumentFormat.Rtf);
        paperDetail.DocumentContent = docBytes;

        return Json(new[] { paperDetail }.ToDataSourceResult(request, ModelState));
    }


    public virtual ActionResult Import()
    {
        try
        {
            return View();
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View();
    }

    [HttpPost]
    public ActionResult Import(IEnumerable<HttpPostedFileBase> files)
    {
        try
        {
            var file = files?.FirstOrDefault();
            if (null == file)
                throw new Exception("No file selected for import");

            var importModels = _studentService.Import(file);
            return Json(new { success = true, importModels }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return Json(new
            {
                success = false,
                message = exception.Message
            }, JsonRequestBehavior.AllowGet);
        }
    }

    private void OnProgressChanged(object sender, ProgressDto e)
    {
        var context = GlobalHost.ConnectionManager.GetHubContext<ProgressHub>();
        context.Clients.All.receiveProgress(e);
    }

    [HttpPost]
    public ActionResult Remove(string[] fileNames)
    {
        // Handle file removal
        foreach (var file in fileNames)
        {
            var filePath = Path.Combine(Server.MapPath("~/App_Data/uploads"), file);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
        }
        return Json(new { success = true });
    }

    [HttpPost]
    public async Task<ActionResult> CancelImport()
    {
        try
        {
            _studentService.CancelImport();
            await Task.Delay(0);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
        }
        return Json(new { success = true }, JsonRequestBehavior.AllowGet);
    }

    #endregion
}