namespace ReportLibrary.Exam
{
    using Globals;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for TimeTableRpt.
    /// </summary>
    public partial class TimeTableRpt : Telerik.Reporting.Report
    {
        public TimeTableRpt()
        {
            InitializeComponent();
        }
        public TimeTableRpt(int instanceID)
        {
            InitializeComponent();
            sdsCourse.ConnectionString = GlobalVariables.ConnectionString_ExamServer;
            sdsCourse.ConnectionString = GlobalVariables.ConnectionString_ExamServer;

            sdsTimeTable.ConnectionString = sdsTimeTable.ConnectionString;
            sdsTimeTable.SelectCommand = sdsTimeTable.SelectCommand.Replace("@InstanceID", instanceID.ToString());
        }
    }
}