using System;
using Corno.Data.Core;
using Corno.Data.Corno;
using Corno.Globals.Enums;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Corno;

public class ExamService :BaseService, IExamService
{
    #region -- Constructors --
    public ExamService(ICoreService coreService)
    {
        _coreService = coreService;
    }
    #endregion

    #region -- Data Members --

    private readonly ICoreService _coreService;
    #endregion

    #region -- Public Methods --
    public void AddExamInExamServer(Exam model, string userName)
    {
        // Get from TBL_STUDENT_YR_CHNG
        if (model.CoursePartId == null) return;

        var studentExam = new Tbl_APP_TEMP
        {
            Num_FORM_ID = Convert.ToInt32(model.FormNo),
            Chr_APP_VALID_FLG = "A",
            DELETE_FLG = "N",
            Chr_APP_PRN_NO = model.PrnNo,
            Num_FK_COPRT_NO = (short)model.CoursePartId,
            Num_FK_INST_NO = (short)model.InstanceId
        };

        if (model.BranchId == null)
            studentExam.Num_FK_BR_CD = 0;
        else
            studentExam.Num_FK_BR_CD = (short)model.BranchId;

        if (model.CollegeId != null) studentExam.Num_FK_COLLEGE_CD = (short)model.CollegeId;

        if (null == model.CentreId)
            studentExam.Num_FK_DistCenter_ID = 0;
        else
            studentExam.Num_FK_DistCenter_ID = (short)model.CentreId;

        studentExam.Chr_BUNDAL_NO = model.Bundle;
        studentExam.AadharNo = model.AadharNo;
        //addition in AppTemp
        studentExam.Num_FK_STACTV_CD = 0;
        studentExam.Num_FK_STUDCAT_CD = 0;
        studentExam.Var_USR_NM = userName;
        studentExam.Dtm_DTE_CR = model.CreatedDate;
        studentExam.Dtm_DTE_UP = model.ModifiedDate;
        studentExam.Chr_REPEATER_FLG = "N";
        studentExam.Chr_IMPROVEMENT_FLG = "N";

        //fee structure
        studentExam.Num_ExamFee = model.ExamFee ?? 0;
        studentExam.Num_CAPFee = model.CapFee ?? 0;
        studentExam.Num_StatementFee = model.StatementOfMarksFee ?? 0;
        studentExam.Num_LateFee = model.LateFee ?? 0;
        studentExam.Num_SuperLateFee = model.SuperLateFee ?? 0;
        studentExam.Num_Fine = model.OthersFee ?? 0;
        studentExam.Num_PassingCertificateFee = model.CertificateOfPassingFee ?? 0;
        studentExam.Num_DissertationFee = model.DissertationFee ?? 0;
        studentExam.Num_BacklogFee = model.BacklogFee ?? 0;
        studentExam.Num_TotalFee = model.Total ?? 0;

        _coreService.Tbl_APP_TEMP_Repository.Add(studentExam);
        _coreService.Save();

        // Save App_Temp Subjects
        foreach (var subject in model.ExamSubjects)
        {
            if (subject.CoursePartId != null)
            {
                if (subject.SubjectCode != null)
                {
                    var examSubject = new Tbl_APP_TEMP_SUB
                    {
                        Num_FK_INST_NO = studentExam.Num_FK_INST_NO,
                        Num_FK_ENTRY_ID = studentExam.Num_PK_ENTRY_ID,
                        Num_FK_COPRT_NO = (short)subject.CoursePartId,
                        Num_FK_SUB_CD = (short)subject.SubjectCode,
                        Chr_DELETE_FLG = "N"
                    };

                    if (subject.SubjectType == SubjectType.BackLog.ToString())
                        examSubject.Chr_REPH_FLG = "R";

                    _coreService.Tbl_APP_TEMP_SUB_Repository.Add(examSubject);
                }
            }
            _coreService.Save();
        }
    }
    #endregion
}