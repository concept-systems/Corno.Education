using Corno.Data.Common;
using Mapster;
using System;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corno.Data.Corno.Paper_Setting.Models;

[Serializable]
public class AppointmentDetail : BaseModel
{
    #region -- Constructors --
    public AppointmentDetail()
    {
        IsInternal = false;
        IsBarred = false;
        IsChairman = false;
        IsPaperSetter = false;
        IsModerator = false;
        IsManuscript = false;

        NoOfAttempts = 0;
        EmailCount = 0;
        SmsCount = 0;
    }
    #endregion

    #region -- Properties --
    public int? AppointmentId { get; set; }

    public int? StaffId { get; set; }
    public bool? IsInternal { get; set; }
    public bool? IsBarred { get; set; }
    public bool IsChairman { get; set; }
    public bool IsPaperSetter { get; set; }
    public bool IsModerator { get; set; }
    public bool IsManuscript { get; set; }
    public int? OriginalId { get; set; }
    public int? NoOfAttempts { get; set; }
    public int? EmailCount { get; set; }
    public DateTime? EmailDate { get; set; }
    public int? SmsCount { get; set; }

    [NotMapped]
    public string StaffName { get; set; }
    [NotMapped]
    public string EmailId { get; set; }
    [NotMapped]
    public string MobileNo { get; set; }
    [NotMapped]
    [DisplayName("")]
    public string Group => IsInternal ?? false ? "Internal" : "External";

    [AdaptIgnore]
    protected virtual Appointment Appointment { get; set; }

    #endregion

    #region -- Public Methods --
    public void Copy(AppointmentDetail other)
    {
        if (null == other) return;

        SerialNo = other.SerialNo;
        Code = other.Code;

        StaffId = other.StaffId;
        IsInternal = other.IsInternal;
        IsBarred = other.IsBarred;
        IsChairman = other.IsChairman;
        IsPaperSetter = other.IsPaperSetter;
        IsModerator = other.IsModerator;
        IsManuscript = other.IsManuscript;
        
        OriginalId = other.OriginalId;
        NoOfAttempts = other.NoOfAttempts;
        EmailCount = other.EmailCount;
        EmailDate = other.EmailDate;
        SmsCount = other.SmsCount;

        Status = other.Status;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;
    }
    #endregion
}