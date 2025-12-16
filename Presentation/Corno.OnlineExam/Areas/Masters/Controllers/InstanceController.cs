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

namespace Corno.OnlineExam.Areas.Masters.Controllers;

public class InstanceController : CornoController
{
    #region -- Constructors --
    public InstanceController(IInstanceService instanceService)
    {
        _instanceService = instanceService;

        const string viewPath = "~/Areas/Masters/views/Instance/";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly IInstanceService _instanceService;

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

    // GET: /Instance/Create

    public ActionResult Create()
    {
        return View(_createPath, new Instance());
    }

    // POST: /Instance/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(Instance model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_createPath, model);

            _instanceService.AddAndSave(model);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, model);
    }

    // GET: /Instance/Edit/5
    public ActionResult Edit(int? id)
    {
        if (null == id)
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        // Get Instance
        var instance = _instanceService.GetById((int)id);
            
        return View(_editPath, instance);
    }

    // POST: /Instance/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(Instance model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_editPath, model);

            _instanceService.UpdateAndSave(model);

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
            var query = _instanceService.GetQuery().Where(i => i.Status == StatusConstants.Active);
            var result = query.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(new {Success = false, Message = exception.Message}, JsonRequestBehavior.AllowGet);
        }
    }
    #endregion
}