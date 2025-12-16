using System;
using System.Linq;
using Corno.Reports.Base;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Helper;
using Telerik.Reporting;

namespace Corno.Reports.Exam;

public partial class ExamChallanRpt : BaseReport
{
    #region -- Data Members --
    private readonly int _instanceId;
    #endregion

    public ExamChallanRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    public ExamChallanRpt(int instanceId, string prnNo)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        ReportParameters["PRNNo"].Value = prnNo;
        _instanceId = instanceId;

        DataSource = null;
    }

    private object GetChallan(string prn, string copy, string header, string footer)
    {
        var examService = Bootstrapper.Get<ICoreService>();
        if (null == examService) return null;

        var challan = (from appTemp in examService.Tbl_APP_TEMP_Repository.Get(a => a.Num_FK_INST_NO == _instanceId &&
                a.Chr_APP_PRN_NO == prn)
            join studentInfo in examService.Tbl_STUDENT_INFO_Repository.Get() on
                appTemp.Chr_APP_PRN_NO equals studentInfo.Chr_PK_PRN_NO into defaultStudentInfo
            from studentInfo in defaultStudentInfo.DefaultIfEmpty()
            join college in examService.TBL_COLLEGE_MSTRRepository.Get()
                on appTemp.Num_FK_COLLEGE_CD equals college.Num_PK_COLLEGE_CD into defaultCollege
            from college in defaultCollege.DefaultIfEmpty()
                       join coursePart in examService.Tbl_COURSE_PART_MSTR_Repository.Get()
                on appTemp.Num_FK_COPRT_NO equals coursePart.Num_PK_COPRT_NO into defaultCoursePart
            from coursePart in defaultCoursePart
            join course in examService.Tbl_COURSE_MSTR_Repository.Get()
                on coursePart.Num_FK_CO_CD equals course.Num_PK_CO_CD into defaultCourse
            from course in defaultCourse.DefaultIfEmpty()
                       join branch in examService.Tbl_BRANCH_MSTR_Repository.Get()
                on appTemp.Num_FK_BR_CD equals branch.Num_PK_BR_CD into defaultBranch
            from branch in defaultBranch.DefaultIfEmpty()
            join instance in examService.Tbl_SYS_INST_Repository.Get()
                on appTemp.Num_FK_INST_NO equals instance.Num_PK_INST_SRNO into defaultInstance
            from instance in defaultInstance.DefaultIfEmpty()
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
                CourseName = course.Var_CO_SHRT_NM,
                CoursePartShortName = coursePart.Var_COPRT_SHRT_NM,
                BranchShortName = branch.Var_BR_SHRT_NM,
                BankAccountNo = college.Num_BankACNo,
                BankBranchCode = college.Chr_BankBranch_Code,
                Copy = copy,
                Header = header,
                Footer = footer,
                InstanceName = instance.Var_INST_REM
            });

        return challan.Count() switch
        {
            <= 0 => throw new Exception("Zero Challan available."),
            > 1 => throw new Exception("More than 1 Challan available."),
            _ => challan.FirstOrDefault()
        };
    }

    private void ExamChallanRpt_NeedDataSource(object sender, EventArgs e)
    {
        var report = (Telerik.Reporting.Processing.Report) sender;
        var prn = report.Parameters["PRNNo"].Value.ToString();

        var list = new System.Collections.Generic.List<object>(4)
        {
            GetChallan(prn, "Bank Copy",
                "For Online Payment Visit :" + System.Environment.NewLine + "www.bharatividyapeethfees.com",
                "For Online Payment Visit : " + System.Environment.NewLine + "www.bharatividyapeethfees.com"),
            GetChallan(prn, "College Copy",
                "Attach the receipt of the online payment to this challan and submit to college",
                "Attach the receipt of the online payment to this challan and submit to college"),
            GetChallan(prn, "University Copy", "", ""),
            GetChallan(prn, "Student Copy", "", "")
        };

        report.DataSource = list;
    }

    private void ExamChallanRpt_Error(object sender, ErrorEventArgs eventArgs)
    {
        var procEl = (Telerik.Reporting.Processing.ProcessingElement) sender;
        procEl.Exception = null;
    }
}