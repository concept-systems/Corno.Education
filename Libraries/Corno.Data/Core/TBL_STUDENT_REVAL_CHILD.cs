using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corno.Data.Core;

public class TBL_STUDENT_REVAL_CHILD
{
    [Key, Column(Order = 0)]
    public short? Num_FK_INST_NO { get; set; }
    [Key, Column(Order = 1)]
    public short? Num_FK_COPRT_NO { get; set; }
    [Key, Column(Order = 2)]
    public int? NUM_FK_SUB_CD { get; set; }
    [Key, Column(Order = 3)]
    public string NUM_FK_PRN_NO { get; set; }
    public int? Num_FK_CAT_CD { get; set; }
    public int? Num_FK_PAP_CD { get; set; }
    public int? Num_FK_SEC_CD { get; set; }
    public int? OLD_MARK { get; set; }
    public int? NEW_MARK { get; set; }
    public double? NUM_REVALUATION_FEE { get; set; }
    public double? NUM_VERIFICATION_FEE { get; set; }
    public double? NUM_TOTAL_FEE { get; set; }
}