using System;
using System.ComponentModel.DataAnnotations;

namespace Corno.Data.Core;

public partial class Tbl_ADDITIONAL_CREDITS
{
    [Key]
    public int Num_PK_ENTRY_ID { get; set; }
    public string Chr_ADD_PRN_NO { get; set; }
    public short Num_FK_INST_NO { get; set; }
    public short Num_FK_COLLEGE_CD { get; set; }
    public short Num_FK_COURCE_CD { get; set; }
    //public short Num_FK_COPRT_NO { get; set; }
    public short? Num_FK_BR_CD { get; set; }

    public short Num_FK_SUB_CD { get; set; }
    public short Num_MAX_CREDITS { get; set; }
    public string Chr_IS_COMPLETED { get; set; }
    public DateTime? Dtm_COMPLETED { get; set; }

    public string DELETE_FLG { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
}