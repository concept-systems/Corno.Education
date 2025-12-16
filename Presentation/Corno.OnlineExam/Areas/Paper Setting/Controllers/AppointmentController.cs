using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Web.Mvc;
using Corno.Data.Corno;
using Corno.Data.ViewModels;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.OnlineExam.Areas.Paper_Setting.Reports.Appointment;
using Corno.OnlineExam.Attributes;
using Corno.OnlineExam.Controllers;
using Corno.Reports.Base;
using Corno.Reports.Enrollment;
using Corno.Services.Corno.Question_Bank.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using Corno.Services.Email.Interfaces;
using Telerik.Reporting.Processing;
using System.IO;
using System.Net;
using System.Net.Mail;
using Corno.Data.Corno.Paper_Setting.Dtos;
using Corno.Data.Corno.Paper_Setting.Models;
using Corno.Data.Helpers;
using Corno.Data.ViewModels.Appointment;
using Corno.Logger;
using Corno.Services.Bootstrapper;
using Corno.Services.SMS.Interfaces;
using Corno.Services.Corno.Admin.Interfaces;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Paper_Setting.Interfaces;
using Corno.Services.File.Interfaces;
using Exception = System.Exception;

namespace Corno.OnlineExam.Areas.Paper_Setting.Controllers;

[Authorize]
public class AppointmentController : CornoController
{
    #region -- Constructors --
    public AppointmentController(IAppointmentService appointmentService,
        IEmailService emailService, ISmsService smsService,
        IUserService userService, IPaperService paperService)
    {
        _appointmentService = appointmentService;
        _emailService = emailService;
        _smsService = smsService;
        _userService = userService;
        _paperService = paperService;

        var path = "~/Areas/paper setting/views/Appointment/";
        _indexPath = $"{path}Index.cshtml";
        _createPath = $"{path}Create.cshtml";
        _editPath = $"{path}Edit.cshtml";
        //_billPath = $"{path}Bill.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly IAppointmentService _appointmentService;
    private readonly IEmailService _emailService;
    private readonly ISmsService _smsService;
    private readonly IUserService _userService;
    private readonly IPaperService _paperService;

    private readonly string _indexPath;
    private readonly string _createPath;
    private readonly string _editPath;
    //private readonly string _billPath;
    #endregion

    #region -- Private Methods --

    private EmailSetting GetEmailSettings()
    {
        return new EmailSetting
        {
            From = ConfigurationManager.AppSettings[SettingConstants.From],
            Password = ConfigurationManager.AppSettings[SettingConstants.EmailPassword],
            SmtpServer = ConfigurationManager.AppSettings[SettingConstants.SmtpServer],
            SmtpPort = ConfigurationManager.AppSettings[SettingConstants.SmtpPort].ToInt(),
            EnableSsl = ConfigurationManager.AppSettings[SettingConstants.EnableSsl].ToBoolean(),
            Cc = ConfigurationManager.AppSettings[SettingConstants.Cc],
            Bcc = ConfigurationManager.AppSettings[SettingConstants.Bcc],
            /*Subject = ConfigurationManager.AppSettings[SettingConstants.ExamLinkEmailSubject],
            Body = ConfigurationManager.AppSettings[SettingConstants.EmailBody],*/
        };
    }

    private string AddUserDetail(ReportModel reportModel, string body)
    {
        if (false == reportModel.IsInternal)
        {
            body = body.Replace("@loginDetails", Environment.NewLine);
            return body;
        }

        var user = _userService.CreateUser(reportModel.MobileNo, new List<string> { RoleConstants.PaperSetter });

        var loginBody = "\n\n";
        loginBody += "Your login credentials are as follows:\n";
        loginBody += $"User Name: {user.UserName}\n";
        loginBody += $"Password: {user.Password}\n\n\n";

        body = body.Replace("@loginDetails", loginBody);

        return body;
    }
        
    private void SendEmail(Appointment appointment)
    {
        var data = _appointmentService.GetData(appointment, false)
            .ToList();

        if ((appointment.BranchId ?? 0) > 0)
            data = data.Where(d => d.Branch.Id == appointment.BranchId).ToList();

        if ((appointment.SubjectId ?? 0) > 0)
            data = data.Where(d => d.Subject.Id == appointment.SubjectId).ToList();

        if (data.Count <= 0)
            throw new Exception("No appointments to send mail");

        var appointments = new List<Appointment>();
        foreach (var record in data.Where(record => null != record))
        {
            if (null == record.MeetingDate)
            {
                LogHandler.LogInfo($"No meeting date for Subject : {record.Subject.Code} and Staff : {record.StaffId}");
                continue;
                //throw new Exception($"Meeting date is not assigned for subject {record.SubjectId} - {record.SubjectName} ");
            }

            var reportData = data.Where(d => d.Subject.Id == record.Subject.Id).ToList();
            var bCreateUser = false;
            try
            {
                // Create papers 
                var setsToBeDrawn = reportData.FirstOrDefault()?.SetsToBeDrawn ?? 0;
                if (setsToBeDrawn <= 0)
                    throw new Exception($"Invalid sets to be drawn : {setsToBeDrawn}");
                _paperService.Create(reportData.FirstOrDefault(), setsToBeDrawn);
                bCreateUser = true;
            }
            catch (Exception exception)
            {
                LogHandler.LogError(exception);
            }

            BaseReport report;
            string subject;
            string body;
            switch (appointment.CategoryId)
            {
                case 2:
                    if (record.IsModerator ?? false)
                        report = new ModeratorLetterRpt(reportData);
                    else
                        report = new TheoryLetterRpt(reportData);
                    /*if (!appointment.IsModerator)
                    {
                        report.ReportParameters[FieldConstants.Medical].Value = false;
                        report.ReportParameters[FieldConstants.OnlyInternal].Value = true;
                    }*/

                    subject = ConfigurationManager.AppSettings["TheoryEmailSubject"];
                    body = ConfigurationManager.AppSettings["TheoryEmailBody"];
                    break;
                case 8:
                    report = new PracticalLetterRpt(reportData);
                    /*report.ReportParameters[FieldConstants.Medical].Value = false;
                    report.ReportParameters[FieldConstants.OnlyInternal].Value = true;*/

                    subject = ConfigurationManager.AppSettings["PracticalEmailSubject"];
                    body = ConfigurationManager.AppSettings["PracticalEmailBody"];
                    break;
                case 54:
                    report = new ModeratorLetterRpt(reportData);
                    /*report.ReportParameters[FieldConstants.Medical].Value = false;*/

                    subject = ConfigurationManager.AppSettings["ModeratorEmailSubject"];
                    body = ConfigurationManager.AppSettings["ModeratorEmailBody"];
                    break;
                default:
                    continue;
            }

            /*// For cancel appointments
            if (chkCancelAppointment.IsChecked)
            {
                emailSubject = ConfigurationManager.AppSettings["CancelEmailSubject"];
                emailBody = ConfigurationManager.AppSettings["CancelEmailBody"];
            }*/

            body = body.Replace("\\n", "<br>");
            body = body.Replace("@courseName", record.Course.Name);
            body = body.Replace("@coursePartName", record.CoursePart.Name);
            body = body.Replace("@subjectName", record.Subject.Name);
            body = body.Replace("@meetingDate", record.MeetingDate?.ToString("dd/MM/yyyy"));
            body = body.Replace("@setsToBeDrawn", record.SetsToBeDrawn.ToString());
            body = body.Replace("@date", record.MeetingDate?.ToString("dd/MM/yyyy"));
            body = body.Replace("@time", record.MeetingDate?.ToString("hh:mm"));

            if (bCreateUser)
            {
                // Add User details
                body = AddUserDetail(record, body);
            }

            body = body.Replace("\n", "<br>");

            // Attachment
            var reportProcessor = new ReportProcessor();
            var result = reportProcessor.RenderReport("PDF", report, null);
            using var pdfStream = new MemoryStream(result.DocumentBytes);
            pdfStream.Seek(0, SeekOrigin.Begin); // Reset stream position to beginning
            var attachment = new Attachment(pdfStream, "Report.pdf", "application/pdf");

            var emailSettings = GetEmailSettings();
            emailSettings.StreamAttachments ??= new List<Attachment>();
            emailSettings.StreamAttachments.Add(attachment);

            //emailSettings.To = "umesh.kale@concsystems.in";
            //emailSettings.To = string.Join(";", group.Select(g => g.EmailId)); //record.EmailId;
            if (string.IsNullOrEmpty(record.EmailId))
                throw new Exception($"Email id is not assigned for staff : ({record.StaffId}) {record.StaffName}, Subject : {record.Subject.Id}" +
                                    $"");
            emailSettings.To = record.EmailId;
            emailSettings.Subject = subject;
            emailSettings.Body = body;
            emailSettings.EnableSsl = true;

            _emailService.SendEmail(emailSettings);

            var appointmentDetail = record.Appointment?.AppointmentDetails.FirstOrDefault(d => d.StaffId == record.StaffId);
            if (null == appointmentDetail) continue;
            appointmentDetail.EmailCount ??= 0;
            appointmentDetail.EmailCount += 1;
            appointmentDetail.EmailDate = DateTime.Now;

            if (!appointments.Contains(record.Appointment))
            {
                // Send mail to college 
                emailSettings.To = record.College.Email;
                _emailService.SendEmail(emailSettings);

                appointments.Add(record.Appointment);
            }
        }

        // Save appointment
        _appointmentService.UpdateRangeAndSave(appointments);
    }

    private void SendSms(Appointment appointment)
    {
        var data = _appointmentService.GetData(appointment, false).ToList();

        if ((appointment.BranchId ?? 0) > 0)
            data = data.Where(d => d.Branch.Id == appointment.BranchId).ToList();
        if ((appointment.SubjectId ?? 0) > 0)
            data = data.Where(d => d.Subject.Id == appointment.SubjectId).ToList();

        if (data.Count <= 0)
            throw new Exception("No appointments to send sms");

        //// For testing purpose
        //data.ForEach(p => 
        //{
        //    p.MeetingDate = DateTime.Now;
        //    p.MobileNo = "9373333210";
        //});

        var appointments = new List<Appointment>();
        foreach (var record in data)
        {
            if (null == record.MeetingDate)
                continue;
            //throw new Exception($"Meeting date is not assigned for subject {record.SubjectId} - {record.SubjectName} ");

            var smsUrl = ConfigurationManager.AppSettings[SettingConstants.SmsUrl];
            string message;
            switch (appointment.CategoryId)
            {
                case 2:
                    message = ConfigurationManager.AppSettings["TheorySmsMessage"];
                    break;
                case 8:
                    message = ConfigurationManager.AppSettings["PracticalSmsMessage"];
                    break;
                case 54:
                    message = ConfigurationManager.AppSettings["ModeratorSmsMessage"];
                    break;
                default:
                    continue;
            }

            /*// For cancel appointments
            if (chkCancelAppointment.IsChecked)
            {
                emailSubject = ConfigurationManager.AppSettings["CancelEmailSubject"];
                emailBody = ConfigurationManager.AppSettings["CancelEmailBody"];
            }*/

            var smsMessage = message.Replace("{#var#}", record.MeetingDate?.ToString("dd/MM/yyyy"));

            smsMessage = smsMessage.Replace("&", "and");

            if (string.IsNullOrEmpty(record.MobileNo)) continue;
            smsUrl = smsUrl.Replace("@mobileNo", record.MobileNo);
            smsUrl = smsUrl.Replace("@message", smsMessage);
            _smsService.SendSms(smsUrl);

            var appointmentDetail = record.Appointment?.AppointmentDetails.FirstOrDefault(d => d.StaffId == record.StaffId);
            if (null == appointmentDetail) continue;
            appointmentDetail.SmsCount ??= 0;
            appointmentDetail.SmsCount += 1;

            if (!appointments.Contains(record.Appointment))
                appointments.Add(record.Appointment);
        }

        // Save appointment
        _appointmentService.UpdateRangeAndSave(appointments);
    }
    #endregion

    #region -- Actions --
    [Authorize]
    public ActionResult Index()
    {
        return View(_indexPath);
    }

    [Authorize]
    public ActionResult Create()
    {
        var sessionData = Session[User.Identity.Name] as SessionData;
        var appointment = new Appointment
        {
            InstanceId = sessionData?.InstanceId ?? 0,
            CollegeId = sessionData?.CollegeId,
        };

        return View(_createPath, appointment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [MultipleButton(Name = "action", Argument = "Save")]
    public ActionResult Create(Appointment appointment)
    {
        if (!ModelState.IsValid)
            return View(_createPath, appointment);

        try
        {
            // Validate fields
            _appointmentService.ValidateFields(appointment);

            // Add or update Appointment
            _appointmentService.Save(appointment);

            TempData["Success"] = "Saved successfully.";
            appointment.AppointmentDetails.Clear();

            /*ModelState.Clear();
            return RedirectToAction("Create", new {  });*/
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, appointment);
    }

    public ActionResult Edit(int? id)
    {
        if (null == id)
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        var appointment = _appointmentService.FirstOrDefault(p => p.Id == id, p => p);
        // Get all Appointment subjects
        appointment = _appointmentService.GetStaffs(appointment);
        return View(_editPath, appointment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [ValidateInput(false)]
    [MultipleButton(Name = "action", Argument = "Edit")]
    public ActionResult Edit(Appointment model)
    {
        try
        {
            if (!ModelState.IsValid)
                return View(_editPath, model);

            _appointmentService.UpdateAndSave(model);

            return RedirectToAction("Index");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_editPath, model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "GetStaffs")]
    public ActionResult GetStaffs(Appointment appointment)
    {
        if (!ModelState.IsValid || null == appointment)
            return View(_createPath, appointment);

        try
        {
            // Get all Appointment subjects
            appointment = _appointmentService.GetStaffs(appointment);

            ModelState.Remove(ModelConstants.Id);

            /*ModelState.Clear();
            return View(_createPath, appointment);*/
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        //appointment.EnableHeader = true;
        return View(_createPath, appointment);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "ShowReport")]
    public ActionResult ShowReport(Appointment model, string type)
    {
        if (!ModelState.IsValid)
            return View(_createPath, model);

        try
        {
            // Validate header
            _appointmentService.ValidateReportHeader(model);

            switch (type)
            {
                case ReportName.TheoryLetter:
                    model.CategoryId = 2;
                    Session[ModelConstants.Report] = new TheoryLetterRpt(model);
                    break;
                case ReportName.PracticalLetter:
                    model.CategoryId = 8;
                    Session[ModelConstants.Report] = new PracticalLetterRpt(model);
                    break;
                case ReportName.ManuscriptLetter:
                    model.CategoryId = 53;
                    Session[ModelConstants.Report] = new ManuscriptLetterRpt(model);
                    break;
                case ReportName.ModeratorLetter:
                    model.CategoryId = 54;
                    Session[ModelConstants.Report] = new ModeratorLetterRpt(model);
                    break;
                case ReportName.AlternativeLetter:
                    Session[ModelConstants.Report] = new AlternateExaminerLetterRpt(model);
                    break;

                case ReportName.AppointedExaminers:
                    Session[ModelConstants.Report] = new AppointedExaminersRpt(model);
                    break;
                case ReportName.ManuscriptExaminers:
                    Session[ModelConstants.Report] = new ManuscriptExaminersRpt(model);
                    break;

                // Summary
                case ReportName.AppointmentEvaluation:
                    {
                        var excelFileService = Bootstrapper.Get<IExcelFileService<EvaluationDto>>();
                        var evaluationDtos = _appointmentService.GetEvaluationReport(model)
                            .ToList();
                        var stream = excelFileService.GetMemoryStream(evaluationDtos);
                        return File(stream.ToArray(),
                            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                            "Evaluation.xlsx");
                    }
            }

            return RedirectToAction("Details", "Report", new { area = "Reports", reportName = nameof(EnrollmentRpt), description = string.Empty });
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_createPath, model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "Send")]
    public ActionResult Send(Appointment model, string type)
    {
        if (!ModelState.IsValid)
            return View(_editPath, model);
        model.InstanceId = (Session[User.Identity.Name] as SessionData)?.InstanceId;
        try
        {
            // Validate header
            _appointmentService.ValidateEmailHeader(model);

            switch (type)
            {
                case "Email":
                    SendEmail(model);
                    break;
                case "SMS":
                    SendSms(model);
                    break;
            }

            TempData["Success"] = "Sent successfully.";
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_createPath, model);
    }

    public ActionResult GetIndexViewModels([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var collegeService = Bootstrapper.Get<ICollegeService>();
            var courseService = Bootstrapper.Get<ICourseService>();
            var coursePartService = Bootstrapper.Get<ICoursePartService>();
            var subjectService = Bootstrapper.Get<ISubjectService>();

            var colleges = collegeService.GetQuery();
            var courses = courseService.GetQuery();
            var courseParts = coursePartService.GetQuery();
            var subjects = subjectService.GetQuery();

            var sessionData = Session[User.Identity.Name] as SessionData;
            /*var query = _appointmentService.GetQuery()
                .Where(p => p.InstanceId == sessionData?.InstanceId)
                .Select(p => new AppointmentIndexViewModel
                {
                    Id = p.Id,
                    InstanceId = p.InstanceId,
                    CollegeId = p.CollegeId,
                    CourseId = p.CourseId,
                    CoursePartId = p.CoursePartId,
                    SubjectId = p.SubjectId,

                    CollegeName = $"({p.CollegeId}) {collegeService.GetViewModel(p.CollegeId)?.Name}",
                    CourseName = $"({p.CourseId}) {courseService.GetViewModel(p.CourseId)?.Name}",
                    CoursePartName = $"({p.CourseId}) {coursePartService.GetViewModel(p.CoursePartId)?.Name}",
                    SubjectName = $"({p.SubjectId}) {subjectService.GetViewModel(p.SubjectId)?.Name}"
                }).ToList();*/

            // Perform an inner join so that the supplier name is retrieved via the query.
            var query = from appointment in _appointmentService.GetQuery().Where(p => p.InstanceId == sessionData.InstanceId)
                        join college in colleges on appointment.CollegeId equals college.Id
                        join course in courses on appointment.CourseId equals course.Id
                        join coursePart in courseParts on appointment.CoursePartId equals coursePart.Id
                        join subject in subjects on appointment.SubjectId equals subject.Id
                        select new AppointmentIndexViewModel
                        {
                            Id = appointment.Id,
                            InstanceId = appointment.InstanceId,
                            CollegeId = appointment.CollegeId,
                            CourseId = appointment.CourseId,
                            CoursePartId = appointment.CoursePartId,
                            SubjectId = appointment.SubjectId,

                            CollegeName = college.Name,
                            CourseName = course.Name,
                            CoursePartName = coursePart.Name,
                            SubjectName = subject.Name
                        };

            var result = query.ToDataSourceResult(request);

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
        }

        return Json(null, JsonRequestBehavior.AllowGet);
    }

    /*public ActionResult GetIndexViewModels([DataSourceRequest] DataSourceRequest request)
    {
        try
        {
            var collegeService = Bootstrapper.Get<ICollegeService>();
            var courseService = Bootstrapper.Get<ICourseService>();
            var coursePartService = Bootstrapper.Get<ICoursePartService>();
            var subjectService = Bootstrapper.Get<ISubjectService>();

            var sessionData = Session[User.Identity.Name] as SessionData;
            var query = _appointmentService.GetQuery()
                .Where(p => p.InstanceId == sessionData?.InstanceId)
                //.Skip(request.Page * request.PageSize).Take(request.PageSize)
                .Select(p => new AppointmentIndexViewModel
                {
                    Id = p.Id,
                    InstanceId = p.InstanceId,
                    CollegeId = p.CollegeId,
                    CourseId = p.CourseId,
                    CoursePartId = p.CoursePartId,
                    SubjectId = p.SubjectId,

                    CollegeName = $"({p.CollegeId}) {collegeService.GetViewModel(p.CollegeId)?.Name}",
                    CourseName = $"({p.CourseId}) {courseService.GetViewModel(p.CourseId)?.Name}",
                    CoursePartName = $"({p.CourseId}) {coursePartService.GetViewModel(p.CoursePartId)?.Name}",
                    SubjectName = $"({p.SubjectId}) {subjectService.GetViewModel(p.SubjectId)?.Name}"
                }).ToList();
            var result = query.ToDataSourceResult(request);

            /*var collegeService = Bootstrapper.Get<ICollegeService>();
            var courseService = Bootstrapper.Get<ICourseService>();
            var coursePartService = Bootstrapper.Get<ICoursePartService>();
            var subjectService = Bootstrapper.Get<ISubjectService>();

            var resultData = result.Data as IEnumerable<AppointmentIndexViewModel>;
            resultData?.ForEach(p =>
            {
                p.CollegeName = $"({p.CollegeId}) {collegeService.GetViewModel(p.CollegeId)?.Name}";
                p.CourseName = $"({p.CourseId}) {courseService.GetViewModel(p.CourseId)?.Name}";
                p.CoursePartName = $"({p.CourseId}) {coursePartService.GetViewModel(p.CoursePartId)?.Name}";
                p.SubjectName = $"({p.SubjectId}) {subjectService.GetViewModel(p.SubjectId)?.Name}";
            });#1#

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
        }

        return Json(null, JsonRequestBehavior.AllowGet);
    }*/

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Inline_Create_Update_Destroy([DataSourceRequest] DataSourceRequest request, LinkDetail model)
    {
        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    }

    /*[HttpPost]*/
    public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
    {
        var fileContents = Convert.FromBase64String(base64);

        return File(fileContents, contentType, fileName);
    }
    #endregion
}