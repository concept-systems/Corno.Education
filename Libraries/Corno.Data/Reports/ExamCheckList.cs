using Corno.Data.Common;

namespace Corno.Data.Reports;

public class ExamCheckListViewModel : UniversityBaseModel
{
    //[Required]
    public string Bundle { get; set; }

    public string PrnNo { get; set; }
}