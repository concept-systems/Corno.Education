using Corno.Globals;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Helper;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Report = Telerik.Reporting.Report;

namespace Corno.Reports.Convocation;

/// <summary>
///     Summary description for CollegewiseRegistration.
/// </summary>
public partial class ConvocationFeeSummaryRpt : Report
{
    public ConvocationFeeSummaryRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    public ConvocationFeeSummaryRpt(int instanceId, int collegeId)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        sdsCollege.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        if (collegeId > 0)
            sdsCollege.SelectCommand += " And Num_PK_COLLEGE_CD = " + collegeId;
        sdsCenters.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCourse.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsBranch.ConnectionString = GlobalVariables.ConnectionStringExamServer;

        // Get Convo No from Instance Id
        var examService = Bootstrapper.Get<ICoreService>();
        var convoNo = ExamServerHelper.GetConvoNo(instanceId, examService);

        sdsStudentFeeRpt.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsStudentFeeRpt.SelectCommand = sdsStudentFeeRpt.SelectCommand.Replace("@ConvoNo", convoNo.ToString());
        //sdsStudentFeeRpt.SelectCommand = sdsStudentFeeRpt.SelectCommand.Replace("@InstanceID", instanceId.ToString());
    }

    private void ConvocationFeeSummaryRpt_Error(object sender, ErrorEventArgs eventArgs)
    {
        var procEl = (ProcessingElement) sender;
        procEl.Exception = null;
    }
}