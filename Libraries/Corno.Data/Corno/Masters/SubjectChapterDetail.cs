using Corno.Data.Common;
using Mapster;
using System;

namespace Corno.Data.Corno.Masters;

[Serializable]
public class SubjectChapterDetail : BaseModel
{
    #region -- Constructors --
    public SubjectChapterDetail()
    {
    }
    #endregion

    #region -- Properties --
    public int? SubjectId { get; set; }
    public string Name { get; set; }
    public string Description { get; set; }

    [AdaptIgnore]
    protected virtual Subject Subject { get; set; }

    #endregion

    #region -- Public Methods --
    public void Copy(SubjectChapterDetail other)
    {
        if (null == other) return;

        SerialNo = other.SerialNo;
        Code = other.Code;

        Name = other.Name;
        Description = other.Description;

        Status = other.Status;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;
    }
    #endregion
}