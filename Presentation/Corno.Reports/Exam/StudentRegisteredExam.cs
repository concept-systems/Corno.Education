namespace ReportLibrary.Exam
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for StudentRegisteredExam.
    /// </summary>
    public partial class StudentRegisteredExam : Telerik.Reporting.Report
    {
        public StudentRegisteredExam()
        {
            InitializeComponent();
        }
        public StudentRegisteredExam(int instanceID)
        {
            InitializeComponent();
            sdsStudentsRegisterExam.ConnectionString = sdsStudentsRegisterExam.ConnectionString;
            sdsStudentsRegisterExam.SelectCommand = sdsStudentsRegisterExam.SelectCommand.Replace("@InstanceID", instanceID.ToString());
        }
    }
}