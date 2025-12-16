using System;
using System.Linq;
using Corno.Data.Corno.Paper_Setting.Models;
using Corno.Data.Corno.Question_Bank;
using Corno.Data.Helpers;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Paper_Setting.Interfaces;

namespace Corno.Services.Corno.Paper_Setting;

public class ScheduleService : MainService<Schedule>, IScheduleService
{
    #region -- Constructors --
    public ScheduleService(ISubjectService subjectService)
    {
        _subjectService = subjectService;

        SetIncludes(nameof(Schedule.ScheduleDetails));
    }
    #endregion

    #region -- Data Members --

    private readonly ISubjectService _subjectService;

    #endregion

    #region -- Private Methods --

    private Schedule GetExisting(Schedule schedule)
    {
        var existing = FirstOrDefault(s =>
                (s.InstanceId ?? 0) == (schedule.InstanceId ?? 0) &&
                (s.FacultyId ?? 0) == (schedule.FacultyId ?? 0) && (s.CollegeId ?? 0) == (schedule.CollegeId ?? 0) &&
                (s.CourseId ?? 0) == (schedule.CourseId ?? 0) &&
                (s.CoursePartId ?? 0) == (schedule.CoursePartId ?? 0) &&
                (s.BranchId ?? 0) == (schedule.BranchId ?? 0) &&
                (s.CategoryId ?? 0) == (schedule.CategoryId ?? 0), s => s);
        return existing;
    }

    private void ValidateHeader(Schedule schedule)
    {
        if (schedule.InstanceId.ToInt() <= 0)
            throw new Exception("Invalid instance / session.");
        if (schedule.CollegeId.ToInt() <= 0)
            throw new Exception("Invalid College.");
        if (schedule.CourseId.ToInt() <= 0)
            throw new Exception("Invalid Course.");
        if (schedule.CoursePartId.ToInt() <= 0)
            throw new Exception("Invalid Course Part.");
        if (schedule.CategoryId.ToInt() <= 0)
            throw new Exception("Invalid Subject Category.");
    }
    #endregion

    #region -- Public Methods --
    public void ValidateFields(Schedule schedule)
    {
        // Validate header
        ValidateHeader(schedule);

        if (schedule.ScheduleDetails.Count <= 0)
            throw new Exception("No rows in schedule.");
        if (schedule.ScheduleDetails.All(d => null == d.FromDate || null == d.Time))
            throw new Exception("All rows are blank.");
    }

    public void ValidateReportHeader(Schedule appointment)
    {
        if (appointment.InstanceId.ToInt() <= 0)
            throw new Exception("Invalid instance / session.");
    }

    public Schedule GetExisting(Appointment appointment)
    {
        var existing = FirstOrDefault(s =>
            (s.InstanceId ?? 0) == (appointment.InstanceId ?? 0) &&
            (s.FacultyId ?? 0) == (appointment.FacultyId ?? 0) && (s.CollegeId ?? 0) == (appointment.CollegeId ?? 0) &&
            (s.CourseId ?? 0) == (appointment.CourseId ?? 0) &&
            (s.CoursePartId ?? 0) == (appointment.CoursePartId ?? 0) &&
            (s.BranchId ?? 0) == (appointment.BranchId ?? 0) &&
            (s.CategoryId ?? 0) == (appointment.CategoryId ?? 0), s => s);
        return existing;
    }

    public Schedule GetAllSubjects(Schedule schedule)
    {
        // Validate header
        ValidateHeader(schedule);

        var existing = GetExisting(schedule);
        if (null != existing)
        {
            var subjectIds = existing.ScheduleDetails.Select(d => d.SubjectId ?? 0).ToList();
            var existingSubjects = _subjectService.GetViewModelList(p => subjectIds.Contains(p.Id ?? 0)).ToList();
            existing.ScheduleDetails.ForEach(d =>
            {
                var subject = existingSubjects.FirstOrDefault(x => x.Id == d.SubjectId);
                d.SubjectName = subject?.NameWithCode;
                d.AvailableSets = 0;
            });

            return existing;
        }

        var subjects = _subjectService.GetSubjectsByCategory(schedule.CourseId,
            schedule.CoursePartId, schedule.BranchId, schedule.CategoryId);
        schedule.ScheduleDetails.Clear();
        foreach (var subject in subjects)
        {
            schedule.ScheduleDetails.Add(new ScheduleDetail
            {
                SubjectId = subject.Id,
                SubjectName = subject.NameWithCode,
                AvailableSets = 0
            });
        }

        return schedule;
    }

    public void Save(Schedule schedule)
    {
        // Generate outward No. :
        var modifiedScheduleDetails = schedule.ScheduleDetails.Where(d =>
            d.FromDate != null && (d.OutwardNo ?? 0) <= 0).ToList();
        if (modifiedScheduleDetails.Count > 0)
        {
            var maxOutwardNo = Get(p => p.InstanceId == schedule.InstanceId, p => p)
                .SelectMany(p => p.ScheduleDetails, (_, d) => d.OutwardNo).Max();
            var lastOutwardNo = (maxOutwardNo ?? 0) + 1;
            modifiedScheduleDetails.ForEach(d => d.OutwardNo = lastOutwardNo ++);
        }

        var existing = GetExisting(schedule);
        if (null == existing)
        {
            AddAndSave(schedule);
            return;
        }

        // Update existing
        existing.ScheduleDetails.ForEach(d =>
        {
            var newDetail = schedule.ScheduleDetails.FirstOrDefault(x => x.SubjectId == d.SubjectId);

            d.FromDate = newDetail?.FromDate;
            d.ToDate = newDetail?.ToDate;
            d.EndDate = newDetail?.EndDate;
            d.Time = newDetail?.Time;
            d.AvailableSets = newDetail?.AvailableSets;
            d.SetsToBeDrawn = newDetail?.SetsToBeDrawn;
            d.UsedSets = newDetail?.UsedSets;
            d.OutwardNo = newDetail?.OutwardNo;
        });

        UpdateAndSave(existing);
    }
    #endregion
}