using System;

namespace Corno.Data.Core;

public partial class TBL_STUDENT_EXAMS
{
    public string Chr_FK_PRN_NO { get; set; }
    public short? Num_FK_BR_CD { get; set; }
    public short Num_FK_COPRT_NO { get; set; }
    public long? Num_ST_SEAT_NO { get; set; }
    public short Num_FK_INST_NO { get; set; }
    public short? Num_FK_CO_CD { get; set; }
    public short? Num_COPRT_SEMI_NO { get; set; }
    public short? Num_COPRT_PART_NO { get; set; }
    public string Chr_ST_COPRT_RES { get; set; }
    public short? Num_FK_CLASS_CD { get; set; }
    public string Chr_PART_TOT_PASSFAIL_FLG { get; set; }
    public short? Num_ST_ORD_MRK { get; set; }
    public short? Num_FK_ORD_NO { get; set; }
    public string Chr_ST_YR_RES { get; set; }
    public string Chr_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public string Chr_ST_REV { get; set; }
    public string Chr_IMPROVEMENT_FLG { get; set; }
    public string Chr_ST_BOP_FLG { get; set; }
}

//public class StudentExamViewModel
//{
//    public string Chr_IMPROVEMENT_FLG { get; set; }
//    public short? Num_FK_CO_CD { get; set; }
//    public short Num_FK_COPRT_NO { get; set; }
//    public short Num_FK_INST_NO { get; set; }
//    public string Var_COPRT_DESC { get; set; }
//}