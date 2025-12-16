using Corno.Data.Corno.Masters;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Corno.Masters.Interfaces;

public interface ICoursePartService : IMainMasterService<CoursePart>
{
    #region -- Methods --

    /*CoursePart GetById(int id);

    IEnumerable<CoursePart> Get(Expression<Func<CoursePart, bool>> filter = null,
        Func<IQueryable<CoursePart>, IOrderedQueryable<CoursePart>> orderBy = null, string includeProperties = "");

    IEnumerable<CoursePart> GetQuery();

    List<MasterViewModel> GetCourseParts(int? courseId, string filter = default);

    void Add(CoursePart coursePart);
    void Update(CoursePart coursePart);*/

    #endregion
}