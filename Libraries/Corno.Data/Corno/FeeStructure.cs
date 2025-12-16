using System;

namespace Corno.Data.Corno;

public class FeeStructure
{
    public FeeStructure()
    {
        ExamFee = 0;
        BacklogFee = 0;
        CapFee = 0;
        StatementOfMarksFee = 0;
        PracticalFee = 0;
        DissertationFee = 0;
        OthersFee = 0;
        LateFee = 0;
        SuperLateFee = 0;
        EnvironmentExamFee = 0;
        CertificateOfPassingFee = 0;
        Total = 0;

        BacklogSummary = string.Empty;
        LateFeeDate = null;
        SuperLateFeeDate = null;
    }

    public double ExamFee { get; set; }
    public double BacklogFee { get; set; }
    public double CapFee { get; set; }
    public double StatementOfMarksFee { get; set; }
    public double PracticalFee { get; set; }
    public double DissertationFee { get; set; }
    public double OthersFee { get; set; }
    public double LateFee { get; set; }
    public double SuperLateFee { get; set; }
    public double CertificateOfPassingFee { get; set; }
    public double Total { get; set; }
    public string BacklogSummary { get; set; }
    public DateTime? LateFeeDate { get; set; }
    public DateTime? SuperLateFeeDate { get; set; }
    public double EnvironmentExamFee { get; set; }
    public double EnvironmentLateFee { get; set; }
    public double EnvironmentSuperLateFee { get; set; }
    public DateTime? EnvironmentLateFeeDate { get; set; }
    public DateTime? EnvironmentSuperLateFeeDate { get; set; }

    #region -- Methods --
    public double GetTotal()
    {
        return ExamFee + BacklogFee + CapFee + StatementOfMarksFee + PracticalFee + DissertationFee + OthersFee + LateFee + SuperLateFee + CertificateOfPassingFee;
        //return ExamFee + BacklogFee + CAPFee + StatementOfMarksFee + PracticalFee + DissertationFee + OthersFee + LateFee + SuperLateFee + EnvironmentalExaminationFee + CertificateOfPassingFee;
    }
    #endregion
}