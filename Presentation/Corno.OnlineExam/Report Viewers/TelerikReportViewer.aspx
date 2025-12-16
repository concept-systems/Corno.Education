<%@ Page Language="C#" AutoEventWireup="true" CodeBehind="TelerikReportViewer.aspx.cs" Inherits="Corno.OnlineExam.Report_Viewers.TelerikReportViewer" %>

<%----%>

<!DOCTYPE html>

<%@ Register Assembly="Telerik.ReportViewer.WebForms, Culture=neutral, PublicKeyToken=a9d7983dfcc261be"
    Namespace="Telerik.ReportViewer.WebForms" TagPrefix="telerik" %>

<!DOCTYPE html>

<html xmlns="http://www.w3.org/1999/xhtml">

<head runat="server">
    <title></title>

    <%--
    <link href="~/Content/plugins/fancybox/jquery.fancybox.css" rel="stylesheet">--%>
    <%--<link href="~/Content/plugins/bootstrap/bootstrap.min.css" rel="stylesheet">
    <link href="~/Content/css/font-awesome.min.css" rel="stylesheet">
    <link href="~/Content/kendo/2023.1.117/kendo.default-v2.min.css" rel="stylesheet" type="text/css" />--%>
</head>
<body>
    <%--<button class="w-100 k-button k-button-sm k-rounded-md k-button-outline k-button-outline-tertiary float-end" id="back_to_list" data-role="button" type="button" role="button"
        aria-disabled="false" tabindex="0" onclick="javascript:history.back()">
        <span class="k-icon k-i-arrow-left k-button-icon"></span>
        <span class="k-button-text">Back</span>
    </button>--%>
    <%--<button onclick="javascript:history.back()" class="k-button k-button-md k-rounded-md k-button-outline k-button-outline-tertiary">
        <i class="icon-angle-left"></i>
        Back to previous page
    </button>
    <a href="javascript:history.back()" class="btn btn-primary">
        <i class="fa fa-arrow-left"></i>
        Back to previous page
    </a>--%>
    <form id="form1" runat="server">
        <%--<button onclick="javascript:history.back()" class="k-button k-button-md k-rounded-md k-button-outline k-button-outline-tertiary">
            <i class="icon-angle-left"></i>
            Back to previous page
        </button>--%>
        <div>
            <telerik:ReportViewer ID="ReportViewer1" runat="server"
                Width="100%" Height="1400px" ShowZoomSelect="True"
                ViewMode="PrintPreview">
            </telerik:ReportViewer>
        </div>
    </form>
</body>
</html>

<script type="text/javascript">

    <%--// Attach itemClick event handler when the document is ready
    $(document).ready(function () {
        var reportViewer = $find("<%= ReportViewer1.ClientID %>");
        reportViewer.addHandler("itemClick", handleItemClick);
    });

    // Function to handle itemClick event
    function handleItemClick(args) {
        alert("item clicked")

        // Get the clicked report item
        var clickedItem = args.get_item();

        // Check if the clicked item is a textbox
        if (clickedItem.get_type() === Telerik.Reporting.ReportItemTypes.TextBox) {
            // Extract the value of the textbox
            var textBoxValue = clickedItem.get_value();

            // Send the value to the controller using AJAX
            $.ajax({
                type: "POST",
                url: "YourController/YourAction",
                data: JSON.stringify({ textBoxValue: textBoxValue }),
                contentType: "application/json; charset=utf-8",
                dataType: "json",
                success: function (response) {
                    // Handle the response from the controller if needed
                    alert("Value sent to controller successfully!");
                },
                error: function (xhr, status, error) {
                    // Handle errors if any
                    console.error(error);
                }
            });
        }
    }--%>

    <%--ReportViewer1.prototype.PrintReport = function () {
        // Print directly to default print
        this.PrintAs("Default");

        //switch (this.defaultPrintFormat) {
        //case "Default":
        //    this.DefaultPrint();
        //    break;
        //case "PDF":
        //    this.PrintAs("PDF");
        //    previewFrame = document.getElementById(this.previewFrameID);
        //    previewFrame.onload = function () { previewFrame.contentDocument.execCommand("print", true, null); }
        //    break;
        //}
    };--%>

</script>
