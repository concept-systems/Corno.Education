using Corno.Data.Common;
using Mapster;
using System;
using System.ComponentModel.DataAnnotations;

namespace Corno.Data.Corno.Question_Bank.Models;

[Serializable]
public class StructureDetail : BaseModel
{
    #region -- Properties --
    public int? StructureId { get; set; }
    [Range(1, int.MaxValue, ErrorMessage = "Value must be greater than zero")]
    public int? QuestionNo { get; set; }
    public int? SectionNo { get; set; }
    public string ChapterNos { get; set; }
    public int? NofOptions { get; set; }
    public double? Marks { get; set; }
    public int? QuestionTypeId { get; set; }
    public string QuestionText { get; set; }
    public int? AttemptCount { get; set; }

    [AdaptIgnore]
    protected virtual Structure Structure { get; set; }

    #endregion

    #region -- Public Methods --
    public void Copy(StructureDetail other)
    {
        if (null == other) return;

        SerialNo = other.SerialNo;
        Code = other.Code;

        QuestionNo = other.QuestionNo;
        SectionNo = other.SectionNo;
        ChapterNos = other.ChapterNos;
        NofOptions = other.NofOptions;
        Marks = other.Marks;
        QuestionTypeId = other.QuestionTypeId;
        QuestionText = other.QuestionText;
        AttemptCount = other.AttemptCount;

        Status = other.Status;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = DateTime.Now;
    }
    #endregion
}
