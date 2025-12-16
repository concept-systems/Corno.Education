using Corno.Data.Corno;
using Corno.Data.Helpers;
using Corno.Data.Operation;
using Corno.Data.ViewModels;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Globals.Enums;
using Corno.Logger;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Payment.Interfaces;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Web.Mvc;

namespace Corno.OnlineExam.Areas.Transactions.Controllers;

[Authorize]
public class StudentConvocationController : ConvocationController
{
    #region -- Constructors --
    public StudentConvocationController(ICornoService cornoService, ILinkService linkService, IEaseBuzzService easeBuzzService)
    {
        _cornoService = cornoService;
        _linkService = linkService;
        _easeBuzzService = easeBuzzService;

        _formType = FormType.Convocation;
    }
    #endregion

    #region -- Data Members --

    private readonly ICornoService _cornoService;
    private readonly ILinkService _linkService;
    private readonly IEaseBuzzService _easeBuzzService;

    private readonly FormType _formType;

    #endregion

    #region -- Private Methods --
    private string PaymentApi(ConvocationViewModel viewModel)
    {
        var successUrl = Url.Action("OnPaymentSuccess", "StudentConvocation", null, Request?.Url?.Scheme, null);
        var failureUrl = Url.Action("OnPaymentFailure", "StudentConvocation", null, Request?.Url?.Scheme, null);
        var productInfo = "Test";
        //viewModel.Total = 1;
        var instanceId = (int)HttpContext.Session[ModelConstants.InstanceId];

        LogHandler.LogInfo($"Success URL : {successUrl}");
        var operationRequest = new OperationRequest
        {
            InputData = new Dictionary<string, object>
            {
                [ModelConstants.FormType] = _formType.ToString(),
                [ModelConstants.TransactionId] = viewModel.TransactionId,
                [ModelConstants.StudentName] = viewModel.StudentName,
                [ModelConstants.Email] = viewModel.Email,
                [ModelConstants.Mobile] = viewModel.Phone,
                [ModelConstants.Amount] = viewModel.TotalFee.ToString(CultureInfo.InvariantCulture),
                [ModelConstants.ProductInfo] = productInfo,
                [ModelConstants.SuccessUrl] = successUrl,
                [ModelConstants.FailureUrl] = failureUrl,
                [ModelConstants.PaymentMode] = string.Empty,
                [ModelConstants.SplitPayments] = string.Empty,
                [ModelConstants.PrnNo] = viewModel.PrnNo,
                [ModelConstants.CollegeId] = viewModel.CollegeId.ToString(),
                [ModelConstants.CourseId] = viewModel.CourseId.ToString(),
                [ModelConstants.InstanceId] = instanceId
            }
        };
        return _easeBuzzService.PaymentApiRequest(operationRequest);
    }

    public static List<ExamIndexViewModel> GetIndexViewModels(string prn, int serialNo, SessionData session)
    {
        var instanceService = Bootstrapper.Get<IInstanceService>();
        var coreService = Bootstrapper.Get<ICoreService>();
        var cornoService = Bootstrapper.Get<ICornoService>();
        var centerName = session.CenterId > 0 ? $"({session.CenterId}) {session.CenterName}" : "-";
        var viewModels = (from convocation in cornoService.ConvocationRepository.Get(e => 
                    e.PrnNo == prn && e.Status == StatusConstants.Paid).AsEnumerable()
                join instance in instanceService.GetQuery().AsEnumerable()
                    on convocation.InstanceId equals instance.Id into defaultInstance
                from instance in defaultInstance.DefaultIfEmpty()
                select new
                {
                    convocation,
                    instance
                }).AsEnumerable() // Materialize here
            .Select(x => new ExamIndexViewModel
            {
                Id = x.convocation.Id,
                FormType = FormType.Convocation,
                InstanceId = x.convocation.InstanceId,
                InstanceName = $"({x.instance?.Id}) {x.instance?.Name}", // Now safe
                CollegeId = x.convocation.CollegeId,
                CollegeName = session.CollegeName,
                CentreId = x.convocation.CentreId,
                CentreName = centerName,
                CourseId = session.CourseId,
                CourseName = session.CourseName,
                CoursePartId = x.convocation.CoursePartId,
                Prn = session.Prn,
                StudentName = session.StudentName,
                FormDate = x.convocation.CreatedDate,
                TransactionId = x.convocation.TransactionId,
                TotalFee = x.convocation.TotalFee
            }).OrderByDescending(d => d.CoursePartId).ToList();

        var coursePartIds = viewModels.Select(p => p.CoursePartId)
            .Distinct();
        var courseParts = coreService.Tbl_COURSE_PART_MSTR_Repository.Get(p =>
            coursePartIds.Contains(p.Num_PK_COPRT_NO));
        viewModels.ForEach(p =>
        {
            var coursePart = courseParts.FirstOrDefault(x => x.Num_PK_COPRT_NO == p.CoursePartId);
            p.SerialNo = serialNo++;
            p.CoursePartName = $"({p.CoursePartId}) {coursePart?.Var_COPRT_SHRT_NM}";
        });

        return viewModels;
        /*var viewModels = cornoService.ConvocationRepository.Get(e => e.PrnNo == prn)
            .Select(e =>
            {
                var coursePartName = ExamServerHelper.GetCoursePartName(e.CoursePartId, coreService);
                var instance = instanceService.GetById(e.InstanceId);
                return new ExamIndexViewModel
                {
                    SerialNo = serialNo++,
                    Id = e.Id,
                    FormType = FormType.Convocation,
                    InstanceId = e.InstanceId,
                    InstanceName = $"({instance?.Id}) {instance?.Name}",
                    CollegeId = e.CollegeId,
                    CollegeName = session.CollegeName,
                    CentreId = e.CentreId,
                    CentreName = session.CenterId > 0 ? $"({session.CenterId}) {session.CenterName}" : "-",
                    CourseId = session.CourseId,
                    CourseName = session.CourseName,
                    CoursePartId = e.CoursePartId,
                    CoursePartName = $"({e.CoursePartId}) {coursePartName}",

                    Prn = session.Prn,
                    StudentName = session.StudentName,

                    FormDate = e.CreatedDate,
                    TransactionId = e.TransactionId,
                    TotalFee = e.TotalFee
                };
            }).OrderByDescending(d => d.CoursePartId)
            .ToList();
        return viewModels;*/
    }

    private void PaymentSuccess(string prn, string transactionId, double paidAmount, string mobile, DateTime paymentDate)
    {
        var ipAddress = GetClientIpAddress();

        // Update in convocation tables
        Update(prn, transactionId, paidAmount, paymentDate, ipAddress);

        // Update Links
        _linkService.UpdatePayment(prn, transactionId, paidAmount, paymentDate, _formType, ipAddress);

        // Send payment sms
        _linkService.SendPaymentSms(mobile, transactionId, _formType);
    }

    private void HandlePaymentApIResponse(FormCollection form)
    {
        _easeBuzzService.PaymentApiResponse(form);

        var prn = form[EaseBuzzConstants.Udf1];
        var transactionId = form[EaseBuzzConstants.TransactionId];
        var paidAmount = form[EaseBuzzConstants.Amount].ToDouble();
        var mobile = form[EaseBuzzConstants.Phone];
        if (null == transactionId)
            throw new Exception("Invalid transaction Id");
        if (null == prn)
            throw new Exception("Invalid payment prn");

        PaymentSuccess(prn, transactionId, paidAmount, mobile, DateTime.Now);
    }

    /*private bool IsFormAlreadySubmitted(out string transactionId)
    {
        transactionId = string.Empty;
        if (!(HttpContext.Session[User.Identity.Name] is SessionData sessionData))
            throw new Exception("Invalid session data");

        var existingForm = _cornoService.ConvocationRepository.Get(e => e.PrnNo == sessionData.Prn &&
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
                [ModelConstants.Amount] = existingForm.TotalFee.ToString("F1"),
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
            _cornoService.Save();#1#

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
    }*/
    #endregion

    #region -- Events --
    // GET: /Convocation/Create
    [Authorize]
    public override ActionResult Create()
    {
        var viewModel = new ConvocationViewModel();
        try
        {
            if (!(HttpContext.Session[User.Identity.Name] is SessionData sessionData))
                throw new Exception("Invalid session data");

            viewModel.PrnNo = sessionData.Prn;
            ViewBag.Disabled = false;

            var link = _linkService.GetExamLink(viewModel.PrnNo, _formType);
            if (null == link)
                throw new Exception("No Link is generated for you to fill the form.");
            var linkDetail = link.LinkDetails.FirstOrDefault(d => d.Prn == viewModel.PrnNo);
            if (null == linkDetail)
                throw new Exception("No Link details are generated for you to fill the form 1.");

            // Get convocation info for PRN
            viewModel.InstanceId = linkDetail.InstanceId ?? 0;
            PrnChange(viewModel);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            viewModel.Clear();
        }
        return View(viewModel);
        //return View(new ConvocationViewModel());
    }

    // POST: /Convocation/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public override ActionResult Create(ConvocationViewModel viewModel, string submitType)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // Generate new form with selected subjects from view model
                viewModel.Status = StatusConstants.InProcess;
                viewModel.TransactionId = GetTransactionId();

                // Fill Fee details
                FillFeeStructure(viewModel);

                // Save
                //Save(viewModel);
                var convocation = _cornoService.ConvocationRepository.Get(e => e.PrnNo == viewModel.PrnNo &&
                                                                               e.Status == StatusConstants.InProcess)
                    .OrderByDescending(p => p.Id)
                    .FirstOrDefault();
                if (null == convocation)
                    AddInCornoDatabase(viewModel);
                else
                {
                    convocation.Status = viewModel.Status;
                    convocation.TransactionId = viewModel.TransactionId;
                    _cornoService.ConvocationRepository.Update(convocation);
                    _cornoService.Save();
                }

                var result = PaymentApi(viewModel);
                return Content(result);
            }
        }
        catch (Exception exception)
        {
            viewModel.Clear();

            HandleControllerException(exception);
            //ModelState.Clear();
        }

        //PrnChange(viewModel);
        return View(viewModel);
    }

    #endregion

    #region -- Payment gateway events --
    // POST: /Reg/Create
    //[HttpPost]
    [AllowAnonymous]
    public ActionResult OnPaymentSuccess(FormCollection form)
    {
        try
        {
            LogHandler.LogInfo($"OnPaymentSuccess. PRN : {form[EaseBuzzConstants.Udf1]}, Form Type : {form[EaseBuzzConstants.Udf5]}, Transaction Id : {form[EaseBuzzConstants.TransactionId]},  Status : {form["status"]}");

            HandlePaymentApIResponse(form);

            //TempData[ModelConstants.Success] = $"Convocation form is submitted successfully. Your payment transaction id is {transactionId}";
            //return RedirectToAction("PaymentLogin", "Account", new { area = "Admin", prn });
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            HandleControllerException(exception);

            TempData[ModelConstants.Error] = exception.Message;
        }

        var prn = form[EaseBuzzConstants.Udf1];
        return RedirectToAction("PaymentLogin", "Account", new { area = "Admin", prn });
    }

    //[HttpPost]
    [AllowAnonymous]
    public ActionResult OnPaymentFailure(FormCollection form)
    {
        try
        {
            LogHandler.LogInfo($"OnPaymentFailure. PRN : {form[EaseBuzzConstants.Udf1]}, Form Type : {form[EaseBuzzConstants.Udf5]}, Transaction Id : {form[EaseBuzzConstants.TransactionId]},  Status : {form["status"]}");

            var prn = form[EaseBuzzConstants.Udf1];
            var transactionId = form[EaseBuzzConstants.TransactionId];
            var mobile = form[EaseBuzzConstants.Phone];
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
        return RedirectToAction("Create", "StudentConvocation");
    }

    [AllowAnonymous]
    public ActionResult OnWebHook(FormCollection form)
    {
        try
        {
            LogHandler.LogInfo($"OnWebHook. PRN : {form[EaseBuzzConstants.Udf1]}, Form Type : {form[EaseBuzzConstants.Udf5]}, Transaction Id : {form[EaseBuzzConstants.TransactionId]},  Status : {form["status"]}");
            LogHandler.LogInfo("Before calling Convocation WebHook");

            HandlePaymentApIResponse(form);

            var prn = form[EaseBuzzConstants.Udf1];
            var transactionId = form[EaseBuzzConstants.TransactionId];
            TempData[ModelConstants.Success] = $"Convocation form is submitted successfully. Your payment transaction id is {transactionId}";

            LogHandler.LogInfo("After calling Convocation WebHook");

            return RedirectToAction("PaymentLogin", "Account", new { area = "Admin", prn });
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            HandleControllerException(exception);
        }
        return RedirectToAction("Create", "StudentConvocation");
    }

    #endregion
}