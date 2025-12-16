using AutoMapper;
using Corno.Data.Masters;
using Corno.Globals.Constants;
using Corno.OnlineExam.Helpers;
using Microsoft.AspNet.Identity;
using System;
using System.Net;
using System.Web.Mvc;
using Corno.Services.Base.Interfaces;

namespace Corno.OnlineExam.Areas.Masters.Controllers
{
    public class ExamScheduleController : Controller
    {
        IMasterService _masterService;
        public ExamScheduleController(IMasterService masterService)
        {
            _masterService = masterService;
        }
        //
        // GET: /ExamSchedule/
         [Authorize]
        public ActionResult Index()
        {
            var viewModel = _masterService.ExamScheduleRepository.Get().ToMappedList<ExamSchedule, ExamScheduleViewModel>();
            return View(viewModel);
        }

        //
        // GET: /ExamSchedule/Details/5

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamSchedule examSchedule = _masterService.ExamScheduleRepository.GetByID(id);

            if (examSchedule == null)
            {
                return HttpNotFound();
            }
            return View(examSchedule);
        }

        //
        // GET: /ExamSchedule/Create
         [Authorize]
        public ActionResult Create()
        {
            ExamScheduleViewModel viewModel = new ExamScheduleViewModel();
            //viewModel.CourseParts = _masterService.CoursePartRepository.Get().ToList();
            //viewModel.ExamFees = _masterService.ExamFeeRepository.Get().ToList();
            //viewModel.Centres = _masterService.CentreRepository.Get().ToList();
            return View(viewModel);
        }

        //
        // POST: /ExamSchedule/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ExamScheduleViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                ExamSchedule model = Mapper.Map<ExamSchedule>(viewModel);
                _masterService.ExamScheduleRepository.Add(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            //viewModel.CourseParts = _masterService.CoursePartRepository.Get().ToList();
            // viewModel.ExamFees = _masterService.ExamFeeRepository.Get().ToList();
            //viewModel.Centres = _masterService.CentreRepository.Get().ToList();
            return View(viewModel);
        }

        //
        // GET: /ExamSchedule/Edit/5

        public ActionResult Edit(int? id)
        {

            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamSchedule model = _masterService.ExamScheduleRepository.GetByID(id);
            ExamScheduleViewModel viewModel = Mapper.Map<ExamScheduleViewModel>(model);
            if (model == null)
            {
                return HttpNotFound();
            }
            //viewModel.CourseParts = _masterService.CoursePartRepository.Get().ToList();
            // viewModel.ExamFees = _masterService.ExamFeeRepository.Get().ToList();
            //viewModel.Centres = _masterService.CentreRepository.Get().ToList();
            return View(viewModel);
        }

        //
        // POST: /ExamSchedule/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(ExamScheduleViewModel viewModel)
        {

            if (ModelState.IsValid)
            {
                ExamSchedule model = Mapper.Map<ExamSchedule>(viewModel);
                viewModel.ModifiedBy = User.Identity.GetUserId();
                viewModel.ModifiedDate = DateTime.Now;
                _masterService.ExamScheduleRepository.Update(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            //viewModel.CourseParts = _masterService.CoursePartRepository.Get().ToList();
            //  viewModel.ExamFees = _masterService.ExamFeeRepository.Get().ToList();
            //viewModel.Centres = _masterService.CentreRepository.Get().ToList();
            return View(viewModel);
        }

        //
        // GET: /ExamSchedule/Delete/5

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ExamSchedule model = _masterService.ExamScheduleRepository.GetByID(id);

            ExamScheduleViewModel viewModel = Mapper.Map<ExamScheduleViewModel>(model);
            //viewModel.CourseParts = _masterService.CoursePartRepository.Get().ToList();
            //viewModel.ExamFees = _masterService.ExamFeeRepository.Get().ToList();
            //viewModel.Centres = _masterService.CentreRepository.Get().ToList();
            return View(viewModel);
        }

        //
        // POST: /ExamSchedule/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ExamSchedule model = _masterService.ExamScheduleRepository.GetByID(id);
            model.DeletedBy = User.Identity.GetUserId();
            model.DeletedDate = DateTime.Now;
            model.Status = ValueConstants.Deleted;
            _masterService.ExamScheduleRepository.Update(model);
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