namespace ReportLibrary.Convocation
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for ConvocationDistancewise.
    /// </summary>
    public partial class ConvocationDistancewise : Telerik.Reporting.Report
    {
        public ConvocationDistancewise()
        {
            // Required for telerik Reporting designer support
            InitializeComponent();
            sdsConvocationdistance.ConnectionString = sdsConvocationdistance.ConnectionString;
            sdsConvocationdistance.SelectCommand = sdsConvocationdistance.SelectCommand.Replace("@CompanyID", "1");
        }
    }
}