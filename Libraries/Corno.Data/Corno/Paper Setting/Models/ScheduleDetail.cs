using Corno.Data.Common;
using Mapster;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corno.Data.Corno.Paper_Setting.Models;

[Serializable]
public class ScheduleDetail : BaseModel
{
    #region -- Constructors --
    public ScheduleDetail()
    {
        AvailableSets = 0;
        SetsToBeDrawn = 0;
        UsedSets = 0;
        Balance = 0;
    }
    #endregion

    #region -- Properties --
    public int? ScheduleId { get; set; }
    public int? SubjectId { get; set; }
    public DateTime? FromDate { get; set; }
    public DateTime? ToDate { get; set; }
    public DateTime? EndDate { get; set; }
    public TimeSpan? Time { get; set; }
    
    public int? AvailableSets { get; set; }
    public int? SetsToBeDrawn { get; set; }
    public int? UsedSets { get; set; }
    public int? Balance { get; set; }

    public int? OutwardNo { get; set; }

    [NotMapped]
    public string SubjectName { get; set; }

    [AdaptIgnore]
    protected virtual Schedule Schedule { get; set; }

    #endregion

    #region -- Public Methods --
    public void Copy(ScheduleDetail other)
    {
        if (null == other) return;

        SerialNo = other.SerialNo;
        Code = other.Code;

        SubjectId = other.SubjectId;
        FromDate = other.FromDate;
        ToDate = other.ToDate;
        EndDate = other.EndDate;
        Time = other.Time;

        AvailableSets = other.AvailableSets;
        SetsToBeDrawn = other.SetsToBeDrawn;
        UsedSets = other.UsedSets;
        Balance = other.Balance;

        OutwardNo = other.OutwardNo;

        Status = other.Status;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;
    }
    #endregion
}