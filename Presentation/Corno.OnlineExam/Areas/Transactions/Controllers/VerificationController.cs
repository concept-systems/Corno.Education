using AutoMapper;
using Corno.Data.Transactions;
using Corno.Globals.Constants;
using Corno.OnlineExam.Controllers;
using Corno.OnlineExam.Helpers;
using Corno.Services.Base.Interfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Net;
using System.Web.Mvc;
using Corno.Services.Corno.Interfaces;

namespace Corno.OnlineExam.Areas.Transactions.Controllers
{
    [Authorize]
    public class VerificationController : BaseController
    {
        private readonly IMasterService _masterService;
        public VerificationController(IMasterService masterService)
        {
            _masterService = masterService;
        }

        [Authorize]
        public ActionResult Index()
        {
            var viewModels = _masterService.VerificationRepository.Get().ToList().ToMappedList<Verification, VerificationViewModel>();
            return View(viewModels);
        }

        [Authorize]
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var verification = _masterService.VerificationRepository.GetByID(id);
            if (verification == null)
            {
                return HttpNotFound();
            }
            return View(verification);
        }

        [Authorize]
        public ActionResult Create()
        {
            var viewModel = new VerificationViewModel
            {
                Tehsils = _masterService.TehsilRepository.Get().ToList(),
                Citys = _masterService.CityRepository.Get().ToList(),
                Districts = _masterService.DistrictRepository.Get().ToList(),
                States = _masterService.StateRepository.Get().ToList(),
                Countrys = _masterService.CountryRepository.Get().ToList()
            };
            //viewModel.Courses = _masterService.CourseRepository.Get().ToList();
            //viewModel.CourseParts = _masterService.CoursePartRepository.Get().ToList();
            //viewModel.Branchs= _masterService.BranchRepository.Get().ToList();
            //viewModel.Subjects = _masterService.SubjectRepository.Get().ToList();

            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Create(VerificationViewModel viewModel)
        {
            try
            {

                if (ModelState.IsValid)
                {

                    Verification model = Mapper.Map<Verification>(viewModel);
                    _masterService.VerificationRepository.Add(model);
                    _masterService.Save();
                    return RedirectToAction("Index");
                }
                //viewModel.Courses = _masterService.CourseRepository.Get().ToList();
                //viewModel.CourseParts = _masterService.CoursePartRepository.Get().ToList();
                //viewModel.Branchs = _masterService.BranchRepository.Get().ToList();
                viewModel.Tehsils = _masterService.TehsilRepository.Get().ToList();
                viewModel.Citys = _masterService.CityRepository.Get().ToList();
                viewModel.Districts = _masterService.DistrictRepository.Get().ToList();
                viewModel.States = _masterService.StateRepository.Get().ToList();
                viewModel.Countrys = _masterService.CountryRepository.Get().ToList();
                //viewModel.Subjects = _masterService.SubjectRepository.Get().ToList();
                return View(viewModel);
            }
            catch (System.Data.Entity.Validation.DbEntityValidationException dbEx)
            {
                Exception raise = dbEx;
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                    {
                        var message = string.Format("{0}:{1}",
                            validationErrors.Entry.Entity,
                            validationError.ErrorMessage);
                        // raise a new exception nesting  
                        // the current instance as InnerException  
                        raise = new InvalidOperationException(message, raise);
                    }
                }
                throw raise;
            }
        }

        [Authorize]
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Verification model = _masterService.VerificationRepository.GetByID(id);
            VerificationViewModel viewModel = Mapper.Map<VerificationViewModel>(model);
            if (viewModel == null)
            {
                return HttpNotFound();
            }
            //viewModel.Courses = _masterService.CourseRepository.Get().ToList();
            //viewModel.CourseParts = _masterService.CoursePartRepository.Get().ToList();
            //viewModel.Branchs = _masterService.BranchRepository.Get().ToList();
            viewModel.Tehsils = _masterService.TehsilRepository.Get().ToList();
            viewModel.Citys = _masterService.CityRepository.Get().ToList();
            viewModel.Districts = _masterService.DistrictRepository.Get().ToList();
            viewModel.States = _masterService.StateRepository.Get().ToList();
            viewModel.Countrys = _masterService.CountryRepository.Get().ToList();
            //viewModel.Subjects = _masterService.SubjectRepository.Get().ToList();
            return View(viewModel);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        [Authorize]
        public ActionResult Edit(VerificationViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                Verification model = Mapper.Map<Verification>(viewModel);
                viewModel.ModifiedBy = User.Identity.GetUserId();
                viewModel.ModifiedDate = DateTime.Now;

                _masterService.VerificationRepository.Update(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            //viewModel.Courses = _masterService.CourseRepository.Get().ToList();
            //viewModel.CourseParts = _masterService.CoursePartRepository.Get().ToList();
            //viewModel.Branchs = _masterService.BranchRepository.Get().ToList();
            viewModel.Tehsils = _masterService.TehsilRepository.Get().ToList();
            viewModel.Citys = _masterService.CityRepository.Get().ToList();
            viewModel.Districts = _masterService.DistrictRepository.Get().ToList();
            viewModel.States = _masterService.StateRepository.Get().ToList();
            viewModel.Countrys = _masterService.CountryRepository.Get().ToList();
            //viewModel.Subjects = _masterService.SubjectRepository.Get().ToList();
            return View(viewModel);
        }

        [Authorize]
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Verification model = _masterService.VerificationRepository.GetByID(id);
            VerificationViewModel viewModel = Mapper.Map<Verification, VerificationViewModel>(model);
            //viewModel.Courses = _masterService.CourseRepository.Get().ToList();
            //viewModel.CourseParts = _masterService.CoursePartRepository.Get().ToList();
            //viewModel.Branchs = _masterService.BranchRepository.Get().ToList();
            viewModel.Tehsils = _masterService.TehsilRepository.Get().ToList();
            viewModel.Citys = _masterService.CityRepository.Get().ToList();
            viewModel.Districts = _masterService.DistrictRepository.Get().ToList();
            viewModel.States = _masterService.StateRepository.Get().ToList();
            viewModel.Countrys = _masterService.CountryRepository.Get().ToList();
            //viewModel.Subjects = _masterService.SubjectRepository.Get().ToList();
            return View(viewModel);
        }

        [Authorize]
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            var verification = _masterService.VerificationRepository.GetByID(id);
            verification.DeletedBy = User.Identity.GetUserId();
            verification.DeletedDate = DateTime.Now;

            verification.Status = ValueConstants.Deleted;
            _masterService.VerificationRepository.Update(verification);
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