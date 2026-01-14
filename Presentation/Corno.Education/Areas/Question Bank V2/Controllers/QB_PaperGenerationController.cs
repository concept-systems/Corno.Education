using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Corno.Data.Corno.Question_Bank_V2.Models;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Logger;
using Corno.Education.Attributes;
using Corno.Education.Controllers;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Corno.Question_Bank_V2.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Corno.Education.Areas.Question_Bank_V2.Controllers
{
    [Authorize]
    public class QB_PaperGenerationController : CornoController
    {
        private readonly IQB_PaperGenerationService _paperGenerationService;
        private readonly IQB_AppointmentService _appointmentService;
        private readonly IMainService<QB_Paper> _paperService;
        
        public QB_PaperGenerationController()
        {
            _paperGenerationService = Bootstrapper.Get<IQB_PaperGenerationService>();
            _appointmentService = Bootstrapper.Get<IQB_AppointmentService>();
            _paperService = Bootstrapper.Get<IMainService<QB_Paper>>();
        }
        
        public ActionResult Index(int? appointmentId)
        {
            ViewBag.AppointmentId = appointmentId;
            return View();
        }
        
        public ActionResult GenerateAuto(int appointmentId)
        {
            try
            {
                var appointment = _appointmentService.GetById(appointmentId);
                if (appointment == null)
                    return HttpNotFound("Appointment not found.");
                
                ViewBag.Appointment = appointment;
                ViewBag.AppointmentId = appointmentId;
                
                return View();
            }
            catch (Exception ex)
            {
                HandleControllerException(ex);
                return RedirectToAction("Index", new { appointmentId });
            }
        }
        
        [HttpPost]
        public ActionResult GenerateAuto(int appointmentId, int noOfSets)
        {
            try
            {
                var appointment = _appointmentService.GetById(appointmentId);
                if (appointment == null)
                    return Json(new { success = false, message = "Appointment not found." });
                
                var generatedPapers = new List<QB_Paper>();
                
                for (int setNumber = 1; setNumber <= noOfSets; setNumber++)
                {
                    var paper = _paperGenerationService.GeneratePaperAuto(appointment, setNumber, User.Identity.Name);
                    generatedPapers.Add(paper);
                }
                
                return Json(new { 
                    success = true, 
                    message = $"{noOfSets} paper(s) generated successfully.",
                    papers = generatedPapers.Select(p => new { 
                        Id = p.Id, 
                        PaperCode = p.PaperCode,
                        SetNumber = p.SetNumber 
                    })
                });
            }
            catch (Exception ex)
            {
                LogHandler.LogError(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }
        
        public ActionResult GenerateManual(int appointmentId)
        {
            try
            {
                var appointment = _appointmentService.GetById(appointmentId);
                if (appointment == null)
                    return HttpNotFound("Appointment not found.");
                
                ViewBag.Appointment = appointment;
                ViewBag.AppointmentId = appointmentId;
                
                return View();
            }
            catch (Exception ex)
            {
                HandleControllerException(ex);
                return RedirectToAction("Index", new { appointmentId });
            }
        }
        
        [HttpPost]
        public ActionResult GenerateManual(int appointmentId, int setNumber, Dictionary<int, int> questionSelections)
        {
            try
            {
                var appointment = _appointmentService.GetById(appointmentId);
                if (appointment == null)
                    return Json(new { success = false, message = "Appointment not found." });
                
                var paper = _paperGenerationService.GeneratePaperManual(appointment, setNumber, 
                    questionSelections, User.Identity.Name);
                
                return Json(new { 
                    success = true, 
                    message = "Paper generated successfully.",
                    paper = new { 
                        Id = paper.Id, 
                        PaperCode = paper.PaperCode,
                        SetNumber = paper.SetNumber 
                    }
                });
            }
            catch (Exception ex)
            {
                LogHandler.LogError(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }
        
        public ActionResult ViewPaper(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(400);
            
            var paper = _paperService.GetById(id);
            if (paper == null)
                return HttpNotFound();
            
            return View(paper);
        }
        
        [HttpPost]
        public ActionResult DrawPaper(int id)
        {
            try
            {
                if (!_paperGenerationService.CanModifyPaper(id))
                    return Json(new { success = false, message = "Paper has already been drawn and cannot be modified." });
                
                _paperGenerationService.DrawPaper(id, User.Identity.Name);
                
                return Json(new { success = true, message = "Paper drawn successfully." });
            }
            catch (Exception ex)
            {
                LogHandler.LogError(ex);
                return Json(new { success = false, message = ex.Message });
            }
        }
        
        public ActionResult DownloadWord(int id)
        {
            try
            {
                var paper = _paperService.GetById(id);
                if (paper == null)
                    return HttpNotFound();
                
                if (paper.WordDocumentContent == null || paper.WordDocumentContent.Length == 0)
                {
                    // Generate if not exists
                    var documentContent = _paperGenerationService.GenerateWordDocument(paper);
                    if (documentContent == null)
                        throw new Exception("Failed to generate Word document.");
                    
                    paper.WordDocumentContent = documentContent;
                    paper.WordDocumentFileName = $"{paper.PaperCode}_Set{paper.SetNumber}.docx";
                    paper.WordDocumentGeneratedDate = DateTime.Now;
                    _paperService.UpdateAndSave(paper);
                }
                
                return File(paper.WordDocumentContent, 
                    "application/vnd.openxmlformats-officedocument.wordprocessingml.document", 
                    paper.WordDocumentFileName);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(ex);
                TempData["Error"] = ex.Message;
                return RedirectToAction("ViewPaper", new { id });
            }
        }
        
        [HttpPost]
        public ActionResult GetPapers([DataSourceRequest] DataSourceRequest request, int? appointmentId)
        {
            try
            {
                var papers = _paperService.Get(p => (appointmentId == null || p.AppointmentId == appointmentId) &&
                                                    p.Status != StatusConstants.Deleted, p => p,
                    p => p.OrderByDescending(x => x.GeneratedDate));
                
                var data = papers.Select(p => new
                {
                    Id = p.Id,
                    PaperCode = p.PaperCode,
                    SetNumber = p.SetNumber,
                    PaperType = p.PaperType,
                    MaxMarks = p.MaxMarks,
                    Status = p.Status,
                    GeneratedDate = p.GeneratedDate,
                    DrawnDate = p.DrawnDate,
                    CanModify = _paperGenerationService.CanModifyPaper(p.Id ?? 0)
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
    }
}
