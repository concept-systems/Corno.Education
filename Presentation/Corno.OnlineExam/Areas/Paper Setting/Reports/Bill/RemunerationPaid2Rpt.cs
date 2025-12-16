using Corno.Data.Helpers;
using Corno.Data.ViewModels;
using Corno.Data.ViewModels.Appointment;
using Corno.Reports.Base;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Paper_Setting.Interfaces;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Corno.Services.Bootstrapper;
using NPOI.SS.Formula.Functions;

namespace Corno.OnlineExam.Areas.Paper_Setting.Reports.Bill;

public partial class RemunerationPaid2Rpt : BaseReport
{
    #region -- Constructors --
    public RemunerationPaid2Rpt(Data.Corno.Paper_Setting.Models.Appointment appointment)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        _appointment = appointment;
    }
    #endregion

    #region -- Data Members --
    private readonly Data.Corno.Paper_Setting.Models.Appointment _appointment;
    #endregion

    #region -- Events --
    private void RemunerationPaid2Rpt_NeedDataSource(object sender, System.EventArgs e)
    {
        if (!(sender is Telerik.Reporting.Processing.Report report)) return;

        var fromDate = report.Parameters["FromDate"].Value.ToDateTime();
        var toDate = report.Parameters["ToDate"].Value.ToDateTime();

        var appointmentService = Bootstrapper.Get<IAppointmentService>();
        var staffService = Bootstrapper.Get<IStaffService>();
        var instanceService = Bootstrapper.Get<IInstanceService>();

        // Update Name flags
        var ifsFlags = new List<MasterViewModel>
        {
            new() { Id = 1, Name = "SVCB0010002" },
            new() { Id = 2, Name = "SVCB0010005" },
            new() { Id = 3, Name = "SVCB0010006" },
            new() { Id = 4, Name = "SVCB0010007" },
            new() { Id = 5, Name = "SVCB0010008" },
            new() { Id = 6, Name = "SVCB0010009" },
            new() { Id = 7, Name = "SVCB0010010" },
            new() { Id = 8, Name = "SVCB0010014" },
            new() { Id = 9, Name = "SVCB0010015" },
            new() { Id = 10, Name = "SVCB0010020" }
        };

        var appointments = appointmentService.Get(p =>
                p.InstanceId == _appointment.InstanceId &&
                p.AppointmentDetails.Any(d => d.IsPaperSetter) &&
                p.AppointmentBillDetails.Any(d =>
                    DbFunctions.TruncateTime(d.BillDate) >= DbFunctions.TruncateTime(fromDate) &&
                    DbFunctions.TruncateTime(d.BillDate) <= DbFunctions.TruncateTime(toDate)),
            p => p).ToList();

        var appointmentDetails = appointments.SelectMany(p =>
            p.AppointmentDetails.Where(d => d.IsPaperSetter), (_, d) => d)
            .ToList();

        var appointmentBillDetails = appointments.SelectMany(p =>
                p.AppointmentBillDetails.Where(d =>
                    d.BillDate >= fromDate &&
                    d.BillDate <= toDate),
            (_, d) => d).ToList();

        var staffIds = appointmentBillDetails.Select(d => d.StaffId).Distinct().ToList();
        var staffs = staffService.Get(p => staffIds.Contains(p.Id), p => p).ToList();
        var instance = instanceService.GetViewModel(_appointment.InstanceId);
        var dataSource = appointmentBillDetails.Select(x =>
        {
            var staff = staffs.FirstOrDefault(s => s.Id == x.StaffId);
            if(null == staff)
                return null;
            var ifscFlag = 0;
            var flag = ifsFlags.FirstOrDefault(f => f.Name == staff.IfscCode);
            if (null != flag)
                ifscFlag = flag.Id;
            var appointmentDetail = appointmentDetails.FirstOrDefault(d => d.IsPaperSetter && 
                                                                           d.StaffId == x.StaffId && 
                                                                           d.AppointmentId == x.AppointmentId);
            if(null == appointmentDetail)
                return null;
            return new ReportModel
            {
                Instance = new MasterViewModel
                {
                    Id = instance.Id,
                    Name = instance.Name,
                },
                StaffId = staff.Id,
                StaffName =
                    $"{staff?.Salutation} {staff?.Name}{(appointmentDetail.IsChairman ? " (Chairman)" : "")}",
                BillModel = new ReportBillModel
                {
                    BillNo = x.Id,
                    TotalFee = x.TotalFee,
                },
                BankModel = new ReportBankModel
                {
                    IfscCode = staff.IfscCode,
                    IfscFlag = ifscFlag,
                    BankAccountNo = staff.BankAccountNo,
                    BankName = staff.BankName,
                    BankBranch = staff.BranchName
                }
            };
        });

        if (!dataSource.Any())
            return;
        report.DataSource = dataSource;

        /*var dataSource = (from appointmentBillDetail in appointmentService
                    .GetQuery()
                    .Where(a => a.InstanceId == _appointment.InstanceId)
                    .SelectMany(a => a.AppointmentBillDetails)
                          let appointment = appointmentBillDetail.Appointment
                          from appointmentDetail in appointment.AppointmentDetails
                          where appointmentDetail.IsPaperSetter && appointmentDetail.StaffId == appointmentBillDetail.StaffId &&
                                DbFunctions.TruncateTime(appointmentBillDetail.BillDate) >= fromDate.Date &&
                                DbFunctions.TruncateTime(appointmentBillDetail.BillDate) <= toDate.Date
                          join staff in staffService.GetQuery().ToList()
                              on appointmentBillDetail.StaffId equals staff.Id /*into staffGroup
                from staff in staffGroup.DefaultIfEmpty()#1#
                          select new
                          {
                              Staff = staff,
                              AppointmentDetail = appointmentDetail,
                              AppointmentBillDetail = appointmentBillDetail
                          }).AsEnumerable()
            .Select(x =>
            {
                var ifscFlag = 0;
                var flag = ifsFlags.FirstOrDefault(f => f.Name == x.Staff.IfscCode);
                if (null != flag)
                    ifscFlag = flag.Id;
                return new ReportModel
                {
                    //StaffName = x.Staff.Name,
                    StaffName =
                        $"{x.Staff?.Salutation} {x.Staff?.Name}{(x.AppointmentDetail.IsChairman ? " (Chairman)" : "")}",
                    BillModel = new ReportBillModel
                    {
                        BillNo = x.AppointmentBillDetail.Id,
                        TotalFee = x.AppointmentBillDetail.TotalFee,
                    },
                    BankModel = new ReportBankModel
                    {
                        IfscCode = x.Staff?.IfscCode,
                        IfscFlag = ifscFlag,
                        BankAccountNo = x.Staff?.BankAccountNo,
                        BankName = x.Staff?.BankName,
                        BankBranch = x.Staff?.BranchName,
                    }
                };
            });

        if (!dataSource.Any())
            return;
        report.DataSource = dataSource;*/

        /*var appointments = appointmentService.Get(p =>
p.AppointmentDetails.Any(d => d.IsPaperSetter) &&
p.AppointmentBillDetails.Any(d =>
DbFunctions.TruncateTime(d.BillDate) >= DbFunctions.TruncateTime(fromDate) &&
DbFunctions.TruncateTime(d.BillDate) <= DbFunctions.TruncateTime(toDate)),
p => p).ToList();

        var scheduleService = Bootstrapper.Get<IScheduleService>();
        var schedules = scheduleService.Get(p =>
            p.InstanceId == _appointment.InstanceId && p.FacultyId == _appointment.FacultyId &&
            p.CollegeId == _appointment.CollegeId && p.CourseId == _appointment.CourseId, p => p).ToList();

        var dataSource = appointmentService.GetData(appointments, schedules, false).ToList();

        if (dataSource.Count <= 0)
            return;

        // Update Name flags
        var ifsFlags = new List<MasterViewModel>
        {
            new() { Id = 1, Name = "SVCB0010002" },
            new() { Id = 2, Name = "SVCB0010005" },
            new() { Id = 3, Name = "SVCB0010006" },
            new() { Id = 4, Name = "SVCB0010007" },
            new() { Id = 5, Name = "SVCB0010008" },
            new() { Id = 6, Name = "SVCB0010009" },
            new() { Id = 7, Name = "SVCB0010010" },
            new() { Id = 8, Name = "SVCB0010014" },
            new() { Id = 9, Name = "SVCB0010015" },
            new() { Id = 10, Name = "SVCB0010020" }
        };

        dataSource.ForEach(d =>
        {
            var id = ifsFlags.FirstOrDefault(x => x.Name == d.BankModel.IfscCode)?.Id;
            if (id != null)
                d.BankModel.IfscFlag = (int)id;
        });

        report.DataSource = dataSource.OrderBy(d => d.BillModel.BillNo);*/
    }
    #endregion
}