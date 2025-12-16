using System.Collections.Generic;

namespace Corno.Data.Corno;

public sealed class Exam : ExamCommon
{
    public Exam()
    {
        ExamSubjects = new List<ExamSubject>();
        //this.Tbl_SUBJECT_MSTRs = new List<Tbl_SUBJECT_MSTR>();
    }
    // public virtual ICollection<Tbl_SUBJECT_MSTR> Tbl_SUBJECT_MSTRs { get; set; }
    public ICollection<ExamSubject> ExamSubjects { get; set; }
    //public virtual College College { get; set; }
    //public virtual Course Course { get; set; }
    public Registration Student { get; set; }
    //public virtual CoursePart CoursePart { get; set; }
}