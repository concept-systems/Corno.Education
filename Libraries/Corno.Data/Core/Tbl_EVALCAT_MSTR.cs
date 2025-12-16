using System;
using System.ComponentModel.DataAnnotations;

namespace Corno.Data.Core;

public partial class Tbl_EVALCAT_MSTR
{
    [Key]
    public short Num_PK_CAT_CD { get; set; }
    public string CHR_WRT_UNI_APL { get; set; }
    public string Var_CAT_NM { get; set; }
    public string Var_CAT_SHRT_NM { get; set; }
    public string Chr_DELETE_FLG { get; set; }
    public string var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
}