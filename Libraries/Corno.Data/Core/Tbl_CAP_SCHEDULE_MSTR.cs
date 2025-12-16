using System;

namespace Corno.Data.Core;

public partial class Tbl_CAP_SCHEDULE_MSTR
{
    public short Num_FK_COPRT_NO { get; set; }
    public short Num_FK_BR_NO { get; set; }
    public DateTime? Dtm_Commencement_DateOfExam { get; set; }
    public DateTime? Dtm_ConclExam { get; set; }
    public DateTime? Dtm_ComplCAP { get; set; }
    public DateTime? Dtm_MarkFromCAP { get; set; }
    public DateTime? Dtm_Result_Date { get; set; }
    public short? Num_FK_INST_NO { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
}