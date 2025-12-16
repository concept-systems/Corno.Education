using System;
using Corno.Data.Common;

namespace Corno.Data.Corno.Masters;

[Serializable]
public class Faculty : MasterModel
{
    public int? StaffId { get; set; }

    public string Address { get; set; }
    public string Dean { get; set; }
    public string Mobile { get; set; }
    public string Email { get; set; }
    public DateTime? RecognitionDate { get; set; }
}