using System;
using System.Collections.Generic;
using System.Data;
using System.Data.Entity;
using System.Linq;
using System.Net;
using System.Web;
using System.Web.Mvc;
using Corno.DAL;

namespace Corno.Models
{
    public class ProjectTypeController : Controller
    {
        private UnitOfWork unitOfWork = new UnitOfWork();

        // GET: /ProjectType/
        public ActionResult Index()
        {
            return View(unitOfWork.ProjectTypeRepository.Get());
        }

        // GET: /ProjectType/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectTypeMaster projecttypemaster = unitOfWork.ProjectTypeRepository.GetByID(id);
            if (projecttypemaster == null)
            {
                return HttpNotFound();
            }
            return View(projecttypemaster);
        }

        // GET: /ProjectType/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /ProjectType/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(ProjectTypeMaster projecttypemaster)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.ProjectTypeRepository.Add(projecttypemaster);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(projecttypemaster);
        }

        // GET: /ProjectType/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectTypeMaster projecttypemaster = unitOfWork.ProjectTypeRepository.GetByID(id);
            if (projecttypemaster == null)
            {
                return HttpNotFound();
            }
            return View(projecttypemaster);
        }

        // POST: /ProjectType/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit([Bind(Include="ID,CompanyID,SerialNo,Code,Name,Status,CreatedBy,CreatedDate,ModifiedBy,ModifiedDate,DeletedBy,DeletedDate")] ProjectTypeMaster projecttypemaster)
        {
            if (ModelState.IsValid)
            {
                unitOfWork.ProjectTypeRepository.Update(projecttypemaster);
                unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(projecttypemaster);
        }

        // GET: /ProjectType/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            ProjectTypeMaster projecttypemaster = unitOfWork.ProjectTypeRepository.GetByID(id);
            if (projecttypemaster == null)
            {
                return HttpNotFound();
            }
            return View(projecttypemaster);
        }

        // POST: /ProjectType/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            ProjectTypeMaster projecttypemaster = unitOfWork.ProjectTypeRepository.GetByID(id);
            unitOfWork.ProjectTypeRepository.Delete(projecttypemaster);
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
