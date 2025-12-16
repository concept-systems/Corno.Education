using System.Collections.Generic;
using Corno.Data.Corno.Masters;
using Corno.Data.ViewModels;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Corno.Masters.Interfaces;

public interface IBranchService : IMainMasterService<Branch>
{
    #region -- Methods --

    /*Branch GetById(int id);
    IEnumerable<Branch> GetQuery();

    IEnumerable<CoursePart> Get(Expression<Func<CoursePart, bool>> filter = null,
        Func<IQueryable<CoursePart>, IOrderedQueryable<CoursePart>> orderBy = null, string includeProperties = "");*/
    List<MasterViewModel> GetBranches(int? courseId, int? coursePartId, string filter = default);

    /*void Add(Branch branch);
    void Update(Branch branch);*/

    #endregion
}