using System.Collections.Generic;
using Corno.Data.Corno.Masters;
using Corno.Data.ViewModels;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Corno.Masters.Interfaces;

public interface ICourseService : IMainMasterService<Course>
{
    #region -- Methods --

    List<MasterViewModel> GetCourses(int? facultyId, int? collegeId, string filter = default);
    List<MasterViewModel> GetCourses(int? collegeId, string filter = default);
    #endregion
}