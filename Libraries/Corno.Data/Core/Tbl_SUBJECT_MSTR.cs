using System;

namespace Corno.Data.Core;

public partial class Tbl_SUBJECT_MSTR
{
    public short Num_FK_COPRT_NO { get; set; }
    public int Num_PK_SUB_CD { get; set; }
    public string Var_SUBJECT_NM { get; set; }
    public string Var_SUBJECT_SHRT_NM { get; set; }
    public short Num_SUBJECT_MAX_MRK { get; set; }
    public double Num_SUBJECT_PASS_MRK { get; set; }
    public short Num_SUBJECT_EXMP_MRK { get; set; }
    public short Num_SUBJECT_GRD_MIN { get; set; }
    public int Num_SUBJECT_PRINT_SEQ { get; set; }
    public short? Num_FK_BOS_CD { get; set; }
    public string CHR_WRT_UNI_APL { get; set; }
    public string Chr_SUBJECT_ORD_FLG { get; set; }
    public string Chr_SUBJECT_REVAL_FLG { get; set; }
    public string Chr_SUBJECT_PRINT_FLG { get; set; }
    public string Var_SUBJECT_CODE_FLG { get; set; }
    public string Var_SUBJECT_TEMP_CODE_FLG { get; set; }
    public string Chr_SUBJECT_INCLUDE_CLASS { get; set; }
    public string Var_SUBJECT_TOTAL_FLG { get; set; }
    public string Chr_NOTINCL_COPART_TOTAL { get; set; }
    public int Num_SUBJECT_SR_NO { get; set; }
    public string Var_SUBJECT_NM_BL { get; set; }
    public string Chr_SUB_VALID_FLG { get; set; }
    public string Chr_SUBJECT_ATKT_FLG { get; set; }
    public string Chr_SUBJECT_STS_FLG { get; set; }
    public int? Num_EQUI_SUB_CD { get; set; }
    public string Var_SUBJ_CANCEL_FROM_MON { get; set; }
    public string Var_SUBJ_CANCEL_FROM_YR { get; set; }
    public string Chr_DELETE_FLG { get; set; }
    public string var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public string Chr_SUBMRK_GRD_CONV_FLG { get; set; }
    public string Var_Subject_Sts { get; set; }
    public string Chr_SCALEDOWN_APL { get; set; }
    public short? NUM_CAT_SCALEDOWN_FROM_T1 { get; set; }
    public short? NUM_CAT_SCALEDOWN_TO_T1 { get; set; }
    public short? NUM_CAT_SCALEDOWN_FROM_T2 { get; set; }
    public short? NUM_CAT_SCALEDOWN_TO_T2 { get; set; }
    public short? NUM_PAP_SCALEDOWN_FROM_T3 { get; set; }
    public short? NUM_PAP_SCALEDOWN_TO_T3 { get; set; }
    public double? Num_SUBJECT_CREDIT { get; set; }
    public double? Num_MIN_GRD_POINT { get; set; }
    public int? Num_Subject_Div { get; set; }
    public int? Num_Practical_Repeat { get; set; }
    public string Var_CommonSubject { get; set; }
    public string Var_SUBJECT_NM_API { get; set; }
}


public partial class Tbl_SUBJECT_MSTR_ViewModel
{
    public short Num_FK_COPRT_NO { get; set; }
    public int Num_PK_SUB_CD { get; set; }
    public string Var_SUBJECT_NM { get; set; }
    public short Num_SUBJECT_MAX_MRK { get; set; }
    public double Num_SUBJECT_PASS_MRK { get; set; }
    public short Num_SUBJECT_EXMP_MRK { get; set; }
}