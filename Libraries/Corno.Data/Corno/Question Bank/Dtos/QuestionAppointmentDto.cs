using System;
using System.Collections.Generic;

namespace Corno.Data.Corno.Question_Bank.Dtos;

[Serializable]
public class QuestionAppointmentDto
{
    public int Id { get; set; }
    public int? InstanceId { get; set; }
    public int? FacultyId { get; set; }
    public int? CollegeId { get; set; }
    public int? CentreId { get; set; }
    public int? CourseId { get; set; }
    public int? CoursePartId { get; set; }
    public int? CourseTypeId { get; set; }
    public int? BranchId { get; set; }

    public int? CategoryId { get; set; }
    public int? SubjectId { get; set; }

    public int SetsToBeDrawn { get; set; }

    public List<QuestionAppointmentDetailDto> QuestionAppointmentDetailDtos { get; set; } = new();
    public List<QuestionAppointmentTypeDetailDto> QuestionAppointmentTypeDetailDtos { get; set; } = new();
}