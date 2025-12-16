using System.Collections.Generic;
using Corno.Data.Common;

namespace Corno.Data.Corno;

public class RevalutionCommon : BaseModel
{
    public string PrnNo { get; set; }
    public long? SeatNo { get; set; }
    public int CollegeId { get; set; }
    public int CourseId { get; set; }
    public int CoursePartId { get; set; }
    public bool IsRevaluation { get; set; }
    public bool IsVerification { get; set; }
}
public sealed class Revalution : RevalutionCommon
{
    public Revalution()
    {
        RevalutionSubjects = new List<RevalutionSubject>();
    }
    public ICollection<RevalutionSubject> RevalutionSubjects { get; set; }
}
public sealed class RevalutionViewModel : RevalutionCommon
{
    public RevalutionViewModel()
    {
        RevalutionSubjectViewModels = new List<RevalutionSubjectViewModel>();
    }
    public int? CentreId { get; set; }
    public int? BranchId { get; set; }
    public string CollegeName { get; set; }
    public string CourseName { get; set; }
    public string CoursePartName { get; set; }
    public string SubjectName { get; set; }
    public string StudentName { get; set; }
    public string CentreName { get; set; }
    public string CourseTypeName { get; set; }
    public string BranchName { get; set; }
    public string Photo { get; set; }
    public ICollection<RevalutionSubjectViewModel> RevalutionSubjectViewModels { get; set; }
}

public class RevalutionIndex
{
    public int? Id { get; set; }
    // public string PRNNo { get; set; }
    public string Status { get; set; }
    public string CollegeName { get; set; }
    public string CourseName { get; set; }
    public long SeatNo { get; set; }
    public bool IsApproved { get; set; }
}