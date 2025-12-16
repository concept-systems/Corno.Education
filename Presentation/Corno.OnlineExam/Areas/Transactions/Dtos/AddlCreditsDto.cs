using System.Collections.Generic;

namespace Corno.OnlineExam.Areas.Transactions.Dtos;

public class AddlCreditsDto
{
    public string Prn { get; set; }
    public string StudentName { get; set; }
    public int InstanceId { get; set; }
    public string InstanceName { get; set; }
    public int CollegeId { get; set; }
    public string CollegeName { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public int BranchId { get; set; }
    public string BranchName { get; set; }
    public List<AddlCreditsSubjectDto> Subjects { get; set; } = [];
}