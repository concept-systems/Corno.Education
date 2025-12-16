using Corno.Data.Common;
using Corno.Data.ViewModels;
using Mapster;
using System;
using System.ComponentModel.DataAnnotations.Schema;
using System.Runtime.Serialization;
using System.Text.Json.Serialization;

namespace Corno.Data.Corno;

/*[Serializable]*/
public class LinkDetail : BaseModel
{
    #region -- Constructors --
    public LinkDetail()
    {
        NotMapped = new NotMapped();
    }
    #endregion

    #region -- Properties --
    public int? LinkId { get; set; }
    public int? InstanceId { get; set; }
    public int? BranchId { get; set; }

    public string Prn { get; set; }
    public string SeatNo { get; set; }
    public string Mobile { get; set; }
    public string EmailId { get; set; }
    public DateTime? SentDate { get; set; }
    public string SmsResponse { get; set; }
    public string EmailResponse { get; set; }
    public string TransactionId { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime? SettlementDate { get; set; }
    public double? PaidAmount { get; set; }
    public string Remarks { get; set; }

    [NotMapped]
    public bool Selected { get; set; }
    [NotMapped]
    public NotMapped NotMapped { get; set; }
    [NotMapped]
    public MasterViewModel BranchViewModel { get; set; }

    [AdaptIgnore]
    protected virtual Link Link { get; set; }

    #endregion

    #region -- Public Methods --
    public void Copy(LinkDetail other)
    {
        if (null == other) return;

        SerialNo = other.SerialNo;
        Code = other.Code;

        InstanceId = other.InstanceId;
        BranchId = other.BranchId;
            
        Prn = other.Prn;
        Mobile = other.Mobile;
        EmailId = other.EmailId;
        SentDate = other.SentDate;
        Status = other.Status;

        SmsResponse = other.SmsResponse;
        EmailResponse = other.EmailResponse;
        Remarks = other.Remarks;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;
    }
    #endregion
}