using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Corno.Data.Corno;
using Corno.Data.ViewModels.Admin;
using Corno.Globals.Constants;
using Corno.OnlineExam.Controllers;
using Corno.Services.Corno.Admin.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Corno.OnlineExam.Areas.Admin.Controllers;

public class UserController : CornoController
{
    #region -- Constructors --
    public UserController(IUserService userService)
    {
        _userService = userService;

        const string viewPath = "~/Areas/admin/views/User/";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";
        _changePasswordPath = $"{viewPath}/ChangePassword.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly IUserService _userService;

    private readonly string _indexPath;
    private readonly string _createPath;
    private readonly string _editPath;
    private readonly string _changePasswordPath;

    #endregion

    #region -- Actions --
    [Authorize]
    public ActionResult Index(int? page)
    {
        return View(_indexPath, null);
    }

    public ActionResult Create()
    {
        return View(_createPath, new UserCrudViewModel());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(UserCrudViewModel viewModel)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_createPath, viewModel);

            // Create user.
            _userService.Create(viewModel);
            
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

        var viewModel = _userService.GetViewModelWithRoles(id);
 
        return View(_editPath, viewModel);
    }

    // POST: /AspNetUser/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(UserCrudViewModel viewModel)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_editPath, viewModel);

            // Edit user
            _userService.Edit(viewModel);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_editPath, viewModel);
    }

    public ActionResult ChangePassword(string id)
    {
        if (null == id)
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);

        var viewModel = _userService.GetViewModelWithRoles(id);

        return View(_changePasswordPath, viewModel);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult ChangePassword(UserCrudViewModel viewModel)
    {
        try
        {
            // ChangePassword
            _userService.ChangePassword(viewModel);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_changePasswordPath, viewModel);
    }

    public ActionResult GetIndexModels([DataSourceRequest] DataSourceRequest request)
    {
        var query = _userService.GetQuery()
            .Where(p => p.LastName != RoleConstants.Student)
            .Select(p => new UserIndexViewModel
            {
                Id = p.Id,
                Type = p.Type,
                UserName = p.UserName,
                FirstName = p.FirstName,
                LastName = p.LastName,
                Email = p.Email,
                PhoneNumber = p.PhoneNumber,
                Locked = p.LockoutEnabled
            });
        var result = query.ToDataSourceResult(request);
        return Json(result, JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Inline_Create_Update_Destroy([DataSourceRequest] DataSourceRequest request, LinkDetail model)
    {
        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    }

    #endregion
}