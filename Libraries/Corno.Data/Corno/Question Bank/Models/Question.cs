using System;
using System.ComponentModel.DataAnnotations;

namespace Corno.Data.Corno.Question_Bank.Models;

[Serializable]
public class Question : QuestionBankModel
{
    #region -- Constructors --
    public Question()
    {
        FacultyId = 0;
        CourseId = 0;
        CoursePartId = 0;
        BranchId = 0;
        PaperCategoryId = 0;
    }
    #endregion

    #region -- Properties --
    public DateTime? Date { get; set; } = DateTime.Now;

    public int? StaffId { get; set; }

    //[Required(ErrorMessage = "Question Type is required")]
    public int? QuestionTypeId { get; set; }
    //[Required(ErrorMessage = "Chapter is required")]
    public int? ChapterId { get; set; }
    //[Required(ErrorMessage = "Difficulty Level is required")]
    public int? DifficultyLevel { get; set; }
    public int? LearningPriorityId { get; set; }

    public string Description { get; set; }
    public string ModelAnswer { get; set; }
    //[Required(ErrorMessage = "Marks is required")]
    //[Range(1, int.MaxValue, ErrorMessage = "Value must be greater than zero")]
    public double? Marks { get; set; }
    public int? CoNo { get; set; }
    public int? TaxonomySerialNo { get; set; }
    #endregion

    #region -- Methods --
    public void Clear()
    {
        Id = 0;
        FacultyId = 0;
        CourseId = 0;
        CoursePartId = 0;
        BranchId = 0;
        Date = DateTime.Now;
        ChapterId = 0;
        DifficultyLevel = -1;
        LearningPriorityId = -1;
        Description = string.Empty;
        Marks = 0;
        CoNo = 0;
    }
    #endregion
}