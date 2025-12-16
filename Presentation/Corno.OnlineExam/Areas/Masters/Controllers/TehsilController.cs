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
    public class TehsilController : Controller
    {
        IMasterService _masterService;

        public TehsilController(IMasterService masterService)
        {
            _masterService = masterService;
        }

        // GET: /Tehsil/
         [Authorize]
        public ActionResult Index(int? page)
        {
            var viewModel = _masterService.TehsilRepository.Get().ToList().ToMappedList<Tehsil, TehsilViewModel>();
            return View(viewModel);
        }

        // GET: /Tehsil/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tehsil Tehsil = _masterService.TehsilRepository.GetByID(id);
            if (Tehsil == null)
            {
                return HttpNotFound();
            }
            return View(Tehsil);
        }

        // GET: /Tehsil/Create
        public ActionResult Create()
        {
            Tehsil Tehsil = new Tehsil();
            TehsilViewModel viewModel = new TehsilViewModel();
            viewModel.Countrys = _masterService.CountryRepository.Get().ToList();
            viewModel.States = _masterService.StateRepository.Get().ToList();
            viewModel.Districts = _masterService.DistrictRepository.Get().ToList();
            viewModel.Citys = _masterService.CityRepository.Get().ToList();
            return View(viewModel);
        }

        // POST: /Tehsil/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(TehsilViewModel viewModel, Tehsil Tehsil)
        {
            if (ModelState.IsValid)
            {
                Tehsil model = Mapper.Map<Tehsil>(viewModel);
                _masterService.TehsilRepository.Add(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            viewModel.Countrys = _masterService.CountryRepository.Get().ToList();
            viewModel.States = _masterService.StateRepository.Get().ToList();
            viewModel.Districts = _masterService.DistrictRepository.Get().ToList();
            viewModel.Citys = _masterService.CityRepository.Get().ToList();
            return View(viewModel);
        }

        // GET: /Tehsil/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tehsil model = _masterService.TehsilRepository.GetByID(id);
            TehsilViewModel viewModel = Mapper.Map<TehsilViewModel>(model);
            if (model == null)
            {
                return HttpNotFound();
            }
            viewModel.Countrys = _masterService.CountryRepository.Get().ToList();
            viewModel.States = _masterService.StateRepository.Get().ToList();
            viewModel.Districts = _masterService.DistrictRepository.Get().ToList();
            viewModel.Citys = _masterService.CityRepository.Get().ToList();
            return View(viewModel);
        }

        // POST: /Tehsil/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(TehsilViewModel viewModel, Tehsil Tehsil)
        {
            if (ModelState.IsValid)
            {
                Tehsil model = Mapper.Map<Tehsil>(viewModel);
                viewModel.ModifiedBy = User.Identity.GetUserId();
                viewModel.ModifiedDate = DateTime.Now;
                _masterService.TehsilRepository.Update(Tehsil);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            viewModel.Countrys = _masterService.CountryRepository.Get().ToList();
            viewModel.States = _masterService.StateRepository.Get().ToList();
            viewModel.Districts = _masterService.DistrictRepository.Get().ToList();
            viewModel.Citys = _masterService.CityRepository.Get().ToList();
            return View(viewModel);
        }

        // GET: /Tehsil/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Tehsil model = _masterService.TehsilRepository.GetByID(id);
            TehsilViewModel viewModel = Mapper.Map<Tehsil, TehsilViewModel>(model);
            viewModel.Countrys = _masterService.CountryRepository.Get().ToList();
            viewModel.States = _masterService.StateRepository.Get().ToList();
            viewModel.Districts = _masterService.DistrictRepository.Get().ToList();
            viewModel.Citys = _masterService.CityRepository.Get().ToList();
            return View(viewModel);
        }

        // POST: /Tehsil/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Tehsil Tehsil = _masterService.TehsilRepository.GetByID(id);
            Tehsil.DeletedBy = User.Identity.GetUserId();
            Tehsil.DeletedDate = DateTime.Now;

            Tehsil.Status = ValueConstants.Deleted;
            _masterService.TehsilRepository.Update(Tehsil);
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
