using Corno.Data.Corno.Question_Bank;
using Corno.Data.Helpers;
using Corno.Reports.Base;
using Corno.Services.Corno.Question_Bank.Interfaces;
using System;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Paper_Setting.Interfaces;
using LinqKit;

namespace Corno.OnlineExam.Areas.Paper_Setting.Reports.Bill;

public partial class UnattendedExaminersRpt : BaseReport
{
    #region -- Constructors --
    public UnattendedExaminersRpt(Data.Corno.Paper_Setting.Models.Appointment appointment)
    {
        InitializeComponent();

        _appointment = appointment;
    }
    #endregion

    #region -- Data Members --
    private readonly Data.Corno.Paper_Setting.Models.Appointment _appointment;
    #endregion

    #region -- Events --
    private void UnattendedExaminersRpt_NeedDataSource(object sender, System.EventArgs e)
    {
        if (sender is not Telerik.Reporting.Processing.Report report) return;

        var fromDate = report.Parameters["FromDate"].Value.ToDateTime();
        var toDate = report.Parameters["ToDate"].Value.ToDateTime();

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
        if ((_appointment.CategoryId ?? 0) > 0)
            schedulePredicate = schedulePredicate.And(s => s.CategoryId == _appointment.CategoryId);
        var schedules = scheduleService.Get(schedulePredicate, p => p).ToList();
        /*var schedules = scheduleService.Get(p =>
            p.InstanceId == _appointment.InstanceId && p.ScheduleDetails.Any(d =>
                DbFunctions.TruncateTime(d.FromDate) >= DbFunctions.TruncateTime(fromDate) &&
                DbFunctions.TruncateTime(d.FromDate) <= DbFunctions.TruncateTime(toDate)), p => p).ToList();*/

        var facultyIds = schedules.Select(ad => ad.FacultyId).Distinct().ToList();
        var collegeIds = schedules.Select(ad => ad.CollegeId).Distinct().ToList();
        var courseIds = schedules.Select(ad => ad.CourseId).Distinct().ToList();
        var subjectIds = schedules.SelectMany(p => p.ScheduleDetails
            .Where(d => d.FromDate?.Date >= fromDate.Date &&
                        d.FromDate?.Date <= toDate.Date), (_, d) => d.SubjectId).ToList();

        var appointmentService = Bootstrapper.Get<IAppointmentService>();
        var appointments = appointmentService.Get(p =>
                p.InstanceId == _appointment.InstanceId && facultyIds.Contains(p.FacultyId) &&
                collegeIds.Contains(p.CollegeId) && courseIds.Contains(p.CourseId) &&
                subjectIds.Contains(p.SubjectId) &&
                p.AppointmentDetails.Any(d => d.IsPaperSetter),
            p => p).ToList();

        /*var schedules1 = scheduleService.Get(p =>
            p.InstanceId == _instanceId && facultyIds.Contains(p.FacultyId) &&
            collegeIds.Contains(p.CollegeId) && courseIds.Contains(p.CourseId), p => p).ToList();*/

        var dataSource = appointmentService.GetData(appointments, schedules, false).ToList();

        if (dataSource.Count <= 0)
            return;

        dataSource = dataSource.Where(d => (d.BillModel.BillNo ?? 0) <= 0).ToList();

        report.DataSource = dataSource.OrderBy(d => d.BillModel.BillNo);
    }
    #endregion
}