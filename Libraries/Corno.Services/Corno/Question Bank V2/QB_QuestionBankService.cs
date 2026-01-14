using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text.RegularExpressions;
using System.Web;
using Corno.Data.Corno.Question_Bank_V2.Models;
using Corno.Globals.Constants;
using Corno.Logger;
using Corno.Services.Corno;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Corno.Question_Bank_V2.Interfaces;
using Corno.Services.Corno.Question_Bank_V2.Security;

namespace Corno.Services.Corno.Question_Bank_V2
{
    public class QB_QuestionBankService : MainService<QB_QuestionBank>, IQB_QuestionBankService
    {
        private readonly QuestionEncryptionService _encryptionService;
        
        public QB_QuestionBankService(QuestionEncryptionService encryptionService)
        {
            _encryptionService = encryptionService;
            SetIncludes(nameof(QB_QuestionBank.QuestionOptions));
        }
        
        public new QB_QuestionBank GetById(int? id)
        {
            var question = base.GetById(id);
            if (question != null)
            {
                DecryptQuestion(question);
            }
            return question;
        }
        
        public new IEnumerable<QB_QuestionBank> Get(System.Linq.Expressions.Expression<Func<QB_QuestionBank, bool>> predicate = null, 
            Func<IQueryable<QB_QuestionBank>, IOrderedQueryable<QB_QuestionBank>> orderBy = null)
        {
            var query = GetQuery();
            
            if (predicate != null)
                query = query.Where(predicate);
            
            if (orderBy != null)
                query = orderBy(query);
            else
                query = query.OrderByDescending(x => x.CreatedDate);
            
            var questions = query.ToList();
            
            foreach (var question in questions)
            {
                DecryptQuestion(question);
            }
            
            return questions;
        }
        
        public int GetCount(System.Linq.Expressions.Expression<Func<QB_QuestionBank, bool>> predicate = null)
        {
            var query = GetQuery();
            if (predicate != null)
                query = query.Where(predicate);
            return query.Count();
        }
        
        public void SaveQuestion(QB_QuestionBank question, string userId, int instanceId, bool isEdit)
        {
            // Validate user permission
            ValidateUserPermission(question, userId, isEdit);
            
            // Sanitize HTML content
            var questionText = SanitizeHtml(question.GetQuestionTextPlain());
            var modelAnswer = SanitizeHtml(question.GetModelAnswerPlain());
            var answerExplanation = SanitizeHtml(question.GetAnswerExplanationPlain());
            var hints = SanitizeHtml(question.GetHintsPlain());
            var solutionSteps = SanitizeHtml(question.GetSolutionStepsPlain());
            
            // Encrypt before saving
            question.QuestionTextEncrypted = _encryptionService.Encrypt(questionText);
            question.ModelAnswerEncrypted = string.IsNullOrEmpty(modelAnswer) ? null : _encryptionService.Encrypt(modelAnswer);
            question.AnswerExplanationEncrypted = string.IsNullOrEmpty(answerExplanation) ? null : _encryptionService.Encrypt(answerExplanation);
            question.HintsEncrypted = string.IsNullOrEmpty(hints) ? null : _encryptionService.Encrypt(hints);
            question.SolutionStepsEncrypted = string.IsNullOrEmpty(solutionSteps) ? null : _encryptionService.Encrypt(solutionSteps);
            
            // Validate content
            ValidateQuestionContent(question);
            
            if (isEdit)
            {
                var existing = base.GetById(question.Id);
                if (existing == null)
                    throw new Exception("Question not found.");
                
                // Log changes
                LogQuestionChanges(existing, question, userId);
                
                // Update encrypted fields
                existing.QuestionTextEncrypted = question.QuestionTextEncrypted;
                existing.ModelAnswerEncrypted = question.ModelAnswerEncrypted;
                existing.AnswerExplanationEncrypted = question.AnswerExplanationEncrypted;
                existing.HintsEncrypted = question.HintsEncrypted;
                existing.SolutionStepsEncrypted = question.SolutionStepsEncrypted;
                
                // Update other fields
                existing.QuestionTypeId = question.QuestionTypeId;
                existing.UnitId = question.UnitId;
                existing.DifficultyLevelId = question.DifficultyLevelId;
                existing.TaxonomyLevelId = question.TaxonomyLevelId;
                existing.Marks = question.Marks;
                existing.TimeAllotted = question.TimeAllotted;
                existing.Tags = question.Tags;
                existing.Keywords = question.Keywords;
                
                existing.ModifiedBy = userId;
                existing.ModifiedDate = DateTime.Now;
                
                UpdateAndSave(existing);
            }
            else
            {
                // Generate question code
                question.QuestionCode = GenerateQuestionCode(instanceId);
                question.CreatedBy = userId;
                question.CreatedDate = DateTime.Now;
                question.SetterUserId = userId;
                question.Status = "Draft";
                
                AddAndSave(question);
            }
        }
        
        public void DecryptQuestion(QB_QuestionBank question)
        {
            if (question.QuestionTextEncrypted != null)
            {
                var decrypted = _encryptionService.Decrypt(question.QuestionTextEncrypted);
                question.SetQuestionTextPlain(decrypted);
            }
            
            if (question.ModelAnswerEncrypted != null)
            {
                var decrypted = _encryptionService.Decrypt(question.ModelAnswerEncrypted);
                question.SetModelAnswerPlain(decrypted);
            }
            
            if (question.AnswerExplanationEncrypted != null)
            {
                var decrypted = _encryptionService.Decrypt(question.AnswerExplanationEncrypted);
                question.SetAnswerExplanationPlain(decrypted);
            }
            
            if (question.HintsEncrypted != null)
            {
                var decrypted = _encryptionService.Decrypt(question.HintsEncrypted);
                question.SetHintsPlain(decrypted);
            }
            
            if (question.SolutionStepsEncrypted != null)
            {
                var decrypted = _encryptionService.Decrypt(question.SolutionStepsEncrypted);
                question.SetSolutionStepsPlain(decrypted);
            }
        }
        
        public void SubmitForCheck(int questionId, string userId)
        {
            var question = base.GetById(questionId);
            if (question == null)
                throw new Exception("Question not found.");
            
            if (question.SetterUserId != userId)
                throw new UnauthorizedAccessException("You can only submit your own questions.");
            
            if (question.Status != "Draft" && question.Status != "Needs Revision")
                throw new Exception($"Cannot submit question with status: {question.Status}");
            
            question.Status = "Submitted for Check";
            question.ModifiedBy = userId;
            question.ModifiedDate = DateTime.Now;
            
            UpdateAndSave(question);
        }
        
        public void ApproveQuestion(int questionId, string userId, string roleName, string comments)
        {
            var question = base.GetById(questionId);
            if (question == null)
                throw new Exception("Question not found.");
            
            if (roleName == "Question Checker")
            {
                question.Status = "Approved by Checker";
                question.CheckerUserId = userId;
                question.CheckerApprovedDate = DateTime.Now;
                question.CheckerComments = comments;
            }
            else if (roleName == "Moderator")
            {
                question.Status = "Approved";
                question.ModeratorUserId = userId;
                question.ModeratorApprovedDate = DateTime.Now;
                question.ModeratorComments = comments;
            }
            
            question.ModifiedBy = userId;
            question.ModifiedDate = DateTime.Now;
            
            UpdateAndSave(question);
        }
        
        public void RejectQuestion(int questionId, string userId, string roleName, string reason)
        {
            var question = base.GetById(questionId);
            if (question == null)
                throw new Exception("Question not found.");
            
            question.Status = $"Rejected by {roleName}";
            question.RejectedByRole = roleName;
            question.RejectedByUserId = userId;
            question.RejectionReason = reason;
            question.ModifiedBy = userId;
            question.ModifiedDate = DateTime.Now;
            
            UpdateAndSave(question);
        }
        
        public void RequestRevision(int questionId, string userId, string comments)
        {
            var question = base.GetById(questionId);
            if (question == null)
                throw new Exception("Question not found.");
            
            question.Status = "Needs Revision";
            question.CheckerComments = comments;
            question.ModifiedBy = userId;
            question.ModifiedDate = DateTime.Now;
            
            UpdateAndSave(question);
        }
        
        public List<QB_QuestionBank> GetQuestionsForSetter(string userId, int instanceId, int? subjectId)
        {
            var query = GetQuery().Where(q => q.InstanceId == instanceId && 
                                             q.SetterUserId == userId &&
                                             q.Status != StatusConstants.Deleted);
            
            if (subjectId.HasValue)
                query = query.Where(q => q.SubjectId == subjectId);
            
            var questions = query.ToList();
            foreach (var q in questions)
                DecryptQuestion(q);
            
            return questions;
        }
        
        public List<QB_QuestionBank> GetQuestionsForChecker(string userId, int instanceId, int? subjectId)
        {
            var query = GetQuery().Where(q => q.InstanceId == instanceId &&
                                             (q.Status == "Submitted for Check" || q.Status == "Under Check") &&
                                             q.Status != StatusConstants.Deleted);
            
            if (subjectId.HasValue)
                query = query.Where(q => q.SubjectId == subjectId);
            
            var questions = query.ToList();
            foreach (var q in questions)
                DecryptQuestion(q);
            
            return questions;
        }
        
        public List<QB_QuestionBank> GetApprovedQuestions(int instanceId, int subjectId)
        {
            var questions = GetQuery().Where(q => q.InstanceId == instanceId &&
                                        q.SubjectId == subjectId &&
                                        q.Status == "Approved" &&
                                        q.Status != StatusConstants.Deleted).ToList();
            
            foreach (var q in questions)
                DecryptQuestion(q);
            
            return questions;
        }
        
        public string GenerateQuestionCode(int instanceId)
        {
            var year = DateTime.Now.Year;
            var count = GetQuery().Count(q => q.InstanceId == instanceId && 
                                             q.QuestionCode.StartsWith($"QB-{year}"));
            
            return $"QB-{year}-{(count + 1):D5}";
        }
        
        private void ValidateUserPermission(QB_QuestionBank question, string userId, bool isEdit)
        {
            if (isEdit)
            {
                var existing = base.GetById(question.Id);
                if (existing.SetterUserId != userId)
                    throw new UnauthorizedAccessException("You can only edit your own questions.");
            }
        }
        
        private string SanitizeHtml(string html)
        {
            if (string.IsNullOrEmpty(html)) return html;
            
            // Basic HTML sanitization - in production, use HtmlSanitizer library
            // For now, just trim
            return html.Trim();
        }
        
        private void ValidateQuestionContent(QB_QuestionBank question)
        {
            if (string.IsNullOrEmpty(question.GetQuestionTextPlain()))
                throw new Exception("Question text is required.");
            
            if (question.Marks <= 0)
                throw new Exception("Marks must be greater than zero.");
        }
        
        private void LogQuestionChanges(QB_QuestionBank oldQuestion, QB_QuestionBank newQuestion, string userId)
        {
            // Log changes to change log table
            // Implementation depends on change log service
        }
    }
}
