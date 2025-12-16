using System;
using System.ComponentModel.DataAnnotations;

namespace Corno.Data.Core;

public partial class Tbl_APP_TEMP
{
    [Key]
    public int Num_PK_ENTRY_ID { get; set; }
    public int Num_FORM_ID { get; set; }
    public string Chr_APP_VALID_FLG { get; set; }
    public string Chr_APP_PRN_NO { get; set; }
    public short Num_FK_COPRT_NO { get; set; }
    public short? Num_FK_INST_NO { get; set; }
    public short? Num_FK_BR_CD { get; set; }
    public short Num_FK_COLLEGE_CD { get; set; }
    public short Num_FK_CENTER_CD { get; set; }
    public string Chr_BUNDAL_NO { get; set; }
    public short? Num_FK_STUDCAT_CD { get; set; }
    public short? Num_FK_STACTV_CD { get; set; }
    public string DELETE_FLG { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public string Chr_REPEATER_FLG { get; set; }
    public string Chr_IMPROVEMENT_FLG { get; set; }
    public string Chr_College_Chnage { get; set; }
    public string Chr_Branch_Chnage { get; set; }
    public short? Num_FK_DistCenter_ID { get; set; }

    public double? Num_ExamFee { get; set; }
    public double? Num_BacklogFee { get; set; }
    public double? Num_PassingCertificateFee { get; set; }
    public double? Num_CAPFee { get; set; }
    public double? Num_StatementFee { get; set; }
    public double? Num_LateFee { get; set; }
    public double? Num_SuperLateFee { get; set; }
    public double? Num_Fine { get; set; }
    public double? Num_DissertationFee { get; set; }
    public double? Num_TotalFee { get; set; }
    public string Chr_Fee_Submit { get; set; }
    public string AadharNo { get; set; }
    public string Num_Transaction_Id { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime? SettlementDate { get; set; }
    public double? PaidAmount { get; set; }

    public string Form_at_Exam { get;}

    // Online Education (College 45)
    public int? FeeId { get; set; }
    public double? Num_RegularFee45 { get; set; } = 0;
    public double? Num_BacklogFee45 { get; set; } = 0;
    public double? Num_CapFee45 { get; set; } = 0;
    public double? Num_StatementFee45 { get; set; } = 0;
    public double? Num_TotalFee45 { get; set; } = 0;
}