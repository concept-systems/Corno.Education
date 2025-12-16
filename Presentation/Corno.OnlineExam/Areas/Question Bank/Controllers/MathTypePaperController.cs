using Corno.Data.Admin;
using Corno.Data.Corno.Question_Bank.Models;
using Corno.Data.Helpers;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Globals.Enums;
using Corno.Logger;
using Corno.OnlineExam.Areas.Question_Bank.Dtos;
using Corno.OnlineExam.Attributes;
using Corno.OnlineExam.Controllers;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Paper_Setting.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;
using DevExpress.Web.Mvc;
using DevExpress.XtraPrinting;
using DevExpress.XtraRichEdit;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Web;
using System.Web.Mvc;

namespace Corno.OnlineExam.Areas.Question_Bank.Controllers;

[Authorize]
public class MathTypePaperController : CornoController
{
    #region -- Constructors --
    public MathTypePaperController(ITelerikDocumentService telerikDocumentService, IRichEditDocumentService richEditDocumentService,
        ISubjectService subjectService, IMiscMasterService miscMasterService)
    {
        _telerikDocumentService = telerikDocumentService;
        _richEditDocumentService = richEditDocumentService;
        _subjectService = subjectService;
        _miscMasterService = miscMasterService;
        _paperService = Bootstrapper.Get<IPaperService>();

        const string viewPath = "~/Areas/question bank/views/MathTypePaper/";
        _createPath = $"{viewPath}/Create.cshtml";
        _editPath = $"{viewPath}/Edit.cshtml";
        _indexPath = $"{viewPath}/Index.cshtml";
        _createPath = $"{viewPath}/Create.cshtml";
        _previewPath = $"{viewPath}/View.cshtml";
        _analysisPath = $"{viewPath}/Analysis.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly IPaperService _paperService;
    private readonly ITelerikDocumentService _telerikDocumentService;
    private readonly IRichEditDocumentService _richEditDocumentService;
    private readonly ISubjectService _subjectService;
    private readonly IMiscMasterService _miscMasterService;

    private readonly string _indexPath;
    private readonly string _createPath;
    private readonly string _editPath;
    private readonly string _previewPath;
    private readonly string _analysisPath;
    #endregion

    #region -- Private Methods --
    /*private byte[] ConvertToDocx(RadFlowDocument document)
    {
        using var ms = new MemoryStream();
        var provider = new DocxFormatProvider();
        provider.Export(document, ms);
        return ms.ToArray();
    }

    private byte[] ConvertToPdf(RadFlowDocument document)
    {
        using var ms = new MemoryStream();
        var provider = new PdfFormatProvider
        {
            // Configure PDF export settings
            ExportSettings = new PdfExportSettings
            {
                NumberingFieldsPrecision = NumberingFieldsPrecisionLevel.High,
            }
        };
        provider.Export(document, ms);
        return ms.ToArray();
    }

    private string ConvertToHtml(RadFlowDocument document)
    {
        var exportSettings = new HtmlExportSettings
        {
            DocumentExportLevel = DocumentExportLevel.Document
        };

        var provider = new HtmlFormatProvider
        {
            ExportSettings = exportSettings
        };
        var htmlContent = provider.Export(document);
        return htmlContent;
    }*/

    /*private Paper LoadErrorQuestion(Paper paper)
    {
        var docBytes = RichEditExtension.SaveCopy(ModelConstants.Description,
            DocumentFormat.Rtf);

        var existing = _paperService.GetById(paper.Id);
        existing.DocumentContent = docBytes;
        existing.DifficultyLevel = paper.DifficultyLevel;
        existing.CoNo = paper.CoNo;
        existing.QuestionSerialNo = paper.QuestionSerialNo;

        existing.DocumentContent ??= Array.Empty<byte>();

        // Update paper details
        UpdatePaperDetails(existing);

        return existing;
    }

    private Paper LoadQuestion(Paper paper)
    {
        var existing = _paperService.GetById(paper.Id);
        var paperDetail = existing.PaperDetails.FirstOrDefault(d =>
            d.SerialNo == paper.QuestionSerialNo);
        existing.DocumentContent = paperDetail?.DocumentContent;
        existing.DifficultyLevel = paperDetail?.DifficultyLevel ?? 0;
        existing.CoNo = paperDetail?.CoNo ?? 0;
        existing.QuestionSerialNo = paper.QuestionSerialNo;

        existing.DocumentContent ??= Array.Empty<byte>();

        // Update paper details
        UpdatePaperDetails(existing);

        existing.QuestionInfo = paperDetail;

        return existing;
    }

    private Paper UpdateQuestion(Paper paper)
    {
        var docBytes = RichEditExtension.SaveCopy(ModelConstants.Description,
            DocumentFormat.Rtf);

        var existing = _paperService.GetById(paper.Id);

        var paperDetail = existing.PaperDetails.FirstOrDefault(d => d.SerialNo == paper.QuestionSerialNo);
        if (null != paperDetail && null != docBytes)
        {
            var htmlContent = _richEditDocumentService.ConvertByteArrayToHtml(docBytes);
            // Get the HTML string from the server
            paperDetail.Description = htmlContent;
            paperDetail.DocumentContent = docBytes;
            paperDetail.DifficultyLevel = paper.DifficultyLevel;
            paperDetail.CoNo = paper.CoNo;

            var plainText = _richEditDocumentService.ConvertByteArrayToPlainText(paperDetail.DocumentContent);
            paperDetail.Taxonomy = _paperService.GetTaxonomy(plainText);

            paperDetail.Status = StatusConstants.Completed;
        }

        _paperService.UpdateAndSave(existing);

        UpdatePaperDetails(existing);

        // Clear Document content
        existing.DocumentContent = Array.Empty<byte>();
        existing.QuestionSerialNo = 0;
        existing.DifficultyLevel = 0;
        existing.CoNo = 0;

        return existing;
    }*/

    private void UpdatePaperDetails(Paper paper)
    {
        paper.PaperDetails.ForEach(d =>
        {
            d.QuestionTypeName = _paperService.GetQuestionTypeName(d.QuestionTypeId);
            //d.Description = _richEditDocumentService.ConvertByteArrayToHtml(d.DocumentContent);
        });
    }
    #endregion

    #region -- Actions --
    [Authorize]
    public ActionResult Index(int? page)
    {
        return View(_indexPath, null);
    }

    [Authorize]
    public ActionResult Create()
    {
        return View(_createPath, new Paper { Code = "Test" });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [ValidateInput(false)]
    [MultipleButton(Name = "action", Argument = "Create")]
    public ActionResult Create(Paper paper)
    {
        if (!ModelState.IsValid)
            return View(_createPath, paper);

        try
        {
            // Add or update question
            // Validate Fields
            _paperService.ValidateFields(paper);

            // Update difficulty levels from taxonomy
            _paperService.UpdateTaxonomy(paper);

            _paperService.AddAndSave(paper);
            //_paperService.Save(paper);

            TempData["Success"] = "Saved successfully.";

            // Clear details so that user can add new question for other details
            ModelState.Clear();
            return RedirectToAction("Create");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        paper.PaperDetails.ForEach(d => d.QuestionTypeName =
            _paperService.GetQuestionTypeName(d.QuestionTypeId));

        return View(_createPath, paper);
    }

    public ActionResult Edit(int? id)
    {
        if (null == id)
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        var paper = _paperService.FirstOrDefault(p => p.Id == id, p => p);
        paper.PaperCategoryName = _miscMasterService.GetViewModel(paper.PaperCategoryId)?.NameWithId;

        // Update details
        UpdatePaperDetails(paper);

        paper.DocumentContent = Array.Empty<byte>();

        return View(_editPath, paper);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ValidateInput(false)]
    [MultipleButton(Name = "action", Argument = "Edit")]
    public ActionResult Edit(Paper model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_editPath, model);

            // Validate Fields
            _paperService.ValidateFields(model);

            // Update difficulty levels from taxonomy
            _paperService.UpdateTaxonomy(model);

            if (!string.IsNullOrEmpty(model.PaperType))
            {
                var docBytes = RichEditExtension.SaveCopy(ModelConstants.Description, DocumentFormat.Rtf);
                model.DocumentContent = docBytes;
            }

            _paperService.UpdateAndSave(model);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_editPath, model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "GetQuestions")]
    public ActionResult GetQuestions(Paper paper)
    {
        if (!ModelState.IsValid || null == paper)
            return View(_createPath, paper);

        try
        {
            // Get all Structure subjects
            if ((paper.InstanceId ?? 0) <= 0)
                paper.InstanceId = (Session[User.Identity.Name] as SessionData)?.InstanceId;
            paper = _paperService.GetQuestions(paper);

            ModelState.Clear();
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        paper.PaperDetails.ForEach(d => d.QuestionTypeName =
            _paperService.GetQuestionTypeName(d.QuestionTypeId));

        return View(_createPath, paper);
    }


    [HttpPost]
    public JsonResult SaveDescription(QuestionUpdateDto dto)
    {
        try
        {
            // Validate and save to DB
            var existing = _paperService.GetById(dto.PaperId);
            if(null == existing)
                throw new Exception($"Question paper with Id {dto.PaperId} is either not available.");
            var paperDetail = existing.PaperDetails.FirstOrDefault(d => d.SerialNo == dto.QuestionSerialNo);
            if (null != paperDetail && !string.IsNullOrEmpty(dto.Content))
            {
                paperDetail.Description = dto.Content;
                paperDetail.DocumentContent = Encoding.UTF8.GetBytes(dto.Content); //_richEditDocumentService.ConvertHtmlToByteArray(dto.Content);
                paperDetail.DifficultyLevel = dto.DifficultyLevel;
                paperDetail.LearningPriorityId = dto.LearningPriorityId;
                paperDetail.CoNo = dto.CoNo;

                var plainText = _richEditDocumentService.ConvertByteArrayToPlainText(paperDetail.DocumentContent);
                paperDetail.Taxonomy = _paperService.GetTaxonomy(plainText);

                paperDetail.Status = StatusConstants.Completed;
            }

            _paperService.UpdateAndSave(existing);

            //UpdatePaperDetails(existing);

            return Json(new { success = true, message = "Updated question successfully" });
        }
        catch(Exception exception)
        {
            return Json(new { success = false, message = "Error: " + exception.Message });
        }
    }

    [HttpPost]
    public JsonResult SaveModelAnswer(QuestionUpdateDto dto)
    {
        try
        {
            // Validate and save to DB
            var existing = _paperService.GetById(dto.PaperId);
            if (null == existing)
                throw new Exception($"Question paper with Id {dto.PaperId} is either not available.");
            var paperDetail = existing.PaperDetails.FirstOrDefault(d => d.SerialNo == dto.QuestionSerialNo);
            if (null != paperDetail && !string.IsNullOrEmpty(dto.Content))
            {
                paperDetail.ModelAnswer = dto.Content;
            }

            _paperService.UpdateAndSave(existing);

            // Validate and save to DB
            //bool success = SaveToDatabase(dto); // your logic here
            return Json(new { success = true, message = "Updated model answer successfully" });
        }
        catch (Exception exception)
        {
            return Json(new { success = false, message = "Error: " + exception.Message });
        }
    }


    /*[HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "UpdateDescription")]
    public ActionResult UpdateDescription(Paper paper)
    {
        if (!ModelState.IsValid || null == paper)
            return View(_editPath, paper);

        try
        {
            if (paper.QuestionSerialNo <= 0)
                throw new Exception("Please, select question serial no.");
            if (paper.DifficultyLevel <= 0)
                throw new Exception("Please, enter difficulty level.");

            var existing = UpdateQuestion(paper);

            ModelState.Clear();

            return View(_editPath, existing);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_editPath, LoadErrorQuestion(paper));
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "LoadDescription")]
    public ActionResult LoadDescription(Paper paper)
    {
        if (!ModelState.IsValid || null == paper)
            return View(_editPath, paper);

        try
        {
            if (paper.QuestionSerialNo <= 0)
            {
                paper = _paperService.GetById(paper.Id);

                paper.DocumentContent = Array.Empty<byte>();
                paper.DifficultyLevel = 0;
                paper.CoNo = 0;
                throw new Exception("Please, select question serial no.");
            }

            var existing = LoadQuestion(paper);

            ModelState.Clear();

            return View(_editPath, existing);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_editPath, paper);
    }*/

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ValidateInput(false)]
    [MultipleButton(Name = "action", Argument = "Draw")]
    public ActionResult Draw(Paper paper)
    {
        if (!ModelState.IsValid)
            return View(_previewPath, paper);

        try
        {
            var existing = _paperService.GetById(paper.Id);
            if (null == existing)
                throw new Exception("Question paper is either not available or already drawn.");

            if (existing.PaperDetails.Any(d => null == d.DocumentContent || d.DocumentContent?.Length <= 0))
                throw new Exception("Paper is not yet completed. Please, complete it and then draw it");

            existing.Status = StatusConstants.Drawn;
            _paperService.UpdateAndSave(existing);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        paper.PaperDetails.ForEach(d => d.QuestionTypeName =
            _paperService.GetQuestionTypeName(d.QuestionTypeId));

        return View(_editPath, paper);
    }
    public ActionResult Preview(int? id)
    {
        return null == id ? new HttpStatusCodeResult(HttpStatusCode.BadRequest) :
            Preview(new Paper { Id = id });
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ValidateInput(false)]
    [MultipleButton(Name = "action", Argument = "Preview")]
    public ActionResult Preview(Paper paper)
    {
        if (!ModelState.IsValid)
            return View(_editPath, paper);

        try
        {
            paper = _paperService.GetById(paper.Id);
            if (null == paper)
                throw new Exception("No paper found");

            var radFlowDocument = _telerikDocumentService.Build(paper);
            var pdfBytes = _telerikDocumentService.ExportPdf(radFlowDocument);
            
            ViewBag.PdfBytes = pdfBytes;
            return View(_previewPath);

            // Export to PDF in memory
            /*var documentServer = _paperService.DrawRichEdit(paper);
            using var pdfStream = new MemoryStream();
            var options = new PdfExportOptions();
            documentServer.ExportToPdf(pdfStream, options);
            pdfStream.Seek(0, SeekOrigin.Begin);

            ViewBag.PdfBytes = pdfStream.ToArray();
            return View(_previewPath);*/
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        paper.PaperDetails.ForEach(d => d.QuestionTypeName =
            _paperService.GetQuestionTypeName(d.QuestionTypeId));

        return View(_previewPath, paper);
    }

    [HttpPost]
    public ActionResult UploadModelAnswer(HttpPostedFileBase pdfModelAnswer, Paper paper)
    {
        try
        {
            var basePath = Server.MapPath("~/App_Data/Model Answers");
            _paperService.UploadModelAnswer(pdfModelAnswer, basePath, paper);

            return Json(new { success = true, message = "File uploaded successfully." });

        }
        catch (Exception ex)
        {
            return Json(new { success = false, message = "Upload failed: " + ex.Message });
        }
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult UploadImage()
    {
        var file = Request.Files["upload"];
        if (file is not { ContentLength: > 0 }) 
            return Json(new { error = new { message = "No file uploaded." } });
        var fileName = Path.GetFileName(file.FileName);
        var uploadPath = Server.MapPath("~/App_Data/Question Images/");
        if (!Directory.Exists(uploadPath))
        {
            Directory.CreateDirectory(uploadPath);
        }

        var filePath = Path.Combine(uploadPath, fileName);
        file.SaveAs(filePath);

        var imageUrl = Url.Content("~/App_Data/Question Images/" + fileName);
        return Json(new { url = imageUrl });

    }


    [HttpPost]
    [ValidateAntiForgeryToken]
    [ValidateInput(false)]
    [MultipleButton(Name = "action", Argument = "Analysis")]
    public ActionResult Analysis(Paper paper)
    {
        if (!ModelState.IsValid)
            return View(_editPath, paper);

        try
        {
            var existing = _paperService.GetById(paper.Id);
            if (null == existing)
                throw new Exception("Question paper is either not available or already drawn.");
            var subject = _subjectService.GetById(existing.SubjectId);
            if (null == subject)
                throw new Exception("Subject is not available.");
            var allTaxonomies = _miscMasterService.Get(p =>
                p.MiscType == MiscConstants.Taxonomy, p => p).ToList();
            var analysisDto = new AnalysisDto
            {
                Id = paper.Id ?? 0,
                DifficultyDtos = existing.PaperDetails
                    .GroupBy(g => g.DifficultyLevel)
                    .Select(g =>
                    {
                        var first = g.First();
                        var difficultyLevel = (DifficultyLevel)(first.DifficultyLevel ?? 1);
                        return new AnalysisDifficultyDto
                        {
                            DifficultyLevelId = (int)difficultyLevel,
                            DifficultyLevelName = difficultyLevel.ToString(),
                            Count = g.Count(),
                            Percentage = g.Count() * 100.ToDouble() / existing.PaperDetails.Count
                        };
                    }).ToList(),
                TaxonomyDtos = allTaxonomies
                    .GroupBy(g => g.SerialNo)
                    .Select(g =>
                    {
                        var first = g.First();
                        var appliedCount = existing.PaperDetails.Count(p => p.Taxonomy == first.SerialNo);
                        return new AnalysisTaxonomyDto
                        {
                            SerialNo = first.SerialNo ?? 0,
                            TaxonomyName = first.Code,
                            Count = appliedCount,
                            Percentage = appliedCount * 100.ToDouble() / existing.PaperDetails.Count
                        };
                    }).ToList(),
                ChapterDtos = subject.SubjectChapterDetails
                .GroupBy(g => g.SerialNo)
                .Select(g =>
                {
                    var first = g.First();
                    var appliedCount = existing.PaperDetails.Count(p => p.ChapterId == first.SerialNo);
                    return new AnalysisChapterDto
                    {
                        SerialNo = first.SerialNo ?? 0,
                        ChapterName = first.Name,
                        Count = appliedCount,
                        Percentage = (appliedCount * 100).ToDouble() / existing.PaperDetails.Count
                    };
                }).ToList()

            };

            return View(_analysisPath, analysisDto);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_editPath, paper);
    }

    public ActionResult GetIndexPapers([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var appointmentService = Bootstrapper.Get<IAppointmentService>();

            var query = _paperService.GetQuery() as IEnumerable<Paper>;
            var instanceId = (Session[User.Identity.Name] as SessionData)?.InstanceId;
            if (null == instanceId)
                throw new Exception("Session expired. Please, login again.");
            if (User.IsInRole(RoleConstants.PaperSetter))
            {
                var staffService = Bootstrapper.Get<IStaffService>();
                var user = Session[ModelConstants.User] as ApplicationUser;

                var staffs = staffService.Get(p => p.Mobile == user.UserName && 
                        p.Status == StatusConstants.Active, p => p)
                    .ToList();
                if (staffs.Count <= 0)
                    throw new Exception("Your mobile is not linked with subject in staff master");
                var staffId = staffs.First().Id;
                var subjectIds = appointmentService.Get(p => p.AppointmentDetails.Any(d =>
                    d.StaffId == staffId), p => p.SubjectId).ToList();

                /*var subjectIds = staffs.SelectMany(d => d.StaffSubjectDetails,
                    (_, d) => d.SubjectId).ToList();*/
                if (subjectIds.Count <= 0)
                    throw new Exception("No subjects assigned to staff.");
                query = query.Where(p => p.InstanceId == instanceId && p.Status == StatusConstants.Active &&
                                         subjectIds.Contains(p.SubjectId ?? 0));
            }
            else
            {
                query = query.Where(p => p.Status == StatusConstants.Drawn);
            }

            var facultyService = Bootstrapper.Get<IFacultyService>();
            var courseService = Bootstrapper.Get<ICourseService>();
            var subjectService = Bootstrapper.Get<ISubjectService>();
            var miscMasterService = Bootstrapper.Get<IMiscMasterService>();

            var data = from paper in query
                       join faculty in facultyService.GetQuery()
                           on paper.FacultyId equals faculty.Id into defaultFaculty
                       from faculty in defaultFaculty.DefaultIfEmpty()
                       join course in courseService.GetQuery()
                           on paper.CourseId equals course.Id into defaultCourse
                       from course in defaultCourse.DefaultIfEmpty()
                       join subject in subjectService.GetQuery()
                           on paper.SubjectId equals subject.Id into defaultSubject
                       from subject in defaultSubject.DefaultIfEmpty()
                       join paperCategory in miscMasterService.GetQuery()
                           on paper.PaperCategoryId equals paperCategory.Id into defaultPaperCategory
                       from paperCategory in defaultPaperCategory.DefaultIfEmpty()
                       select new
                       {
                           Id = paper.Id ?? 0,
                           paper.Code,
                           PaperCategoryName = paperCategory?.Name,
                           FacultyName = faculty?.Name,
                           CourseName = $"({course?.Id}) {course?.Name}",
                           SubjectName = $"({subject?.Code}) {subject?.Name}",
                           SetNo = $"Set {paper.SerialNo}",
                           paper.Status
                       };

            var result = data.ToDataSourceResult(request);
            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
        }

        return Json(null, JsonRequestBehavior.AllowGet);
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Inline_Create_Update_Destroy([DataSourceRequest] DataSourceRequest request, PaperDetail paperDetail)
    {
        var docBytes = RichEditExtension.SaveCopy(ModelConstants.Description,
            DocumentFormat.Rtf);
        paperDetail.DocumentContent = docBytes;

        return Json(new[] { paperDetail }.ToDataSourceResult(request, ModelState));
    }

    #endregion
}