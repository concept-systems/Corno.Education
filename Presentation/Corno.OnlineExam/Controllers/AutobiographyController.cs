using Corno.Controllers;
using Corno.DAL.Classes;
using System.Net;
using System.Web.Mvc;
using PagedList;
using Corno.Helpers;

namespace Corno.Models
{
    public class AutobiographyController : BaseController
    {
        IAutobiographyService _autobiographyService;

        public AutobiographyController(IAutobiographyService autobiographyService)
        {
            _autobiographyService = autobiographyService;
        }

        // GET: /Autobiography/
        public ActionResult Index(int? page)
        {
            var viewModels = _autobiographyService.AutobiographyRepository.Get().ToPagedList(page ?? 1, 10).ToMappedPagedList<Autobiography, AutobiographyViewModel>();
            return View(viewModels);
            //return View(_autobiographyService.AutobiographyRepository.Get().ToPagedList(page?? 1, 10));
        }

        /// <summary>
        /// Get value from view textbox and send to model
        /// </summary>
        /// <param name="txtValue"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult Index(string txtValue)
        {
            // Check length before for process
            if (txtValue.Length > 0)
            {
                return View(_autobiographyService.AutobiographyRepository.Get(b => b.Name.ToLower().Contains(txtValue)).ToPagedList(1, 10).ToMappedPagedList<Autobiography, AutobiographyViewModel>());
            }
            else
            {
                return View(_autobiographyService.AutobiographyRepository.Get().ToPagedList(1, 10).ToMappedPagedList<Autobiography, AutobiographyViewModel>());
            }
        }

        // GET: /Autobiography/Details/5
        public ActionResult Details(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Autobiography autobiography = _autobiographyService.AutobiographyRepository.GetByID(id);
            if (autobiography == null)
            {
                return HttpNotFound();
            }
            return View(autobiography);
        }

        // GET: /Autobiography/Create
        public ActionResult Create()
        {
            return View();
        }

        // POST: /Autobiography/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(Autobiography autobiography)
        {
            if (ModelState.IsValid)
            {
                _autobiographyService.AutobiographyRepository.Add(autobiography);
                _autobiographyService.Save();
                return RedirectToAction("Index");
            }

            return View(autobiography);
        }

        // GET: /Autobiography/Edit/5
        public ActionResult Edit(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Autobiography autobiography = _autobiographyService.AutobiographyRepository.GetByID(id);
            if (autobiography == null)
            {
                return HttpNotFound();
            }
            return View(autobiography);
        }

        // POST: /Autobiography/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Edit(Autobiography autobiography)
        {
            if (ModelState.IsValid)
            {
                _autobiographyService.AutobiographyRepository.Update(autobiography);
                _autobiographyService.Save();
                return RedirectToAction("Index");
            }
            return View(autobiography);
        }

        // GET: /Autobiography/Delete/5
        public ActionResult Delete(int? id)
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            Autobiography autobiography = _autobiographyService.AutobiographyRepository.GetByID(id);
            if (autobiography == null)
            {
                return HttpNotFound();
            }
            return View(autobiography);
        }

        // POST: /Autobiography/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public ActionResult DeleteConfirmed(int id)
        {
            Autobiography autobiography = _autobiographyService.AutobiographyRepository.GetByID(id);
            _autobiographyService.AutobiographyRepository.Delete(autobiography);
            _autobiographyService.Save();
            return RedirectToAction("Index");
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _autobiographyService.Dispose(disposing);
            }
            base.Dispose(disposing);
        }
    }
}
