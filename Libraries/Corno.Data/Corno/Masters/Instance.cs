using System;
using Corno.Data.Common;

namespace Corno.Data.Corno.Masters;

[Serializable]
public class Instance : MasterModel
{
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
}