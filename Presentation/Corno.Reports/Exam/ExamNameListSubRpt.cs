using Corno.Globals;
using Telerik.Reporting;

namespace Corno.Reports.Exam;

public partial class ExamNameListSubRpt : Report
{
    public ExamNameListSubRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        sdsCollege.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCourse.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCoursePart.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsBranch.ConnectionString = GlobalVariables.ConnectionStringExamServer;

        sdsSubjectNames.ConnectionString = GlobalVariables.ConnectionStringExamServer;
    }
}