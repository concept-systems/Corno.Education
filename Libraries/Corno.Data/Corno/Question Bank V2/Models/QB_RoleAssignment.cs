using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Data.Common;

namespace Corno.Data.Corno.Question_Bank_V2.Models
{
    [Serializable]
    public class QB_RoleAssignment : BaseModel
    {
        #region -- Properties --

        [Required]
        public int? InstanceId { get; set; }

        [Required]
        public int? SubjectId { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        [Required]
        [StringLength(128)]
        public string RoleId { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; }

        public bool IsActive { get; set; } = true;

        public DateTime? AssignedDate { get; set; }

        [StringLength(100)]
        public string AssignedBy { get; set; }

        // NotMapped properties
        [NotMapped]
        public string InstanceName { get; set; }

        [NotMapped]
        public string SubjectName { get; set; }

        [NotMapped]
        public string UserName { get; set; }

        [NotMapped]
        public string RoleDisplayName { get; set; }

        #endregion

        #region -- Methods --

        public override void Copy(BaseModel other)
        {
            if (other is not QB_RoleAssignment model) return;

            base.Copy(other);

            InstanceId = model.InstanceId;
            SubjectId = model.SubjectId;
            UserId = model.UserId;
            RoleId = model.RoleId;
            RoleName = model.RoleName;
            IsActive = model.IsActive;
        }

        #endregion
    }
}
