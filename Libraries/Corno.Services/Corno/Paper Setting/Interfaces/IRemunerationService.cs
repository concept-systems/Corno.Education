using Corno.Data.Corno.Paper_Setting.Models;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Corno.Paper_Setting.Interfaces;

public interface IRemunerationService : IMainService<Remuneration>
{
    #region -- Methods --

    void ValidateFields(Remuneration remuneration);

    Remuneration GetExisting(Appointment appointment);
    Remuneration GetCourseParts(Remuneration remuneration);
    void Save(Remuneration remuneration);
    #endregion
}