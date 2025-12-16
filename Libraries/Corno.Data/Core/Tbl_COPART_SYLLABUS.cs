using System;

namespace Corno.Data.Core;

public partial class Tbl_COPART_SYLLABUS
{
    public short Num_PK_SYL_NO { get; set; }
    public short Num_FK_COPRT_NO { get; set; }
    public short Num_FK_PAT_CD { get; set; }
    public short Num_FK_BR_CD { get; set; }
    public short Num_FK_GRP_CD { get; set; }
    public short Num_FK_SUB_GRP_CD { get; set; }
    public short NUM_FK_SUB_SUB_GRP_CD { get; set; }
    public short Num_GRP_MAX_MRK { get; set; }
    public short Num_GRP_PASS_MRK { get; set; }
    public short Num_GRP_EXMP_MRK { get; set; }
    public short Num_GRP_GRD_MIN { get; set; }
    public short Num_COMPL_SUB { get; set; }
    public short Num_COMPL_OPT_SUB { get; set; }
    public short Num_OPT_SUB { get; set; }
    public string Chr_MUST_PASS_FLG { get; set; }
    public string Chr_INCL_FLG { get; set; }
    public string Chr_PASS_FLG { get; set; }
    public short? Num_CO_SYB_PRINT_SEQ { get; set; }
    public string Chr_DELETE_FLG { get; set; }
    public string var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public short? Num_Syl_Seq { get; set; }
}