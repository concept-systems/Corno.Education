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
    public class CountryController : Controller
    {
        IMasterService _masterService;

        public CountryController(IMasterService masterService)
        {
            _masterService = masterService;
        }

        // GET: /Country/
         [Authorize]
        public ActionResult Index(int? page)
        {
            var viewModel= _masterService.CountryRepository.Get().ToList().ToMappedList<Country,CountryViewModel>();
            return View(viewModel);
        }

        // GET: /Country/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Country country = _masterService.CountryRepository.GetByID(id);
            if (country == null)
            {
                return HttpNotFound();
            }
            return View(country);
        }

        // GET: /Country/Create
        public ActionResult Create()
        {
            CountryViewModel viewModel = new CountryViewModel();
            return View(viewModel);
        }

        // POST: /Country/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CountryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Country model=Mapper.Map<Country>(viewModel);
                _masterService.CountryRepository.Add(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        // GET: /Country/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Country model = _masterService.CountryRepository.GetByID(id);
            CountryViewModel viewModel =Mapper.Map<CountryViewModel>(model);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }

        // POST: /Country/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CountryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Country model=Mapper.Map<Country>(viewModel);
                viewModel.ModifiedBy = User.Identity.GetUserId();
                viewModel.ModifiedDate = DateTime.Now;

                _masterService.CountryRepository.Update(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        // GET: /Country/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Country model = _masterService.CountryRepository.GetByID(id);
            CountryViewModel viewModel =Mapper.Map<Country,CountryViewModel>(model);

            return View(viewModel);
        }

        // POST: /Country/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Country country = _masterService.CountryRepository.GetByID(id);
            country.DeletedBy = User.Identity.GetUserId();
            country.DeletedDate = DateTime.Now;

            country.Status = ValueConstants.Deleted;
            _masterService.CountryRepository.Update(country);

            //_masterService.CountryRepository.Delete(country);
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
