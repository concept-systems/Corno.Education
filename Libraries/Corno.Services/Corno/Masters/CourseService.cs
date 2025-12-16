using System;
using System.Collections.Generic;
using System.Linq;
using Corno.Data.Corno.Masters;
using Corno.Data.ViewModels;
using Corno.Logger;
using Corno.Services.Corno.Masters.Interfaces;

namespace Corno.Services.Corno.Masters;

public class CourseService : MainMasterService<Course>, ICourseService
{
    #region -- Constructors --
    public CourseService(ICollegeService collegeService)
    {
        _collegeService = collegeService;

        SetIncludes(nameof(Course.CourseCategoryDetails));
    }
    #endregion

    #region -- Data Members --
    private readonly ICollegeService _collegeService;
    #endregion

    #region -- Public Methods --
    public List<MasterViewModel> GetCourses(int? facultyId, int? collegeId, string filter = default)
    {
        try
        {
            var college = _collegeService.GetById(collegeId);
            var courseIds = college.CollegeCourseDetails.Where(d =>
                    d.FacultyId == facultyId && d.CollegeId == collegeId)
                .Select(d => d.CourseId).Distinct().ToList();

            var courses = GetViewModelList(p => courseIds.Contains(p.Id ?? 0))
                .OrderByDescending(p => p.Id).ToList();
            if (!string.IsNullOrEmpty(filter))
                return courses.Where(p => p.Id.ToString().ToLower().Contains(filter.ToLower()) ||
                                             p.Name.ToLower().Contains(filter.ToLower()) || p.NameWithId.ToLower().Contains(filter.ToLower()) ||
                                             p.NameWithCode.ToLower().Contains(filter.ToLower())).ToList();
            return courses;
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
        }

        return null;
    }

    public List<MasterViewModel> GetCourses(int? collegeId, string filter = default)
    {
        try
        {
            var college = _collegeService.GetById(collegeId);
            var courseIds = college.CollegeCourseDetails.Where(d =>
                    d.CollegeId == collegeId)
                .Select(d => d.CourseId).Distinct().ToList();

            var courses = GetViewModelList(p => courseIds.Contains(p.Id ?? 0))
                .OrderByDescending(p => p.Id).ToList();
            if (!string.IsNullOrEmpty(filter))
                return courses.Where(p => p.Id.ToString().ToLower().Contains(filter.ToLower()) ||
                                          p.Name.ToLower().Contains(filter.ToLower()) || p.NameWithId.ToLower().Contains(filter.ToLower()) ||
                                          p.NameWithCode.ToLower().Contains(filter.ToLower())).ToList();
            return courses;
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
        }

        return null;
    }

    /*public void Add(Course course)
    {
        _masterService.CourseRepository.Add(course);
        _masterService.Save();
    }

    public void Update(Course course)
    {
        _masterService.CourseRepository.Update(course);
        _masterService.Save();
    }*/
    #endregion
}