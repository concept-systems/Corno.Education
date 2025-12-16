using System;

namespace Corno.Data.Core;

public partial class TBL_DISTANCE_CENTERS
{
    public short Num_PK_DistCenter_ID { get; set; }
    public string DIST_CENT_NAME { get; set; }
    public string Chr_USR_NM { get; set; }
    public DateTime? Dtm_DTE_CR { get; set; }
    public DateTime? Dtm_DTE_UP { get; set; }
}