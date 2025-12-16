using System;
using System.ComponentModel.DataAnnotations;

namespace Corno.Data.Core;

public partial class Tbl_REG_TEMP
{
    [Key]
    public int Num_PK_RECORD_NO { get; set; }
    public string Var_BUNDLE_NO { get; set; }
    public string Chr_PK_FORM_NO { get; set; }
    public string Chr_REG_TIFF_NO { get; set; }
    public string Chr_REG_VALID_FLG { get; set; }
    public string Chr_REG_PRN_NO { get; set; }
    public string Chr_REG_COPRT_NO { get; set; }
    public string Chr_REG_EXM_PAT_CD { get; set; }
    public string Chr_REG_BR_CD { get; set; }
    public short? Num_FK_INST_NO { get; set; }
    public string Chr_REG_YEAR { get; set; }
    public string Chr_REG_MONTH { get; set; }
    public string Chr_REG_ST_NM { get; set; }
    public string Chr_REG_FATH_NM { get; set; }
    public string Chr_REG_MOTH_NM { get; set; }
    public string Chr_REG_CAST_CD { get; set; }
    public string Var_REG_RELIGION { get; set; }
    public string Var_REG_CASTE { get; set; }
    public string Chr_REG_SEX_CD { get; set; }
    public string Chr_REG_DOB_DT { get; set; }
    public string Chr_REG_DOB_MONTH { get; set; }
    public string Chr_REG_DOB_YEAR { get; set; }
    public string Chr_REG_ADD1 { get; set; }
    public string Chr_REG_ADD2 { get; set; }
    public string Chr_REG_ADD3 { get; set; }
    public string Chr_REG_CITY { get; set; }
    public string Chr_REG_DISTRICT { get; set; }
    public string Chr_REG_PIN { get; set; }
    public string Chr_REG_ENTRY_FLG { get; set; }
    public string Chr_REG_COLLEGE_CD { get; set; }
    public string Chr_REG_ADM_ROLL { get; set; }
    public string Chr_REG_FEES { get; set; }
    public byte[] Ima_REG_PHOTO { get; set; }
    public byte[] Ima_REG_ADDRESS { get; set; }
    public string Chr_REG_INCM_CD { get; set; }
    public string Var_CHKLIST_FLG { get; set; }
    public string Chr_REPEATER_FLG { get; set; }
    public string Chr_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public string Chr_STUDENT_NATIONALITY { get; set; }
    public string Var_OLD_PRN_NO { get; set; }
    public string Var_FOREIGN_ADD { get; set; }
    public string Var_FOREIGN_CITY { get; set; }
    public string Var_COUNTRY { get; set; }
    public string Var_PASSPORT { get; set; }
    public string Chr_FATHER_NAME { get; set; }
    public string Num_PHONE { get; set; }
    public string Num_PHONE1 { get; set; }
    public string Num_MOBILE { get; set; }
    public string Chr_Student_Email { get; set; }
    public string Chr_GUARDIAN { get; set; }
    public string Chr_PREV_EXAM { get; set; }
    public string Chr_PREV_EXAM_UNI { get; set; }
    public string Chr_PREV_EXAM_SEATNO { get; set; }
    public string Chr_PREV_EXAM_PERCENT { get; set; }
    public string Chr_PREV_EXAM_MON_YR { get; set; }
    public string Chr_DIST_EDU { get; set; }
    public string Chr_MIGRATION_FLG { get; set; }
    public string Chr_LOCAL_ADD { get; set; }
    public string Chr_PERMANENT_ADD { get; set; }
    public string Chr_GUARDIAN_ADD { get; set; }
    public string Chr_IMPROVEMENT_FLG { get; set; }
    public int Num_INCREMENT_PART_INST { get; set; }
    public string Num_PHONE_GRDAN { get; set; }
    public string Chr_LICENSE_NO { get; set; }
    public string Chr_BLOOD_GROUP { get; set; }
    public string Chr_PAN_CARD_NO { get; set; }
    public string Chr_Num_Enclose { get; set; }
    public short? Num_FK_DistCenter_ID { get; set; }
    public string REGISTER_NO { get; set; }
    public string ENROLLMENT_NO { get; set; }
    public string PAN { get; set; }
    public string AadharNo { get; set; }
    public  DateTime? ADMISSION_DATE { get; set; }
    public DateTime? LEAVING_DATE { get; set; }
    public string RENEWED_PASSPORT_NO { get; set; }
    public DateTime? PASSPORT_ISSUED_ON { get; set; }
    public DateTime? RENEWED_PASSPORT_ISSUED_ON { get; set; }
    public string VISA_NO { get; set; }
    public DateTime? VISA_ISSUED_ON { get; set; }
    public  string IS_ADMISSION_CANCELLED { get; set; }
    public DateTime? ADMISSION_CANCELLED_DATE { get; set; }
    public string BIRTH_COUNTRY { get; set; }
    public string QUALIFYING_EXAMINATIONS { get; set; }
    public string QUALIFYING_SUBJECTS { get; set; }
    public string SSC_TOTAL_MARKS { get; set; }
    public string HSC_TOTAL_MARKS { get; set; }
    public  string UG_TOTAL_MARKS { get; set; }
    public string SSC_MARKS_OUT_OF { get; set; }
    public string HSC_MARKS_OUT_OF { get; set; }
    public string UG_MARKS_OUT_OF { get; set; }

    public string Chr_Erp_No { get; set; }
    public string Chr_Abc_Id { get; set; }

}