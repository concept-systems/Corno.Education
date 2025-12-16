using Corno.Data.Common;
using Mapster;
using System;

namespace Corno.Data.Corno.Question_Bank.Models;

[Serializable]
public class QuestionAppointmentDetail : BaseModel
{
    #region -- Properties --
    public int? QuestionAppointmentId { get; set; }

    public int? StaffId { get; set; }
    public bool IsSetter { get; set; }
    public bool IsChecker { get; set; }
    public int? EmailCount { get; set; } = 0;
    public int? SmsCount { get; set; } = 0;

    [AdaptIgnore]
    protected virtual QuestionAppointment QuestionAppointment { get; set; }

    #endregion

    #region -- Public Methods --
    public void Copy(QuestionAppointmentDetail other)
    {
        if (null == other) return;

        SerialNo = other.SerialNo;
        Code = other.Code;

        StaffId = other.StaffId;
        IsChecker = other.IsChecker;
        IsSetter = other.IsSetter;
        EmailCount = other.EmailCount;
        SmsCount = other.SmsCount;

        Status = other.Status;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;
    }
    #endregion
}