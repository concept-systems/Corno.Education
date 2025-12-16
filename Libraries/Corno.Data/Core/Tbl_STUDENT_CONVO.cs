using System;

namespace Corno.Data.Core;

public partial class Tbl_STUDENT_CONVO
{
    public int NUM_PK_RECORD_ID { get; set; }
    public string Num_ST_BUN_NO { get; set; }
    public int? Num_ST_SR_NO { get; set; }
    public int? Num_ST_FRM_NO { get; set; }
    public string Chr_ST_YEAR { get; set; }
    public int? Num_FK_CO_CD { get; set; }
    public string Chr_ST_PA_FLG { get; set; }
    public string Chr_FK_PRN_NO { get; set; }
    public long? Num_ST_SEAT_NO { get; set; }
    public string Var_ST_NM { get; set; }
    public string Chr_ST_SEX_CD { get; set; }
    public string Chr_ST_ADD1 { get; set; }
    public string Chr_ST_ADD2 { get; set; }
    public string Chr_ST_ADD3 { get; set; }
    public string Chr_ST_ADD4 { get; set; }
    public string Chr_ST_PINCODE { get; set; }
    public string Var_RES_PH { get; set; }
    public string Var_E_MAIL { get; set; }
    public short? Num_FK_CONVO_NO { get; set; }
    public short? Num_ST_PASS_MONTH { get; set; }
    public string Chr_ST_PASS_YEAR { get; set; }
    public short? Num_ST_PASS_MONTH1 { get; set; }
    public string Chr_ST_PASS_YEAR1 { get; set; }
    public short Num_FK_RESULT_CD { get; set; }
    public int? Num_ST_CONVO_NO { get; set; }
    public string Chr_ST_NMCHNG_FLG { get; set; }
    public string Chr_ST_VALID_FLG { get; set; }
    public int? Num_FK_COLLEGE_CD { get; set; }
    public int? Num_FK_BR_CD { get; set; }
    public string Chr_FEES_STATUS { get; set; }
    public int? Con_ST_FEES_AMT { get; set; }
    public short? Num_FK_FA_CD { get; set; }
    public string Chr_PRINC_SUBJECT { get; set; }
    public string Chr_PRINC_SUBJECT1 { get; set; }
    public string Var_ST_USR_NM { get; set; }
    public DateTime? ST_DTE_CR { get; set; }
    public DateTime? ST_DTE_UP { get; set; }
    public string Chr_DELETE_FLG { get; set; }
    public short? Num_FK_INST_NO { get; set; }
    public string Chr_Convo_Sts { get; set; }
    public byte[] Ima_ST_PHOTO { get; set; }
    public string Chr_Foreign_Student { get; set; }
    public string Chr_Improvement { get; set; }
    public short? DegreePart { get; set; }
    public string Destination { get; set; }
    public decimal? Num_CGPA_AVG { get; set; }
    public string AdharNo { get; set; }
    public string Chr_Form_Submit { get; set; }
    public int? Num_Fk_Centre_cd { get; set; }
    public string Chr_Transaction_Id { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime? SettlementDate { get; set; }
    public double? PaidAmount { get; set; }

    public string Var_FATHER_NAME { get; set; }
    public string Var_MOTHER_NAME { get; set; }
    public short? Num_ST_PASS_MONTH_INT { get; set; }
    public string Chr_ST_PASS_YEAR_INT { get; set; }

    public string Form_at_Exam { get; set; }

}