using Corno.Data.Corno.Masters;
using Corno.Data.ViewModels;
using Corno.Services.Corno.Masters.Interfaces;
using System.Linq;

namespace Corno.Services.Corno.Masters;

public class CollegeService : MainMasterService<College>, ICollegeService
{
    #region -- Constructors --
    public CollegeService()
    {
        SetIncludes(nameof(College.CollegeCourseDetails));
    }
    #endregion

    #region -- Public Methods --
    public IQueryable<MasterViewModel> GetByFaculty(int? facultyId, string filter = default)
    {
        var query = GetViewModelList(m => m.CollegeCourseDetails.Any(c =>
            c.FacultyId == facultyId));

        if (!string.IsNullOrEmpty(filter))
            query = query.Where(d => d.Id.ToString().ToLower().Contains(filter.ToLower()) ||
                                     d.Name.ToLower().Contains(filter.ToLower()));
        return query;
    }
    #endregion
}