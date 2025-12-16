using System;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Data.Common;
using Mapster;

namespace Corno.Data.Corno.Paper_Setting.Models;

[Serializable]
public class AppointmentBillDetail : BaseModel
{
    #region -- Constructors --
    public AppointmentBillDetail()
    {
        SetsDrawn = 0;
        CourseFee = 0;
        AllottedFee = 0;
        Ta = 0;
        Da = 0;
        ChairmanAllowance = 0;
        TranslationCharges = 0;
        Travel = 0;
        BasicPay = 0;
        OtherAllowance = 0;
        RemunerationOthers = 0;

        TotalFee = 0;
    }
    #endregion

    #region -- Properties --
    public int? AppointmentId { get; set; }

    public int? StaffId { get; set; }
    public DateTime? BillDate { get; set; }

    public int? SetsDrawn { get; set; }
    public double? CourseFee { get; set; }
    public double? AllottedFee { get; set; }
    public double? Ta { get; set; }
    public double? Da { get; set; }
    public double? ChairmanAllowance { get; set; }
    public double? TranslationCharges { get; set; }
    public double? Travel { get; set; }
    public double? BasicPay { get; set; }
    public double? OtherAllowance { get; set; }
    public double? TotalFee { get; set; }

    public DateTime? DateOfJourney { get; set; }
    public string FromPlace { get; set; }
    public string ToPlace { get; set; }
    public int? TotalDistance { get; set; }
    public string TravelingMode { get; set; }
    public DateTime? TimeIn { get; set; }
    public DateTime? TimeOut { get; set; }

    public bool? IsHindi { get; set; }
    public bool? IsMarathi { get; set; }
    public bool? IsSanskrit { get; set; }
    public bool? IsSchemeOfMarking { get; set; }
    public bool? IsModelAnswers { get; set; }

    [NotMapped]
    public double? RemunerationOthers { get; set; }
    [NotMapped]
    public double? SchemeOfMarking { get; set; }
    [NotMapped]
    public double? ModelAnswers { get; set; }

    [AdaptIgnore]
    public virtual Appointment Appointment { get; set; }

    #endregion

    #region -- Public Methods --
    public void Copy(AppointmentBillDetail other)
    {
        if (null == other) return;

        SerialNo = other.SerialNo;
        Code = other.Code;

        StaffId = other.StaffId;

        BillDate = other.BillDate;
        SetsDrawn = other.SetsDrawn;
        CourseFee = other.CourseFee;
        AllottedFee = other.AllottedFee;
        Ta = other.Ta;
        Da = other.Da;
        ChairmanAllowance = other.ChairmanAllowance;
        TranslationCharges = other.TranslationCharges;
        Travel = other.Travel;
        BasicPay = other.BasicPay;
        OtherAllowance = other.OtherAllowance;
        TotalFee = other.TotalFee;

        DateOfJourney = other.DateOfJourney;
        FromPlace = other.FromPlace;
        ToPlace = other.ToPlace;
        TotalDistance = other.TotalDistance;
        TravelingMode = other.TravelingMode;
        TimeIn = other.TimeIn;
        TimeOut = other.TimeOut;

        IsHindi = other.IsHindi;
        IsMarathi = other.IsMarathi;
        IsSanskrit = other.IsSanskrit;
        IsSchemeOfMarking = other.IsSchemeOfMarking;
        IsModelAnswers = other.IsModelAnswers;

        Status = other.Status;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;
        DeletedBy = other.DeletedBy;
    }
    #endregion
}