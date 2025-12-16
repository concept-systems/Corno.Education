using Corno.Data.Corno.Masters;
using Corno.Services.Corno.Masters.Interfaces;

namespace Corno.Services.Corno.Masters;

public class InstanceService : MainMasterService<Instance>, IInstanceService
{
    /*#region -- Constructors --
    public InstanceService(IMasterService masterService, ICoursePartService coursePartService)
    {
        _masterService = masterService;
        _coursePartService = coursePartService;
    }
    #endregion

    #region -- Data Members --

    private readonly IMasterService _masterService;
    private readonly ICoursePartService _coursePartService;

    #endregion

    #region -- Public Methods --
    public Instance GetById(int id)
    {
        // This is because, we need to fetch include properties also
        return _masterService.InstanceRepository.Get(s => s.Id == id).FirstOrDefault();
    }

    public IEnumerable<Instance> GetQuery()
    {
        return _masterService.InstanceRepository.GetQuery();
    }

    public IEnumerable<CoursePart> Get(Expression<Func<CoursePart, bool>> filter = null,
        Func<IQueryable<CoursePart>, IOrderedQueryable<CoursePart>> orderBy = null, string includeProperties = "")
    {
        // This is because, we need to fetch include properties also
        return _masterService.CoursePartRepository.Get(filter, orderBy, includeProperties);
    }

    public void Add(Instance instance)
    {
        _masterService.InstanceRepository.Add(instance);
        _masterService.Save();
    }

    public void Update(Instance instance)
    {
        _masterService.InstanceRepository.Update(instance);
        _masterService.Save();
    }
    #endregion*/
}