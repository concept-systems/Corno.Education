using Corno.Data.Common;
using Corno.Globals.Constants;
using Mapster;
using System.Web;

namespace Corno.Data.Corno;

public class RevalutionSubjectCommon : BaseModel
{
    public RevalutionSubjectCommon()
    {
        InstanceId = (int)HttpContext.Current.Session[ModelConstants.InstanceId];

        IsRevaluation = false;
        IsVerification = false;
        RevaluationFee = 0;
        VerficationFee = 0;
        TotalFee = 0;
    }
    public int InstanceId { get; set; }
    public int? CoursePartId { get; set; }
    public int? RevalutionId { get; set; }
    public int? SubjectCode { get; set; }
    public int? CategoryCode { get; set; }
    public bool IsRevaluation { get; set; }
    public bool IsVerification { get; set; }

    public double? RevaluationFee { get; set; }
    public double? VerficationFee { get; set; }
    public double? TotalFee { get; set; }
}
public class RevalutionSubject : RevalutionSubjectCommon
{
    [AdaptIgnore]
    public virtual Revalution Revalution { get; set; }
    //public virtual ICollection<Exam> Exams { get; set; }
}

public class RevalutionSubjectViewModel : RevalutionSubjectCommon
{
    public RevalutionSubjectViewModel()
    {
        IsRevaluation = false;
        IsVerification = false;
    }

    public string CoursePartName { get; set; }
    public string SubjectName { get; set; }
    public string SubjectCategory { get; set; }
    public int MaxOptionalCount { get; set; }
    public bool ShowRevaluation { get; set; }
    public bool ShowVerification { get; set; }
}