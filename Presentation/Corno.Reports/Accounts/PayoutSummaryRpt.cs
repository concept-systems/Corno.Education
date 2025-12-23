using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Web;
using Corno.Data.Core;
using Corno.Data.Helpers;
using Corno.Data.Payment;
using Corno.Globals.Constants;
using Corno.Globals.Enums;
using Corno.Logger;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Corno.Online_Education.Interfaces;
using Corno.Services.Helper;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Report = Telerik.Reporting.Report;
// Needed for EnvironmentStudy model
using Corno.Data.Corno;

namespace Corno.Reports.Accounts;

public partial class PayoutSummaryRpt : Report
{
    #region -- Constructors --
    public PayoutSummaryRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        //ReportParameters[ModelConstants.Form].AvailableValues.DataSource = new List<MasterViewModel>
        //{
        //    new MasterViewModel{ Id = (int)FormType.All, Name = FormType.All.ToString()},
        //    new MasterViewModel{ Id = (int)FormType.Exam, Name = FormType.Exam.ToString() },
        //    new MasterViewModel{ Id = (int)FormType.Environment, Name = FormType.Environment.ToString() },
        //    new MasterViewModel{ Id = (int)FormType.Convocation, Name = FormType.Convocation.ToString() }
        //};
    }
    #endregion

    #region -- Private Methods --
    // This function is duplicate of AddExamInExamServer in ExamController.
    // Make it common in future.
    private void AddInAppTemp(Data.Corno.Exam model)
    {
        // Get from TBL_STUDENT_YR_CHNG
        if (model.CoursePartId == null)
            return;

        //var ipAddress = GetClientIpAddress();
        var coreService = Bootstrapper.Get<ICoreService>();
        var existing = coreService.Tbl_APP_TEMP_Repository.Get(e => e.Num_FK_INST_NO == model.InstanceId &&
                                                            e.Chr_APP_PRN_NO == model.PrnNo &&
                                                            e.Num_Transaction_Id == model.TransactionId)
            .FirstOrDefault();
        if (null != existing)
            return;

        var onlineEducationStudentService = Bootstrapper.Get<IOnlineEducationStudentService>();
        var appTemp = new Tbl_APP_TEMP
        {
            Num_FORM_ID = model.FormNo.ToInt(),
            Chr_APP_VALID_FLG = "A",
            DELETE_FLG = "N",
            Chr_APP_PRN_NO = model.PrnNo,
            Num_FK_COPRT_NO = (short)model.CoursePartId,
            Num_FK_INST_NO = (short)model.InstanceId
        };

        if (model.BranchId == null)
            appTemp.Num_FK_BR_CD = 0;
        else
            appTemp.Num_FK_BR_CD = (short)model.BranchId;

        if (model.CollegeId != null) appTemp.Num_FK_COLLEGE_CD = (short)model.CollegeId;

        if (null == model.CentreId)
            appTemp.Num_FK_DistCenter_ID = 0;
        else
            appTemp.Num_FK_DistCenter_ID = (short)model.CentreId;

        appTemp.Chr_BUNDAL_NO = model.Bundle;
        appTemp.AadharNo = model.AadharNo;
        //addition in AppTemp
        appTemp.Num_FK_STACTV_CD = 0;
        appTemp.Num_FK_STUDCAT_CD = 0;
        appTemp.Var_USR_NM = HttpContext.Current.User.Identity.Name;
        appTemp.Dtm_DTE_CR = model.CreatedDate;
        appTemp.Dtm_DTE_UP = model.ModifiedDate;
        appTemp.Chr_REPEATER_FLG = "N";
        appTemp.Chr_IMPROVEMENT_FLG = "N";

        //fee structure
        appTemp.Num_ExamFee = model.ExamFee ?? 0;
        appTemp.Num_CAPFee = model.CapFee ?? 0;
        appTemp.Num_StatementFee = model.StatementOfMarksFee ?? 0;
        appTemp.Num_LateFee = model.LateFee ?? 0;
        appTemp.Num_SuperLateFee = model.SuperLateFee ?? 0;
        appTemp.Num_Fine = model.OthersFee ?? 0;
        appTemp.Num_PassingCertificateFee = model.CertificateOfPassingFee ?? 0;
        appTemp.Num_DissertationFee = model.DissertationFee ?? 0;
        appTemp.Num_BacklogFee = model.BacklogFee ?? 0;
        appTemp.Num_TotalFee = model.Total ?? 0;
        appTemp.Num_Transaction_Id = model.TransactionId;
        appTemp.PaidAmount ??= 0;
        appTemp.PaidAmount += model.PaidAmount;
        appTemp.PaymentDate = model.PaymentDate;

        if (model.CollegeId == 45)
            onlineEducationStudentService.UpdateAppTemp(appTemp, model);

        coreService.Tbl_APP_TEMP_Repository.Add(appTemp);
        //coreService.Save();

        // Save App_Temp Subjects
        foreach (var subject in model.ExamSubjects)
        {
            if (subject.CoursePartId == null) continue;
            if (subject.SubjectCode == null) continue;

            var examSubject = new Tbl_APP_TEMP_SUB
            {
                Num_FK_INST_NO = appTemp.Num_FK_INST_NO,
                Num_FK_ENTRY_ID = appTemp.Num_PK_ENTRY_ID,
                Num_FK_COPRT_NO = (short)subject.CoursePartId,
                Num_FK_SUB_CD = (short)subject.SubjectCode,
                Chr_DELETE_FLG = "N"
            };

            if (subject.SubjectType == nameof(SubjectType.BackLog))
                examSubject.Chr_REPH_FLG = "R";

            coreService.Tbl_APP_TEMP_SUB_Repository.Add(examSubject);
        }
        coreService.Save();
    }

    private void AddInStudentConvo(Data.Corno.Convocation model)
    {
        var instanceId = model.InstanceId;
        var coreService = Bootstrapper.Get<ICoreService>();
        var existing = coreService.Tbl_STUDENT_CONVO_Repository.Get(e => e.Chr_FK_PRN_NO == model.PrnNo &&
                                                                         e.Chr_Transaction_Id == model.TransactionId)
            .FirstOrDefault();
        if (null != existing)
            return;

        var studentConvo = new Tbl_STUDENT_CONVO
        {
            Num_FK_INST_NO = (short?)instanceId,
            Num_CGPA_AVG = model.Cgpa.ToDecimal(),
            Chr_ST_YEAR = DateTime.Now.Year.ToString(),
            Num_FK_CO_CD = model.CourseId,
            Chr_ST_PA_FLG = model.InPerson == false ? "A" : "P",
            Chr_FK_PRN_NO = model.PrnNo,
            Num_ST_SEAT_NO = model.SeatNo.ToLong(),
            Var_ST_NM = ExamServerHelper.GetStudentName(model.PrnNo, coreService),
            Chr_ST_SEX_CD = ExamServerHelper.GetGender(coreService, model.PrnNo),
            Chr_ST_ADD1 = model.Address,
            Chr_ST_ADD2 = string.Empty,
            Chr_ST_ADD3 = string.Empty,
            Chr_ST_ADD4 = string.Empty,
            Chr_ST_PINCODE = model.PinCode.ToString(),
            Var_RES_PH = model.Phone,
            Var_E_MAIL = model.Email,
            Num_ST_PASS_MONTH = (short)model.PassMonth.ToUShort(),
            Chr_ST_PASS_YEAR = model.PassYear,
            Num_ST_PASS_MONTH1 = (short)model.PassMonth1.ToUShort(),
            Chr_ST_PASS_YEAR1 = model.PassYear1,
            AdharNo = model.AdharNo,
            Chr_ST_NMCHNG_FLG = "N",
            Chr_ST_VALID_FLG = "A",
            Chr_DELETE_FLG = "N",
            Num_FK_COLLEGE_CD = model.CollegeId,
            Num_FK_BR_CD = model.BranchId,
            Chr_FEES_STATUS = "C",
            Con_ST_FEES_AMT = model.TotalFee.ToUShort(),
            Destination = model.Destination,
            DegreePart = (short?)model.CoursePartId,
            Chr_Improvement = model.ClassImprovementStudent ? "Y" : "N",
            Chr_Foreign_Student = model.ForeignStudent ? "Y" : "N",
            Chr_PRINC_SUBJECT = model.PrincipleSubject1,
            Chr_PRINC_SUBJECT1 = model.PrincipleSubject2,
            Var_ST_USR_NM = HttpContext.Current.User.Identity.Name,
            ST_DTE_CR = model.CreatedDate,
            ST_DTE_UP = model.ModifiedDate,
            Chr_Form_Submit = string.Empty,
            Num_Fk_Centre_cd = model.CentreId,
            Chr_Transaction_Id = model.TransactionId,
            PaidAmount = model.PaidAmount,
            PaymentDate = model.PaymentDate,
            SettlementDate = model.SettlementDate
        };

        var faculty = coreService.Tbl_COURSE_MSTR_Repository.Get(s => s.Num_PK_CO_CD == model.CourseId).FirstOrDefault();
        if (faculty != null)
            studentConvo.Num_FK_FA_CD = faculty.Num_FK_FA_CD;
        var sysInstance = coreService.Tbl_SYS_INST_Repository.Get(i =>
            i.Num_PK_INST_SRNO == instanceId).FirstOrDefault();
        if (sysInstance?.Num_CONVO_NO != null)
            studentConvo.Num_FK_CONVO_NO = (short)sysInstance.Num_CONVO_NO;
        //if (!string.IsNullOrEmpty(model.ClassCode))
        studentConvo.Num_FK_RESULT_CD = (short)model.ClassCode.ToInt();
        var studentInfo = coreService.Tbl_STUDENT_INFO_ADR_Repository.Get(c => c.Chr_FK_PRN_NO == model.PrnNo).FirstOrDefault();
        if (null != studentInfo)
            studentConvo.Ima_ST_PHOTO = studentInfo.Ima_ST_PHOTO;

        coreService.Tbl_STUDENT_CONVO_Repository.Add(studentConvo);
        coreService.Save();
    }

    // Copied addition logic from EnvironmentController.AddInCoreDatabase
    private void AddInStudentEnvo(EnvironmentStudy model)
    {
        var instanceId = model.InstanceId ?? 0;

        var coreService = Bootstrapper.Get<ICoreService>();
        // Avoid duplicate insert if already present for PRN + TransactionId
        var existing = coreService.TBl_STUDENT_ENV_STUDIES_Repository.Get(e =>
            e.Chr_FK_PRN_NO == model.PrnNo && e.Chr_Transaction_Id == model.TransactionId).FirstOrDefault();
        if (existing != null)
            return;

        // Get Environment Month & Year from Online DB (same as controller)
        var cornoService = Bootstrapper.Get<ICornoService>();
        var environmentSetting = cornoService.EnvironmentSettingRepository
            .Get(i => i.InstanceId == instanceId).FirstOrDefault();
        if (environmentSetting == null)
            throw new Exception("Instance No. " + instanceId + " is not created in EnvironmentSetting Table.");

        var environmentStudies = new TBl_STUDENT_ENV_STUDIES
        {
            Chr_FK_PRN_NO = model.PrnNo,

            Num_FK_CO_CD = null == model.CourseId ? (short)0 : (short)model.CourseId,
            Num_FK_COL_CD = (short?)model.CollegeId,
            Num_FK_DistCenter_ID = (short)(model.CenterId ?? 0),
            Num_FK_COPRT_NO = null == model.CoursePartId ? 0 : (short?)model.CoursePartId,
            Chr_ST_SUB_STS = "A",
            Num_FK_INST_NO = (short)instanceId,
            Num_YEAR = environmentSetting.Year.ToString(),
            Num_MONTH_NO = (short)(environmentSetting.Month ?? 0),
            Chr_ST_SUB_RES = null,
            Num_ST_SUB_MRK = 0,
            Num_EnviFee = model.EnvironmentFee,
            Num_EnviLateFee = model.LateFee,
            Num_EnviSuperLateFee = model.SuperLateFee,
            Num_EnviOtherFee = model.OtherFee,
            Num_EnviTotalFee = model.TotalFee,
            Var_USR_NM = HttpContext.Current?.User?.Identity?.Name ?? ModelConstants.Online,
            Dtm_DTE_CR = model.CreatedDate,
            Dtm_DTE_UP = model.ModifiedDate,
            Chr_Transaction_Id = model.TransactionId,
            PaidAmount = model.PaidAmount,
            PaymentDate = model.PaymentDate,
            SettlementDate = model.SettlementDate
        };

        coreService.TBl_STUDENT_ENV_STUDIES_Repository.Add(environmentStudies);
        coreService.Save();
    }

    private IList<Payout> GetPayouts(ICoreService coreService, DateTime fromDate, DateTime toDate)
    {
        return coreService.PayoutRepository.Get(p =>
                DbFunctions.TruncateTime(p.SettlementDate) >= DbFunctions.TruncateTime(fromDate) &&
                DbFunctions.TruncateTime(p.SettlementDate) <= DbFunctions.TruncateTime(toDate))
            .ToList();
    }

    private IList<Tbl_COLLEGE_MSTR> GetColleges(ICoreService coreService, IEnumerable<int?> collegeIds)
    {
        var ids = collegeIds.Distinct();
        return coreService.TBL_COLLEGE_MSTRRepository.Get(c => ids.Contains(c.Num_PK_COLLEGE_CD)).ToList();
    }

    private class ExamResult
    {
        public List<Tbl_APP_TEMP> AppTemps { get; set; }
        public List<Payout> ProblemPayouts { get; set; }
    }

    private ExamResult ProcessExamPayouts(ICoreService coreService, ICornoService cornoService, IList<Payout> payoutEntries)
    {
        var examPayouts = payoutEntries.Where(s => s.FormType == nameof(FormType.Exam)).ToList();
        var appTemps = new List<Tbl_APP_TEMP>();

        if (examPayouts.Count > 0)
        {
            appTemps = (from settlementEntry in examPayouts
                        join appTemp in coreService.Tbl_APP_TEMP_Repository.Get() on new
                        {
                            InstanceId = settlementEntry.InstanceId ?? 0,
                            CollegeId = settlementEntry.CollegeId ?? 0,
                            settlementEntry.Prn,
                            settlementEntry.TransactionId
                        }
                        equals new
                        {
                            InstanceId = (int)(appTemp.Num_FK_INST_NO ?? 0),
                            CollegeId = (int)appTemp.Num_FK_COLLEGE_CD,
                            Prn = appTemp.Chr_APP_PRN_NO,
                            TransactionId = appTemp.Num_Transaction_Id
                        }
                        select appTemp).ToList();
        }

        var problemExamPayouts = new List<Payout>();

        foreach (var payout in examPayouts)
        {
            var appTempByPrn = appTemps.Where(p => p.Chr_APP_PRN_NO == payout.Prn).ToList();
            switch (appTempByPrn.Count)
            {
                case <= 0:
                    {
                        problemExamPayouts.Add(payout);
                        var exam = cornoService.ExamRepository.Get(e => e.InstanceId == payout.InstanceId &&
                                                                        e.PrnNo == payout.Prn &&
                                                                        e.TransactionId == payout.TransactionId)
                            .FirstOrDefault();
                        if (null != exam)
                            AddInAppTemp(exam);
                        continue;
                    }
                case > 1:
                    problemExamPayouts.Add(payout);
                    continue;
            }

            if (!payout.SettlementAmount.Equals(appTempByPrn.Sum(p => p.Num_TotalFee)))
                problemExamPayouts.Add(payout);
        }

        return new ExamResult
        {
            AppTemps = appTemps,
            ProblemPayouts = problemExamPayouts
        };
    }

    private class EnvironmentResult
    {
        public List<TBl_STUDENT_ENV_STUDIES> EnvStudies { get; set; }
        public List<Payout> ProblemPayouts { get; set; }
    }

    private EnvironmentResult ProcessEnvironmentPayouts(ICoreService coreService, IList<Payout> payoutEntries)
    {
        var environmentPayouts = payoutEntries.Where(s => s.FormType == FormType.Environment.ToString()).ToList();
        var environmentStudies = new List<TBl_STUDENT_ENV_STUDIES>();

        if (environmentPayouts.Count > 0)
        {
            environmentStudies = (from settlementEntry in environmentPayouts
                                  join environmentStudy in coreService.TBl_STUDENT_ENV_STUDIES_Repository.Get() on
                                      new { CollegeId = settlementEntry.CollegeId ?? 0, settlementEntry.Prn, settlementEntry.TransactionId }
                                      equals new
                                      {
                                          CollegeId = (int)(environmentStudy.Num_FK_COL_CD ?? 0),
                                          Prn = environmentStudy.Chr_FK_PRN_NO,
                                          TransactionId = environmentStudy.Chr_Transaction_Id
                                      }
                                  select environmentStudy).ToList();
        }

        var problemEnvironmentPayouts = new List<Payout>();
        var cornoService = Bootstrapper.Get<ICornoService>();

        foreach (var payout in environmentPayouts)
        {
            var envStudyByPrn = environmentStudies.Where(p => p.Chr_FK_PRN_NO == payout.Prn).ToList();
            switch (envStudyByPrn.Count)
            {
                case <= 0:
                    {
                        problemEnvironmentPayouts.Add(payout);
                        // Try to add missing Environment record from Online DB, like Exam/Convocation
                        var environmentToAdd = cornoService.EnvironmentStudyRepository.Get(e =>
                            e.PrnNo == payout.Prn && e.TransactionId == payout.TransactionId).FirstOrDefault();
                        if (environmentToAdd != null)
                            AddInStudentEnvo(environmentToAdd);
                        continue;
                    }
                case > 1:
                    problemEnvironmentPayouts.Add(payout);
                    continue;
            }

            if (!payout.SettlementAmount.Equals(envStudyByPrn.Sum(p => p.Num_EnviTotalFee)))
                problemEnvironmentPayouts.Add(payout);
        }

        return new EnvironmentResult
        {
            EnvStudies = environmentStudies,
            ProblemPayouts = problemEnvironmentPayouts
        };
    }

    private class ConvocationResult
    {
        public List<Tbl_STUDENT_CONVO> Convocations { get; set; }
        public List<Payout> ProblemPayouts { get; set; }
    }

    private ConvocationResult ProcessConvocationPayouts(ICoreService coreService, ICornoService cornoService, IList<Payout> payoutEntries)
    {
        var convocationPayouts = payoutEntries.Where(s => s.FormType == FormType.Convocation.ToString()).ToList();
        var convocations = new List<Tbl_STUDENT_CONVO>();

        if (convocationPayouts.Count > 0)
        {
            convocations = (from settlementEntry in convocationPayouts
                            join convocation in coreService.Tbl_STUDENT_CONVO_Repository.Get() on new { settlementEntry.Prn, settlementEntry.TransactionId }
                                equals new { Prn = convocation.Chr_FK_PRN_NO, TransactionId = convocation.Chr_Transaction_Id }
                            select convocation).ToList();
        }

        var problemConvocationPayouts = new List<Payout>();

        foreach (var payout in convocationPayouts)
        {
            var convocationByPrn = convocations.Where(p => p.Chr_FK_PRN_NO == payout.Prn).ToList();
            switch (convocationByPrn.Count)
            {
                case <= 0:
                    {
                        problemConvocationPayouts.Add(payout);
                        var convocationToAdd = cornoService.ConvocationRepository.Get(e =>
                                e.PrnNo == payout.Prn &&
                                e.TransactionId == payout.TransactionId)
                            .FirstOrDefault();
                        if (null != convocationToAdd)
                            AddInStudentConvo(convocationToAdd);
                        continue;
                    }
                case > 1:
                    problemConvocationPayouts.Add(payout);
                    continue;
            }

            if (!payout.SettlementAmount.Equals(convocationByPrn.Sum(p => p.Con_ST_FEES_AMT.ToDouble())))
                problemConvocationPayouts.Add(payout);
        }

        return new ConvocationResult
        {
            Convocations = convocations,
            ProblemPayouts = problemConvocationPayouts
        };
    }

    private void AssignTableData(DateTime fromDate, DateTime toDate/*, FormType formType*/)
    {
        var coreService = Bootstrapper.Get<ICoreService>();
        if (null == coreService) return;

        var payoutEntries = GetPayouts(coreService, fromDate, toDate);
        if (payoutEntries.Count <= 0) return;

        var colleges = GetColleges(coreService, payoutEntries.Select(p => p.CollegeId));

        var cornoService = Bootstrapper.Get<ICornoService>();
        var examResult = ProcessExamPayouts(coreService, cornoService, payoutEntries);
        var envResult = ProcessEnvironmentPayouts(coreService, payoutEntries);
        var convoResult = ProcessConvocationPayouts(coreService, cornoService, payoutEntries);

        var dataSource = payoutEntries
            .GroupBy(g => g.CollegeId)
            .Select(g =>
            {
                var college = colleges.FirstOrDefault(c => c.Num_PK_COLLEGE_CD == g.Key);

                var groupedAppTemps = examResult.AppTemps.Where(a => a.Num_FK_COLLEGE_CD == (g.Key ?? 0)).ToList();
                var groupedEnvironments = envResult.EnvStudies.Where(a => (a.Num_FK_COL_CD ?? 0) == (g.Key ?? 0)).ToList();
                var groupedConvocations = convoResult.Convocations.Where(a => (a.Num_FK_COLLEGE_CD ?? 0) == (g.Key ?? 0)).ToList();

                var groupedProblemExamPayouts = examResult.ProblemPayouts.Where(a => (a.CollegeId ?? 0) == (g.Key ?? 0))
                    .Select(x => x.Prn).ToList();
                var groupedProblemEnvironmentPayouts = envResult.ProblemPayouts.Where(a => (a.CollegeId ?? 0) == (g.Key ?? 0))
                    .Select(x => x.Prn).ToList();
                var groupedProblemConvocationPayouts = convoResult.ProblemPayouts.Where(a => (a.CollegeId ?? 0) == (g.Key ?? 0))
                    .Select(x => x.Prn).ToList();

                return new
                {
                    CollegeId = g.Key,
                    CollegeName = college?.Var_CL_SHRT_NM,
                    CityName = college?.Var_CL_CITY_NM,

                    EaseBuzzExamFee = g.Where(x => x.FormType == FormType.Exam.ToString()).Sum(x => x.SettlementAmount),
                    EaseBuzzEnvironmentFee = g.Where(x => x.FormType == FormType.Environment.ToString()).Sum(x => x.SettlementAmount),
                    EaseBuzzConvocationFee = g.Where(x => x.FormType == FormType.Convocation.ToString()).Sum(x => x.SettlementAmount),

                    ReceivedExamFee = groupedAppTemps.Sum(x => x.Num_TotalFee),
                    ReceivedEnvironmentFee = groupedEnvironments.Sum(x => x.Num_EnviFee),
                    ReceivedConvocationFee = groupedConvocations.Sum(x => x.Con_ST_FEES_AMT),

                    ProblemExamPrn = groupedProblemExamPayouts.Any() ? $"Exam PRN : {string.Join(",", groupedProblemExamPayouts)}" : string.Empty,
                    ProblemEnvironmentPrn = groupedProblemEnvironmentPayouts.Any() ? $"Environment PRN : {string.Join(",", groupedProblemEnvironmentPayouts)}" : string.Empty,
                    ProblemConvocationPrn = groupedProblemConvocationPayouts.Any() ? $"Convocation PRN : {string.Join(",", groupedProblemConvocationPayouts)}" : string.Empty
                };
            }).ToList();

        table1.DataSource = dataSource;
    }
    #endregion

    #region -- Events --
    private void OnlinePaymentSummaryRpt_NeedDataSource(object sender, EventArgs e)
    {
        if (!(sender is Telerik.Reporting.Processing.Report report)) return;

        var fromDate = report.Parameters[ModelConstants.From].Value.ToDateTime();
        var toDate = report.Parameters[ModelConstants.To].Value.ToDateTime();
        //Enum.TryParse(report.Parameters[ModelConstants.Form].Value.ToString(), out FormType formType);

        AssignTableData(fromDate, toDate/*, formType*/);
    }

    private void OnlinePaymentSummaryRpt_Error(object sender, ErrorEventArgs eventArgs)
    {
        LogHandler.LogInfo(eventArgs.Exception);

        var procEl = (ProcessingElement)sender;
        procEl.Exception = null;
    }
    #endregion
}