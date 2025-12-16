using System;
using System.Linq;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Helper;
using Telerik.Reporting;

namespace Corno.Reports.Environment;

public partial class ReceiptRpt : Report
{
    #region -- Data Members --
    private readonly int _instanceId;
    #endregion

    public ReceiptRpt(int instanceId, string prn)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        _instanceId = instanceId;

        DataSource = GetChallan(prn);
    }

    private object GetChallan(string prn)
    {
        var examService = Bootstrapper.Get<ICoreService>();
        if (null == examService) return null;

        var challan = (from environment in examService.TBl_STUDENT_ENV_STUDIES_Repository.Get(a => a.Chr_FK_PRN_NO == prn)
                       join studentInfo in examService.Tbl_STUDENT_INFO_Repository.Get() on
                           environment.Chr_FK_PRN_NO equals studentInfo.Chr_PK_PRN_NO
                       join college in examService.TBL_COLLEGE_MSTRRepository.Get() on
                           environment.Num_FK_COL_CD equals college.Num_PK_COLLEGE_CD
                       join course in examService.Tbl_COURSE_MSTR_Repository.Get() on
                           environment.Num_FK_CO_CD equals course.Num_PK_CO_CD into defaultCourse
                       from course in defaultCourse.DefaultIfEmpty()
                       join coursePart in examService.Tbl_COURSE_PART_MSTR_Repository.Get() on
                           environment.Num_FK_COPRT_NO equals coursePart.Num_PK_COPRT_NO into defaultCoursePart
                       from coursePart in defaultCoursePart.DefaultIfEmpty()
                       join instance in examService.Tbl_SYS_INST_Repository.Get() on
                           environment.Num_FK_INST_NO equals instance.Num_PK_INST_SRNO into defaultInstance
                       from instance in defaultInstance.DefaultIfEmpty()
                       select new
                       {
                           PaymentDate = environment.PaymentDate ?? environment.Dtm_DTE_CR,
                           PRNNo = environment.Chr_FK_PRN_NO,
                           StudentName = studentInfo.Var_ST_NM,
                           EnvironmentFee = environment.Num_EnviFee,
                           LateFee = environment.Num_EnviLateFee,
                           SuperlateFee = environment.Num_EnviSuperLateFee,
                           OtherFee = environment.Num_EnviOtherFee,
                           TotalFee = environment.Num_EnviTotalFee,
                           CollegeName = college.Var_CL_COLLEGE_NM1,
                           ShortCollegeName = college.Var_CL_SHRT_NM,
                           CourseName = course.Var_CO_SHRT_NM,
                           CoursePartShortName = coursePart.Var_COPRT_SHRT_NM,
                           BankAccountNo = college.Num_BankACNo,
                           BankBranchCode = college.Chr_BankBranch_Code,
                           InstanceName = instance.Var_INST_REM,
                           TransactionId = environment.Chr_Transaction_Id
                       }).ToList();

        return challan.Count switch
        {
            <= 0 => throw new Exception("Zero Challan available."),
            > 1 => throw new Exception("More than 1 Challan available."),
            _ => challan.FirstOrDefault()
        };
    }

    private void ReceiptRpt_Error(object sender, ErrorEventArgs eventArgs)
    {
        var procEl = (Telerik.Reporting.Processing.ProcessingElement)sender;
        procEl.Exception = null;
    }
}