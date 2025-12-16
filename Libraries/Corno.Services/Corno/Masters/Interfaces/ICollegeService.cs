using Corno.Data.Corno.Masters;
using Corno.Data.ViewModels;
using Corno.Services.Corno.Interfaces;
using System.Linq;

namespace Corno.Services.Corno.Masters.Interfaces;

public interface ICollegeService : IMainMasterService<College>
{
    #region -- Methods --

    IQueryable<MasterViewModel> GetByFaculty(int? facultyId, string filter = default);

    #endregion
}