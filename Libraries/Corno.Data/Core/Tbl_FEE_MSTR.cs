using System;

namespace Corno.Data.Core;

public partial class Tbl_FEE_MSTR
{
    public short Num_PK_FEE_CD { get; set; }
    public string Var_FEE_DESC { get; set; }
    public string Num_FEE_SEQ_NO { get; set; }
    public string Chr_DELETE_FLG { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
}