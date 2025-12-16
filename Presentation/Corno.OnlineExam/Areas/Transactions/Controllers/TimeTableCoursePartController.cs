using Corno.Globals.Constants;
using System;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using Corno.Data.Corno;
using Corno.Logger;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;

namespace Corno.OnlineExam.Areas.Transactions.Controllers;

[Authorize]
public class TimeTableCoursePartController : TimeTableController
{
    #region -- Constructors --
    public TimeTableCoursePartController(ICornoService transactionService, ICoreService examService)
        : base(transactionService, examService)
    {
        _transactionService = transactionService;

        _examService = examService;
        //_practicalCategories = new List<int> { 6, 7, 8, 15, 16, 17, 18, 19, 23, 25, 30, 34, 35, 37, 41, 42, 43, 44, 46, 49, 50, 51, 54, 57, 58, 59 };
    }
    #endregion

    #region -- Data Members --

    private readonly ICornoService _transactionService;
    private readonly ICoreService _examService;

    //private readonly List<int> _practicalCategories;
    #endregion

    #region -- Methods --

    protected void GetCourseParts(TimeTable model)
    {
        if (null == model)
            model = new TimeTable();

        var courseParts = _examService.Tbl_COURSE_PART_MSTR_Repository.Get()
            .Where(p => p.Num_FK_CO_CD == model.CourseId).OrderBy(b => b.Num_PK_COPRT_NO)
            .ToList();

        foreach (var coursePart in courseParts)
        {
            model.TimeTableCoursePartDetails.Add(new TimeTableCoursePartDetail()
            {
                CoursePartId = coursePart.Num_PK_COPRT_NO,
                CoursePartName = ExamServerHelper.GetCoursePartName(coursePart.Num_PK_COPRT_NO, _examService)
            });
        }
    }

    protected override TimeTable GetExistingTimeTable(TimeTable model)
    {
        //var timeTable = base.GetExistingTimeTable(model);
        //if (null == timeTable) return null;

        var timeTable = _transactionService.TimeTableRepository.Get(t => t.InstanceId == model.InstanceId &&
                                                                         t.CollegeId == model.CollegeId &&
                                                                         t.CentreId == model.CentreId &&
                                                                         t.CourseId == model.CourseId &&
                                                                         (t.CoursePartId ?? 0) == (model.CoursePartId ?? 0) &&
                                                                         (t.BranchId ?? 0) == (model.BranchId ?? 0),
            null, "TimeTableTheoryDetails, TimeTablePracticalDetails, TimeTableCoursePartDetails").FirstOrDefault();
        if (null == timeTable) return null;

        if (timeTable.TimeTableCoursePartDetails.Count <= 0)
            GetCourseParts(timeTable);

        foreach (var detail in timeTable.TimeTableCoursePartDetails)
            detail.CoursePartName = ExamServerHelper.GetCoursePartName(detail.CoursePartId, _examService);

        return timeTable;

    }
    #endregion

    #region -- Actions --
    // GET: /Reg/Create
    [Authorize]
    public ActionResult Create(bool isPractical = false)
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
                GetCourseParts(model);
                ModelState.Clear();
                return View(model);
            }

            if (ModelState.IsValid)
            {
                // Validations
                if (model.TimeTableCoursePartDetails.Count(t => t.StartDate != null && t.StartTime != null) <= 0)
                    throw new Exception("At least one subject should be have time filled.");
                if (model.TimeTableCoursePartDetails.Count(t => t.EndTime != null && t.StartTime >= t.EndTime) > 0)
                    throw new Exception("Start Time cannot be greater than End Time.");

                //using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout });
                try
                {
                    // Check if already exists in the system
                    var existing = GetExistingTimeTable(model);
                    if (null != existing)
                    {
                        // Update existing details.
                        UpdateUserDetails(model);

                        //var id = existing.Id;
                        //model.CopyPropertiesTo(existing);
                        //existing.Id = id;
                        foreach (var detail in existing.TimeTableCoursePartDetails)
                        {
                            var newDetail = model.TimeTableCoursePartDetails.FirstOrDefault(t => t.CoursePartId == detail.CoursePartId);
                            if (newDetail == null)
                                continue;

                            detail.SubjectGroup = newDetail.SubjectGroup;
                            detail.StartDate = newDetail.StartDate;
                            detail.EndDate = newDetail.EndDate;
                            detail.StartTime = newDetail.StartTime;
                            detail.EndTime = newDetail.EndTime;
                        }


                        _transactionService.TimeTableRepository.Update(existing);
                        _transactionService.Save();

                        //scope.Complete();
                        TempData["Success"] = "Course Part Time Table Updated Successfully";
                    }
                    else
                    {
                        UpdateUserDetails(model, true);

                        _transactionService.TimeTableRepository.Add(model);
                        _transactionService.Save();

                        //scope.Complete();
                        TempData["Success"] = "Course Part Time Table Added Successfully";
                    }


                    return RedirectToAction("Create"/*, model.IsPractical*/);
                }
                catch (Exception exception)
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