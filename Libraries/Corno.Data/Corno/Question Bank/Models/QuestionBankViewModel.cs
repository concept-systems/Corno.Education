using System;

namespace Corno.Data.Corno.Question_Bank.Models;

[Serializable]
public class QuestionBankViewModel
{
    public int? FacultyId { get; set; }
    public int? CourseId { get; set; }
    public int? CoursePartId { get; set; }
    public int? BranchId { get; set; }

    public int? SubjectId { get; set; }
}