using Corno.Data.Common;
using Mapster;
using System;

namespace Corno.Data.Corno;

[Serializable]
public class MarksEntryDetail : BaseModel
{
    public int? MarksEntryId { get; set; }

    public string Prn { get; set; }
    public long? SeatNo { get; set; }
    public double MaxMarks { get; set; }
    public string Marks { get; set; }

    [AdaptIgnore]
    public virtual MarksEntry MarkEntry { get; set; }
}