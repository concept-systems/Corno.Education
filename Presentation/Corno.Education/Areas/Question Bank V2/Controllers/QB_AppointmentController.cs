using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Corno.Data.Corno.Question_Bank_V2.Models;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Logger;
using Corno.Education.Controllers;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;
using Corno.Services.Corno.Question_Bank_V2.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Corno.Education.Areas.Question_Bank_V2.Controllers
{
    [Authorize]
    public class QB_AppointmentController : CornoController
    {
        private readonly IQB_AppointmentService _appointmentService;
        private readonly IFacultyService _facultyService;
        private readonly IStructureService _structureService;
        private readonly IStaffService _staffService;

        public QB_AppointmentController()
        {
            _appointmentService = Bootstrapper.Get<IQB_AppointmentService>();
            _facultyService = Bootstrapper.Get<IFacultyService>();
            _structureService = Bootstrapper.Get<IStructureService>();
            _staffService = Bootstrapper.Get<IStaffService>();
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult Create()
        {
            var model = new QB_Appointment
            {
                AppointmentDate = DateTime.Now.AddDays(7),
                NoOfPapers = 1
            };

            LoadViewBagData();
            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Create(QB_Appointment model, List<int> setterStaffIds,
            List<int> checkerStaffIds, List<int> moderatorStaffIds)
        {
            if (!ModelState.IsValid)
            {
                LoadViewBagData();
                return View(model);
            }

            try
            {
                var sessionData = Session[User.Identity.Name] as SessionData;
                var instanceId = sessionData?.InstanceId ?? 0;
                model.InstanceId = instanceId;

                // Create appointment
                _appointmentService.CreateAppointment(model, User.Identity.Name);

                // Assign roles
                if (setterStaffIds != null && setterStaffIds.Any())
                {
                    _appointmentService.AssignRoles(model, setterStaffIds,
                        checkerStaffIds ?? new List<int>(),
                        moderatorStaffIds ?? new List<int>());
                }

                // Generate login credentials
                _appointmentService.GenerateLoginCredentials(model);

                TempData["Success"] = "Appointment created successfully.";
                return RedirectToAction("Index");
            }
            catch (Exception ex)
            {
                HandleControllerException(ex);
                LoadViewBagData();
                return View(model);
            }
        }

        public ActionResult View(int? id)
        {
            if (id == null)
                return new HttpStatusCodeResult(400);

            var model = _appointmentService.GetById(id);
            if (model == null)
                return HttpNotFound();

            return View(model);
        }

        [HttpPost]
        public ActionResult SendNotifications(int id, string notificationType)
        {
            try
            {
                var appointment = _appointmentService.GetById(id);
                if (appointment == null)
                    return Json(new { success = false, message = "Appointment not found." });

                _appointmentService.SendNotifications(appointment, notificationType);

                return Json(new { success = true, message = $"{notificationType} notifications sent successfully." });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        [HttpPost]
        public ActionResult GetAppointments([DataSourceRequest] DataSourceRequest request, int? instanceId)
        {
            try
            {
                var sessionData = Session[User.Identity.Name] as SessionData;
                var instId = instanceId ?? sessionData?.InstanceId ?? 0;

                var appointments = _appointmentService.Get(a => a.InstanceId == instId &&
                                                               a.Status != StatusConstants.Deleted, p => p,
                                                          a => a.OrderByDescending(x => x.CreatedDate));

                var data = appointments.Select(a => new
                {
                    a.Id,
                    a.AppointmentCode,
                    SubjectName = a.SubjectName ?? "N/A",
                    a.NoOfPapers,
                    a.AppointmentDate,
                    a.Status,
                    a.EmailSent,
                    a.SmsSent,
                    a.WhatsAppSent,
                    a.CreatedDate
                });

                var result = data.ToDataSourceResult(request);
                return Json(result);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(ex);
                return Json(new DataSourceResult { Errors = new[] { ex.Message } });
            }
        }

        [HttpPost]
        public ActionResult GetStaffBySubject(int subjectId, int instanceId)
        {
            try
            {
                var staff = _staffService.Get(s => s.StaffSubjectDetails.Any(ssd => ssd.SubjectId == subjectId) &&
                                                   s.Status == StatusConstants.Active, s => new
                                                   {
                                                       s.Id,
                                                       Name = $"({s.Id}) {s.Name}",
                                                       s.Mobile,
                                                       s.Email
                                                   }).ToList();

                return Json(staff, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        [HttpPost]
        public ActionResult GetStructuresBySubject(int subjectId, int instanceId)
        {
            try
            {
                var structures = _structureService.Get(s => s.SubjectId == subjectId &&
                                                            s.Status == StatusConstants.Active,
                        s => new
                        {
                            s.Id,
                            Name = $"Structure - {s.MaxMarks} Marks - {s.NoOfSections} Sections"
                        }, s => s.OrderByDescending(x => x.CreatedDate))
                    .ToList();

                return Json(structures, JsonRequestBehavior.AllowGet);
            }
            catch (Exception ex)
            {
                return Json(new { error = ex.Message }, JsonRequestBehavior.AllowGet);
            }
        }

        private void LoadViewBagData()
        {
            var sessionData = Session[User.Identity.Name] as SessionData;

            ViewBag.Faculties = _facultyService.GetViewModelList().ToList();

            ViewBag.Courses = new List<SelectListItem>();
            ViewBag.Subjects = new List<SelectListItem>();
            ViewBag.Structures = new List<SelectListItem>();
        }
    }
}
