using System;

namespace Corno.Data.Core;

public partial class Tbl_CLASS_MSTR
{
    public short Num_PK_CLS_CD { get; set; }
    public string Var_CLS_NM { get; set; }
    public string Var_CLS_SHRT_NM { get; set; }
    public short Num_CLS_SEQ_NO { get; set; }
    public string Var_CLASS_NM_BL { get; set; }
    public string Chr_DELETE_FLG { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
}