using Corno.Globals;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Report = Telerik.Reporting.Report;

namespace Corno.Reports.Revaluation
{
    /// <summary>
    ///     Summary description for CollegewiseRegistration.
    /// </summary>
    public partial class RevaluationRpt : Report
    {
        public RevaluationRpt()
        {
            // Required for telerik Reporting designer support
            InitializeComponent();
        }

        public RevaluationRpt(int instanceId, int collegeId)
        {
            // Required for telerik Reporting designer support
            InitializeComponent();

            sdsCollege.ConnectionString = GlobalVariables.ConnectionStringExamServer;
            if (collegeId > 0)
                sdsCollege.SelectCommand += " And Num_PK_COLLEGE_CD = " + collegeId;
            sdsCourse.ConnectionString = GlobalVariables.ConnectionStringExamServer;
            sdsCoursePart.ConnectionString = GlobalVariables.ConnectionStringExamServer;
            sdsBranch.ConnectionString = GlobalVariables.ConnectionStringExamServer;
            //sdsSubject.ConnectionString = GlobalVariables.ConnectionString_ExamServer;
            sdsRevaluationRpt.ConnectionString = GlobalVariables.ConnectionStringExamServer;

            sdsRevaluationRpt.SelectCommand = sdsRevaluationRpt.SelectCommand.Replace("@InstanceID",
                instanceId.ToString());
        }

        private void RevaluationRpt_Error(object sender, ErrorEventArgs eventArgs)
        {
            var procEl = (ProcessingElement)sender;
            procEl.Exception = null;
        }
    }
}