using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Data.Common;

namespace Corno.Data.Corno.Question_Bank_V2.Models
{
    [Serializable]
    public class QB_QuestionChangeLog : BaseModel
    {
        #region -- Properties --

        [Required]
        public int? QuestionBankId { get; set; }

        [Required]
        [StringLength(50)]
        public string FieldName { get; set; }

        public byte[] OldValue { get; set; }

        public byte[] NewValue { get; set; }

        [StringLength(100)]
        public string ChangedBy { get; set; }

        public DateTime? ChangeDate { get; set; }

        [Required]
        [StringLength(20)]
        public string ChangeType { get; set; }

        public bool IsDirectDbChange { get; set; }

        #endregion

        #region -- Methods --

        public override void Copy(BaseModel other)
        {
            if (other is not QB_QuestionChangeLog model) return;

            base.Copy(other);

            QuestionBankId = model.QuestionBankId;
            FieldName = model.FieldName;
            OldValue = model.OldValue;
            NewValue = model.NewValue;
            ChangedBy = model.ChangedBy;
            ChangeDate = model.ChangeDate;
            ChangeType = model.ChangeType;
            IsDirectDbChange = model.IsDirectDbChange;
        }

        #endregion
    }
}
