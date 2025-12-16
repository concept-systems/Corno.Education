using System;

namespace Corno.Data.Core;

public partial class TBL_STUDENT_CGPA
{
    public string Chr_FK_PRN_NO { get; set; }
    public short? Num_FK_BR_CD { get; set; }
    public short? Num_FK_COPRT_NO { get; set; }
    public short? Num_FK_INST_NO { get; set; }
    public short? Num_FK_GRADE_NO { get; set; }
    public decimal? Num_GRADE_POINTS { get; set; }
    public double? Num_TOTAL_CREDITS { get; set; }
    public decimal? Num_CGPA { get; set; }
    public short? Num_FK_GRADE_NO_AVG { get; set; }
    public decimal? Num_GRADE_POINTS_AVG { get; set; }
    public short? Num_TOTAL_CREDITS_AVG { get; set; }
    public decimal? Num_CGPA_AVG { get; set; }
    public string Chr_CGPA_AVG_RES { get; set; }
    public string Chr_CGPA_RES { get; set; }
    public string Chr_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public decimal? Num_CGPA_PER { get; set; }
    public decimal? Num_CGPA_GRACE { get; set; }
}