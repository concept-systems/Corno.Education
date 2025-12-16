using System;
using System.Collections.Generic;
using System.Linq;
using Corno.Data.Corno.Masters;
using Corno.Data.ViewModels;
using Corno.Logger;
using Corno.Services.Corno.Masters.Interfaces;

namespace Corno.Services.Corno.Masters;

public class SubjectService : MainMasterService<Subject>, ISubjectService
{
    #region -- Constructors --
    public SubjectService(ICourseService courseService, ICoursePartService coursePartService, IBranchService branchService)
    {
        _courseService = courseService;
        _coursePartService = coursePartService;
        _branchService = branchService;

        SetIncludes($"{nameof(Subject.SubjectCategoryDetails)},{nameof(Subject.SubjectChapterDetails)},{nameof(Subject.SubjectInstructionDetails)},{nameof(Subject.SubjectSectionDetails)}");
    }
    #endregion

    #region -- Data Members --

    private readonly ICourseService _courseService;
    private readonly ICoursePartService _coursePartService;
    private readonly IBranchService _branchService;

    #endregion

    #region -- Public Methods --
    public List<MasterViewModel> GetSubjectsByCoursePart(int? coursePartId, int? branchId,
        string filter = default)
    {
        var isBranchApplicable = _coursePartService.FirstOrDefault(p => p.Id == coursePartId,
            p => p.IsBranchApplicable ?? false);
        List<MasterViewModel> subjects;
        if (isBranchApplicable)
        {
            var branch = _branchService.GetById(branchId);
            var subjectIds = branch?.BranchSubjectDetails?
                .Where(d => d.CoursePartId == coursePartId)
                .Select(d => d.SubjectId).Distinct().ToList();
            subjects = GetViewModelList(p => subjectIds.Contains(p.Id ?? 0)).ToList();
        }
        else
        {
            subjects = GetViewModelList(p => p.CoursePartId == coursePartId)
                .ToList();
        }

        // Add filter by user in combobox selection
        if (!string.IsNullOrEmpty(filter))
            subjects = subjects.Where(p => p.Id.ToString().ToLower().Contains(filter.ToLower()) ||
                                           p.Name.ToLower().Contains(filter.ToLower())).ToList();
        return subjects;

    }

    public List<MasterViewModel> GetSubjectsByCategory(int? courseId, int? coursePartId, int? branchId, int? categoryId,
        string filter = default)
    {
        try
        {
            var isBranchApplicable = _coursePartService.FirstOrDefault(p => p.Id == coursePartId,
                p => p.IsBranchApplicable ?? false);
            List<MasterViewModel> subjects;
            if (isBranchApplicable)
            {
                var branch = _branchService.GetById(branchId);
                var subjectIds = branch?.BranchSubjectDetails?
                    .Where(d => d.CoursePartId == coursePartId)
                    .Select(d => d.SubjectId).Distinct().ToList();
                subjects = GetViewModelList(p => subjectIds.Contains(p.Id ?? 0)).ToList();

                //return subjects;
            }
            else
            {
                var course = _courseService.GetById(courseId);
                var categoryIds = course.CourseCategoryDetails.Where(d =>
                    d.RootCategoryId == categoryId).Select(d => d.CategoryId).ToList();

                subjects = GetViewModelList(p => p.CourseId == courseId && p.CoursePartId == coursePartId &&
                                                 p.SubjectCategoryDetails.Any(d => categoryIds.Contains(d.CategoryId)))
                    .ToList();
            }

            // Add filter by user in combobox selection
            if (!string.IsNullOrEmpty(filter))
                subjects = subjects.Where(p => p.Id.ToString().ToLower().Contains(filter.ToLower()) ||
                                               p.Name.ToLower().Contains(filter.ToLower())).ToList();

            return subjects;
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
        }

        return null;
    }

    public List<MasterViewModel> GetChapters(int? subjectId)
    {
        //var subject = GetById(subjectId);
        var subject = FirstOrDefault(p => p.Id == subjectId, p => p);

        return subject?.SubjectChapterDetails
            .Select(b => new MasterViewModel { Id = b.Id ?? 0, Name = b.Name, NameWithCode = $"({b.Code}) {b.Name}", NameWithId = $"({b.Id}) {b.Name}" })
            .OrderBy(b => b.Id).ToList();
    }

    #endregion
}