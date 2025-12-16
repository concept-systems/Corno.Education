using System.Collections.Generic;
using System.Linq;

namespace Corno.Data.Corno.Online_Education;

public class FeeInfo
{
    #region -- Constructors --
    public int FeeId { get; set; }
    public bool RegularFee { get; set; }
    public bool BacklogFee { get; set; }
    #endregion

    #region -- Public Methods --

    public static FeeInfo GetFeeInfo(int feeId)
    {
        var fees = new List<FeeInfo>
        {
            new() { FeeId = 1, RegularFee = false, BacklogFee = true },
            new() { FeeId = 2, RegularFee = true, BacklogFee = true },
            new() { FeeId = 3, RegularFee = false, BacklogFee = false },
            new() { FeeId = 4, RegularFee = true, BacklogFee = false }
        };

        return fees.FirstOrDefault(p => p.FeeId == feeId);
    }
    #endregion
}