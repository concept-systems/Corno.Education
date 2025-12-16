using System;

namespace Corno.Data.Common;

[Serializable]
public class UniversityBaseModel : BaseModel
{
    public int? InstanceId { get; set; }

    public int? FacultyId { get; set; }
    public int? CollegeId { get; set; }
    public int? CentreId { get; set; }
    public int? CourseId { get; set; }
    public int? CoursePartId { get; set; }
    public int? CourseTypeId { get; set; }
    public int? BranchId { get; set; }
    /*public int? CategoryId { get; set; }
    public int? SubjectId { get; set; }*/
}