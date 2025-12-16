using Corno.Globals;
using Telerik.Reporting;

namespace Corno.Reports.Environment;

/// <summary>
///     Summary description for CollegewiseStudent.
/// </summary>
public partial class EnvironmentCapRpt : Report
{
    public EnvironmentCapRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    public EnvironmentCapRpt(int instanceId, int collegeId)
    {
        //var instanceID = (int)HttpContext.Session[ModelConstants.InstanceID]
        // Required for telerik Reporting designer support
        InitializeComponent();

        sdsCollege.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        if (collegeId > 0)
            sdsCollege.SelectCommand += " And Num_PK_COLLEGE_CD = " + collegeId;
        sdsCourse.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCoursePart.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsBranch.ConnectionString = GlobalVariables.ConnectionStringExamServer;

        sdsEnvList.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsEnvList.SelectCommand = sdsEnvList.SelectCommand.Replace("@InstanceID", instanceId.ToString());
    }
}