using System;
using System.Collections.Generic;

namespace Corno.Data.Payment;

public class PayoutsHistoryDatum
{
    public bool paid_out { get; set; }
    public DateTime payout_actual_date { get; set; }
    public double payout_amount { get; set; }
    public string payout_id { get; set; }
    public List<object> peb_refunds { get; set; }
    public string bank_name { get; set; }
    public string account_number { get; set; }
    public double service_charge_amount { get; set; }
    public string bank_transaction_id { get; set; }
    public List<PebTransaction> peb_transactions { get; set; }
    public List<object> split_payouts { get; set; }
}