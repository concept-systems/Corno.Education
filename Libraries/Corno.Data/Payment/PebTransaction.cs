using System.Collections.Generic;

namespace Corno.Data.Payment;

public class PebTransaction
{
    public string peb_transaction_id { get; set; }
    public double amount { get; set; }
    public double peb_service_charge { get; set; }
    public double peb_service_tax { get; set; }
    public double peb_settlement_amount { get; set; }
    public string transaction_type { get; set; }
    public object submerchant_id { get; set; }
    public List<object> split_transactions { get; set; }
    public string realised_payment_date { get; set; }
    public string status { get; set; }
    public string txnid { get; set; }
    public string udf1 { get; set; }
    public string udf2 { get; set; }
    public string udf3 { get; set; }
    public string udf4 { get; set; }
    public string udf5 { get; set; }
}