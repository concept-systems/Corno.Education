using System;
using System.Collections.Generic;
using System.Web;
using Corno.Data.Corno;

namespace Corno.Data.ViewModels;

public sealed class ExamViewModel : ExamCommon
{
    public ExamViewModel()
    {
        ExamSubjectViewModels = new List<ExamSubjectViewModel>();
        ShowBranchCombo = false;
        DegreeApplicable = false;

    }
    public HttpPostedFileBase UploadPhoto { get; set; }
    public string CollegeName { get; set; }
    public string CourseName { get; set; }
    public string StudentName { get; set; }
    public string CentreName { get; set; }
    public string CourseTypeName { get; set; }
    public string CoursePartName { get; set; }
    public string BranchName { get; set; }
    public bool DegreeApplicable { get; set; }

    public bool ShowBranchCombo { get; set; }
    public string ValidFlag { get; set; }

    public DateTime? LateFeeDate { get; set; }
    public DateTime? SuperLateFeeDate { get; set; }

    public string Mobile { get; set; }
    public string Email { get; set; }

    public int? FeeId { get; set; }

    public ICollection<ExamSubjectViewModel> ExamSubjectViewModels { get; set; }
}