using Corno.Data.Admin;
using System.Net;
using System.Web.Mvc;
using Corno.Services.Login.Interfaces;

namespace Corno.OnlineExam.Areas.Admin.Controllers;

[Authorize]
public class AspNetRoleController : Controller
{
    private readonly IAspNetRoleService _aspnetroleService;

    public AspNetRoleController(IAspNetRoleService aspnetroleService)
    {
        _aspnetroleService = aspnetroleService;
    }

    // GET: /AspNetRole/
    [Authorize(Roles = "Admin")]
    [Authorize]
    public ActionResult Index()
    {
        return View(_aspnetroleService.AspNetRoleRepository.Get());
    }

    // GET: /AspNetRole/Create
    [Authorize(Roles = "Admin")]
    public ActionResult Create()
    {
        return View(new AspNetRole());
    }

    // POST: /AspNetRole/Create
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(AspNetRole aspNetRole)
    {
        if (!ModelState.IsValid) return View(aspNetRole);

        var identityManager = new IdentityManager();
        identityManager.CreateRole(aspNetRole.Name);
        return RedirectToAction("Index");
    }

    // GET: /AspNetRole/Edit/5
    [Authorize(Roles = "Admin")]
    public ActionResult Edit(string id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        var aspnetrole = _aspnetroleService.AspNetRoleRepository.GetById(id);
        if (aspnetrole == null)
        {
            return HttpNotFound();
        }
        return View(aspnetrole);
    }

    // POST: /AspNetRole/Edit/5
    // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
    // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Edit(AspNetRole aspNetRole)
    {
        if (!ModelState.IsValid) return View(aspNetRole);
        //IdentityManager identityManager = new IdentityManager();
        //identityManager.EditRole(aspNetRole);

        _aspnetroleService.AspNetRoleRepository.Update(aspNetRole);
        _aspnetroleService.Save();

        return RedirectToAction("Index");
    }

    // GET: /AspNetRole/Delete/5
    [Authorize(Roles = "Admin")]
    public ActionResult Delete(string id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        var aspnetrole = _aspnetroleService.AspNetRoleRepository.GetById(id);
        if (aspnetrole == null)
        {
            return HttpNotFound();
        }
        return View(aspnetrole);
    }

    // POST: /AspNetRole/Delete/5
    [Authorize(Roles = "Admin")]
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteConfirmed(AspNetRole aspNetRole)
    {
        if (!ModelState.IsValid) return View(aspNetRole);
        var identityManager = new IdentityManager();
        identityManager.DeleteRole(aspNetRole);

        return RedirectToAction("Index");
    }

    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _aspnetroleService.Dispose(true);
        }
        base.Dispose(disposing);
    }
}