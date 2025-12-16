using OnlineExam.DAL.Classes;
using OnlineExam.Models;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using System;
using Globals.Constants;
using Microsoft.AspNet.Identity;
using AutoMapper;
using OnlineExam.Helpers;

namespace OnlineExam.Controllers
{
    public class College_Course_MapController : Controller
    {
        IMasterService _masterService;

        public College_Course_MapController(IMasterService masterService)
        {
            _masterService = masterService;
        }

        // GET: /College_Course_Map/
         [Authorize]
        public ActionResult Index(int? page)
        {
            var viewModel = _masterService.College_Course_MapRepository.Get().ToList().ToMappedList<College_Course_Map, College_Course_MapViewModel>();
            return View(viewModel);
        }

        // GET: /College_Course_Map/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            College_Course_Map College_Course_Map = _masterService.College_Course_MapRepository.GetByID(id);
            if (College_Course_Map == null)
            {
                return HttpNotFound();
            }
            return View(College_Course_Map);
        }

        // GET: /College_Course_Map/Create
        public ActionResult Create()
        {
            College_Course_MapViewModel viewModel = new College_Course_MapViewModel();
            viewModel.Colleges = _masterService.CollegeRepository.Get().ToList();
            viewModel.Courses = _masterService.CourseRepository.Get().ToList();
            return View(viewModel);
        }

        // POST: /College_Course_Map/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(College_Course_MapViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                College_Course_Map model = Mapper.Map<College_Course_Map>(viewModel);
                _masterService.College_Course_MapRepository.Add(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            viewModel.Colleges = _masterService.CollegeRepository.Get().ToList();
            viewModel.Courses = _masterService.CourseRepository.Get().ToList();
            return View(viewModel);
        }

        // GET: /College_Course_Map/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            College_Course_Map model = _masterService.College_Course_MapRepository.GetByID(id);
            College_Course_MapViewModel viewModel = Mapper.Map<College_Course_MapViewModel>(model);
            if (model == null)
            {
                return HttpNotFound();
            }
            viewModel.Colleges = _masterService.CollegeRepository.Get().ToList();
            viewModel.Courses = _masterService.CourseRepository.Get().ToList();
            return View(viewModel);
        }

        // POST: /College_Course_Map/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(College_Course_MapViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                College_Course_Map model = Mapper.Map<College_Course_Map>(viewModel);
                viewModel.ModifiedBy = User.Identity.GetUserId();
                viewModel.ModifiedDate = DateTime.Now;

                _masterService.College_Course_MapRepository.Update(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            viewModel.Colleges = _masterService.CollegeRepository.Get().ToList();
            viewModel.Courses = _masterService.CourseRepository.Get().ToList();
            return View(viewModel);
        }

        // GET: /College_Course_Map/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            College_Course_Map model = _masterService.College_Course_MapRepository.GetByID(id);
            College_Course_MapViewModel viewModel = Mapper.Map<College_Course_Map, College_Course_MapViewModel>(model);
            viewModel.Colleges = _masterService.CollegeRepository.Get().ToList();
            viewModel.Courses = _masterService.CourseRepository.Get().ToList();
            return View(viewModel);
        }

        // POST: /College_Course_Map/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            College_Course_Map college_Course_Map = _masterService.College_Course_MapRepository.GetByID(id);
            college_Course_Map.DeletedBy = User.Identity.GetUserId();
            college_Course_Map.DeletedDate = DateTime.Now;

            college_Course_Map.Status = ValueConstants.Deleted;
            _masterService.College_Course_MapRepository.Update(college_Course_Map);

            //_masterService.College_Course_MapRepository.Delete(College_Course_Map);
            _masterService.Save();
            //viewModel.Colleges = _masterService.CollegeRepository.Get().ToList();
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
