using System.Collections.Generic;

namespace Corno.Data.Api;

public class ApiResponse
{
    public string ResponseCode { get; set; }
    public List<DataItem> Data { get; set; }
    public string Message { get; set; }
    public string NumberOfRecords { get; set; }
}