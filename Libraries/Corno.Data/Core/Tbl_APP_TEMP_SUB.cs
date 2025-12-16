using System;

namespace Corno.Data.Core;

public partial class Tbl_APP_TEMP_SUB
{
    public int Num_FK_ENTRY_ID { get; set; }
    public short Num_FK_SUB_CD { get; set; }
    public int? Num_FK_INST_NO { get; set; }
    public short? Num_FK_COPRT_NO { get; set; }
    public string Chr_REPH_FLG { get; set; }
    public string Chr_SUB_FLG { get; set; }
    public string Chr_DELETE_FLG { get; set; }
    public string Chr_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public string Var_Appear_CATEGORY { get; set; }
}