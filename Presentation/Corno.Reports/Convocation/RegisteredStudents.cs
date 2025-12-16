namespace ReportLibrary.Convocation
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for RegisteredStudents.
    /// </summary>
    public partial class RegisteredStudents : Telerik.Reporting.Report
    {
        public RegisteredStudents()
        {
            InitializeComponent();
            sdsRegisteredStudent.ConnectionString = sdsRegisteredStudent.ConnectionString;
            sdsRegisteredStudent.SelectCommand = sdsRegisteredStudent.SelectCommand.Replace("@CompanyID", "1");
        }
    }
}