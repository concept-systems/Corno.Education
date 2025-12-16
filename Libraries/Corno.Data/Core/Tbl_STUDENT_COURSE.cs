using System;

namespace Corno.Data.Core;

public partial class Tbl_STUDENT_COURSE
{
    public string Chr_FK_PRN_NO { get; set; }
    public short Num_FK_CO_CD { get; set; }
    public short Num_FK_COPRT_NO { get; set; }
    public short? Num_ST_COLLEGE_CD { get; set; }
    public string Chr_ST_COL_ADM_ROLL { get; set; }
    public string Chr_ST_REG_YEAR { get; set; }
    public short? Num_FK_INST_NO { get; set; }
    public string Chr_ST_REG_CARD_PRINT_FLG { get; set; }
    public string Num_REG_FORM_NO { get; set; }
    public string Num_ST_REG_FEES { get; set; }
    public string Chr_STCO_MIG { get; set; }
    public string CHR_MIGRATED { get; set; }
    public string CHR_CO_RESULT { get; set; }
    public short? NUM_CO_CLASS { get; set; }
    public short? NUM_CO_GRADE { get; set; }
    public string CHR_DEGREE_ISSUED { get; set; }
    public string CHR_PRACTICE_APPLICABLE { get; set; }
    public string CHR_PASS_CERT_ISSUED { get; set; }
    public byte[] Img_Pdf { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public short? Num_FK_BR_CD { get; set; }
    public string Chr_Num_Enclose { get; set; }
    public short? Num_FK_DistCenter_ID { get; set; }
    public string Num_Enroll_No { get; set; }
    public string ABC_Active { get; set; }
    public string Chr_EnviStudies_Exempted { get; set; }
}