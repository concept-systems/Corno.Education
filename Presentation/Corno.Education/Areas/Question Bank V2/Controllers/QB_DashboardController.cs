using System;
using System.Linq;
using System.Web.Mvc;
using Corno.Data.Corno.Question_Bank_V2.Models;
using Corno.Data.Corno.Question_Bank_V2.Dtos;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Education.Controllers;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Corno.Question_Bank_V2.Interfaces;

namespace Corno.Education.Areas.Question_Bank_V2.Controllers
{
    [Authorize]
    public class QB_DashboardController : CornoController
    {
        private readonly IQB_QuestionBankService _questionBankService;
        private readonly IQB_AppointmentService _appointmentService;
        private readonly IMainService<QB_Paper> _paperService;
        
        public QB_DashboardController()
        {
            _questionBankService = Bootstrapper.Get<IQB_QuestionBankService>();
            _appointmentService = Bootstrapper.Get<IQB_AppointmentService>();
            _paperService = Bootstrapper.Get<IMainService<QB_Paper>>();
        }
        
        public ActionResult Index()
        {
            var sessionData = Session[User.Identity.Name] as SessionData;
            var instanceId = sessionData?.InstanceId ?? 0;
            var userId = User.Identity.Name;
            
            // Check user role
            var isSetter = User.IsInRole("Question Setter");
            var isChecker = User.IsInRole("Question Checker");
            var isModerator = User.IsInRole("Moderator");
            
            if (isSetter)
                return RedirectToAction("SetterDashboard");
            if (isChecker)
                return RedirectToAction("CheckerDashboard");
            if (isModerator)
                return RedirectToAction("ModeratorDashboard");
            
            // Default admin dashboard
            return View("AdminDashboard", GetAdminDashboardData(instanceId));
        }
        
        public ActionResult SetterDashboard()
        {
            var sessionData = Session[User.Identity.Name] as SessionData;
            var instanceId = sessionData?.InstanceId ?? 0;
            var userId = User.Identity.Name;
            
            var model = new SetterDashboardDto
            {
                TotalQuestions = _questionBankService.GetQuery()
                    .Count(q => q.InstanceId == instanceId && 
                               q.SetterUserId == userId &&
                               q.Status != StatusConstants.Deleted),
                PendingQuestions = _questionBankService.GetQuery()
                    .Count(q => q.InstanceId == instanceId && 
                               q.SetterUserId == userId &&
                               q.Status == StatusConstants.Draft),
                SubmittedQuestions = _questionBankService.GetQuery()
                    .Count(q => q.InstanceId == instanceId && 
                               q.SetterUserId == userId &&
                               q.Status == StatusConstants.Submitted),
                ApprovedQuestions = _questionBankService.GetQuery()
                    .Count(q => q.InstanceId == instanceId && 
                               q.SetterUserId == userId &&
                               q.Status == StatusConstants.Approved),
                RecentAppointments = _appointmentService.GetAppointmentsForUser(userId, instanceId, "Question Setter")
                    .Take(5)
                    .ToList()
            };
            
            return View(model);
        }
        
        public ActionResult CheckerDashboard()
        {
            var sessionData = Session[User.Identity.Name] as SessionData;
            var instanceId = sessionData?.InstanceId ?? 0;
            var userId = User.Identity.Name;
            
            var model = new CheckerDashboardDto
            {
                PendingReview = _questionBankService.GetQuery()
                    .Count(q => q.InstanceId == instanceId && 
                               q.CheckerUserId == userId &&
                               q.Status == StatusConstants.Submitted),
                ReviewedToday = _questionBankService.GetQuery()
                    .Count(q => q.InstanceId == instanceId && 
                               q.CheckerUserId == userId &&
                               q.Status == StatusConstants.Reviewed &&
                               q.ModifiedDate.HasValue &&
                               q.ModifiedDate.Value.Date == DateTime.Today),
                TotalReviewed = _questionBankService.GetQuery()
                    .Count(q => q.InstanceId == instanceId && 
                               q.CheckerUserId == userId &&
                               q.Status == StatusConstants.Reviewed),
                RecentAppointments = _appointmentService.GetAppointmentsForUser(userId, instanceId, "Question Checker")
                    .Take(5)
                    .ToList()
            };
            
            return View(model);
        }
        
        public ActionResult ModeratorDashboard()
        {
            var sessionData = Session[User.Identity.Name] as SessionData;
            var instanceId = sessionData?.InstanceId ?? 0;
            var userId = User.Identity.Name;
            
            var model = new ModeratorDashboardDto
            {
                TotalPapers = _paperService.GetQuery()
                    .Count(p => p.InstanceId == instanceId && 
                               p.ModeratorUserId == userId &&
                               p.Status != StatusConstants.Deleted),
                GeneratedPapers = _paperService.GetQuery()
                    .Count(p => p.InstanceId == instanceId && 
                               p.ModeratorUserId == userId &&
                               p.Status == StatusConstants.Generated),
                DrawnPapers = _paperService.GetQuery()
                    .Count(p => p.InstanceId == instanceId && 
                               p.ModeratorUserId == userId &&
                               p.Status == StatusConstants.Drawn),
                RecentAppointments = _appointmentService.GetAppointmentsForUser(userId, instanceId, "Moderator")
                    .Take(5)
                    .ToList()
            };
            
            return View(model);
        }
        
        private AdminDashboardDto GetAdminDashboardData(int instanceId)
        {
            return new AdminDashboardDto
            {
                TotalQuestions = _questionBankService.GetQuery()
                    .Count(q => q.InstanceId == instanceId && q.Status != StatusConstants.Deleted),
                ApprovedQuestions = _questionBankService.GetQuery()
                    .Count(q => q.InstanceId == instanceId && q.Status == StatusConstants.Approved),
                TotalAppointments = _appointmentService.GetQuery()
                    .Count(a => a.InstanceId == instanceId && a.Status != StatusConstants.Deleted),
                TotalPapers = _paperService.GetQuery()
                    .Count(p => p.InstanceId == instanceId && p.Status != StatusConstants.Deleted)
            };
        }
    }
}
