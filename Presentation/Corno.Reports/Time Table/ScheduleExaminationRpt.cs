using System;
using Corno.Globals;
using Telerik.Reporting;

namespace Corno.Reports.Time_Table;

public partial class ScheduleExaminationRpt : Report
{
    #region -- Connstructors --
    public ScheduleExaminationRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    public ScheduleExaminationRpt(int instanceId, int collegeId)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        sdsCollege.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCourse.ConnectionString = GlobalVariables.ConnectionStringExamServer;

        _instanceId = instanceId;
    }
    #endregion

    #region -- Data Members --

    private readonly int _instanceId;
    #endregion

    #region -- Events --
    private void Schedule1Rpt_NeedDataSource(object sender, EventArgs e)
    {
        ScheduleCAPRpt.HandleReport(sender, _instanceId);
    }
    #endregion
}