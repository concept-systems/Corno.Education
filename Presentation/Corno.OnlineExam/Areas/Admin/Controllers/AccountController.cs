using Corno.Data.Admin;
using Corno.Globals;
using Corno.Globals.Constants;
using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using Microsoft.Owin.Security;
using System;
using System.Data.Entity.Validation;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Web.Mvc;
using System.Web.Security;
using Corno.Data.Helpers;
using Corno.Globals.Enums;
using Corno.Logger;
using Corno.OnlineExam.Areas.Services;
using Corno.OnlineExam.Attributes;
using Corno.Services.Bootstrapper;
using Corno.Services.Core;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Corno.Masters.Interfaces;

namespace Corno.OnlineExam.Areas.Admin.Controllers;

[Authorize]
public class AccountController : Controller
{
    #region -- Constructors --
    public AccountController() : this(new UserManager<ApplicationUser>(new UserStore<ApplicationUser>(new ApplicationDbContext()))
    )
    {
    }

    public AccountController(UserManager<ApplicationUser> userManager)
    {
        UserManager = userManager;
        RoleManager = new RoleManager<IdentityRole>(new RoleStore<IdentityRole>(new ApplicationDbContext()));
    }
    #endregion

    #region -- Data Members --

    private UserManager<ApplicationUser> UserManager { get; set; }
    private RoleManager<IdentityRole> RoleManager { get; set; }
    #endregion

    #region -- Private Methods --

    private bool IsUserAlreadyLoggedIn(string username)
    {
        // Check if the user has an active session by querying your session tracking mechanism
        // For example, you could check if the user session exists in a database table or cache

        // Return true if the user has an active session, false otherwise
        return Session[username] is SessionData;
    }

    private async Task<ApplicationUser> AuthenticateUser(LoginViewModel viewModel)
    {
        if (string.IsNullOrEmpty(viewModel.UserName) || string.IsNullOrEmpty(viewModel.Password))
            throw new Exception("Invalid user name or password.");
        if (viewModel.InstanceId <= 0)
            throw new Exception("Please, select instance.");

        var user = await UserManager.FindAsync(viewModel.UserName, viewModel.Password);
        if (null == user) return null;

        await SignInAsync(user, false);
        // this one if customer wants to remember password: model.RememberMe);
        return user;
    }

    private SessionData CreateStudentSession(LoginViewModel viewModel, ApplicationUser user)
    {
        var coreService = Bootstrapper.Get<ICoreService>();
        var cornoService = Bootstrapper.Get<ICornoService>();
            
        var studentInfoAdr = coreService.Tbl_STUDENT_INFO_ADR_Repository.FirstOrDefault(s =>
                s.Chr_FK_PRN_NO == viewModel.Prn, p => p);
        if (null == studentInfoAdr)
            throw new Exception("Invalid Mobile.");
        var studentCourse = coreService.Tbl_STUDENT_COURSE_Repository.FirstOrDefault(s =>
            s.Chr_FK_PRN_NO.Trim() == studentInfoAdr.Chr_FK_PRN_NO, p => p);
        var studentInfo = coreService.Tbl_STUDENT_INFO_Repository.FirstOrDefault(s =>
            s.Chr_PK_PRN_NO == studentInfoAdr.Chr_FK_PRN_NO, p => p);
        var linkDetail = cornoService.LinkDetailRepository.Get(d => d.Prn == studentInfo.Chr_PK_PRN_NO)
            .OrderByDescending(d => d.ModifiedDate).FirstOrDefault();
        var link = cornoService.LinkRepository.Get(p => p.Id == linkDetail.LinkId, 
            p => new {p.InstanceId}).FirstOrDefault();
        int instanceId = studentCourse?.Num_FK_INST_NO ?? 0;
        //LogHandler.LogInfo($"Student Course: {studentCourse?.Num_FK_INST_NO ?? 0}");
        if (null != link)
        {
            /*var linkDetail = link.LinkDetails.FirstOrDefault(d => d.Prn == studentInfo?.Chr_PK_PRN_NO);
            instanceId = linkDetail?.InstanceId ?? 0;*/
            //LogHandler.LogInfo($"Link: {link.InstanceId ?? 0}, {link.Id}");
            instanceId = link?.InstanceId ?? 0;
        }

        var sessionData = new SessionData
        {
            UserId = user.Id,
            UserName = viewModel.Prn,

            InstanceId = instanceId, //studentCourse?.Num_FK_INST_NO ?? 0,
            InstanceName = string.Empty,
            CollegeId = studentCourse?.Num_ST_COLLEGE_CD ?? -1,
            CollegeName = ExamServerHelper.GetCollegeName(studentCourse?.Num_ST_COLLEGE_CD, coreService),
            CourseId = studentCourse?.Num_FK_CO_CD ?? -1,
            CourseName = studentCourse?.Num_FK_CO_CD.ToInt() > 0 ? ExamServerHelper.GetCourseName(studentCourse.Num_FK_CO_CD, coreService) : string.Empty,
            CenterId = studentCourse?.Num_FK_DistCenter_ID ?? -1,
            CenterName = studentCourse?.Num_FK_DistCenter_ID.ToInt() > 0 ? ExamServerHelper.GetCourseName(studentCourse.Num_FK_DistCenter_ID, coreService) : string.Empty,
                
            Prn = studentInfoAdr.Chr_FK_PRN_NO,
            StudentName = studentInfo?.Var_ST_NM
        };


        Session[ModelConstants.InstanceName] = $"{studentInfo?.Var_ST_NM} - {studentInfoAdr.Chr_FK_PRN_NO}";
        Session[user.UserName] = sessionData;
        HttpContext.Session[ModelConstants.UserId] = user.Id;

        viewModel.InstanceId = instanceId;

        return sessionData;
    }

    private SessionData CreateOtherSession(LoginViewModel viewModel, ApplicationUser user)
    {
        var coreService = Bootstrapper.Get<ICoreService>();
        var instanceService = Bootstrapper.Get<IInstanceService>();

        var sessionData = new SessionData
        {
            UserId = user?.Id,
            UserName = user?.UserName,

            InstanceId = viewModel.InstanceId.ToInt(),
            InstanceName = instanceService.GetById(viewModel.InstanceId)?.Name,
            CollegeId = user?.CollegeId ?? -1,
            CollegeName = user?.CollegeId.ToInt() > 0 ? ExamServerHelper.GetCollegeName(user.CollegeId, coreService) : string.Empty,
            CourseId = user?.CourseId ?? -1,
            CourseName = user?.CourseId.ToInt() > 0 ? ExamServerHelper.GetCourseName(user.CourseId, coreService) : string.Empty,

            Prn = UserManager.IsInRole(user?.Id, RoleConstants.Student) ? ExamServerHelper.GetStudentPrn(user?.PhoneNumber, coreService) : string.Empty
        };
        return sessionData;
    }


    private void StoreSession(LoginViewModel viewModel, ApplicationUser user, UserType userType)
    {
        var sessionData = !string.IsNullOrEmpty(viewModel.Prn) ? 
            CreateStudentSession(viewModel, user) : 
            CreateOtherSession(viewModel, user);
            
        sessionData.UserType = userType;
        Session[user?.UserName] = sessionData;
        Session[ModelConstants.User] = user;

        HttpContext.Session[ModelConstants.UserId] = user?.Id;
        if (viewModel.InstanceId == null) return;

        HttpContext.Session[ModelConstants.InstanceId] = sessionData.InstanceId;
        HttpContext.Session[ModelConstants.InstanceName] = sessionData.InstanceName;
    }

    private void RemoveSession(string userName)
    {
        HttpContext.Session.Remove(userName);

        HttpContext.Session.Remove(ModelConstants.UserId);
        HttpContext.Session.Remove(ModelConstants.CollegeId);
        HttpContext.Session.Remove(ModelConstants.CollegeName);
        HttpContext.Session.Remove(ModelConstants.InstanceId);
        HttpContext.Session.Remove(ModelConstants.InstanceName);
        HttpContext.Session.Remove(ModelConstants.Password);
    }

    public ApplicationUser GetOrCreateUser(LoginViewModel loginViewModel)
    {
        var user = UserManager.FindByName(loginViewModel.Prn);
        if (null != user)
            return user;

        var coreService = Bootstrapper.Get<ICoreService>();
        var studentInfoAdr = coreService.Tbl_STUDENT_INFO_ADR_Repository.Get(s => s.Chr_FK_PRN_NO == loginViewModel.Prn)
            .FirstOrDefault();
        if (studentInfoAdr == null)
            throw new Exception($"Invalid PRN {loginViewModel.Prn}");
        var studentInfo = coreService.Tbl_STUDENT_INFO_Repository.Get(s => s.Chr_PK_PRN_NO == studentInfoAdr.Chr_FK_PRN_NO)
            .FirstOrDefault();
        var studentCourse = coreService.Tbl_STUDENT_COURSE_Repository.Get(s => s.Chr_FK_PRN_NO == studentInfoAdr.Chr_FK_PRN_NO)
            .FirstOrDefault();

        var roleName = "Student";
        user = new ApplicationUser
        {
            UserName = loginViewModel.Prn,
            FirstName = studentInfo?.Var_ST_NM,
            LastName = roleName,

            PhoneNumber = studentInfoAdr.Num_MOBILE,
            Email = studentInfoAdr.Chr_Student_Email,

            Type = roleName,
            CollegeId = studentCourse?.Num_ST_COLLEGE_CD,
            CourseId = studentCourse?.Num_FK_CO_CD,
        };

        var idManager = new IdentityManager();
        var password = $"{loginViewModel.Prn}#987";
        idManager.CreateUser(user, password);

        idManager.AddUserToRole(user.Id, roleName);

        return user;
    }

    #endregion

    #region -- Events --
    //
    // GET: /Account/Login
    [AllowAnonymous]
    public ActionResult Login(string prn, string returnUrl, string error)
    {
        var viewModel = new LoginViewModel();
        //var masterService = (IMasterService)Bootstrapper.GetService(typeof(MasterService));
        var instanceService = Bootstrapper.Get<IInstanceService>();

        if (null != instanceService)
        {
            /*viewModel.Instances = masterService.InstanceRepository.Get(i => i.Status == StatusConstants.Active)
                .OrderByDescending(c => c.SerialNo).ToList();*/
            viewModel.Instances = instanceService.GetViewModelList(p => p.Status == StatusConstants.Active)
                .OrderByDescending(p => p.Id)
                .ToList();
            viewModel.Prn = prn;
            viewModel.UserName = "";
            viewModel.Password = "";
        }

        if(!string.IsNullOrEmpty(error))
            ModelState.AddModelError("Error", error);

        ViewBag.ReturnUrl = returnUrl;
                
        return View("~/Areas/Admin/Views/Account/Login.cshtml", viewModel);
    }

    // POST: /Account/Login
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "LogIn")]
    public async Task<ActionResult> Login(LoginViewModel viewModel, string returnUrl)
    {
        var instanceService = Bootstrapper.Get<IInstanceService>();
        try
        {
            if (!ModelState.IsValid)
                throw new Exception(@"Invalid username or password.");

            if (string.IsNullOrEmpty(viewModel.UserName) || string.IsNullOrEmpty(viewModel.Password))
                throw new Exception("Invalid user name or password.");
            if (viewModel.InstanceId <= 0)
                throw new Exception("Please, select instance.");

            var user = await UserManager.FindAsync(viewModel.UserName, viewModel.Password) ?? throw new Exception("Invalid user name or password.");

            await SignInAsync(user, false);

            // Fill Session Data
            viewModel.Prn = null;
            var userType = user.CollegeId > 0 ? UserType.College : UserType.Admin;
            StoreSession(viewModel, user, userType);

            // this one if customer wants to remember password: model.RememberMe);
            return RedirectToLocal(returnUrl);
            //return RedirectToAction("Index", "Home");
        }
        catch (Exception exception)
        {
            ModelState.AddModelError("Error", LogHandler.GetDetailException(exception).Message);
        }

        if (instanceService == null)
            ModelState.AddModelError("Error", @"Invalid master service");
        //viewModel.Instances = masterService?.InstanceRepository.Get().ToList();
        viewModel.Instances = instanceService?.GetViewModelList(p => p.Status == StatusConstants.Active)
            .OrderByDescending(p => p.Id)
            .ToList();
        // If we got this far, something failed, redisplay form
        return View("~/Areas/Admin/Views/Account/Login.cshtml", viewModel);
        //return View(model);
    }

    //
    // POST: /Account/Login
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "SendOtp")]
    public async Task<ActionResult> SendOtp(LoginViewModel loginViewModel, string returnUrl)
    {
        if (!ModelState.IsValid)
        {
            throw new Exception(ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                .Select(m => m.ErrorMessage).FirstOrDefault());
        }

        try
        {
            var otpService = new OtpService();
            otpService.SendLoginOtp(loginViewModel);

            TempData["Success"] = "Otp is sent to your registered mobile no.";
            loginViewModel.Otp = string.Empty;
        }
        catch (Exception exception)
        {
            ModelState.AddModelError("Error", exception.Message);
            ViewBag.Error = exception.Message;
            LogHandler.LogInfo(exception);
        }

        await Task.Delay(0);

        return RedirectToAction("Login", new {prn = loginViewModel.Prn, error = ViewBag.Error});

        //TempData["Success"] = "Otp is sent to your registered mobile no.";
        //return View("Login", loginViewModel);
        //return View("~/Areas/Admin/Views/Account/Login.cshtml", loginViewModel);
    }

    //
    // GET: /Account/OtpLogin
    [AllowAnonymous]
    public ActionResult OtpLogin(string prn, string returnUrl)
    {
        var loginViewModel = new LoginViewModel
        {
            Prn = prn
        };

        ViewBag.ReturnUrl = returnUrl;
        return View("~/Areas/Admin/Views/Account/Login.cshtml", loginViewModel);
    }

    //
    // POST: /Account/OtpLogin
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "OtpLogin")]
    public async Task<ActionResult> OtpLogin(LoginViewModel loginViewModel, string returnUrl)
    {
        if (!ModelState.IsValid)
            return View("Login", loginViewModel);

        try
        {
            //var validationResult = _apiService.Post("ValidateOtp", model, ApiName.Otp);
            var otpService = new OtpService();
            otpService.ValidateOtp(loginViewModel);

            // OTP verification succeeded, sign in the user and redirect
            /*var claims = new List<Claim>
            {
                new Claim(ClaimTypes.Name, loginViewModel.MobileNo),
                // You can add other claims as needed
            };

            var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationType);
            var principal = new ClaimsPrincipal(identity);
            AuthenticationManager.SignIn(identity);*/

            //FormsAuthentication.SetAuthCookie(loginViewModel.MobileNo, false);
            var user = GetOrCreateUser(loginViewModel);

            await SignInAsync(user, true);

            StoreSession(loginViewModel, user, UserType.Student);

            // this one if customer wants to remember password: model.RememberMe);
            return RedirectToLocal(returnUrl);
        }
        catch (Exception exception)
        {
            ModelState.AddModelError("Error", exception.Message);
            LogHandler.LogError(LogHandler.GetDetailException(exception));
        }
        return View("~/Areas/Admin/Views/Account/Login.cshtml", loginViewModel);
    }

    //[HttpPost]
    [AllowAnonymous]
    //[ValidateAntiForgeryToken]
    public async Task<ActionResult> PaymentLogin(string prn, string returnUrl)
    {
        var loginViewModel = new LoginViewModel { Prn = prn };

        if (!ModelState.IsValid)
        {
            var errorMessage = ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                .Select(m => m.ErrorMessage).FirstOrDefault();
            LogHandler.LogInfo(errorMessage);
            return View("Login", loginViewModel);
        }

        try
        {
            var user = GetOrCreateUser(loginViewModel);
            await SignInAsync(user, false);

            StoreSession(loginViewModel, user, UserType.Student);

            // this one if customer wants to remember password: model.RememberMe);
            return RedirectToLocal(returnUrl);
        }
        catch (Exception exception)
        {
            ModelState.AddModelError("Error", exception.Message);
            LogHandler.LogInfo(exception);
        }
        return View("~/Areas/Admin/Views/Account/Login.cshtml", loginViewModel);
    }

    //
    // GET: /Account/Register
    [AllowAnonymous]
    public ActionResult Register()
    {
        var viewModel = new RegisterViewModel();
        var examService = Bootstrapper.Get<ICoreService>();
        if (null != examService)
        {
            ViewBag.Colleges = examService.TBL_COLLEGE_MSTRRepository.Get().ToList()
                .Select(c => new { ID = c.Num_PK_COLLEGE_CD, Name = c.Var_CL_COLLEGE_NM1, NameWithID = "(" + c.Num_PK_COLLEGE_CD.ToString() + ") " + c.Var_CL_COLLEGE_NM1 })
                .ToList().Distinct().OrderBy(c => c.ID);
            //viewModel.Courses = masterService.CourseRepository.Get().ToList();
        }
        return View(viewModel);
    }

    //POST: /Account/Register
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Register(RegisterViewModel model)
    {
        try
        {
            if (ModelState.IsValid && ValidateViewModel(model))
            {
                var user = new ApplicationUser()
                {
                    UserName = model.UserName,
                    FirstName = model.FirstName,
                    LastName = model.LastName,
                    Email = model.Email,
                    PhoneNumber = model.PhoneNumber,

                    CollegeId = model.CollegeId,
                };
                var idManager = new IdentityManager();
                var result = idManager.CreateUser(user, model.Password);
                if (result)
                {
                    await SignInAsync(user, isPersistent: false);
                    return RedirectToAction("Index", "Home");
                }

                AddErrors(new IdentityResult("Unable to create user"));
            }
        }
        catch (DbEntityValidationException e)
        {
            var sb = new StringBuilder();
            foreach (var eve in e.EntityValidationErrors)
            {
                sb.AppendLine(
                    $"Entity of type \"{eve.Entry.Entity.GetType().Name}\" in state \"{eve.Entry.State}\" has the following validation errors:");
                foreach (var ve in eve.ValidationErrors)
                {
                    sb.AppendLine($"- Property: \"{ve.PropertyName}\", Error: \"{ve.ErrorMessage}\"");
                }
            }
            throw new DbEntityValidationException(sb.ToString(), e);
        }
        // If we got this far, something failed, redisplay form
        var examService = Bootstrapper.Get<ICoreService>();
        if (null != examService)
        {
            ViewBag.Colleges = examService.TBL_COLLEGE_MSTRRepository.Get().ToList()
                .Select(c => new { ID = c.Num_PK_COLLEGE_CD, Name = c.Var_CL_COLLEGE_NM1, NameWithID = "(" + c.Num_PK_COLLEGE_CD.ToString() + ") " + c.Var_CL_COLLEGE_NM1 })
                .ToList().Distinct().OrderBy(c => c.ID);
            //viewModel.Courses = masterService.CourseRepository.Get().ToList();
        }
        //model.Colleges = masterService.CollegeRepository.Get().ToList();
        //model.Courses = masterService.CourseRepository.Get().ToList();
        return View(model);
    }

    //
    // POST: /Account/Disassociate
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Disassociate(string loginProvider, string providerKey)
    {
        var result = await UserManager.RemoveLoginAsync(User.Identity.GetUserId(), new UserLoginInfo(loginProvider, providerKey));
        ManageMessageId? message = result.Succeeded ? ManageMessageId.RemoveLoginSuccess : ManageMessageId.Error;
        return RedirectToAction("Manage", new { Message = message });
    }

    //
    // GET: /Account/Manage
    public ActionResult Manage(ManageMessageId? message)
    {
        ViewBag.StatusMessage =
            message == ManageMessageId.ChangePasswordSuccess ? "Your password has been changed."
            : message == ManageMessageId.SetPasswordSuccess ? "Your password has been set."
            : message == ManageMessageId.RemoveLoginSuccess ? "The external login was removed."
            : message == ManageMessageId.Error ? "An error has occurred."
            : "";
        ViewBag.HasLocalPassword = HasPassword();
        ViewBag.ReturnUrl = Url.Action("Manage");
        return View();
    }

    //
    // POST: /Account/Manage
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Manage(ManageUserViewModel model)
    {
        var hasPassword = HasPassword();
        ViewBag.HasLocalPassword = hasPassword;
        ViewBag.ReturnUrl = Url.Action("Manage");
        if (hasPassword)
        {
            if (ModelState.IsValid)
            {
                var result = await UserManager.ChangePasswordAsync(User.Identity.GetUserId(), model.OldPassword, model.NewPassword);
                if (result.Succeeded)
                {
                    return RedirectToAction("Manage", new { Message = ManageMessageId.ChangePasswordSuccess });
                }

                AddErrors(result);
            }
        }
        else
        {
            // User does not have a password so remove any validation errors caused by a missing OldPassword field
            var state = ModelState["OldPassword"];
            state?.Errors.Clear();

            if (!ModelState.IsValid) return View(model);
            var result = await UserManager.AddPasswordAsync(User.Identity.GetUserId(), model.NewPassword);
            if (result.Succeeded)
            {
                return RedirectToAction("Manage", new { Message = ManageMessageId.SetPasswordSuccess });
            }

            AddErrors(result);
        }

        // If we got this far, something failed, redisplay form
        return View(model);
    }

    //
    // POST: /Account/ExternalLogin
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public ActionResult ExternalLogin(string provider, string returnUrl)
    {
        // Request a redirect to the external login provider
        return new ChallengeResult(provider, Url.Action("ExternalLoginCallback", "Account", new { ReturnUrl = returnUrl }));
    }

    //
    // GET: /Account/ExternalLoginCallback
    [AllowAnonymous]
    public async Task<ActionResult> ExternalLoginCallback(string returnUrl)
    {
        var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync();
        if (loginInfo == null)
        {
            return RedirectToAction("Login");
        }

        // Sign in the user with this external login provider if the user already has a login
        var user = await UserManager.FindAsync(loginInfo.Login);
        if (user != null)
        {
            await SignInAsync(user, isPersistent: false);
            return RedirectToLocal(returnUrl);
        }

        // If the user does not have an account, then prompt the user to create an account
        ViewBag.ReturnUrl = returnUrl;
        ViewBag.LoginProvider = loginInfo.Login.LoginProvider;
        return View("ExternalLoginConfirmation", new ExternalLoginConfirmationViewModel { UserName = loginInfo.DefaultUserName });
    }

    //
    // POST: /Account/LinkLogin
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult LinkLogin(string provider)
    {
        // Request a redirect to the external login provider to link a login for the current user
        return new ChallengeResult(provider, Url.Action("LinkLoginCallback", "Account"), User.Identity.GetUserId());
    }

    //
    // GET: /Account/LinkLoginCallback
    public async Task<ActionResult> LinkLoginCallback()
    {
        var loginInfo = await AuthenticationManager.GetExternalLoginInfoAsync(XsrfKey, User.Identity.GetUserId());
        if (loginInfo == null)
        {
            return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
        }
        var result = await UserManager.AddLoginAsync(User.Identity.GetUserId(), loginInfo.Login);
        if (result.Succeeded)
        {
            return RedirectToAction("Manage");
        }
        return RedirectToAction("Manage", new { Message = ManageMessageId.Error });
    }

    //
    // POST: /Account/ExternalLoginConfirmation
    [HttpPost]
    [AllowAnonymous]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> ExternalLoginConfirmation(ExternalLoginConfirmationViewModel model, string returnUrl)
    {
        if (User.Identity.IsAuthenticated)
        {
            return RedirectToAction("Manage");
        }

        if (ModelState.IsValid)
        {
            // Get the information about the user from the external login provider
            var info = await AuthenticationManager.GetExternalLoginInfoAsync();
            if (info == null)
            {
                return View("ExternalLoginFailure");
            }
            var user = new ApplicationUser() { UserName = model.UserName };
            var result = await UserManager.CreateAsync(user);
            if (result.Succeeded)
            {
                result = await UserManager.AddLoginAsync(user.Id, info.Login);
                if (result.Succeeded)
                {
                    await SignInAsync(user, isPersistent: false);

                    return RedirectToLocal(returnUrl);
                }
            }
            AddErrors(result);
        }

        ViewBag.ReturnUrl = returnUrl;
        return View(model);
    }

    //
    // POST: /Account/LogOff
    //[HttpPost, ActionName("LogOff")]
    //[AllowAnonymous]
    //[ValidateAntiForgeryToken]
    public ActionResult LogOff()
    {
        // Remove Sessions 
        RemoveSession(User.Identity.Name);

        AuthenticationManager.SignOut();

        return RedirectToAction("Login", "Account");
    }

    //
    // GET: /Account/ExternalLoginFailure
    [AllowAnonymous]
    public ActionResult ExternalLoginFailure()
    {
        return View();
    }

    [ChildActionOnly]
    public ActionResult RemoveAccountList()
    {
        var linkedAccounts = UserManager.GetLogins(User.Identity.GetUserId());
        ViewBag.ShowRemoveButton = HasPassword() || linkedAccounts.Count > 1;
        return PartialView("_RemoveAccountPartial", linkedAccounts);
    }

    [Authorize(Roles = "Admin")]

    public ActionResult Index()
    {
        //var dbContext = new ApplicationDbContext();
        //var users = dbContext.Users.ToList();
        // Find the role by name
        var role = RoleManager.FindByName(RoleConstants.Student);
        var users = UserManager.Users.Where(u => u.Roles.All(r => r.RoleId != role.Id)).ToList();
        //var studentUsers = UserManager.Users.Where(u => u.Roles.Any(r => r.RoleId == role.Id)).ToList();
        var model = users.Select(user => new EditUserViewModel(user)).ToList();
        return View(model);
    }


    [Authorize(Roles = "Admin")]
    public ActionResult Edit(EditUserViewModel model, string id, ManageMessageId? message = null)
    {
        if (model == null) throw new ArgumentNullException(nameof(model));

        var db = new ApplicationDbContext();
        var user = db.Users.First(u => u.UserName == id);
        model = new EditUserViewModel(user);
        model = AutoMapperConfig.CornoMapper.Map<EditUserViewModel>(model);
        var examService = Bootstrapper.Get<ICoreService>();
        if (null != examService)
        {
            ViewBag.Colleges = examService.TBL_COLLEGE_MSTRRepository.Get().ToList()
                .Select(c => new { ID = c.Num_PK_COLLEGE_CD, Name = c.Var_CL_COLLEGE_NM1, NameWithID = "(" + c.Num_PK_COLLEGE_CD.ToString() + ") " + c.Var_CL_COLLEGE_NM1 })
                .ToList().Distinct().OrderBy(c => c.ID);
            //viewModel.Courses = masterService.CourseRepository.Get().ToList();
        }
        ViewBag.MessageId = message;
        return View(model);
    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public async Task<ActionResult> Edit(EditUserViewModel model)
    {
        if (ModelState.IsValid)
        {
            var db = new ApplicationDbContext();
            var user = db.Users.First(u => u.UserName == model.UserName);
            user.FirstName = model.FirstName;
            user.LastName = model.LastName;
            user.Email = model.Email;
            user.PhoneNumber = model.PhoneNumber;
            user.CollegeId = model.CollegeId;
            //user.CourseID = model.CourseID;
            db.Entry(user).State = System.Data.Entity.EntityState.Modified;
            await db.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        // If we got this far, something failed, redisplay form
        return View(model);
    }


    [Authorize(Roles = "Admin")]
    public ActionResult Delete(string id = null)
    {
        var db = new ApplicationDbContext();
        var user = db.Users.First(u => u.UserName == id);
        var model = new EditUserViewModel(user);
        if (user == null)
        {
            return HttpNotFound();
        }
        var examService = Bootstrapper.Get<ICoreService>();
        if (null != examService)
        {
            ViewBag.Colleges = examService.TBL_COLLEGE_MSTRRepository.Get().ToList()
                .Select(c => new { ID = c.Num_PK_COLLEGE_CD, Name = c.Var_CL_COLLEGE_NM1, NameWithID = "(" + c.Num_PK_COLLEGE_CD.ToString() + ") " + c.Var_CL_COLLEGE_NM1 })
                .ToList().Distinct().OrderBy(c => c.ID);
            //viewModel.Courses = masterService.CourseRepository.Get().ToList();
        }
        return View(model);
    }


    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize(Roles = "Admin")]
    public ActionResult DeleteConfirmed(string id)
    {
        var db = new ApplicationDbContext();
        var user = db.Users.First(u => u.UserName == id);
        db.Users.Remove(user);
        db.SaveChanges();
        return RedirectToAction("Index");
    }


    //[Authorize(Roles = "Admin")]
    public ActionResult UserRoles(string id)
    {
        var db = new ApplicationDbContext();
        var user = db.Users.First(u => u.UserName == id);
        var model = new SelectUserRolesViewModel(user);
        return View(model);
    }


    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public ActionResult UserRoles(SelectUserRolesViewModel model)
    {
        if (ModelState.IsValid)
        {
            var idManager = new IdentityManager();
            var db = new ApplicationDbContext();
            var user = db.Users.First(u => u.UserName == model.UserName);
            idManager.ClearUserRoles(user.Id);
            foreach (var role in model.Roles)
            {
                if (role.Selected)
                {
                    idManager.AddUserToRole(user.Id, role.RoleName);
                }
            }
            return RedirectToAction("index");
        }
        return View(model);
    }


    [Authorize(Roles = "Admin")]
    public ActionResult RoleCreate()
    {
        return View();
    }

    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult RoleCreate(string roleName)
    {

        Roles.CreateRole(Request.Form["RoleName"]);
        // ViewBag.ResultMessage = "Role created successfully !";

        return RedirectToAction("RoleIndex", "Account");
    }


    [Authorize(Roles = "Admin")]
    public ActionResult RoleIndex()
    {
        var roles = Roles.GetAllRoles();
        return View(roles);
    }

    [Authorize(Roles = "Admin")]
    public ActionResult RoleDelete(string roleName)
    {

        Roles.DeleteRole(roleName);
        // ViewBag.ResultMessage = "Role deleted successfully !";


        return RedirectToAction("RoleIndex", "Account");
    }

    /// <summary>
    /// Create a new role to the user
    /// </summary>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    public ActionResult RoleAddToUser()
    {
        var list = new SelectList(Roles.GetAllRoles());
        ViewBag.Roles = list;

        return View();
    }

    /// <summary>
    /// Add role to the user
    /// </summary>
    /// <param name="roleName"></param>
    /// <param name="userName"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult RoleAddToUser(string roleName, string userName)
    {

        if (Roles.IsUserInRole(userName, roleName))
        {
            ViewBag.ResultMessage = "This user already has the role specified !";
        }
        else
        {
            Roles.AddUserToRole(userName, roleName);
            ViewBag.ResultMessage = "Username added to the role successfully !";
        }

        var list = new SelectList(Roles.GetAllRoles());
        ViewBag.Roles = list;
        return View();
    }

    /// <summary>
    /// Get all the roles for a particular user
    /// </summary>
    /// <param name="userName"></param>
    /// <returns></returns>
    [Authorize(Roles = "Admin")]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult GetRoles(string userName)
    {
        if (!string.IsNullOrWhiteSpace(userName))
        {
            ViewBag.RolesForThisUser = Roles.GetRolesForUser(userName);
            var list = new SelectList(Roles.GetAllRoles());
            ViewBag.Roles = list;
        }
        return View("RoleAddToUser");
    }

    [HttpPost]
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public ActionResult DeleteRoleForUser(string userName, string roleName)
    {

        if (Roles.IsUserInRole(userName, roleName))
        {
            Roles.RemoveUserFromRole(userName, roleName);
            ViewBag.ResultMessage = "Role removed from this user successfully !";
        }
        else
        {
            ViewBag.ResultMessage = "This user doesn't belong to selected role.";
        }
        ViewBag.RolesForThisUser = Roles.GetRolesForUser(userName);
        var list = new SelectList(Roles.GetAllRoles());
        ViewBag.Roles = list;


        return View("RoleAddToUser");
    }
    #endregion

    #region Helpers
    // Used for XSRF protection when adding external logins
    private const string XsrfKey = "XsrfId";

    private IAuthenticationManager AuthenticationManager => HttpContext.GetOwinContext().Authentication;

    private async Task SignInAsync(ApplicationUser user, bool isPersistent)
    {
        AuthenticationManager.SignOut(DefaultAuthenticationTypes.ExternalCookie);
        var identity = await UserManager.CreateIdentityAsync(user, DefaultAuthenticationTypes.ApplicationCookie);
        AuthenticationManager.SignIn(new AuthenticationProperties() { IsPersistent = isPersistent }, identity);
    }

    private void AddErrors(IdentityResult result)
    {
        foreach (var error in result.Errors)
        {
            ModelState.AddModelError("", error);
        }
    }

    private bool HasPassword()
    {
        var user = UserManager.FindById(User.Identity.GetUserId());
        return user?.PasswordHash != null;
    }

    public enum ManageMessageId
    {
        ChangePasswordSuccess,
        SetPasswordSuccess,
        RemoveLoginSuccess,
        Error
    }

    private ActionResult RedirectToLocal(string returnUrl)
    {
        if (Url.IsLocalUrl(returnUrl))
        {
            return Redirect(returnUrl);
        }

        return RedirectToAction("Index", "Home");
    }

    private class ChallengeResult : HttpUnauthorizedResult
    {
        public ChallengeResult(string provider, string redirectUri, string userId = null)
        {
            LoginProvider = provider;
            RedirectUri = redirectUri;
            UserId = userId;
        }

        private string LoginProvider { get; }
        private string RedirectUri { get; }
        private string UserId { get; }

        public override void ExecuteResult(ControllerContext context)
        {
            var properties = new AuthenticationProperties() { RedirectUri = RedirectUri };
            if (UserId != null)
            {
                properties.Dictionary[XsrfKey] = UserId;
            }
            context.HttpContext.GetOwinContext().Authentication.Challenge(properties, LoginProvider);
        }
    }

    public ActionResult ResetPassword()
    {
        return View();

    }
    [HttpPost]
    public async Task<ActionResult> ResetPassword(ResetPasswordViewModel model)
    {
        if (ModelState.IsValid)
        {

            var context = new ApplicationDbContext();
            var store = new UserStore<ApplicationUser>(context);
            var userManager = new UserManager<ApplicationUser>(store);

            var userId = User.Identity.GetUserId();//"<YourLogicAssignsRequestedUserId>";
            var cUser = await store.FindByIdAsync(userId);
            //var passhash = await store.GetPasswordHashAsync(cUser);
            //var oldpassword = model.Password.ToString();
            //String hashedoldPassword = UserManager.PasswordHasher.HashPassword(oldpassword);
            var newPassword = model.NewPassword; //"<PasswordAsTypedByUser>";
            var hashedNewPassword = userManager.PasswordHasher.HashPassword(newPassword);
            //if (passhash == hashedoldPassword)
            //    {
            await store.SetPasswordHashAsync(cUser, hashedNewPassword);
            await store.UpdateAsync(cUser);
            //}
            //else
            //{
            //    ModelState.AddModelError("Password", "Plese Enter correct Old Password");
            //}

            ViewBag.successMessage = "Password reset successfully.";
        }
        return View(model);
    }

    public ActionResult UserProfile()
    {
        //string a;
        //a = System.Security.Principal.WindowsIdentity.GetCurrent().Name.ToString();
        var currentUser = UserManager.FindById(User.Identity.GetUserId());

        var viewModel = new UserProfileViewModel
        {
            FirstName = currentUser.FirstName,
            LastName = currentUser.LastName,
            Email = currentUser.Email
        };
        return View(viewModel);
    }
    private bool ValidateViewModel(RegisterViewModel viewModel)
    {
        if (null != viewModel.CollegeId) return true;

        ModelState.AddModelError("Error", @"College is required");
        return false;
    }
    #endregion

    protected override void Dispose(bool disposing)
    {
        if (disposing && UserManager != null)
        {
            UserManager.Dispose();
            UserManager = null;
        }
        base.Dispose(disposing);
    }
}