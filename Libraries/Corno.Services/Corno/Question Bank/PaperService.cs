using Corno.Data.Common;
using Corno.Data.Corno.Question_Bank;
using Corno.Data.Corno.Question_Bank.Models;
using Corno.Data.Helpers;
using Corno.Data.ViewModels.Appointment;
using Corno.Globals.Constants;
using Corno.Globals.Enums;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;
using DevExpress.XtraRichEdit;
using Kendo.Mvc.Extensions;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using System.Web.Mvc;
using Abp.Extensions;
using Telerik.Windows.Documents.Flow.Model;

namespace Corno.Services.Corno.Question_Bank;

public class PaperService : MainService<Paper>, IPaperService
{
    #region -- Constructors --
    public PaperService(ICornoService cornoService, IStructureService structureService,
        IDocumentService documentService, IMiscMasterService miscMasterService,
        IRichEditDocumentService richEditDocumentService)
    {
        _cornoService = cornoService;
        _structureService = structureService;
        _documentService = documentService;
        _miscMasterService = miscMasterService;
        _richEditDocumentService = richEditDocumentService;

        SetIncludes(nameof(Paper.PaperDetails));
    }
    #endregion

    #region -- Data Members --

    private readonly ICornoService _cornoService;
    private readonly IStructureService _structureService;
    private readonly IDocumentService _documentService;
    private readonly IMiscMasterService _miscMasterService;
    private readonly IRichEditDocumentService _richEditDocumentService;

    #endregion

    #region -- Private Methods --
    private static void ValidateHeaderFields(QuestionBankModel questionBankModel)
    {
        if (questionBankModel.FacultyId.ToInt() <= 0)
            throw new Exception("Invalid Faculty.");
        if (questionBankModel.CourseId.ToInt() <= 0)
            throw new Exception("Invalid Course.");
        if (questionBankModel.CoursePartId.ToInt() <= 0)
            throw new Exception("Invalid Course Part.");
        if (questionBankModel.SubjectId.ToInt() <= 0)
            throw new Exception("Invalid Subject.");
    }

    private Paper CreatePaper(Paper model)
    {
        var structure = _structureService.GetExisting(model);
        if (null == structure)
            throw new Exception($"Structure is not defined for subject {model.SubjectId}");

        var paper = new Paper
        {
            InstanceId = model.InstanceId,
            FacultyId = model.FacultyId,
            CourseId = model.CourseId,
            CoursePartId = model.CoursePartId,
            BranchId = model.BranchId,
            SubjectId = model.SubjectId,
            PaperCategoryId = model.PaperCategoryId,

            NoOfSections = structure.NoOfSections,
            NoOfChapters = structure.NoOfChapters,
            NoOfQuestions = structure.NoOfQuestions,
            MaxMarks = structure.MaxMarks
        };

        // Add questions and with its options
        var serialNo = 1;
        //LogHandler.LogInfo($"No of Details : {structure.StructureDetails.Count}");
        foreach (var structureDetail in structure.StructureDetails)
        {
            var chapterNos = structureDetail.ChapterNos?.Split(',');
            if (null == chapterNos || !chapterNos.Length.Equals(structureDetail.NofOptions))
                throw new Exception($"No. of options for Question No {structureDetail.SerialNo} doesn't match with chapter nos. count");
            for (var index = 0; index < structureDetail.NofOptions; index++)
            {
                var paperDetail = new PaperDetail
                {
                    SerialNo = serialNo++,
                    QuestionId = 0,
                    QuestionNo = structureDetail.QuestionNo,
                    SectionNo = structureDetail.SectionNo,
                    //QuestionNo = detail.SerialNo,
                    QuestionTypeId = structureDetail.QuestionTypeId,
                    ChapterId = chapterNos[index].ToInt(),
                    DifficultyLevel = 0,
                    Marks = structureDetail.Marks ?? 0
                };
                paper.PaperDetails.Add(paperDetail);
            }
        }
        return paper;
    }

    private static IEnumerable<string> GetSubstrings(IReadOnlyCollection<string> words)
    {
        for (var length = words.Count; length > 0; length--)
        {
            for (var start = 0; start <= words.Count - length; start++)
                yield return string.Join(" ", words.Skip(start).Take(length)).ToLower();
        }
    }

    private static int GetTaxonomyLevel(IEnumerable<MiscMaster> taxonomies, string question)
    {
        var words = question.Split(' ');

        var miscMasters = taxonomies.ToList();
        foreach (var word in words)
        {
            /*var result = miscMasters.FirstOrDefault(p =>
                p.Name.Trim().ToLower().Contains(word.Trim().ToLower()));*/
            var result = miscMasters.FirstOrDefault(p =>
                p.Name.Trim().ToLower().Equals(word.Trim().ToLower(), StringComparison.OrdinalIgnoreCase));
            if (null != result)
                return result.SerialNo ?? 0;
        }

        return 0;
    }

    private string SanitizeName(string name)
    {
        if (string.IsNullOrEmpty(name)) return string.Empty;

        name = Regex.Replace(name, @"[\\\/:*?""<>|]", ""); // Remove illegal characters
        name = Regex.Replace(name, @"[\t\n\r]", "");       // Remove tabs and newlines
        return name.Trim();
    }

    #endregion

    #region -- Public Methods --
    public Paper GetById(int id)
    {
        return _cornoService.PaperRepository.GetById(id);
    }

    private Paper GetExisting(QuestionBankModel model, string status)
    {
        var existing = _cornoService.PaperRepository.Get(s => (s.FacultyId ?? 0) == (model.FacultyId ?? 0) &&
                                                              (s.CourseId ?? 0) == (model.CourseId ?? 0) && (s.CoursePartId ?? 0) == (model.CoursePartId ?? 0) &&
                                                              (s.BranchId ?? 0) == (model.BranchId ?? 0) && (s.SubjectId ?? 0) == (model.SubjectId ?? 0) &&
                                                              s.Status == status).FirstOrDefault();
        return existing;
    }

    public Paper GetQuestions(Paper model)
    {
        // Validate fields
        ValidateHeaderFields(model);

        var paper = CreatePaper(model);

        return paper;
    }

    public void ValidateFields(Paper paper, bool validateDetails = true)
    {
        // Validate header fields
        ValidateHeaderFields(paper);

        if (paper.NoOfSections.ToInt() <= 0)
            throw new Exception("Invalid No. Of Sections.");
        if (paper.NoOfChapters.ToInt() <= 0)
            throw new Exception("Invalid No. Of Chapters.");
        if (paper.NoOfQuestions.ToInt() <= 0)
            throw new Exception("Invalid No. Of Questions.");
        if (paper.MaxMarks.ToInt() <= 0)
            throw new Exception("Invalid Max Marks.");

        if (!validateDetails) return;

        if (paper.PaperDetails.Count <= 0)
            throw new Exception("No rows in question paper.");
        if (paper.PaperDetails.All(d => null == d.QuestionNo))
            throw new Exception("All rows are blank.");
        /*if (paper.PaperDetails.Any(d => d.SectionNo > paper.NoOfSections))
            throw new Exception("Section no in one of the row is greater than No. Of Sections.");*/
        if (paper.PaperDetails.Any(d => (d.Marks ?? 0) <= 0))
            throw new Exception("Marks in one of the row is cannot be less or equal to zero.");
    }

    public int GetTaxonomy(string question)
    {
        var taxonomies = _miscMasterService.Get(p => p.MiscType == MiscConstants.Taxonomy,
            p => p).ToList();
        return GetTaxonomyLevel(taxonomies, question);
    }

    public void UpdateTaxonomy(Paper paper)
    {
        var taxonomies = _miscMasterService.Get(p => p.MiscType == MiscConstants.Taxonomy,
            p => p).ToList();
        foreach (var detail in paper.PaperDetails)
        {
            if (!string.IsNullOrEmpty(detail.Description))
                detail.Taxonomy = GetTaxonomyLevel(taxonomies, detail.Description);
        }
    }

    public void Save(Paper paper)
    {
        // Validate Fields
        ValidateFields(paper);

        // Update difficulty levels from taxonomy
        UpdateTaxonomy(paper);

        var existing = GetExisting(paper, StatusConstants.Active);
        if (null != existing)
        {
            existing.PaperDetails.ForEach(d =>
            {
                var newDetail = paper.PaperDetails.FirstOrDefault(x => x.Id == d.Id);

                d.QuestionNo = newDetail?.QuestionNo;
                d.SectionNo = newDetail?.SectionNo;
                d.ChapterId = newDetail?.ChapterId;
                d.DifficultyLevel = newDetail?.DifficultyLevel;
                d.QuestionTypeId = newDetail?.QuestionTypeId;
                //d.QuestionTypeName = GetQuestionTypeName(d.QuestionTypeId);

                d.Description = newDetail?.Description;
                d.Marks = newDetail?.Marks;
            });

            _cornoService.PaperRepository.Update(existing);
        }
        else
            _cornoService.PaperRepository.Add(paper);

        //paper.PaperDetails.ForEach(d => d.QuestionTypeName = GetQuestionTypeName(d.QuestionTypeId));

        _cornoService.Save();
    }

    private Paper CreatePaper(ReportModel reportModel, int? paperCategoryId, int paperSerialNo)
    {
        var paperDto = new Paper
        {
            InstanceId = reportModel.Instance.Id,
            FacultyId = reportModel.Faculty.Id,
            CourseId = reportModel.Course.Id,
            CoursePartId = reportModel.CoursePart.Id,
            BranchId = reportModel.Branch.Id,
            SubjectId = reportModel.Subject.Id,
            PaperCategoryId = paperCategoryId
        };

        var paper = GetQuestions(paperDto);

        ValidateFields(paper);

        // Update difficulty levels from taxonomy
        UpdateTaxonomy(paper);

        paper.SerialNo = paperSerialNo;

        return paper;
    }

    public void Create(ReportModel reportModel, int setsToBeDrawn)
    {
        var subjectService = Bootstrapper.Bootstrapper.Get<ISubjectService>();
        var subject = subjectService.GetById(reportModel.Subject.Id);
        var theoryCategories = new List<int> { 2, 48 };
        foreach (var subjectCategoryDetail in subject.SubjectCategoryDetails.Where(d => theoryCategories.Contains(d.CategoryId)).Distinct())
        {
            var existingPapers = Get(p => p.InstanceId == reportModel.Instance.Id &&
                                          p.SubjectId == reportModel.Subject.Id &&
                    p.PaperCategoryId == subjectCategoryDetail.PaperCategoryId, p => new { p.SerialNo })
                .ToList();

            var serialNo = (existingPapers.Max(p => p.SerialNo) ?? 0) + 1;
            setsToBeDrawn -= existingPapers.Count;

            for (var index = 0; index < setsToBeDrawn; index++)
            {
                var paper = CreatePaper(reportModel, subjectCategoryDetail.PaperCategoryId, serialNo);

                Add(paper);
                serialNo++;
            }
        }

        Save();
    }

    public RadFlowDocument Draw(Paper paper)
    {
        // Validation
        //if (paper.PaperDetails.Any(d => (d.DifficultyLevel ?? 0) <= 0))
        //    throw new Exception("One of more questions doesn't have difficulty level.");
        if (paper.PaperDetails.Any(d => string.IsNullOrEmpty(d.Description)))
            throw new Exception("One of more questions doesn't have description.");

        //var existing = GetExisting(paper, StatusConstants.Active);
        var existing = GetById(paper.Id);
        if (null == existing)
            throw new Exception("Question paper is either not available or already drawn.");

        var document = _documentService.CreateWordFile(existing);

        /*// Create word file
        var document = CreateWordFile(paper);*/

        /*// Save the document
            var filePath = @"E:\Home\Bharti Vidyapeeth\Online Exam\Development\Source\Presentation\Corno.OnlineExam\Content\UserFiles\Papers\YourDocument.docx";
            using var stream = new FileStream(filePath, FileMode.Create);
            var provider = new DocxFormatProvider();
            provider.Export(document, stream);*/

        return document;
    }

    public RichEditDocumentServer DrawRichEdit(Paper paper)
    {
        /*if (paper.PaperDetails.Any(d => string.IsNullOrEmpty(d.Description)))
            throw new Exception("One of more questions doesn't have description.");*/

        var existing = GetById(paper.Id);
        if (null == existing)
            throw new Exception("Question paper is either not available or already drawn.");

        var documentServer = _richEditDocumentService.CreateWordFile(existing);

        return documentServer;
    }

    public string GetQuestionTypeName(int? questionTypeId)
    {
        return null == questionTypeId
            ? string.Empty
            : ((QuestionType)questionTypeId).ToString().SplitPascalCase();
    }

    public void UploadModelAnswer(HttpPostedFileBase pdfModelAnswer, string basePath, Paper paper)
    {
        if (pdfModelAnswer is not { ContentLength: > 0 })
            throw new Exception("No file uploaded.");

        // Build the directory path
        var fullPath = Path.Combine(basePath, 
            "Faculty - " + paper.FacultyId, 
            "Course - " + paper.CourseId, 
            "Course Part - " + paper.CoursePartId, 
            "Subject - " + paper.SubjectId);

        // Create directories if they don't exist
        Directory.CreateDirectory(fullPath);

        // Define the file name and path
        var paperCategoryName = _miscMasterService.GetViewModel(paper.PaperCategoryId)?.NameWithId;
        var fileName = $"{paperCategoryName} - Set {paper.SerialNo}.pdf";
        var filePath = Path.Combine(fullPath, fileName);

        // Save the file
        pdfModelAnswer.SaveAs(filePath);
    }
    #endregion
}