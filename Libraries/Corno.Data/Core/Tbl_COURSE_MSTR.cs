using System;

namespace Corno.Data.Core;

public partial class Tbl_COURSE_MSTR
{
    public short Num_FK_FA_CD { get; set; }
    public short Num_FK_TYP_CD { get; set; }
    public short Num_PK_CO_CD { get; set; }
    public string Var_CO_NM { get; set; }
    public string Var_CO_SHRT_NM { get; set; }
    public short? Num_CO_SEQ_NO { get; set; }
    public string Chr_CO_IMPRVO_FLG { get; set; }
    public string Chr_CO_MIG_FLG { get; set; }
    public string Chr_CO_AGR_FLG { get; set; }
    public string Chr_CO_FIRST_ATT_FLG { get; set; }
    public string Chr_CO_GRAD_FLG { get; set; }
    public double Num_CO_MAX_DURATION { get; set; }
    public string Var_CO_NM_BL { get; set; }
    public string Chr_SUB_DIST_APL_FLG { get; set; }
    public short? Num_PERCENTAGE { get; set; }
    public string Chr_CO_VALID_FLG { get; set; }
    public string Chr_REGISTRATION_DATA_POSTING { get; set; }
    public string Chr_EXAMINATION_APPLICATION_FORM_POSTING { get; set; }
    public string Chr_MARK_POSTING { get; set; }
    public string Chr_PRN_GENERATION { get; set; }
    public string Chr_EXAMINATION_APPLICATION_FORM_VALIDATION { get; set; }
    public string Chr_REGISTRATION_VALIDATION { get; set; }
    public string Chr_CENTER_ALLOCATION { get; set; }
    public string Chr_SUB_VALIDATION { get; set; }
    public string Chr_SEATNO_GENERATION { get; set; }
    public string Chr_COURSE_VALIDATION { get; set; }
    public string Chr_GROUP_VALIDATION { get; set; }
    public string Chr_GRP_FAIL { get; set; }
    public string Chr_RESULT_PROCESS { get; set; }
    public short Num_ENROLL_NO { get; set; }
    public string Chr_DELETE_FLG { get; set; }
    public string CHR_CO_MIG_DEGR_ALLOWED { get; set; }
    public string CHR_CO_SECOND_DEGR_ALLOWED { get; set; }
    public string CHR_CO_SECOND_FLG { get; set; }
    public string CHR_CO_LOCK_FLG { get; set; }
    public string CHR_MARK_VALIDATION { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public string Chr_GEN_ORD_APL { get; set; }
    public string Chr_Env_Studies { get; set; }
    public short? Num_Env_Studies_Maxmrk { get; set; }
    public short? Num_Env_Studies_Passmrk { get; set; }
    public short? Num_COMMON_CO_CD { get; set; }
    public short? SUB_DIST_MARK_ADD { get; set; }
    public string Var_CO_NM_Certificate { get; set; }
    public string Var_CO_NM_CertificateBold { get; set; }
    public string Chr_Registration_Active { get; set; }

    public short? Num_DeanGracingApplicable { get; set; }
    public string Chr_AdditionalCredits { get; set; }
    public string Chr_TimeTable_Active { get; set; }
    public string Chr_Close_Flg { get; set; }
}