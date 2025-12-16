using System.Collections.Generic;
using Corno.Data.Corno.Masters;
using Corno.Data.ViewModels;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Corno.Masters.Interfaces;

public interface ISubjectService : IMainMasterService<Subject>
{
    #region -- Methods --

    List<MasterViewModel> GetSubjectsByCoursePart(int? coursePartId, int? branchId,
        string filter = default);
    List<MasterViewModel> GetSubjectsByCategory(int? courseId, int? coursePartId, int? branchId, int? categoryId, string filter = default);
    List<MasterViewModel> GetChapters(int? subjectId);
    #endregion
}