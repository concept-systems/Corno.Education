using System;

namespace Corno.Data.Corno.Question_Bank.Dtos;

[Serializable]
public class QuestionAppointmentTypeDetailDto
{
    public int Id { get; set; }
    public int? QuestionAppointmentId { get; set; }

    public int? PaperCategoryId { get; set; }
    public string PaperCategoryName { get; set; }
    public int? QuestionTypeId { get; set; }
    public string QuestionTypeName { get; set; }
    public int? QuestionCount { get; set; }
    public int? CompletedCount { get; set; }
}