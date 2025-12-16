using OnlineExam.DAL.Classes;
using OnlineExam.Helpers;
using OnlineExam.Models;
using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using AutoMapper;
using System;
using Globals.Constants;
using Microsoft.AspNet.Identity;

namespace OnlineExam.Controllers
{
    public class CourseCategoryMapController : Controller
    {
        ITransactionService _registrationService;
        IMasterService _masterService;

        public CourseCategoryMapController( ITransactionService registrationService,IMasterService masterService)
        {
            _registrationService = registrationService;
            _masterService = masterService;
        }

         [Authorize]
        public ActionResult Index()
        {
            var viewModels = _masterService.CourseCategoryMapRepository.Get().ToList().ToMappedList<CourseCategoryMap, CourseCategoryMapViewModel>();
            return View(viewModels);
        }
        //
        // GET: /CourseCategoryMap/Details/5

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseCategoryMap CourseCategoryMap = _masterService.CourseCategoryMapRepository.GetByID(id);
            if (CourseCategoryMap == null)
            {
                return HttpNotFound();
            }
            return View(CourseCategoryMap);
        }

        //
        // GET: /CourseCategoryMap/Create

        public ActionResult Create()
        {
            CourseCategoryViewModel viewModel = new CourseCategoryViewModel();
            ViewData["SubjectCategorys"] = _masterService.SubjectCategoryRepository.Get().ToList();
            viewModel.Courses = _masterService.CourseRepository.Get().ToList();
            viewModel.Instances = _masterService.InstanceRepository.Get().ToList();

            return View(viewModel);
        }

        //
        // POST: /CourseCategoryMap/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CourseCategoryViewModel viewModel)
        {
             try
            {
                if (ModelState.IsValid)
                {
                    foreach (CourseCategoryMap category in viewModel.CourseCategoryMaps)
                    {
            
                CourseCategoryMap model = Mapper.Map<CourseCategoryMap>(viewModel);
                category.SubjectCategoryID = viewModel.SubjectCategoryID;
                category.CreatedBy = User.Identity.GetUserId();
                category.CreatedDate = DateTime.Now;
                _masterService.CourseCategoryMapRepository.Add(model);
                _masterService.Save();
                    }
                return RedirectToAction("Index");
            }
                 }
            catch
            {

            }
             ViewData["SubjectCategorys"] = _masterService.SubjectCategoryRepository.Get().ToList();
             viewModel.Courses = _masterService.CourseRepository.Get().ToList();
            viewModel.Instances = _masterService.InstanceRepository.Get().ToList();
            return View(viewModel);
           
             
        }

        //
        // GET: /CourseCategoryMap/Edit/5

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            //CourseCategoryViewModel viewModel = new CourseCategoryViewModel();
            //viewModel.SubjectCategoryID = (int)id;
            CourseCategoryMap model = _masterService.CourseCategoryMapRepository.GetByID(id); 
            CourseCategoryMapViewModel viewModel = Mapper.Map<CourseCategoryMapViewModel>(model);
            if (viewModel == null)
            {
                return HttpNotFound(); 
            }
            ViewData["SubjectCategorys"] = _masterService.SubjectCategoryRepository.Get().ToList();
            viewModel.Courses = _masterService.CourseRepository.Get().ToList();
            viewModel.Instances = _masterService.InstanceRepository.Get().ToList();
            return View(viewModel);
        }

        //
        // POST: /CourseCategoryMap/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CourseCategoryViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                foreach (CourseCategoryMap category in viewModel.CourseCategoryMaps)
                {
                    CourseCategoryMap model = Mapper.Map<CourseCategoryMap>(viewModel);
                    category.ModifiedBy = User.Identity.GetUserId();
                    category.ModifiedDate = DateTime.Now;

                    _masterService.CourseCategoryMapRepository.Update(model);
                    _masterService.Save();
                }
                return RedirectToAction("Index");
            }

            ViewData["SubjectCategorys"] = _masterService.SubjectCategoryRepository.Get().ToList();
            viewModel.Courses = _masterService.CourseRepository.Get().ToList();
            viewModel.Instances = _masterService.InstanceRepository.Get().ToList();
            return View(viewModel);
        }

        //
        // GET: /CourseCategoryMap/Delete/5

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CourseCategoryMap model = _masterService.CourseCategoryMapRepository.GetByID(id);
            CourseCategoryMapViewModel viewModel = Mapper.Map<CourseCategoryMap, CourseCategoryMapViewModel>(model);
            ViewData["SubjectCategorys"] = _masterService.SubjectCategoryRepository.Get().ToList();
            viewModel.Courses = _masterService.CourseRepository.Get().ToList();
            viewModel.Instances = _masterService.InstanceRepository.Get().ToList();
            return View(viewModel);
        }

        //
        // POST: /CourseCategoryMap/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CourseCategoryMap courseCategoryMap = _masterService.CourseCategoryMapRepository.GetByID(id);
            courseCategoryMap.DeletedBy = User.Identity.GetUserId();
            courseCategoryMap.DeletedDate = DateTime.Now;

            courseCategoryMap.Status = ValueConstants.Deleted;
            _masterService.CourseCategoryMapRepository.Update(courseCategoryMap);
            _masterService.Save();
            return RedirectToAction("Index");
        }
        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingInline_Create([DataSourceRequest] DataSourceRequest request, CourseCategoryMap CourseCategoryMap)
        {
            if (CourseCategoryMap != null && ModelState.IsValid)
            {
                //salesInvoiceDetail.Amount = salesInvoiceDetail.Quantity * salesInvoiceDetail.Rate;
            }

            return Json(new[] { CourseCategoryMap }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingInline_Update([DataSourceRequest] DataSourceRequest request, CourseCategoryMap CourseCategoryMap)
        {
            if (CourseCategoryMap != null && ModelState.IsValid)
            {
                //salesInvoiceDetail.Amount = salesInvoiceDetail.Quantity * salesInvoiceDetail.Rate;
            }

            return Json(new[] { CourseCategoryMap }.ToDataSourceResult(request, ModelState));
        }

        [AcceptVerbs(HttpVerbs.Post)]
        public ActionResult EditingInline_Destroy([DataSourceRequest] DataSourceRequest request, CourseCategoryMap CourseCategoryMap)
        {
            if (CourseCategoryMap != null)
            {
            }

            return Json(new[] { CourseCategoryMap }.ToDataSourceResult(request, ModelState));
        }


        protected override void Dispose(bool disposing)
        {
            _masterService.Dispose(disposing);
            base.Dispose(disposing);
        }
    }
}