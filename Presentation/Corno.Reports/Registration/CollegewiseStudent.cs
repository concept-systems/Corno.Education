using Corno.Globals;
using Telerik.Reporting;

namespace Corno.Reports.Registration;

/// <summary>
///     Summary description for CollegewiseStudent.
/// </summary>
public partial class CollegewiseStudent : Report
{
    public CollegewiseStudent()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    public CollegewiseStudent(int instanceId, int collegeId)
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

        sdsMain.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsMain.SelectCommand = sdsMain.SelectCommand.Replace("@InstanceID",
            instanceId.ToString());
    }
}