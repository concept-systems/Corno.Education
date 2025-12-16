namespace Corno.Data.Core;

public class Tbl_TimeTableINST
{
    public short? Num_FK_INST_NO { get; set; }
    public short? Num_PK_CO_CD { get; set; }
    public short? Num_FK_FA_CD { get; set; }
    public short? Num_FK_TYP_CD { get; set; }
    public string Var_CO_NM { get; set; }
    public string Chr_FreezeTimeTable { get; set; }
    public string Chr_FreezeConvocation { get; set; }
}