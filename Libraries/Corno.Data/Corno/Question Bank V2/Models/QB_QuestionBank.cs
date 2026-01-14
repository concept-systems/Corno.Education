using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web.Mvc;
using Corno.Data.Common;

namespace Corno.Data.Corno.Question_Bank_V2.Models
{
    [Serializable]
    public class QB_QuestionBank : BaseModel
    {
        #region -- Constructors --
        public QB_QuestionBank()
        {
            QuestionOptions = new List<QB_QuestionOptions>();
        }
        #endregion

        #region -- Properties --

        [Required]
        [StringLength(50)]
        public string QuestionCode { get; set; }

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

        // Encrypted storage columns (in database)
        [Column("QuestionText")]
        public byte[] QuestionTextEncrypted { get; set; }

        [Column("ModelAnswer")]
        public byte[] ModelAnswerEncrypted { get; set; }

        [Column("AnswerExplanation")]
        public byte[] AnswerExplanationEncrypted { get; set; }

        [Column("Hints")]
        public byte[] HintsEncrypted { get; set; }

        [Column("SolutionSteps")]
        public byte[] SolutionStepsEncrypted { get; set; }

        // Plain text properties (for UI - not stored in DB)
        [NotMapped]
        [AllowHtml]
        public string QuestionText
        {
            get { return _questionTextPlain; }
            set { _questionTextPlain = value; }
        }

        [NotMapped]
        [AllowHtml]
        public string ModelAnswer
        {
            get { return _modelAnswerPlain; }
            set { _modelAnswerPlain = value; }
        }

        [NotMapped]
        [AllowHtml]
        public string AnswerExplanation
        {
            get { return _answerExplanationPlain; }
            set { _answerExplanationPlain = value; }
        }

        [NotMapped]
        [AllowHtml]
        public string Hints
        {
            get { return _hintsPlain; }
            set { _hintsPlain = value; }
        }

        [NotMapped]
        [AllowHtml]
        public string SolutionSteps
        {
            get { return _solutionStepsPlain; }
            set { _solutionStepsPlain = value; }
        }

        // Private fields for temporary storage during model binding
        private string _questionTextPlain;
        private string _modelAnswerPlain;
        private string _answerExplanationPlain;
        private string _hintsPlain;
        private string _solutionStepsPlain;

        [Required]
        public int? QuestionTypeId { get; set; }

        public int? UnitId { get; set; } // Maps to SubjectChapterDetail.Id

        public int? TopicId { get; set; }

        public int? SubTopicId { get; set; }

        [Required]
        public int? DifficultyLevelId { get; set; }

        [Required]
        public int? TaxonomyLevelId { get; set; }

        public int? LearningOutcomeId { get; set; }

        public int? CognitiveLevelId { get; set; }

        [Required]
        public decimal? Marks { get; set; }

        public int? TimeAllotted { get; set; }

        public decimal? NegativeMarks { get; set; }

        public bool PartialMarksAllowed { get; set; }

        public bool HasImage { get; set; }

        public bool HasAudio { get; set; }

        public bool HasVideo { get; set; }

        public bool HasFormula { get; set; }

        public int? LanguageId { get; set; }

        [StringLength(200)]
        public string Source { get; set; }

        [StringLength(500)]
        public string Reference { get; set; }

        [StringLength(500)]
        public string Tags { get; set; }

        [StringLength(500)]
        public string Keywords { get; set; }

        [Required]
        [StringLength(20)]
        public string Status { get; set; } = "Draft";

        [Required]
        [StringLength(128)]
        public string SetterUserId { get; set; }

        [StringLength(128)]
        public string CheckerUserId { get; set; }

        [StringLength(128)]
        public string ModeratorUserId { get; set; }

        public string CheckerComments { get; set; }

        public DateTime? CheckerApprovedDate { get; set; }

        public string ModeratorComments { get; set; }

        public DateTime? ModeratorApprovedDate { get; set; }

        public string RejectionReason { get; set; }

        [StringLength(20)]
        public string RejectedByRole { get; set; }

        [StringLength(128)]
        public string RejectedByUserId { get; set; }

        public decimal? QualityScore { get; set; }

        public int UsageCount { get; set; }

        public DateTime? LastUsedDate { get; set; }

        public int Version { get; set; } = 1;

        public int? ParentQuestionId { get; set; }

        public bool IsLatestVersion { get; set; } = true;

        public int? AverageTimeTaken { get; set; }

        public decimal? SuccessRate { get; set; }

        public decimal? DiscriminationIndex { get; set; }

        // Navigation properties
        public virtual List<QB_QuestionOptions> QuestionOptions { get; set; }

        // NotMapped properties for display
        [NotMapped]
        public string InstanceName { get; set; }

        [NotMapped]
        public string FacultyName { get; set; }

        [NotMapped]
        public string CourseName { get; set; }

        [NotMapped]
        public string CoursePartName { get; set; }

        [NotMapped]
        public string BranchName { get; set; }

        [NotMapped]
        public string SubjectName { get; set; }

        [NotMapped]
        public string PaperCategoryName { get; set; }

        [NotMapped]
        public string QuestionTypeName { get; set; }

        [NotMapped]
        public string UnitName { get; set; }

        [NotMapped]
        public string DifficultyLevelName { get; set; }

        [NotMapped]
        public string TaxonomyLevelName { get; set; }

        [NotMapped]
        public string SetterUserName { get; set; }

        [NotMapped]
        public string CheckerUserName { get; set; }

        [NotMapped]
        public string ModeratorUserName { get; set; }

        #endregion

        #region -- Methods --

        public void SetQuestionTextPlain(string plainText)
        {
            _questionTextPlain = plainText;
        }

        public string GetQuestionTextPlain()
        {
            return _questionTextPlain;
        }

        public void SetModelAnswerPlain(string plainText)
        {
            _modelAnswerPlain = plainText;
        }

        public string GetModelAnswerPlain()
        {
            return _modelAnswerPlain;
        }

        public void SetAnswerExplanationPlain(string plainText)
        {
            _answerExplanationPlain = plainText;
        }

        public string GetAnswerExplanationPlain()
        {
            return _answerExplanationPlain;
        }

        public void SetHintsPlain(string plainText)
        {
            _hintsPlain = plainText;
        }

        public string GetHintsPlain()
        {
            return _hintsPlain;
        }

        public void SetSolutionStepsPlain(string plainText)
        {
            _solutionStepsPlain = plainText;
        }

        public string GetSolutionStepsPlain()
        {
            return _solutionStepsPlain;
        }

        public override void Copy(BaseModel other)
        {
            if (other is not QB_QuestionBank model) return;

            base.Copy(other);

            QuestionCode = model.QuestionCode;
            InstanceId = model.InstanceId;
            FacultyId = model.FacultyId;
            CourseId = model.CourseId;
            CoursePartId = model.CoursePartId;
            BranchId = model.BranchId;
            SubjectId = model.SubjectId;
            PaperCategoryId = model.PaperCategoryId;
            QuestionTypeId = model.QuestionTypeId;
            UnitId = model.UnitId;
            DifficultyLevelId = model.DifficultyLevelId;
            TaxonomyLevelId = model.TaxonomyLevelId;
            Marks = model.Marks;
            Status = model.Status;
        }

        #endregion
    }
}
