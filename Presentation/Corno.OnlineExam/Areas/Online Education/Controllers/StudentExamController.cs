using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Threading;
using System.Web.Mvc;
using Corno.Data.Corno;
using Corno.Data.Helpers;
using Corno.Data.Operation;
using Corno.Data.Payment;
using Corno.Data.ViewModels;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Globals.Enums;
using Corno.Logger;
using Corno.OnlineExam.Areas.Transactions.Controllers;
using Corno.OnlineExam.Helpers;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Online_Education.Interfaces;
using Corno.Services.Payment.Interfaces;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Telerik.Reporting;

namespace Corno.OnlineExam.Areas.Online_Education.Controllers;

[Authorize]
public class OnlineExamController : ExamController
{
    #region -- Constructors --
    public OnlineExamController(ICornoService cornoService, 
        ICoreService coreService, ILinkService linkService, IEaseBuzzService easeBuzzService,
        IOnlineEducationStudentService onlineEducationStudentService)
        : base(cornoService, coreService, linkService, easeBuzzService, onlineEducationStudentService)
    {
        _cornoService = cornoService;
        _coreService = coreService;
        _linkService = linkService;
        _easeBuzzService = easeBuzzService;

        _formType = FormType.Exam;
    }
    #endregion

    #region -- Data Members --

    private readonly ICornoService _cornoService;
    private readonly ICoreService _coreService;
    private readonly ILinkService _linkService;
    private readonly IEaseBuzzService _easeBuzzService;

    private readonly FormType _formType;

    #endregion

    #region -- Private Methods --
    private void GetExamViewModel(ExamViewModel viewModel)
    {
        var link = _linkService.GetExamLink(viewModel.PrnNo, _formType);
        //LogHandler.LogInfo($"Link Id : {link.Id}, Link Instance Id : {link.InstanceId}, Form Type : {_formType}");
        if (null == link)
            throw new Exception("No Link is generated for you to fill the form.");
        var linkDetail = link.LinkDetails.FirstOrDefault(d => d.Prn == viewModel.PrnNo);
        if (null == linkDetail)
            throw new Exception("No Link details are generated for you to fill the form 1.");

        // Check whether exam form is already filled.
        var existing = _cornoService.ExamRepository.Get(e => e.InstanceId == link.InstanceId &&
                                                             e.CollegeId == link.CollegeId &&
                                                             e.CourseId == link.CourseId &&
                                                             e.CoursePartId == link.CoursePartId &&
                                                             e.PrnNo == viewModel.PrnNo).FirstOrDefault();
        if (existing is { Status: StatusConstants.Paid })
            throw new Exception("Exam form is already filled for this PRN in this instance.");

        viewModel.InstanceId = link.InstanceId ?? 0;
        viewModel.CollegeId = link.CollegeId ?? 0;
        viewModel.CentreId = link.CentreId ?? 0;
        viewModel.CourseId = link.CourseId ?? 0;
        viewModel.CoursePartId = link.CoursePartId ?? 0;
        viewModel.BranchId = linkDetail.BranchId ?? 0;
        viewModel.ShowBranchCombo = false;

        ExamFormHelper.GetPrnInfoForPaymentGateway(viewModel);
    }

    public List<ExamIndexViewModel> GetIndexViewModels(string prn, int serialNo, SessionData session)
    {
        var instanceService = Bootstrapper.Get<IInstanceService>();
        var viewModels = from exam in _cornoService.ExamRepository.Get(e => e.PrnNo == prn && e.Status == StatusConstants.Paid)
            join coursePart in _coreService.Tbl_COURSE_PART_MSTR_Repository.Get()
                on exam.CoursePartId equals coursePart.Num_PK_COPRT_NO into defaultCoursePart
            from coursePart in defaultCoursePart.DefaultIfEmpty()
            join instance in instanceService.GetQuery()
                on exam.InstanceId equals instance.Id into defaultInstance
            from instance in defaultInstance.DefaultIfEmpty()
            select new ExamIndexViewModel
            {
                //SerialNo = serialNo++,
                Id = exam.Id,
                FormType = FormType.Exam,
                InstanceId = exam.InstanceId,
                InstanceName = $"({instance.Id}) {instance.Name}",
                CollegeId = exam.CollegeId,
                CollegeName = session.CollegeName,
                CentreId = exam.CentreId,
                CentreName = session.CenterId > 0 ? $"({session.CenterId}) {session.CenterName}" : "-",
                CourseId = session.CourseId,
                CourseName = session.CourseName,
                CoursePartId = exam.CoursePartId,
                CoursePartName = $"({exam.CoursePartId}) {coursePart.Var_COPRT_SHRT_NM}",
                Prn = session.Prn,
                StudentName = session.StudentName,
                FormDate = exam.Date,
                TransactionId = exam.TransactionId,
                TotalFee = exam.Total
            };
        return viewModels.OrderByDescending(d => d.CoursePartId).ToList();
        /*var instanceService = Bootstrapper.Get<IInstanceService>();
        var viewModels = _cornoService.ExamRepository.Get(e => e.PrnNo == prn && e.Status == StatusConstants.Paid)
            .Select(e =>
            {
                var coursePartName = ExamServerHelper.GetCoursePartName(e.CoursePartId, _coreService);
                var instance = instanceService.GetById(e.InstanceId);
                return new ExamIndexViewModel
                {
                    SerialNo = serialNo++,
                    Id = e.Id,
                    FormType = FormType.Exam,
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

                    FormDate = e.Date,
                    TransactionId = e.TransactionId,
                    TotalFee = e.Total
                };
            }).OrderByDescending(d => d.CoursePartId)
            .ToList();
        return viewModels;*/
    }

    #endregion

    #region -- Events --
    public ActionResult Index()
    {
        try
        {
            var session = GetSession();
            if (null == session)
                return View(new List<ExamIndexViewModel>());
            var prn = session.Prn;
            var serialNo = 1;

            var exams = GetIndexViewModels(prn, serialNo, session);
            serialNo += exams.Count;
            var environments = StudentEnvironmentController.GetIndexViewModels(prn, serialNo, session);
            serialNo += exams.Count;
            var convocations = StudentConvocationController.GetIndexViewModels(prn, serialNo, session);
            exams.AddRange(environments);
            exams.AddRange(convocations);

            return View(exams);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(new List<ExamIndexViewModel>());
    }

    // GET: /Reg/Create
    [Authorize]
    public override ActionResult Create()
    {
        var viewModel = new ExamViewModel();
        try
        {
            if (!(HttpContext.Session[User.Identity.Name] is SessionData sessionData))
                throw new Exception("Invalid session data");

            //if (IsFormAlreadySubmitted(out var transactionId))
            //{
            //    TempData[ModelConstants.Success] = $"Already submitted. Your payment transaction id is {transactionId}";
            //    return RedirectToAction("PaymentLogin", "Account", new { area = "Admin", prn = sessionData.Prn });
            //}

            viewModel.PrnNo = sessionData.Prn;

            ViewBag.Disabled = false;

            // Get Exam form info
            GetExamViewModel(viewModel);
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
    public override ActionResult Create(ExamViewModel viewModel, string submitType)
    {
        try
        {
            if (string.IsNullOrEmpty(viewModel.PrnNo))
                throw new Exception("Invalid PRN.");

            if (ModelState.IsValid)
            {
                viewModel.Status = StatusConstants.InProcess;
                viewModel.TransactionId = GetTransactionId();

                var newViewModel = new ExamViewModel
                {
                    PrnNo = viewModel.PrnNo
                };
                // Get Exam form info
                GetExamViewModel(newViewModel);
                //ExamFormHelper.GetFeeStructure(viewModel, viewModel.InstanceId);
                viewModel.Total = newViewModel.Total;

                var result = Pay(viewModel);

                return Content(result);
            }

            ViewData["Error"] = "Error message text.";
            ViewBag.Disabled = true;
            return View(viewModel);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        ViewBag.Disabled = true;
        return View(viewModel);
    }

    /*// GET: /Reg/Create
    [Authorize]
    public override ActionResult Create()
    {
        var viewModel = new ExamViewModel();
        try
        {
            if (!(HttpContext.Session[User.Identity.Name] is SessionData sessionData))
                throw new Exception("Invalid session data");

            /*var existingForm = _cornoService.ExamRepository.Get(e => e.PrnNo == sessionData.Prn && e.Status == StatusConstants.InProcess)
                .FirstOrDefault();
            if(null != existingForm)
                GetTransactionDetails(existingForm);#1#

            viewModel.PrnNo = sessionData.Prn;

            ViewBag.Disabled = false;

            // Get Exam form info
            GetExamViewModel(viewModel);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(viewModel);
    }*/

    /*// POST: /Reg/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public override ActionResult Create(ExamViewModel viewModel, string submitType)
    {
        try
        {
            if (string.IsNullOrEmpty(viewModel.PrnNo))
                throw new Exception("Invalid PRN.");

            if (ModelState.IsValid)
            {
                // First delete previous In-Process forms
                var existingForms = _cornoService.ExamRepository.Get(e => e.PrnNo == viewModel.PrnNo
                                                                     && e.Status == StatusConstants.InProcess)
                    .ToList();
                if (existingForms.Count > 0)
                {
                    foreach (var examForm in existingForms)
                        _cornoService.ExamRepository.Delete(examForm);
                    _cornoService.Save();
                }

                // Generate new form with selected subjects from view model
                var model = GetExamModel(viewModel);
                model.Status = StatusConstants.InProcess;

                model.TransactionId = GetTransactionId();

                // Save the college details
                _cornoService.ExamRepository.Add(model);
                _cornoService.Save();

                // Generate payment request
                viewModel.TransactionId = model.TransactionId;
                var result = PaymentApi(viewModel);
                
                return Content(result);
            }

            ViewData["Error"] = "Error message text.";
            ViewBag.Disabled = true;
            return View(viewModel);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        ViewBag.Disabled = true;
        return View(viewModel);
    }*/

    // GET: Products/Details/1
    public ActionResult Details(int? formId, int? formType)
    {
        var session = GetSession();
        if (null == session || null == formId || formType != (int)FormType.Exam)
            return View(new ExamDetailViewModel());
        var exam = _cornoService.ExamRepository.Get(e => e.Id == formId).FirstOrDefault();
        var instanceService = Bootstrapper.Get<IInstanceService>();
        var instance = instanceService.GetById(exam.InstanceId);
        var coursePartName = ExamServerHelper.GetCoursePartName(exam?.CoursePartId, _coreService);
        var branchName = ExamServerHelper.GetBranchName(exam?.BranchId, _coreService);

        // TODO : Testing for transaction API
        //GetTransactionDetails(exam);

        var viewModel = new ExamDetailViewModel
        {
            Id = exam?.Id,
            InstanceId = exam?.InstanceId,
            InstanceName = $"({instance?.Id}) {instance?.Name}",
            CollegeId = exam?.CollegeId,
            CollegeName = $"({session.CollegeId}) {session.CollegeName}",
            CentreId = exam?.CentreId,
            CentreName = session.CenterId > 0 ? $"({session.CenterId}) {session.CenterName}" : "-",
            CourseId = session.CourseId,
            CourseName = $"({session.CourseId}) {session.CourseName}",
            CoursePartId = exam?.CoursePartId,
            CoursePartName = $"({exam?.CoursePartId}) {coursePartName}",
            BranchId = exam?.BranchId,
            BranchName = $"({exam?.BranchId}) {branchName}",

            Prn = session.Prn,
            StudentName = session.StudentName,

            FormDate = exam?.Date,
            Fee = exam?.Total ?? 0,
            TransactionId = exam?.TransactionId,
        };

        if (exam?.ExamSubjects == null)
            return View(viewModel);

        foreach (var detail in exam.ExamSubjects)
        {
            var partName = ExamServerHelper.GetCoursePartName(detail?.CoursePartId, _coreService);

            var subjectName =
                $"({detail?.SubjectCode}) {ExamServerHelper.GetSubjectName(detail?.SubjectCode, _coreService)}";
            viewModel.Subjects.Add(new KeyValuePair<string, string>(partName, subjectName));
        }

        return View(viewModel);
    }

    public ActionResult Receipt(int instanceId, string prn, int formType)
    {
        var session = GetSession();
        if (null == session)
            return RedirectToAction("Index");
        /*if(null == formType)
            return RedirectToAction("Index");*/

        Report report = (FormType)formType switch
        {
            FormType.Exam => new Corno.Reports.Exam.ReceiptRpt(instanceId, prn),
            FormType.Environment => new Corno.Reports.Environment.ReceiptRpt(instanceId, prn),
            FormType.Convocation => new Corno.Reports.Convocation.ReceiptRpt(instanceId, prn),
            _ => null
        };

        Session[ModelConstants.Report] = report;

        return RedirectToAction("Details", "Report", new { area = "Reports" });
    }

    #endregion

    #region -- Payment gateway events --
    // POST: /Reg/Create
    //[HttpPost]
    //[AllowAnonymous]
    public ActionResult OnPaymentSuccess(FormCollection form)
    {
        //// If no URL is stored, redirect to a default page
        //return RedirectToAction("Index", "Home");

        var returnUrl = HttpContext.Session["ReturnUrl"];
        var returnPath = TempData["ReturnPath"];

        var prn = form[EaseBuzzConstants.Udf1];
        try
        {
            _easeBuzzService.PaymentApiResponse(form);

            var transactionId = form[EaseBuzzConstants.TransactionId];
            var paidAmount = form[EaseBuzzConstants.Amount].ToDouble();
            var mobile = form[EaseBuzzConstants.Phone];
            var instanceId = form[EaseBuzzConstants.Udf4].ToInt();
            if (null == transactionId)
                throw new Exception("Invalid transaction Id");
            if (null == prn)
                throw new Exception("Invalid payment prn");

            PaymentSuccess(instanceId, prn, transactionId, paidAmount, mobile, DateTime.Now, _formType);

            TempData[ModelConstants.Success] = $"Exam form is submitted successfully. Your payment transaction id is {transactionId}";

            return RedirectToAction("PaymentLogin", "Account", new { area = "Admin", prn });
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            HandleControllerException(exception);

            TempData[ModelConstants.Error] = exception.Message;
        }
        return RedirectToAction("PaymentLogin", "Account", new { area = "Admin", prn });
    }

    //[HttpPost]
    [AllowAnonymous]
    public ActionResult OnPaymentFailure(FormCollection form)
    {
        try
        {
            var transactionId = form[EaseBuzzConstants.TransactionId];
            //var paidAmount = form[EaseBuzzConstants.Amount].ToDouble();
            var prn = form[EaseBuzzConstants.Udf1];
            var mobile = form[EaseBuzzConstants.Phone];
            //var instanceId = form[EaseBuzzConstants.Udf4].ToInt();
            //var status = form[EaseBuzzConstants.Status];
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
        return RedirectToAction("Create", "StudentExam");

        /*//var sessionData = GetSession();

        /#1#/ Create an authentication ticket using the mobile number as the identifier
        var authTicket = new FormsAuthenticationTicket(
            1,                            // Version
            mobile,                // Mobile number (identifier)
            DateTime.Now,                 // Issue time
            DateTime.Now.AddMinutes(30),  // Expiration time
            false,                        // Persistent
            null                          // User roles (if applicable)
        );

        // Encrypt the ticket and add it to a cookie
        string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
        var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
        Response.Cookies.Add(authCookie);

        // Set the user as authenticated
        HttpContext.User = new System.Security.Principal.GenericPrincipal(
            new FormsIdentity(authTicket), null);#1#

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, mobile),
            // Add any additional claims if needed
        };

        var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

        var authProps = new AuthenticationProperties
        {
            IsPersistent = false, // You can set this to true for persistent login
        };

        var authenticationManager = HttpContext.GetOwinContext().Authentication;
        authenticationManager.SignIn(authProps, identity);

        // Redirect to the authorized action
        return RedirectToAction("Create");*/

        //return RedirectToAction("PaymentLogin", "Account", new { area = "Admin", mobileNo = mobile });
        //return View("Views/Home/Index");
    }

    [AllowAnonymous]
    public ActionResult OnWebHook(FormCollection form)
    {
        try
        {
            Thread.Sleep(2000);

            var transactionId = form[EaseBuzzConstants.TransactionId];
            var paidAmount = form[EaseBuzzConstants.Amount].ToDouble();
            var prn = form[EaseBuzzConstants.Udf1];
            var mobile = form[EaseBuzzConstants.Phone];
            var instanceId = form[EaseBuzzConstants.Udf4].ToInt();
            var status = form[EaseBuzzConstants.Status];
            LogHandler.LogInfo($"Web hook =>Status : {status}, Instance Id : {instanceId}, Transaction Id : {transactionId}, Mobile : {mobile}, PRN : {prn}");

            if (string.Equals(status, StatusConstants.Success, StringComparison.CurrentCultureIgnoreCase))
                PaymentSuccess(instanceId, prn, transactionId, paidAmount, mobile, DateTime.Now, _formType);

            //_easeBuzzService.PaymentApiResponse(form);

            //TempData[ModelConstants.Success] = $"Exam form is submitted successfully. Your payment transaction id is {transactionId}";

            //return RedirectToAction("PaymentLogin", "Account", new { area = "Admin"/*, mobileNo = mobile*/ });
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            HandleControllerException(exception);
        }

        return RedirectToAction("Create", "StudentExam");
    }

    /*// POST: /Reg/Create
    //[HttpPost]
    [AllowAnonymous]
    public ActionResult OnPaymentSuccess(FormCollection form)
    {
        try
        {
            _easyBuzzService.PaymentApiResponse(form);

            var transactionId = form["txnid"];
            var prn = form["udf1"];
            var mobile = form["phone"];
            if (null == transactionId)
                throw new Exception("Invalid transaction Id");
            if (null == prn)
                throw new Exception("Invalid payment prn");

            // Update Links
            _linkService.UpdatePayment(prn, transactionId, _formType);
            // Update Exam form
            var exam = UpdatePayment(prn, transactionId);
            // Add to APP_TEMP
            AddExamInExamServer(exam);

            // Send payment sms
            _linkService.SendPaymentSms(mobile, transactionId, _formType);

            TempData[ModelConstants.Success] = $"Exam form is submitted successfully. Your payment transaction id is {transactionId}";

            return RedirectToAction("PaymentLogin", "Account", new { area = "Admin", prn = prn });
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            HandleControllerException(exception);
        }
        return RedirectToAction("Create", "StudentExam");
    }

    //[HttpPost]
    [AllowAnonymous]
    public ActionResult OnPaymentFailure(FormCollection form)
    {
        try
        {
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
        return RedirectToAction("Create", "StudentExam");

        /#1#/var sessionData = GetSession();

        /#2#/ Create an authentication ticket using the mobile number as the identifier
        var authTicket = new FormsAuthenticationTicket(
            1,                            // Version
            mobile,                // Mobile number (identifier)
            DateTime.Now,                 // Issue time
            DateTime.Now.AddMinutes(30),  // Expiration time
            false,                        // Persistent
            null                          // User roles (if applicable)
        );

        // Encrypt the ticket and add it to a cookie
        string encryptedTicket = FormsAuthentication.Encrypt(authTicket);
        var authCookie = new HttpCookie(FormsAuthentication.FormsCookieName, encryptedTicket);
        Response.Cookies.Add(authCookie);

        // Set the user as authenticated
        HttpContext.User = new System.Security.Principal.GenericPrincipal(
            new FormsIdentity(authTicket), null);#2#

        var claims = new List<Claim>
        {
            new Claim(ClaimTypes.Name, mobile),
            // Add any additional claims if needed
        };

        var identity = new ClaimsIdentity(claims, DefaultAuthenticationTypes.ApplicationCookie);

        var authProps = new AuthenticationProperties
        {
            IsPersistent = false, // You can set this to true for persistent login
        };

        var authenticationManager = HttpContext.GetOwinContext().Authentication;
        authenticationManager.SignIn(authProps, identity);

        // Redirect to the authorized action
        return RedirectToAction("Create");#1#

        //return RedirectToAction("PaymentLogin", "Account", new { area = "Admin", mobileNo = mobile });
        //return View("Views/Home/Index");
    }

    [AllowAnonymous]
    public ActionResult OnWebHook(FormCollection form)
    {
        try
        {
            _easyBuzzService.PaymentApiResponse(form);


            //TempData[ModelConstants.Success] = $"Exam form is submitted successfully. Your payment transaction id is {transactionId}";

            return RedirectToAction("PaymentLogin", "Account", new { area = "Admin"/*, mobileNo = mobile#1# });
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            HandleControllerException(exception);
        }
        return RedirectToAction("Create", "StudentExam");
    }*/

    #endregion
}