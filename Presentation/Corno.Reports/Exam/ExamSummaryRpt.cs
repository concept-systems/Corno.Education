namespace ReportLibrary.Exam
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for ExamSummaryRpt.
    /// </summary>
    public partial class ExamSummaryRpt : Telerik.Reporting.Report
    {
        public ExamSummaryRpt()
        {
            //
            // Required for telerik Reporting designer support
            //
            InitializeComponent();
            sdsExamSummary.ConnectionString = sdsExamSummary.ConnectionString;
            sdsExamSummary.SelectCommand = sdsExamSummary.SelectCommand.Replace("@CompanyID", "1");
        }
    }
}