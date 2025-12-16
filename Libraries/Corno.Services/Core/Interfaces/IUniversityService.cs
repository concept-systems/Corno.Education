using System.Collections.Generic;
using Corno.Data.ViewModels;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Core.Interfaces;

public interface IUniversityService : IBaseService
{
    #region -- Methods --

    List<MasterViewModel> GetBranchesByCoursePart(int? courseId, int? coursePartId);

    #endregion
}