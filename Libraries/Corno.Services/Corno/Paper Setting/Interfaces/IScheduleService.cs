using Corno.Data.Corno.Paper_Setting.Models;
using Corno.Data.Corno.Question_Bank;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Corno.Paper_Setting.Interfaces;

public interface IScheduleService : IMainService<Schedule>
{
    #region -- Methods --

    Schedule GetExisting(Appointment appointment);
    Schedule GetAllSubjects(Schedule schedule);

    void ValidateFields(Schedule schedule);
    void ValidateReportHeader(Schedule appointment);

    void Save(Schedule schedule);
    #endregion
}