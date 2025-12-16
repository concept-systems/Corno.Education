using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Data.Common;

namespace Corno.Data.Corno.Question_Bank.Models;

[Serializable]
public class QuestionBankModel : BaseModel
{
    #region -- Properties --

    [Required(ErrorMessage = "Faculty is required")]
    public int? FacultyId { get; set; }
    [Required(ErrorMessage = "Course is required")]
    public int? CourseId { get; set; }
    [Required(ErrorMessage = "Course Part is required")]
    public int? CoursePartId { get; set; }
    public int? BranchId { get; set; }

    [Required(ErrorMessage = "Subject is required")]
    public int? SubjectId { get; set; }
    [Required(ErrorMessage = "Paper Category is required")]

    public int? PaperCategoryId { get; set; }

    [NotMapped]
    public string FacultyName { get; set; }
    [NotMapped]
    public string CourseName { get; set; }
    [NotMapped]
    public string CoursePartName { get; set; }
    [NotMapped]
    public string BranchName { get; set; }

    [NotMapped]
    public string SubjectName { get; set; }
    [NotMapped]
    public string PaperCategoryName { get; set; }

    #endregion

    #region -- Methods --

    public override void Copy(BaseModel other)
    {
        if (other is not QuestionBankModel model) return;

        base.Copy(other);

        FacultyId = model.FacultyId;
        CourseId = model.CourseId;
        CoursePartId = model.CoursePartId;
        BranchId = model.BranchId;
        SubjectId = model.SubjectId;
        FacultyName = model.FacultyName;
        CourseName = model.CourseName;
        CoursePartName = model.CoursePartName;
        BranchName = model.BranchName;
        SubjectName = model.SubjectName;
        //Description = model.Description;
    }

    #endregion
}
