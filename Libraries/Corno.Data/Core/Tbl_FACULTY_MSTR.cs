using System;

namespace Corno.Data.Core;

public partial class Tbl_FACULTY_MSTR
{
    public short Num_PK_FA_CD { get; set; }
    public string Var_FA_NM { get; set; }
    public short Num_FA_SEQ_NO { get; set; }
    public string Var_FA_NM_BL { get; set; }
    public string Chr_DELETE_FLG { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
}