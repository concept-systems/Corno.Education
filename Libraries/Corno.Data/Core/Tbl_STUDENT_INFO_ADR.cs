using System;
using System.ComponentModel.DataAnnotations;

namespace Corno.Data.Core;

public partial class Tbl_STUDENT_INFO_ADR
{
    [Key]
    public string Chr_FK_PRN_NO { get; set; }
    public string Var_ST_FATH_NM { get; set; }
    public string Var_ST_MOTH_NM { get; set; }
    public string Chr_ST_ADD1 { get; set; }
    public string Chr_ST_ADD2 { get; set; }
    public string Chr_ST_ADD3 { get; set; }
    public string Chr_ST_CITY { get; set; }
    public string Chr_ST_DISTRICT { get; set; }
    public string Chr_ST_PINCODE { get; set; }
    public string Var_REG_RELIGION { get; set; }
    public string Var_REG_CASTE { get; set; }
    public string Chr_ST_TIFF_NO { get; set; }
    public byte[] Ima_ST_PHOTO { get; set; }
    public byte[] Ima_ST_ADDRESS { get; set; }
    public string Ima_Flg { get; set; }
    public string Chr_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public string Var_FOREIGN_ADD { get; set; }
    public string Var_FOREIGN_CITY { get; set; }
    public string Var_COUNTRY { get; set; }
    public string Var_PASSPORT { get; set; }
    public string Num_PHONE { get; set; }
    public string Num_PHONE1 { get; set; }
    public string Num_MOBILE { get; set; }
    public string Chr_GUARDIAN { get; set; }
    public string Chr_PREV_EXAM { get; set; }
    public string Chr_PREV_EXAM_UNI { get; set; }
    public string Chr_PREV_EXAM_SEATNO { get; set; }
    public string Chr_PREV_EXAM_PERCENT { get; set; }
    public string Chr_PREV_EXAM_MON_YR { get; set; }
    public string Chr_DIST_EDU { get; set; }
    public string Chr_LOCAL_ADD { get; set; }
    public string Chr_PERMANENT_ADD { get; set; }
    public string Chr_GUARDIAN_ADD { get; set; }
    public string Num_PHONE_GRDAN { get; set; }
    public string Chr_LICENSE_NO { get; set; }
    public string Chr_BLOOD_GROUP { get; set; }
    public string Chr_PAN_CARD_NO { get; set; }
    public string Chr_Student_Email { get; set; }
    public string Chr_Abc { get; set; }
    public string AADHAAR_NAME { get; set; }
}