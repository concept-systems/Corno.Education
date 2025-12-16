namespace Corno.Data.Dtos.Report;

public class BaseReportDto
{
    public int InstanceId { get; set; }
    public int? CollegeId { get; set; }
    public int? CenterId { get; set; }
    public int? CourseId { get; set; }
    public int? CoursePartId { get; set; }
    public int? BranchId { get; set; }
    public int? SubjectId { get; set; }
    public int? CategoryId { get; set; }
    public int? StaffId { get; set; }

    public string InstanceName { get; set; }
    public string CollegeName { get; set; }
    public string CenterName { get; set; }
    public string CourseName { get; set; }
    public string CoursePartName { get; set; }
    public string BranchName { get; set; }
    public string CategoryName { get; set; }
    public string SubjectName { get; set; }
    public string StaffName { get; set; }
}