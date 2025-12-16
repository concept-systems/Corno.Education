using Corno.Data.Common;
using Corno.Data.Helpers;
using Corno.Globals.Constants;
using Mapster;
using System.Web;

namespace Corno.Data.Corno;

public class ExamSubjectCommon : BaseModel
{
    public ExamSubjectCommon()
    {
        InstanceId = HttpContext.Current.Session[ModelConstants.InstanceId].ToInt();
    }
    public int InstanceId { get; set; }
    public int? ExamId { get; set; }
    public int? SubjectCode { get; set; }
    public string SubjectType { get; set; }
    public int? CoursePartId { get; set; }
}
public class ExamSubject : ExamSubjectCommon
{
    [AdaptIgnore]
    public virtual Exam Exam { get; set; }
    //public virtual ICollection<Exam> Exams { get; set; }
}

public class ExamSubjectViewModel : ExamSubjectCommon
{
    public ExamSubjectViewModel()
    {
        RowSelector = false;
        OptionalIndex = 0;
        MaxOptionalCount = 0;
    }

    public bool RowSelector { get; set; }
    public bool IsSelected { get; set; }
    public string CoursePartName { get; set; }
    public string SubjectName { get; set; }
    public int OptionalIndex { get; set; }
    public int MaxOptionalCount { get; set; }

    //public virtual ICollection<Exam> Exams { get; set; }
    //public virtual ICollection<Revalution> Revalutions { get; set; }
}