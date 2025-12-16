using System;

namespace Corno.Data.Core;

public partial class Tbl_STUDENT_CAT_MARKS
{
    public string Var_FK_PRN_NO { get; set; }
    public int Num_FK_SUB_CD { get; set; }
    public short Num_FK_CAT_CD { get; set; }
    public short? Num_ST_PH_MRK { get; set; }
    public string Var_ST_PH_STS { get; set; }
    public short? Num_ST_GRD_NO { get; set; }
    public string Var_ST_PH_RES { get; set; }
    public short Num_FK_INST_NO { get; set; }
    public string Var_ST_PH_USR_NM { get; set; }
    public DateTime? Dtm_ST_PH_DTE_CR { get; set; }
    public DateTime? Dtm_ST_PH_DTE_UP { get; set; }
    public short? Num_ST_PH_ACT_MRK { get; set; }
}

public partial class Tbl_STUDENT_CAT_MARKS_ViewModel
{
    public short Num_FK_CAT_CD { get; set; }
    public short? Num_ST_PH_MRK { get; set; }
    public string Var_ST_PH_RES { get; set; }
}