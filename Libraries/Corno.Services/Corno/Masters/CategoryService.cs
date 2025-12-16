using Corno.Data.Corno.Masters;
using Corno.Services.Corno.Masters.Interfaces;

namespace Corno.Services.Corno.Masters;

public class CategoryService : MainMasterService<Category>, ICategoryService
{
    /*#region -- Constructors --
    public CategoryService(IMasterService masterService)
    {
        _masterService = masterService;
    }
    #endregion

    #region -- Data Members --

    private readonly IMasterService _masterService;
    #endregion

    #region -- Public Methods --
    public Category GetById(int id)
    {
        // This is because, we need to fetch include properties also
        return _masterService.CategoryRepository.Get(s => s.Id == id).FirstOrDefault();
    }

    public IEnumerable<Category> Get(Expression<Func<Category, bool>> filter = null,
        Func<IQueryable<Category>, IOrderedQueryable<Category>> orderBy = null, string includeProperties = "")
    {
        // This is because, we need to fetch include properties also
        return _masterService.CategoryRepository.Get(filter, orderBy, includeProperties);
    }

    public IEnumerable<Category> GetQuery()
    {
        return _masterService.CategoryRepository.GetQuery();
    }

    public IEnumerable<int?> GetCategoryIds(int courseId, int rootCategoryId)
    {
        var subCategoryIds = _masterService.CourseCategoryDetailRepository.Get(c => c.CourseId == courseId &&
                                                                    c.RootCategoryId == rootCategoryId)
            .Select(d => d.CategoryId).ToList();
        return subCategoryIds;
    }

    public List<MasterViewModel> GetCategories(int? courseId, string filter = default)
    {
        try
        {
            if ((courseId ?? 0) <= 0)
                throw new Exception("Invalid Course Code");
            var rootCategoryIds = _masterService.CourseCategoryDetailRepository.Get(d => d.CourseId == courseId)
                .Select(d => d.RootCategoryId).Distinct().ToList();
            if (!rootCategoryIds.Any())
                throw new Exception($"No categories available in course {courseId}");

            var query = _masterService.CategoryRepository.GetQuery();
            query = query.Where(c => rootCategoryIds.Contains(c.Id) && c.Status != StatusConstants.Cancelled);

            if (!string.IsNullOrEmpty(filter))
                query = query.Where(d => d.Id.ToString().ToLower().Contains(filter.ToLower()) ||
                                         d.Name.ToLower().Contains(filter.ToLower()));
            var result = query.Select(c => new MasterViewModel { Id = c.Id ?? 0, Name = c.Name, NameWithCode = $"({c.Code}) {c.Name}", NameWithId = $"({c.Id}) {c.Name}" })
                .OrderBy(c => c.Id).ToList();
            return result;

            /*var categories = _masterService.CategoryRepository.Get(c => c.Status != StatusConstants.Cancelled &&
                                                                        rootCategoryIds.Contains(c.Id))
                .Select(c => new MasterViewModel { Id = c.Id ?? 0, Name = c.Name, NameWithId = $"({c.Id}) {c.Name}" })
                .OrderBy(b => b.Id).ToList();

            return categories;#1#
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
        }

        return null;
    }

    public List<MasterViewModel> GetCategories()
    {
        try
        {
            var categories = _masterService.CategoryRepository.Get(c => c.Status != StatusConstants.Cancelled)
                .Select(c => new MasterViewModel { Id = c.Id ?? 0, Name = c.Name, NameWithId = $"({c.Id}) {c.Name}" })
                .OrderBy(b => b.Id).ToList();

            return categories;
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
        }

        return null;
    }

    public void Add(Category model)
    {
        _masterService.CategoryRepository.Add(model);
        _masterService.Save();
    }

    public void Update(Category model)
    {
        _masterService.CategoryRepository.Update(model);
        _masterService.Save();
    }
    #endregion*/
}