using System.Linq;
using Corno.Reports;
using Corno.Reports.Base;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Paper_Setting.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;

namespace Corno.OnlineExam.Areas.Paper_Setting.Reports.Bill;

public partial class NeftRpt : BaseReport
{
    public NeftRpt(Data.Corno.Paper_Setting.Models.Appointment appointment)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        var appointmentService = Bootstrapper.Get<IAppointmentService>();

        var appointments = appointmentService.Get(p =>
                p.InstanceId == appointment.InstanceId && p.FacultyId == appointment.FacultyId &&
                p.CollegeId == appointment.CollegeId && p.CourseId == appointment.CourseId &&
                p.CoursePartId == appointment.CoursePartId &&
                p.CategoryId == appointment.CategoryId && p.SubjectId == appointment.SubjectId &&
                p.AppointmentDetails.Any(d => d.IsPaperSetter &&
                                              d.StaffId == appointment.StaffId),
            p => p).ToList();

        var scheduleService = Bootstrapper.Get<IScheduleService>();
        var schedules = scheduleService.Get(p =>
            p.InstanceId == appointment.InstanceId && p.FacultyId == appointment.FacultyId &&
            p.CollegeId == appointment.CollegeId && p.CourseId == appointment.CourseId, p => p).ToList();

        var dataSource = appointmentService.GetData(appointments, schedules, false)
            .Where(d => d.StaffId == appointment.StaffId).ToList();

        dataSource.ForEach(d => d.InWords = TelerikReportHelper.GetAmountInWords(
            (d.BillModel.Ta ?? 0) + (d.BillModel.Da ?? 0) + (d.BillModel.Travel ?? 0)));

        if (dataSource.Count <= 0)
            return;

        var first = dataSource.FirstOrDefault();
        txtAmount.Value = first?.BillModel.TotalFee.ToString();
        txtAmountInWords.Value = TelerikReportHelper.GetAmountInWords(first?.BillModel.TotalFee ?? 0);

        table3.DataSource = dataSource;
    }
}