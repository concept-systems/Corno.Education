using System.Linq;
using Corno.Globals;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Report = Telerik.Reporting.Report;

namespace Corno.Reports.Convocation;

/// <summary>
///     Summary description for ConvocationRpt.
/// </summary>
public partial class ConvocationRpt : Report
{
    public ConvocationRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    public ConvocationRpt(int instanceId)
    {
        InitializeComponent();

        sdsCollege.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCourse.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsMain.ConnectionString = GlobalVariables.ConnectionStringExamServer;

        var examService = Bootstrapper.Get<ICoreService>();
        var sysInstance = examService.Tbl_SYS_INST_Repository.Get(i => i.Num_PK_INST_SRNO == instanceId).FirstOrDefault();
        var convocationNo = 0;
        if (sysInstance?.Num_CONVO_NO != null)
            convocationNo = (short) sysInstance.Num_CONVO_NO;

        sdsMain.SelectCommand = sdsMain.SelectCommand.Replace("@ConvocationNo",
            convocationNo.ToString());
    }

    private void ConvocationRpt_Error(object sender, ErrorEventArgs eventArgs)
    {
        var procEl = (ProcessingElement)sender;
        procEl.Exception = null;
    }
}