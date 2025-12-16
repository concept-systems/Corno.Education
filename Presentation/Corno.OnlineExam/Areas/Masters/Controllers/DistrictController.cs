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
    public class DistrictController : Controller
    {
        IMasterService _masterService;

        public DistrictController(IMasterService masterService)
        {
            _masterService = masterService;
        }

        // GET: /District/
         [Authorize]
        public ActionResult Index(int? page)
        {
            var viewModel = _masterService.DistrictRepository.Get().ToList().ToMappedList<District, DistrictViewModel>();
            return View(viewModel);
        }

        // GET: /District/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            District District = _masterService.DistrictRepository.GetByID(id);
            if (District == null)
            {
                return HttpNotFound();
            }
            return View(District);
        }

        // GET: /District/Create
        public ActionResult Create()
        {
            District District = new District();
            DistrictViewModel viewModel = new DistrictViewModel();
            viewModel.Countrys = _masterService.CountryRepository.Get().ToList();
            viewModel.States = _masterService.StateRepository.Get().ToList();

            return View(viewModel);
        }

        // POST: /District/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DistrictViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                District model = Mapper.Map<District>(viewModel);
                _masterService.DistrictRepository.Add(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            viewModel.Countrys = _masterService.CountryRepository.Get().ToList();
            viewModel.States = _masterService.StateRepository.Get().ToList();
            return View(viewModel);
        }

        // GET: /District/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            District model = _masterService.DistrictRepository.GetByID(id);
            DistrictViewModel viewModel = Mapper.Map<DistrictViewModel>(model);
            if (model == null)
            {
                return HttpNotFound();
            }
            viewModel.Countrys = _masterService.CountryRepository.Get().ToList();
            viewModel.States = _masterService.StateRepository.Get().ToList();
            return View(viewModel);
        }

        // POST: /District/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DistrictViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                District model = Mapper.Map<District>(viewModel);
                viewModel.ModifiedBy = User.Identity.GetUserId();
                viewModel.ModifiedDate = DateTime.Now;
                _masterService.DistrictRepository.Update(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            viewModel.Countrys = _masterService.CountryRepository.Get().ToList();
            viewModel.States = _masterService.StateRepository.Get().ToList();
            return View(viewModel);
        }

        // GET: /District/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            District model = _masterService.DistrictRepository.GetByID(id);
            DistrictViewModel viewModel = Mapper.Map<District, DistrictViewModel>(model);
            viewModel.Countrys = _masterService.CountryRepository.Get().ToList();
            viewModel.States = _masterService.StateRepository.Get().ToList();
            return View(viewModel);
        }

        // POST: /District/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            District District = _masterService.DistrictRepository.GetByID(id);
            District.DeletedBy = User.Identity.GetUserId();
            District.DeletedDate = DateTime.Now;

            District.Status = ValueConstants.Deleted;
            _masterService.DistrictRepository.Update(District);
            //_masterService.DistrictRepository.Delete(District);
            _masterService.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _masterService.Dispose(disposing);
            }
            base.Dispose(disposing);
        }
    }
}
