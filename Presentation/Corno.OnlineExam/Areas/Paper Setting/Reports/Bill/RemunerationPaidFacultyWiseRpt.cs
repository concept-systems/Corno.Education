using Corno.Reports.Base;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using Corno.OnlineExam.Areas.Paper_Setting.Dtos.Bill;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Paper_Setting.Interfaces;

namespace Corno.OnlineExam.Areas.Paper_Setting.Reports.Bill;

public partial class RemunerationPaidFacultyWiseRpt : BaseReport
{
    #region -- Constructors --
    public RemunerationPaidFacultyWiseRpt(int instanceId)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        _instanceId = instanceId;

        var instanceService = Bootstrapper.Get<IInstanceService>();
        if (instanceService == null)
            throw new System.Exception("Instance service is not available.");
        txtInstance.Value = instanceService.GetViewModel(instanceId).NameWithId;

        table1.DataSource = GetDataSource();
    }
    #endregion

    #region -- Data Members --
    private readonly int _instanceId;
    #endregion

    #region -- Private Methods --
    private List<RemunerationPaidFacultyWiseDto> GetDataSource()
    {
        var appointmentService = Bootstrapper.Get<IAppointmentService>();
        var facultyService = Bootstrapper.Get<IFacultyService>();
        var query = appointmentService.GetQuery()
            .Where(apt => apt.InstanceId == _instanceId && apt.AppointmentBillDetails.Any())
            .Include(nameof(Data.Corno.Paper_Setting.Models.Appointment.AppointmentBillDetails));

        var dataSource = (from appointment in query
            from detail in appointment.AppointmentBillDetails
            join faculty in facultyService.GetQuery()
                on appointment.FacultyId equals faculty.Id into facultyGroup
            from faculty in facultyGroup.DefaultIfEmpty()
            group detail by new
            {
                appointment.FacultyId,
                FacultyName = faculty != null ? faculty.Name : string.Empty,
                appointment.SubjectId
            } into g
            select new RemunerationPaidFacultyWiseDto
            {
                FacultyId = g.Key.FacultyId ?? 0,
                FacultyName = g.Key.FacultyName,
                SubjectId = g.Key.SubjectId ?? 0,
                SetsDrawn = g.FirstOrDefault().SetsDrawn ?? 0,
                Ta = g.Sum(x => x.Ta ?? 0),
                Da = g.Sum(x => x.Da ?? 0),
                CourseFee = g.Sum(x => x.CourseFee ?? 0),
                ChairmanAllowance = g.Sum(x => x.ChairmanAllowance ?? 0),
                TotalFee = g.Sum(x => x.TotalFee ?? 0)
            }).ToList();

        var dataSource1 = dataSource.GroupBy(x => x.FacultyId)
            .Select(g => new RemunerationPaidFacultyWiseDto
            {
                FacultyId = g.FirstOrDefault().FacultyId,
                FacultyName = g.FirstOrDefault().FacultyName,
                SetsDrawn = g.Sum(x => x.SetsDrawn ),
                Ta = g.Sum(x => x.Ta),
                Da = g.Sum(x => x.Da),
                CourseFee = g.Sum(x => x.CourseFee),
                ChairmanAllowance = g.Sum(x => x.ChairmanAllowance),
                TotalFee = g.Sum(x => x.TotalFee)
            });

        return dataSource1.ToList();
    }
    #endregion
}