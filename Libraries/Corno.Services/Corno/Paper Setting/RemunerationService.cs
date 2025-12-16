using System;
using System.Linq;
using Corno.Data.Corno.Paper_Setting.Models;
using Corno.Data.Helpers;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Paper_Setting.Interfaces;

namespace Corno.Services.Corno.Paper_Setting;

public class RemunerationService : MainService<Remuneration>, IRemunerationService
{
    #region -- Constructors --
    public RemunerationService(ICoursePartService coursePartService)
    {
        _coursePartService = coursePartService;

        SetIncludes(nameof(Remuneration.RemunerationDetails));
    }
    #endregion

    #region -- Data Members --

    private readonly ICoursePartService _coursePartService;

    #endregion

    #region -- Private Methods --

    private Remuneration GetExisting(Remuneration remuneration)
    {
        var existing = FirstOrDefault(p =>
                (p.InstanceId ?? 0) == (remuneration.InstanceId ?? 0) &&
                (p.FacultyId ?? 0) == (remuneration.FacultyId ?? 0) && (p.CollegeId ?? 0) == (remuneration.CollegeId ?? 0) &&
                (p.CourseId ?? 0) == (remuneration.CourseId ?? 0), p => p);
        return existing;
    }

    private void ValidateHeader(Remuneration remuneration)
    {
        if (remuneration.InstanceId.ToInt() <= 0)
            throw new Exception("Invalid instance / session.");
        if (remuneration.CollegeId.ToInt() <= 0)
            throw new Exception("Invalid College.");
        if (remuneration.CourseId.ToInt() <= 0)
            throw new Exception("Invalid Course.");
    }
    #endregion

    #region -- Public Methods --

    public Remuneration GetExisting(Appointment appointment)
    {
        var existing = FirstOrDefault(p =>
            (p.InstanceId ?? 0) == (appointment.InstanceId ?? 0) &&
            (p.FacultyId ?? 0) == (appointment.FacultyId ?? 0) && (p.CollegeId ?? 0) == (appointment.CollegeId ?? 0) &&
            (p.CourseId ?? 0) == (appointment.CourseId ?? 0), p => p);
        return existing;
    }

    public void ValidateFields(Remuneration remuneration)
    {
        // Validate header
        ValidateHeader(remuneration);

        if (remuneration.RemunerationDetails.Count <= 0)
            throw new Exception("No rows in Remuneration.");
        if (remuneration.RemunerationDetails.All(d => null == d.CoursePartId || null == d.Fee))
            throw new Exception("All rows are blank.");
    }

    public Remuneration GetCourseParts(Remuneration remuneration)
    {
        // Validate header
        ValidateHeader(remuneration);

        remuneration = GetExisting(remuneration) ?? remuneration;
        //var coursePartIds = remuneration.RemunerationDetails.Select(d => d.CoursePartId).ToList();
        //var courseParts = _coursePartService.GetViewModelList(p =>
        //    coursePartIds.Contains(p.Id)).ToList();
        var courseParts = _coursePartService.GetViewModelList(p =>
            p.CourseId == (remuneration.CourseId ?? 0)).ToList();
        foreach (var coursePart in courseParts)
        {
            var remunerationDetail = remuneration.RemunerationDetails.FirstOrDefault(a =>
                a.CoursePartId == coursePart.Id);
            if (null != remunerationDetail)
            {
                remunerationDetail.CoursePartName = coursePart.Name;
                continue;
            }

            remuneration.RemunerationDetails.Add(new RemunerationDetail
            {
                CoursePartId = coursePart.Id,
                CoursePartName = coursePart.Name,
                Fee = 0,
                Others = 0,
                SchemeOfMarking = 0,
                ModelAnswers = 0
            });
        }

        return remuneration;
    }

    public void Save(Remuneration remuneration)
    {
        var existing = GetExisting(remuneration);
        if (null == existing)
        {
            AddAndSave(remuneration);
            return;
        }

        // Update existing
        existing.RemunerationDetails.ForEach(d =>
        {
            var newDetail = remuneration.RemunerationDetails.FirstOrDefault(x => x.CoursePartId == d.CoursePartId);
            d.Fee = newDetail?.Fee;
            d.Others = newDetail?.Others;
            d.SchemeOfMarking = newDetail?.SchemeOfMarking;
            d.ModelAnswers = newDetail?.ModelAnswers;
        });

        UpdateAndSave(existing);
    }
    #endregion
}