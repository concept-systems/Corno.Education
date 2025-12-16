namespace ReportLibrary.Convocation
{
    using System;
    using System.ComponentModel;
    using System.Drawing;
    using System.Windows.Forms;
    using Telerik.Reporting;
    using Telerik.Reporting.Drawing;

    /// <summary>
    /// Summary description for CertificateList.
    /// </summary>
    public partial class CertificateList : Telerik.Reporting.Report
    {
        public CertificateList()
        {
            InitializeComponent();

            sdsCertificateList.ConnectionString = sdsCertificateList.ConnectionString;
            sdsCertificateList.SelectCommand = sdsCertificateList.SelectCommand.Replace("@CompanyID", "1");
        }
    }
}