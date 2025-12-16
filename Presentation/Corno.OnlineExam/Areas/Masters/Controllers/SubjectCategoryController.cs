using OnlineExam.DAL.Classes;
using OnlineExam.Helpers;
using OnlineExam.Models;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using System;
using Globals.Constants;
using Microsoft.AspNet.Identity;
using System.Web;

namespace OnlineExam.Controllers
{
    public class SubjectCategoryController : Controller
    {
        IMasterService _masterService;

        public SubjectCategoryController(IMasterService masterService)
        {
            _masterService = masterService;
        }

         [Authorize]
        public ActionResult Index()
        {
            var viewModels = _masterService.SubjectCategoryRepository.Get().ToList().ToMappedList<SubjectCategory, SubjectCategoryViewModel>();
            return View(viewModels);
        }

        //
        // GET: /Branch/Details/5

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubjectCategory subjectCategory = _masterService.SubjectCategoryRepository.GetByID(id);
            if (subjectCategory == null)
            {
                return HttpNotFound();
            }
            return View(subjectCategory);
        }

        //
        // GET: /Branch/Create

        public ActionResult Create()
        {
            SubjectCategoryViewModel viewModel = new SubjectCategoryViewModel();
           
            return View(viewModel);
        }

        //
        // POST: /Branch/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(SubjectCategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                SubjectCategory model = Mapper.Map<SubjectCategory>(viewModel);
                _masterService.SubjectCategoryRepository.Add(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
           
            return View(viewModel);
        }

        //
        // GET: /Branch/Edit/5

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubjectCategory model = _masterService.SubjectCategoryRepository.GetByID(id);
            SubjectCategoryViewModel viewModel = Mapper.Map<SubjectCategoryViewModel>(model);
            if (viewModel == null)
            {
                return HttpNotFound();
            }
            
            return View(viewModel);
        }

        //
        // POST: /Branch/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(SubjectCategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                SubjectCategory model = Mapper.Map<SubjectCategory>(viewModel);
                viewModel.ModifiedBy = User.Identity.GetUserId();
                viewModel.ModifiedDate = DateTime.Now;

                _masterService.SubjectCategoryRepository.Update(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            
            return View(viewModel);
        }

        //
        // GET: /Branch/Delete/5

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            SubjectCategory model = _masterService.SubjectCategoryRepository.GetByID(id);
            SubjectCategoryViewModel viewModel = Mapper.Map<SubjectCategory, SubjectCategoryViewModel>(model);
            
            return View(viewModel);
        }

        //
        // POST: /Branch/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            SubjectCategory subjectCategory = _masterService.SubjectCategoryRepository.GetByID(id);
            subjectCategory.DeletedBy = User.Identity.GetUserId();
            subjectCategory.DeletedDate = DateTime.Now;

            subjectCategory.Status = ValueConstants.Deleted;
            _masterService.SubjectCategoryRepository.Update(subjectCategory);
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