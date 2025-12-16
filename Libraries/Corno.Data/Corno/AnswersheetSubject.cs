using Corno.Data.Common;
using Corno.Globals.Constants;
using Mapster;
using System.ComponentModel.DataAnnotations.Schema;
using System.Web;

namespace Corno.Data.Corno;

public class AnswerSheetSubject : BaseModel
{
    public AnswerSheetSubject()
    {
        InstanceId = (int)HttpContext.Current.Session[ModelConstants.InstanceId];
        IsSelected = false;
        Fee = 0;
    }
    public int InstanceId { get; set; }
    public int? AnswerSheetId { get; set; }
    public int? CoursePartId { get; set; }
    public int? SubjectId { get; set; }
    public int? CategoryCode { get; set; }
    public double? Fee { get; set; }

    [NotMapped]
    public string CoursePartName { get; set; }
    [NotMapped]
    public string SubjectName { get; set; }
    [NotMapped]
    public string CategoryName { get; set; }
    [NotMapped]
    public bool IsSelected { get; set; }
    [NotMapped]
    public bool ShowIsSelected { get; set; }

    [AdaptIgnore]
    public virtual AnswerSheet AnswerSheet { get; set; }
}