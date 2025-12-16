using Corno.Data.Common;
using Mapster;
using System;

namespace Corno.Data.Corno.Masters;

[Serializable]
public class StudentResearchDetail : BaseModel
{
    #region -- Constructors -- 
    public StudentResearchDetail()
    {
    }
    #endregion

    #region -- Properties --
    public string Description { get; set; }
    public  DateTime? Date { get; set; }
    public byte[] Document { get; set; }
    public string Comments { get; set; }

    [AdaptIgnore]
    protected virtual Student Student { get; set; }
    #endregion

    #region -- Public Methods --
    public void Copy(StudentResearchDetail other)
    {
        if (null == other) return;

        SerialNo = other.SerialNo;
        Code = other.Code;

        Date = other.Date;
        Document = other.Document;

        Status = other.Status;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;
    }
    #endregion
}