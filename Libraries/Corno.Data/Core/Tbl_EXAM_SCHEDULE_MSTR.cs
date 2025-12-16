using System;
using System.ComponentModel.DataAnnotations;

namespace Corno.Data.Core;

public partial class Tbl_EXAM_SCHEDULE_MSTR
{
    [Key]
    public short Num_PK_Sr_NO { get; set; }
    public short Num_FK_COPRT_NO { get; set; }
    public short Num_FK_COURSE_NO { get; set; }
    public short Num_FK_BR_NO { get; set; }
    public DateTime? Dtm_FormFilll_Date { get; set; }
    public DateTime? Dtm_Commencement_DateOfExam { get; set; }
    public DateTime? Dtm_LstDateOfForm_WithoutLateFee { get; set; }
    public DateTime? Dtm_LstDateOfForm_WithLateFee { get; set; }
    public DateTime? Dtm_LstDateOfForm_WithSuperLateFee { get; set; }
    public short Num_Exam_Fees { get; set; }
    public string Var_Exam_Fee_Narration { get; set; }
    public DateTime? Dtm_Result_Date { get; set; }
    public short? Num_FK_INST_NO { get; set; }
    public string Var_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
    public DateTime? Dtm_Reval_Date { get; set; }
    public DateTime? Dtm_EnviForm_WithoutLateFee { get; set; }
    public DateTime? Dtm_EnviForm_WithLateFee { get; set; }
    public DateTime? Dtm_EnviForm_WithSuperLateFee { get; set; }
    public DateTime? Dtm_Convo_Last_Date { get; set; }

    public DateTime? Dtm_Reval_Res_Date { get; set; }
    public DateTime? Dtm_ConclusionofExam_Date { get; set; }
    public DateTime? Dtm_StartofCAP_Date { get; set; }
    public DateTime? Dtm_CompletionofCAP_Date { get; set; }
    public DateTime? Dtm_MarksReceive_Date { get; set; }
    public DateTime? Dtm_Envi_Examdt { get; set; }
}