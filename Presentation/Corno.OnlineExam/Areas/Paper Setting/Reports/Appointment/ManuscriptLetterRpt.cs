using Corno.Data.ViewModels.Appointment;
using Corno.OnlineExam.Properties;
using Corno.Reports.Base;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Corno.Data.Helpers;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Paper_Setting.Interfaces;

namespace Corno.OnlineExam.Areas.Paper_Setting.Reports.Appointment
{
    public partial class ManuscriptLetterRpt : BaseReport
    {
        #region -- Constructors --
        public ManuscriptLetterRpt(IEnumerable<ReportModel> dataSource)
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

        public ManuscriptLetterRpt(Data.Corno.Paper_Setting.Models.Appointment appointment)
        {
            InitializeComponent();

            _appointmentService = Bootstrapper.Get<IAppointmentService>();

            var imageConverter = new ImageConverter();
            pictureBox1.Value = imageConverter.ConvertFrom(Resources.logo1_min);
            pictureBox2.Value = imageConverter.ConvertFrom(Resources.logo2_min);
            pictureBox3.Value = imageConverter.ConvertFrom(Resources.roz_sir_sign);

            _appointment = appointment;

            var dataSource = _appointmentService.GetReportDataFromAppointment(_appointment, false)
                .ToList();

            if (dataSource.Count <= 0)
                return;

            DataSource = dataSource;
        }
        #endregion

        #region -- Data Members --

        private readonly Data.Corno.Paper_Setting.Models.Appointment _appointment;

        private readonly IAppointmentService _appointmentService;
        #endregion

        #region -- Events --
        private void ManuscriptLetterRpt_NeedDataSource(object sender, System.EventArgs e)
        {
            var report = (Telerik.Reporting.Processing.Report)sender;

            report.DataSource = _appointmentService.GetData(_appointment, false);
        }
        #endregion
    }
}