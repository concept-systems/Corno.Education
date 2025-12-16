using System;
using System.Data;
using System.Data.SqlClient;
using Corno.Data.Helpers;
using Corno.Globals;
using Corno.Reports.Base;
using Telerik.Reporting;
using Telerik.Reporting.Processing;

namespace Corno.Reports.Revaluation;

public partial class RevaluationChallanRpt : BaseReport
{
    public RevaluationChallanRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    public RevaluationChallanRpt(int instanceId, string prnNo)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        sdsRevaluationFees.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsRevaluationFees.SelectCommand = sdsRevaluationFees.SelectCommand.Replace("@InstanceID", instanceId.ToString());

        sdsChildFees.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsChildFees.SelectCommand = sdsChildFees.SelectCommand.Replace("@InstanceID", instanceId.ToString());

        ReportParameters["PRNNo"].Value = prnNo;

        DataSource = null;
    }

    private DataTable GetDataTable(SqlDataSource sds, string parameter)
    {
        using var conn = new SqlConnection(GlobalVariables.ConnectionStringExamServer);

        // Create a connection to the database        
        //   var conn = new SqlConnection(sds.ConnectionString);
        // Create a command to extract the required data and assign it the connection string
        var cmd = new SqlCommand(sds.SelectCommand.Replace("@PRNNo", "'" + parameter + "'"), conn)
        {
            CommandType = CommandType.Text
        };
        // Create a DataAdapter to run the command and fill the DataTable
        var da = new SqlDataAdapter {SelectCommand = cmd};
        var dataTable = new DataTable();
        da.Fill(dataTable);
        conn.Close();

        return dataTable;
    }

    private void RevaluationChallanRpt_NeedDataSource(object sender, EventArgs e)
    {
        try
        {
            var report = (Telerik.Reporting.Processing.Report) sender;

            var dataTableRevaluation = GetDataTable(sdsRevaluationFees, report.Parameters["PRNNo"].Value.ToString());

            var dataTableChild = GetDataTable(sdsChildFees, report.Parameters["PRNNo"].Value.ToString());
            switch (dataTableRevaluation.Rows.Count)
            {
                case <= 0 when dataTableChild.Rows.Count <= 0:
                    return;
                case <= 0:
                    dataTableRevaluation.ImportRow(dataTableChild.Rows[0]);
                    break;
                default:
                {
                    if (dataTableChild.Rows.Count > 0)
                    {
                        var verificationFee = dataTableRevaluation.Rows[0]["VerificationFee"]?.ToInt() +
                                              dataTableChild.Rows[0]["VerificationFee"]?.ToInt();
                        dataTableRevaluation.Rows[0]["VerificationFee"] = verificationFee;
                        dataTableRevaluation.Rows[0]["TotalFee"] = Convert.ToInt32(dataTableRevaluation.Rows[0]["RevaluationFee"]) +
                                                                   Convert.ToInt32(dataTableRevaluation.Rows[0]["VerificationFee"]);
                    }

                    break;
                }
            }

            for (var index = 0; index < 3; index++)
                dataTableRevaluation.ImportRow(dataTableRevaluation.Rows[0]);

            if (dataTableRevaluation.Rows.Count == 4)
            {
                dataTableRevaluation.Rows[0]["Copy"] = "Bank Copy";
                dataTableRevaluation.Rows[1]["Copy"] = "College Copy";
                dataTableRevaluation.Rows[2]["Copy"] = "University Copy";
                dataTableRevaluation.Rows[3]["Copy"] = "Student Copy";
            }

            report.DataSource = dataTableRevaluation;
        }
        catch (Exception exception)
        {
            Console.WriteLine(exception);
            throw;
        }
    }

    private void RevaluationChallanRpt_Error(object sender, ErrorEventArgs eventArgs)
    {
        var procEl = (ProcessingElement) sender;
        procEl.Exception = null;
    }
}