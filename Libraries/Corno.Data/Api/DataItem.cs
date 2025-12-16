using Newtonsoft.Json;

namespace Corno.Data.Api;

public class DataItem
{
    public string RollNo { get; set; }
    [JsonProperty("Part 1 Marks")]
    public double Part1Marks { get; set; }

    [JsonProperty("Part 2 Marks")]
    public double Part2Marks { get; set; }
    public double TotalScore { get; set; }
}