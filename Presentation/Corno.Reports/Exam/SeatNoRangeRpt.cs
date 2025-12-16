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
    /// Summary description for SeatNoRangeRpt.
    /// </summary>
    public partial class SeatNoRangeRpt : Telerik.Reporting.Report
    {
        public SeatNoRangeRpt()
        {
            InitializeComponent();
        }
        public SeatNoRangeRpt(int instanceID)
        {
            InitializeComponent();
            sdsCollege.ConnectionString = GlobalVariables.ConnectionString_ExamServer;
            sdsCourse.ConnectionString = GlobalVariables.ConnectionString_ExamServer;
            sdsCoursePart.ConnectionString = GlobalVariables.ConnectionString_ExamServer;

            sdsSeatNoRange.ConnectionString = sdsSeatNoRange.ConnectionString;
            sdsSeatNoRange.SelectCommand = sdsSeatNoRange.SelectCommand.Replace("@InstanceID", instanceID.ToString());
        }
    }
}