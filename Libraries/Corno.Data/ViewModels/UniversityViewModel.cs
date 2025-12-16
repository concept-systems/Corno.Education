using Corno.Data.Common;

namespace Corno.Data.ViewModels;

public class UniversityViewModel : UniversityBaseModel
{
    public string InstanceName { get; set; }
    public string FacultyName { get; set; }
    public string CollegeName { get; set; }
    public string CentreName { get; set; }
    public string CourseName { get; set; }
    public string CourseTypeName { get; set; }
    public string CoursePartName { get; set; }
    public string BranchName { get; set; }
    public string CategoryName { get; set; }
    public string SubjectName { get; set; }
    public string StudentName { get; set; }
}