using Corno.Globals;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Report = Telerik.Reporting.Report;

namespace Corno.Reports.Registration;

/// <summary>
///     Summary description for CollegewiseRegistration.
/// </summary>
public partial class CollegeWiseRegistration : Report
{
    public CollegeWiseRegistration()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    public CollegeWiseRegistration(int instanceId, int collegeId)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        sdsCollege.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        if (collegeId > 0)
            sdsCollege.SelectCommand += " And Num_PK_COLLEGE_CD = " + collegeId;
        sdsCourse.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCoursePart.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsBranch.ConnectionString = GlobalVariables.ConnectionStringExamServer;

        sdsMain.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsMain.SelectCommand = sdsMain.SelectCommand.Replace("@InstanceID",
            instanceId.ToString());
    }

    // Don't Delete. This event is specifically for picturebox null event.
    private void CollegeWiseRegistration_Error(object sender, ErrorEventArgs eventArgs)
    {
        var procEl = (ProcessingElement)sender;
        procEl.Exception = null;
    }
}