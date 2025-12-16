using Corno.Data.Corno.Masters;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Corno.Masters.Interfaces;

public interface IInstanceService : IMainMasterService<Instance>
{
    /*#region -- Methods --

    Instance GetById(int id);
    IEnumerable<Instance> GetQuery();

    IEnumerable<CoursePart> Get(Expression<Func<CoursePart, bool>> filter = null,
        Func<IQueryable<CoursePart>, IOrderedQueryable<CoursePart>> orderBy = null, string includeProperties = "");
    //List<MasterViewModel> GetFaculties(int? courseId, int? coursePartId, string filter = default);

    void Add(Instance instance);
    void Update(Instance instance);

    #endregion*/
}