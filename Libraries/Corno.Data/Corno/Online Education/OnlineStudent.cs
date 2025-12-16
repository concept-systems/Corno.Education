using Corno.Data.Common;

namespace Corno.Data.Corno.Online_Education;

public class OnlineStudent : BaseModel
{
    public string Prn { get; set; }
    public int? CollegeId { get; set; }
    public int? CourseId { get; set; }
    public int? CoursePartId { get; set; }
    public int? BranchId { get; set; }
    public int? MonthOfAdmission { get; set; }
    public int? FeeId { get; set; }
    public double RegularFee { get; set; }
    public double BacklogFee { get; set; }
}

/*public class OnlineStudent : MasterModel
{
    public string Prn { get; set; }
    public string Gender { get; set; }
    public string Mobile { get; set; }
    public string Course { get; set; }
    public string AdmissionSession { get; set; }
    public string Batch { get; set; }
    public string StudentType { get; set; }
    public double? RegularFee { get; set; }
    public double? BacklogFee { get; set; }
    public double? CapFee { get; set; }
    public double? StatementFee { get; set; }
    public double? PassingFee { get; set; }
    public double? LateFee { get; set; }
    public double? SuperLateFee { get; set; }
    public double? Fine { get; set; }
    public double? RevaluationFee { get; set; }
    public double? VerificationFee { get; set; }
    public double? OtherFee { get; set; }
    public bool RegularFeeShow { get; set; }
    public bool BackLogFeeShow { get; set; }
    public bool OtherFeeShow { get; set; }
}*/