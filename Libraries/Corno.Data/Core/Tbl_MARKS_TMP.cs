using System;

namespace Corno.Data.Core;

public partial class Tbl_MARKS_TMP
{
    public int Num_PK_RECORDID { get; set; }
    public string Chr_FK_COPRT_NO { get; set; }
    public int Num_FORM_NO { get; set; }
    public int? Num_FORM_SRNO { get; set; }
    public int? Chr_FK_BRANCH_CD { get; set; }
    public string Chr_FK_SUB_CD { get; set; }
    public string Chr_FK_CAT_CD { get; set; }
    public string Chr_FK_PAP_CD { get; set; }
    public string Chr_FK_SEC_CD { get; set; }

    public string Chr_EXAMINER_CD { get; set; }
    public string Chr_SCRUTINISER_CD { get; set; }
    public string Chr_HD_EXAMINER_CD { get; set; }

    public long Chr_CODE_SEAT_NO { get; set; }
    public string Chr_MARKS { get; set; }
    public string Chr_STATUS_FLG { get; set; }

    public short? Num_ENTRY_NO { get; set; }
    public short? Num_EVAL_NO { get; set; }
    public string Var_BUNDLE_NO { get; set; }
    public string Var_CODE_FLG { get; set; }
    public short? Num_FK_INST_NO { get; set; }
    public string Var_CHKLIST_FLG { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }

    public string CHR_EXAMINER_NM { get; set; }
    public short? Num_FK_COL_CD { get; set; }
    public string Chr_MARKS2 { get; set; }
    public string Chr_STATUS_FLG2 { get; set; }
    public string Var_USR_NM2 { get; set; }

    public DateTime? Dtm_DTE_CR2 { get; set; }
    public DateTime? Dtm_DTE_UP2 { get; set; }

    public short? Num_FK_DISTCOL_CD { get; set; }
    public short? Num_Application_No { get; set; }
}