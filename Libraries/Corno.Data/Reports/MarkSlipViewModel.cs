using Corno.Data.Common;

namespace Corno.Data.Reports;

public class MarkSlipViewModel : UniversityBaseModel
{
    public MarkSlipViewModel()
    {
        BranchId = 0;
        SubjectId = 0;
        CategoryId = 0;
        PaperNo = 0;
        SectionNo = 0;
    }
        
    public int? SubjectId { get; set; }
    public int? CategoryId { get; set; }
    public int? PaperNo { get; set; }
    public int? SectionNo { get; set; }
    public bool IsMedical { get; set; }

    public string PrnNo { get; set; }
}