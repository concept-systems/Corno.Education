using System.Collections.Generic;
using System.Linq;
using Corno.Data.Corno.Paper_Setting.Dtos;
using Corno.Data.Corno.Paper_Setting.Models;
using Corno.Data.Corno.Question_Bank;
using Corno.Data.ViewModels.Appointment;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Corno.Paper_Setting.Interfaces;

public interface IAppointmentService : IMainService<Appointment>
{
    #region -- Methods --

    void ValidateHeader(Appointment appointment);
    void ValidateEmailHeader(Appointment appointment);
    void ValidateReportHeader(Appointment appointment);
    void ValidateFields(Appointment appointment);

    Appointment GetExisting(Appointment appointment);

    IEnumerable<ReportModel> GetData(Appointment appointment, bool onlyInternal);
    IEnumerable<ReportModel> GetReportDataFromAppointment(Appointment appointment, bool onlyInternal);
    IEnumerable<ReportModel> GetReportDataFromSchedule(Appointment appointment, bool onlyInternal);

    IEnumerable<ReportModel> GetData(List<Appointment> appointments, 
        List<Schedule> schedules, bool onlyInternal);

    IEnumerable<EvaluationDto> GetEvaluationReport(Appointment appointment);

    Appointment GetStaffs(Appointment appointment);

    void Save(Appointment appointment);
    #endregion
}