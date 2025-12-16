using Corno.Globals;
using Telerik.Reporting;

namespace Corno.Reports.Exam;

public partial class ExamNameListRpt : Report
{
    public ExamNameListRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    public ExamNameListRpt(int instanceId)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        sdsCollege.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCenters.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCourse.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCoursePart.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsBranch.ConnectionString = GlobalVariables.ConnectionStringExamServer;

        sdsMain.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsMain.SelectCommand = sdsMain.SelectCommand.Replace("@InstanceID", instanceId.ToString());
    }
}