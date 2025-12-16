using System.Net;
using System.Web.Mvc;
using Corno.Models;
using PagedList;

namespace Corno.Controllers
{
    public class UrlCategoryController : BaseController
    {
        //private UnitOfWork unitOfWork = new UnitOfWork();

        // GET: /UrlCategory/
        public ActionResult Index(int? page)
        {
            return View(_unitOfWork.UrlCategoryRepository.Get().ToPagedList(page ?? 1,1));
        }

        // GET: /UrlCategory/Details/5
        public ActionResult Details(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UrlCategoryMaster urlcategorymaster = _unitOfWork.UrlCategoryRepository.GetByID(Id);
            if (urlcategorymaster == null)
            {
                return HttpNotFound();
            }
            return View(urlcategorymaster);
        }

        // GET: /UrlCategory/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /UrlCategory/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(UrlCategoryMaster urlcategorymaster)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.UrlCategoryRepository.Add(urlcategorymaster);
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }

            return View(urlcategorymaster);
        }

        // GET: /UrlCategory/Edit/5
        public ActionResult Edit(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UrlCategoryMaster urlcategorymaster = _unitOfWork.UrlCategoryRepository.GetByID(Id);
            if (urlcategorymaster == null)
            {
                return HttpNotFound();
            }
            return View(urlcategorymaster);
        }

        // POST: /UrlCategory/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(UrlCategoryMaster urlcategorymaster)
        {
            if (ModelState.IsValid)
            {
                _unitOfWork.Save();
                return RedirectToAction("Index");
            }
            return View(urlcategorymaster);
        }

        // GET: /UrlCategory/Delete/5
        public ActionResult Delete(int? Id)
        {
            if (Id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            UrlCategoryMaster urlcategorymaster = _unitOfWork.UrlCategoryRepository.GetByID(Id);
            if (urlcategorymaster == null)
            {
                return HttpNotFound();
            }
            return View(urlcategorymaster);
        }

        // POST: /UrlCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int Id)
        {
            UrlCategoryMaster urlcategorymaster = _unitOfWork.UrlCategoryRepository.GetByID(Id);
            _unitOfWork.UrlCategoryRepository.Delete(urlcategorymaster);
            _unitOfWork.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _unitOfWork.Dispose();
            }
            base.Dispose(disposing);
        }
    }
}
