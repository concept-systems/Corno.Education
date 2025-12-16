using Corno.Data.Common;
using Mapster;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corno.Data.Corno.Paper_Setting.Models;

[Serializable]
public class RemunerationDetail : BaseModel
{
    #region -- Constructors --
    public RemunerationDetail()
    {
    }
    #endregion

    #region -- Properties --
    public int? RemunerationId { get; set; }
    public int? CoursePartId { get; set; }
    public double? Fee { get; set; }
    public double? Others { get; set; }
    public double? SchemeOfMarking { get; set; }
    public double? ModelAnswers { get; set; }

    [NotMapped]
    public string CoursePartName { get; set; }

    [AdaptIgnore]
    protected virtual Remuneration Remuneration { get; set; }

    #endregion

    #region -- Public Methods --
    public void Copy(RemunerationDetail other)
    {
        if (null == other) return;

        SerialNo = other.SerialNo;
        Code = other.Code;

        CoursePartId = other.CoursePartId;
        Fee = other.Fee;
        Others = other.Others;
        SchemeOfMarking = other.SchemeOfMarking;
        ModelAnswers = other.ModelAnswers;
        
        Status = other.Status;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;
    }
    #endregion
}