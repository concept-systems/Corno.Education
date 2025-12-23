using System;
using Corno.Data.Corno.Question_Bank.Models;

namespace Corno.Data.Corno.Question_Bank.Dtos;

[Serializable]
public class QuestionListDto : QuestionBankViewModel
{
    #region -- Properties --
    public int? SerialNo { get; set; }
    public int? Id { get; set; }
    public DateTime? Date { get; set; }
    public string QuestionTypeName { get; set; }
    public string SubjectName { get; set; }
    public string ChapterName { get; set; }
    public string DifficultyLevelName { get; set; }
    public string Description { get; set; }
    public string ModelAnswer { get; set; }
    public string PaperCategoryName { get; set; }
    public double? Marks { get; set; }
    public int? CoNo { get; set; }
    public int? TaxonomySerialNo { get; set; }
    public string Status { get; set; }

    #endregion
}