using System;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Data.Common;
using Mapster;

namespace Corno.Data.Corno.Masters;

[Serializable]
public class BranchSubjectDetail : BaseModel
{
    #region -- Properties --
    public int? BranchId { get; set; }
    public int CoursePartId { get; set; }
    public int SubjectId { get; set; }

    [NotMapped]
    public string CoursePartName { get; set; }
    [NotMapped]
    public string SubjectName { get; set; }

    [AdaptIgnore]
    protected virtual Branch Branch { get; set; }
    #endregion

    #region -- Public Methods --
    public void Copy(BranchSubjectDetail other)
    {
        if (null == other) return;

        SerialNo = other.SerialNo;
        Code = other.Code;

        CoursePartId = other.CoursePartId;
        SubjectId = other.SubjectId;

        Status = other.Status;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;
    }
    #endregion
}