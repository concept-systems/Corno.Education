using System;
using System.Net;
using System.Web.Mvc;
using Corno.Data.ViewModels.Admin;
using Corno.OnlineExam.Controllers;
using Corno.Services.Corno.Admin.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Mapster;

namespace Corno.OnlineExam.Areas.Admin.Controllers;

public class RoleController : CornoController
{
    #region -- Constructors --
    public RoleController(IRoleService roleService)
    {
        _roleService = roleService;

        const string viewPath = "~/Areas/admin/views/Role/";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly IRoleService _roleService;

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

    public ActionResult Create()
    {
        return View(_createPath, new RoleViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(RoleViewModel viewModel)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_createPath, viewModel);

            // Create Role
            _roleService.Create(viewModel);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, viewModel);
    }

    public ActionResult Edit(string id)
    {
        if (null == id)
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        // Get AspNetRole
        var role = _roleService.GetById(id);

        var viewModel = new RoleViewModel();
        role.Adapt(viewModel);
         
        return View(_editPath, viewModel);
    }

    // POST: /AspNetRole/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(RoleViewModel viewModel)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_editPath, viewModel);

            // Edit role
            _roleService.Edit(viewModel);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_editPath, viewModel);
    }

        
    public ActionResult GetIndexModels([DataSourceRequest] DataSourceRequest request)
    {
        var query = _roleService.GetQuery();
        var result = query.ToDataSourceResult(request);
        return Json(result, JsonRequestBehavior.AllowGet);
    }

    #endregion
}