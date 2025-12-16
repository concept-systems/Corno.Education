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
    public class DegreeController : Controller
    {
        IMasterService _masterService;

        public DegreeController(IMasterService masterService)
        {
            _masterService = masterService;
        }

         [Authorize]
        public ActionResult Index()
        {
            var viewModels = _masterService.DegreeRepository.Get().ToList().ToMappedList<Degree, DegreeViewModel>();
            return View(viewModels);
        }
        //
        // GET: /Degree/Details/5

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Degree degree = _masterService.DegreeRepository.GetByID(id);
            if (degree == null)
            {
                return HttpNotFound();
            }
            return View(degree);
        }

        //
        // GET: /Degree/Create

        public ActionResult Create()
        {
            DegreeViewModel viewModel = new DegreeViewModel();
            return View(viewModel);
        }

        //
        // POST: /Degree/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DegreeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Degree model = Mapper.Map<Degree>(viewModel);
                _masterService.DegreeRepository.Add(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        //
        // GET: /Degree/Edit/5

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Degree model = _masterService.DegreeRepository.GetByID(id);
            DegreeViewModel viewModel = Mapper.Map<DegreeViewModel>(model);
            if (viewModel == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }

        //
        // POST: /Degree/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DegreeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Degree model = Mapper.Map<Degree>(viewModel);

                viewModel.ModifiedBy = User.Identity.GetUserId();
                viewModel.ModifiedDate = DateTime.Now;

                _masterService.DegreeRepository.Update(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        //
        // GET: /Degree/Delete/5

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Degree model = _masterService.DegreeRepository.GetByID(id);
            DegreeViewModel viewModel = Mapper.Map<Degree, DegreeViewModel>(model);
            return View(viewModel);
        }

        //
        // POST: /Degree/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Degree degree = _masterService.DegreeRepository.GetByID(id);
            degree.DeletedBy = User.Identity.GetUserId();
            degree.DeletedDate = DateTime.Now;

            degree.Status = ValueConstants.Deleted;
            _masterService.DegreeRepository.Update(degree);
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