using Corno.Globals.Constants;
using Corno.OnlineExam.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Corno.Data.Core;
using Corno.Data.Corno;
using Corno.Globals.Enums;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;
using Microsoft.AspNet.Identity;
using MoreLinq;

namespace Corno.OnlineExam.Areas.Transactions.Controllers;

[Authorize]
public class TimeTableController : BaseController
{
    #region -- Constructors --
    public TimeTableController(ICornoService transactionService, ICoreService examService)
    {
        _transactionService = transactionService;
        _examService = examService;
    }
    #endregion

    #region -- Data Members --

    private readonly ICornoService _transactionService;
    private readonly ICoreService _examService;

    #endregion

    #region -- Private Methods --
    private void CheckClashesInCompulsory(TimeTable timeTable, Tbl_COPART_SYLLABUS syllabus)
    {

        var theoryCategories = ExamServerHelper.GetTheoryCategories();
        var syllabusSubjects = (from syllabusSubject in _examService.Tbl_COPART_SYLLABUS_TRX_Repository.Get(
                    st => st.Num_FK_SYL_NO == syllabus.Num_PK_SYL_NO &&
                          st.Chr_SUB_CMP_OPT_FLG == "C" &&
                          st.Chr_DELETE_FLG != "Y").ToList()
                join timeTableTheoryDetail in timeTable.TimeTableTheoryDetails.Where(t => t.StartDate != null &&
                        theoryCategories.Contains(t.CategoryCode ?? 0))
                    on syllabusSubject.Num_FK_SUB_CD equals timeTableTheoryDetail.SubjectId
                select timeTableTheoryDetail)
            .ToList();

        var duplicates = syllabusSubjects.GroupBy(g => new { g.StartDate, g.StartTime })
            .Where(g => g.Count() > 1)
            .Select(g => g.Key)
            .ToList();
        if (duplicates.Count > 0)
            throw new Exception("Date clashes in subjects (Date : " +
                                duplicates.FirstOrDefault()?.StartDate?.ToString("dd/MM/yyyy") +
                                duplicates.FirstOrDefault()?.StartTime?.ToString("hh:mm") +
                                ")");
    }

    private List<TimeTableTheoryDetail> GetAllSyllabusSubjects(TimeTable timeTable, IEnumerable<Tbl_COPART_SYLLABUS> syllabuses)
    {
        var allSubjects = new List<TimeTableTheoryDetail>();
        var theoryCategories = ExamServerHelper.GetTheoryCategories();
        foreach (var syllabus in syllabuses)
        {
            var syllabusSubjects = (from syllabusSubject in _examService.Tbl_COPART_SYLLABUS_TRX_Repository.Get(
                        st => st.Num_FK_SYL_NO == syllabus.Num_PK_SYL_NO &&
                              st.Chr_DELETE_FLG != "Y").ToList()
                    join timeTableTheoryDetail in timeTable.TimeTableTheoryDetails.Where(t => t.StartDate != null &&
                            theoryCategories.Contains(t.CategoryCode ?? 0))
                        on syllabusSubject.Num_FK_SUB_CD equals timeTableTheoryDetail.SubjectId
                    select timeTableTheoryDetail)
                .ToList();

            allSubjects.AddRange(syllabusSubjects);
        }

        return allSubjects;
    }
    #endregion

    #region -- Protected Methods --
    [Authorize]
    protected virtual void AddSubject(TimeTable model, Tbl_SUBJECT_MSTR subject, int categoryCode, int? paperCode,
        string categoryName, string subjectType, string CHR_WRT_UNI_APL)
    {
            
    }

    protected void GetSubjects(TimeTable model)
    {
        model ??= new TimeTable();

        var timeTableBranchAppFlag = ExamServerHelper.GetCoursePart(model.CoursePartId, _examService)?
            .Chr_BRANCH_TIMETABLE_APP_FLG;

        IEnumerable<Tbl_COPART_SYLLABUS> syllabuses;
        if (timeTableBranchAppFlag == "Y")
        {
            syllabuses = _examService.Tbl_COPART_SYLLABUS_Repository.Get(
                s => s.Num_FK_COPRT_NO == model.CoursePartId &&
                     s.Num_FK_BR_CD == (model.BranchId ?? 0) && s.Chr_DELETE_FLG != "Y");
        }
        else
        {
            syllabuses = _examService.Tbl_COPART_SYLLABUS_Repository.Get(
                s => s.Num_FK_COPRT_NO == model.CoursePartId && s.Chr_DELETE_FLG != "Y");
        }

        if (null == syllabuses)
            return;

        // Get subjects from for selected syllabus
        var optionalCount = 0;
        foreach (var syllabus in syllabuses)
        {
            var syllabusSubjects =
                _examService.Tbl_COPART_SYLLABUS_TRX_Repository.Get(
                    st => st.Num_FK_SYL_NO == syllabus.Num_PK_SYL_NO && st.Chr_DELETE_FLG != "Y").ToList();

            if (syllabusSubjects.Count(s => s.Chr_SUB_CMP_OPT_FLG != "C") > 0)
                optionalCount++;

            foreach (var syllabusSubject in syllabusSubjects)
            {
                var subjectType = syllabusSubject.Chr_SUB_CMP_OPT_FLG == "C"
                    ? SubjectType.Compulsory.ToString()
                    : SubjectType.Optional + " " + optionalCount;

                var subject = _examService.Tbl_SUBJECT_MSTR_Repository
                    .Get(s => s.Num_FK_COPRT_NO == model.CoursePartId &&
                              s.Num_PK_SUB_CD == syllabusSubject.Num_FK_SUB_CD)
                    .FirstOrDefault();
                if (null == subject)
                    continue;

                var subjectCategories =
                    _examService.Tbl_SUBJECT_CAT_MSTR_Repository.Get(c =>
                            c.Num_FK_COPRT_NO == subject.Num_FK_COPRT_NO &&
                            c.Num_FK_SUB_CD == syllabusSubject.Num_FK_SUB_CD)
                        .ToList();

                if (subjectCategories.Count <= 0) continue;

                foreach (var subjectCategory in subjectCategories)
                {
                    if (subjectCategory.chr_CAT_SUBCAT_APL == "Y")
                    {
                        var papers = _examService.Tbl_SUB_CATPAP_MSTR_Repository.Get(c =>
                                c.Num_FK_COPRT_NO == subject.Num_FK_COPRT_NO && c.Num_FK_SUB_CD == subject.Num_PK_SUB_CD &&
                                c.Num_FK_CAT_CD == subjectCategory.Num_FK_CAT_CD)
                            .ToList();

                        foreach (var paper in papers)
                        {
                            // Get Category Code.
                            var evalCatMaster =
                                _examService.Tbl_EVALCAT_MSTR_Repository.Get(
                                    e => e.Num_PK_CAT_CD == subjectCategory.Num_FK_CAT_CD).FirstOrDefault();
                            if (null == evalCatMaster) continue;

                            //if (paper.CHR_WRT_UNI_APL == "Y" || practicalCategories.Contains(paper.Num_FK_CAT_CD))
                            //    AddSubject(model, subject, evalCatMaster.Num_PK_CAT_CD, paper.Num_PK_PAP_CD,
                            //        evalCatMaster.Var_CAT_SHRT_NM, subjectType);
                            AddSubject(model, subject, evalCatMaster.Num_PK_CAT_CD, paper.Num_PK_PAP_CD,
                                evalCatMaster.Var_CAT_SHRT_NM, subjectType, paper.CHR_WRT_UNI_APL);
                        }
                    }
                    else
                    {
                        // Get Category Code.
                        var evalCatMaster =
                            _examService.Tbl_EVALCAT_MSTR_Repository.Get(
                                e => e.Num_PK_CAT_CD == subjectCategory.Num_FK_CAT_CD).FirstOrDefault();
                        if (null == evalCatMaster) continue;

                        //if (subjectCategory.CHR_WRT_UNI_APL == "Y" || practicalCategories.Contains(subjectCategory.Num_FK_CAT_CD))
                        //    AddSubject(model, subject, evalCatMaster.Num_PK_CAT_CD, null,
                        //        evalCatMaster.Var_CAT_SHRT_NM, subjectType);
                        AddSubject(model, subject, evalCatMaster.Num_PK_CAT_CD, null,
                            evalCatMaster.Var_CAT_SHRT_NM, subjectType, subjectCategory.CHR_WRT_UNI_APL);
                    }
                }
            }
        }
    }

    protected void AddTimeTableInExamServer(TimeTable model)
    {
        if (null == model.CoursePartId)
            throw new Exception("Invalid course part.");

        var instanceId = Convert.ToInt32(HttpContext.Session[ModelConstants.InstanceId].ToString());
        foreach (var detail in model.TimeTableTheoryDetails.OrderBy(d => d.SubjectId))
        {
            var found = _examService.Tbl_TIMETABLE_TRX_Repository.Get(t => t.Num_FK_INST_NO == model.InstanceId &&
                                                                           t.Num_FK_COPRT_NO == model.CoursePartId &&
                                                                           t.Num_FK_PH_CD == detail.SubjectId &&
                                                                           t.Num_FK_CAT_CD == detail.CategoryCode &&
                                                                           (t.Num_FK_PP_CD ?? 0) == (detail.PaperCode ?? 0) &&
                                                                           (t.NUM_FK_SUB_DIV_CD ?? 0) == (detail.SubjectDivisionCode ?? 0))
                .FirstOrDefault();
            if (null != found)
                continue;

            var timeTableTrx = new Tbl_TIMETABLE_TRX
            {
                Num_FK_COPRT_NO = (short)(model.CoursePartId ?? 0),
                Num_FK_Course_No = (short)(model.CourseId ?? 0),
                Num_FK_PH_CD = detail.SubjectId ?? 0,
                Num_FK_CAT_CD = (short)(detail.CategoryCode ?? 0),
                Num_FK_PP_CD = detail.PaperCode ?? 0,
                NUM_FK_SUB_DIV_CD = (short)(detail.SubjectDivisionCode ?? 0),
                Dtm_TBM_FROM_TIME = detail.StartDate, // ?? new DateTime(1900, 01, 01),
                Dtm_TBM_TO_TIME = null,
                Num_FK_INST_NO = (short)instanceId,
                Var_USR_NM = User.Identity.Name,
                Dtm_DTE_CR = DateTime.Now,
                Dtm_DTE_UP = DateTime.Now,
                VAR_START_TIME = detail.StartTime?.ToString("hh:mm tt"),
                VAR_TO_TIME = detail.EndTime?.ToString("hh:mm tt")
            };

            _examService.Tbl_TIMETABLE_TRX_Repository.Add(timeTableTrx);
            _examService.Save();
        }
    }

    protected void AddUpdateScheduleInExamServer(TimeTable model)
    {
        if (null == model.CoursePartId)
            throw new Exception("Invalid course part.");

        var bFound = true;
        var instanceId = Convert.ToInt32(HttpContext.Session[ModelConstants.InstanceId].ToString());
        var firstDateRecord = model.TimeTableTheoryDetails.Where(t => t.StartDate != null)
            .OrderBy(t => t.StartDate).FirstOrDefault();
        var lastDateRecord = model.TimeTableTheoryDetails.Where(t => t.StartDate != null)
            .OrderByDescending(t => t.StartDate).FirstOrDefault();

        var schedule = _examService.Tbl_EXAM_SCHEDULE_MSTR_Repository.Get(t => t.Num_FK_INST_NO == model.InstanceId &&
                                                                               t.Num_FK_COPRT_NO == model.CoursePartId &&
                                                                               (t.Num_FK_BR_NO) == (model.BranchId ?? 0))
            .FirstOrDefault();

        if (null == schedule)
        {
            schedule = new Tbl_EXAM_SCHEDULE_MSTR();
            bFound = false;
        }

        schedule.Num_FK_INST_NO = (short)instanceId;
        schedule.Num_FK_COURSE_NO = (short)(model.CourseId ?? 0);
        schedule.Num_FK_COPRT_NO = (short)(model.CoursePartId ?? 0);
        schedule.Num_FK_BR_NO = (short)(model.BranchId ?? 0);
        schedule.Dtm_Commencement_DateOfExam = firstDateRecord?.StartDate;
        /*schedule.Dtm_LstDateOfForm_WithoutLateFee = firstDateRecord?.StartDate?.AddDays(-42);
        schedule.Dtm_LstDateOfForm_WithLateFee = firstDateRecord?.StartDate?.AddDays(-21);
        schedule.Dtm_LstDateOfForm_WithSuperLateFee = firstDateRecord?.StartDate?.AddDays(-5);*/
        schedule.Dtm_LstDateOfForm_WithoutLateFee = firstDateRecord?.StartDate?.AddDays(-21);
        schedule.Dtm_LstDateOfForm_WithLateFee = firstDateRecord?.StartDate?.AddDays(-11);
        schedule.Dtm_LstDateOfForm_WithSuperLateFee = firstDateRecord?.StartDate?.AddDays(-5);

        schedule.Num_Exam_Fees = 0;
        schedule.Var_Exam_Fee_Narration = string.Empty;

        schedule.Dtm_Result_Date = lastDateRecord?.StartDate?.AddDays(45);
        schedule.Dtm_Reval_Date = lastDateRecord?.StartDate?.AddDays(60);
        schedule.Dtm_Reval_Res_Date = lastDateRecord?.StartDate?.AddDays(75);

        schedule.Dtm_FormFilll_Date = firstDateRecord?.StartDate?.AddDays(-84);
        schedule.Dtm_ConclusionofExam_Date = lastDateRecord?.StartDate;
        schedule.Dtm_StartofCAP_Date = firstDateRecord?.StartDate?.AddDays(3);
        schedule.Dtm_CompletionofCAP_Date = lastDateRecord?.StartDate?.AddDays(3);
        schedule.Dtm_MarksReceive_Date = lastDateRecord?.StartDate?.AddDays(9);
            
            
        schedule.Var_USR_NM = User.Identity.Name;
        schedule.Dtm_DTE_CR = DateTime.Now;
        schedule.Dtm_DTE_UP = DateTime.Now;

        if (bFound == false)
        {
            //schedule.Num_PK_Sr_NO =
            //    (short) (_examService.Tbl_EXAM_SCHEDULE_MSTR_Repository.Get().Max(s => s.Num_PK_Sr_NO) + 1);
            _examService.Tbl_EXAM_SCHEDULE_MSTR_Repository.Add(schedule);
        }
        else
            _examService.Tbl_EXAM_SCHEDULE_MSTR_Repository.Update(schedule);
        //_examService.Save();
    }

    protected virtual void EditTimeTableInExamServer(TimeTable model)
    {
        foreach (var detail in model.TimeTableTheoryDetails)
        {
            var existing = _examService.Tbl_TIMETABLE_TRX_Repository.Get(t => t.Num_FK_INST_NO == model.InstanceId &&
                                                                              t.Num_FK_COPRT_NO == model.CoursePartId &&
                                                                              t.Num_FK_PH_CD == detail.SubjectId &&
                                                                              t.Num_FK_CAT_CD == detail.CategoryCode &&
                                                                              (t.Num_FK_PP_CD ?? 0) == (detail.PaperCode ?? 0) &&
                                                                              (t.NUM_FK_SUB_DIV_CD ?? 0) == (detail.SubjectDivisionCode ?? 0))
                .FirstOrDefault();
            if (null == existing)
                continue;

            existing.Dtm_TBM_FROM_TIME = detail.StartDate;// ?? new DateTime(1900, 01, 01);
            existing.Dtm_TBM_TO_TIME = null;//new DateTime(1900, 01, 01);
            existing.Dtm_DTE_UP = DateTime.Now;
            existing.VAR_START_TIME = detail.StartTime?.ToString("hh:mm tt");
            existing.VAR_TO_TIME = detail.EndTime?.ToString("hh:mm tt");

            _examService.Tbl_TIMETABLE_TRX_Repository.Update(existing);
        }
        _examService.Save();
    }

    protected virtual TimeTable GetExistingTimeTable(TimeTable model)
    {
        var timeTable = _transactionService.TimeTableRepository.Get(t => t.InstanceId == model.InstanceId &&
                                                                         //t.CollegeId == model.CollegeId &&
                                                                         //t.CentreId == model.CentreId &&
                                                                         t.CourseId == model.CourseId &&
                                                                         (t.CoursePartId ?? 0) == (model.CoursePartId ?? 0) &&
                                                                         (t.BranchId ?? 0) == (model.BranchId ?? 0),
            null, "TimeTableTheoryDetails, TimeTablePracticalDetails, TimeTableCoursePartDetails").FirstOrDefault();

        return timeTable;

        //foreach (var detail in timeTable.TimeTableTheoryDetails)
        //{
        //    detail.SubjectName = ExamServerHelper.GetSubjectName(detail.SubjectId, _examService);
        //    if ((detail.SubjectDivisionCode ?? 0) > 0)
        //        detail.SubjectName += " (Paper " + (detail.SubjectDivisionCode ?? 0) + ")";
        //    detail.CategoryName = ExamServerHelper.GetCategoryShortName(detail.CategoryCode, _examService);
        //}
    }

    protected void UpdateUserDetails(TimeTable timeTable, bool bAdd = false)
    {
        var userId = User.Identity.GetUserId();
        if (bAdd)
        {
            timeTable.CreatedBy = userId;
            timeTable.CreatedDate = DateTime.Now;

            timeTable.TimeTableTheoryDetails.ForEach(t => t.CreatedBy = userId);
            timeTable.TimeTableTheoryDetails.ForEach(t => t.CreatedDate = DateTime.Now);
        }

        timeTable.ModifiedBy = userId;
        timeTable.ModifiedDate = DateTime.Now;

        timeTable.TimeTableTheoryDetails.ForEach(t => t.ModifiedBy = userId);
        timeTable.TimeTableTheoryDetails.ForEach(t => t.ModifiedDate = DateTime.Now);
    }

        

    protected void CheckForTimeTableFreeze(TimeTable timeTable)
    {
        // Check whether time table is freeze
        var timeTableFreeze = _examService.Tbl_TimeTableINST_Repository.Get(t =>
                t.Num_FK_INST_NO == timeTable.InstanceId &&
                t.Num_PK_CO_CD == timeTable.CourseId)
            .FirstOrDefault();
        var course = _examService.Tbl_COURSE_MSTR_Repository.Get(c =>
            c.Num_PK_CO_CD == timeTable.CourseId).FirstOrDefault();

        if (null == timeTableFreeze)
        {
            timeTableFreeze = new Tbl_TimeTableINST
            {
                Num_PK_CO_CD = (short)(timeTable.CourseId ?? 0),
                Num_FK_INST_NO = (short)(timeTable.InstanceId ?? 0),
                Num_FK_FA_CD = course?.Num_FK_FA_CD,
                Num_FK_TYP_CD = course?.Num_FK_TYP_CD,
                Var_CO_NM = course?.Var_CO_NM,
                Chr_FreezeTimeTable = "N",
                Chr_FreezeConvocation = "Y"
            };
            _examService.Tbl_TimeTableINST_Repository.Add(timeTableFreeze);
            _examService.Save();
        }

        if (null != timeTableFreeze && timeTableFreeze.Chr_FreezeTimeTable == "Y")
            throw new Exception("Time Table filling is frozen. Not allowed to fill it.");
    }

    protected void CheckSubjectClashes(TimeTable timeTable)
    {
        var syllabuses = _examService.Tbl_COPART_SYLLABUS_Repository.Get(
                s => s.Num_FK_COPRT_NO == timeTable.CoursePartId &&
                     s.Num_FK_BR_CD == (timeTable.BranchId ?? 0) && s.Chr_DELETE_FLG != "Y")
            .ToList();
        if (syllabuses.Count <= 0)
            return;

        var allSubjects = GetAllSyllabusSubjects(timeTable, syllabuses);
        var theoryCategories = ExamServerHelper.GetTheoryCategories();
        // Get subjects from for selected syllabus
        foreach (var syllabus in syllabuses)
        {
            if (syllabus.Num_COMPL_SUB > 0)
                CheckClashesInCompulsory(timeTable, syllabus);

            var syllabusSubjects = (from syllabusSubject in _examService.Tbl_COPART_SYLLABUS_TRX_Repository.Get(
                        st => st.Num_FK_SYL_NO == syllabus.Num_PK_SYL_NO &&
                              st.Chr_DELETE_FLG != "Y").ToList()
                    join timeTableTheoryDetail in timeTable.TimeTableTheoryDetails.Where(t => t.StartDate != null &&
                            theoryCategories.Contains(t.CategoryCode ?? 0))
                        on syllabusSubject.Num_FK_SUB_CD equals timeTableTheoryDetail.SubjectId
                    select timeTableTheoryDetail)
                .ToList();
            foreach (var subject in syllabusSubjects)
            {
                var existing = allSubjects.FirstOrDefault(s => s.StartDate == subject.StartDate &&
                                                               s.StartTime == subject.StartTime &&
                                                               s.SubjectType != subject.SubjectType);
                if (null != existing)
                    throw new Exception("Date clashes in subjects (Date : " +
                                        existing.StartDate?.ToString("dd/MM/yyyy") +
                                        existing.StartTime?.ToString("hh:mm") + ")");
            }
        }
    }
    #endregion
}