using System;

namespace Corno.Data.Core;

public partial class Tbl_FEE_DTL
{
    public short? Num_FK_COL_CD { get; set; }
    public short? Num_FK_CO_CD { get; set; }
    public short Num_FK_COPRT_NO { get; set; }
    public short? Num_FK_BR_CD { get; set; }
    public short? Num_FK_INST_NO { get; set; }
    public short? Num_FK_FEE_CD { get; set; }
    public decimal? FEE_AMOUNT { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
}