using Corno.Globals.Constants;
using Corno.OnlineExam.Helpers;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Corno.Data.Corno;
using Corno.OnlineExam.Controllers;
using Corno.Services.Corno.Interfaces;

namespace Corno.OnlineExam.Areas.Transactions.Controllers;

[Authorize]
public class ComplaintController : BaseController
{
    readonly IMasterService _masterService;
    public ComplaintController(IMasterService masterService)
    {
        _masterService = masterService;
    }

    //
    // GET: /Bos/
    [Authorize]
    public ActionResult Index()
    {
        var viewModels = _masterService.ComplaintRepository.Get().ToList().ToMappedList<Complaint, ComplaintViewModel>();
        return View(viewModels);
    }

    //
    // GET: /Bos/Details/5
    [Authorize]
    public ActionResult Details(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        Complaint complaint = _masterService.ComplaintRepository.GetById(id);
        ComplaintViewModel viewModel = AutoMapperConfig.CornoMapper.Map<ComplaintViewModel>(complaint);

        if (viewModel == null)
        {
            return HttpNotFound();
        }
        //viewModel.Staffs = _masterService.StaffRepository.Get().ToList();
        return View(viewModel);
    }

    //
    // GET: /Bos/Create
    [Authorize]
    public ActionResult Create()
    {
        ComplaintViewModel viewModel = new ComplaintViewModel();
        //viewModel.Staffs = _masterService.StaffRepository.Get().ToList();

        return View(viewModel);
    }

    //
    // POST: /Bos/Create
    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(ComplaintViewModel viewModel)
    {
        if (ModelState.IsValid)
        {
            Complaint model = AutoMapperConfig.CornoMapper.Map<Complaint>(viewModel);
            _masterService.ComplaintRepository.Add(model);
            _masterService.Save();
            return RedirectToAction("Index");
        }
        //viewModel.Staffs = _masterService.StaffRepository.Get().ToList();
        return View(viewModel);
    }

    //
    // GET: /Bos/Edit/5
    [Authorize]
    public ActionResult Edit(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        Complaint model = _masterService.ComplaintRepository.GetById(id);
        ComplaintViewModel viewModel = AutoMapperConfig.CornoMapper.Map<ComplaintViewModel>(model);
        if (viewModel == null)
        {
            return HttpNotFound();
        }
        //viewModel.Staffs = _masterService.StaffRepository.Get().ToList();
        return View(viewModel);
    }

    //
    // POST: /Bos/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public ActionResult Edit(ComplaintViewModel viewModel)
    {
        if (!ModelState.IsValid) return View(viewModel);

        Complaint model = AutoMapperConfig.CornoMapper.Map<Complaint>(viewModel);
        viewModel.ModifiedBy = User.Identity.GetUserId();
        viewModel.ModifiedDate = DateTime.Now;

        _masterService.ComplaintRepository.Update(model);
        _masterService.Save();
        return RedirectToAction("Index");
        //viewModel.Staffs = _masterService.StaffRepository.Get().ToList();
    }

    //
    // GET: /Bos/Delete/5
    [Authorize]
    public ActionResult Delete(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        Complaint model = _masterService.ComplaintRepository.GetById(id);
        ComplaintViewModel viewModel = AutoMapperConfig.CornoMapper.Map<Complaint, ComplaintViewModel>(model);
        //viewModel.Staffs = _masterService.StaffRepository.Get().ToList();
        return View(viewModel);
    }

    //
    // POST: /Bos/Delete/5

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public ActionResult DeleteConfirmed(int id)
    {
        Complaint complaint = _masterService.ComplaintRepository.GetById(id);
        complaint.DeletedBy = User.Identity.GetUserId();
        complaint.DeletedDate = DateTime.Now;

        complaint.Status = StatusConstants.Deleted;
        _masterService.ComplaintRepository.Update(complaint);
        _masterService.Save();
        return RedirectToAction("Index");
    }

    //protected override void Dispose(bool disposing)
    //{
    //    _masterService.Dispose(disposing);
    //    base.Dispose(disposing);
    //}
}