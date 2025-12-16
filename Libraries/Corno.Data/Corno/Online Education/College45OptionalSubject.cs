using System;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Data.Common;

namespace Corno.Data.Corno.Online_Education;

[Table("College45OptionalSubject")]
public class College45OptionalSubject : BaseModel
{
    public int? SrNo { get; set; }
    public int InstanceId { get; set; }
    public int CourseId { get; set; }
    public int CoursePartId { get; set; }
    public long BranchId { get; set; }
    public string Prn { get; set; }
    public int ElectiveSubject1 { get; set; }
    public int? ElectiveSubject2 { get; set; }
    public int? ElectiveSubject3 { get; set; }
    public int? ElectiveSubject4 { get; set; }
    public int? ElectiveSubject5 { get; set; }
    public int? ElectiveSubject6 { get; set; }
    public int? ElectiveSubject7 { get; set; }
}
