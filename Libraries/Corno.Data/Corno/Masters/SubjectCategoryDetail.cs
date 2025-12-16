using Corno.Data.Common;
using Mapster;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corno.Data.Corno.Masters;

[Serializable]
public class SubjectCategoryDetail : BaseModel
{
    #region -- Properties --
    public int? SubjectId { get; set; }
    public int CategoryId { get; set; }
    public int PaperCategoryId { get; set; }
    public int? MaxMarks { get; set; }
    public int? PassingMarks { get; set; }
    public int? ExemptionMarks { get; set; }

    [NotMapped]
    public string CategoryName { get; set; }
    [NotMapped]
    public string PaperCategoryName { get; set; }

    [AdaptIgnore]
    protected virtual Subject Subject { get; set; }

    #endregion

    #region -- Public Methods --
    public void Copy(SubjectCategoryDetail other)
    {
        if (null == other) return;

        SerialNo = other.SerialNo;
        Code = other.Code;

        CategoryId = other.CategoryId;
        PaperCategoryId = other.PaperCategoryId;
        MaxMarks = other.MaxMarks;
        PassingMarks = other.PassingMarks;
        ExemptionMarks = other.ExemptionMarks;

        Status = other.Status;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;
    }
    #endregion
}