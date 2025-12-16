namespace ReportLibrary.Exam
{
    using System;
    using Globals.Constants;
    using System.Web;
    using Globals;

    /// <summary>
    /// Summary description for GainedMarksRpt.
    /// </summary>
    public partial class GainedMarksRpt : Telerik.Reporting.Report
    {
        public GainedMarksRpt()
        {
            InitializeComponent();

        }
        public GainedMarksRpt(int instanceID)
        {
            InitializeComponent();
            sdsCollege.ConnectionString = GlobalVariables.ConnectionString_ExamServer;
            sdsCourse.ConnectionString = GlobalVariables.ConnectionString_ExamServer;
            sdsCoursePart.ConnectionString = GlobalVariables.ConnectionString_ExamServer;

            sdsGainedMarks.ConnectionString = sdsGainedMarks.ConnectionString;
            sdsGainedMarks.SelectCommand = sdsGainedMarks.SelectCommand.Replace("@InstanceID", instanceID.ToString());
        }
    }
}