namespace ReportLibrary.Convocation
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for NotAppearStudents.
    /// </summary>
    public partial class NotAppearStudents : Telerik.Reporting.Report
    {
        public NotAppearStudents()
        {
            // Required for telerik Reporting designer support

            InitializeComponent();
            sdsNotAppear.ConnectionString = sdsNotAppear.ConnectionString;
            sdsNotAppear.SelectCommand = sdsNotAppear.SelectCommand.Replace("@CompanyID", "1");
        }
    }
}