using System.Collections.Generic;

namespace Corno.Data.ViewModels;

public class MarksApiViewModel
{
    public int? InstanceId { get; set; }
    public int? FacultyId { get; set; }
    public int? CollegeId { get; set; }
    public int? CourseTypeId { get; set; }
    public int? CourseId { get; set; }
    public int? CoursePartId { get; set; }
    public int? SubjectId { get; set; }
    public int? CategoryId { get; set; }
    public int? PaperId { get; set; }
    public int? PageSize { get; set; }

    public string InstanceName { get; set; }
    public string SubjectName { get; set; }
    
    // For displaying imported data
    public List<MarksApiDetailDto> ImportedMarks { get; set; }
    
    public MarksApiViewModel()
    {
        ImportedMarks = new List<MarksApiDetailDto>();
        PageSize = 120; // Default page size
    }
}

public class MarksApiDetailDto
{
    public long SeatNo { get; set; }
    public string Marks { get; set; }
    public string Status { get; set; }
    public string Prn { get; set; }
}

public class MarksApiDto
{
    public int TotalRecordsFetched { get; set; }
    public int TotalRecordsInserted { get; set; }
    public int TotalRecordsUpdated { get; set; }
    public List<MarksApiDetailDto> ImportedMarks { get; set; }
    public string Message { get; set; }
    
    public MarksApiDto()
    {
        ImportedMarks = new List<MarksApiDetailDto>();
    }
}