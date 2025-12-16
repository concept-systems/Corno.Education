using Corno.Data.Common;
using Mapster;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corno.Data.Corno.Masters;

[Serializable]
public class SubjectSectionDetail : BaseModel
{
    #region -- Properties --
    public int? SubjectId { get; set; }
    public int PaperCategoryId { get; set; }
    [Required]
    public int SectionNo { get; set; }
    public int NoOfQuestions { get; set; }
    public int AttemptCount { get; set; }

    [NotMapped]
    public string PaperCategoryName { get; set; }

    [AdaptIgnore]
    protected virtual Subject Subject { get; set; }

    #endregion

    #region -- Public Methods --
    public void Copy(SubjectSectionDetail other)
    {
        if (null == other) return;

        SerialNo = other.SerialNo;
        Code = other.Code;

        PaperCategoryId = other.PaperCategoryId;
        SectionNo = other.SectionNo;
        NoOfQuestions = other.NoOfQuestions;
        AttemptCount = other.AttemptCount;

        Status = other.Status;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;
    }
    #endregion
}