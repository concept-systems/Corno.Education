using System;

namespace Corno.Data.ViewModels.Appointment;

public class ReportBillModel
{
    #region -- Properties --

    public int? BillNo { get; set; }
    public DateTime? BillDate { get; set; }
    public double? BasicPay { get; set; }
    public double? Ta { get; set; }
    public double? Da { get; set; }
    public double? Travel { get; set; }
    public double? AllottedFee { get; set; }
    public double? CourseFee { get; set; }
    public double? ChairmanAllowance { get; set; }
    public double? TotalFee { get; set; }
    public string FromPlace { get; set; }
    public string ToPlace { get; set; }
    public double? TotalDistance { get; set; }
    public string TravelingMode { get; set; }
    public bool IsHindi { get; set; }
    public double? HindiFee { get; set; }
    public bool IsMarathi { get; set; }
    public double? MarathiFee { get; set; }
    public bool IsSanskrit { get; set; }
    public double? SanskritFee { get; set; }
    public double? ModelAnswersFee { get; set; }
    public int? SetsDrawn { get; set; }

    public int? Remuneration { get; set; }
    public int? RemunerationOthers { get; set; }
    public int? ModelAnswers { get; set; }
    #endregion
}

public class ReportBankModel
{
    public ReportBankModel()
    {
        IfscFlag = 0;
    }

    public string BankName { get; set; }
    public string BankBranch { get; set; }
    public string BankAccountNo { get; set; }
    public string IfscCode { get; set; }
    public int? IfscFlag { get; set; }
}

public class CourseReportModel
{
    public CourseReportModel()
    {
        CourseId = 0;
    }

    public int CourseId { get; set; }
    public string CourseCode { get; set; }
    public string CourseName { get; set; }
}