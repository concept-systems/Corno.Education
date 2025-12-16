namespace ReportLibrary.Convocation
{
    using Globals;
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for StudentsAppliedRpt.
    /// </summary>
    public partial class StudentsAppliedRpt : Telerik.Reporting.Report
    {
        public StudentsAppliedRpt()
        {
            InitializeComponent();

            sdsConvocationStudent.ConnectionString = GlobalVariables.ConnectionString;
            sdsConvocationStudent.SelectCommand = sdsConvocationStudent.SelectCommand.Replace("@CompanyID", "1");
        }
    }
}