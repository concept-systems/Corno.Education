using System;
using Corno.Data.Common;

namespace Corno.Data.Corno.Masters;

[Serializable]
public class CoursePart : MasterModel
{
    public string ShortName { get; set; }
    public int? FacultyId { get; set; }
    public int? CourseId { get; set; }
    public int? PartNo { get; set; }
    public int? SemesterNo { get; set; }
    public int? TotalSubjects { get; set; }
    public int? TotalYears { get; set; }
    public int? TotalAttempts { get; set; }
    public int? PassPercentage { get; set; }
    public bool? IsBranchApplicable { get; set; }
}