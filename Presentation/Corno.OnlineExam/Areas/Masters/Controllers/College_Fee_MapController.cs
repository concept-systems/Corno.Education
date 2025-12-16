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
    public class College_Fee_MapController : Controller
    {
        IMasterService _masterService;

        public College_Fee_MapController(IMasterService masterService)
        {
            _masterService = masterService;
        }

        // GET: /College_Fee_Map/
         [Authorize]
        public ActionResult Index(int? page)
        {
            var viewModel = _masterService.College_Fee_MapRepository.Get().ToList().ToMappedList<College_Fee_Map, College_Fee_MapViewModel>();
            return View(viewModel);
        }

        // GET: /College_Fee_Map/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            College_Fee_Map College_Fee_Map = _masterService.College_Fee_MapRepository.GetByID(id);
            if (College_Fee_Map == null)
            {
                return HttpNotFound();
            }
            return View(College_Fee_Map);
        }

        // GET: /College_Fee_Map/Create
        public ActionResult Create()
        {
            College_Fee_MapViewModel viewModel = new College_Fee_MapViewModel();
            //viewModel.Colleges = _masterService.CollegeRepository.Get().ToList();
            viewModel.Fees = _masterService.FeeRepository.Get().ToList();
            return View(viewModel);
        }

        // POST: /College_Fee_Map/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(College_Fee_MapViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                College_Fee_Map model = Mapper.Map<College_Fee_Map>(viewModel);
                _masterService.College_Fee_MapRepository.Add(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            //viewModel.Colleges = _masterService.CollegeRepository.Get().ToList();
            viewModel.Fees = _masterService.FeeRepository.Get().ToList();
            return View(viewModel);
        }

        // GET: /College_Fee_Map/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            College_Fee_Map model = _masterService.College_Fee_MapRepository.GetByID(id);
            College_Fee_MapViewModel viewModel = Mapper.Map<College_Fee_MapViewModel>(model);
            if (model == null)
            {
                return HttpNotFound();
            }
            //viewModel.Colleges = _masterService.CollegeRepository.Get().ToList();
            viewModel.Fees = _masterService.FeeRepository.Get().ToList();
            return View(viewModel);
        }

        // POST: /College_Fee_Map/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(College_Fee_MapViewModel viewModel)
        {
            if (ModelState.IsValid)
            {
                College_Fee_Map model = Mapper.Map<College_Fee_Map>(viewModel);
                viewModel.ModifiedBy = User.Identity.GetUserId();
                viewModel.ModifiedDate = DateTime.Now;

                _masterService.College_Fee_MapRepository.Update(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            //viewModel.Colleges = _masterService.CollegeRepository.Get().ToList();
            viewModel.Fees = _masterService.FeeRepository.Get().ToList();
            return View(viewModel);
        }

        // GET: /College_Fee_Map/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            College_Fee_Map model = _masterService.College_Fee_MapRepository.GetByID(id);
            College_Fee_MapViewModel viewModel = Mapper.Map<College_Fee_Map, College_Fee_MapViewModel>(model);
            //viewModel.Colleges = _masterService.CollegeRepository.Get().ToList();
            viewModel.Fees = _masterService.FeeRepository.Get().ToList();
            return View(viewModel);
        }

        // POST: /College_Fee_Map/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            College_Fee_Map College_Fee_Map = _masterService.College_Fee_MapRepository.GetByID(id);
            College_Fee_Map.DeletedBy = User.Identity.GetUserId();
            College_Fee_Map.DeletedDate = DateTime.Now;

            College_Fee_Map.Status = ValueConstants.Deleted;
            _masterService.College_Fee_MapRepository.Update(College_Fee_Map);

            //_masterService.College_Fee_MapRepository.Delete(College_Fee_Map);
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
