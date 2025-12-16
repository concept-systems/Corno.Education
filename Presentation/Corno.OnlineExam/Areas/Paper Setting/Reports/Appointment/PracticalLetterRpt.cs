using Corno.Data.ViewModels.Appointment;
using Corno.OnlineExam.Properties;
using Corno.Reports.Base;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Paper_Setting.Interfaces;

namespace Corno.OnlineExam.Areas.Paper_Setting.Reports.Appointment;

public partial class PracticalLetterRpt : BaseReport
{
    #region -- Constructors --
    public PracticalLetterRpt(IEnumerable<ReportModel> dataSource)
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

    public PracticalLetterRpt(Data.Corno.Paper_Setting.Models.Appointment appointment)
    {
        InitializeComponent();

        _appointmentService = Bootstrapper.Get<IAppointmentService>();

        var imageConverter = new ImageConverter();
        pictureBox1.Value = imageConverter.ConvertFrom(Resources.logo1_min);
        pictureBox2.Value = imageConverter.ConvertFrom(Resources.logo2_min);
        pictureBox3.Value = imageConverter.ConvertFrom(Resources.roz_sir_sign);

        var dataSource = _appointmentService.GetReportDataFromAppointment(appointment, false)
            .ToList();

        if (dataSource.Count <= 0)
            return;

        DataSource = dataSource;
    }
    #endregion

    #region -- Data Members --

    private readonly IAppointmentService _appointmentService;
    private readonly IScheduleService _scheduleService;
    private readonly IInstanceService _instanceService;
    private readonly ICollegeService _collegeService;
    private readonly ICourseService _courseService;
    private readonly ICoursePartService _coursePartService;
    private readonly IBranchService _branchService;
    private readonly ISubjectService _subjectService;
    private readonly IStaffService _staffService;

    #endregion
}