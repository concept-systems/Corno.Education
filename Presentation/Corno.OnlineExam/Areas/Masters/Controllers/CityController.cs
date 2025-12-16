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
    public class CityController : Controller
    {
        IMasterService _masterService;

        public CityController(IMasterService masterService)
        {
            _masterService = masterService;
        }

        // GET: /City/
         [Authorize]
        public ActionResult Index(int? page)
        {
            var viewModel = _masterService.CityRepository.Get().ToList().ToMappedList<City, CityViewModel>();
            return View(viewModel);
        }

        // GET: /City/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            City city = _masterService.CityRepository.GetByID(id);
            if (city == null)
            {
                return HttpNotFound();
            }
            return View(city);
        }

        // GET: /City/Create
        public ActionResult Create()
        {
            CityViewModel viewModel = new CityViewModel();
            viewModel.Countrys = _masterService.CountryRepository.Get().ToList();
            viewModel.States= _masterService.StateRepository.Get().ToList();
            viewModel.Districts = _masterService.DistrictRepository.Get().ToList();
            viewModel.Tehsils = _masterService.TehsilRepository.Get().ToList();
            return View(viewModel);
        }

        // POST: /City/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CityViewModel viewModel, City city)
        {
            if (ModelState.IsValid)
            {
                City model = Mapper.Map<City>(viewModel);
                _masterService.CityRepository.Add(city);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            viewModel.Countrys = _masterService.CountryRepository.Get().ToList();
            viewModel.States = _masterService.StateRepository.Get().ToList();
            viewModel.Districts = _masterService.DistrictRepository.Get().ToList();
            viewModel.Tehsils = _masterService.TehsilRepository.Get().ToList();
            return View(viewModel);
        }

        // GET: /City/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            City model = _masterService.CityRepository.GetByID(id);
            CityViewModel viewModel = Mapper.Map<CityViewModel>(model);
            if (model == null)
            {
                return HttpNotFound();
            }
            viewModel.Countrys = _masterService.CountryRepository.Get().ToList();
            viewModel.States = _masterService.StateRepository.Get().ToList();
            viewModel.Districts = _masterService.DistrictRepository.Get().ToList();
            viewModel.Tehsils = _masterService.TehsilRepository.Get().ToList(); 
            return View(viewModel);
        }

        // POST: /City/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CityViewModel viewModel, City city)
        {
            if (ModelState.IsValid)
            {
                City model = Mapper.Map<City>(viewModel);
                viewModel.ModifiedBy = User.Identity.GetUserId();
                viewModel.ModifiedDate = DateTime.Now;
                _masterService.CityRepository.Update(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            viewModel.Countrys = _masterService.CountryRepository.Get().ToList();
            viewModel.States = _masterService.StateRepository.Get().ToList();
            viewModel.Districts = _masterService.DistrictRepository.Get().ToList();
            viewModel.Tehsils = _masterService.TehsilRepository.Get().ToList(); return View(viewModel);
        }

        // GET: /City/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            City model = _masterService.CityRepository.GetByID(id);

            CityViewModel viewModel = Mapper.Map<CityViewModel>(model);
            viewModel.Countrys = _masterService.CountryRepository.Get().ToList();
            viewModel.States = _masterService.StateRepository.Get().ToList();
            viewModel.Districts = _masterService.DistrictRepository.Get().ToList();
            viewModel.Tehsils = _masterService.TehsilRepository.Get().ToList(); return View(viewModel);
        }

        // POST: /City/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            City city = _masterService.CityRepository.GetByID(id);
            city.DeletedBy = User.Identity.GetUserId();
            city.DeletedDate = DateTime.Now;

            city.Status = ValueConstants.Deleted;
            _masterService.CityRepository.Update(city);
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
