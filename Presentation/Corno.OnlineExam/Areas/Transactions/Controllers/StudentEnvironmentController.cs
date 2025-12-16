using Corno.Data.Corno;
using Corno.Data.Helpers;
using Corno.Data.Operation;
using Corno.Data.Payment;
using Corno.Data.ViewModels;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Globals.Enums;
using Corno.Logger;
using Corno.Services.Bootstrapper;
using Corno.Services.Common.Interfaces;
using Corno.Services.Core;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Payment.Interfaces;
using Kendo.Mvc.UI;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web.Mvc;

namespace Corno.OnlineExam.Areas.Transactions.Controllers;

[Authorize]
public class StudentEnvironmentController : EnvironmentController
{
    #region -- Constructors --
    public StudentEnvironmentController(ICornoService cornoService, ICoreService coreService,
        IAmountInWordsService amountInWordsService, ILinkService linkService, IEaseBuzzService easeBuzzService)
        : base(cornoService, coreService, amountInWordsService)

    {
        _cornoService = cornoService;
        _linkService = linkService;
        _easeBuzzService = easeBuzzService;

        _formType = FormType.Environment;
    }
    #endregion

    #region -- Data Members --

    private readonly ICornoService _cornoService;
    private readonly ILinkService _linkService;
    private readonly IEaseBuzzService _easeBuzzService;

    private readonly FormType _formType;

    #endregion

    #region -- Private Methods --
    private string PaymentApi(EnvironmentStudy model)
    {
        var successUrl = Url.Action("OnPaymentSuccess", "StudentEnvironment", null, Request?.Url?.Scheme, null);
        var failureUrl = Url.Action("OnPaymentFailure", "StudentEnvironment", null, Request?.Url?.Scheme, null);
        var productInfo = "Test";
        //viewModel.Total = 1;
        var operationRequest = new OperationRequest
        {
            InputData = new Dictionary<string, object>
            {
                [ModelConstants.FormType] = _formType.ToString(),
                [ModelConstants.TransactionId] = model.TransactionId,
                [ModelConstants.StudentName] = model.StudentName,
                [ModelConstants.Email] = model.Email,
                [ModelConstants.Mobile] = model.Mobile,
                [ModelConstants.Amount] = model.TotalFee?.ToString(),
                [ModelConstants.ProductInfo] = productInfo,
                [ModelConstants.SuccessUrl] = successUrl,
                [ModelConstants.FailureUrl] = failureUrl,
                [ModelConstants.PaymentMode] = string.Empty,
                [ModelConstants.SplitPayments] = string.Empty,
                [ModelConstants.PrnNo] = model.PrnNo,
                [ModelConstants.CollegeId] = model.CollegeId.ToString(),
                [ModelConstants.CourseId] = model.CourseId.ToString(),
                [ModelConstants.InstanceId] = model.InstanceId.ToString(),
            }
        };
        return _easeBuzzService.PaymentApiRequest(operationRequest);
    }

    private EnvironmentStudy GetModel(string prn)
    {
        var model = new EnvironmentStudy { PrnNo = prn };
        var link = _linkService.GetExamLink(model.PrnNo, _formType);
        if (null == link)
            throw new Exception("No Link is generated for you to fill the form.");
        var linkDetail = link.LinkDetails.FirstOrDefault(d => d.Prn == model.PrnNo);
        if (null == linkDetail)
            throw new Exception("No Link details are generated for you to fill the form 1.");
        model.InstanceId = link.InstanceId;

        // Get environment study info for PRN
        GetPrnInfo(model);

        return model;
    }

    public static List<ExamIndexViewModel> GetIndexViewModels(string prn, int serialNo, SessionData session)
    {
        var coreService = Bootstrapper.Get<ICoreService>();
        var rawData = from environmentStudy in coreService.TBl_STUDENT_ENV_STUDIES_Repository
                .Get(e => e.Chr_FK_PRN_NO == prn).AsEnumerable()
            join coursePart in coreService.Tbl_COURSE_PART_MSTR_Repository.Get().AsEnumerable()
                on environmentStudy.Num_FK_COPRT_NO equals coursePart.Num_PK_COPRT_NO into defaultCoursePart
            from coursePart in defaultCoursePart.DefaultIfEmpty()
            join instance in coreService.Tbl_SYS_INST_Repository.Get().AsEnumerable()
                on environmentStudy.Num_FK_INST_NO equals instance.Num_PK_INST_SRNO into defaultInstance
            from instance in defaultInstance.DefaultIfEmpty()
            select new
            {
                environmentStudy,
                coursePart,
                instance
            };

        var viewModels = rawData
            .AsEnumerable() // Materialize here
            .Select(x => new ExamIndexViewModel
            {
                Id = x.environmentStudy.Num_PK_ENTRY_ID,
                FormType = FormType.Environment,
                InstanceId = x.environmentStudy.Num_FK_INST_NO,
                InstanceName = $"({x.instance?.Num_PK_INST_SRNO}) {x.instance?.Var_INST_REM}",
                CollegeId = x.environmentStudy.Num_FK_COL_CD,
                CollegeName = session.CollegeName,
                CentreId = x.environmentStudy.Num_FK_DistCenter_ID,
                CentreName = session.CenterId > 0 ? $"({session.CenterId}) {session.CenterName}" : "-",
                CourseId = session.CourseId,
                CourseName = session.CourseName,
                CoursePartId = x.environmentStudy.Num_FK_COPRT_NO,
                CoursePartName = $"({x.environmentStudy.Num_FK_COPRT_NO}) {x.coursePart?.Var_COPRT_SHRT_NM}",
                Prn = session.Prn,
                StudentName = session.StudentName,
                FormDate = x.environmentStudy.Dtm_DTE_CR,
                TransactionId = x.environmentStudy.Chr_Transaction_Id,
                TotalFee = x.environmentStudy.Num_EnviTotalFee
            })
            .OrderByDescending(d => d.CoursePartId)
            .ToList();

        return viewModels;
        /*var coreService = Bootstrapper.Get<ICoreService>();
        var instanceService = Bootstrapper.Get<IInstanceService>();
        var viewModels = coreService.TBl_STUDENT_ENV_STUDIES_Repository.Get(e => 
                e.Chr_FK_PRN_NO == prn)
            .Select(e =>
            {
                var coursePartName = ExamServerHelper.GetCoursePartName(e.Num_FK_COPRT_NO, coreService);
                var instance = instanceService.GetById(e.Num_FK_INST_NO);
                return new ExamIndexViewModel
                {
                    SerialNo = serialNo++,
                    Id = e.Num_PK_ENTRY_ID,
                    FormType = FormType.Environment,
                    InstanceId = e.Num_FK_INST_NO,
                    InstanceName = $"({instance?.Id}) {instance?.Name}",
                    CollegeId = e.Num_FK_COL_CD,
                    CollegeName = session.CollegeName,
                    CentreId = e.Num_FK_DistCenter_ID,
                    CentreName = session.CenterId > 0 ? $"({session.CenterId}) {session.CenterName}" : "-",
                    CourseId = session.CourseId,
                    CourseName = session.CourseName,
                    CoursePartId = e.Num_FK_COPRT_NO,
                    CoursePartName = $"({e.Num_FK_COPRT_NO}) {coursePartName}",

                    Prn = session.Prn,
                    StudentName = session.StudentName,

                    FormDate = e.Dtm_DTE_CR,
                    TransactionId = e.Chr_Transaction_Id,
                    TotalFee = e.Num_EnviTotalFee
                };
            }).OrderByDescending(d => d.CoursePartId)
            .ToList();
        return viewModels;*/
    }

    private void PaymentSuccess(string prn, string transactionId, double paidAmount, string mobile, DateTime paymentDate)
    {
        var ipAddress = GetClientIpAddress();

        // Update in environ studies tables
        var model = UpdateInCornoDatabase(prn, transactionId, paidAmount, paymentDate, ipAddress);

        // Update Links
        _linkService.UpdatePayment(prn, transactionId, paidAmount, paymentDate, _formType, ipAddress);
            
        // Update in core database.
        AddInCoreDatabase(model, ModelConstants.Online);

        // Send payment sms
        _linkService.SendPaymentSms(mobile, transactionId, _formType);
    }

    private void HandleApiResponse(FormCollection form)
    {
        var prn = form[EaseBuzzConstants.Udf1];
        _easeBuzzService.PaymentApiResponse(form);

        var transactionId = form[EaseBuzzConstants.TransactionId];
        var paidAmount = form[EaseBuzzConstants.Amount].ToDouble();
        var mobile = form[EaseBuzzConstants.Phone];
        if (null == transactionId)
            throw new Exception("Invalid transaction Id");
        if (null == prn)
            throw new Exception("Invalid payment prn");

        PaymentSuccess(prn, transactionId, paidAmount, mobile, DateTime.Now);
    }

    private bool IsFormAlreadySubmitted(out string transactionId)
    {
        transactionId = string.Empty;
        if (!(HttpContext.Session[User.Identity.Name] is SessionData sessionData))
            throw new Exception("Invalid session data");

        var existingForm = _cornoService.EnvironmentStudyRepository.Get(e => e.PrnNo == sessionData.Prn && 
                                                                             e.Status == StatusConstants.InProcess).FirstOrDefault();
        if (null == existingForm || string.IsNullOrEmpty(existingForm.TransactionId)) return false;

        var link = _linkService.GetExamLink(existingForm.PrnNo, _formType);
        if (null == link)
            throw new Exception("No Link is generated for you to fill the form.");
        var linkDetail = link.LinkDetails.FirstOrDefault(d => d.Prn == existingForm.PrnNo);
        if (null == linkDetail)
            throw new Exception("No Link details are generated for you to fill the form 1.");

        //LogHandler.LogInfo("Link detail found");
        var operationRequest = new OperationRequest
        {
            InputData = new Dictionary<string, object>
            {
                [ModelConstants.TransactionId] = existingForm.TransactionId,
                [ModelConstants.Email] = linkDetail.EmailId,
                [ModelConstants.Mobile] = linkDetail.Mobile,
                [ModelConstants.Amount] = existingForm.TotalFee?.ToString("F1"),
            }
        };
        //LogHandler.LogInfo($"Operation Request : {operationRequest}");

        var jsonString = _easeBuzzService.TransactionApi(operationRequest);
        //var transactionApiResult = JsonConvert.DeserializeObject<TransactionRoot>(result);
        var jsonObject = JObject.Parse(jsonString);
        //var transactionApiResult = JsonConvert.DeserializeObject<TransactionRoot>(result);
        var result = jsonObject["status"].ToBoolean();
        //LogHandler.LogInfo($"Result : {result}, JsonString : {jsonString}");
        if (!result)
        {
            /*_cornoService.EnvironmentStudyRepository.Delete(existingForm);
            _cornoService.Save();*/

            return false;
        }

        // Delete if form was not submitted successfully
        var message = JsonConvert.DeserializeObject<TransactionMessage>(jsonObject["msg"]?.ToString() ?? string.Empty);
        if (message.status != "success")
            return false;
        if (existingForm.PrnNo != message.udf1 && existingForm.TransactionId != message.txnid)
            throw new Exception($"Transaction Id {message.txnid} does not belong to PRN {existingForm.PrnNo}");
        transactionId = message.txnid;
        PaymentSuccess(existingForm.PrnNo, existingForm.TransactionId, message.amount.ToDouble(), message.phone, message.addedon.ToDateTime());

        return true;
    }
    #endregion

    #region -- Actions --
    // GET: /Reg/Create
    [Authorize]
    public override ActionResult Create()
    {
        try
        {
            if (!(HttpContext.Session[User.Identity.Name] is SessionData sessionData))
                throw new Exception("Invalid session data");

            //if (IsFormAlreadySubmitted(out var transactionId))
            //{
            //    TempData[ModelConstants.Success] = $"Environment form is already submitted successfully. Your payment transaction id is {transactionId}";
            //    return RedirectToAction("PaymentLogin", "Account", new { area = "Admin", prn = sessionData.Prn });
            //}

            ViewBag.Disabled = false;

            return View(GetModel(sessionData.Prn));
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(new EnvironmentStudy());
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public override ActionResult Create(EnvironmentStudy model, string submitType)
    {
        if (!ModelState.IsValid)
            return View(model);
        try
        {
            //Check for invalid PRN No
            if (string.IsNullOrEmpty(model.PrnNo))
                throw new Exception("Error: Invalid PRN No");


            // Generate new form with selected subjects from view model
            model.Status = StatusConstants.InProcess;
            model.TransactionId = GetTransactionId();

            // First delete previous In-Process forms
            var existing = _cornoService.EnvironmentStudyRepository.Get(e => e.PrnNo == model.PrnNo
                                                                             && e.Status == StatusConstants.InProcess).FirstOrDefault();
            if (null == existing)
            {
                // Add in corno database
                AddInCornoDatabase(model, ModelConstants.Online);
                _cornoService.Save();
            }
            else
            {
                _cornoService.EnvironmentStudyRepository.Update(existing);
                _cornoService.Save();
            }

            // Generate payment request
            var result = PaymentApi(model);
            return Content(result);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(model);
    }

    #endregion

    #region -- Payment gateway events --
    // POST: 
    [AllowAnonymous]
    public ActionResult OnPaymentSuccess(FormCollection form)
    {
        var prn = form[EaseBuzzConstants.Udf1];
        try
        {
            LogHandler.LogInfo($"OnPaymentSuccess. PRN : {prn}, Form Type : {form[EaseBuzzConstants.Udf5]}, Transaction Id : {form[EaseBuzzConstants.TransactionId]},  Status : {form["status"]}");

            HandleApiResponse(form);

            var transactionId = form[EaseBuzzConstants.TransactionId];
            TempData[ModelConstants.Success] = $"Environment form is submitted successfully. Your payment transaction id is {transactionId}";
            return RedirectToAction("PaymentLogin", "Account", new { area = "Admin", prn = prn });
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            HandleControllerException(exception);

            TempData[ModelConstants.Error] = exception.Message;
        }
        return RedirectToAction("PaymentLogin", "Account", new { area = "Admin", prn = prn });
    }

    //[HttpPost]
    [AllowAnonymous]
    public ActionResult OnPaymentFailure(FormCollection form)
    {
        try
        {
            LogHandler.LogInfo($"OnPaymentFailure. PRN : {form[EaseBuzzConstants.Udf5]}, Form Type : {form[EaseBuzzConstants.Udf2]}, Transaction Id : {form[EaseBuzzConstants.TransactionId]},  Status : {form["status"]}");

            var transactionId = form["txnid"];
            var mobile = form["phone"];
            var prn = form["udf1"];
            if (null == transactionId)
                throw new Exception("Invalid transaction Id");
            if (null == prn)
                throw new Exception("Invalid payment prn");

            return RedirectToAction("PaymentLogin", "Account", new { area = "Admin", mobileNo = mobile });
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            HandleControllerException(exception);
        }
        return RedirectToAction("Create", "StudentEnvironment");
    }

    [AllowAnonymous]
    public ActionResult OnWebHook(FormCollection form)
    {
        var prn = form[EaseBuzzConstants.Udf1];
        try
        {
            LogHandler.LogInfo($"OnWebHook. PRN : {form[EaseBuzzConstants.Udf5]}, Form Type : {form[EaseBuzzConstants.Udf5]}, Transaction Id : {form[EaseBuzzConstants.TransactionId]},  Status : {form["status"]}");

            HandleApiResponse(form);

            var transactionId = form[EaseBuzzConstants.TransactionId];
            TempData[ModelConstants.Success] = $"Environment form is submitted successfully. Your payment transaction id is {transactionId}";

            LogHandler.LogInfo("After calling Environment WebHook");

            return RedirectToAction("PaymentLogin", "Account", new { area = "Admin", prn = prn });
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            HandleControllerException(exception);
        }
        return RedirectToAction("Create", "StudentEnvironment");
    }

    #endregion
}