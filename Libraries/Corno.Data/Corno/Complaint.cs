using System;
using Corno.Data.Common;

namespace Corno.Data.Corno;

public class ComplaintCommon : BaseModel
{
    public ComplaintCommon()
    {
        IsApproved = false;
    }
    public string Name { get; set; }
    public bool IsApproved { get; set; }
    public int? StaffId { get; set; }
    public DateTime? ComplaintDate { get; set; }
    public string ComplaintBy { get; set; }
    public string Description { get; set; }
    public string AssignedTo { get; set; }
    public string AssignedBy { get; set; }
    public DateTime? AssignedDate { get; set; }
    public string ResolvedBy { get; set; }
    public DateTime? ResolvedDate { get; set; }
    public string ClosedBy { get; set; }
    public DateTime? ClosedDate { get; set; }
}

public class Complaint : ComplaintCommon
{
    //public virtual Staff Staff { get; set; }

}

public class ComplaintViewModel : ComplaintCommon
{
    public string StaffName { get; set; }
    public string StaffName1 { get; set; }
    //public virtual ICollection<Staff> Staffs { get; set; }

}