using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Threading.Tasks;
using System.Web.Mvc;
using Corno.Data.Corno;
using Corno.Data.Corno.Paper_Setting.Models;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Logger;
using Corno.Education.Controllers;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Paper_Setting.Interfaces;
using Corno.Services.Corno.Masters.Interfaces;

namespace Corno.Education.Areas.Paper_Setting.Controllers;

[Authorize]
public class PaperSettingController : CornoController
{
    #region -- Static Progress Tracking --
    private static readonly ConcurrentDictionary<string, TransferProgress> TransferProgressDict = new ConcurrentDictionary<string, TransferProgress>();
    #endregion

    #region -- Data Members --
    private readonly IAppointmentService _appointmentService;
    private readonly IScheduleService _scheduleService;
    private readonly IRemunerationService _remunerationService;
    #endregion

    #region -- Constructors --
    public PaperSettingController(IAppointmentService appointmentService, IScheduleService scheduleService, IRemunerationService remunerationService)
    {
        _appointmentService = appointmentService;
        _scheduleService = scheduleService;
        _remunerationService = remunerationService;
    }
    #endregion

    #region -- Progress Tracking Class --
    private class TransferProgress
    {
        public int Total { get; set; }
        public int Processed { get; set; }
        public int Transferred { get; set; }
        public int Skipped { get; set; }
        public string CurrentItem { get; set; } = "";
        public bool IsCompleted { get; set; }
        public bool HasError { get; set; }
        public bool IsCancelled { get; set; }
        public string ErrorMessage { get; set; } = "";
    }
    #endregion

    #region -- Private Methods --

    private string GetItemName(int? subjectId, int? coursePartId = null)
    {
        try
        {
            if (subjectId.HasValue && subjectId > 0)
            {
                var subjectService = Bootstrapper.Get<ISubjectService>();
                var subject = subjectService.GetViewModel(subjectId.Value);
                return subject?.Name ?? $"Subject ID: {subjectId}";
            }

            if (coursePartId.HasValue && coursePartId > 0)
            {
                var coursePartService = Bootstrapper.Get<ICoursePartService>();
                var coursePart = coursePartService.GetViewModel(coursePartId.Value);
                return coursePart?.Name ?? $"CoursePart ID: {coursePartId}";
            }
        }
        catch (Exception ex)
        {
            LogHandler.LogError(ex);
        }

        return "";
    }

    #endregion

    #region -- Public Methods --

    [Authorize]
    public JsonResult GetTransferInstances()
    {
        try
        {
            var sessionData = Session[User.Identity.Name] as SessionData;
            if (sessionData == null)
                return Json(new List<object>(), JsonRequestBehavior.AllowGet);

            var currentInstanceId = sessionData.InstanceId;
            var instanceService = Bootstrapper.Get<IInstanceService>();
            var instances = instanceService.GetViewModelList(i => 
                i.Status == StatusConstants.Active && i.Id < currentInstanceId)
                .OrderByDescending(i => i.Id)
                .ToList();

            return Json(instances, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(new List<object>(), JsonRequestBehavior.AllowGet);
        }
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public JsonResult TransferData(string transferType, int sourceInstanceId)
    {
        try
        {
            var sessionData = Session[User.Identity.Name] as SessionData;
            if (sessionData == null)
                return Json(new { success = false, message = "Session data not found." }, JsonRequestBehavior.AllowGet);

            var targetInstanceId = sessionData.InstanceId;

            if (sourceInstanceId >= targetInstanceId)
                return Json(new { success = false, message = "Source instance ID must be lower than current instance ID." }, JsonRequestBehavior.AllowGet);

            if (string.IsNullOrEmpty(transferType))
                return Json(new { success = false, message = "Transfer type not specified." }, JsonRequestBehavior.AllowGet);

            // Initialize progress tracking
            var progressKey = $"TransferProgress_{User.Identity.Name}_{DateTime.Now.Ticks}";
            var progress = new TransferProgress();
            TransferProgressDict.TryAdd(progressKey, progress);

            // Start async transfer
            Task.Run(() =>
            {
                try
                {
                    switch (transferType.ToLower())
                    {
                        case "appointment":
                            TransferAppointmentsAsync(sourceInstanceId, targetInstanceId, sessionData, progressKey);
                            break;
                        case "schedule":
                            TransferSchedulesAsync(sourceInstanceId, targetInstanceId, sessionData, progressKey);
                            break;
                        case "remuneration":
                            TransferRemunerationsAsync(sourceInstanceId, targetInstanceId, sessionData, progressKey);
                            break;
                        default:
                            LogHandler.LogError(new Exception($"Unknown transfer type: {transferType}"));
                            progress.HasError = true;
                            progress.ErrorMessage = "Unknown transfer type.";
                            progress.IsCompleted = true;
                            break;
                    }
                }
                catch (Exception ex)
                {
                    LogHandler.LogError(ex);
                    progress.HasError = true;
                    progress.ErrorMessage = ex.Message;
                    progress.IsCompleted = true;
                }
            });

            Session[$"TransferProgressKey_{User.Identity.Name}"] = progressKey;

            return Json(new { success = true, message = "Transfer started.", progressKey = progressKey }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
        }
    }

    [Authorize]
    public JsonResult GetTransferProgress()
    {
        try
        {
            var progressKey = Session[$"TransferProgressKey_{User.Identity.Name}"] as string;
            if (string.IsNullOrEmpty(progressKey))
                return Json(new { success = false, message = "Progress key not found." }, JsonRequestBehavior.AllowGet);

            if (!TransferProgressDict.TryGetValue(progressKey, out var progress))
                return Json(new { success = false, message = "Progress data not found." }, JsonRequestBehavior.AllowGet);

            var percent = progress.Total > 0 ? (progress.Processed * 100) / progress.Total : 0;

            return Json(new
            {
                success = true,
                total = progress.Total,
                processed = progress.Processed,
                transferred = progress.Transferred,
                skipped = progress.Skipped,
                currentItem = progress.CurrentItem,
                percent = percent,
                isCompleted = progress.IsCompleted,
                hasError = progress.HasError,
                errorMessage = progress.ErrorMessage
            }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
        }
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    public JsonResult CancelTransfer()
    {
        try
        {
            var progressKey = Session[$"TransferProgressKey_{User.Identity.Name}"] as string;
            if (!string.IsNullOrEmpty(progressKey) && TransferProgressDict.TryGetValue(progressKey, out var progress))
            {
                progress.IsCancelled = true;
            }

            return Json(new { success = true, message = "Transfer cancellation initiated." }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(new { success = false, message = exception.Message }, JsonRequestBehavior.AllowGet);
        }
    }

    #endregion

    #region -- Async Transfer Methods --

    private void TransferAppointmentsAsync(int sourceInstanceId, int targetInstanceId, SessionData sessionData, string progressKey)
    {
        try
        {
            if (!TransferProgressDict.TryGetValue(progressKey, out var progress))
                return;

            var sourceAppointments = _appointmentService.GetQuery()
                .Where(a => a.InstanceId == sourceInstanceId)
                .Include(nameof(Appointment.AppointmentDetails))
                .ToList();

            progress.Total = sourceAppointments.Count;

            if (progress.Total == 0)
            {
                progress.IsCompleted = true;
                return;
            }

            foreach (var sourceAppointment in sourceAppointments)
            {
                if (progress.IsCancelled)
                {
                    progress.IsCompleted = true;
                    progress.ErrorMessage = "Transfer was cancelled by user.";
                    return;
                }

                progress.Processed++;
                progress.CurrentItem = GetItemName(sourceAppointment.SubjectId);

                // Check if appointment already exists in target instance
                var existing = _appointmentService.FirstOrDefault(p =>
                    (p.InstanceId ?? 0) == targetInstanceId &&
                    (p.CollegeId ?? 0) == (sourceAppointment.CollegeId ?? 0) &&
                    (p.CourseId ?? 0) == (sourceAppointment.CourseId ?? 0) &&
                    (p.CoursePartId ?? 0) == (sourceAppointment.CoursePartId ?? 0) &&
                    (p.SubjectId ?? 0) == (sourceAppointment.SubjectId ?? 0) &&
                    (p.CategoryId ?? 0) == (sourceAppointment.CategoryId ?? 0),
                    a => a);

                if (existing != null)
                {
                    progress.Skipped++;
                    continue;
                }

                // Create new appointment for target instance
                var newAppointment = new Appointment
                {
                    InstanceId = targetInstanceId,
                    FacultyId = sourceAppointment.FacultyId,
                    CollegeId = sourceAppointment.CollegeId,
                    CourseId = sourceAppointment.CourseId,
                    CoursePartId = sourceAppointment.CoursePartId,
                    CourseTypeId = sourceAppointment.CourseTypeId,
                    BranchId = sourceAppointment.BranchId,
                    CategoryId = sourceAppointment.CategoryId,
                    SubjectId = sourceAppointment.SubjectId,
                    Status = sourceAppointment.Status,
                    Code = sourceAppointment.Code,
                    SerialNo = sourceAppointment.SerialNo,
                    CreatedBy = sessionData.UserId,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = sessionData.UserId,
                    ModifiedDate = DateTime.Now,
                    AppointmentDetails = new List<AppointmentDetail>()
                };

                // Copy AppointmentDetails
                if (sourceAppointment.AppointmentDetails != null && sourceAppointment.AppointmentDetails.Any())
                {
                    foreach (var sourceDetail in sourceAppointment.AppointmentDetails)
                    {
                        var newDetail = new AppointmentDetail
                        {
                            StaffId = sourceDetail.StaffId,
                            IsInternal = sourceDetail.IsInternal,
                            IsBarred = sourceDetail.IsBarred,
                            IsChairman = sourceDetail.IsChairman,
                            IsPaperSetter = sourceDetail.IsPaperSetter,
                            IsModerator = sourceDetail.IsModerator,
                            IsManuscript = sourceDetail.IsManuscript,
                            OriginalId = sourceDetail.OriginalId,
                            NoOfAttempts = sourceDetail.NoOfAttempts,
                            EmailCount = sourceDetail.EmailCount,
                            EmailDate = sourceDetail.EmailDate,
                            SmsCount = sourceDetail.SmsCount,
                            Status = sourceDetail.Status,
                            Code = sourceDetail.Code,
                            SerialNo = sourceDetail.SerialNo,
                            CreatedBy = sessionData.UserId,
                            CreatedDate = DateTime.Now,
                            ModifiedBy = sessionData.UserId,
                            ModifiedDate = DateTime.Now
                        };
                        newAppointment.AppointmentDetails.Add(newDetail);
                    }
                }

                _appointmentService.Save(newAppointment);
                progress.Transferred++;
            }

            progress.IsCompleted = true;
        }
        catch (Exception ex)
        {
            LogHandler.LogError(ex);
            if (TransferProgressDict.TryGetValue(progressKey, out var progress))
            {
                progress.HasError = true;
                progress.ErrorMessage = ex.Message;
                progress.IsCompleted = true;
            }
        }
    }

    private void TransferSchedulesAsync(int sourceInstanceId, int targetInstanceId, SessionData sessionData, string progressKey)
    {
        try
        {
            if (!TransferProgressDict.TryGetValue(progressKey, out var progress))
                return;

            var sourceSchedules = _scheduleService.GetQuery()
                .Where(s => s.InstanceId == sourceInstanceId)
                .Include(nameof(Schedule.ScheduleDetails))
                .ToList();

            progress.Total = sourceSchedules.Count;

            if (progress.Total == 0)
            {
                progress.IsCompleted = true;
                return;
            }

            foreach (var sourceSchedule in sourceSchedules)
            {
                if (progress.IsCancelled)
                {
                    progress.IsCompleted = true;
                    progress.ErrorMessage = "Transfer was cancelled by user.";
                    return;
                }

                progress.Processed++;
                progress.CurrentItem = $"Schedule {sourceSchedule.Code ?? ""}";

                // Check if schedule already exists in target instance
                var existing = _scheduleService.FirstOrDefault(s =>
                    (s.InstanceId ?? 0) == targetInstanceId &&
                    (s.CollegeId ?? 0) == (sourceSchedule.CollegeId ?? 0) &&
                    (s.CourseId ?? 0) == (sourceSchedule.CourseId ?? 0) &&
                    (s.CoursePartId ?? 0) == (sourceSchedule.CoursePartId ?? 0) &&
                    (s.CategoryId ?? 0) == (sourceSchedule.CategoryId ?? 0),
                    s => s);

                if (existing != null)
                {
                    progress.Skipped++;
                    continue;
                }

                // Create new schedule for target instance
                var newSchedule = new Schedule
                {
                    InstanceId = targetInstanceId,
                    FacultyId = sourceSchedule.FacultyId,
                    CollegeId = sourceSchedule.CollegeId,
                    CourseId = sourceSchedule.CourseId,
                    CoursePartId = sourceSchedule.CoursePartId,
                    CategoryId = sourceSchedule.CategoryId,
                    Status = sourceSchedule.Status,
                    Code = sourceSchedule.Code,
                    SerialNo = sourceSchedule.SerialNo,
                    CreatedBy = sessionData.UserId,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = sessionData.UserId,
                    ModifiedDate = DateTime.Now,
                    ScheduleDetails = new List<ScheduleDetail>()
                };

                // Copy ScheduleDetails
                if (sourceSchedule.ScheduleDetails != null && sourceSchedule.ScheduleDetails.Any())
                {
                    foreach (var sourceDetail in sourceSchedule.ScheduleDetails)
                    {
                        var newDetail = new ScheduleDetail
                        {
                            SubjectId = sourceDetail.SubjectId,
                            FromDate = null,
                            ToDate = null,
                            EndDate = null,
                            Time = null,
                            AvailableSets = sourceDetail.AvailableSets,
                            SetsToBeDrawn = sourceDetail.SetsToBeDrawn,
                            UsedSets = sourceDetail.UsedSets,
                            Balance = sourceDetail.Balance,
                            OutwardNo = null,
                            Status = sourceDetail.Status,
                            Code = sourceDetail.Code,
                            SerialNo = sourceDetail.SerialNo,
                            CreatedBy = sessionData.UserId,
                            CreatedDate = DateTime.Now,
                            ModifiedBy = sessionData.UserId,
                            ModifiedDate = DateTime.Now
                        };
                        newSchedule.ScheduleDetails.Add(newDetail);
                    }
                }

                _scheduleService.Save(newSchedule);
                progress.Transferred++;
            }

            progress.IsCompleted = true;
        }
        catch (Exception ex)
        {
            LogHandler.LogError(ex);
            if (TransferProgressDict.TryGetValue(progressKey, out var progress))
            {
                progress.HasError = true;
                progress.ErrorMessage = ex.Message;
                progress.IsCompleted = true;
            }
        }
    }

    private void TransferRemunerationsAsync(int sourceInstanceId, int targetInstanceId, SessionData sessionData, string progressKey)
    {
        try
        {
            if (!TransferProgressDict.TryGetValue(progressKey, out var progress))
                return;

            var sourceRemunerations = _remunerationService.GetQuery()
                .Where(r => r.InstanceId == sourceInstanceId)
                .Include(nameof(Remuneration.RemunerationDetails))
                .ToList();

            progress.Total = sourceRemunerations.Count;

            if (progress.Total == 0)
            {
                progress.IsCompleted = true;
                return;
            }

            foreach (var sourceRemuneration in sourceRemunerations)
            {
                if (progress.IsCancelled)
                {
                    progress.IsCompleted = true;
                    progress.ErrorMessage = "Transfer was cancelled by user.";
                    return;
                }

                progress.Processed++;
                progress.CurrentItem = $"Remuneration {sourceRemuneration.Code ?? ""}";

                // Check if remuneration already exists in target instance
                var existing = _remunerationService.FirstOrDefault(r =>
                    (r.InstanceId ?? 0) == targetInstanceId &&
                    (r.CollegeId ?? 0) == (sourceRemuneration.CollegeId ?? 0) &&
                    (r.CourseId ?? 0) == (sourceRemuneration.CourseId ?? 0),
                    r => r);

                if (existing != null)
                {
                    progress.Skipped++;
                    continue;
                }

                // Create new remuneration for target instance
                var newRemuneration = new Remuneration
                {
                    InstanceId = targetInstanceId,
                    FacultyId = sourceRemuneration.FacultyId,
                    CollegeId = sourceRemuneration.CollegeId,
                    CourseId = sourceRemuneration.CourseId,
                    Status = sourceRemuneration.Status,
                    Code = sourceRemuneration.Code,
                    SerialNo = sourceRemuneration.SerialNo,
                    CreatedBy = sessionData.UserId,
                    CreatedDate = DateTime.Now,
                    ModifiedBy = sessionData.UserId,
                    ModifiedDate = DateTime.Now,
                    RemunerationDetails = new List<RemunerationDetail>()
                };

                // Copy RemunerationDetails
                if (sourceRemuneration.RemunerationDetails != null && sourceRemuneration.RemunerationDetails.Any())
                {
                    foreach (var sourceDetail in sourceRemuneration.RemunerationDetails)
                    {
                        var newDetail = new RemunerationDetail
                        {
                            CoursePartId = sourceDetail.CoursePartId,
                            Fee = sourceDetail.Fee,
                            Others = sourceDetail.Others,
                            SchemeOfMarking = sourceDetail.SchemeOfMarking,
                            ModelAnswers = sourceDetail.ModelAnswers,
                            Status = sourceDetail.Status,
                            Code = sourceDetail.Code,
                            SerialNo = sourceDetail.SerialNo,
                            CreatedBy = sessionData.UserId,
                            CreatedDate = DateTime.Now,
                            ModifiedBy = sessionData.UserId,
                            ModifiedDate = DateTime.Now
                        };
                        newRemuneration.RemunerationDetails.Add(newDetail);
                    }
                }

                _remunerationService.Save(newRemuneration);
                progress.Transferred++;
            }

            progress.IsCompleted = true;
        }
        catch (Exception ex)
        {
            LogHandler.LogError(ex);
            if (TransferProgressDict.TryGetValue(progressKey, out var progress))
            {
                progress.HasError = true;
                progress.ErrorMessage = ex.Message;
                progress.IsCompleted = true;
            }
        }
    }

    #endregion
}
