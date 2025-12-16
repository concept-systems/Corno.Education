using System;

namespace Corno.Data.Core;

public partial class Tbl_COURSE_TYPE_MSTR
{
    public short Num_PK_TYP_CD { get; set; }
    public string Var_TYP_NM { get; set; }
    public string Var_TYP_SHRT_NM { get; set; }
    public string Chr_DELETE_FLG { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
}