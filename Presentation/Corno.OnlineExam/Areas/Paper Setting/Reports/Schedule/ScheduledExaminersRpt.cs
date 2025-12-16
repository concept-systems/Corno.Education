using Corno.Reports.Base;
using MoreLinq;
using System.Linq;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Paper_Setting.Interfaces;

namespace Corno.OnlineExam.Areas.Paper_Setting.Reports.Schedule;

public partial class ScheduledExaminersRpt : BaseReport
{
    public ScheduledExaminersRpt(Data.Corno.Paper_Setting.Models.Schedule schedule)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        var appointmentService = Bootstrapper.Get<IAppointmentService>();
        var appointment = new Data.Corno.Paper_Setting.Models.Appointment
        {
            InstanceId = schedule.InstanceId,
            FacultyId = schedule.FacultyId,
            CollegeId = schedule.CollegeId,
            CourseId = schedule.CourseId,
            CoursePartId = schedule.CoursePartId,
            BranchId = schedule.BranchId,
            CategoryId = schedule.CategoryId
        };

        var dataSource = appointmentService.GetReportDataFromAppointment(appointment, false)
            .ToList();
        dataSource = dataSource
        .DistinctBy(x => new { CategoryId = x.Category.Id, SubjectId = x.Subject.Id, x.StaffId })
        .OrderBy(x => x.Subject.Id)
        .ToList();

        if (dataSource.Count <= 0)
            return;

        DataSource = dataSource;
    }
}