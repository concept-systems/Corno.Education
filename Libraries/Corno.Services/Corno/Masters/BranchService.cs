using Corno.Data.Corno.Masters;
using Corno.Data.ViewModels;
using Corno.Logger;
using Corno.Services.Corno.Masters.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using Corno.Globals.Constants;
using LinqKit;

namespace Corno.Services.Corno.Masters;

public class BranchService : MainMasterService<Branch>, IBranchService
{
    #region -- Constructors --
    public BranchService(ICoursePartService coursePartService)
    {
        _coursePartService = coursePartService;

        SetIncludes(nameof(Branch.BranchSubjectDetails));
    }
    #endregion

    #region -- Data Members --
    private readonly ICoursePartService _coursePartService;
    #endregion

    #region -- Public Methods --

    public List<MasterViewModel> GetBranches(int? courseId, int? coursePartId, string filter = default)
    {
        try
        {
            var isBranchApplicable =
                _coursePartService.FirstOrDefault(p => p.Id == coursePartId, p => p.IsBranchApplicable ?? false);
            if (!isBranchApplicable)
                return null;

            Expression<Func<Branch, bool>> predicate = p => p.CourseId == courseId && p.Status == StatusConstants.Active;
            /*if (!string.IsNullOrEmpty(filter))
                predicate = predicate.And(p => p.Id.ToString().Contains(filter) || p.Name.Contains(filter));*/
            predicate = AddContainsFilter(predicate, filter);
            var branches = GetViewModelList(predicate).ToList();

            //var branches = GetViewModelList(p => p.CourseId == courseId).ToList();
            return branches;
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
        }

        return null;
    }
    #endregion
}