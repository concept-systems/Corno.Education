using Corno.Globals.Constants;
using System;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using Corno.Data.Core;
using Corno.Data.Corno;
using Corno.Globals.Enums;
using Corno.Logger;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;

namespace Corno.OnlineExam.Areas.Transactions.Controllers;

[Authorize]
public class TimeTablePracticalController : TimeTableController
{
    #region -- Constructors --
    public TimeTablePracticalController(ICornoService transactionService, ICoreService examService)
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
    [Authorize]
    protected override void AddSubject(TimeTable model, Tbl_SUBJECT_MSTR subject, int categoryCode, int? paperCode,
        string categoryName, string subjectType, string CHR_WRT_UNI_APL)
    {
        if (subject == null) return;

        // If in practical categories, then only add.
        var practicalCategories = ExamServerHelper.GetPracticalCategories();
        if (!practicalCategories.Contains(categoryCode)) return;
        if (model.TimeTablePracticalDetails.FirstOrDefault(t => t.SubjectId == subject.Num_PK_SUB_CD) != null)
            return;

        var timeTablePracticalDetail = new TimeTablePracticalDetail()
        {
            SubjectId = subject.Num_PK_SUB_CD,
            SubjectName = subject.Var_SUBJECT_NM,
            CategoryName = categoryName,
            CategoryCode = categoryCode,
            PaperCode = paperCode,
            SubjectType = subjectType,
            SubjectDivisionCode = 0
        };

        var existing = _examService.Tbl_TIMETABLE_TRX_Repository.Get(t => t.Num_FK_INST_NO == model.InstanceId &&
                                                                          t.Num_FK_PH_CD == subject.Num_PK_SUB_CD &&
                                                                          t.Num_FK_COPRT_NO ==
                                                                          (model.CoursePartId ?? 0) &&
                                                                          t.Num_FK_CAT_CD ==
                                                                          timeTablePracticalDetail.CategoryCode &&
                                                                          (t.Num_FK_PP_CD ?? 0) ==
                                                                          (timeTablePracticalDetail.PaperCode ??
                                                                           0) &&
                                                                          (t.NUM_FK_SUB_DIV_CD ?? 0) ==
                                                                          (timeTablePracticalDetail
                                                                              .SubjectDivisionCode ?? 0))
            .FirstOrDefault();
        if (existing?.VAR_START_TIME != null && null != existing.VAR_TO_TIME)
        {
            timeTablePracticalDetail.StartDate = existing.Dtm_TBM_FROM_TIME;
            timeTablePracticalDetail.StartTime = Convert.ToDateTime(existing.VAR_START_TIME);
            timeTablePracticalDetail.EndTime = Convert.ToDateTime(existing.VAR_TO_TIME);
        }

        // Subject Division.
        var practicalRepeatCount = subject.Num_Practical_Repeat ?? 0;
        if (practicalRepeatCount > 1 &&
            model.TimeTablePracticalDetails.Count(t => t.SubjectId == subject.Num_PK_SUB_CD) < practicalRepeatCount)
        {
            for (var index = 0; index < practicalRepeatCount; index++)
            {
                var cloned = timeTablePracticalDetail.DeepClone();
                cloned.SubjectDivisionCode = index + 1;
                //cloned.SubjectName += " (Paper " + (index + 1) + ")";

                model.TimeTablePracticalDetails.Add(cloned);
            }
            return;
        }

        model.TimeTablePracticalDetails.Add(timeTablePracticalDetail);
    }

    protected override TimeTable GetExistingTimeTable(TimeTable model)
    {
        var timeTable = _transactionService.TimeTableRepository.Get(t => t.InstanceId == model.InstanceId &&
                                                                         t.CollegeId == model.CollegeId &&
                                                                         t.CentreId == model.CentreId &&
                                                                         t.CourseId == model.CourseId &&
                                                                         (t.CoursePartId ?? 0) == (model.CoursePartId ?? 0) &&
                                                                         (t.BranchId ?? 0) == (model.BranchId ?? 0),
            null, "TimeTableTheoryDetails, TimeTablePracticalDetails, TimeTableCoursePartDetails").FirstOrDefault();
        if (null == timeTable) return null;

        if (timeTable.TimeTablePracticalDetails.Count <= 0)
        {
            //model.TimeTablePracticalDetails.Clear();
            GetSubjects(timeTable);
        }

        foreach (var detail in timeTable.TimeTablePracticalDetails)
        {
            detail.SubjectName = ExamServerHelper.GetSubjectName(detail.SubjectId, _examService);
            //if ((detail.SubjectDivisionCode ?? 0) > 0)
            //    detail.SubjectName += " (Paper " + (detail.SubjectDivisionCode ?? 0) + ")";
            detail.CategoryName = ExamServerHelper.GetCategoryShortName(detail.CategoryCode, _examService);
        }

        return timeTable;
    }

    private void AddOrthoSubject(TimeTable model)
    {
        if (model.CoursePartId != 122) return;

        if (null != model.TimeTablePracticalDetails
                .FirstOrDefault(t => t.SubjectId == 21527))
            return;
        var subject = _examService.Tbl_SUBJECT_MSTR_Repository.Get(s => s.Num_PK_SUB_CD == 21527)
            .FirstOrDefault();
        if (null != subject)
            AddSubject(model, subject, 8, null, "CLINICAL", SubjectType.Compulsory.ToString(), "N");
    }
    #endregion

    #region -- Actions --
    // GET: /Reg/Create
    [Authorize]
    public ActionResult Create(bool isPractical = false)
    {
        return View(new TimeTable()/* {IsPractical = isPractical}*/);
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
                    // Temporary Arrangement
                    AddOrthoSubject(existing);

                    ModelState.Clear();
                    //existing.IsPractical = model.IsPractical;
                    return View(existing);
                }

                // Get new subjects.
                model.TimeTablePracticalDetails.Clear();
                GetSubjects(model);

                // Temporary Arrangement
                AddOrthoSubject(model);

                ModelState.Clear();
                return View(model);
            }

            if (!ModelState.IsValid)
                return View(model);

            // Validations
            if (model.TimeTablePracticalDetails.Count(t => t.StartDate != null && t.StartTime != null) <= 0)
                throw new Exception("At least one subject should be have time filled.");
            if (model.TimeTablePracticalDetails.Count(t => t.StartTime >= t.EndTime) > 0)
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
                    foreach (var detail in existing.TimeTablePracticalDetails)
                    {
                        var newDetail = model.TimeTablePracticalDetails.FirstOrDefault(t => t.SubjectId == detail.SubjectId &&
                            t.CategoryCode == detail.CategoryCode &&
                            t.SubjectDivisionCode == detail.SubjectDivisionCode);
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

                    // Edit Student To Exam Server
                    EditTimeTableInExamServer(model);

                    //scope.Complete();
                    TempData["Success"] = "Practical Time Table Updated Successfully";
                }
                else
                {
                    UpdateUserDetails(model, true);

                    _transactionService.TimeTableRepository.Add(model);
                    _transactionService.Save();

                    // Add Student To Exam Server
                    AddTimeTableInExamServer(model);

                    //scope.Complete();
                    TempData["Success"] = "Practical Time Table Added Successfully";
                }


                return RedirectToAction("Create"/*, model.IsPractical*/);
            }
            catch(Exception exception)
            {
                //scope.Dispose();
                LogHandler.LogError(exception);
                throw;
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