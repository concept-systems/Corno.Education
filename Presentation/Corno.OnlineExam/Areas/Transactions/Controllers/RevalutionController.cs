using Corno.Globals.Constants;
using Corno.OnlineExam.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using Corno.Data.Core;
using Corno.Data.Corno;
using Corno.Logger;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;
using MoreLinq;

namespace Corno.OnlineExam.Areas.Transactions.Controllers;

[Authorize]
public class RevaluationController : BaseController
{
    #region -- Construtors --
    public RevaluationController(ICornoService registrationService, ICoreService examService)
    {
        _registrationService = registrationService;
        _examService = examService;
    }
    #endregion

    #region -- Data Members --
    private readonly ICornoService _registrationService;
    private readonly ICoreService _examService;
    #endregion

    #region -- Methods --
    private void GetSeatNoInfo(RevalutionViewModel viewModel)
    {
        var instanceId = (int)HttpContext.Session[ModelConstants.InstanceId];
        var yearChangeRecord = _examService.Tbl_STUDENT_YR_CHNG_Repository.Get(s => s.Num_ST_SEAT_NO == viewModel.SeatNo && s.Num_FK_INST_NO == instanceId).OrderByDescending(s => s.Num_FK_INST_NO).FirstOrDefault();
        if (null == yearChangeRecord)
            throw new Exception("Seat No (" + viewModel.SeatNo + ") not available for this examination.");

        var existingRevaluation = _registrationService.RevaluationRepository.Get(e => e.SeatNo == viewModel.SeatNo).ToList();
        if (existingRevaluation.Count > 0)
            throw new Exception("Revaluation form for this Seat No " + viewModel.SeatNo + " already exists.");

        // Validate for Revaluation date.
        var revalEndDate = ExamServerHelper.GetRevalStartDate(_examService, instanceId, yearChangeRecord.Num_FK_COPRT_NO);
        if (null == revalEndDate)
            throw new Exception("Revaluation End Date is not available for Instance (" + instanceId +
                                ") and Course Part (" + yearChangeRecord.Num_FK_COPRT_NO + ")");

        if (DateTime.Now.Date > revalEndDate.Value.Date)
            throw new Exception("Revaluation End Date (" + revalEndDate.Value.ToString("dd/MM/yyyy") + ") is over for Instance (" + instanceId +
                                ") and Course Part (" + yearChangeRecord.Num_FK_COPRT_NO + ")");

        if (null != yearChangeRecord.Num_FK_DistCenter_ID)
            viewModel.CentreId = (int)yearChangeRecord.Num_FK_DistCenter_ID;

        viewModel.CollegeId = yearChangeRecord.Num_FK_COL_CD;
        viewModel.CoursePartId = yearChangeRecord.Num_FK_COPRT_NO;
        viewModel.PrnNo = yearChangeRecord.Chr_FK_PRN_NO;
        if (yearChangeRecord.Num_FK_BR_CD >= 0)
            viewModel.BranchId = yearChangeRecord.Num_FK_BR_CD;

        viewModel.StudentName = ExamServerHelper.GetStudentName(viewModel.PrnNo, _examService);
        viewModel.CollegeName = ExamServerHelper.GetCollegeName(viewModel.CollegeId, _examService);
        viewModel.CentreName = ExamServerHelper.GetCentreName(viewModel.CentreId, _examService);
        viewModel.CourseName = ExamServerHelper.GetCourseNameFromCoursePartId(viewModel.CoursePartId, _examService);
        viewModel.CourseId = ExamServerHelper.GetCourseId(viewModel.CoursePartId, _examService);
        viewModel.CourseName = ExamServerHelper.GetCourseNameFromCoursePartId(viewModel.CoursePartId, _examService);
        viewModel.CoursePartName = ExamServerHelper.GetCoursePartName(viewModel.CoursePartId, _examService);
        viewModel.BranchName = ExamServerHelper.GetBranchName(viewModel.BranchId, _examService);
        var photo = _examService.Tbl_STUDENT_INFO_ADR_Repository.Get(c => c.Chr_FK_PRN_NO == viewModel.PrnNo).FirstOrDefault();
        if (photo?.Ima_ST_PHOTO != null)
            viewModel.Photo = "data:image;base64, " + Convert.ToBase64String(photo.Ima_ST_PHOTO);
    }

    private void AddSubject(RevalutionViewModel viewModel, Tbl_STUDENT_SUBJECT subject, int categorCode, string categoryName,
        bool showRevaluation, bool showVerification)
    {
        var tblSubjectMstr = _examService.Tbl_SUBJECT_MSTR_Repository.Get(s => s.Num_PK_SUB_CD == subject.Num_FK_SUB_CD && s.Chr_DELETE_FLG != "Y").FirstOrDefault();
        if (tblSubjectMstr != null)
        {
            viewModel.RevalutionSubjectViewModels.Add(new RevalutionSubjectViewModel
            {
                InstanceId = (int)HttpContext.Session[ModelConstants.InstanceId],
                CoursePartId = subject.Num_FK_COPRT_NO,
                CoursePartName = ExamServerHelper.GetCoursePartName(subject.Num_FK_COPRT_NO, _examService),
                SubjectCode = subject.Num_FK_SUB_CD,
                SubjectName = tblSubjectMstr.Var_SUBJECT_NM,
                SubjectCategory = categoryName,
                CategoryCode = categorCode,
                ShowRevaluation = showRevaluation,
                ShowVerification = showVerification
            });
        }
    }

    private void GetSubjects(RevalutionViewModel viewModel)
    {
        var instanceId = (int)HttpContext.Session[ModelConstants.InstanceId];
        viewModel.RevalutionSubjectViewModels.Clear();
        var allSubjects = _examService.Tbl_STUDENT_SUBJECT_Repository
            .Get(s => s.Num_FK_INST_NO == instanceId && s.Chr_FK_PRN_NO == viewModel.PrnNo &&
                      s.Chr_ST_SUB_STS == "P")
            .OrderBy(s => s.Num_FK_COPRT_NO)
            .ThenBy(s => s.Num_FK_SUB_CD);

        // For Theory.
        foreach (var subject in allSubjects)
        {
            var subjectCategories =
                _examService.Tbl_SUBJECT_CAT_MSTR_Repository.Get(c =>
                    c.Num_FK_COPRT_NO == subject.Num_FK_COPRT_NO && c.Num_FK_SUB_CD == subject.Num_FK_SUB_CD).ToList();

            if (subjectCategories.Count <= 0) continue;

            foreach (var subjectCategory in subjectCategories)
            {
                var catMarks = _examService.Tbl_STUDENT_CAT_MARKS_Repository.Get(c =>
                    c.Num_FK_INST_NO == instanceId && c.Var_FK_PRN_NO == viewModel.PrnNo &&
                    c.Num_FK_SUB_CD == subjectCategory.Num_FK_SUB_CD &&
                    c.Num_FK_CAT_CD == subjectCategory.Num_FK_CAT_CD).FirstOrDefault();
                if (null == catMarks || catMarks.Var_ST_PH_STS == "E" || catMarks.Var_ST_PH_STS == "N")
                    continue;

                if (subjectCategory.chr_CAT_SUBCAT_APL == "Y")
                {
                    var papers = _examService.Tbl_SUB_CATPAP_MSTR_Repository.Get(c =>
                            c.Num_FK_COPRT_NO == subject.Num_FK_COPRT_NO && c.Num_FK_SUB_CD == subject.Num_FK_SUB_CD &&
                            c.Num_FK_CAT_CD == subjectCategory.Num_FK_CAT_CD)
                        .ToList();

                    foreach (var paper in papers)
                    {
                        // Get Category Code.
                        var evalCatMaster =
                            _examService.Tbl_EVALCAT_MSTR_Repository.Get(
                                e => e.Num_PK_CAT_CD == subjectCategory.Num_FK_CAT_CD).FirstOrDefault();
                        if (null == evalCatMaster) continue;

                        if (paper.CHR_WRT_UNI_APL == "Y")
                            AddSubject(viewModel, subject, evalCatMaster.Num_PK_CAT_CD, evalCatMaster.Var_CAT_NM, true, true);
                        else
                        if (paper.CHR_WRT_UNI_APL != "Y" && paper.Verification_Accept == "Y")
                            AddSubject(viewModel, subject, evalCatMaster.Num_PK_CAT_CD, evalCatMaster.Var_CAT_NM, false, true);
                    }
                }
                else
                {
                    // Get Category Code.
                    var evalCatMaster =
                        _examService.Tbl_EVALCAT_MSTR_Repository.Get(
                            e => e.Num_PK_CAT_CD == subjectCategory.Num_FK_CAT_CD).FirstOrDefault();
                    if (null == evalCatMaster) continue;

                    if (subjectCategory.CHR_WRT_UNI_APL == "Y")
                        AddSubject(viewModel, subject, evalCatMaster.Num_PK_CAT_CD, evalCatMaster.Var_CAT_NM, true, true);
                    else if (subjectCategory.CHR_WRT_UNI_APL != "Y" && subjectCategory.Verification_Accept == "Y")
                        AddSubject(viewModel, subject, evalCatMaster.Num_PK_CAT_CD, evalCatMaster.Var_CAT_NM, false, true);
                }
            }

            viewModel.RevalutionSubjectViewModels =
                viewModel.RevalutionSubjectViewModels.OrderBy(r => r.SubjectCategory).ToList();
        }
    }

    private void CalculateFee(RevalutionViewModel viewModel)
    {
        var instanceId = (int)HttpContext.Session[ModelConstants.InstanceId];
        foreach (var selectedSubject in viewModel.RevalutionSubjectViewModels)
        {
            selectedSubject.TotalFee = 0;
            if (!selectedSubject.IsRevaluation && !selectedSubject.IsVerification)
                continue;

            if (selectedSubject.IsRevaluation)
            {
                var feeDetail = _examService.Tbl_FEE_DTL_Repository.Get(f => f.Num_FK_COPRT_NO == selectedSubject.CoursePartId &&
                                                                             f.Num_FK_INST_NO == instanceId && f.Num_FK_FEE_CD == 13).FirstOrDefault();
                if (null == feeDetail)
                    throw new Exception("Revaulation fee is not added in Fee Detail for Course Part ID " + selectedSubject.CoursePartId);
                selectedSubject.RevaluationFee = (double?)feeDetail.FEE_AMOUNT;
            }
            if (selectedSubject.IsVerification)
            {
                var feeDetail = _examService.Tbl_FEE_DTL_Repository.Get(f => f.Num_FK_COPRT_NO == selectedSubject.CoursePartId &&
                                                                             f.Num_FK_INST_NO == instanceId && f.Num_FK_FEE_CD == 14).FirstOrDefault();
                if (null == feeDetail)
                    throw new Exception("Verification fee is not added in Fee Detail for Course Part ID " + selectedSubject.CoursePartId);
                selectedSubject.VerficationFee = (double?)feeDetail.FEE_AMOUNT;
            }

            selectedSubject.TotalFee = selectedSubject.RevaluationFee + selectedSubject.VerficationFee;
        }
    }

    private void UpdateFee(TBL_STUDENT_REVALUATION tblStudentRevaluation, RevalutionSubject subject)
    {
        if (IsTheorySubject(subject.CategoryCode))
        {
            tblStudentRevaluation.NUM_REVALUATION_FEE = subject.RevaluationFee;
            tblStudentRevaluation.NUM_VERIFICATION_FEE = subject.VerficationFee;
            tblStudentRevaluation.NUM_TOTAL_FEE = subject.TotalFee;
        }
        else
        {
            tblStudentRevaluation.NUM_REVALUATION_FEE = 0;
            tblStudentRevaluation.NUM_VERIFICATION_FEE = 0;
            tblStudentRevaluation.NUM_TOTAL_FEE = 0;
        }
    }
    #endregion

    #region -- Events --
    // GET: /Revalution/Create
    [Authorize]
    public ActionResult Create()
    {
        var viewModel = new RevalutionViewModel
        {
            RevalutionSubjectViewModels = new List<RevalutionSubjectViewModel>()
        };

        return View(viewModel);
    }

    // POST: /Revalution/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public ActionResult Create(RevalutionViewModel viewModel, string submitType)
    {
        try
        {
            switch (submitType)
            {
                case "Search":
                    GetSeatNoInfo(viewModel);
                    GetSubjects(viewModel);
                    return View(viewModel);
                case "FeeCalculation":
                    GetSeatNoInfo(viewModel);
                    CalculateFee(viewModel);
                    return View(viewModel);
            }

            if (ModelState.IsValid)
            {
                // Check for existing Seat No for Current Instance
                //var seatNo = viewModel.SeatNo;
                if ((viewModel.SeatNo ?? 0) <= 0)
                    throw new Exception("Invalid Seat No");

                var instanceId = (int)HttpContext.Session[ModelConstants.InstanceId];
                var yearChangeRecord = _examService.Tbl_STUDENT_YR_CHNG_Repository.Get(s =>
                        s.Num_ST_SEAT_NO == viewModel.SeatNo &&
                        s.Num_FK_INST_NO == instanceId).OrderByDescending(s => s.Num_FK_INST_NO)
                    .FirstOrDefault();
                if (null == yearChangeRecord)
                    throw new Exception("Seat No (" + viewModel.SeatNo + ") not available for this examination.");

                // Validations
                var selectedSubjects = viewModel.RevalutionSubjectViewModels.Where(r => r.IsRevaluation || r.IsVerification).ToList();
                if (selectedSubjects.Count <= 0)
                    throw new Exception("Please, select at least one subject for revaluation or verification.");

                var coursePartIds = viewModel.RevalutionSubjectViewModels.Where(r => r.IsRevaluation).Select(r => r.CoursePartId).Distinct().ToList();
                foreach (var coursePartId in coursePartIds)
                {
                    var coursePart = _examService.Tbl_COURSE_PART_MSTR_Repository.Get(c => c.Num_PK_COPRT_NO == coursePartId).FirstOrDefault();
                    var revalSubjects = viewModel.RevalutionSubjectViewModels.Where(
                            r => r.IsRevaluation && r.CoursePartId == coursePartId)
                        .DistinctBy(r => r.SubjectCode)
                        .ToList();
                    if (null != coursePart && revalSubjects.Count > coursePart.Num_COPRT_EVAL_NO)
                        throw new Exception("Maximum number of subjects for revaluation is " + coursePart.Num_COPRT_EVAL_NO + " for course part (" + coursePart.Var_COPRT_SHRT_NM + ")");
                }

                var model = AutoMapperConfig.CornoMapper.Map<Revalution>(viewModel);
                model.PrnNo = yearChangeRecord.Chr_FK_PRN_NO;

                var existingRevaluation = _examService.TBL_STUDENT_REVALUATION_Repository.Get(e => e.NUM_FK_PRN_NO == viewModel.PrnNo).ToList();
                if (existingRevaluation.Count > 0)
                    return Json("Revaluation form for this PRN " + viewModel.PrnNo + " already exists.");

                // Calculate fee
                CalculateFee(viewModel);

                var studentCourse = _examService.Tbl_STUDENT_COURSE_Repository.Get(s => s.Chr_FK_PRN_NO == model.PrnNo).FirstOrDefault();
                if (null != studentCourse)
                {
                    if (studentCourse.Num_ST_COLLEGE_CD != null)
                        model.CollegeId = (short)studentCourse.Num_ST_COLLEGE_CD;
                    model.CourseId = studentCourse.Num_FK_CO_CD;
                }
                foreach (var selectedSubject in viewModel.RevalutionSubjectViewModels)
                {
                    if (!selectedSubject.IsRevaluation && !selectedSubject.IsVerification) continue;

                    var subject = new RevalutionSubject
                    {
                        RevalutionId = viewModel.Id,
                        SubjectCode = selectedSubject.SubjectCode,
                        CategoryCode = selectedSubject.CategoryCode,
                        CoursePartId = selectedSubject.CoursePartId,
                        IsRevaluation = selectedSubject.IsRevaluation,
                        IsVerification = selectedSubject.IsVerification,
                        RevaluationFee = selectedSubject.RevaluationFee,
                        VerficationFee = selectedSubject.VerficationFee,
                        TotalFee = selectedSubject.TotalFee
                    };
                    model.RevalutionSubjects.Add(subject);
                }

                /*using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
                {*/
                try
                {
                    _registrationService.RevaluationRepository.Add(model);
                    _registrationService.Save();

                    AddRevaluationInExamServer(model);
                    //scope.Complete();

                    TempData["Success"] = "Revaluation / Verification form submitted successfully.";
                }
                catch (Exception exception)
                {
                    //scope.Dispose();
                    LogHandler.LogError(exception);
                    throw;
                }
                //}

                return RedirectToAction("Create");
            }
        }
        catch (Exception exception)
        {
            LogHandler.LogInfo(exception);
            if (null != exception?.InnerException)
                LogHandler.LogInfo(exception?.InnerException);
            HandleControllerException(exception);
        }

        return View(viewModel);
    }
    #endregion

    #region -- Methods --

    private static bool IsTheorySubject(int? categoryCode)
    {
        if (null == categoryCode) return false;

        return categoryCode == 2 || categoryCode == 10 || categoryCode == 48 ||
               categoryCode == 26;
    }

    private void AddRevaluationInExamServer(Revalution model)
    {
        var instanceId = Convert.ToInt16(HttpContext.Session[ModelConstants.InstanceId].ToString());
        var tblStudentExams = _examService.TBL_STUDENT_EXAMS_Repository.Get(s =>
            s.Num_FK_INST_NO == instanceId && s.Num_ST_SEAT_NO == model.SeatNo).FirstOrDefault();

        if (instanceId <= 0)
            throw new Exception("Invalid Instance Id");
        if (null == tblStudentExams)
            throw new Exception("There is no entry in Student Exams for this seat no.");
        if (string.IsNullOrEmpty(tblStudentExams.Chr_FK_PRN_NO))
            throw new Exception("Invalid PRN No in Student Exams");

        foreach (var subject in model.RevalutionSubjects)
        {
            // New or Update
            var existing =
                _examService.TBL_STUDENT_REVALUATION_Repository.Get(
                    r => r.NUM_FK_PRN_NO == tblStudentExams.Chr_FK_PRN_NO &&
                         r.Num_FK_INST_NO == instanceId && r.Num_FK_COPRT_NO == subject.CoursePartId &&
                         r.NUM_FK_SUB_CD == subject.SubjectCode).FirstOrDefault();
            if (null != existing)
            {
                if (existing.Chr_Reval_UniExmHd != "Y")
                    existing.Chr_Reval_UniExmHd = IsTheorySubject(subject.CategoryCode) ? "Y" : "N";
                if (subject.IsRevaluation)
                    existing.Chr_REVAL_VERI_FLG = "R";
                if (subject.IsVerification)
                    existing.Chr_REVAL_VERI_FLG = "V";
                if (subject.IsVerification && subject.IsRevaluation)
                    existing.Chr_REVAL_VERI_FLG = "R";

                // Update Fee
                UpdateFee(existing, subject);

                _examService.TBL_STUDENT_REVALUATION_Repository.Update(existing);
                _examService.Save();
            }
            else
            {
                // For theory subjects.
                var tblStudentRevaluation = new TBL_STUDENT_REVALUATION
                {
                    NUM_FK_PRN_NO = tblStudentExams.Chr_FK_PRN_NO,
                    Num_FK_INST_NO = instanceId
                };

                if (subject.CoursePartId != null)
                    tblStudentRevaluation.Num_FK_COPRT_NO = (short)subject.CoursePartId;
                tblStudentRevaluation.NUM_FK_SUB_CD = subject.SubjectCode;
                if (subject.IsRevaluation)
                    tblStudentRevaluation.Chr_REVAL_VERI_FLG = "R";
                if (subject.IsVerification)
                    tblStudentRevaluation.Chr_REVAL_VERI_FLG = "V";
                if (subject.IsVerification && subject.IsRevaluation)
                    tblStudentRevaluation.Chr_REVAL_VERI_FLG = "R";

                // Update Fee
                UpdateFee(tblStudentRevaluation, subject);

                // For Therory = "Y" and Practical = "N"
                tblStudentRevaluation.Chr_Reval_UniExmHd = IsTheorySubject(subject.CategoryCode) ? "Y" : "N";
                _examService.TBL_STUDENT_REVALUATION_Repository.Add(tblStudentRevaluation);
                _examService.Save();
            }

            // Check if category is other than thoery subjects.
            if (IsTheorySubject(subject.CategoryCode))
                continue;

            // For practical subjects.
            var tblStudentRevalChild = new TBL_STUDENT_REVAL_CHILD
            {
                NUM_FK_PRN_NO = tblStudentExams.Chr_FK_PRN_NO,
                Num_FK_INST_NO = Convert.ToInt16(HttpContext.Session[ModelConstants.InstanceId].ToString()),
                NUM_FK_SUB_CD = subject.SubjectCode,
                Num_FK_CAT_CD = subject.CategoryCode,
                Num_FK_PAP_CD = 0,
                Num_FK_SEC_CD = 0,
                NUM_REVALUATION_FEE = subject.RevaluationFee,
                NUM_VERIFICATION_FEE = subject.VerficationFee,
                NUM_TOTAL_FEE = subject.TotalFee
            };

            if (subject.CoursePartId != null)
                tblStudentRevalChild.Num_FK_COPRT_NO = (short)subject.CoursePartId;

            _examService.TBL_STUDENT_REVAL_CHILD_Repository.Add(tblStudentRevalChild);
            _examService.Save();
        }
    }

    #endregion
}