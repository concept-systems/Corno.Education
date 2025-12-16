using System;

namespace Corno.Data.Core;

public partial class Tbl_COURSE_PART_MSTR
{
    public short Num_FK_CO_CD { get; set; }
    public short Num_PK_COPRT_NO { get; set; }
    public short Num_COPRT_PART_NO { get; set; }
    public short Num_COPRT_SEMI_NO { get; set; }
    public string Var_COPRT_DESC { get; set; }
    public string Var_COPRT_SHRT_NM { get; set; }
    public string Chr_COPRT_RESULT_FLG { get; set; }
    public string Chr_COPRT_ORD_FLG { get; set; }
    public string Chr_COPRT_CLASS_FLG { get; set; }
    public string Chr_COPRT_SCALE_FLG { get; set; }
    public double? CHR_SCALINGDWN_PERCENTAGE { get; set; }
    public short? Num_COPRT_MAX_YRS_TO_CMPLT { get; set; }
    public short? Num_COPRT_MAX_ATMPTS_TO_CMPLT { get; set; }
    public string Chr_PHY_EDU_FLG { get; set; }
    public short? Chr_PHY_EDU_MRK { get; set; }
    public string Chr_COPRT_BRANCH_APP_FLG { get; set; }
    public string Chr_COPRT_GRP_APP_FLG { get; set; }
    public string Chr_COPRT_GRP_PRINT_APP_FLG { get; set; }
    public string Chr_COPRT_GRP_PREEXAM_APL_FLG { get; set; }
    public string Chr_COPRT_GRP_RES_APP_FLG { get; set; }
    public string Chr_COPRT_CODING_FLG { get; set; }
    public string Chr_COPRT_QPAPER_CODING_FLG { get; set; }
    public string Chr_COPRT_EXMP_FLG { get; set; }
    public string Chr_SUB_CODE_DECODE_APL_FLG { get; set; }
    public short? Num_COPRT_TOT_SUBJECT { get; set; }
    public short? Num_COPRT_EVAL_NO { get; set; }
    public string Chr_COPRT_EVAL_TYPE { get; set; }
    public string Chr_COPRT_REJECT_FLG { get; set; }
    public string Var_COPRT_DESC_BL { get; set; }
    public string Chr_CGPA_APL_FLG { get; set; }
    public string Chr_MRK_GRD_CONV_FLG { get; set; }
    public double? Flt_DIFF_EVAL_PER { get; set; }
    public string Chr_COTOT_PH_FLG { get; set; }
    public string Chr_COTOT_ATKT_FLG { get; set; }
    public string Chr_ATKT_APL_FLG { get; set; }
    public string Chr_ATKT_APL_TYPE { get; set; }
    public short? Chr_ATKT_SUBNO { get; set; }
    public string Chr_FAILHD_TYPE_FLG { get; set; }
    public string Chr_ELIGIBLITY_APL_FLG { get; set; }
    public string Chr_COPRT_VALID_FLG { get; set; }
    public short? NUM_ADD_SUB { get; set; }
    public string CHR_SUB_MRK_PRINT { get; set; }
    public string CHR_CAT_MRK_PRINT { get; set; }
    public string CHR_COPRT_CLASS_YEARLY { get; set; }
    public string CHR_OTH_ACTIVITY_FLG { get; set; }
    public short? CHR_OTH_ACTIVITY_MRK { get; set; }
    public string CHR_COPRT_YRLY_FLG { get; set; }
    public double? CHR_COTOT_PASS_PERCENTAGE { get; set; }
    public short? CHR_COTOT_PH_FAIL_NO_OF_HEADS_APPEAR { get; set; }
    public string CHR_PAP_MRK_PRINT { get; set; }
    public string CHR_PRACTICE_FLG { get; set; }
    public string CHR_PRACTICE_NAME { get; set; }
    public string Chr_DELETE_FLG { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public string Chr_DEG_APL_FLG { get; set; }
    public short? NUM_CAT_HD_PASS { get; set; }
    public short? Num_COPRT_SEQ_NO { get; set; }
    public string Chr_NotMARK_GRADE_FLG { get; set; }
    public string Chr_YEARLY_RES_FLG { get; set; }
    public string Chr_YEARLY_AGGR_RES_FLG { get; set; }
    public string Chr_FAIL_ALL_CATEGORIES { get; set; }
    public string Chr_AdditionalCredits { get; set; }
    public double? Num_ClassMinPercentage { get; set; }
    public short? Num_FK_Degree_CD { get; set; }
    public string Chr_BRANCH_TIMETABLE_APP_FLG { get; set; }
    public string Chr_NCRFLevel { get; set; }
}