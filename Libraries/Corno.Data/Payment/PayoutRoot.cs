using System.Collections.Generic;

namespace Corno.Data.Payment;

public class PayoutRoot
{
    public bool status { get; set; }
    public List<PayoutsHistoryDatum> payouts_history_data { get; set; }
}