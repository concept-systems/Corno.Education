using Corno.Data.Common;
using Mapster;
using System;

namespace Corno.Data.Corno.Question_Bank.Models;

[Serializable]
public class QuestionAppointmentTypeDetail : BaseModel
{
    #region -- Properties --
    public int? QuestionAppointmentId { get; set; }

    public int? PaperCategoryId { get; set; }
    public int? QuestionTypeId { get; set; }
    public int? QuestionCount { get; set; }
    public int? CompletedCount { get; set; }

    [AdaptIgnore]
    protected virtual QuestionAppointment QuestionAppointment { get; set; }

    #endregion

    #region -- Public Methods --
    public void Copy(QuestionAppointmentTypeDetail other)
    {
        if (null == other) return;

        SerialNo = other.SerialNo;
        Code = other.Code;

        PaperCategoryId = other.PaperCategoryId;
        QuestionTypeId = other.QuestionTypeId;
        QuestionCount = other.QuestionCount;
        CompletedCount = other.CompletedCount;
        
        Status = other.Status;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;
    }
    #endregion
}