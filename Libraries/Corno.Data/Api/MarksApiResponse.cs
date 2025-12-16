using System.Collections.Generic;
using Newtonsoft.Json;

namespace Corno.Data.Api;

public class MarksApiResponse
{
    [JsonProperty("responseCode")]
    public string ResponseCode { get; set; }
    
    [JsonProperty("data")]
    public List<MarksApiDataItem> Data { get; set; }
    
    [JsonProperty("message")]
    public string Message { get; set; }
    
    [JsonProperty("numberOfRecords")]
    public int? NumberOfRecords { get; set; }
}

public class MarksApiDataItem
{
    [JsonProperty("collegeCode")]
    public string CollegeId { get; set; }
    [JsonProperty("centerCode")]
    public string CenterId { get; set; }
    [JsonProperty("courseNo")]
    public int CourseId { get; set; }
    [JsonProperty("coursePart")]
    public string CoursePartId { get; set; }
    [JsonProperty("subjectCode")]
    public string SubjectCode { get; set; }
    [JsonProperty("seatNo")]
    public long? SeatNo { get; set; }
    
    [JsonProperty("marks")]
    public string Marks { get; set; }
    
    [JsonProperty("prn")]
    public string Prn { get; set; }
    
    [JsonProperty("status")]
    public string Status { get; set; }
}

