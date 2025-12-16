using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using Corno.Data.Common;

namespace Corno.Data.Corno;

public class AnswerSheet : UniversityBaseModel
{
    public AnswerSheet()
    {
        AnswerSheetSubjects = new List<AnswerSheetSubject>();
    }

    public string Prn { get; set; }
    public long? SeatNo { get; set; }

    //[StringLength(10, ErrorMessage = "The Mobile must contains 10 characters", MinimumLength = 10)]
    public string MobileNo { get; set; }
    //[RegularExpression(@"^([a-zA-Z0-9_\.\-])+\@(([a-zA-Z0-9\-])+\.)+([a-zA-Z0-9]{2,4})+$",
    //ErrorMessage = "Please Enter Correct Email Address")]
    public string EmailId { get; set; }
    public double? TotalFee { get; set; }

    [NotMapped]
    public string CollegeName { get; set; }
    [NotMapped]
    public string CourseName { get; set; }
    [NotMapped]
    public string CoursePartName { get; set; }
    [NotMapped]
    public string StudentName { get; set; }
    [NotMapped]
    public string CentreName { get; set; }
    [NotMapped]
    public string CourseTypeName { get; set; }
    [NotMapped]
    public string BranchName { get; set; }
    [NotMapped]
    public string Photo { get; set; }

    public ICollection<AnswerSheetSubject> AnswerSheetSubjects { get; set; }
}