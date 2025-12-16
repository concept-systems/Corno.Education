using Corno.Data.Corno.Masters;
using Corno.Services.Corno.Masters.Interfaces;

namespace Corno.Services.Corno.Masters;

public class StaffService : MainMasterService<Staff>, IStaffService
{
    #region -- Constructors --
    public StaffService()
    {
        SetIncludes(nameof(Staff.StaffSubjectDetails));
    }
    #endregion

    /*#region -- Data Members --

    private readonly IMasterService _masterService;

    #endregion

    #region -- Public Methods --
    public Staff GetById(int id)
    {
        // This is because, we need to fetch include properties also
        return _masterService.StaffRepository.Get(s => s.Id == id).FirstOrDefault();
    }
    public virtual IEnumerable<Staff> Get(Expression<Func<Staff, bool>> filter = null,
        Func<IQueryable<Staff>, IOrderedQueryable<Staff>> orderBy = null, string includeProperties = "")
    {
        return _masterService.StaffRepository.Get(filter, orderBy, includeProperties);
    }

    public IEnumerable<Staff> GetQuery()
    {
        return _masterService.StaffRepository.GetQuery();
    }

    public List<StaffIndexViewModel> GetList(DataSourceRequest request)
    {
        try
        {
            var staffs = _masterService.StaffRepository.Get()
                .OrderBy(b => b.Id) // Apply ordering first
                .Skip((request.Page - 1) * request.PageSize) // Then apply skip
                .Take(request.PageSize) // And take
                .Select(b => new StaffIndexViewModel
                {
                    Id = b.Id ?? 0,
                    Name = b.Name,
                    NameWithCode = $"({b.Code}) {b.Name}",
                    NameWithId = $"({b.Id}) {b.Name}",
                    Status = b.Status
                })
                .OrderBy(b => b.Id)
                .ToList();

            return staffs;
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
        }

        return null;
    }

    public void Add(Staff staff)
    {
        _masterService.StaffRepository.Add(staff);
        _masterService.Save();
    }

    public void Update(Staff staff)
    {
        _masterService.StaffRepository.Update(staff);
        _masterService.Save();
    }
    #endregion*/
}