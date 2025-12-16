using Corno.Data.Corno.Masters;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Corno.Masters.Interfaces;

public interface ICategoryService : IMainMasterService<Category>
{
    /*#region -- Methods --

    Category GetById(int id);

    IEnumerable<Category> Get(Expression<Func<Category, bool>> filter = null,
        Func<IQueryable<Category>, IOrderedQueryable<Category>> orderBy = null, string includeProperties = "");

    IEnumerable<Category> GetQuery();

    IEnumerable<int?> GetCategoryIds(int courseId, int rootCategoryId);
    List<MasterViewModel> GetCategories();
    List<MasterViewModel> GetCategories(int? courseId, string filter = default);

    void Add(Category model);
    void Update(Category model);

    #endregion*/
}