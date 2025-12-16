using System;
using System.Data.Entity;
using System.Linq;
using Corno.Data.Helpers;
using Corno.Globals.Constants;
using Corno.Globals.Enums;
using Corno.Logger;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Report = Telerik.Reporting.Report;

namespace Corno.Reports.Accounts;

public partial class PayoutMissingRpt : Report
{
    #region -- Constructors --
    public PayoutMissingRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        _coreService = Bootstrapper.Get<ICoreService>();

        //ReportParameters[ModelConstants.Form].AvailableValues.DataSource = new List<MasterViewModel>
        //{
        //    new MasterViewModel{ Id = (int)FormType.All, Name = FormType.All.ToString()},
        //    new MasterViewModel{ Id = (int)FormType.Exam, Name = FormType.Exam.ToString() },
        //    new MasterViewModel{ Id = (int)FormType.Environment, Name = FormType.Environment.ToString() },
        //    new MasterViewModel{ Id = (int)FormType.Convocation, Name = FormType.Convocation.ToString() }
        //};
    }
    #endregion

    #region -- Data Members --

    private readonly ICoreService _coreService;
    #endregion

    #region -- Private Methods --

    private void AssignTableData(DateTime fromDate, DateTime toDate/*, FormType formType*/)
    {
        if (null == _coreService) return;

        var payoutEntries = _coreService.PayoutRepository.Get(p =>
                DbFunctions.TruncateTime(p.SettlementDate) >= DbFunctions.TruncateTime(fromDate) &&
                DbFunctions.TruncateTime(p.SettlementDate) <= DbFunctions.TruncateTime(toDate))
            .ToList();

        if (payoutEntries.Count <= 0) return;

        var collegeIds = payoutEntries.Select(p => p.CollegeId).Distinct();
        var colleges = _coreService.TBL_COLLEGE_MSTRRepository.Get(c => collegeIds.Contains(c.Num_PK_COLLEGE_CD))
            .ToList();

        var examPayouts = payoutEntries.Where(s => s.FormType == FormType.Exam.ToString()).ToList();
        var appTemps = (from payoutEntry in examPayouts
            join appTemp in _coreService.Tbl_APP_TEMP_Repository.Get() on new { InstanceId = payoutEntry.InstanceId ?? 0, CollegeId = payoutEntry.CollegeId ?? 0, payoutEntry.Prn }
                equals new { InstanceId = (int)(appTemp.Num_FK_INST_NO ?? 0), CollegeId = (int)(appTemp.Num_FK_COLLEGE_CD), Prn = appTemp.Chr_APP_PRN_NO }
            select appTemp).ToList();
        var appTempPrnList = appTemps.Select(a => a.Chr_APP_PRN_NO).Distinct().ToList();
        var missingExamPayouts = examPayouts.Where(e => !appTempPrnList.Contains(e.Prn)).ToList();

        var environmentPayouts = payoutEntries.Where(s => s.FormType == FormType.Environment.ToString()).ToList();
        var environmentStudies = (from payoutEntry in environmentPayouts
            join environmentStudy in _coreService.TBl_STUDENT_ENV_STUDIES_Repository.Get() on new { CollegeId = payoutEntry.CollegeId ?? 0, payoutEntry.Prn }
                equals new { CollegeId = (int)(environmentStudy.Num_FK_COL_CD ?? 0), Prn = environmentStudy.Chr_FK_PRN_NO }
            select environmentStudy).ToList();
        var environmentPrnList = environmentStudies.Select(a => a.Chr_FK_PRN_NO).Distinct().ToList();
        var missingEnvironmentPayouts = environmentPayouts.Where(e => !environmentPrnList.Contains(e.Prn)).ToList();

        var convocationPayouts = payoutEntries.Where(s => s.FormType == FormType.Convocation.ToString()).ToList();
        var convocations = (from payout in convocationPayouts
            join convocation in _coreService.Tbl_STUDENT_CONVO_Repository.Get() on new { CollegeId = payout.CollegeId ?? 0, payout.Prn }
                equals new { CollegeId = (int)(convocation.Num_FK_COLLEGE_CD ?? 0), Prn = convocation.Chr_FK_PRN_NO }
            select convocation).ToList();
        var convocationPrnList = convocations.Select(a => a.Chr_FK_PRN_NO).Distinct().ToList();
        var missingConvocationSettlements = convocationPayouts.Where(e => !convocationPrnList.Contains(e.Prn)).ToList();

        var missingPayouts = missingExamPayouts.Concat(missingEnvironmentPayouts)
            .Concat(missingConvocationSettlements);

        var dataSource = missingPayouts.Select(m =>
        {
            var college = colleges.FirstOrDefault(c => c.Num_PK_COLLEGE_CD == m.CollegeId);

            return new
            {
                m.InstanceId,
                m.CollegeId,
                CollegeName = college?.Var_CL_SHRT_NM,
                m.CourseId,
                m.Prn,
                m.TransactionId,
                m.EazeBuzzTransactionId,
                m.Amount,
                m.SettlementAmount,
                m.TransactionType,
                m.SettlementDate,
                m.FormType
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