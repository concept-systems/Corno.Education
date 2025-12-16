using Corno.Globals;
using System;
using System.Linq;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Helper;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Report = Telerik.Reporting.Report;

namespace Corno.Reports.Convocation;

public partial class ConvocationChallanRpt : Report
{
    #region -- Data Members --
    private readonly int _instanceId;
    #endregion

    public ConvocationChallanRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    public ConvocationChallanRpt(int instanceId, string prnNo)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        sdsFees.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsFees.SelectCommand = sdsFees.SelectCommand.Replace("@InstanceID", instanceId.ToString());
        ReportParameters["PRNNo"].Value = prnNo;
        _instanceId = instanceId;

        DataSource = null;
    }

    private object GetChallan(string prn, string copy)
    {
        var coreService = Bootstrapper.Get<ICoreService>();
        if (null == coreService) return null;

        //var concocations = coreService.Tbl_STUDENT_CONVO_Repository.Get(a => a.Chr_FK_PRN_NO == prn, p => p).ToList();
        var challanList = (from convocation in coreService.Tbl_STUDENT_CONVO_Repository.Get(a => a.Chr_FK_PRN_NO == prn)
                           join studentInfo in coreService.Tbl_STUDENT_INFO_Repository.Get() on
                               convocation.Chr_FK_PRN_NO equals studentInfo.Chr_PK_PRN_NO
                           join college in coreService.TBL_COLLEGE_MSTRRepository.Get() on
                               convocation.Num_FK_COLLEGE_CD equals college.Num_PK_COLLEGE_CD
                           join course in coreService.Tbl_COURSE_MSTR_Repository.Get()
                               on convocation.Num_FK_CO_CD equals course.Num_PK_CO_CD into defaultCourse
                           from course in defaultCourse.DefaultIfEmpty()
                           join instance in coreService.Tbl_SYS_INST_Repository.Get() on
                               convocation.Num_FK_INST_NO equals instance.Num_PK_INST_SRNO into defaultInstance
                           from instance in defaultInstance.DefaultIfEmpty()
                           select new
                           {
                               PRNNo = convocation.Chr_FK_PRN_NO,
                               StudentName = studentInfo.Var_ST_NM,
                               ConvocationFee = convocation.Con_ST_FEES_AMT,
                               TotalFee = convocation.Con_ST_FEES_AMT,
                               CollegeName = college.Var_CL_COLLEGE_NM1,
                               ShortCollegeName = college.Var_CL_SHRT_NM,
                               CourseName = course.Var_CO_SHRT_NM,
                               BankAccountNo = college.Num_BankACNo,
                               BankBranchCode = college.Chr_BankBranch_Code,
                               Copy = copy,
                               InstanceName = instance.Var_INST_REM,
                           }).ToList();

        return challanList.Count switch
        {
            <= 0 => throw new Exception("Zero Challan available."),
            > 1 => throw new Exception("More than 1 Challan available."),
            _ => challanList.FirstOrDefault()
        };
    }

    private void ConvocationChallanRpt_NeedDataSource(object sender, EventArgs e)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;
        var prn = report.Parameters["PRNNo"].Value.ToString();

        var list = new System.Collections.Generic.List<object>(4)
        {
            GetChallan(prn, "Bank Copy"),
            GetChallan(prn, "College Copy"),
            GetChallan(prn, "University Copy"),
            GetChallan(prn, "Student Copy")
        };

        report.DataSource = list;

        //using (var conn = new SqlConnection(GlobalVariables.ConnectionStringExamServer))
        //{
        //    //var conn = new SqlConnection(sdsFees.ConnectionString);
        //    // Create a command to extract the required data and assign it the connection string
        //    var cmd = new SqlCommand(
        //            sdsFees.SelectCommand.Replace("@PRNNo", "'" + report.Parameters["PRNNo"].Value + "'"), conn)
        //        {
        //            CommandType = CommandType.Text
        //        };
        //    // Create a DataAdapter to run the command and fill the DataTable
        //    var da = new SqlDataAdapter {SelectCommand = cmd};

        //    var dataTable = new DataTable();

        //    da.Fill(dataTable);

        //    conn.Close();

        //    if (dataTable.Rows.Count <= 0) return;

        //    for (var index = 0; index < 3; index++)
        //        dataTable.ImportRow(dataTable.Rows[0]);


        //    if (dataTable.Rows.Count == 4)
        //    {
        //        dataTable.Rows[0]["Copy"] = "Bank Copy";
        //        dataTable.Rows[1]["Copy"] = "College Copy";
        //        dataTable.Rows[2]["Copy"] = "University Copy";
        //        dataTable.Rows[3]["Copy"] = "Student Copy";
        //    }

        //    report.DataSource = dataTable;
        //}
    }

    private void ConvocationChallanRpt_Error(object sender, ErrorEventArgs eventArgs)
    {
        var procEl = (ProcessingElement)sender;
        procEl.Exception = null;
    }
}