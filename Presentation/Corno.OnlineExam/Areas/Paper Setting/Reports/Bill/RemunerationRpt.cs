using System;
using System.Drawing;
using System.Linq;
using Corno.OnlineExam.Properties;
using Corno.Reports;
using Corno.Reports.Base;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Paper_Setting.Interfaces;

namespace Corno.OnlineExam.Areas.Paper_Setting.Reports.Bill;

public partial class RemunerationRpt : BaseReport
{
    public RemunerationRpt(Data.Corno.Paper_Setting.Models.Appointment appointment)
    {
        InitializeComponent();

        var imageConverter = new ImageConverter();
        pictureBox1.Value = imageConverter.ConvertFrom(Resources.logo1_min);
        pictureBox2.Value = imageConverter.ConvertFrom(Resources.logo2_min);

        var appointmentService = Bootstrapper.Get<IAppointmentService>();

        var appointments = appointmentService.Get(p =>
                p.InstanceId == appointment.InstanceId && p.FacultyId == appointment.FacultyId &&
                p.CollegeId == appointment.CollegeId && p.CourseId == appointment.CourseId &&
                p.CoursePartId == appointment.CoursePartId  &&
                p.CategoryId == appointment.CategoryId && p.SubjectId == appointment.SubjectId &&
                p.AppointmentDetails.Any(d => (d.IsPaperSetter) &&
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

        DataSource = dataSource;
    }

    public static int RoundToNearestZero(double? value)
    {
        return (int)Math.Round(value ?? 0, 1);
    }

    public static string GetAmountInWords(double value)
    {
        return TelerikReportHelper.GetAmountInWords(value);
    }
}