using System;
using System.ComponentModel.DataAnnotations;
using Corno.Data.Common;

namespace Corno.Data.Corno.Question_Bank_V2.Models
{
    [Serializable]
    public class QB_QuestionType : BaseModel
    {
        #region -- Properties --

        [Required]
        [StringLength(100)]
        public string Name { get; set; }

        [Required]
        [StringLength(20)]
        public string Code { get; set; }

        [StringLength(500)]
        public string Description { get; set; }

        public bool HasOptions { get; set; }

        public bool HasSubQuestions { get; set; }

        public bool AllowPartialMarks { get; set; } = true;

        public decimal? DefaultMarks { get; set; }

        [StringLength(100)]
        public string Icon { get; set; }

        public int DisplayOrder { get; set; }

        public bool IsActive { get; set; } = true;

        #endregion

        #region -- Methods --

        public override void Copy(BaseModel other)
        {
            if (other is not QB_QuestionType model) return;

            base.Copy(other);

            Name = model.Name;
            Code = model.Code;
            Description = model.Description;
            HasOptions = model.HasOptions;
            HasSubQuestions = model.HasSubQuestions;
            AllowPartialMarks = model.AllowPartialMarks;
            DefaultMarks = model.DefaultMarks;
            DisplayOrder = model.DisplayOrder;
            IsActive = model.IsActive;
        }

        #endregion
    }
}
