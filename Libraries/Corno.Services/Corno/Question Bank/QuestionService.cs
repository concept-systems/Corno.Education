using System;
using System.Linq;
using System.Text.RegularExpressions;
using System.Web;
using Corno.Data.Corno.Question_Bank.Models;
using Corno.Globals.Constants;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;

namespace Corno.Services.Corno.Question_Bank;

public class QuestionService : MainService<Question>, IQuestionService
{
    #region -- Constructors --
    public QuestionService(ISubjectService subjectService, IPaperService paperService, IQuestionAppointmentService appointmentService)
    {
        _subjectService = subjectService;
        _paperService = paperService;
        _appointmentService = appointmentService;
    }
    #endregion

    #region -- Data Members --

    private readonly ISubjectService _subjectService;
    private readonly IPaperService _paperService;
    private readonly IQuestionAppointmentService _appointmentService;

    #endregion

    #region -- Private Methods --

    private static void ValidateHeader(Question question)
    {
        if ((question.SubjectId ?? 0) <= 0)
            throw new Exception("Invalid Subject.");
    }
    
    private Question GetExisting(Question question)
    {
        return GetById(question.Id);
    }

    private static string ToPlainText(string html)
    {
        if (string.IsNullOrWhiteSpace(html)) return string.Empty;
        // Remove HTML tags
        var noTags = Regex.Replace(html, "<.*?>", string.Empty);
        // Decode HTML entities
        return HttpUtility.HtmlDecode(noTags)?.Trim() ?? string.Empty;
    }
    #endregion

    #region -- Public Methods --

    public void ValidateFields(Question question)
    {
        // Validate header
        ValidateHeader(question);

        if(string.IsNullOrEmpty(question.Description))
            throw new Exception("Question cannot be empty.");
        if(string.IsNullOrEmpty(question.ModelAnswer))
            throw new Exception("Model Answer cannot be empty.");

        if ((question.ChapterId ?? 0) <= 0)
            throw new Exception("Invalid Chapter.");
        if ((question.DifficultyLevel ?? 0) <= 0)
            throw new Exception("Invalid Difficulty level.");
        if ((question.QuestionTypeId ?? 0) <= 0)
            throw new Exception("Invalid Question Type.");
        if ((question.LearningPriorityId ?? 0) <= 0)
            throw new Exception("Invalid Learning Priority.");
        if ((question.Marks ?? 0) <= 0)
            throw new Exception("Invalid Marks.");
    }

    public void SaveQuestion(Question question, int staffId, int instanceId, bool isEdit)
    {
        // Validate
        ValidateFields(question);

        // Assign staff
        question.StaffId = staffId;

        // Compute taxonomy
        var plainText = ToPlainText(question.Description);
        question.TaxonomySerialNo = _paperService.GetTaxonomy(plainText);

        // Duplicate check
        var existingQuestions = Get(p => p.SubjectId == question.SubjectId && p.Status == StatusConstants.Active, p => p)
            .AsEnumerable();
        if (isEdit)
            existingQuestions = existingQuestions.Where(q => q.Id != question.Id);
        if (existingQuestions.Any(q => ToPlainText(q.Description)
            .Equals(plainText, StringComparison.OrdinalIgnoreCase)))
            throw new Exception("Similar question already exists.");

        // Update appointment completed count only for create
        if (!isEdit)
            _appointmentService.UpdateCompletedCount(question, instanceId);

        // Persist
        if (isEdit)
            UpdateAndSave(question);
        else
            AddAndSave(question);
    }

    public void Accept(Question question)
    {
        // Validate fields
        ValidateFields(question);

        var existing = GetExisting(question);
        if (null == existing)
            throw new Exception("Question not found.");

        existing.ModifiedDate = DateTime.Now;
        existing.Status = StatusConstants.Accepted;

        UpdateAndSave(existing);
    }

    public void Reject(Question question)
    {
        // Validate fields
        ValidateFields(question);

        var existing = GetExisting(question);
        if (null == existing)
            throw new Exception("Question not found.");

        existing.ModifiedDate = DateTime.Now;
        existing.DeletedDate = DateTime.Now;
        existing.Status = StatusConstants.Rejected;

        UpdateAndSave(existing);
    }
    #endregion
}