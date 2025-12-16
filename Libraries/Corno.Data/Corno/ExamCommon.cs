using System;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Corno.Data.Common;
using Corno.Globals.Constants;

namespace Corno.Data.Corno;

public class ExamCommon : BaseModel
{
    public ExamCommon()
    {
        ApplyForImprovement = false;
        IsApproved = false;
        AttendanceFullfilled = false;
        if (null != HttpContext.Current.Session[ModelConstants.InstanceId])
            InstanceId = (int) HttpContext.Current.Session[ModelConstants.InstanceId];

        Date = DateTime.Now;
        ApprovalDate = DateTime.Now;

        ExamFee = 0;
        BacklogFee = 0;
        CapFee = 0;
        StatementOfMarksFee = 0;
        PracticalFee = 0;
        DissertationFee = 0;
        OthersFee = 0;
        LateFee = 0;
        SuperLateFee = 0;
        EnvironmentalExaminationFee = 0;
        CertificateOfPassingFee = 0;
        Total = 0;
    }

    public int InstanceId { get; set; }
    public string PrnNo { get; set; }
    public string SeatNo { get; set; }
    public int? CollegeId { get; set; }
    public int? CentreId { get; set; }
    public int? CourseId { get; set; }
    public int? StudentId { get; set; }
    public int? CoursePartId { get; set; }
    public int? BranchId { get; set; }
    //[Required]
    public string Bundle { get; set; }
    public string Pattern { get; set; }
    public string AadharNo { get; set; }
    public string Part { get; set; }
    [DataType(DataType.Date)]
    public DateTime? Date { get; set; }
    public string Activity { get; set; }
    public bool ApplyForImprovement { get; set; }
    public bool IsApproved { get; set; }
    public bool AttendanceFullfilled { get; set; }
    public DateTime? ApprovalDate { get; set; }
    public string Remark { get; set; }
    //[Required]
    public string FormNo { get; set; }

    // Fees
    public double? ExamFee { get; set; }
    public double? BacklogFee { get; set; }
    public double? CapFee { get; set; }
    public double? StatementOfMarksFee { get; set; }
    public double? PracticalFee { get; set; }
    public double? DissertationFee { get; set; }
    public double? OthersFee { get; set; }
    public double? LateFee { get; set; }
    public double? SuperLateFee { get; set; }
    public double? EnvironmentalExaminationFee { get; set; }
    public double? CertificateOfPassingFee { get; set; }
    public double? Total { get; set; }
    public string BacklogSummary { get; set; }
    public string FeeInWord { get; set; }
    public byte[] Photo { get; set; }

    public string TransactionId { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime? SettlementDate { get; set; }
    public double? PaidAmount { get; set; }


    // Online Education (College 45)
    public int? FeeId { get; set; }
    public double? RegularFee45 { get; set; }
    public double? BackLogFee45 { get; set; }
    public double? CapFee45 { get; set; }
    public double? StatementOfMarksFee45 { get; set; }
    public double? TotalFee45 { get; set; }
}