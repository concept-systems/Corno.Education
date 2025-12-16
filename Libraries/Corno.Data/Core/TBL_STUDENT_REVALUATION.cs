using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corno.Data.Core;

public partial class TBL_STUDENT_REVALUATION
{
    [Key, Column(Order = 0)]
    public short? Num_FK_INST_NO { get; set; }
    [Key, Column(Order = 1)]
    public short? Num_FK_COPRT_NO { get; set; }
    [Key, Column(Order = 2)]
    public int? NUM_FK_SUB_CD { get; set; }
    [Key, Column(Order = 3)]
    public string NUM_FK_PRN_NO { get; set; }
    public string Var_ST_SUB_NOCHNG { get; set; }
    public string Chr_REVAL_VERI_FLG { get; set; }
    public int? OLD_MARK { get; set; }
    public int? NEW_MARK { get; set; }
    public string Chr_Reval_UniExmHd { get; set; }
    public double? NUM_REVALUATION_FEE { get; set; }
    public double? NUM_VERIFICATION_FEE { get; set; }
    public double? NUM_TOTAL_FEE { get; set; }
}