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
    public class CollegeFeeController : Controller
    {
        IMasterService _masterService;

        public CollegeFeeController(IMasterService masterService)
        {
            _masterService = masterService;
        }
        //
        // GET: /CollegeFee/
         [Authorize]
        public ActionResult Index()
        {
            var viewModels = _masterService.CollegeFeeRepository.Get().ToList().ToMappedList<CollegeFee, CollegeFeeViewModel>();
            return View(viewModels);
        }

        //
        // GET: /CollegeFee/Details/5

        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CollegeFee CollegeFee = _masterService.CollegeFeeRepository.GetByID(id);
            if (CollegeFee == null)
            {
                return HttpNotFound();
            }
            return View(CollegeFee);
        }

        //
        // GET: /CollegeFee/Create

        public ActionResult Create()
        {
            CollegeFeeViewModel viewModel = new CollegeFeeViewModel();
            //viewModel.Colleges = _masterService.CollegeRepository.Get().ToList();
            return View(viewModel);
        }

        //
        // POST: /CollegeFee/Create

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(CollegeFeeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                CollegeFee model = Mapper.Map<CollegeFee>(viewModel);
                _masterService.CollegeFeeRepository.Add(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            //viewModel.Colleges = _masterService.CollegeRepository.Get().ToList();
            //viewModel.Fees = _masterService.FeeRepository.Get().ToList();
            return View(viewModel);
        }

        //
        // GET: /CollegeFee/Edit/5

        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CollegeFee model = _masterService.CollegeFeeRepository.GetByID(id);
            CollegeFeeViewModel viewModel = Mapper.Map<CollegeFeeViewModel>(model);
            if (viewModel == null)
            {
                return HttpNotFound();
            }
            //viewModel.Colleges = _masterService.CollegeRepository.Get().ToList();
           // viewModel.Fees = _masterService.FeeRepository.Get().ToList();
            return View(viewModel);
        }

        //
        // POST: /CollegeFee/Edit/5

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(CollegeFeeViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                CollegeFee model = Mapper.Map<CollegeFee>(viewModel);
                viewModel.ModifiedBy = User.Identity.GetUserId();
                viewModel.ModifiedDate = DateTime.Now;

                _masterService.CollegeFeeRepository.Update(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            //viewModel.Colleges = _masterService.CollegeRepository.Get().ToList();
           // viewModel.Fees = _masterService.FeeRepository.Get().ToList();
            return View(viewModel);
        }

        //
        // GET: /CollegeFee/Delete/5

        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            CollegeFee model = _masterService.CollegeFeeRepository.GetByID(id);
            CollegeFeeViewModel viewModel = Mapper.Map<CollegeFee, CollegeFeeViewModel>(model);
            //viewModel.Colleges = _masterService.CollegeRepository.Get().ToList();
           // viewModel.Fees = _masterService.FeeRepository.Get().ToList();
            return View(viewModel);
        }

        //
        // POST: /CollegeFee/Delete/5

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            CollegeFee CollegeFee = _masterService.CollegeFeeRepository.GetByID(id);
            CollegeFee.DeletedBy = User.Identity.GetUserId();
            CollegeFee.DeletedDate = DateTime.Now;

            CollegeFee.Status = ValueConstants.Deleted;
            _masterService.CollegeFeeRepository.Update(CollegeFee);
            _masterService.Save();
            return RedirectToAction("Index");
        }
        public JsonResult GetCascadeCollege()
        {
            //var Colleges =_masterService.CollegeRepository.Get().ToList();

            //return Json(Colleges.Select(p => new { ID = p.ID, Name = p.Name }), JsonRequestBehavior.AllowGet);
            return null;
        }
        //public JsonResult GetCascadeFees(int? collegeID, string feeFilter)
        //{
        //    collegeID = (int)collegeID;
        //    var fees = _masterService.FeeRepository.Get().Where(p => p.CollegeID == collegeID).ToList();

        //    if (!string.IsNullOrEmpty(feeFilter))
        //    {
        //        fees = _masterService.FeeRepository.Get().Where(p => p.Name == feeFilter).ToList();
        //    }

        //    return Json(fees.Select(p => new { ID = p.ID, Name = p.Name }), JsonRequestBehavior.AllowGet);
        //}
        protected override void Dispose(bool disposing)
        {
            _masterService.Dispose(disposing);
            base.Dispose(disposing);
        }
    }
}