using System;
using Corno.Data.Common;

namespace Corno.Data.Corno;

public class EnvironmentSetting : BaseModel
{
    public int? InstanceId { get; set; }
    public int? Year { get; set; }
    public int? Month { get; set; }

    public double? ExamFee { get; set; }
    public double? LateFee { get; set; }
    public double? SuperLateFee { get; set; }
    public double? OtherFee { get; set; }

    public DateTime? ExamDate { get; set; }
    public DateTime? WithoutLateFeeDate { get; set; }
    public DateTime? LateFeeDate { get; set; }
    public DateTime? SuperLateFeeDate { get; set; }
}