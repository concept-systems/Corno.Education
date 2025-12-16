using System;
using System.Collections.Generic;
using Corno.Data.Common;

namespace Corno.Data.Corno;

[Serializable]
public class TimeTable : UniversityBaseModel
{
    public TimeTable()
    {
        TimeTableTheoryDetails = new List<TimeTableTheoryDetail>();
        TimeTablePracticalDetails = new List<TimeTablePracticalDetail>();
        TimeTableCoursePartDetails = new List<TimeTableCoursePartDetail>();
    }
        
    public ICollection<TimeTableTheoryDetail> TimeTableTheoryDetails { get; set; }
    public ICollection<TimeTablePracticalDetail> TimeTablePracticalDetails { get; set; }
    public ICollection<TimeTableCoursePartDetail> TimeTableCoursePartDetails { get; set; }
}

public class TimeTableViewModel : UniversityBaseModel
{
    public int? CenterId { get; set; }
    public string InstanceName { get; set; }
    public string CollegeName { get; set; }
    public string CenterName { get; set; }
    public string CourseName { get; set; }
    public string CoursePartName { get; set; }
    public string BranchName { get; set; }
    public DateTime? ExamStartDate { get; set; }
    public DateTime? ExamEndDate { get; set; }
    public double? ExamFee { get; set; }
    public double? StatementMarksAndCapFee { get; set; }
}