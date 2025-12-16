using System;
using Corno.Data.Corno.Question_Bank.Models;

namespace Corno.Data.Corno.Question_Bank.Dtos;

[Serializable]
public class QuestionIndexDto : QuestionBankModel
{
    #region -- Properties --
    public string PaperCategoryName { get; set; }
    #endregion
}