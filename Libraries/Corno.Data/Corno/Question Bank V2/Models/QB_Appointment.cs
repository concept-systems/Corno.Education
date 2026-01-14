using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Data.Common;

namespace Corno.Data.Corno.Question_Bank_V2.Models
{
    [Serializable]
    public class QB_Appointment : BaseModel
    {
        #region -- Constructors --
        public QB_Appointment()
        {
            AppointmentDetails = new List<QB_AppointmentDetail>();
        }
        #endregion

        #region -- Properties --

        [Required]
        [StringLength(50)]
        public string AppointmentCode { get; set; }

        [Required]
        public int? InstanceId { get; set; }

        [Required]
        public int? FacultyId { get; set; }

        [Required]
        public int? CourseId { get; set; }

        [Required]
        public int? CoursePartId { get; set; }

        public int? BranchId { get; set; }

        [Required]
        public int? SubjectId { get; set; }

        [Required]
        public int? PaperCategoryId { get; set; }

        [Required]
        public int? StructureId { get; set; }

        [Required]
        public int NoOfPapers { get; set; } = 1;

        [Required]
        public DateTime? AppointmentDate { get; set; }

        public TimeSpan? AppointmentTime { get; set; }

        public string Instructions { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Created";

        public bool EmailSent { get; set; }

        public bool SmsSent { get; set; }

        public bool WhatsAppSent { get; set; }

        public DateTime? EmailSentDate { get; set; }

        public DateTime? SmsSentDate { get; set; }

        public DateTime? WhatsAppSentDate { get; set; }

        public virtual List<QB_AppointmentDetail> AppointmentDetails { get; set; }

        // NotMapped properties
        [NotMapped]
        public string InstanceName { get; set; }

        [NotMapped]
        public string FacultyName { get; set; }

        [NotMapped]
        public string CourseName { get; set; }

        [NotMapped]
        public string SubjectName { get; set; }

        [NotMapped]
        public string StructureName { get; set; }

        #endregion

        #region -- Methods --

        public override void Copy(BaseModel other)
        {
            if (other is not QB_Appointment model) return;

            base.Copy(other);

            AppointmentCode = model.AppointmentCode;
            InstanceId = model.InstanceId;
            FacultyId = model.FacultyId;
            CourseId = model.CourseId;
            CoursePartId = model.CoursePartId;
            BranchId = model.BranchId;
            SubjectId = model.SubjectId;
            PaperCategoryId = model.PaperCategoryId;
            StructureId = model.StructureId;
            NoOfPapers = model.NoOfPapers;
            AppointmentDate = model.AppointmentDate;
            AppointmentTime = model.AppointmentTime;
            Instructions = model.Instructions;
            Status = model.Status;
        }

        #endregion
    }
}
