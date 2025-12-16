using System;
using Corno.Data.Helpers;

namespace Corno.Data.Payment;

public class Payout
{
    #region -- Constructors --
    // Keep for database transactions
    public Payout()
    {
    }

    public Payout(PebTransaction pebTransaction, DateTime? settlementDate)
    {
        InstanceId = pebTransaction.udf4.ToInt();
        CollegeId = pebTransaction.udf2.ToInt();
        CourseId = pebTransaction.udf3.ToInt();
        Prn = pebTransaction.udf1;
        TransactionId = pebTransaction.txnid;
        EazeBuzzTransactionId = pebTransaction.peb_transaction_id;
        Amount = pebTransaction.amount;
        SettlementAmount = pebTransaction.peb_settlement_amount;
        ServiceCharge = pebTransaction.peb_service_charge;
        ServiceTax = pebTransaction.peb_service_tax;
        TransactionType = pebTransaction.transaction_type;
        PaymentDate = !string.IsNullOrEmpty(pebTransaction.realised_payment_date) ? pebTransaction.realised_payment_date?.ToDateTime() : null;
        SettlementDate = settlementDate;
        FormType = pebTransaction.udf5;
        Status = pebTransaction.status;
    }
    #endregion

    #region -- Properties --
    public int CompanyId { get; set; }
    public int SerialNo { get; set; }
    public string Code { get; set; }
    public int Id { get; set; }
    public int? InstanceId { get; set; }
    public int? CentreId { get; set; }
    public int? CollegeId { get; set; }
    public int? CourseTypeId { get; set; }
    public int? CourseId { get; set; }
    public int? CoursePartId { get; set; }
    public int? BranchId { get; set; }
    public string Prn { get; set; }
    public string TransactionId { get; set; }
    public string EazeBuzzTransactionId { get; set; }
    public double? Amount { get; set; }
    public double? SettlementAmount { get; set; }
    public double? ServiceCharge { get; set; }
    public double? ServiceTax { get; set; }
    public string TransactionType { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime? SettlementDate { get; set; }
    public string FormType { get; set; }
    public string Status { get; set; }
    public string CreatedBy { get; set; }
    public DateTime? CreatedDate { get; set; }
    public string ModifiedBy { get; set; }
    public DateTime? ModifiedDate { get; set; }
    public string DeletedBy { get; set; }
    public DateTime? DeletedDate { get; set; }
    #endregion
}