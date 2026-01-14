using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Data.Common;

namespace Corno.Data.Corno.Question_Bank_V2.Models
{
    [Serializable]
    public class QB_QuestionOptions : BaseModel
    {
        #region -- Properties --

        [Required]
        public int? QuestionBankId { get; set; }

        [Required]
        public string OptionText { get; set; }

        [Required]
        public int OptionOrder { get; set; }

        public bool IsCorrect { get; set; }

        public bool IsPartialCorrect { get; set; }

        public decimal? Marks { get; set; }

        public string Explanation { get; set; }

        public bool HasImage { get; set; }

        [StringLength(500)]
        public string ImagePath { get; set; }

        #endregion

        #region -- Methods --

        public override void Copy(BaseModel other)
        {
            if (other is not QB_QuestionOptions model) return;

            base.Copy(other);

            QuestionBankId = model.QuestionBankId;
            OptionText = model.OptionText;
            OptionOrder = model.OptionOrder;
            IsCorrect = model.IsCorrect;
            IsPartialCorrect = model.IsPartialCorrect;
            Marks = model.Marks;
            Explanation = model.Explanation;
        }

        #endregion
    }
}
