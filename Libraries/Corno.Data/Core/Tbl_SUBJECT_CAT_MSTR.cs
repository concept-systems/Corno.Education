using System;

namespace Corno.Data.Core;

public partial class Tbl_SUBJECT_CAT_MSTR
{
    public short Num_FK_COPRT_NO { get; set; }
    public int Num_FK_SUB_CD { get; set; }
    public short Num_FK_CAT_CD { get; set; }
    public short Num_CAT_MAX_MRK { get; set; }
    public double Num_CAT_PASS_MRK { get; set; }
    public short Num_CAT_EXMP_MRK { get; set; }
    public short Num_CAT_GRD_MIN { get; set; }
    public short? Num_CAT_PRINT_SEQ { get; set; }
    public string Chr_CAT_CODE_FLG { get; set; }
    public string chr_CAT_SUBCAT_APL { get; set; }
    public string Chr_DELETE_FLG { get; set; }
    public string var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public string CHR_WRT_UNI_APL { get; set; }
    public double? WEIGHTAGE { get; set; }
    public string Verification_Accept { get; set; }
}