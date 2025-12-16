namespace Corno.Data.Corno.Question_Bank.Dtos;

public class EmailSmsDto
{
    public string InstanceName { get; set; }
    public string CourseName { get; set; }
    public string CoursePartName { get; set; }
    public int? SubjectId { get; set; }
    public string SubjectName { get; set; }
    public int? SetsToBeDrawn { get; set; }
    public int? StaffId { get; set; }
    public string StaffName { get; set; }
    public string MobileNo { get; set; }
    public string EmailId { get; set; }
    public bool? IsSetter { get; set; }
    public bool? IsChecker { get; set; }
}