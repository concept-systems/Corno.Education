using System;
using System.Linq;
using Corno.Services.Bootstrapper;
using Corno.Services.Core;
using Corno.Services.Core.Interfaces;
using Corno.Services.Helper;
using Telerik.Reporting;

namespace Corno.Reports.Exam
{
    public partial class ReceiptRpt : Report
    {
        #region -- Data Members --
        private readonly int _instanceId;
        #endregion

        /*public ReceiptRpt()
        {
            // Required for telerik Reporting designer support
            InitializeComponent();
        }*/

        public ReceiptRpt(int instanceId, string prn)
        {
            // Required for telerik Reporting designer support
            InitializeComponent();

            //ReportParameters["PRNNo"].Value = prn;
            _instanceId = instanceId;

            DataSource = GetChallan(prn, "", "Student Copy", "");
        }

        private object GetChallan(string prn, string copy, string header, string footer)
        {
            var examService = (ICoreService) Bootstrapper.GetService(typeof(CoreService));
            if (null == examService) return null;

            var challan = (from appTemp in examService.Tbl_APP_TEMP_Repository.Get(a => a.Num_FK_INST_NO == _instanceId &&
                                                                                  a.Chr_APP_PRN_NO == prn)
                            join studentInfo in examService.Tbl_STUDENT_INFO_Repository.Get() on
                                 appTemp.Chr_APP_PRN_NO equals studentInfo.Chr_PK_PRN_NO
                            join college in examService.TBL_COLLEGE_MSTRRepository.Get() on
                                 appTemp.Num_FK_COLLEGE_CD equals college.Num_PK_COLLEGE_CD
                            select new
                            {
                                PRNNo = appTemp.Chr_APP_PRN_NO,
                                StudentName = studentInfo.Var_ST_NM,
                                ExamFee = appTemp.Num_ExamFee,
                                CAPFee = appTemp.Num_CAPFee,
                                StatementFee = appTemp.Num_StatementFee,
                                LateFee = appTemp.Num_LateFee,
                                SuperlateFee = appTemp.Num_SuperLateFee,
                                Fine = appTemp.Num_Fine,
                                DissertationFee = appTemp.Num_DissertationFee,
                                BacklogFee = appTemp.Num_BacklogFee,
                                CertificateFee = appTemp.Num_PassingCertificateFee,
                                TotalFee = appTemp.Num_TotalFee,
                                CollegeName = college.Var_CL_COLLEGE_NM1,
                                ShortCollegeName = college.Var_CL_SHRT_NM,
                                CourseName = ExamServerHelper.GetCourseShortNameFromCoursePartId(appTemp.Num_FK_COPRT_NO, examService),
                                CoursePartShortName = ExamServerHelper.GetCoursePartShortName(appTemp.Num_FK_COPRT_NO, examService),
                                BranchShortName = ExamServerHelper.GetBranchShortName(appTemp.Num_FK_BR_CD, examService),
                                BankAccountNo = college.Num_BankACNo,
                                BankBranchCode = college.Chr_BankBranch_Code,
                                Copy = copy,
                                Header = header,
                                Footer = footer,
                                InstanceName = ExamServerHelper.GetInstanceName(_instanceId, examService)
                            }).ToList();
            if (challan.Count <= 0)
                throw new Exception("Zero Challan available.");
            if (challan.Count > 1)
                throw new Exception("More than 1 Challan available.");

            return challan.FirstOrDefault();
        }

        private void ReceiptRpt_NeedDataSource(object sender, EventArgs e)
        {
            var report = (Telerik.Reporting.Processing.Report) sender;
            var prn = report.Parameters["PRNNo"].Value.ToString();

            var list = new System.Collections.Generic.List<object>(4)
            {
                //GetChallan(prn, "Bank Copy",
                //    "For Online Payment Visit :" + System.Environment.NewLine + "www.bharatividyapeethfees.com",
                //    "For Online Payment Visit : " + System.Environment.NewLine + "www.bharatividyapeethfees.com"),
                //GetChallan(prn, "College Copy",
                //    "Attach the receipt of the online payment to this challan and submit to college",
                //    "Attach the receipt of the online payment to this challan and submit to college"),
                //    GetChallan(prn, "University Copy", "", ""),
                    GetChallan(prn, "Student Copy", "", "")
            };

            report.DataSource = list;
        }

        private void ReceiptRpt_Error(object sender, ErrorEventArgs eventArgs)
        {
            var procEl = (Telerik.Reporting.Processing.ProcessingElement) sender;
            procEl.Exception = null;
        }
    }
}