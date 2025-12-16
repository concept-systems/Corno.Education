using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Corno.Data.ViewModels.Appointment;
using Corno.OnlineExam.Properties;
using Corno.Reports.Base;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Paper_Setting.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;

namespace Corno.OnlineExam.Areas.Paper_Setting.Reports.Appointment;

public partial class TheoryLetterRpt : BaseReport
{
    #region -- Constructors --
    public TheoryLetterRpt(IEnumerable<ReportModel> dataSource)
    {
        InitializeComponent();

        var imageConverter = new ImageConverter();
        pictureBox1.Value = imageConverter.ConvertFrom(Resources.logo1_min);
        pictureBox2.Value = imageConverter.ConvertFrom(Resources.logo2_min);
        pictureBox3.Value = imageConverter.ConvertFrom(Resources.roz_sir_sign);

        if (dataSource.ToList().Count <= 0)
            return;

        DataSource = dataSource;
    }

    public TheoryLetterRpt(Data.Corno.Paper_Setting.Models.Appointment appointment)
    {
        InitializeComponent();

        var appointmentService = Bootstrapper.Get<IAppointmentService>();
        
        var imageConverter = new ImageConverter();
        pictureBox1.Value = imageConverter.ConvertFrom(Resources.logo1_min);
        pictureBox2.Value = imageConverter.ConvertFrom(Resources.logo2_min);
        pictureBox3.Value = imageConverter.ConvertFrom(Resources.roz_sir_sign);

        var dataSource = appointmentService.GetReportDataFromAppointment(appointment, false)
            .ToList();

        if (dataSource.Count <= 0)
            return;

        DataSource = dataSource;
    }
    #endregion
}