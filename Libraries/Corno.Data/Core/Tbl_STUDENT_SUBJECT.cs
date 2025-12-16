using System;

namespace Corno.Data.Core;

public partial class Tbl_STUDENT_SUBJECT
{
    public short Num_FK_COPRT_NO { get; set; }
    public string Chr_FK_PRN_NO { get; set; }
    public int Num_FK_SUB_CD { get; set; }
    public short? Num_ST_SUB_MRK { get; set; }
    public string Chr_ST_SUB_STS { get; set; }
    public short Num_ST_GRD_NO { get; set; }
    public string Chr_ST_SUB_RES { get; set; }
    public short Num_FK_INST_NO { get; set; }
    public string Chr_ST_SUB_CAN { get; set; }
    public short Num_ST_SUB_ORG_MRK { get; set; }
    public string Chr_ST_SUB_ORDINANCE { get; set; }
    public string CHR_ST_ORD_FLG { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public string Chr_IMPROVEMENT_FLG { get; set; }
    public double? Chr_ST_SUB_GPA { get; set; }
    public short? Num_FK_COPRT_NO_AddCr { get; set; }
}