using System;
using System.ComponentModel.DataAnnotations;

namespace Corno.Data.Core;

public class Tbl_CONVO_MSTR
{
    [Key]
    public short Num_PK_CONVO_NO { get; set; } // Primary Key
    public string Chr_CONVO_YEAR { get; set; }
    public DateTime? Dtm_CONVO_DATE { get; set; }
    public string Var_CONVO_GUEST_NM { get; set; }
    public string Var_DEDSEIPTION { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
}