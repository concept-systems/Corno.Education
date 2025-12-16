using System;

namespace Corno.Data.Core;

public partial class Tbl_SYS_INST
{
    public Tbl_SYS_INST()
    {
        //this.Tbl_CENTER_COL_TRX = new List<Tbl_CENTER_COL_TRX>();
        //this.Tbl_CENTER_TRX = new List<Tbl_CENTER_TRX>();
        //this.Tbl_CODE_DECODE = new List<Tbl_CODE_DECODE>();
        //this.Tbl_COURSE_PART_INST_TRX = new List<Tbl_COURSE_PART_INST_TRX>();
        //this.Tbl_STUDENT_DEBARRED = new List<Tbl_STUDENT_DEBARRED>();
        //this.Tbl_STUDENT_PAP_MARKS = new List<Tbl_STUDENT_PAP_MARKS>();
        //this.Tbl_STUDENT_PRV_STATUS = new List<Tbl_STUDENT_PRV_STATUS>();
        //this.Tbl_STUDENT_SEC_MARKS = new List<Tbl_STUDENT_SEC_MARKS>();
        //this.Tbl_STUDENT_YR_CHNG = new List<Tbl_STUDENT_YR_CHNG>();
    }

    public short Num_PK_INST_SRNO { get; set; }
    public short Num_INST_YEAR { get; set; }
    public short Num_INST_MONTH { get; set; }
    public string Chr_INST_LOCK { get; set; }
    public string Chr_INST_STATUS { get; set; }
    public string Chr_INST_DEFA_FLG { get; set; }
    public string Var_INST_REM { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public short? Num_SEQ_NO { get; set; }
    public short? Num_FK_PANEL_NO { get; set; }
    public short? Num_ENV_MONTH { get; set; }
    public short? Num_CONVO_NO { get; set; }
    public string Var_INST_NM_API { get; set; }
    //public virtual ICollection<Tbl_CENTER_COL_TRX> Tbl_CENTER_COL_TRX { get; set; }
    //public virtual ICollection<Tbl_CENTER_TRX> Tbl_CENTER_TRX { get; set; }
    //public virtual ICollection<Tbl_CODE_DECODE> Tbl_CODE_DECODE { get; set; }
    //public virtual ICollection<Tbl_COURSE_PART_INST_TRX> Tbl_COURSE_PART_INST_TRX { get; set; }
    //public virtual ICollection<Tbl_STUDENT_DEBARRED> Tbl_STUDENT_DEBARRED { get; set; }
    //public virtual ICollection<Tbl_STUDENT_PAP_MARKS> Tbl_STUDENT_PAP_MARKS { get; set; }
    //public virtual ICollection<Tbl_STUDENT_PRV_STATUS> Tbl_STUDENT_PRV_STATUS { get; set; }
    //public virtual ICollection<Tbl_STUDENT_SEC_MARKS> Tbl_STUDENT_SEC_MARKS { get; set; }
    //public virtual ICollection<Tbl_STUDENT_YR_CHNG> Tbl_STUDENT_YR_CHNG { get; set; }
}