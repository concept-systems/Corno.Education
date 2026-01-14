using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Data.Common;

namespace Corno.Data.Corno.Question_Bank_V2.Models
{
    [Serializable]
    public class QB_Paper : BaseModel
    {
        #region -- Constructors --
        public QB_Paper()
        {
            PaperDetails = new List<QB_PaperDetail>();
        }
        #endregion

        #region -- Properties --

        [Required]
        [StringLength(50)]
        public string PaperCode { get; set; }

        [Required]
        public int? AppointmentId { get; set; }

        [Required]
        public int? StructureId { get; set; }

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
        [StringLength(20)]
        public string PaperType { get; set; } // Auto, Manual

        [Required]
        public int SetNumber { get; set; }

        [Required]
        public int MaxMarks { get; set; }

        public int? NoOfSections { get; set; }

        public int? TimeDuration { get; set; }

        [StringLength(100)]
        public string GeneratedBy { get; set; }

        public DateTime? GeneratedDate { get; set; }

        [Required]
        [StringLength(128)]
        public string ModeratorUserId { get; set; }

        public string ModeratorComments { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Draft"; // Draft, Generated, Drawn

        public DateTime? DrawnDate { get; set; }

        [StringLength(100)]
        public string DrawnBy { get; set; }

        public byte[] WordDocumentContent { get; set; }

        [StringLength(255)]
        public string WordDocumentFileName { get; set; }

        public DateTime? WordDocumentGeneratedDate { get; set; }

        public virtual List<QB_PaperDetail> PaperDetails { get; set; }

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
        public string ModeratorUserName { get; set; }

        #endregion

        #region -- Methods --

        public override void Copy(BaseModel other)
        {
            if (other is not QB_Paper model) return;

            base.Copy(other);

            PaperCode = model.PaperCode;
            AppointmentId = model.AppointmentId;
            StructureId = model.StructureId;
            PaperType = model.PaperType;
            SetNumber = model.SetNumber;
            MaxMarks = model.MaxMarks;
            Status = model.Status;
        }

        #endregion
    }
}
