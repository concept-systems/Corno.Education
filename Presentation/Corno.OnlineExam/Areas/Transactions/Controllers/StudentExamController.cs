using Corno.Data.Helpers;
using Corno.Data.Reports;
using Corno.Data.ViewModels;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Globals.Enums;
using Corno.Logger;
using Corno.OnlineExam.Areas.Services;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Online_Education.Interfaces;
using Corno.Services.Payment.Interfaces;
using CrystalDecisions.CrystalReports.Engine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using System.Web.Mvc;
using Telerik.Reporting;

namespace Corno.OnlineExam.Areas.Transactions.Controllers;

[Authorize]
public class StudentExamController : ExamController
{
    #region -- Constructors --
    public StudentExamController(ICornoService cornoService, ICoreService coreService,
        ILinkService linkService, IEaseBuzzService easeBuzzService,
        IOnlineEducationStudentService onlineEducationStudentService, ICollege45OptionalSubjectService college45OptionalSubjectService)
        : base(cornoService, coreService, linkService, easeBuzzService, onlineEducationStudentService)
    {
        _cornoService = cornoService;
        _coreService = coreService;
        _linkService = linkService;
        _easeBuzzService = easeBuzzService;
        _onlineEducationStudentService = onlineEducationStudentService;
        _college45OptionalSubjectService = college45OptionalSubjectService;

        _formType = FormType.Exam;
    }
    #endregion

    #region -- Data Members --

    private readonly ICornoService _cornoService;
    private readonly ICoreService _coreService;
    private readonly ILinkService _linkService;
    private readonly IEaseBuzzService _easeBuzzService;
    private readonly IOnlineEducationStudentService _onlineEducationStudentService;
    private readonly ICollege45OptionalSubjectService _college45OptionalSubjectService;

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

        viewModel.TransactionId = existing?.TransactionId;

        ExamFormHelper.GetPrnInfoForPaymentGateway(viewModel);

        // Only for college 45 - Online Education   
        if (viewModel.CollegeId == 45)
        {
            _onlineEducationStudentService.UpdateExamFees(viewModel);
            _college45OptionalSubjectService.UpdateExamOptionalSubjects(viewModel);
        }
    }

    public List<ExamIndexViewModel> GetIndexViewModels(string prn, int serialNo, SessionData session)
    {
        var instanceService = Bootstrapper.Get<IInstanceService>();
        var centerName = session.CenterId > 0 ? $"({session.CenterId}) {session.CenterName}" : "-";
        var viewModels = (from exam in _cornoService.ExamRepository.Get(e => e.PrnNo == prn &&
                                                                             e.Status == StatusConstants.Paid).AsEnumerable()
                          join instance in instanceService.GetQuery().AsEnumerable()
                              on exam.InstanceId equals instance.Id into defaultInstance
                          from instance in defaultInstance.DefaultIfEmpty()
                          select new
                          {
                              exam,
                              instance
                          }).ToList() // Materialize here
            .Select(x => new ExamIndexViewModel
            {
                Id = x.exam.Id,
                FormType = FormType.Exam,
                InstanceId = x.exam.InstanceId,
                InstanceName = $"({x.instance?.Id}) {x.instance?.Name}", // Now safe
                CollegeId = x.exam.CollegeId,
                CollegeName = session.CollegeName,
                CentreId = x.exam.CentreId,
                CentreName = centerName,
                CourseId = session.CourseId,
                CourseName = session.CourseName,
                CoursePartId = x.exam.CoursePartId,
                Prn = session.Prn,
                StudentName = session.StudentName,
                FormDate = x.exam.Date,
                TransactionId = x.exam.TransactionId,
                TotalFee = x.exam.Total
            }).OrderByDescending(d => d.CoursePartId).ToList();

        var coursePartIds = viewModels.Select(p => p.CoursePartId)
            .Distinct();
        var courseParts = _coreService.Tbl_COURSE_PART_MSTR_Repository.Get(p =>
            coursePartIds.Contains(p.Num_PK_COPRT_NO));
        viewModels?.ForEach(p =>
        {
            var coursePart = courseParts.FirstOrDefault(x => x.Num_PK_COPRT_NO == p.CoursePartId);
            p.SerialNo = serialNo++;
            p.CoursePartName = $"({p.CoursePartId}) {coursePart?.Var_COPRT_SHRT_NM}";
        });

        return viewModels;
    }

    private void HandlePaymentApIResponse(FormCollection form)
    {
        _easeBuzzService.PaymentApiResponse(form);

        var prn = form[EaseBuzzConstants.Udf1];
        var transactionId = form[EaseBuzzConstants.TransactionId];
        var paidAmount = form[EaseBuzzConstants.Amount].ToDouble();
        var mobile = form[EaseBuzzConstants.Phone];
        var instanceId = form[EaseBuzzConstants.Udf4].ToInt();
        if (null == transactionId)
            throw new Exception("Invalid transaction Id");
        if (null == prn)
            throw new Exception("Invalid payment prn");

        PaymentSuccess(instanceId, prn, transactionId, paidAmount, mobile, DateTime.Now, _formType);
    }

    #endregion

    #region -- Events --
    // GET: Products
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
                var newViewModel = new ExamViewModel { PrnNo = viewModel.PrnNo };
                // Get Exam form info
                GetExamViewModel(newViewModel);

                viewModel.TransactionId = GetTransactionId();
                viewModel.Status = StatusConstants.InProcess;

                viewModel.Total = newViewModel.Total;

                if (viewModel.CollegeId == 45 && viewModel.Total <= 0)
                {
                    // Bypass payment gateway for college 45 and total fee is 0
                    CreateFormForPaymentGateway(viewModel);
                    PaymentSuccess(viewModel.InstanceId, viewModel.PrnNo, null, 0, viewModel.Mobile, DateTime.Now, _formType);
                    return RedirectToAction("PaymentLogin", "Account", new { area = "Admin", viewModel.PrnNo });
                }

                // Go to payment gateway
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

    // GET: Products/Details/1
    public ActionResult Details(int? formId, int? formType)
    {
        var session = GetSession();
        if (null == session || null == formId || formType != (int)FormType.Exam)
            return View(new ExamDetailViewModel());
        var exam = _cornoService.ExamRepository.Get(e => e.Id == formId).FirstOrDefault();
        var instanceService = Bootstrapper.Get<IInstanceService>();
        var instance = instanceService.GetById(exam?.InstanceId);
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
        try
        {
            var session = GetSession();
            if (null == session)
                return RedirectToAction("Index");

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
        catch (Exception exception)
        {
            HandleControllerException(exception);
            ModelState.AddModelError("Error", exception.Message);
        }

        return RedirectToAction("Index", "StudentExam", new { area = "Transactions" });
    }

    public async Task<ActionResult> AdmitCard()
    {
        try
        {
            if (HttpContext.Session[User.Identity.Name] is not SessionData sessionData)
                throw new Exception("Invalid session data");

            var yrChange = _coreService.Tbl_STUDENT_YR_CHNG_Repository.Get(p =>
                    p.Chr_FK_PRN_NO == sessionData.Prn && p.Num_FK_COL_CD == 45, p => p)
                .OrderByDescending(p => p.Dtm_DTE_UP).FirstOrDefault();
            if (null == yrChange)
                throw new Exception("No year change record found for this PRN.");
            var dto = new HallTicketViewModel
            {
                CollegeId = sessionData.CollegeId,
                CourseId = sessionData.CourseId,
                CoursePartId = yrChange.Num_FK_COPRT_NO,
                CentreId = sessionData.CenterId,
                PrnNo = sessionData.Prn,
                FromSeatNo = yrChange.Num_ST_SEAT_NO.ToString(),
                ToSeatNo = yrChange.Num_ST_SEAT_NO.ToString()
            };

            var hallTicketService = new HallTicketService();
            var report = await hallTicketService.GetCrystalReport(dto, sessionData.InstanceId);

            ViewData["ReportName"] = "Hall Ticket";
            if (HttpContext.Session["CrystalReport"] is ReportDocument existingReport)
            {
                existingReport.Close();
                existingReport.Dispose();
            }
            HttpContext.Session["CrystalReport"] = report;
            HttpContext.Session["HideGroupTree"] = true;

            return RedirectToAction("Details", "Report", new { area = "Reports", reportName = "Hall Ticket" });
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return RedirectToAction("Index", "Home");
    }

    #endregion

    #region -- Payment gateway events --
    // POST: /Reg/Create
    //[HttpPost]
    //[AllowAnonymous]
    public ActionResult OnPaymentSuccess(FormCollection form)
    {
        var prn = form[EaseBuzzConstants.Udf1];
        try
        {
            LogHandler.LogInfo($"OnPaymentSuccess. PRN : {form[EaseBuzzConstants.Udf1]}, " +
                               $"Form Type : {form[EaseBuzzConstants.Udf5]}, " +
                               $"Transaction Id : {form[EaseBuzzConstants.TransactionId]}, " +
                               $"Status : {form["status"]}");

            HandlePaymentApIResponse(form);

            var transactionId = form[EaseBuzzConstants.TransactionId];
            if (User.Identity.IsAuthenticated)
            {
                TempData[ModelConstants.Success] = $"Exam form is submitted successfully. Your payment transaction id is {transactionId}"; ViewBag.Message = "Your payment was successful!";
                ViewBag.UserName = User.Identity.Name;
                var createPath = Url.Action("Create", "StudentExam", new { area = "Transactions" });
                return View(createPath, new ExamViewModel());
            }

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
            LogHandler.LogInfo($"OnPaymentFailure. PRN : {form[EaseBuzzConstants.Udf1]}, Form Type : {form[EaseBuzzConstants.Udf5]}, Transaction Id : {form[EaseBuzzConstants.TransactionId]}, Status : {form["status"]}");

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
            Enum.TryParse(form[EaseBuzzConstants.Udf5], out FormType formType);
            LogHandler.LogInfo($"OnWebHook. PRN : {form[EaseBuzzConstants.Udf1]}, Form Type : {formType}, Transaction Id : {form[EaseBuzzConstants.TransactionId]},  Status : {form["status"]}");

            Thread.Sleep(2000);

            if (form["status"] != "success")
                return RedirectToAction("Create", "StudentExam");

            LogHandler.LogInfo($"OnWebHook => form status : {form["status"]}");
            switch (formType)
            {
                case FormType.Environment:
                    return RedirectToAction("OnWebHook", "StudentEnvironment", new { area = "Transactions", form });
                case FormType.Convocation:
                    return RedirectToAction("OnWebHook", "StudentConvocation", new { area = "Transactions", form });
                default:
                    LogHandler.LogInfo("Before calling Exam WebHook");
                    HandlePaymentApIResponse(form);
                    LogHandler.LogInfo("After calling Exam WebHook");
                    break;
            }
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