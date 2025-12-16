using System;

namespace Corno.Data.Core;

public partial class Tbl_SUB_CATPAP_MSTR
{
    public short Num_FK_COPRT_NO { get; set; }
    public int Num_FK_SUB_CD { get; set; }
    public short Num_FK_CAT_CD { get; set; }
    public short Num_PK_PAP_CD { get; set; }
    public string Var_PAP_NM { get; set; }
    public string Var_PAP_SHRT_NM { get; set; }
    public short Num_PAP_MAX_MRK { get; set; }
    public double Num_PAP_PASS_MRK { get; set; }
    public short Num_PAP_EXMP_MRK { get; set; }
    public short Num_PAP_GRD_MIN { get; set; }
    public string Chr_PAP_CODE_FLG { get; set; }
    public string CHR_PAP_SEC_APL { get; set; }
    public string Chr_DELETE_FLG { get; set; }
    public string var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public string CHR_WRT_UNI_APL { get; set; }
    public string Verification_Accept { get; set; }
}