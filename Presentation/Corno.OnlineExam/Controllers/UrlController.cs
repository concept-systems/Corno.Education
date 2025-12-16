using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Corno.DAL;
using Corno.Models;

namespace Corno.Controllers
{
    public class UrlController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET: /Url/
        public ActionResult Index()
        {
            return View(unitOfWork.UrlRepository.Get());
        }

        // GET: /Url/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Url url = unitOfWork.UrlRepository.GetByID(id);
            if (url == null)
            {
                return HttpNotFound();
            }
            return View(url);
        }

        // GET: /Url/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Url/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create( Url url)
        {
            if (ModelState.IsValid)
            {
               unitOfWork.UrlRepository.Add(url);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }


            return View(url);
        }

        // GET: /Url/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Url url = unitOfWork.UrlRepository.GetByID(id);
            if (url == null)
            {
                return HttpNotFound();
            }

            return View(url);
        }

        // POST: /Url/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit( Url url)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.UrlRepository.Update(url);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(url);
        }

        // GET: /Url/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Url url = unitOfWork.UrlRepository.GetByID(id);
            if (url == null)
            {
                return HttpNotFound();
            }
            return View(url);
        }

        // POST: /Url/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Url url = unitOfWork.UrlRepository.GetByID(id);
            unitOfWork.UrlRepository.Delete(url);
            unitOfWork.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
