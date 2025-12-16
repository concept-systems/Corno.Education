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

public partial class PayoutExamRpt : Report
{
    #region -- Constructors --
    public PayoutExamRpt()
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

    private void AssignTableData(DateTime fromDate, DateTime toDate/*, FormType formType*/)
    {
        var coreService = Bootstrapper.Get<ICoreService>();
        //var cornoService = (ICornoService)Bootstrapper.GetService(typeof(CornoService));
        if (null == coreService ) return;

        /*var settledEntries = formType switch
        {
            FormType.All => cornoService.PayoutRepository.Get(p =>
                    DbFunctions.TruncateTime(p.SettlementDate) >= DbFunctions.TruncateTime(fromDate) &&
                    DbFunctions.TruncateTime(p.SettlementDate) <= DbFunctions.TruncateTime(toDate))
                .ToList(),
            _ => cornoService.PayoutRepository.Get(p =>
                    DbFunctions.TruncateTime(p.SettlementDate) >= DbFunctions.TruncateTime(fromDate) &&
                    DbFunctions.TruncateTime(p.SettlementDate) <= DbFunctions.TruncateTime(toDate) &&
                    p.FormType == formType.ToString())
                .ToList()
        };*/

        var settledEntries = coreService.PayoutRepository.Get(p =>
                DbFunctions.TruncateTime(p.SettlementDate) >= DbFunctions.TruncateTime(fromDate) &&
                DbFunctions.TruncateTime(p.SettlementDate) <= DbFunctions.TruncateTime(toDate))
            .ToList();

        if (settledEntries.Count <= 0) return;

        var collegeIds = settledEntries.Select(p => p.CollegeId).Distinct();
        var colleges = coreService.TBL_COLLEGE_MSTRRepository.Get(c => collegeIds.Contains(c.Num_PK_COLLEGE_CD))
            .ToList();

        var appTemps = (from settlementEntry in settledEntries.Where(s => s.FormType == FormType.Exam.ToString())
            join appTemp in coreService.Tbl_APP_TEMP_Repository.Get() on new { InstanceId = settlementEntry.InstanceId ?? 0, CollegeId = settlementEntry.CollegeId ?? 0, settlementEntry.Prn, settlementEntry.TransactionId }
                equals new { InstanceId = (int)(appTemp.Num_FK_INST_NO ?? 0), CollegeId = (int)(appTemp.Num_FK_COLLEGE_CD), Prn = appTemp.Chr_APP_PRN_NO, TransactionId = appTemp.Num_Transaction_Id }
            select appTemp).ToList();

        var problemAppTemps = (from appTemp in appTemps
            let calculatedFee = (appTemp.Num_ExamFee ?? 0) + (appTemp.Num_BacklogFee ?? 0) + (appTemp.Num_CAPFee ?? 0) + (appTemp.Num_StatementFee ?? 0) + 
                                (appTemp.Num_LateFee ?? 0) + (appTemp.Num_SuperLateFee ?? 0) + (appTemp.Num_Fine ?? 0) + (appTemp.Num_PassingCertificateFee ?? 0) + 
                                (appTemp.Num_DissertationFee ?? 0)
            where !appTemp.Num_TotalFee.Equals(calculatedFee)
            select appTemp).ToList();

        /*var environmentStudies = (from settlementEntry in settledEntries.Where(s => s.FormType == FormType.Environment.ToString())
            join environmentStudy in coreService.TBl_STUDENT_ENV_STUDIES_Repository.Get() on new { InstanceId = settlementEntry.InstanceId ?? 0, CollegeId = settlementEntry.CollegeId ?? 0, settlementEntry.Prn }
                equals new { InstanceId = (int)(environmentStudy.Num_FK_INST_NO ?? 0), CollegeId = (int)(environmentStudy.Num_FK_COL_CD ?? 0), Prn = environmentStudy.Chr_FK_PRN_NO }
            select environmentStudy).ToList();
        var convocations = (from settlementEntry in settledEntries.Where(s => s.FormType == FormType.Convocation.ToString())
            join convocation in coreService.Tbl_STUDENT_CONVO_Repository.Get() on new { InstanceId = settlementEntry.InstanceId ?? 0, CollegeId = settlementEntry.CollegeId ?? 0, settlementEntry.Prn }
                equals new { InstanceId = (int)(convocation.Num_FK_INST_NO ?? 0), CollegeId = (int)(convocation.Num_FK_COLLEGE_CD ?? 0), Prn = convocation.Chr_FK_PRN_NO }
            select convocation).ToList();*/

        var dataSource = settledEntries.GroupBy(g => g.CollegeId)
            .Select(g =>
            {
                var college = colleges.FirstOrDefault(c => c.Num_PK_COLLEGE_CD == g.Key);

                var groupedAppTemps = appTemps.Where(a => a.Num_FK_COLLEGE_CD == g.Key).ToList();
                var groupedProblemAppTemps = problemAppTemps.Where(a => a.Num_FK_COLLEGE_CD == g.Key).Select(x => x.Chr_APP_PRN_NO).ToList();
                var firstOrDefault = groupedAppTemps.FirstOrDefault();
                return new
                {
                    CollegeId = g.Key,
                    CollegeName = college?.Var_CL_SHRT_NM,
                    CityName = college?.Var_CL_CITY_NM,

                    ReceivedExamFee = g.Where(x => x.FormType == FormType.Exam.ToString()).Sum(x => x.SettlementAmount),
                    ReceivedEnvironmentFee = g.Where(x => x.FormType == FormType.Environment.ToString()).Sum(x => x.SettlementAmount),
                    ReceivedConvocationFee = g.Where(x => x.FormType == FormType.Convocation.ToString()).Sum(x => x.SettlementAmount),

                    Num_ExamFee = groupedAppTemps.Sum(x => x.Num_ExamFee ?? 0),
                    Num_BacklogFee = groupedAppTemps.Sum(x => x.Num_BacklogFee ?? 0),
                    Num_CAPFee = groupedAppTemps.Sum(x => x.Num_CAPFee ?? 0),
                    Num_StatementFee = groupedAppTemps.Sum(x => x.Num_StatementFee ?? 0),
                    Num_LateFee = groupedAppTemps.Sum(x => x.Num_LateFee ?? 0),
                    Num_SuperLateFee = groupedAppTemps.Sum(x => x.Num_SuperLateFee ?? 0),
                    Num_Fine = groupedAppTemps.Sum(x => x.Num_Fine ?? 0),
                    Num_PassingCertificateFee = groupedAppTemps.Sum(x => x.Num_PassingCertificateFee ?? 0),
                    Num_DissertationFee = groupedAppTemps.Sum(x => x.Num_DissertationFee ?? 0),
                    Num_TotalFee = groupedAppTemps.Sum(x => x.Num_TotalFee ?? 0),

                    Num_Online_Fee = groupedAppTemps.Where(x => !string.IsNullOrEmpty(x.Num_Transaction_Id)).Sum(x => x.Num_TotalFee ?? 0),
                    Num_Offline_Fee = groupedAppTemps.Where(x => string.IsNullOrEmpty(x.Num_Transaction_Id) && x.Form_at_Exam != "Y").Sum(x => x.Num_TotalFee ?? 0),
                    Num_At_University_Fee = groupedAppTemps.Where(x => string.IsNullOrEmpty(x.Num_Transaction_Id) && x.Form_at_Exam == "Y").Sum(x => x.Num_TotalFee ?? 0),

                    firstOrDefault?.Chr_Fee_Submit,
                    firstOrDefault?.Form_at_Exam,

                    ProblemPrn = groupedProblemAppTemps.Any() ? string.Join(",", groupedProblemAppTemps) : string.Empty
                };
            }).ToList();

        table1.DataSource = dataSource;
        //table2.DataSource = dataSource;
    }

    #endregion

    #region -- Events --
    private void PayoutExamRpt_NeedDataSource(object sender, EventArgs e)
    {
        if (!(sender is Telerik.Reporting.Processing.Report report)) return;

        var fromDate = report.Parameters[ModelConstants.From].Value.ToDateTime();
        var toDate = report.Parameters[ModelConstants.To].Value.ToDateTime();
        //Enum.TryParse(report.Parameters[ModelConstants.Form].Value.ToString(), out FormType formType);

        AssignTableData(fromDate, toDate/*, formType*/);
    }

    private void PayoutExamRpt_Error(object sender, ErrorEventArgs eventArgs)
    {
        LogHandler.LogInfo(eventArgs.Exception);

        var procEl = (ProcessingElement)sender;
        procEl.Exception = null;
    }
    #endregion
}