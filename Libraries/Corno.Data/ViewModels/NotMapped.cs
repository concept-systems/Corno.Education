using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corno.Data.ViewModels;

public class NotMapped
{
    public NotMapped()
    {
        Branches = new List<MasterViewModel>();
    }

    [NotMapped]
    public int Index { get; set; }

    // University 
    [NotMapped]
    public string InstanceName { get; set; }
    [NotMapped]
    public string CollegeName { get; set; }
    [NotMapped]
    public string CenterName { get; set; }
    [NotMapped]
    public string CourseName { get; set; }
    [NotMapped]
    public string CourseTypeName { get; set; }
    [NotMapped]
    public string CoursePartName { get; set; }
    [NotMapped]
    public List<MasterViewModel> Branches { get; set; }
    [NotMapped]
    public string BranchName { get; set; }
    [NotMapped]
    public bool BranchApplicable { get; set; }

    [NotMapped]
    public bool DegreeApplicable { get; set; }

    // Student Info
    [NotMapped]
    public string Prn { get; set; }
    [NotMapped]
    public string StudentName { get; set; }
    [NotMapped]
    public string AadhaarNo { get; set; }
    [NotMapped]
    public string SubjectName { get; set; }
    [NotMapped]
    public int OptionalIndex { get; set; }
    [NotMapped]
    public int MaxOptionalCount { get; set; }

    // Fee Info
    [NotMapped]
    public byte[] Photo { get; set; }
    [NotMapped]
    public bool ShowBranchCombo { get; set; }

    [NotMapped]
    public bool Hide { get; set; }
}