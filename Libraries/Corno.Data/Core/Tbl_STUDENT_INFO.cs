using System;

namespace Corno.Data.Core;

public partial class Tbl_STUDENT_INFO
{
    public string Chr_PK_PRN_NO { get; set; }
    public string Var_ST_NM { get; set; }
    public string Var_ST_FATHR_NM { get; set; }
    public string VAR_MOTHR_NM { get; set; }
    public string Chr_ST_SEX_CD { get; set; }
    public string Dtm_ST_DOB_DT { get; set; }
    public string Dtm_ST_DOB_MONTH { get; set; }
    public string Dtm_ST_DOB_YEAR { get; set; }
    public short Num_ST_CAST_CD { get; set; }
    public short? Num_FK_INCOME_CD { get; set; }
    public string Chr_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public short NUM_FK_COPART { get; set; }
    public string Chr_STUDENT_NATIONALITY { get; set; }
    public string Var_OLD_PRN_NO { get; set; }
    public string fffff { get; set; }
    public string AadharNo { get; set; }
    //public string Chr_Abc { get; set; }
}