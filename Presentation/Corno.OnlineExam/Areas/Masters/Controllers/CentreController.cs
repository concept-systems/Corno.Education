using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using OnlineExam.Models;
using OnlineExam.DAL.Classes;
using AutoMapper;
using Globals.Constants;
using System.Net;
using OnlineExam.Helpers;
using Microsoft.AspNet.Identity;
namespace OnlineExam.Controllers
{
    public class CentreController : Controller
    {
        IMasterService _masterService;

        public CentreController(IMasterService masterService)
        {
            _masterService = masterService;
        }

        //
        // GET: /Centre/
         [Authorize]
        public ActionResult Index(int? page)
        {
            var viewModel = _masterService.CentreRepository.Get().ToList().ToMappedList<Centre, CentreViewModel>();
            return View(viewModel);
        }

        //
        // GET: /Centre/Details/5

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Centre centre = _masterService.CentreRepository.GetByID(id);
            if (centre == null)
            {
                return HttpNotFound();
            }
            return View(centre);
        }

        //
        // GET: /Centre/Create

        public ActionResult Create()
        {
            CentreViewModel viewModel = new CentreViewModel();
            return View(viewModel);
        }

        //
        // POST: /Centre/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CentreViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Centre model = Mapper.Map<Centre>(viewModel);
                _masterService.CentreRepository.Add(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        //
        // GET: /Centre/Edit/5

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Centre model = _masterService.CentreRepository.GetByID(id);
            CentreViewModel viewModel = Mapper.Map<CentreViewModel>(model);
            if (model == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }

        //
        // POST: /Centre/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CentreViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Centre model = Mapper.Map<Centre>(viewModel);
                viewModel.ModifiedBy = User.Identity.GetUserId();
                viewModel.ModifiedDate = DateTime.Now;

                _masterService.CentreRepository.Update(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
         
            return View(viewModel);
        }

        //
        // GET: /Centre/Delete/5

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Centre model = _masterService.CentreRepository.GetByID(id);
            CentreViewModel viewModel = Mapper.Map<Centre, CentreViewModel>(model);
            
            return View(viewModel);
        }

        //
        // POST: /Centre/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Centre centre = _masterService.CentreRepository.GetByID(id);
            centre.DeletedBy = User.Identity.GetUserId();
            centre.DeletedDate = DateTime.Now;

            centre.Status = ValueConstants.Deleted;
            _masterService.CentreRepository.Update(centre);
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