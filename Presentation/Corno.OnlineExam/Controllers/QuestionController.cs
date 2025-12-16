using System.Collections.Generic;
using System.Data.Entity;
using System.Net;
using System.Web.Mvc;
using Corno.Models;
using PagedList;
using System.Linq;
using Corno.DAL.Classes;

namespace Corno.Controllers
{
    public class QuestionController : BaseController
    {
        IQuestionBankService _questionBankService;

        public QuestionController(IQuestionBankService questionBankService)
        {
            _questionBankService = questionBankService;
        }

        // GET: /Question/
        public ActionResult Index(int? page, string SubjectNames)
        {
            var NameList = new List<string>();

            var NameQuery = _questionBankService.SubjectRepository.Get()
            .GroupBy(x => x.Name)
            .Select(group => group.Key);

            NameList.AddRange(NameQuery.Distinct());
            ViewBag.SubjectNames = new SelectList(NameList);

            IPagedList questions = null;
            int selectedSubjectID = 0;
            if (SubjectNames != null)
            {
                foreach (Subject subject in _questionBankService.SubjectRepository.Get())
                {
                    if (subject.Name == SubjectNames)
                    {
                        selectedSubjectID = (int)subject.ID;
                    }
                }
            }
            if (!string.IsNullOrEmpty(SubjectNames) && selectedSubjectID > 0)
                questions = _questionBankService.QuestionRepository.Get().Where(x => x.SubjectID == selectedSubjectID).OrderBy(x => x.ID).ToPagedList(page ?? 1, 5);
            else
                questions = _questionBankService.QuestionRepository.Get().ToPagedList(page ?? 1, 5);

            ViewBag.SubjectID = _questionBankService.SubjectRepository.Get();
            return View(questions);
        }

        // GET: /Question/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = _questionBankService.QuestionRepository.GetByID(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // GET: /Question/Create
        public ActionResult Create()
        {
            ViewBag.SubjectID = new SelectList(_questionBankService.SubjectRepository.Get(), "ID", "Name");
            ViewBag.DifficultyLevel = new SelectList(_questionBankService.QuestionRepository.Get(), "ID", "DifficultyLevel");
            return View();
        }

        // POST: /Question/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Question question)
        {
            if (ModelState.IsValid)
            {
                _questionBankService.QuestionRepository.Add(question);
                _questionBankService.Save();
                return RedirectToAction("Index");
            }
            ViewBag.SubjectID = new SelectList(_questionBankService.SubjectRepository.Get(), "ID", "Name");
            ViewBag.DifficultyLevel = new SelectList(_questionBankService.QuestionRepository.Get(), "ID", "DifficultyLevel");
            return View(question);
        }

        // GET: /Question/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = _questionBankService.QuestionRepository.GetByID(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            ViewBag.SubjectID = new SelectList(_questionBankService.SubjectRepository.Get(), "ID", "Name");
            ViewBag.DifficultyLevel = new SelectList(_questionBankService.QuestionRepository.Get(), "ID", "DifficultyLevel");
            return View(question);
        }

        // POST: /Question/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Question question)
        {
            if (ModelState.IsValid)
            {
                _questionBankService.QuestionRepository.Update(question);
                _questionBankService.Save();
                return RedirectToAction("Index");
            }
            ViewBag.SubjectID = new SelectList(_questionBankService.SubjectRepository.Get(), "ID", "Name");
            return View(question);
        }

        // GET: /Question/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Question question = _questionBankService.QuestionRepository.GetByID(id);
            if (question == null)
            {
                return HttpNotFound();
            }
            return View(question);
        }

        // POST: /Question/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Question question = _questionBankService.QuestionRepository.GetByID(id);
            _questionBankService.QuestionRepository.Delete(question);
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
