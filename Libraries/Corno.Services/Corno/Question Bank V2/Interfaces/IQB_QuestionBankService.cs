using Corno.Data.Corno.Question_Bank_V2.Models;
using Corno.Services.Corno.Interfaces;
using System.Collections.Generic;

namespace Corno.Services.Corno.Question_Bank_V2.Interfaces
{
    public interface IQB_QuestionBankService : IMainService<QB_QuestionBank>
    {
        void SaveQuestion(QB_QuestionBank question, string userId, int instanceId, bool isEdit);
        void DecryptQuestion(QB_QuestionBank question);
        void SubmitForCheck(int questionId, string userId);
        void ApproveQuestion(int questionId, string userId, string roleName, string comments);
        void RejectQuestion(int questionId, string userId, string roleName, string reason);
        void RequestRevision(int questionId, string userId, string comments);
        List<QB_QuestionBank> GetQuestionsForSetter(string userId, int instanceId, int? subjectId);
        List<QB_QuestionBank> GetQuestionsForChecker(string userId, int instanceId, int? subjectId);
        List<QB_QuestionBank> GetApprovedQuestions(int instanceId, int subjectId);
        string GenerateQuestionCode(int instanceId);
    }
}
