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
    public class StateController : Controller
    {
        IMasterService _masterService;

        public StateController(IMasterService masterService)
        {
            _masterService = masterService;
        }

        // GET: /State/
         [Authorize]
        public ActionResult Index(int? page)
        {
           var viewModel= _masterService.StateRepository.Get().ToList().ToMappedList<State,StateViewModel>();
           return View(viewModel);
        }

        // GET: /State/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            State state = _masterService.StateRepository.GetByID(id);
            if (state == null)
            {
                return HttpNotFound();
            }
            return View(state);
        }

        // GET: /State/Create
        public ActionResult Create()
        {
            State state = new State();
            StateViewModel viewModel = new StateViewModel();
            viewModel.Countrys = _masterService.CountryRepository.Get().ToList();

            return View(viewModel);
        }

        // POST: /State/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(StateViewModel viewModel ,State state)
        {
            if (ModelState.IsValid)
            {
                State model=Mapper.Map<State>(viewModel);
                _masterService.StateRepository.Add(model);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            viewModel.Countrys = _masterService.CountryRepository.Get().ToList();

            return View(viewModel);
        }

        // GET: /State/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            State model = _masterService.StateRepository.GetByID(id);
            StateViewModel viewModel =Mapper.Map<StateViewModel>(model);
            if (model == null)
            {
                return HttpNotFound();
            }
            viewModel.Countrys = _masterService.CountryRepository.Get().ToList();

            return View(viewModel);
        }

        // POST: /State/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(StateViewModel viewModel ,State state)
        {
            if (ModelState.IsValid)
            {
                State model=Mapper.Map<State>(viewModel);
                viewModel.ModifiedBy = User.Identity.GetUserId();
                viewModel.ModifiedDate = DateTime.Now;
                _masterService.StateRepository.Update(state);
                _masterService.Save();
                return RedirectToAction("Index");
            }
            viewModel.Countrys = _masterService.CountryRepository.Get().ToList();

            return View(viewModel);
        }

        // GET: /State/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            State model = _masterService.StateRepository.GetByID(id);
            StateViewModel viewModel=Mapper.Map<State,StateViewModel>(model);
            viewModel.Countrys = _masterService.CountryRepository.Get().ToList();

            return View(viewModel);
        }

        // POST: /State/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            State state = _masterService.StateRepository.GetByID(id);
            state.DeletedBy = User.Identity.GetUserId();
            state.DeletedDate = DateTime.Now;

            state.Status = ValueConstants.Deleted;
            _masterService.StateRepository.Update(state);
            //_masterService.StateRepository.Delete(state);
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
