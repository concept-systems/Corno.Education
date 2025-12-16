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
    public class DepartmentController : Controller
    {
        IMasterService _masterService;

        public DepartmentController(IMasterService masterService)
        {
            _masterService = masterService;
        }

         [Authorize]
        public ActionResult Index()
        {
            var viewModels = _masterService.DepartmentRepository.Get().ToList().ToMappedList<Department, DepartmentViewModel>();
            return View(viewModels);
        }
        //
        // GET: /Department/Details/5

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department department = _masterService.DepartmentRepository.GetByID(id);
            if (department == null)
            {
                return HttpNotFound();
            }
            return View(department);
        }

        //
        // GET: /Department/Create

        public ActionResult Create()
        {
            DepartmentViewModel viewModel = new DepartmentViewModel();
            return View(viewModel);
        }

        //
        // POST: /Department/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(DepartmentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Department model = Mapper.Map<Department>(viewModel);
                _masterService.DepartmentRepository.Add(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }

            return View(viewModel);
        }

        //
        // GET: /Department/Edit/5

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department model = _masterService.DepartmentRepository.GetByID(id);
            DepartmentViewModel viewModel = Mapper.Map<DepartmentViewModel>(model);
            if (viewModel == null)
            {
                return HttpNotFound();
            }
            return View(viewModel);
        }

        //
        // POST: /Department/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(DepartmentViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Department model = Mapper.Map<Department>(viewModel);

                viewModel.ModifiedBy = User.Identity.GetUserId();
                viewModel.ModifiedDate = DateTime.Now;

                _masterService.DepartmentRepository.Update(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            return View(viewModel);
        }

        //
        // GET: /Department/Delete/5

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Department model = _masterService.DepartmentRepository.GetByID(id);
            DepartmentViewModel viewModel = Mapper.Map<Department, DepartmentViewModel>(model);
            return View(viewModel);
        }

        //
        // POST: /Department/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Department department = _masterService.DepartmentRepository.GetByID(id);
            department.DeletedBy = User.Identity.GetUserId();
            department.DeletedDate = DateTime.Now;

            department.Status = ValueConstants.Deleted;
            _masterService.DepartmentRepository.Update(department);
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