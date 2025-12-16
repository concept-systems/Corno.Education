using System.Collections.Generic;

namespace Corno.OnlineExam.Areas.Question_Bank.Dtos;

public class AnalysisDto
{
    public int Id { get; set; }

    public List<AnalysisDifficultyDto> DifficultyDtos { get; set; } = new();
    public List<AnalysisTaxonomyDto> TaxonomyDtos { get; set; } = new();
    public List<AnalysisChapterDto> ChapterDtos { get; set; } = new();
}

public class AnalysisDifficultyDto
{
    public int DifficultyLevelId { get; set; }
    public string DifficultyLevelName { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class AnalysisTaxonomyDto
{
    public int SerialNo { get; set; }
    public string TaxonomyName { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}

public class AnalysisChapterDto
{
    public int SerialNo { get; set; }
    public string ChapterName { get; set; } = string.Empty;
    public int Count { get; set; }
    public double Percentage { get; set; }
}