using Ganss.Excel;

namespace Corno.Data.Common;

public class MiscMaster : MasterModel
{
    public string MiscType { get; set; }
    [Ignore]
    public byte[] Photo { get; set; }
}