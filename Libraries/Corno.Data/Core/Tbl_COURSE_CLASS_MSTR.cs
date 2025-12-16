using System;

namespace Corno.Data.Core;

public partial class Tbl_COURSE_CLASS_MSTR
{
    public short Num_FK_CO_CD { get; set; }
    public short Num_FK_COPRT_NO { get; set; }
    public short? Num_FK_CLASS_CD { get; set; }
    public double? Num_CLASS_MAX_MRK { get; set; }
    public double? Num_CLASS_MIN_MRK { get; set; }
    public double? Num_GRD_POINTS { get; set; }
    public short? Num_FK_GRD_NO { get; set; }
    public short? Num_FK_GRP_CD { get; set; }
    public short Num_FK_COPRT_NO_FROM { get; set; }
    public string Chr_DELETE_FLG { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
}