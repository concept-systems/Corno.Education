using System;

namespace Corno.Data.Core;

public partial class Tbl_GRADE_MSTR
{
    public short Num_PK_GRD_CD { get; set; }
    public short Num_FK_CO_CD { get; set; }
    public string Var_GRD_NM { get; set; }
    public string Var_GRD_SHRT_NM { get; set; }
    public short Num_GRD_SEQ_NO { get; set; }
    public double? Num_GRD_POINTS { get; set; }
    public double? Flt_MAX_MRK { get; set; }
    public double? Flt_MIN_MRK { get; set; }
    public string Chr_DELETE_FLG { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public double? Flt_MAX_GPA { get; set; }
    public double? Flt_MIN_GPA { get; set; }
}