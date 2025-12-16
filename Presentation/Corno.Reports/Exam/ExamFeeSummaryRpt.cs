using Corno.Globals;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Report = Telerik.Reporting.Report;

namespace Corno.Reports.Exam;

public partial class ExamFeeSummaryRpt : Report
{
    public ExamFeeSummaryRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    public ExamFeeSummaryRpt(int instanceId, int collegeId)
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
        sdsMain.ConnectionString = GlobalVariables.ConnectionStringExamServer;

        sdsMain.SelectCommand = sdsMain.SelectCommand.Replace("@InstanceID", instanceId.ToString());
    }

    private void ExamFeeSummaryRpt_Error(object sender, ErrorEventArgs eventArgs)
    {
        var procEl = (ProcessingElement)sender;
        procEl.Exception = null;
    }
}