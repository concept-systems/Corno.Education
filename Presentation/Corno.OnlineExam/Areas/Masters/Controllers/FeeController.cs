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
    public class FeeController : Controller
    {
        IMasterService _masterService;

        public FeeController(IMasterService masterService)
        {
            _masterService = masterService;
        }
        //
        // GET: /Fee/
         [Authorize]
        public ActionResult Index()
        {
            var viewModels = _masterService.FeeRepository.Get().ToList().ToMappedList<Fee, FeeViewModel>();
            return View(viewModels);
        }

        //
        // GET: /Fee/Details/5

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fee fee = _masterService.FeeRepository.GetByID(id);
            if (fee == null)
            {
                return HttpNotFound();
            }
            return View(fee);
        }

        //
        // GET: /Fee/Create

        public ActionResult Create()
        {
            FeeViewModel viewModel = new FeeViewModel();
            return View(viewModel);
        }

        //
        // POST: /Fee/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(FeeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Fee model = Mapper.Map<Fee>(viewModel);
                _masterService.FeeRepository.Add(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        //
        // GET: /Fee/Edit/5

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fee model = _masterService.FeeRepository.GetByID(id);
            FeeViewModel viewModel = Mapper.Map<FeeViewModel>(model);
            if (viewModel == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }

        //
        // POST: /Fee/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(FeeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Fee model = Mapper.Map<Fee>(viewModel);
                viewModel.ModifiedBy = User.Identity.GetUserId();
                viewModel.ModifiedDate = DateTime.Now;

                _masterService.FeeRepository.Update(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        //
        // GET: /Fee/Delete/5

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Fee model = _masterService.FeeRepository.GetByID(id);
            FeeViewModel viewModel = Mapper.Map<Fee, FeeViewModel>(model);
            return View(viewModel);
        }

        //
        // POST: /Fee/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Fee fee = _masterService.FeeRepository.GetByID(id);
            fee.DeletedBy = User.Identity.GetUserId();
            fee.DeletedDate = DateTime.Now;

            fee.Status = ValueConstants.Deleted;
            _masterService.FeeRepository.Update(fee);
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