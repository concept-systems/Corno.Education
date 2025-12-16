using Corno.Data.Common;
using Mapster;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;

namespace Corno.Data.Corno.Question_Bank.Models;

[Serializable]
public class PaperDetail : BaseModel
{
    #region -- Properties --
    public int? PaperId { get; set; }

    public int? QuestionId { get; set; }            // Reference to question id in question table

    [Required(ErrorMessage = "Section No. is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Value must be greater than zero")]
    public int? SectionNo { get; set; }
    [Required(ErrorMessage = "Question No. is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Value must be greater than zero")]
    public int? QuestionNo { get; set; }            // Question no in structure 
    [Required(ErrorMessage = "Question Type is required")]
    public int? QuestionTypeId { get; set; }
    [Required(ErrorMessage = "Chapter is required")]
    public int? ChapterId { get; set; }
    [Required(ErrorMessage = "Difficulty Level is required")]
    public int? DifficultyLevel { get; set; }
    public int? LearningPriorityId { get; set; }
    public int? Taxonomy { get; set; }
    public int? CoNo { get; set; }

    //[Required(ErrorMessage = "Description required")]
    [AllowHtml] // Allow HTML content for this property
    public string Description { get; set; }
    [AllowHtml] // Allow HTML content for this property
    public string ModelAnswer { get; set; }
    public byte[] DocumentContent { get; set; }
    [Required(ErrorMessage = "Marks is required")]
    [Range(1, int.MaxValue, ErrorMessage = "Value must be greater than zero")]
    public double? Marks { get; set; }

    [NotMapped]
    public string QuestionTypeName { get; set; }

    [AdaptIgnore]
    protected virtual Paper Paper { get; set; }

    #endregion

    #region -- Public Methods --
    public void Copy(PaperDetail other)
    {
        if (null == other) return;

        SerialNo = other.SerialNo;
        Code = other.Code;

        QuestionNo = other.QuestionNo;
        ChapterId = other.ChapterId;
        DifficultyLevel = other.DifficultyLevel;
        LearningPriorityId = other.LearningPriorityId;
        CoNo = other.CoNo;
        Description = other.Description;
        ModelAnswer = other.ModelAnswer;
        DocumentContent = other.DocumentContent;
        Taxonomy = other.Taxonomy;
        Marks = other.Marks;
        
        Status = other.Status;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;
    }
    #endregion
}