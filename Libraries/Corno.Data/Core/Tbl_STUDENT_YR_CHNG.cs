using System;

namespace Corno.Data.Core;

public partial class Tbl_STUDENT_YR_CHNG
{
    public string Chr_FK_PRN_NO { get; set; }
    public short Num_FK_INST_NO { get; set; }
    public short Num_FK_COPRT_NO { get; set; }
    public short Num_FK_BR_CD { get; set; }
    public long? Num_ST_SEAT_NO { get; set; }
    public short Num_FK_COL_CD { get; set; }
    public string Chr_ST_RESULT { get; set; }
    public short? Num_FK_RESULT_CD { get; set; }
    public short? Num_ST_ORD_MRK { get; set; }
    public short? Num_FK_ORD_NO { get; set; }
    public short Num_FK_CENTER_COL_CD { get; set; }
    public decimal Num_FK_DEBR_CD { get; set; }
    public string Chr_ST_ADMIT_PRN { get; set; }
    public string Chr_ST_ATTEN_PRN { get; set; }
    public string Chr_ST_MARKSHEET_PRN { get; set; }
    public decimal Num_ST_MARKSHEET_NO { get; set; }
    public string Chr_ST_APP_FORM_NO { get; set; }
    public string Chr_ST_RESERV_FLG { get; set; }
    public string Chr_Ord_Appl_Flg { get; set; }
    public byte[] Img_PDF { get; set; }
    public string Chr_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public string Chr_STYC_REVAL_FLG { get; set; }
    public short? Num_EXAM_GROUP { get; set; }
    public string Chr_IMPROVEMENT_FLG { get; set; }
    public short? Num_FK_DistCenter_ID { get; set; }
}