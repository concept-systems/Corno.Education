using Corno.Reports.Base;
using Corno.Services.Corno.Question_Bank.Interfaces;
using MoreLinq;
using System.Linq;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Paper_Setting.Interfaces;

namespace Corno.OnlineExam.Areas.Paper_Setting.Reports.Schedule;

public partial class ScheduleRpt : BaseReport
{
    #region -- Constructors --
    public ScheduleRpt(Data.Corno.Paper_Setting.Models.Schedule schedule)
    {
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
        
        var dataSource = appointmentService.GetReportDataFromSchedule(appointment, false)
            .Where(p => p.MeetingDate is not null)
            .DistinctBy(d => d.Subject.Id);

        if (dataSource.ToList().Count <= 0)
            return;

        DataSource = dataSource;
    }
    #endregion
}