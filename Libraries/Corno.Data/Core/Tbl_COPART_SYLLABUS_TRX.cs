using System;

namespace Corno.Data.Core;

public partial class Tbl_COPART_SYLLABUS_TRX
{
    public short Num_FK_SYL_NO { get; set; }
    public int Num_FK_SUB_CD { get; set; }
    public short? Num_FK_CAT_CD { get; set; }
    public string Chr_SUB_CMP_OPT_FLG { get; set; }
    public string Chr_DELETE_FLG { get; set; }
    public string var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
}