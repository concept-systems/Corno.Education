using System;

namespace Corno.Data.Core;

public partial class Tbl_BRANCH_MSTRCommon
{
    public short Num_FK_FA_CD { get; set; }
    public short Num_PK_BR_CD { get; set; }
    public string Var_BR_NM { get; set; }
    public string Var_BR_SHRT_NM { get; set; }
    public short Num_BR_SEQ_NO { get; set; }
    public string Chr_DELETE_FLG { get; set; }
    public string Var_BR_NM_BL { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public short? Num_FK_CO_CD { get; set; }
    public string Var_BR_NM_ABC { get; set; }
}
public partial class Tbl_BRANCH_MSTR : Tbl_BRANCH_MSTRCommon
{
    public Tbl_BRANCH_MSTR()
    {

    }
       
}

public partial class Tbl_BRANCH_MSTRViewModel : Tbl_BRANCH_MSTRCommon
{
    public Tbl_BRANCH_MSTRViewModel()
    {

    }
      
}