using Corno.Globals.Constants;
using System;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using Corno.Data.Core;
using Corno.Data.Corno;
using Corno.Logger;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;

namespace Corno.OnlineExam.Areas.Transactions.Controllers;

[Authorize]
public class TimeTableTheoryController : TimeTableController
{
    #region -- Constructors --
    public TimeTableTheoryController(ICornoService transactionService, ICoreService examService)
        : base(transactionService, examService)
    {
        _transactionService = transactionService;
        _examService = examService;
    }
    #endregion

    #region -- Data Members --

    private readonly ICornoService _transactionService;
    private readonly ICoreService _examService;
    #endregion

    #region -- Methods --
    [Authorize]
    protected override void AddSubject(TimeTable model, Tbl_SUBJECT_MSTR subject, int categoryCode, int? paperCode,
        string categoryName, string subjectType, string CHR_WRT_UNI_APL)
    {
        if (subject == null) return;

        if (CHR_WRT_UNI_APL != "Y") return;

        if (null != model.TimeTableTheoryDetails.FirstOrDefault(t => t.SubjectId == subject.Num_PK_SUB_CD))
            return;

        var timeTableTheoryDetail = new TimeTableTheoryDetail
        {
            SubjectId = subject.Num_PK_SUB_CD,
            SubjectName = subject.Var_SUBJECT_NM,
            CategoryName = categoryName,
            CategoryCode = categoryCode,
            PaperCode = paperCode,
            SubjectType = subjectType,
            SubjectDivisionCode = 0
        };

        // For Common subjects.
        var existing = _examService.Tbl_TIMETABLE_TRX_Repository.Get(t => t.Num_FK_INST_NO == model.InstanceId &&
                                                                          t.Num_FK_COPRT_NO == (model.CoursePartId ?? 0) &&
                                                                          t.Num_FK_PH_CD == subject.Num_PK_SUB_CD)
            .FirstOrDefault();
        if (existing?.VAR_START_TIME != null && null != existing.VAR_TO_TIME)
        {
            timeTableTheoryDetail.StartDate = existing.Dtm_TBM_FROM_TIME;
            timeTableTheoryDetail.StartTime = Convert.ToDateTime(existing.VAR_START_TIME);
            timeTableTheoryDetail.EndTime = Convert.ToDateTime(existing.VAR_TO_TIME);
        }

        // Subject Division.
        var subDivCount = subject.Num_Subject_Div ?? 0;
        if (subDivCount > 1)
        {
            for (var index = 0; index < subDivCount; index++)
            {
                var subjectDivisionCode = index + 1;
                // For Common subjects.
                var timeTableTrx = _examService.Tbl_TIMETABLE_TRX_Repository.Get(t => t.Num_FK_INST_NO == model.InstanceId &&
                        t.Num_FK_COPRT_NO == (model.CoursePartId ?? 0) &&
                        t.Num_FK_PH_CD == subject.Num_PK_SUB_CD && 
                        t.NUM_FK_SUB_DIV_CD == subjectDivisionCode)
                    .FirstOrDefault();
                if (timeTableTrx?.VAR_START_TIME != null && null != timeTableTrx.VAR_TO_TIME)
                {
                    timeTableTheoryDetail.StartDate = timeTableTrx.Dtm_TBM_FROM_TIME;
                    timeTableTheoryDetail.StartTime = Convert.ToDateTime(timeTableTrx.VAR_START_TIME);
                    timeTableTheoryDetail.EndTime = Convert.ToDateTime(timeTableTrx.VAR_TO_TIME);
                }

                var cloned = timeTableTheoryDetail.DeepClone();
                cloned.SubjectDivisionCode = subjectDivisionCode;
                cloned.SubjectName += " (Paper " + subjectDivisionCode + ")";

                model.TimeTableTheoryDetails.Add(cloned);
            }
            return;
        }

        model.TimeTableTheoryDetails.Add(timeTableTheoryDetail);
    }

    protected override TimeTable GetExistingTimeTable(TimeTable model)
    {
        var timeTable = base.GetExistingTimeTable(model);
        if (null == timeTable) return null;

        if (timeTable.TimeTableTheoryDetails.Count <= 0)
            GetSubjects(timeTable);

        foreach (var detail in timeTable.TimeTableTheoryDetails)
        {
            detail.SubjectName = ExamServerHelper.GetSubjectName(detail.SubjectId, _examService);
            if ((detail.SubjectDivisionCode ?? 0) > 0)
                detail.SubjectName += " (Paper " + (detail.SubjectDivisionCode ?? 0) + ")";
            detail.CategoryName = ExamServerHelper.GetCategoryShortName(detail.CategoryCode, _examService);
        }

        return timeTable;
    }

    protected override void EditTimeTableInExamServer(TimeTable model)
    {
        foreach (var detail in model.TimeTableTheoryDetails)
        {
            // ToList() instead of FirstOrDefault is only for common subjects.
            var existingList = _examService.Tbl_TIMETABLE_TRX_Repository.Get(t => t.Num_FK_INST_NO == model.InstanceId &&
                    t.Num_FK_COPRT_NO == model.CoursePartId &&
                    t.Num_FK_PH_CD == detail.SubjectId &&
                    t.Num_FK_CAT_CD == detail.CategoryCode &&
                    (t.Num_FK_PP_CD ?? 0) == (detail.PaperCode ?? 0) &&
                    (t.NUM_FK_SUB_DIV_CD ?? 0) == (detail.SubjectDivisionCode ?? 0))
                .ToList();
            if (existingList.Count <= 0)
            {
                AddTimeTableInExamServer(model);
                continue;
            }

            foreach (var existing in existingList)
            {
                existing.Dtm_TBM_FROM_TIME = detail.StartDate; // ?? new DateTime(1900, 01, 01);
                existing.Dtm_TBM_TO_TIME = null; //new DateTime(1900, 01, 01);
                existing.Dtm_DTE_UP = DateTime.Now;
                existing.VAR_START_TIME = detail.StartTime?.ToString("hh:mm tt");
                existing.VAR_TO_TIME = detail.EndTime?.ToString("hh:mm tt");

                _examService.Tbl_TIMETABLE_TRX_Repository.Update(existing);
            }
        }
        //_examService.Save();
    }

    private void ValidateTime(TimeTable model)
    {
        var banTimeStart = new TimeSpan(22, 30, 0);
        var banTimeEnd = new TimeSpan(7, 0, 0);
        foreach (var detail in model.TimeTableTheoryDetails)
        {
            //throw new Exception($"{detail.StartTime?.TimeOfDay}, " +
            //                    $"{new TimeSpan(20, 0, 0)}");

            if (detail.StartDate <= DateTime.Today)
                throw new Exception("Start date should be later date than today.");
            if (detail.EndTime - detail.StartTime > new TimeSpan(4, 30, 0))
                throw new Exception("Exam time should not be greater than 4:30 hours.");
            if(detail.StartTime?.TimeOfDay > banTimeStart || detail.StartTime?.TimeOfDay < banTimeEnd)
                throw new Exception("Start Time should not be between 10:30 PM to 7 AM.");
            if (detail.EndTime?.TimeOfDay > banTimeStart || detail.EndTime?.TimeOfDay < banTimeEnd)
                throw new Exception("End Time should not be between 10:30 PM to 7 AM.");
        }
    }
    #endregion

    #region -- Actions --
    // GET: /Reg/Create
    [Authorize]
    public ActionResult Create()
    {
        return View(new TimeTable());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public ActionResult Create(TimeTable model, string submitType)
    {
        try
        {
            Session[ModelConstants.Password] = "bvdu";
            model.InstanceId = (int)HttpContext.Session[ModelConstants.InstanceId];
            if (submitType == "Search")
            {
                // Check if already exists in the system
                var existing = GetExistingTimeTable(model);
                if (null != existing)
                {
                    ModelState.Clear();
                    return View(existing);
                }

                // Get new subjects.
                model.TimeTableTheoryDetails?.Clear();
                GetSubjects(model);
                ModelState.Clear();
                return View(model);
            }

            if (ModelState.IsValid)
            {
                // Validations
                if (model.TimeTableTheoryDetails.Count(t => t.StartDate != null && t.StartTime != null && t.EndTime != null) <= 0)
                    throw new Exception("At least one subject should be have time filled.");
                if (model.TimeTableTheoryDetails.Count(t => t.StartTime >= t.EndTime) > 0)
                    throw new Exception("Start Time cannot be greater than End Time.");

                // Check time table for freeze;
                CheckForTimeTableFreeze(model);

                // Check subject for date & time clash
                CheckSubjectClashes(model);

                ValidateTime(model);

                //using var scope = new TransactionScope(TransactionScopeOption.RequiresNew, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout });
                try
                {
                    // Check if already exists in the system
                    var existing = GetExistingTimeTable(model);
                            
                    if (null != existing)
                    {
                        // Update existing details.
                        UpdateUserDetails(existing);

                        //var id = existing.Id;
                        //model.CopyPropertiesTo(existing);
                        //existing.Id = id;
                        Logger.LogHandler.LogInfo($"Instance : {existing.InstanceId}," +
                                                  $" Detail Count : {existing.TimeTableTheoryDetails.Count}");

                                
                        foreach (var detail in existing.TimeTableTheoryDetails)
                        {
                            var newDetail = model.TimeTableTheoryDetails.FirstOrDefault(t => t.SubjectId == detail.SubjectId &&
                                t.CategoryCode == detail.CategoryCode &&
                                t.SubjectDivisionCode == detail.SubjectDivisionCode);
                            if (newDetail == null)
                                continue;

                            detail.StartDate = newDetail.StartDate;
                            detail.StartTime = newDetail.StartTime;
                            detail.EndTime = newDetail.EndTime;

                            // Common Subjects
                            var commonSubjectList = _transactionService.TimeTableTheoryDetailRepository.Get(t => t.SubjectId == detail.SubjectId &&
                                    t.CategoryCode == detail.CategoryCode &&
                                    t.SubjectDivisionCode == detail.SubjectDivisionCode)
                                .ToList();
                            foreach (var commonSubject in commonSubjectList)
                            {
                                var currentInstanceTimeTable = _transactionService.TimeTableRepository
                                    .Get(t => t.Id == commonSubject.TimeTableId && t.InstanceId == existing.InstanceId)
                                    .FirstOrDefault();

                                if(null == currentInstanceTimeTable) continue;

                                if (null == newDetail.StartDate && null == newDetail.EndDate)
                                    continue;
                                commonSubject.StartDate = newDetail.StartDate;
                                commonSubject.StartTime = newDetail.StartTime;
                                commonSubject.EndTime = newDetail.EndTime;

                                _transactionService.TimeTableTheoryDetailRepository.Update(commonSubject);
                            }
                        }

                        existing.FacultyId = 1;
                        _transactionService.TimeTableRepository.Update(existing);
                        _transactionService.Save();

                        // Edit Student To Exam Server
                        EditTimeTableInExamServer(model);
                        // Add or update entry in schedule 
                        AddUpdateScheduleInExamServer(model);

                        _examService.Save();

                        //scope.Complete();
                        TempData["Success"] = "Theory Time Table Updated Successfully";
                    }
                    else
                    {
                        //ValidateTime(model);

                        UpdateUserDetails(model, true);

                        _transactionService.TimeTableRepository.Add(model);
                        _transactionService.Save();

                        // Add Student To Exam Server
                        AddTimeTableInExamServer(model);
                        // Add or update entry in schedule 
                        AddUpdateScheduleInExamServer(model);

                        _examService.Save();

                        //scope.Complete();
                        TempData["Success"] = "Theory Time Table Added Successfully";
                    }

                    return RedirectToAction("Create");
                }
                catch(Exception exception)
                {
                    //scope.Dispose();
                    LogHandler.LogError(exception);
                    throw;
                }
            }
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(model);
    }
    #endregion
}