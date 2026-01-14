using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Data.Common;

namespace Corno.Data.Corno.Question_Bank_V2.Models
{
    [Serializable]
    public class QB_QuestionWorkflow : BaseModel
    {
        #region -- Properties --

        [Required]
        public int? QuestionBankId { get; set; }

        [StringLength(20)]
        public string FromStatus { get; set; }

        [Required]
        [StringLength(20)]
        public string ToStatus { get; set; }

        [Required]
        [StringLength(50)]
        public string RoleName { get; set; }

        [Required]
        [StringLength(128)]
        public string UserId { get; set; }

        public string Comments { get; set; }

        public DateTime? ActionDate { get; set; }

        // NotMapped properties
        [NotMapped]
        public string UserName { get; set; }

        #endregion

        #region -- Methods --

        public override void Copy(BaseModel other)
        {
            if (other is not QB_QuestionWorkflow model) return;

            base.Copy(other);

            QuestionBankId = model.QuestionBankId;
            FromStatus = model.FromStatus;
            ToStatus = model.ToStatus;
            RoleName = model.RoleName;
            UserId = model.UserId;
            Comments = model.Comments;
            ActionDate = model.ActionDate;
        }

        #endregion
    }
}
