namespace ReportLibrary.Convocation
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for HighestMarksStudents.
    /// </summary>
    public partial class FirstPosition : Telerik.Reporting.Report
    {
        public FirstPosition()
        {
            // Required for telerik Reporting designer support
            InitializeComponent();

            sdsFirstPosition.ConnectionString = sdsFirstPosition.ConnectionString;
            sdsFirstPosition.SelectCommand = sdsFirstPosition.SelectCommand.Replace("@CompanyID", "1");
        }
    }
}