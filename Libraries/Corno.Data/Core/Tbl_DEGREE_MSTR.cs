using System;

namespace Corno.Data.Core;

public partial class Tbl_DEGREE_MSTR
{
    public short Num_PK_DEGREE_CD { get; set; }
    public string Var_DEGREE_NM { get; set; }
    public string Chr_DELETE_FLG { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
}