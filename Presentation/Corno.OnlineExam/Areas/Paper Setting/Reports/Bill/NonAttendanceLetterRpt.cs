using Corno.Data.Helpers;
using Corno.OnlineExam.Properties;
using Corno.Reports.Base;
using Corno.Services.Corno.Question_Bank.Interfaces;
using LinqKit;
using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Drawing;
using System.Linq;
using System.Linq.Expressions;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Paper_Setting.Interfaces;

namespace Corno.OnlineExam.Areas.Paper_Setting.Reports.Bill;

public partial class NonAttendanceLetterRpt : BaseReport
{
    public NonAttendanceLetterRpt(Data.Corno.Paper_Setting.Models.Appointment appointment)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        var imageConverter = new ImageConverter();
        pictureBox1.Value = imageConverter.ConvertFrom(Resources.logo1_min);
        pictureBox2.Value = imageConverter.ConvertFrom(Resources.logo2_min);

        _appointment = appointment;
    }

    #region -- Data Members --
    private readonly Data.Corno.Paper_Setting.Models.Appointment _appointment;
    #endregion

    private void NonAttendanceLetterRpt_NeedDataSource(object sender, EventArgs e)
    {
        if (sender is not Telerik.Reporting.Processing.Report report) return;

        var fromDate = report.Parameters["FromDate"].Value.ToDateTime();
        var toDate = report.Parameters["ToDate"].Value.ToDateTime();

        var practicalCategories = new List<int> { 8 };

        var scheduleService = Bootstrapper.Get<IScheduleService>();
        Expression<Func<Data.Corno.Paper_Setting.Models.Schedule, bool>> schedulePredicate = p =>
            p.InstanceId == _appointment.InstanceId && p.ScheduleDetails.Any(d =>
                DbFunctions.TruncateTime(d.FromDate) >= DbFunctions.TruncateTime(fromDate) &&
                DbFunctions.TruncateTime(d.FromDate) <= DbFunctions.TruncateTime(toDate));
        if ((_appointment.FacultyId ?? 0) > 0)
            schedulePredicate = schedulePredicate.And(s => s.FacultyId == _appointment.FacultyId);
        if ((_appointment.CollegeId ?? 0) > 0)
            schedulePredicate = schedulePredicate.And(s => s.CollegeId == _appointment.CollegeId);
        if ((_appointment.CourseId ?? 0) > 0)
            schedulePredicate = schedulePredicate.And(s => s.CourseId == _appointment.CourseId);
        if ((_appointment.CoursePartId ?? 0) > 0)
            schedulePredicate = schedulePredicate.And(s => s.CoursePartId == _appointment.CoursePartId);
        if ((_appointment.BranchId ?? 0) > 0)
            schedulePredicate = schedulePredicate.And(s => s.BranchId == _appointment.BranchId);
        if ((_appointment.SubjectId ?? 0) > 0)
            schedulePredicate = schedulePredicate.And(s => s.ScheduleDetails.Any(d => d.SubjectId == _appointment.SubjectId));
        /*if ((_appointment.CategoryId ?? 0) > 0)
            schedulePredicate = schedulePredicate.And(s => s.CategoryId == _appointment.CategoryId);*/
        schedulePredicate = schedulePredicate.And(s => !practicalCategories.Contains(s.CategoryId ?? 0));
        var schedules = scheduleService.Get(schedulePredicate, p => p).ToList();
        /*var schedules = (scheduleService.GetQuery() as IEnumerable<Data.Corno.Question_Bank.Schedule>);
        schedules = schedules.Where(schedulePredicate.Compile());*/

        var facultyIds = schedules.Select(ad => ad.FacultyId).Distinct().ToList();
        var collegeIds = schedules.Select(ad => ad.CollegeId).Distinct().ToList();
        var courseIds = schedules.Select(ad => ad.CourseId).Distinct().ToList();
        var coursePartIds = schedules.Select(ad => ad.CoursePartId).Distinct().ToList();
        var categories = schedules.Select(ad => ad.CategoryId).Distinct().ToList();
        var subjectIds = schedules.SelectMany(p => p.ScheduleDetails
            .Where(d => d.FromDate?.Date >= fromDate.Date &&
                        d.FromDate?.Date <= toDate.Date), (_, d) => d.SubjectId).ToList();

        var appointmentService = Bootstrapper.Get<IAppointmentService>();

        var appointments = appointmentService.Get(p =>
                p.InstanceId == _appointment.InstanceId && facultyIds.Contains(p.FacultyId) &&
                collegeIds.Contains(p.CollegeId) && courseIds.Contains(p.CourseId) &&
                coursePartIds.Contains(p.CoursePartId) &&
                subjectIds.Contains(p.SubjectId) && categories.Contains(p.CategoryId) &&
            p.AppointmentBillDetails.Count <= 0 && !practicalCategories.Contains(p.CategoryId ?? 0),
            p => p).ToList();

        var dataSource = appointmentService.GetData(appointments, schedules, false).ToList();

        if (dataSource.Count <= 0)
            return;

        dataSource = dataSource.Where(d => d.MeetingDate != null && (d.BillModel.BillNo ?? 0) <= 0).ToList();

        report.DataSource = dataSource.OrderBy(d => d.BillModel.BillNo);
    }
}