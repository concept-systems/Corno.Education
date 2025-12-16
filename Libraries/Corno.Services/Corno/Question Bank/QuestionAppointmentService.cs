using Corno.Data.Corno.Paper_Setting.Models;
using Corno.Data.Corno.Question_Bank.Dtos;
using Corno.Data.Corno.Question_Bank.Models;
using Corno.Data.Helpers;
using Corno.Data.ViewModels;
using Corno.Globals.Constants;
using Corno.Logger;
using Corno.Services.Corno.Admin.Interfaces;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;
using Corno.Services.Email.Interfaces;
using LinqKit;
using Mapster;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web.Helpers;

namespace Corno.Services.Corno.Question_Bank;

// Service for managing Question Appointment logic, including validation, staff retrieval, and notifications.
public class QuestionAppointmentService : MainService<QuestionAppointment>, IQuestionAppointmentService
{
    #region -- Constructors --
    // Constructor initializes dependencies and sets up entity includes for eager loading.
    public QuestionAppointmentService(IStaffService staffService, IInstanceService instanceService,  ICourseService courseService,
        ICoursePartService coursePartService, ISubjectService subjectService, IUserService userService,
        IEmailService emailService)
    {
        _staffService = staffService;
        _instanceService = instanceService;
        _courseService = courseService;
        _coursePartService = coursePartService;
        _subjectService = subjectService;
        _userService = userService;
        _emailService = emailService;

        SetIncludes($"{nameof(QuestionAppointment.QuestionAppointmentDetails)},{nameof(QuestionAppointment.QuestionAppointmentTypeDetails)}");
    }
    #endregion

    #region -- Data Members --
    // Service dependencies for staff, course, subject, user, and email operations.
    private readonly IStaffService _staffService;
    private readonly IInstanceService _instanceService;
    private readonly ICourseService _courseService;
    private readonly ICoursePartService _coursePartService;
    private readonly ISubjectService _subjectService;
    private readonly IUserService _userService;
    private readonly IEmailService _emailService;
    #endregion

    #region -- Private Methods --

    // Validates the main fields of the appointment header.
    public void ValidateHeader(QuestionAppointmentDto dto)
    {
        // Validate common fields
        ValidateCommonHeader(dto);

        if (dto.SubjectId.ToInt() <= 0)
            throw new Exception("Invalid Subject.");
    }

    // Validates common header fields for appointment.
    private void ValidateCommonHeader(QuestionAppointmentDto dto)
    {
        if (dto.CollegeId.ToInt() <= 0)
            throw new Exception("Invalid College.");
        if (dto.CourseId.ToInt() <= 0)
            throw new Exception("Invalid Course.");
        if (dto.CoursePartId.ToInt() <= 0)
            throw new Exception("Invalid Course Part.");
        if (dto.CategoryId.ToInt() <= 0)
            throw new Exception("Invalid Subject Category.");
    }

    // Adds user login details to the email body by creating a user and inserting credentials.
    private string AddUserDetail(string emailBody, string mobileNo, List<string> roles)
    {
        var user = _userService.CreateUser(mobileNo, roles);

        var loginBody = "\n\n";
        loginBody += "Your login credentials are as follows:\n";
        loginBody += $"User Name: {user.UserName}\n";
        loginBody += $"Password: {user.Password}\n\n\n";

        emailBody = emailBody.Replace("@loginDetails", loginBody);

        return emailBody;
    }

    // Retrieves email settings from configuration.
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

    private Expression<Func<QuestionAppointment, bool>> GetAppointmentPredicate(QuestionAppointmentDto dto)
    {
        Expression<Func<QuestionAppointment, bool>> predicate = p => p.InstanceId == dto.InstanceId;
        if ((dto.FacultyId ?? 0) > 0)
            predicate = predicate.And(s => s.FacultyId == dto.FacultyId);
        if ((dto.CollegeId ?? 0) > 0)
            predicate = predicate.And(s => s.CollegeId == dto.CollegeId);
        if ((dto.CourseId ?? 0) > 0)
            predicate = predicate.And(s => s.CourseId == dto.CourseId);
        if ((dto.CoursePartId ?? 0) > 0)
            predicate = predicate.And(s => s.CoursePartId == dto.CoursePartId);
        if ((dto.BranchId ?? 0) > 0)
            predicate = predicate.And(s => s.BranchId == dto.BranchId);

        if ((dto.SubjectId ?? 0) > 0)
            predicate = predicate.And(s => s.SubjectId == dto.SubjectId);
        if ((dto.CategoryId ?? 0) > 0)
            predicate = predicate.And(s => s.CategoryId == dto.CategoryId);

        return predicate;
    }

    // Builds a query to get all appointments matching the given criteria for email/SMS notification.
    private IQueryable<EmailSmsDto> GetAppointments(QuestionAppointmentDto dto)
    {
        var appointmentPredicate = GetAppointmentPredicate(dto);

        // Query appointments and join with staff, course, course part, and subject for details.
        var query = GetQuery().Where(appointmentPredicate).Include(nameof(QuestionAppointment.QuestionAppointmentDetails)) as IEnumerable<QuestionAppointment>;
        var records = from appointment in query
                      from detail in appointment.QuestionAppointmentDetails.Where(d => d.IsSetter || d.IsChecker)
                      join staff in _staffService.GetQuery() on detail.StaffId equals staff.Id
                      join course in _courseService.GetQuery() on appointment.CourseId equals course.Id into courseJoin
                      from course in courseJoin.DefaultIfEmpty()
                      join coursePart in _coursePartService.GetQuery() on appointment.CoursePartId equals coursePart.Id into coursePartJoin
                      from coursePart in coursePartJoin.DefaultIfEmpty()
                      join subject in _subjectService.GetQuery() on appointment.SubjectId equals subject.Id into subjectJoin
                      from subject in subjectJoin.DefaultIfEmpty()
                      join instance in _instanceService.GetQuery() on appointment.InstanceId equals instance.Id into instanceJoin
                      from instance in instanceJoin.DefaultIfEmpty()
                      select new EmailSmsDto
                      {
                          InstanceName = instance.Name,
                          CourseName = course.Name,
                          CoursePartName = coursePart.Name,
                          SubjectId = appointment.SubjectId,
                          SubjectName = subject.Name,
                          StaffId = detail.StaffId,
                          StaffName = staff.Name,
                          MobileNo = staff.Mobile,
                          EmailId = staff.Email,
                          IsSetter = detail.IsSetter,
                          IsChecker = detail.IsChecker,
                          SetsToBeDrawn = appointment.SetsToBeDrawn
                      };

        return records.AsQueryable();
    }

    // Sends email notifications to all staff involved in the appointment (setter/checker).
    private void SendEmail(QuestionAppointmentDto dto)
    {
        // Get appointments for notification.
        var emailSmsDtos = GetAppointments(dto).ToList();

        var emailSubject = ConfigurationManager.AppSettings["QuestionEmailSubject"];

        var meetingDate = DateTime.Now;
        var emailSettings = GetEmailSettings();
        foreach (var record in emailSmsDtos)
        {
            LogHandler.LogInfo($"Appointment Info : {JsonConvert.SerializeObject(record)}");
            var emailBody = ConfigurationManager.AppSettings["QuestionEmailBody"];

            // Skip if not setter or checker.
            if (record.IsSetter == false && record.IsChecker == false)
                continue;
            if (record.SetsToBeDrawn.ToInt() <= 0)
                throw new Exception($"Invalid sets to be drawn for subject : {record.SubjectId}");

            // Replace placeholders in email body with actual values, placing IDs at the start.
            emailBody = emailBody.Replace("@instanceName", $"({dto.InstanceId.ToInt()}) {record.InstanceName}");
            emailBody = emailBody.Replace("@courseName", $"({dto.CourseId.ToInt()}) {record.CourseName}");
            emailBody = emailBody.Replace("@coursePartName", $"({dto.CoursePartId.ToInt()}) {record.CoursePartName}");
            emailBody = emailBody.Replace("@subjectName", $"({record.SubjectId.ToInt()}) {record.SubjectName}");
            emailBody = emailBody.Replace("@meetingDate", meetingDate.ToString("dd/MM/yyyy"));
            emailBody = emailBody.Replace("@setsToBeDrawn", record.SetsToBeDrawn.ToString());

            // Add user details and staff type to email.
            var staffType = string.Empty;
            var roles = new List<string>();
            if (record.IsSetter == true)
            {
                staffType = "Question Setter";
                roles = [RoleConstants.QuestionSetter];
            }
            else if (record.IsChecker == true)
            {
                staffType = "Question Checker";
                roles = new List<string> { RoleConstants.QuestionChecker };
                emailBody = emailBody.Replace("It is requested to enter 5 questions for each unit and each question type.", "");
            }
            emailBody = emailBody.Replace("@staffType", staffType);
            emailBody = AddUserDetail(emailBody, record.MobileNo, roles);

            // Ensure email is present before sending.
            if (string.IsNullOrEmpty(record.EmailId))
                throw new Exception(
                    $"Email id is not assigned for staff : ({record.StaffId}) {record.StaffName}, Subject : {record.SubjectId}");

            emailBody = emailBody.Replace("\n", "<br>");
            emailBody = emailBody.Replace("\\n", "<br>");

            // Set email settings and send email.
            emailSettings.To = record.EmailId;
            emailSettings.Subject = emailSubject;
            emailSettings.Body = emailBody;
            emailSettings.EnableSsl = true;

            _emailService.SendEmail(emailSettings);
        }
    }
    #endregion

    #region -- Public Methods --

    // Validates the appointment DTO for required fields and details.
    public void ValidateDto(QuestionAppointmentDto dto)
    {
        // Validate Header
        ValidateHeader(dto);

        // Validate only during save.
        if (dto.SetsToBeDrawn.ToInt() <= 0)
            throw new Exception("Invalid sets to be drawn.");

        if (dto.QuestionAppointmentDetailDtos.Count <= 0)
            throw new Exception("No rows in appointment.");
        if (dto.QuestionAppointmentDetailDtos.All(d => null == d.StaffId))
            throw new Exception("All rows are blank.");
    }

    // Gets an existing appointment matching the DTO's criteria.
    public QuestionAppointment GetExisting(QuestionAppointmentDto dto)
    {
        var existing = FirstOrDefault(p =>
            (p.InstanceId ?? 0) == (dto.InstanceId ?? 0) &&
            (p.FacultyId ?? 0) == (dto.FacultyId ?? 0) &&
            (p.CollegeId ?? 0) == (dto.CollegeId ?? 0) &&
            (p.CourseId ?? 0) == (dto.CourseId ?? 0) &&
            (p.CoursePartId ?? 0) == (dto.CoursePartId ?? 0) &&
            (p.BranchId ?? 0) == (dto.BranchId ?? 0) &&
            (p.CategoryId ?? 0) == (dto.CategoryId ?? 0) &&
            (p.SubjectId ?? 0) == (dto.SubjectId ?? 0), a => a);

        return existing;
    }

    // Populates the appointment DTO with staff details for the given subject and college.
    public void GetStaffs(QuestionAppointmentDto dto)
    {
        // Validate Header
        ValidateHeader(dto);
        dto.QuestionAppointmentDetailDtos.Clear();

        var appointment = GetExisting(dto);
        appointment?.Adapt(dto);

        // Get staffs for the subject and college.
        var staffs = _staffService.Get(p => p.StaffSubjectDetails.Any(m =>
                m.CollegeId == dto.CollegeId && m.SubjectId == dto.SubjectId &&
                p.Status != StatusConstants.Closed), p => p).ToList();

        foreach (var staff in staffs)
        {
            var staffSubjectDetail = staff.StaffSubjectDetails.FirstOrDefault(d => d.CollegeId == dto.CollegeId &&
                d.SubjectId == dto.SubjectId);
            if (null == staffSubjectDetail)
                continue;

            var appointmentDetailDto = dto.QuestionAppointmentDetailDtos.FirstOrDefault(a => a.StaffId == staff.Id);
            if (null != appointmentDetailDto)
            {
                appointmentDetailDto.StaffName = $"{staff.Salutation} {staff.Name}";
                appointmentDetailDto.EmailId = staff.Email;
                appointmentDetailDto.MobileNo = staff.Mobile;

                continue;
            }

            dto.QuestionAppointmentDetailDtos.Add(new QuestionAppointmentDetailDto()
            {
                StaffId = staff.Id,
                StaffName = $"{staff.Salutation} {staff.Name}",
                EmailId = staff.Email,
                MobileNo = staff.Mobile,
            });
        }

        // Remove any staff not in the current list.
        var staffIds = staffs.Select(p => p.Id).ToList();
        dto.QuestionAppointmentDetailDtos.RemoveAll(d => !staffIds.Contains(d.StaffId));
    }

    public void UpdateCompletedCount(Question question, int instanceId)
    {
        var appointment = FirstOrDefault(p => p.InstanceId == instanceId &&
                                              p.SubjectId == question.SubjectId && p.QuestionAppointmentDetails.Any(d =>
                                                  d.StaffId == question.StaffId), p => p);
        if (null == appointment)
            throw new Exception("Appointment not found for the question.");

        var appointmentTypeDetail = appointment.QuestionAppointmentTypeDetails.FirstOrDefault(d =>
                d.QuestionTypeId == question.QuestionTypeId);
        if (null == appointmentTypeDetail)
            throw new Exception("Appointment type detail not found for the question.");
        if (appointmentTypeDetail.CompletedCount >= appointmentTypeDetail.QuestionCount)
            throw new Exception($"Completed Count {appointmentTypeDetail.CompletedCount} should not exceed question count {appointmentTypeDetail.QuestionCount}");

        appointmentTypeDetail.CompletedCount ??= 0;
        appointmentTypeDetail.CompletedCount += 1;

        UpdateAndSave(appointment);
    }

    // Sends email or SMS notifications based on the type parameter.
    public void SendEmailSms(QuestionAppointmentDto dto, string type)
    {
        // Validate header
        ValidateCommonHeader(dto);

        switch (type)
        {
            case "Email":
                SendEmail(dto);
                break;
            case "SMS":
                //SendSms(model);
                break;
        }
    }
    #endregion
}