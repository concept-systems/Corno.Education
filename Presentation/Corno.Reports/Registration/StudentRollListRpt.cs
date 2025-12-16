using Corno.Globals;
using Telerik.Reporting;

namespace Corno.Reports.Registration;

public partial class StudentRollListRpt : Report
{
    public StudentRollListRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    public StudentRollListRpt(int instanceId, int collegeId)
    {
        InitializeComponent();

        sdsCollege.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        if (collegeId > 0)
            sdsCollege.SelectCommand += " And Num_PK_COLLEGE_CD = " + collegeId;

        sdsMain.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsMain.SelectCommand = sdsMain.SelectCommand.Replace("@InstanceID",
            instanceId.ToString());
    }
}