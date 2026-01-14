using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Data.Common;

namespace Corno.Data.Corno.Question_Bank_V2.Models
{
    [Serializable]
    public class QB_AppointmentDetail : BaseModel
    {
        #region -- Properties --

        [Required]
        public int? AppointmentId { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        [Required]
        [StringLength(128)]
        public string RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; }

        [Required]
        [StringLength(100)]
        public string TemporaryUsername { get; set; }

        [Required]
        [StringLength(255)]
        public string TemporaryPassword { get; set; }

        [StringLength(255)]
        public string PasswordSalt { get; set; }

        public bool OtpEnabled { get; set; }

        [StringLength(100)]
        public string OtpSecret { get; set; }

        public bool EmailSent { get; set; }

        public bool SmsSent { get; set; }

        public bool WhatsAppSent { get; set; }

        public DateTime? EmailSentDate { get; set; }

        public DateTime? SmsSentDate { get; set; }

        public DateTime? WhatsAppSentDate { get; set; }

        public int EmailSentCount { get; set; }

        public int SmsSentCount { get; set; }

        public int WhatsAppSentCount { get; set; }

        public bool IsAccepted { get; set; }

        public DateTime? AcceptedDate { get; set; }

        public int LoginCount { get; set; }

        public DateTime? LastLoginDate { get; set; }

        public int QuestionsAssigned { get; set; }

        public int QuestionsCompleted { get; set; }

        public int QuestionsApproved { get; set; }

        public int QuestionsRejected { get; set; }

        // NotMapped properties
        [NotMapped]
        public string UserName { get; set; }

        [NotMapped]
        public string Email { get; set; }

        [NotMapped]
        public string MobileNo { get; set; }

        #endregion

        #region -- Methods --

        public override void Copy(BaseModel other)
        {
            if (other is not QB_AppointmentDetail model) return;

            base.Copy(other);

            AppointmentId = model.AppointmentId;
            UserId = model.UserId;
            RoleId = model.RoleId;
            RoleName = model.RoleName;
            TemporaryUsername = model.TemporaryUsername;
            TemporaryPassword = model.TemporaryPassword;
            OtpEnabled = model.OtpEnabled;
        }

        #endregion
    }
}
