using Corno.Data.Corno.Masters;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Corno.Masters.Interfaces;

public interface IStaffService : IMainMasterService<Staff>
{
    /*#region -- Methods --

    Staff GetById(int id);

    IEnumerable<Staff> GetQuery();
    List<StaffIndexViewModel> GetList(DataSourceRequest request);
    List<MasterViewModel> GetStaffs(int? coursePartId, int? branchId, string filter = default);
    
    IEnumerable<Staff> Get(Expression<Func<Staff, bool>> filter = null, Func<IQueryable<Staff>, IOrderedQueryable<Staff>> orderBy = null, string includeProperties = "");

    void Add(Staff staff);
    void Update(Staff staff);

    #endregion*/
}