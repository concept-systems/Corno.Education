using Corno.Globals;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Report = Telerik.Reporting.Report;

namespace Corno.Reports.Environment;

public partial class EnvironmentFeeSummaryRpt : Report
{
    public EnvironmentFeeSummaryRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    public EnvironmentFeeSummaryRpt(int instanceId, int collegeId)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        sdsCollege.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        if (collegeId > 0)
            sdsCollege.SelectCommand += " And Num_PK_COLLEGE_CD = " + collegeId;
        sdsCenters.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCourse.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCoursePart.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsBranch.ConnectionString = GlobalVariables.ConnectionStringExamServer;

        sdsStudentFeeRpt.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsStudentFeeRpt.SelectCommand = sdsStudentFeeRpt.SelectCommand.Replace("@InstanceID", instanceId.ToString());
    }

    private void EnvironmentFeeSummaryRpt_Error(object sender, ErrorEventArgs eventArgs)
    {
        var procEl = (ProcessingElement) sender;
        procEl.Exception = null;
    }
}