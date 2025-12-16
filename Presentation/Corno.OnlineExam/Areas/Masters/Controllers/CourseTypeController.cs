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
    public class CourseTypeController : Controller
    {
        IMasterService _masterService;

        public CourseTypeController(IMasterService masterService)
        {
            _masterService = masterService;
        }

         [Authorize]
        public ActionResult Index()
        {
            var viewModels = _masterService.CourseTypeRepository.Get().ToList().ToMappedList<CourseType, CourseTypeViewModel>();
            return View(viewModels);
        }
        //
        // GET: /CourseType/Details/5

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseType courseType = _masterService.CourseTypeRepository.GetByID(id);
            if (courseType == null)
            {
                return HttpNotFound();
            }
            return View(courseType);
        }

        //
        // GET: /CourseType/Create

        public ActionResult Create()
        {
            CourseTypeViewModel viewModel = new CourseTypeViewModel();
            return View(viewModel);
        }

        //
        // POST: /CourseType/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CourseTypeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                CourseType model = Mapper.Map<CourseType>(viewModel);
                _masterService.CourseTypeRepository.Add(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        //
        // GET: /CourseType/Edit/5

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseType model = _masterService.CourseTypeRepository.GetByID(id);
            CourseTypeViewModel viewModel = Mapper.Map<CourseTypeViewModel>(model);
            if (viewModel == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }

        //
        // POST: /CourseType/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CourseTypeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                CourseType model = Mapper.Map<CourseType>(viewModel);

                viewModel.ModifiedBy = User.Identity.GetUserId();
                viewModel.ModifiedDate = DateTime.Now;

                _masterService.CourseTypeRepository.Update(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        //
        // GET: /CourseType/Delete/5

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseType model = _masterService.CourseTypeRepository.GetByID(id);
            CourseTypeViewModel viewModel = Mapper.Map<CourseType, CourseTypeViewModel>(model);
            return View(viewModel);
        }

        //
        // POST: /CourseType/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CourseType courseType = _masterService.CourseTypeRepository.GetByID(id);
            courseType.DeletedBy = User.Identity.GetUserId();
            courseType.DeletedDate = DateTime.Now;

            courseType.Status = ValueConstants.Deleted;
            _masterService.CourseTypeRepository.Update(courseType);
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