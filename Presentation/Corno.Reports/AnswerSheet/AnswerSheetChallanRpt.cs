using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using Corno.Logger;
using Corno.Reports.Base;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;
using Telerik.Reporting;

namespace Corno.Reports.AnswerSheet;

public partial class AnswerSheetChallanRpt : BaseReport
{
    #region -- Data Members --
    private readonly int _instanceId;
    #endregion

    public AnswerSheetChallanRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    public AnswerSheetChallanRpt(int instanceId, string prnNo)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        ReportParameters["PRNNo"].Value = prnNo;
        _instanceId = instanceId;

        DataSource = null;
    }

    private object GetChallan(string prn, string copy, string header, string footer)
    {
        var transactionService = Bootstrapper.Get<ICornoService>();
        var examService = Bootstrapper.Get<ICoreService>();
        if (null == transactionService) return null;
        var answerSheets = transactionService.AnswerSheetRepository.Get(a => a.InstanceId == _instanceId &&
                                                                             a.Prn == prn).ToList();
        var challan = (from answerSheet in answerSheets
                       join studentInfo in examService.Tbl_STUDENT_INFO_Repository.Get() on
                answerSheet.Prn equals studentInfo.Chr_PK_PRN_NO
            join college in examService.TBL_COLLEGE_MSTRRepository.Get()
                on answerSheet.CollegeId equals college.Num_PK_COLLEGE_CD into defaultCollege
            from college in defaultCollege.DefaultIfEmpty()
            join coursePart in examService.Tbl_COURSE_PART_MSTR_Repository.Get()
                on answerSheet.CoursePartId equals coursePart.Num_PK_COPRT_NO into defaultCoursePart
            from coursePart in defaultCoursePart
            join course in examService.Tbl_COURSE_MSTR_Repository.Get()
                on answerSheet.CourseId equals course.Num_PK_CO_CD into defaultCourse
            from course in defaultCourse.DefaultIfEmpty()
            join branch in examService.Tbl_BRANCH_MSTR_Repository.Get()
                on answerSheet.BranchId equals branch.Num_PK_BR_CD into defaultBranch
            from branch in defaultBranch.DefaultIfEmpty()
            join instance in examService.Tbl_SYS_INST_Repository.Get()
                on answerSheet.InstanceId equals instance.Num_PK_INST_SRNO into defaultInstance
            from instance in defaultInstance.DefaultIfEmpty()
            select new
            {
                PRNNo = answerSheet.Prn,
                StudentName = studentInfo?.Var_ST_NM,
                answerSheet.TotalFee,
                CollegeName = college?.Var_CL_COLLEGE_NM1,
                ShortCollegeName = college?.Var_CL_SHRT_NM,
                CourseName = course?.Var_CO_SHRT_NM,
                CoursePartShortName = coursePart?.Var_COPRT_SHRT_NM,
                BranchShortName = branch?.Var_BR_SHRT_NM,
                BankAccountNo = college?.Num_BankACNo,
                BankBranchCode = college?.Chr_BankBranch_Code,
                Copy = copy,
                Header = header,
                Footer = footer,
                InstanceName = instance?.Var_INST_REM
            }).ToList();
        if (challan.Count <= 0)
            throw new Exception("Zero Challan available.");
        if (challan.Count > 1)
            throw new Exception("More than 1 Challan available.");

        return challan.FirstOrDefault();
    }

    private void AnswerSheetChallanRpt_NeedDataSource(object sender, EventArgs e)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;
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

    private void AnswerSheetChallanRpt_Error(object sender, ErrorEventArgs eventArgs)
    {
        LogHandler.LogInfo(eventArgs.Exception);

        var procEl = (Telerik.Reporting.Processing.ProcessingElement)sender;
        procEl.Exception = null;
    }
}