using System;
using System.Linq;
using System.Web.Mvc;
using Corno.Data.Corno.Question_Bank_V2.Models;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Logger;
using Corno.Education.Controllers;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Question_Bank_V2.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Corno.Education.Areas.Question_Bank_V2.Controllers
{
    [Authorize]
    public class QB_QuestionBankController : CornoController
    {
        private readonly IQB_QuestionBankService _questionBankService;
        private readonly IFacultyService _facultyService;
        private readonly IMiscMasterService _miscMasterService;
        
        public QB_QuestionBankController()
        {
            _questionBankService = Bootstrapper.Get<IQB_QuestionBankService>();
            _facultyService = Bootstrapper.Get<IFacultyService>();
            Bootstrapper.Get<ICourseService>();
            Bootstrapper.Get<ISubjectService>();
            _miscMasterService = Bootstrapper.Get<IMiscMasterService>();
        }
        
        public ActionResult Index()
        {
            return View();
        }
        
        public ActionResult Create()
        {
            var model = new QB_QuestionBank();
            LoadViewBagData();
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Create(QB_QuestionBank model)
        {
            if (!ModelState.IsValid)
            {
                LoadViewBagData();
                return View(model);
            }
            
            try
            {
                var sessionData = Session[User.Identity.Name] as SessionData;
                var instanceId = sessionData?.InstanceId ?? 0;
                
                _questionBankService.SaveQuestion(model, User.Identity.Name, instanceId, false);
                
                TempData["Success"] = "Question created successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HandleControllerException(ex);
                LoadViewBagData();
                return View(model);
            }
        }
        
        public ActionResult Edit(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(400);
            
            var model = _questionBankService.GetById(id);
            if (model == null)
                return HttpNotFound();
            
            LoadViewBagData();
            return View(model);
        }
        
        [HttpPost]
        [ValidateAntiForgeryToken]
        [ValidateInput(false)]
        public ActionResult Edit(QB_QuestionBank model)
        {
            if (!ModelState.IsValid)
            {
                LoadViewBagData();
                return View(model);
            }
            
            try
            {
                var sessionData = Session[User.Identity.Name] as SessionData;
                var instanceId = sessionData?.InstanceId ?? 0;
                
                _questionBankService.SaveQuestion(model, User.Identity.Name, instanceId, true);
                
                TempData["Success"] = "Question updated successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HandleControllerException(ex);
                LoadViewBagData();
                return View(model);
            }
        }
        
        public ActionResult View(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(400);
            
            var model = _questionBankService.GetById(id);
            if (model == null)
                return HttpNotFound();
            
            return View(model);
        }
        
        [HttpPost]
        public ActionResult SubmitForCheck(int id)
        {
            try
            {
                _questionBankService.SubmitForCheck(id, User.Identity.Name);
                return Json(new { success = true, message = "Question submitted for check successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        
        [HttpPost]
        public ActionResult Approve(int id, string comments)
        {
            try
            {
                var roleName = User.IsInRole("Question Checker") ? "Question Checker" : "Moderator";
                _questionBankService.ApproveQuestion(id, User.Identity.Name, roleName, comments);
                return Json(new { success = true, message = "Question approved successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        
        [HttpPost]
        public ActionResult Reject(int id, string reason)
        {
            try
            {
                var roleName = User.IsInRole("Question Checker") ? "Question Checker" : "Moderator";
                _questionBankService.RejectQuestion(id, User.Identity.Name, roleName, reason);
                return Json(new { success = true, message = "Question rejected." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }
        
        [HttpPost]
        public ActionResult GetQuestions([DataSourceRequest] DataSourceRequest request, int? instanceId, int? subjectId)
        {
            try
            {
                var sessionData = Session[User.Identity.Name] as SessionData;
                var instId = instanceId ?? sessionData?.InstanceId ?? 0;
                
                var query = _questionBankService.GetQuery()
                    .Where(q => q.InstanceId == instId && 
                               (subjectId == null || q.SubjectId == subjectId) &&
                               q.Status != StatusConstants.Deleted);
                
                // Get data
                var questions = query.OrderByDescending(x => x.CreatedDate).ToList();
                
                // Decrypt questions
                foreach (var question in questions)
                {
                    _questionBankService.DecryptQuestion(question);
                }
                
                // Map to DTO for grid
                var data = questions.Select(q => new
                {
                    q.Id,
                    q.QuestionCode,
                    SubjectName = q.SubjectName ?? "N/A",
                    QuestionTypeName = q.QuestionTypeName ?? "N/A",
                    DifficultyLevelName = q.DifficultyLevelName ?? "N/A",
                    TaxonomyLevelName = q.TaxonomyLevelName ?? "N/A",
                    q.Marks,
                    q.Status,
                    q.CreatedDate
                });
                
                var result = data.ToDataSourceResult(request);
                return Json(result);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(ex);
                return Json(new DataSourceResult { Errors = new[] { ex.Message } });
            }
        }
        
        private void LoadViewBagData()
        {
            ViewBag.Faculties = _facultyService.GetViewModelList().ToList();
            
            ViewBag.QuestionTypes = _miscMasterService.GetViewModelList(m => m.MiscType == MiscConstants.QuestionType).ToList();
            
            ViewBag.DifficultyLevels = _miscMasterService.GetViewModelList(m => m.MiscType == MiscConstants.DifficultyLevel).ToList();
            
            ViewBag.TaxonomyLevels = _miscMasterService.GetViewModelList(m => m.MiscType == MiscConstants.Taxonomy).ToList();
        }
    }
}
