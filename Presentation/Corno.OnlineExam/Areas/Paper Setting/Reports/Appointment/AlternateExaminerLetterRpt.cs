using System.Drawing;
using System.Linq;
using Corno.OnlineExam.Properties;
using Corno.Reports.Base;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Paper_Setting.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;

namespace Corno.OnlineExam.Areas.Paper_Setting.Reports.Appointment;

public partial class AlternateExaminerLetterRpt : BaseReport
{
    #region -- Constructors --
    public AlternateExaminerLetterRpt(Data.Corno.Paper_Setting.Models.Appointment appointment)
    {
        InitializeComponent();

        var appointmentService = Bootstrapper.Get<IAppointmentService>();
        var staffService = Bootstrapper.Get<IStaffService>();

        var imageConverter = new ImageConverter();
        pictureBox1.Value = imageConverter.ConvertFrom(Resources.logo1_min);
        pictureBox2.Value = imageConverter.ConvertFrom(Resources.logo2_min);
        pictureBox3.Value = imageConverter.ConvertFrom(Resources.roz_sir_sign);

        var dataSource = appointmentService.GetReportDataFromAppointment(appointment, false).ToList();
        dataSource = dataSource.Where(p => (p.OriginalId ?? 0) > 0).ToList();
        var originalIds = dataSource.Select(p => p.OriginalId).ToList();
        var staffs = staffService.Get(p => originalIds.Contains(p.Id), p => p).ToList();

        dataSource.ForEach(p =>
        {
            var staff = staffs.FirstOrDefault(x => x.Id == p.OriginalId);

            p.OriginalName = $"{staff?.Salutation} {staff?.Name} {(p.IsChairman ?? false ? " (Chairman)" : "")}";
            // TODO : get college address if Address1 is empty
            p.OriginalAddress =
                $"{(string.IsNullOrEmpty(staff?.Address1) ? staff?.Address1 : staff.Address1)}, \n Mob. : {staff?.Mobile} ";
        });

        if (dataSource.Count <= 0)
            return;

        DataSource = dataSource;
    }
    #endregion
}