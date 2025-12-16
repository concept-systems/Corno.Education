using System;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Data.Common;

namespace Corno.Data.Corno;

public class EnvironmentStudyCommon : BaseModel
{
    public string PrnNo { get; set; }
    public int? Year { get; set; }
    public int? Month { get; set; }
    public int? SubMarks { get; set; }
    public string SubSts { get; set; }
    public string SubRes { get; set; }
    public int? CourseId { get; set; }
    public int? CoursePartId { get; set; }
    public int? CollegeId { get; set; }
    public int? CenterId { get; set; }
    public int? InstanceId { get; set; }
    public double? EnvironmentFee { get; set; }
    public double? LateFee { get; set; }
    public double? SuperLateFee { get; set; }
    public double? OtherFee { get; set; }
    public double? TotalFee { get; set; }
    public string FeeInWord { get; set; }
    public string TransactionId { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime? SettlementDate { get; set; }
    public double? PaidAmount { get; set; }

    [NotMapped]
    public string Mobile { get; set; }
    [NotMapped]
    public string Email { get; set; }
    [NotMapped]
    public string StudentName { get; set; }
    [NotMapped]
    public string CollegeName { get; set; }
    [NotMapped]
    public string CenterName { get; set; }
    [NotMapped]
    public string CourseName { get; set; }
    [NotMapped]
    public string CoursePartName { get; set; }
    [NotMapped]
    public byte[] Photo { get; set; }
    [NotMapped]
    public DateTime? LateFeeDate { get; set; }
    [NotMapped]
    public DateTime? SuperLateFeeDate { get; set; }
}

public class EnvironmentStudy : EnvironmentStudyCommon
{
}