using Corno.Data.Common;
using Mapster;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corno.Data.Corno;

[Serializable]
public class TimeTablePracticalDetail : BaseModel
{
    public int? TimeTableId { get; set; }
    public int? SubjectId { get; set; }
    public string SubjectGroup { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }
    public int? CategoryCode { get; set; }
    public int? PaperCode { get; set; }
    public int? SubjectDivisionCode { get; set; }
    public string SubjectType { get; set; }

    [NotMapped]
    public string CoursePartName { get; set; }
    [NotMapped]
    public string SubjectName { get; set; }
    [NotMapped]
    public string CategoryName { get; set; }

    [AdaptIgnore]
    public virtual TimeTable TimeTable { get; set; }
}