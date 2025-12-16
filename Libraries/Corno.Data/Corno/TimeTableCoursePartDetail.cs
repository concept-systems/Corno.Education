using Corno.Data.Common;
using Mapster;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corno.Data.Corno;

[Serializable]
public class TimeTableCoursePartDetail : BaseModel
{
    public int? TimeTableId { get; set; }
    public int? CoursePartId { get; set; }
    public string SubjectGroup { get; set; }
    public DateTime? StartDate { get; set; }
    public DateTime? EndDate { get; set; }
    public DateTime? StartTime { get; set; }
    public DateTime? EndTime { get; set; }

    [NotMapped]
    public string CoursePartName { get; set; }

    [AdaptIgnore]
    public virtual TimeTable TimeTable { get; set; }
}