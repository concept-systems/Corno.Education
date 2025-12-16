using System.Linq;
using Corno.Globals.Constants;
using Corno.Reports.Base;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Paper_Setting.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;
using MoreLinq;

namespace Corno.OnlineExam.Areas.Paper_Setting.Reports.Appointment;

public partial class AppointedExaminersRpt : BaseReport
{
    #region -- Constructors --
    public AppointedExaminersRpt(Data.Corno.Paper_Setting.Models.Appointment appointment)
    {
        InitializeComponent();

        ReportParameters[ModelConstants.BosId].Value = 1;

        var appointmentService = Bootstrapper.Get<IAppointmentService>();
        var dataSource = appointmentService.GetReportDataFromAppointment(appointment, false)
            .DistinctBy(d => new
            {
                CourseId = d.Course.Id, CoursePartId = d.CoursePart.Code, CategoryId = d.Category.Id,
                SubjectId = d.Subject.Id, d.StaffId
            }).ToList();

        if (dataSource.Count <= 0)
            return;

        DataSource = dataSource;
    }
    #endregion
}