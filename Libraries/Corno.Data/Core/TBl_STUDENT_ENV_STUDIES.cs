using System;

namespace Corno.Data.Core;

public partial class TBl_STUDENT_ENV_STUDIES
{
    public int Num_PK_ENTRY_ID { get; set; }
    public short? Num_FK_DistCenter_ID { get; set; }
    public short Num_FK_CO_CD { get; set; }
    public short? Num_FK_COPRT_NO { get; set; }
    public short? Num_FK_COL_CD { get; set; }
    public string Chr_FK_PRN_NO { get; set; }
    public short? Num_ST_SUB_MRK { get; set; }
    public string Chr_ST_SUB_STS { get; set; }
    public string Chr_ST_SUB_RES { get; set; }
    public short? Num_MONTH_NO { get; set; }
    public string Num_YEAR { get; set; }
    public short? Num_ST_ORD_MRK { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public string Var_Bundle { get; set; }
    public short? Num_FK_INST_NO { get; set; }
    public double? Num_EnviFee { get; set; }
    public double? Num_EnviLateFee { get; set; }
    public double? Num_EnviSuperLateFee { get; set; }
    public double? Num_EnviOtherFee { get; set; }
    public double? Num_EnviTotalFee { get; set; }
    public string Chr_Transaction_Id { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime? SettlementDate { get; set; }
    public double? PaidAmount { get; set; }
}