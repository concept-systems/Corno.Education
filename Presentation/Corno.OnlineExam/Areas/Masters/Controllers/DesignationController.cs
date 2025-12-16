using AutoMapper;
using Corno.Data.Masters;
using Corno.Globals.Constants;
using Corno.OnlineExam.Helpers;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Corno.Services.Base.Interfaces;

namespace Corno.OnlineExam.Areas.Masters.Controllers
{
    public class DesignationController : Controller
    {
        IMasterService _masterService;

        public DesignationController(IMasterService masterService)
        {
            _masterService = masterService;
        }

         [Authorize]
        public ActionResult Index()
        {
            var viewModels = _masterService.DesignationRepository.Get().ToList().ToMappedList<Designation, DesignationViewModel>();
            return View(viewModels);
        }
        //
        // GET: /Designation/Details/5

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Designation designation = _masterService.DesignationRepository.GetByID(id);
            if (designation == null)
            {
                return HttpNotFound();
            }
            return View(designation);
        }

        //
        // GET: /Designation/Create

        public ActionResult Create()
        {
            DesignationViewModel viewModel = new DesignationViewModel();
            return View(viewModel);
        }

        //
        // POST: /Designation/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DesignationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Designation model = Mapper.Map<Designation>(viewModel);
                _masterService.DesignationRepository.Add(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        //
        // GET: /Designation/Edit/5

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Designation model = _masterService.DesignationRepository.GetByID(id);
            DesignationViewModel viewModel = Mapper.Map<DesignationViewModel>(model);
            if (viewModel == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }

        //
        // POST: /Designation/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DesignationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Designation model = Mapper.Map<Designation>(viewModel);

                viewModel.ModifiedBy = User.Identity.GetUserId();
                viewModel.ModifiedDate = DateTime.Now;

                _masterService.DesignationRepository.Update(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        //
        // GET: /Designation/Delete/5

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Designation model = _masterService.DesignationRepository.GetByID(id);
            DesignationViewModel viewModel = Mapper.Map<Designation, DesignationViewModel>(model);
            return View(viewModel);
        }

        //
        // POST: /Designation/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Designation designation = _masterService.DesignationRepository.GetByID(id);
            designation.DeletedBy = User.Identity.GetUserId();
            designation.DeletedDate = DateTime.Now;

            designation.Status = ValueConstants.Deleted;
            _masterService.DesignationRepository.Update(designation);
            _masterService.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            _masterService.Dispose(disposing);
            base.Dispose(disposing);
        }
    }
}