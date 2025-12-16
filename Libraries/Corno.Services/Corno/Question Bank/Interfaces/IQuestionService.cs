using Corno.Data.Corno.Question_Bank;
using Corno.Services.Corno.Interfaces;
using System.Collections.Generic;
using Corno.Data.Corno.Question_Bank.Dtos;
using Corno.Data.Corno.Question_Bank.Models;

namespace Corno.Services.Corno.Question_Bank.Interfaces;

public interface IQuestionService : IMainService<Question>
{
    #region -- Methods --

    void ValidateFields(Question question);
    //List<QuestionIndexDto> GetQuestions(QuestionBankViewModel bankViewModel);

    //void Save(Question question);
    void Accept(Question question);
    void Reject(Question question);

    // Common create/edit save logic moved to service
    void SaveQuestion(Question question, int staffId, int instanceId, bool isEdit);

    #endregion
}