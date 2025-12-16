using Corno.Data.Common;
using Mapster;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corno.Data.Corno.Masters;

[Serializable]
public class SubjectInstructionDetail : BaseModel
{
    #region -- Properties --
    public int? SubjectId { get; set; }
    public int PaperCategoryId { get; set; }
    public string Description { get; set; }

    [NotMapped]
    public string PaperCategoryName { get; set; }

    [AdaptIgnore]
    protected virtual Subject Subject { get; set; }

    #endregion

    #region -- Public Methods --
    public void Copy(SubjectInstructionDetail other)
    {
        if (null == other) return;

        SerialNo = other.SerialNo;
        Code = other.Code;

        PaperCategoryId = other.PaperCategoryId;
        Description = other.Description;

        Status = other.Status;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;
    }
    #endregion
}