using Corno.Reports.Base;
using Corno.Services.Corno.Question_Bank.Interfaces;
using System.Drawing;
using System.Linq;
using Corno.OnlineExam.Properties;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Paper_Setting.Interfaces;

namespace Corno.OnlineExam.Areas.Paper_Setting.Reports.Bill;

public partial class CertificateRpt : BaseReport
{
    #region -- Constructors --
    public CertificateRpt(Data.Corno.Paper_Setting.Models.Appointment appointment)
    {
        InitializeComponent();

        var imageConverter = new ImageConverter();
        pictureBox1.Value = imageConverter.ConvertFrom(Resources.logo1_min);
        pictureBox2.Value = imageConverter.ConvertFrom(Resources.logo2_min);
        pictureBox4.Value = imageConverter.ConvertFrom(Resources.roz_sir_sign);

        ReportParameters["Tag"].AvailableValues.DataSource = new[] { "Controller of Examinations", "Addl. Controller of Examinations", "Deputy Registrar", "Assistant Registrar" };
        ReportParameters["Tag"].AvailableValues.ValueMember = "=Fields.Item";
        ReportParameters["Tag"].Value = "Controller of Examinations";

        _appointment = appointment;
    }
    #endregion

    #region -- Data Members --
    private readonly Data.Corno.Paper_Setting.Models.Appointment _appointment;
    #endregion

    #region -- Event Handlers --
    private void CertificateRpt_NeedDataSource(object sender, System.EventArgs e)
    {
        if (sender is not Telerik.Reporting.Processing.Report report) return;

        var appointmentService = Bootstrapper.Get<IAppointmentService>();
        var dataSource = appointmentService.GetReportDataFromAppointment(_appointment, false)
            .Where(d => d.StaffId == _appointment.StaffId).ToList();

        if (dataSource.Count <= 0)
            return;

        report.DataSource = dataSource;
    }
    #endregion
}
