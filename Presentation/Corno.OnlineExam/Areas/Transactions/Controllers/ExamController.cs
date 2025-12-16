using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using System.Web.Routing;
using Corno.Data.Core;
using Corno.Data.Corno;
using Corno.Data.Helpers;
using Corno.Data.Operation;
using Corno.Data.ViewModels;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Globals.Enums;
using Corno.Logger;
using Corno.OnlineExam.Controllers;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Corno.Online_Education.Interfaces;
using Corno.Services.Payment.Interfaces;
using Microsoft.AspNet.Identity;

namespace Corno.OnlineExam.Areas.Transactions.Controllers;

[Authorize]
public class ExamController : UniversityController
{
    #region -- Constructors --
    public ExamController(ICornoService cornoService, ICoreService coreService,
        ILinkService linkService, IEaseBuzzService easeBuzzService, IOnlineEducationStudentService onlineEducationStudentService)
    {
        _cornoService = cornoService;
        _coreService = coreService;
        _linkService = linkService;
        _easeBuzzService = easeBuzzService;
        _onlineEducationStudentService = onlineEducationStudentService;
    }
    #endregion

    #region -- Data Members --

    private readonly ICornoService _cornoService;
    private readonly ICoreService _coreService;
    private readonly ILinkService _linkService;
    private readonly IEaseBuzzService _easeBuzzService;
    private readonly IOnlineEducationStudentService _onlineEducationStudentService;

    #endregion

    #region -- Private Methods --
    private Exam UpdatePayment(int instanceId, string prn, string transactionId, double paidAmount, DateTime paymentDate, string ipAddress)
    {
        // Update Link
        /*var exam = _cornoService.ExamRepository.Get(e => e.PrnNo == prn && e.Status == StatusConstants.InProcess)
            .FirstOrDefault();*/
        var exam = GetExistingForm(instanceId, prn);
        if (null == exam)
            throw new Exception($"No exam form found for PRN ({prn})");

        exam.PaymentDate = paymentDate;
        exam.TransactionId = transactionId;
        exam.PaidAmount ??= 0;
        exam.PaidAmount += paidAmount;
        exam.DeletedBy = ipAddress;

        if (!exam.Total.Equals(exam.PaidAmount))
        {
            _cornoService.ExamRepository.Update(exam);
            _cornoService.Save();
            throw new Exception($"You have made payment of Rs. {paidAmount} which does not match with actual amount Rs. {exam.Total}");
        }

        exam.Status = StatusConstants.Paid;

        _cornoService.ExamRepository.Update(exam);
        _cornoService.Save();

        return exam;
    }

    private void AddExamInExamServer(Exam model)
    {
        // Get from TBL_STUDENT_YR_CHNG
        if (model.CoursePartId == null)
            return;

        //var ipAddress = GetClientIpAddress();

        var appTemp = new Tbl_APP_TEMP
        {
            Num_FORM_ID = model.FormNo.ToInt(),
            Chr_APP_VALID_FLG = "A",
            DELETE_FLG = "N",
            Chr_APP_PRN_NO = model.PrnNo,
            Num_FK_COPRT_NO = (short)model.CoursePartId,
            Num_FK_INST_NO = (short)model.InstanceId
        };

        if (model.BranchId == null)
            appTemp.Num_FK_BR_CD = 0;
        else
            appTemp.Num_FK_BR_CD = (short)model.BranchId;

        if (model.CollegeId != null) appTemp.Num_FK_COLLEGE_CD = (short)model.CollegeId;

        if (null == model.CentreId)
            appTemp.Num_FK_DistCenter_ID = 0;
        else
            appTemp.Num_FK_DistCenter_ID = (short)model.CentreId;

        appTemp.Chr_BUNDAL_NO = model.Bundle;
        appTemp.AadharNo = model.AadharNo;
        //addition in AppTemp
        appTemp.Num_FK_STACTV_CD = 0;
        appTemp.Num_FK_STUDCAT_CD = 0;
        appTemp.Var_USR_NM = User.Identity.Name;
        appTemp.Dtm_DTE_CR = model.CreatedDate;
        appTemp.Dtm_DTE_UP = model.ModifiedDate;
        appTemp.Chr_REPEATER_FLG = "N";
        appTemp.Chr_IMPROVEMENT_FLG = "N";

        //fee structure
        appTemp.Num_ExamFee = model.ExamFee ?? 0;
        appTemp.Num_CAPFee = model.CapFee ?? 0;
        appTemp.Num_StatementFee = model.StatementOfMarksFee ?? 0;
        appTemp.Num_LateFee = model.LateFee ?? 0;
        appTemp.Num_SuperLateFee = model.SuperLateFee ?? 0;
        appTemp.Num_Fine = model.OthersFee ?? 0;
        appTemp.Num_PassingCertificateFee = model.CertificateOfPassingFee ?? 0;
        appTemp.Num_DissertationFee = model.DissertationFee ?? 0;
        appTemp.Num_BacklogFee = model.BacklogFee ?? 0;
        appTemp.Num_TotalFee = model.Total ?? 0;
        appTemp.Num_Transaction_Id = model.TransactionId;
        appTemp.PaidAmount ??= 0;
        appTemp.PaidAmount += model.PaidAmount;
        appTemp.PaymentDate = model.PaymentDate;

        if (model.CollegeId == 45)
            _onlineEducationStudentService.UpdateAppTemp(appTemp, model);

        _coreService.Tbl_APP_TEMP_Repository.Add(appTemp);
        _coreService.Save();

        // Save App_Temp Subjects
        foreach (var subject in model.ExamSubjects)
        {
            if (subject.CoursePartId != null)
            {
                if (subject.SubjectCode != null)
                {
                    var examSubject = new Tbl_APP_TEMP_SUB
                    {
                        Num_FK_INST_NO = appTemp.Num_FK_INST_NO,
                        Num_FK_ENTRY_ID = appTemp.Num_PK_ENTRY_ID,
                        Num_FK_COPRT_NO = (short)subject.CoursePartId,
                        Num_FK_SUB_CD = (short)subject.SubjectCode,
                        Chr_DELETE_FLG = "N"
                    };

                    if (subject.SubjectType == SubjectType.BackLog.ToString())
                        examSubject.Chr_REPH_FLG = "R";

                    _coreService.Tbl_APP_TEMP_SUB_Repository.Add(examSubject);
                }
            }
            _coreService.Save();
        }
    }

    private void UpdateStudentInfo(ExamViewModel viewModel)
    {
        var studentInfo = _coreService.Tbl_STUDENT_INFO_ADR_Repository.Get(s =>
            s.Chr_FK_PRN_NO == viewModel.PrnNo).FirstOrDefault();
        if (null == studentInfo)
            throw new Exception("Student with this prn no is not available.");

        studentInfo.Num_MOBILE = viewModel.Mobile;
        studentInfo.Chr_Student_Email = viewModel.Email;

        _coreService.Tbl_STUDENT_INFO_ADR_Repository.Update(studentInfo);
        _coreService.Save();
    }
    #endregion

    #region -- Protected Methods --
    protected Exam GetExistingForm(int instanceId, string prn)
    {
        return _cornoService.ExamRepository.Get(e => e.InstanceId == instanceId && e.PrnNo == prn
                && e.Status == StatusConstants.InProcess)
            .FirstOrDefault();
    }

    protected virtual Exam GetExamModel(ExamViewModel viewModel)
    {
        if (viewModel.ExamSubjectViewModels.Count <= 0)
            throw new Exception("There should be at least one subject to accept the form.");

        if (viewModel.ExamSubjectViewModels.Count(e => e.RowSelector) <= 0)
            throw new Exception("There should be at least one subject selected.");

        if (viewModel.ExamSubjectViewModels.Count(e => e.RowSelector == false && e.SubjectType == "Compulsory") > 0)
            throw new Exception("All compulsory subjects needs to be selected.");

        if (viewModel.ExamSubjectViewModels.Count(e => e.RowSelector == false && e.SubjectType == "BackLog") > 0)
            throw new Exception("All Backlog subjects needs to be selected.");

        if (string.IsNullOrEmpty(viewModel.Bundle))
            throw new Exception("Bundle is required");
        if (string.IsNullOrEmpty(viewModel.FormNo))
            throw new Exception("Form No is required");
        if (string.IsNullOrEmpty(viewModel.Mobile))
            throw new Exception("Mobile is required");
        if (string.IsNullOrEmpty(viewModel.Email))
            throw new Exception("Email is required");
        if (viewModel.ShowBranchCombo && (viewModel.BranchId == null || viewModel.BranchId == 0))
            throw new Exception("Branch must be selected");

        // Check for existing PRN No for Current Instance
        //var instanceId = (int)HttpContext.Session[ModelConstants.InstanceId];
        var instanceId = viewModel.InstanceId;

        var existingRecord = _coreService.Tbl_APP_TEMP_Repository.Get(e => e.Chr_APP_PRN_NO == viewModel.PrnNo && e.Num_FK_INST_NO == instanceId).ToList();
        if (existingRecord.Count > 0)
            throw new Exception("Examination form for this PRN " + viewModel.PrnNo + " already exists");

        // Check whether all optional counts are within range
        var optionalSubjects = viewModel.ExamSubjectViewModels.Where(e => e.MaxOptionalCount > 0 && e.OptionalIndex > 0).GroupBy(e => new { e.SubjectType, e.MaxOptionalCount }).Select(e => e.First()).ToList();
        //var isOptionalProblem = false;
        foreach (var optional in optionalSubjects)
        {
            var trueCount = viewModel.ExamSubjectViewModels.Count(e => e.OptionalIndex == optional.OptionalIndex && e.RowSelector);
            if (trueCount == optional.MaxOptionalCount) continue;

            throw new Exception($@"Please, Select optional subjects for '{optional.SubjectType}'");
            //isOptionalProblem = true;
        }
        /*if (isOptionalProblem)
            return View(viewModel);*/

        var model = AutoMapperConfig.CornoMapper.Map<Exam>(viewModel);

        // Set College & Course Id.
        var studentCourse = _coreService.Tbl_STUDENT_COURSE_Repository.Get(s => s.Chr_FK_PRN_NO == model.PrnNo).FirstOrDefault();
        if (null != studentCourse)
        {
            model.CollegeId = studentCourse.Num_ST_COLLEGE_CD;
            model.CourseId = studentCourse.Num_FK_CO_CD;
            model.CentreId = studentCourse.Num_FK_DistCenter_ID;
        }
        var coursePart = _coreService.Tbl_COURSE_PART_MSTR_Repository.Get(c => c.Num_PK_COPRT_NO == model.CoursePartId).FirstOrDefault();
        if (coursePart is { Chr_DEG_APL_FLG: "Y" })
        {
            if (null != viewModel.UploadPhoto)
            {
                model.Photo = new byte[viewModel.UploadPhoto.ContentLength];
                viewModel.UploadPhoto?.InputStream?.Read(model.Photo, 0, model.Photo.Length);
            }
        }

        foreach (var selectedSubject in viewModel.ExamSubjectViewModels)
        {
            if (!selectedSubject.RowSelector) continue;
            var examSubject = new ExamSubject
            {
                ExamId = viewModel.Id,
                SubjectCode = selectedSubject.SubjectCode,
                CoursePartId = selectedSubject.CoursePartId,
                SubjectType = selectedSubject.SubjectType,
                CreatedBy = User.Identity.GetUserId(),
                CreatedDate = DateTime.Now
            };

            model.ExamSubjects.Add(examSubject);
        }

        // Upload latest photo to Student_Info table
        if (viewModel.DegreeApplicable && null != viewModel.UploadPhoto)
        {
            model.Photo = new byte[viewModel.UploadPhoto.ContentLength];
            viewModel.UploadPhoto?.InputStream?.Read(model.Photo, 0, model.Photo.Length);
        }
        // Set created details for college
        model.CreatedBy = User.Identity.GetUserId();
        model.CreatedDate = DateTime.Now;
        return model;
    }

    protected void CreateFormForPaymentGateway(ExamViewModel viewModel)
    {
        // First delete previous In-Process forms
        var model = GetExistingForm(viewModel.InstanceId, viewModel.PrnNo);
        if (null == model)
        {
            // Generate new form with selected subjects from view model
            model = GetExamModel(viewModel);

            model.FeeId = viewModel.FeeId;

            model.Status = viewModel.Status;
            model.TransactionId = viewModel.TransactionId;
            model.DeletedBy = GetClientIpAddress();

            // Save the college details
            _cornoService.ExamRepository.Add(model);
            _cornoService.Save();
        }
        else
        {
            model.FeeId = viewModel.FeeId;

            model.Status = viewModel.Status;
            model.TransactionId = viewModel.TransactionId;
            model.Total = viewModel.Total;
            model.DeletedBy = GetClientIpAddress();

            _cornoService.ExamRepository.Update(model);
            _cornoService.Save();
        }
    }

    protected string Pay(ExamViewModel viewModel)
    {
        CreateFormForPaymentGateway(viewModel);

        // Generate payment request
        var result = PaymentApi(viewModel, FormType.Exam);

        return result;
    }

    protected string PaymentApi(ExamViewModel viewModel, FormType formType)
    {
        var successUrl = Url.Action("OnPaymentSuccess", "StudentExam", null, Request?.Url?.Scheme, null);
        var failureUrl = Url.Action("OnPaymentFailure", "StudentExam", null, Request?.Url?.Scheme, null);
        var productInfo = "Test";

        // Store the current URL in session storage or cookies
        HttpContext.Session["ReturnUrl"] = Request?.Url;
        HttpContext.Session["ReturnPath"] = Request?.Path;

        TempData["ReturnUrl"] = Request?.Url;
        TempData["ReturnPath"] = Request?.Path;

        //viewModel.Total = 1;
        var operationRequest = new OperationRequest
        {
            InputData = new Dictionary<string, object>
            {
                [ModelConstants.FormType] = formType.ToString(),
                [ModelConstants.TransactionId] = viewModel.TransactionId,
                [ModelConstants.StudentName] = viewModel.StudentName,
                [ModelConstants.Email] = viewModel.Email,
                [ModelConstants.Mobile] = viewModel.Mobile,
                [ModelConstants.Amount] = viewModel.Total?.ToString(),
                [ModelConstants.ProductInfo] = productInfo,
                [ModelConstants.SuccessUrl] = successUrl,
                [ModelConstants.FailureUrl] = failureUrl,
                [ModelConstants.PaymentMode] = string.Empty,
                [ModelConstants.SplitPayments] = string.Empty,
                [ModelConstants.PrnNo] = viewModel.PrnNo,
                [ModelConstants.CollegeId] = viewModel.CollegeId.ToString(),
                [ModelConstants.CourseId] = viewModel.CourseId.ToString(),
                [ModelConstants.InstanceId] = viewModel.InstanceId.ToString()
            }
        };
        return _easeBuzzService.PaymentApiRequest(operationRequest);
    }

    protected void PaymentSuccess(int instanceId, string prn, string transactionId, double paidAmount, string mobile, DateTime paymentDate,
        FormType formType)
    {
        var ipAddress = GetClientIpAddress();

        // Update Exam form
        var exam = UpdatePayment(instanceId, prn, transactionId, paidAmount, paymentDate, ipAddress);

        // Update Links
        _linkService.UpdatePayment(prn, transactionId, paidAmount, paymentDate, formType, ipAddress);

        // Add to APP_TEMP
        LogHandler.LogInfo($"Payment Success : Instance : {instanceId}, PRN : {prn}, Transaction Id : {transactionId}");
        var existingFormInCore = _coreService.Tbl_APP_TEMP_Repository
            .Get(a => a.Chr_APP_PRN_NO == prn && a.Num_Transaction_Id == transactionId)
            .FirstOrDefault();
        if (null == existingFormInCore)
            AddExamInExamServer(exam);

        // Send payment sms
        _linkService.SendPaymentSms(mobile, transactionId, formType);
    }

    protected void SaveForm(ExamViewModel viewModel)
    {
        var model = GetExamModel(viewModel);

        // Save the college details
        _cornoService.ExamRepository.Add(model);
        _cornoService.Save();

        // Update mobile & email
        UpdateStudentInfo(viewModel);

        // Add subject in exam server 
        AddExamInExamServer(model);
    }

    #endregion

    #region -- Events --
    // GET: /Reg/Create
    [Authorize]
    public virtual ActionResult Create()
    {
        var viewModel = new ExamViewModel();
        try
        {
            ViewBag.Disabled = false;
            viewModel.ExamSubjectViewModels = new List<ExamSubjectViewModel>();
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(viewModel);
    }

    // POST: /Reg/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual ActionResult Create(ExamViewModel viewModel, string submitType)
    {
        try
        {
            if (string.IsNullOrEmpty(viewModel.PrnNo))
                throw new Exception("Invalid PRN.");

            viewModel.InstanceId = Session[ModelConstants.InstanceId].ToInt();
            switch (submitType)
            {
                case "Search":
                case "CoursePartChange":
                case "BranchChange":
                    ExamFormHelper.GetPrnInfo(viewModel);
                    if (viewModel.CollegeId == 45)
                        _onlineEducationStudentService.UpdateExamFees(viewModel);
                    ModelState.Clear();
                    ViewBag.Disabled = true;
                    return View(viewModel);
                case "ReceiveChallan":
                    ExamFormHelper.ReceiveChallan(viewModel.PrnNo);
                    TempData["Success"] = "Challan Received Successfully.";
                    return RedirectToAction("Create");
                case "DirectPay":
                    if (HttpContext.Session[User.Identity.Name] is not SessionData sessionData)
                        throw new Exception("Invalid session data");
                    sessionData.Prn = viewModel.PrnNo;
                    return RedirectToAction("Create", new RouteValueDictionary(new { controller = "StudentExam", action = "Transactions" }));
                case "Pay":
                    viewModel.Status = StatusConstants.InProcess;
                    viewModel.TransactionId = GetTransactionId();

                    /*viewModel.Total = 10;
                    viewModel.StudentName = "test";
                    viewModel.Mobile = "9373333210";
                    viewModel.Email = "umesh.kale@concsystems.in";*/
                    var result = Pay(viewModel);
                    return Content(result);
            }

            if (!ModelState.IsValid)
            {
                ViewData["Error"] = "Error message text.";
                ViewBag.Disabled = true;
                return View(viewModel);
            }

            /*using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
            {
                try
                {*/
            // Save the form
            SaveForm(viewModel);

            TempData["Success"] = "Exam form submitted successfully.";

            //scope.Complete();
            /*}
            catch (Exception)
            {
                //scope.Dispose();
                throw;
            }
            }*/

            return RedirectToAction("Create");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        ViewBag.Disabled = true;
        return View(viewModel);
    }

    [HttpPost]
    public ActionResult PayTransaction(string transactionId)
    {
        try
        {
            var payout = _cornoService.PayoutRepository.Get(p => p.TransactionId == transactionId)
                .FirstOrDefault();
            if (null == payout)
                throw new Exception($"Invalid transaction Id ({transactionId}). It is not imported.");

            var paymentDate = payout.SettlementDate.ToDateTime();
            Enum.TryParse(payout.FormType, out FormType formType);

            var instanceId = payout.InstanceId ?? 0;
            var collegeId = payout.CollegeId ?? 0;
            var courseId = payout.CourseId ?? 0;
            var prn = payout.Prn;

            var exam = _cornoService.ExamRepository.Get(e => e.InstanceId == instanceId && e.CollegeId == collegeId &&
                                                             e.CourseId == courseId && e.PrnNo == prn && e.Status == StatusConstants.InProcess)
                .FirstOrDefault();
            if (null == exam)
                throw new Exception($"No exam form found for PRN ({prn})");

            exam.PaymentDate = paymentDate;
            exam.TransactionId = transactionId;
            exam.Status = StatusConstants.Paid;

            _cornoService.ExamRepository.Update(exam);
            _cornoService.Save();

            // Update Exam form
            //var exam = UpdatePayment(instanceId, payout.Prn, transactionId, paymentDate);

            // Add to APP_TEMP
            var existingFormInCore = _coreService.Tbl_APP_TEMP_Repository.Get(a => a.Num_FK_INST_NO == instanceId &&
                a.Num_FK_COLLEGE_CD == collegeId && a.Chr_APP_PRN_NO == prn).FirstOrDefault();
            if (null == existingFormInCore)
                AddExamInExamServer(exam);

            // Update Links
            // Update Link
            var link = _cornoService.LinkRepository.Get(l => l.FormTypeId == (int)formType && l.InstanceId == instanceId &&
                                                             l.CollegeId == collegeId && l.CourseId == courseId && l.LinkDetails.Any(d => d.Prn == prn))
                .OrderByDescending(p => p.Id).FirstOrDefault();
            if (null == link)
                throw new Exception($"No link found for PRN ({prn})");
            var linkDetail = link.LinkDetails.FirstOrDefault(d => d.Prn == prn);
            if (null == linkDetail)
                throw new Exception($"No link detail found for PRN ({prn})");

            linkDetail.TransactionId = transactionId;
            linkDetail.PaymentDate = paymentDate;// DateTime.Now;
            linkDetail.Status = StatusConstants.Paid;

            _cornoService.LinkRepository.Update(link);
            _cornoService.Save();

            //_linkService.UpdatePayment(prn, transactionId, paymentDate, formType);

            // Send payment sms
            //_linkService.SendPaymentSms(mobile, transactionId, formType);

            /*PaymentSuccess(payout.InstanceId ?? 0, payout.Prn, transactionId, string.Empty, paymentDate,
                formType);*/

            // Return success response
            return Json(new { success = true, message = "Form submitted successfully." });
        }
        catch (Exception exception)
        {
            // Return error response
            return Json(new { success = false, message = exception.Message });
        }
    }

    [HttpPost]
    public ActionResult DirectPay(string prn)
    {
        try
        {
            if (string.IsNullOrEmpty(prn))
                throw new Exception("Invalid PRN");
            if (!(HttpContext.Session[User.Identity.Name] is SessionData sessionData))
                throw new Exception("Invalid session data");
            sessionData.Prn = prn;
            return RedirectToAction("Create", new RouteValueDictionary(new { controller = "StudentExam", action = "Transactions" }));
        }
        catch (Exception exception)
        {
            // Return error response
            return Json(new { success = false, message = exception.Message });
        }
    }

    #endregion
}