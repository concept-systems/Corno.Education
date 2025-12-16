using Corno.Globals.Constants;
using Corno.OnlineExam.Controllers;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using Corno.Data.Core;
using Corno.Data.Corno;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;

namespace Corno.OnlineExam.Areas.Transactions.Controllers;

[Authorize]
public class AnswerSheetController : BaseController
{
    #region -- Construtors --
    public AnswerSheetController(ICornoService transactionService, ICoreService examService)
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
    private void GetSeatNoInfo(AnswerSheet model)
    {
        var instanceId = (int)HttpContext.Session[ModelConstants.InstanceId];
        var yearChangeRecord = _examService.Tbl_STUDENT_YR_CHNG_Repository.Get(s => s.Num_ST_SEAT_NO == model.SeatNo && s.Num_FK_INST_NO == instanceId).OrderByDescending(s => s.Num_FK_INST_NO).FirstOrDefault();
        if (null == yearChangeRecord)
            throw new Exception("Seat No (" + model.SeatNo + ") not available for this examination.");

        var existingAnswerSheet = _transactionService.AnswerSheetRepository.Get(e => e.SeatNo == model.SeatNo).ToList();
        if (existingAnswerSheet.Count > 0)
            throw new Exception("The request for photocopy of seat No(" + model.SeatNo + ") already exists.");

        //if (null != yearChangeRecord.Num_FK_DistCenter_ID)
        model.CentreId = yearChangeRecord.Num_FK_DistCenter_ID;

        model.CollegeId = yearChangeRecord.Num_FK_COL_CD;
        model.CoursePartId = yearChangeRecord.Num_FK_COPRT_NO;
        model.Prn = yearChangeRecord.Chr_FK_PRN_NO;
        //if (yearChangeRecord.Num_FK_BR_CD >= 0)
        model.BranchId = yearChangeRecord.Num_FK_BR_CD;

        model.StudentName = ExamServerHelper.GetStudentName(model.Prn, _examService);
        model.CollegeName = ExamServerHelper.GetCollegeName(model.CollegeId, _examService);
        model.CentreName = ExamServerHelper.GetCentreName(model.CentreId, _examService);
        model.CourseName = ExamServerHelper.GetCourseNameFromCoursePartId(model.CoursePartId, _examService);
        model.CourseId = ExamServerHelper.GetCourseId(model.CoursePartId, _examService);
        model.CourseName = ExamServerHelper.GetCourseNameFromCoursePartId(model.CoursePartId, _examService);
        model.CoursePartName = ExamServerHelper.GetCoursePartName(model.CoursePartId, _examService);
        model.BranchName = ExamServerHelper.GetBranchName(model.BranchId, _examService);
        var photo = _examService.Tbl_STUDENT_INFO_ADR_Repository.Get(c => c.Chr_FK_PRN_NO == model.Prn).FirstOrDefault();
        if (photo?.Ima_ST_PHOTO != null)
            model.Photo = "data:image;base64, " + Convert.ToBase64String(photo.Ima_ST_PHOTO);
    }

    private void AddSubject(AnswerSheet model, Tbl_STUDENT_SUBJECT subject, int categoryCode, string categoryName, bool showIsSelected)
    {
        var tblSubjectMaster = _examService.Tbl_SUBJECT_MSTR_Repository
            .Get(s => s.Num_PK_SUB_CD == subject.Num_FK_SUB_CD && s.Chr_DELETE_FLG != "Y")
            .FirstOrDefault();
        if (tblSubjectMaster != null)
        {
            model.AnswerSheetSubjects.Add(new AnswerSheetSubject
            {
                InstanceId = (int)HttpContext.Session[ModelConstants.InstanceId],
                CoursePartId = subject.Num_FK_COPRT_NO,
                CoursePartName =
                    ExamServerHelper.GetCoursePartName(subject.Num_FK_COPRT_NO, _examService),
                SubjectId = subject.Num_FK_SUB_CD,
                SubjectName = tblSubjectMaster.Var_SUBJECT_NM,
                CategoryCode = categoryCode,
                CategoryName = categoryName,
                ShowIsSelected = showIsSelected,
            });
        }
    }

    private void GetSubjects(AnswerSheet model)
    {
        var instanceId = (int)HttpContext.Session[ModelConstants.InstanceId];
        model.AnswerSheetSubjects.Clear();
        var allSubjects = _examService.Tbl_STUDENT_SUBJECT_Repository.Get(s => s.Num_FK_INST_NO == instanceId
                                                                               && s.Chr_FK_PRN_NO == model.Prn
                                                                               && s.Chr_ST_SUB_STS == "P")
            .OrderBy(s => s.Num_FK_COPRT_NO).ThenBy(s => s.Num_FK_SUB_CD).ToList();

        // For Theory.
        foreach (var subject in allSubjects)
        {
            var subjectCategories =
                _examService.Tbl_SUBJECT_CAT_MSTR_Repository.Get(c =>
                    c.Num_FK_COPRT_NO == subject.Num_FK_COPRT_NO &&
                    c.Num_FK_SUB_CD == subject.Num_FK_SUB_CD &&
                    (c.Num_FK_CAT_CD == 2 || c.Num_FK_CAT_CD == 10 || c.Num_FK_CAT_CD == 26 ||
                     c.Num_FK_CAT_CD == 48)).ToList();

            if (subjectCategories.Count <= 0) continue;

            foreach (var subjectCategory in subjectCategories)
            {
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
                            AddSubject(model, subject, evalCatMaster.Num_PK_CAT_CD, evalCatMaster.Var_CAT_SHRT_NM, true);
                        else if (paper.CHR_WRT_UNI_APL != "Y" && paper.Verification_Accept == "Y")
                            AddSubject(model, subject, evalCatMaster.Num_PK_CAT_CD, evalCatMaster.Var_CAT_SHRT_NM, true);
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
                        AddSubject(model, subject, evalCatMaster.Num_PK_CAT_CD, evalCatMaster.Var_CAT_NM, true);
                    else if (subjectCategory.CHR_WRT_UNI_APL != "Y" && subjectCategory.Verification_Accept == "Y")
                        AddSubject(model, subject, evalCatMaster.Num_PK_CAT_CD, evalCatMaster.Var_CAT_NM, true);
                }
            }

            model.AnswerSheetSubjects = model.AnswerSheetSubjects.OrderBy(r => r.CategoryCode).ToList();
        }
    }

    private void CalculateFee(AnswerSheet model)
    {
        //var instanceId = (int)HttpContext.Session[ModelConstants.InstanceId];
        model.TotalFee = 0;
        foreach (var selectedSubject in model.AnswerSheetSubjects)
        {
            if (!selectedSubject.IsSelected)
                continue;

            //var feeDetail = _examService.Tbl_FEE_DTL_Repository.Get(f => f.Num_FK_COPRT_NO == selectedSubject.CoursePartId &&
            //                    f.Num_FK_INST_NO == instanceId && f.Num_FK_FEE_CD == 13).FirstOrDefault();

            //if (null == feeDetail)
            //    throw new Exception("Total fee is not added in Fee Detail for Course Part ID " + selectedSubject.CoursePartId);

            //selectedSubject.Fee = (double)(feeDetail.FEE_AMOUNT ?? 0);
            //model.TotalFee += (double)(feeDetail.FEE_AMOUNT ?? 0);
            selectedSubject.Fee = 300;
            model.TotalFee += selectedSubject.Fee;
        }
    }

    //private static bool IsTheorySubject(int? categoryCode)
    //{
    //    if (null == categoryCode) return false;

    //    return categoryCode == 2 || categoryCode == 10 || categoryCode == 48 ||
    //           categoryCode == 26;
    //}

    private void ValidateModel(AnswerSheet model)
    {
        if (model.SeatNo < 0)
            throw new Exception("Invalid Seat No");
        if (string.IsNullOrEmpty(model.MobileNo))
            throw new Exception("Invalid Mobile No");
        if (string.IsNullOrEmpty(model.EmailId))
            throw new Exception("Invalid Email");
        var selectedSubjects = model.AnswerSheetSubjects.Where(r => r.IsSelected).ToList();
        if (selectedSubjects.Count <= 0)
            throw new Exception("Please, select at least one subject to submit form.");

        var instanceId = (int)HttpContext.Session[ModelConstants.InstanceId];
        var yearChangeRecord = _examService.Tbl_STUDENT_YR_CHNG_Repository
            .Get(s => s.Num_ST_SEAT_NO == model.SeatNo && s.Num_FK_INST_NO == instanceId)
            .OrderByDescending(s => s.Num_FK_INST_NO)
            .FirstOrDefault();
        if (null == yearChangeRecord)
            throw new Exception("Seat No (" + model.SeatNo + ") not available for this examination.");
        model.Prn = yearChangeRecord.Chr_FK_PRN_NO;
    }

    #endregion

    #region -- Events --


    // GET: /AnswerSheet/Create
    [Authorize]
    public ActionResult Create()
    {
        return View(new AnswerSheet());
    }

    // POST: /AnswerSheet/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public ActionResult Create(AnswerSheet model, string submitType)
    {
        try
        {
            switch (submitType)
            {
                case "Search":
                    GetSeatNoInfo(model);
                    GetSubjects(model);
                    ModelState.Clear();
                    return View(model);
                case "FeeCalculation":
                    GetSeatNoInfo(model);
                    CalculateFee(model);
                    ModelState.Clear();
                    return View(model);
            }

            if (ModelState.IsValid)
            {
                // Validations
                var exists = _transactionService.AnswerSheetRepository.Get(t => t.SeatNo == model.SeatNo).FirstOrDefault();
                if (null != exists)
                    throw new Exception("Photo copy request for this Seat No(" + model.SeatNo + ") already exists.");

                // Validate model
                ValidateModel(model);

                // Calculate fee
                CalculateFee(model);

                /*using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout });
                try
                {*/
                model.CreatedBy = User.Identity.GetUserId();
                model.CreatedDate = DateTime.Now;
                model.InstanceId = (int)HttpContext.Session[ModelConstants.InstanceId];

                _transactionService.AnswerSheetRepository.Add(model);
                _transactionService.Save();

                // AddAnswerInExamServer(viewModel);

                //scope.Complete();
                TempData["Success"] = "Answer Sheet form submitted successfully.";

                return RedirectToAction("Create");
                /*}
                catch (Exception)
                {
                    scope.Dispose();
                    throw;
                }*/
            }
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        //GetSeatNoInfo(model);
        return View(model);
    }
    #endregion

}