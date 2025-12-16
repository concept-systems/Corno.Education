<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="CrystalReportViewer.aspx.cs" Inherits="Corno.OnlineExam.Report_Viewers.CrystalReportViewer" %>

<%@ Register Assembly="CrystalDecisions.Web, Culture=neutral, PublicKeyToken=692fbea5521e1304" Namespace="CrystalDecisions.Web" TagPrefix="CR" %>


<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">
<head runat="server">
    <title></title>
</head>
<body>
<form id="form1" runat="server">
    <div>
        <asp:ScriptManager ID="ScriptManager1" runat="server">
        </asp:ScriptManager>
        <CR:CrystalReportViewer ID="CrystalReportViewer1" runat="server" 
                                AutoDataBind="true" Visible="true" PrintMode="ActiveX" 
                                ViewStateMode="Enabled"
                                EnableParameterPrompt="false" Width="100%"/>
    </div>
</form>
</body>
</html>