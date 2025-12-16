using Corno.Reports.Base;
using Corno.Services.Corno.Question_Bank.Interfaces;
using System.Linq;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Paper_Setting.Interfaces;

namespace Corno.OnlineExam.Areas.Paper_Setting.Reports.Appointment
{
    public partial class ManuscriptExaminersRpt : BaseReport
    {
        #region -- Constructors --

        public ManuscriptExaminersRpt(Data.Corno.Paper_Setting.Models.Appointment appointment)
        {
            InitializeComponent();

            var appointmentService = Bootstrapper.Get<IAppointmentService>();
            var dataSource = appointmentService.GetReportDataFromAppointment(appointment, false)
                .ToList();

            if (dataSource.Count <= 0)
                return;

            DataSource = dataSource;
        }
        #endregion
    }
}