using System;
using System.Data.Entity;
using System.Linq;
using Corno.Data.Helpers;
using Corno.Globals.Constants;
using Corno.Logger;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Report = Telerik.Reporting.Report;

namespace Corno.Reports.Accounts;

public partial class PayoutStudentsRpt : Report
{
    #region -- Constructors --
    public PayoutStudentsRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        _coreService = Bootstrapper.Get<ICoreService>();
    }
    #endregion

    #region -- Data Members --

    private readonly ICoreService _coreService;
    #endregion

    #region -- Private Methods --

    private void AssignTableData(DateTime fromDate, DateTime toDate/*, FormType formType*/)
    {
        //var coreService = (ICoreService)Bootstrapper.GetService(typeof(CoreService));
        //var cornoService = (ICornoService)Bootstrapper.GetService(typeof(CornoService));
        if (null == _coreService) return;

        var payouts = _coreService.PayoutRepository.Get(p =>
                DbFunctions.TruncateTime(p.SettlementDate) >= DbFunctions.TruncateTime(fromDate) &&
                DbFunctions.TruncateTime(p.SettlementDate) <= DbFunctions.TruncateTime(toDate))
            .ToList();

        if (payouts.Count <= 0) return;

        var dataSource = payouts.Select(m =>
        {
            //var college = colleges.FirstOrDefault(c => c.Num_PK_COLLEGE_CD == m.CollegeId);

            return new
            {
                m.InstanceId,
                m.CollegeId,
                //CollegeName = college?.Var_CL_SHRT_NM,
                m.CourseId,
                m.CoursePartId,
                m.BranchId,
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

        AssignTableData(fromDate, toDate);
    }

    private void OnlinePaymentSummaryRpt_Error(object sender, ErrorEventArgs eventArgs)
    {
        LogHandler.LogInfo(eventArgs.Exception);

        var procEl = (ProcessingElement)sender;
        procEl.Exception = null;
    }

        
    #endregion
}