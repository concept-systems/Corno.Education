using System;
using Corno.Data.Common;

namespace Corno.Data.Corno;

public class TransactionOtp : BaseModel
{
    public string Transaction { get; set; }
    public string PrnNo { get; set; }
    public string MobileNo { get; set; }
    public string Otp { get; set; }
    public DateTime? SendTime { get; set; }
    public DateTime? ExpiryTime { get; set; }
}