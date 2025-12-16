
using Mapster;

namespace Corno.Data.Corno;

public class StudentProfileDetail
{
    public string InstanceName { get; set; }
    public string Exam { get; set; }
    public string SeatNo { get; set; }
    public string CourseName { get; set; }
    public string CoursePartName { get; set; }
    public string BranchName { get; set; }
    public string Result { get; set; }
    public string ImpFlg { get; set; }
    public string CollegeName { get; set; }
    public string CentreName { get; set; }
    public double? Sgpa { get; set; }
    public double? Cgpa { get; set; }

    [AdaptIgnore]
    public virtual StudentProfile StudentProfile { get; set; }
}