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
    public class ExamFeeController : Controller
    {
        IMasterService _masterService;

        public ExamFeeController(IMasterService masterService)
        {
            _masterService = masterService;
        }
        //
        // GET: /ExamFee/
         [Authorize]
        public ActionResult Index(int? page)
        {
            var viewModel = _masterService.ExamFeeRepository.Get().ToList().ToMappedList<ExamFee, ExamFeeViewModel>();
 
            return View(viewModel);
        }

        //
        // GET: /ExamFee/Details/5

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamFee ExamFee = _masterService.ExamFeeRepository.GetByID(id);
            if (ExamFee == null)
            {
                return HttpNotFound();
            }
            return View(ExamFee);
        }

        //
        // GET: /ExamFee/Create

        public ActionResult Create()
        {
            ExamFee ExamFee = new ExamFee();
            ExamFeeViewModel viewModel = new ExamFeeViewModel();
            //viewModel.Colleges = _masterService.CollegeRepository.Get().ToList();
            //viewModel.Facultys = _masterService.FacultyRepository.Get().ToList();
            //viewModel.Courses = _masterService.CourseRepository.Get().ToList();
            //viewModel.CourseParts = _masterService.CoursePartRepository.Get().ToList();

            return View(viewModel);
        }

        //
        // POST: /ExamFee/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ExamFeeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                ExamFee model = Mapper.Map<ExamFee>(viewModel);
                _masterService.ExamFeeRepository.Add(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            //viewModel.Colleges = _masterService.CollegeRepository.Get().ToList();
            //viewModel.Facultys = _masterService.FacultyRepository.Get().ToList();
            //viewModel.Courses = _masterService.CourseRepository.Get().ToList();
            //viewModel.CourseParts = _masterService.CoursePartRepository.Get().ToList();
            return View(viewModel);
        }

        //
        // GET: /ExamFee/Edit/5

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamFee model = _masterService.ExamFeeRepository.GetByID(id);
            ExamFeeViewModel viewModel = Mapper.Map<ExamFeeViewModel>(model);
            if (model == null)
            {
                return HttpNotFound();
            }
            //viewModel.Colleges = _masterService.CollegeRepository.Get().ToList();
            //viewModel.Facultys = _masterService.FacultyRepository.Get().ToList();
            //viewModel.Courses = _masterService.CourseRepository.Get().ToList();
            //viewModel.CourseParts = _masterService.CoursePartRepository.Get().ToList();
            return View(viewModel);
        }

        //
        // POST: /ExamFee/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ExamFeeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                ExamFee model = Mapper.Map<ExamFee>(viewModel);
                viewModel.ModifiedBy = User.Identity.GetUserId();
                viewModel.ModifiedDate = DateTime.Now;
                _masterService.ExamFeeRepository.Update(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            //viewModel.Colleges = _masterService.CollegeRepository.Get().ToList();
            //viewModel.Facultys = _masterService.FacultyRepository.Get().ToList();
            //viewModel.Courses = _masterService.CourseRepository.Get().ToList();
            //viewModel.CourseParts = _masterService.CoursePartRepository.Get().ToList();
            return View(viewModel);
        }

        //
        // GET: /ExamFee/Delete/5

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamFee model = _masterService.ExamFeeRepository.GetByID(id);
            ExamFeeViewModel viewModel = Mapper.Map<ExamFee, ExamFeeViewModel>(model);
            //viewModel.Colleges = _masterService.CollegeRepository.Get().ToList();
            //viewModel.Facultys = _masterService.FacultyRepository.Get().ToList();
            //viewModel.Courses = _masterService.CourseRepository.Get().ToList();
            //viewModel.CourseParts = _masterService.CoursePartRepository.Get().ToList();
            return View(viewModel);
        }

        //
        // POST: /ExamFee/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ExamFee ExamFee = _masterService.ExamFeeRepository.GetByID(id);
            ExamFee.DeletedBy = User.Identity.GetUserId();
            ExamFee.DeletedDate = DateTime.Now;

            ExamFee.Status = ValueConstants.Deleted;
            _masterService.ExamFeeRepository.Update(ExamFee);
            //_masterService.DistrictRepository.Delete(District);
            _masterService.Save();
            return RedirectToAction("Index");
        }

        public JsonResult GetCascadeCourses(int? facultyID, string courseFilter)
        {
            facultyID = (int)facultyID;
            //var courses = _masterService.CourseRepository.Get().Where(p => p.FacultyID == facultyID).ToList();

            if (!string.IsNullOrEmpty(courseFilter))
            {
                //courses = _masterService.CourseRepository.Get().Where(p => p.Name == courseFilter).ToList();
            }

            //return Json(courses.Select(p => new { ID = p.ID, Name = p.Name }), JsonRequestBehavior.AllowGet);
            return null;
        }

        public JsonResult GetCascadeCourseParts(int? courseID, string coursePartFilter)
        {
            courseID = (int)courseID;

            //var courseParts = _masterService.CoursePartRepository.Get().Where(p => p.CourseID == courseID).ToList();

            if (!string.IsNullOrEmpty(coursePartFilter))
            {
                //courseParts = _masterService.CoursePartRepository.Get().Where(p => p.Name == coursePartFilter).ToList();
            }

            //return Json(courseParts.Select(p => new { ID = p.ID, Name = p.Name }), JsonRequestBehavior.AllowGet);
            return null;
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