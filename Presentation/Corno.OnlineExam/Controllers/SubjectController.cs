using System.Net;
using System.Web.Mvc;
using Corno.Models;
using PagedList;
using Corno.DAL.Classes;

namespace Corno.Controllers
{
    public class SubjectController : BaseController
    {
        IQuestionBankService _questionBankService;

        public SubjectController(IQuestionBankService questionBankService)
        {
            _questionBankService = questionBankService;
        }

        // GET: /Subject/
        public ActionResult Index(int? page)
        {
            return View(_questionBankService.SubjectRepository.Get().ToPagedList(page ?? 1, 3));
        }

        // GET: /Subject/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subject subject = _questionBankService.SubjectRepository.GetByID(id);
            if (subject == null)
            {
                return HttpNotFound();
            }
            return View(subject);
        }

        // GET: /Subject/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Subject/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Subject subject)
        {
            if (ModelState.IsValid)
            {
                _questionBankService.SubjectRepository.Add(subject);
                _questionBankService.Save();
                return RedirectToAction("Index");
            }

            return View(subject);
        }

        // GET: /Subject/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subject subject = _questionBankService.SubjectRepository.GetByID(id);
            if (subject == null)
            {
                return HttpNotFound();
            }
            return View(subject);
        }

        // POST: /Subject/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Subject subject)
        {
            if (ModelState.IsValid)
            {
                _questionBankService.SubjectRepository.Update(subject);
                _questionBankService.Save();
                return RedirectToAction("Index");
            }
            return View(subject);
        }

        // GET: /Subject/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Subject subject = _questionBankService.SubjectRepository.GetByID(id);
            if (subject == null)
            {
                return HttpNotFound();
            }
            return View(subject);
        }

        // POST: /Subject/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Subject subject = _questionBankService.SubjectRepository.GetByID(id);
            _questionBankService.SubjectRepository.Delete(subject);
            _questionBankService.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _questionBankService.Dispose(disposing);
            }
            base.Dispose(disposing);
        }
    }
}
