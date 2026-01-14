using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using Corno.Data.Common;

namespace Corno.Data.Corno.Question_Bank_V2.Models
{
    [Serializable]
    public class QB_PaperDetail : BaseModel
    {
        #region -- Properties --

        [Required]
        public int? PaperId { get; set; }

        public int? QuestionBankId { get; set; }

        public int? StructureDetailId { get; set; }

        [Required]
        public int SectionNo { get; set; }

        [Required]
        public int QuestionNo { get; set; }

        [Required]
        public int? QuestionTypeId { get; set; }

        public int? UnitId { get; set; }

        public int? TopicId { get; set; }

        [Required]
        public int? DifficultyLevelId { get; set; }

        [Required]
        public int? TaxonomyLevelId { get; set; }

        [Required]
        public decimal Marks { get; set; }

        [Required]
        [AllowHtml]
        public string QuestionText { get; set; }

        [AllowHtml]
        public string ModelAnswer { get; set; }

        [StringLength(20)]
        public string SelectionMethod { get; set; } // Auto, Manual

        public string SelectionCriteria { get; set; } // JSON

        // NotMapped properties
        [NotMapped]
        public string QuestionTypeName { get; set; }

        [NotMapped]
        public string UnitName { get; set; }

        [NotMapped]
        public string DifficultyLevelName { get; set; }

        [NotMapped]
        public string TaxonomyLevelName { get; set; }

        #endregion

        #region -- Methods --

        public override void Copy(BaseModel other)
        {
            if (other is not QB_PaperDetail model) return;

            base.Copy(other);

            PaperId = model.PaperId;
            QuestionBankId = model.QuestionBankId;
            StructureDetailId = model.StructureDetailId;
            SectionNo = model.SectionNo;
            QuestionNo = model.QuestionNo;
            QuestionTypeId = model.QuestionTypeId;
            UnitId = model.UnitId;
            DifficultyLevelId = model.DifficultyLevelId;
            TaxonomyLevelId = model.TaxonomyLevelId;
            Marks = model.Marks;
            QuestionText = model.QuestionText;
            ModelAnswer = model.ModelAnswer;
            SelectionMethod = model.SelectionMethod;
        }

        #endregion
    }
}
