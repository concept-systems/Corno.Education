using System;
using System.Web;
using System.Web.UI;
using Corno.Data.Helpers;
using CrystalDecisions.CrystalReports.Engine;
using CrystalDecisions.Web;

namespace Corno.OnlineExam.Report_Viewers;

public partial class CrystalReportViewer : Page
{
    protected void Page_Load(object sender, EventArgs e)
    {
        /*if (null == HttpContext.Current.Session["CrystalReport"]) 
            return;

        CrystalReportViewer1.ReportSource = HttpContext.Current.Session["CrystalReport"];
        CrystalReportViewer1.ToolPanelView = HttpContext.Current.Session["HideGroupTree"].ToBoolean() ? ToolPanelViewType.None : ToolPanelViewType.GroupTree;*/
    }

    protected void Page_Init(object sender, EventArgs e)
    {
        if (HttpContext.Current.Session["CrystalReport"] == null) 
            return;

        CrystalReportViewer1.ReportSource = HttpContext.Current.Session["CrystalReport"];
        CrystalReportViewer1.ToolPanelView = HttpContext.Current.Session["HideGroupTree"].ToBoolean()
            ? ToolPanelViewType.None
            : ToolPanelViewType.GroupTree;
    }

    /*protected void Page_Unload(object sender, EventArgs e)
    {
        var report = HttpContext.Current.Session["CrystalReport"] as ReportDocument;
        if (report == null)
            return;

        // Check if it's an export or print request
        var crystalParam = Request.Params["CrystalExport"] ?? Request.Params["CrystalPrint"];
        var isExportOrPrint = !string.IsNullOrEmpty(crystalParam);

        // Dispose only if it's not export/print
        if (IsPostBack || isExportOrPrint) return;

        report.Close();
        report.Dispose();
        HttpContext.Current.Session.Remove("CrystalReport");
    }*/


    protected void Page_Unload(object sender, EventArgs e)
    {
        /*var report = HttpContext.Current.Session["CrystalReport"] as ReportDocument;
        if (report == null) 
            return;

        report.Close();
        report.Dispose();*/
    }

    /*protected void CrystalReportViewer1_Unload(object sender, EventArgs e)
    {
        if (HttpContext.Current.Session["CrystalReport"] is not ReportDocument report) 
            return;
        report.Close();
        report.Dispose();
    }*/
}