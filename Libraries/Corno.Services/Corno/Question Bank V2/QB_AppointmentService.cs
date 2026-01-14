using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using Corno.Data.Admin;
using Corno.Data.Corno.Masters;
using Corno.Data.Corno.Question_Bank_V2.Models;
using Corno.Data.ViewModels;
using Corno.Globals.Constants;
using Corno.Logger;
using Corno.Services.Corno;
using Corno.Services.Corno.Admin.Interfaces;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Question_Bank_V2.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;
using Corno.Services.Email.Interfaces;
using Corno.Services.Login.Interfaces;
using Corno.Services.SMS.Interfaces;

namespace Corno.Services.Corno.Question_Bank_V2
{
    public class QB_AppointmentService : MainService<QB_Appointment>, IQB_AppointmentService
    {
        private readonly IStaffService _staffService;
        private readonly IUserService _userService;
        private readonly IAspNetRoleService _roleService;
        private readonly IEmailService _emailService;
        private readonly ISmsService _smsService;
        private readonly IStructureService _structureService;
        
        public QB_AppointmentService(
            IStaffService staffService,
            IUserService userService,
            IAspNetRoleService roleService,
            IEmailService emailService,
            ISmsService smsService,
            IStructureService structureService)
        {
            _staffService = staffService;
            _userService = userService;
            _roleService = roleService;
            _emailService = emailService;
            _smsService = smsService;
            _structureService = structureService;
            SetIncludes(nameof(QB_Appointment.AppointmentDetails));
        }
        
        public void CreateAppointment(QB_Appointment appointment, string userId)
        {
            // Validate
            ValidateAppointment(appointment);
            
            // Generate appointment code
            appointment.AppointmentCode = GenerateAppointmentCode(appointment.InstanceId ?? 0);
            appointment.Status = "Created";
            appointment.CreatedBy = userId;
            appointment.CreatedDate = DateTime.Now;
            
            AddAndSave(appointment);
        }
        
        public void AssignRoles(QB_Appointment appointment, List<int> setterUserIds, List<int> checkerUserIds, List<int> moderatorUserIds)
        {
            appointment.AppointmentDetails = new List<QB_AppointmentDetail>();
            
            // Get role IDs from AspNetRoles
            var setterRole = _roleService.AspNetRoleRepository.GetQuery()
                .FirstOrDefault(r => r.Name == "Question Setter");
            var checkerRole = _roleService.AspNetRoleRepository.GetQuery()
                .FirstOrDefault(r => r.Name == "Question Checker");
            var moderatorRole = _roleService.AspNetRoleRepository.GetQuery()
                .FirstOrDefault(r => r.Name == "Moderator");
            
            if (setterRole == null || checkerRole == null || moderatorRole == null)
                throw new Exception("Required roles (Question Setter, Question Checker, Moderator) not found in AspNetRoles.");
            
            // Assign Setters
            foreach (var staffId in setterUserIds)
            {
                var staff = _staffService.GetById(staffId);
                if (staff == null) continue;
                
                var user = _userService.GetQuery()
                    .FirstOrDefault(u => u.UserName == staff.Mobile);
                if (user == null) continue;
                
                var detail = new QB_AppointmentDetail
                {
                    AppointmentId = appointment.Id,
                    UserId = user.Id,
                    RoleId = setterRole.Id,
                    RoleName = "Question Setter",
                    TemporaryUsername = GenerateUsername(appointment.AppointmentCode, user.UserName, "SET"),
                    TemporaryPassword = GeneratePassword(),
                    OtpEnabled = false
                };
                
                appointment.AppointmentDetails.Add(detail);
            }
            
            // Assign Checkers
            foreach (var staffId in checkerUserIds)
            {
                var staff = _staffService.GetById(staffId);
                if (staff == null) continue;
                
                var user = _userService.GetQuery()
                    .FirstOrDefault(u => u.UserName == staff.Mobile);
                if (user == null) continue;
                
                var detail = new QB_AppointmentDetail
                {
                    AppointmentId = appointment.Id,
                    UserId = user.Id,
                    RoleId = checkerRole.Id,
                    RoleName = "Question Checker",
                    TemporaryUsername = GenerateUsername(appointment.AppointmentCode, user.UserName, "CHK"),
                    TemporaryPassword = GeneratePassword(),
                    OtpEnabled = false
                };
                
                appointment.AppointmentDetails.Add(detail);
            }
            
            // Assign Moderators
            foreach (var staffId in moderatorUserIds)
            {
                var staff = _staffService.GetById(staffId);
                if (staff == null) continue;
                
                var user = _userService.GetQuery()
                    .FirstOrDefault(u => u.UserName == staff.Mobile);
                if (user == null) continue;
                
                var detail = new QB_AppointmentDetail
                {
                    AppointmentId = appointment.Id,
                    UserId = user.Id,
                    RoleId = moderatorRole.Id,
                    RoleName = "Moderator",
                    TemporaryUsername = GenerateUsername(appointment.AppointmentCode, user.UserName, "MOD"),
                    TemporaryPassword = GeneratePassword(),
                    OtpEnabled = false
                };
                
                appointment.AppointmentDetails.Add(detail);
            }
            
            UpdateAndSave(appointment);
        }
        
        public void GenerateLoginCredentials(QB_Appointment appointment)
        {
            foreach (var detail in appointment.AppointmentDetails)
            {
                if (string.IsNullOrEmpty(detail.TemporaryUsername))
                {
                    var user = _userService.GetById(detail.UserId);
                    detail.TemporaryUsername = GenerateUsername(appointment.AppointmentCode, user?.UserName ?? "USER", 
                        detail.RoleName.Substring(0, 3).ToUpper());
                }
                
                if (string.IsNullOrEmpty(detail.TemporaryPassword))
                {
                    detail.TemporaryPassword = GeneratePassword();
                }
            }
            
            UpdateAndSave(appointment);
        }
        
        public void SendNotifications(QB_Appointment appointment, string notificationType)
        {
            var appointmentWithDetails = GetById(appointment.Id);
            if (appointmentWithDetails == null)
                throw new Exception("Appointment not found.");
            
            foreach (var detail in appointmentWithDetails.AppointmentDetails)
            {
                var user = _userService.GetById(detail.UserId);
                if (user == null) continue;
                
                var staff = _staffService.Get(s => s.Mobile == user.UserName, s => s).FirstOrDefault();
                
                if (staff == null) continue;
                
                switch (notificationType.ToUpper())
                {
                    case "EMAIL":
                        SendEmailNotification(appointment, detail, staff, user);
                        break;
                    case "SMS":
                        SendSmsNotification(appointment, detail, staff);
                        break;
                    case "WHATSAPP":
                        SendWhatsAppNotification(appointment, detail, staff);
                        break;
                }
            }
        }
        
        public List<QB_Appointment> GetAppointmentsForUser(string userId, int instanceId, string roleName)
        {
            return GetQuery()
                .Where(a => a.InstanceId == instanceId &&
                           a.AppointmentDetails.Any(ad => ad.UserId == userId && ad.RoleName == roleName) &&
                           a.Status != StatusConstants.Deleted)
                .OrderByDescending(a => a.CreatedDate)
                .ToList();
        }
        
        public string GenerateAppointmentCode(int instanceId)
        {
            var year = DateTime.Now.Year;
            var count = GetQuery().Count(a => a.InstanceId == instanceId && 
                                             a.AppointmentCode.StartsWith($"APT-{year}"));
            
            return $"APT-{year}-{(count + 1):D5}";
        }
        
        public void AcceptAppointment(int appointmentDetailId, string userId)
        {
            var appointment = GetQuery()
                .FirstOrDefault(a => a.AppointmentDetails.Any(ad => ad.Id == appointmentDetailId));
            
            if (appointment == null)
                throw new Exception("Appointment not found.");
            
            var detail = appointment.AppointmentDetails.FirstOrDefault(ad => ad.Id == appointmentDetailId);
            if (detail == null)
                throw new Exception("Appointment detail not found.");
            
            if (detail.UserId != userId)
                throw new UnauthorizedAccessException("You can only accept your own appointments.");
            
            detail.IsAccepted = true;
            detail.AcceptedDate = DateTime.Now;
            
            UpdateAndSave(appointment);
        }
        
        private void ValidateAppointment(QB_Appointment appointment)
        {
            if (appointment.InstanceId <= 0)
                throw new Exception("Instance is required.");
            if (appointment.SubjectId <= 0)
                throw new Exception("Subject is required.");
            if (appointment.StructureId <= 0)
                throw new Exception("Structure is required.");
            if (appointment.NoOfPapers <= 0)
                throw new Exception("Number of papers must be greater than zero.");
            if (appointment.AppointmentDate == null)
                throw new Exception("Appointment date is required.");
        }
        
        private string GenerateUsername(string appointmentCode, string baseUsername, string rolePrefix)
        {
            return $"{appointmentCode}-{rolePrefix}-{baseUsername}";
        }
        
        private string GeneratePassword()
        {
            const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789!@#$%^&*";
            var random = new Random();
            return new string(Enumerable.Repeat(chars, 12)
                .Select(s => s[random.Next(s.Length)]).ToArray());
        }
        
        private void SendEmailNotification(QB_Appointment appointment, QB_AppointmentDetail detail, 
            Staff staff, AspNetUser user)
        {
            try
            {
                var subject = $"Question Bank Appointment - {appointment.AppointmentCode}";
                var body = BuildEmailBody(appointment, detail, staff);
                
                var emailSettings = new EmailSetting
                {
                    To = staff.Email ?? user.Email,
                    Subject = subject,
                    Body = body,
                    EnableSsl = true
                };
                
                _emailService.SendEmail(emailSettings);
                
                detail.EmailSent = true;
                detail.EmailSentDate = DateTime.Now;
                detail.EmailSentCount++;
                
                UpdateAndSave(appointment);
            }
            catch (Exception ex)
            {
                LogHandler.LogError(ex);
                throw new Exception($"Failed to send email notification: {ex.Message}");
            }
        }
        
        private void SendSmsNotification(QB_Appointment appointment, QB_AppointmentDetail detail, 
            Staff staff)
        {
            try
            {
                var message = BuildSmsMessage(appointment, detail);
                var smsUrl = System.Configuration.ConfigurationManager.AppSettings["SmsUrl"] ?? "";
                
                if (!string.IsNullOrEmpty(smsUrl) && !string.IsNullOrEmpty(staff.Mobile))
                {
                    smsUrl = smsUrl.Replace("@mobileNo", staff.Mobile);
                    smsUrl = smsUrl.Replace("@message", message);
                    
                    _smsService.SendSms(smsUrl);
                    
                    detail.SmsSent = true;
                    detail.SmsSentDate = DateTime.Now;
                    detail.SmsSentCount++;
                    
                    UpdateAndSave(appointment);
                }
            }
            catch (Exception ex)
            {
                LogHandler.LogError(ex);
                throw new Exception($"Failed to send SMS notification: {ex.Message}");
            }
        }
        
        private void SendWhatsAppNotification(QB_Appointment appointment, QB_AppointmentDetail detail, 
            Staff staff)
        {
            // WhatsApp integration - implement based on your WhatsApp API
            // For now, just log
            LogHandler.LogInfo($"WhatsApp notification would be sent to {staff.Mobile}");
            
            detail.WhatsAppSent = true;
            detail.WhatsAppSentDate = DateTime.Now;
            detail.WhatsAppSentCount++;
            
            UpdateAndSave(appointment);
        }
        
        private string BuildEmailBody(QB_Appointment appointment, QB_AppointmentDetail detail, 
            Staff staff)
        {
            var sb = new StringBuilder();
            sb.AppendLine($"Dear {staff.Name},");
            sb.AppendLine();
            sb.AppendLine($"You have been assigned as {detail.RoleName} for the following appointment:");
            sb.AppendLine();
            sb.AppendLine($"Appointment Code: {appointment.AppointmentCode}");
            sb.AppendLine($"Subject: [Subject Name]");
            sb.AppendLine($"Number of Papers: {appointment.NoOfPapers}");
            sb.AppendLine($"Deadline: {appointment.AppointmentDate:dd/MM/yyyy} {appointment.AppointmentTime}");
            sb.AppendLine();
            sb.AppendLine("Login Credentials:");
            sb.AppendLine($"Username: {detail.TemporaryUsername}");
            sb.AppendLine($"Password: {detail.TemporaryPassword}");
            sb.AppendLine();
            sb.AppendLine("Please login to the Question Bank V2 portal to view your assignments.");
            sb.AppendLine();
            sb.AppendLine("Thank you.");
            
            return sb.ToString();
        }
        
        private string BuildSmsMessage(QB_Appointment appointment, QB_AppointmentDetail detail)
        {
            return $"Question Bank Appointment {appointment.AppointmentCode}. " +
                   $"Deadline: {appointment.AppointmentDate:dd/MM/yyyy}. " +
                   $"Login: {detail.TemporaryUsername} / {detail.TemporaryPassword}";
        }
    }
}
