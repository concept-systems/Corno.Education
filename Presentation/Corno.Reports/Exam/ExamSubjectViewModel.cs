namespace Corno.Reports.Exam;

public class ExamSubjectViewModel
{
    public int InstanceId { get; set; }
    public string InstanceName { get; set; }
    public int CollegeId { get; set; }
    public string CollegeName { get; set; }
    public int CenterId { get; set; }
    public string CenterName { get; set; }
    public int CourseId { get; set; }
    public string CourseName { get; set; }
    public int CoursePartId { get; set; }
    public string CoursePartName { get; set; }
    public int BranchId { get; set; }
    public string BranchName { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; }
    public string SeatNos { get; set; }
    public string SeatNosTheory { get; set; }
    public string SeatNosPractical { get; set; }
    public string SeatNosIa { get; set; }
    public int M { get; set; }
    public int F { get; set; }
    public int Total { get; set; }


    // Extra Fields
    public long SeatNo { get; set; }
    public string Prn { get; set; }
    public string Gender { get; set; }
    public int CategoryCode { get; set; }
    public string CategoryName { get; set; }
    public int PaperCode { get; set; }
    public string PaperName { get; set; }
    public int MTheory { get; set; }
    public int FTheory { get; set; }
    public int MPractical { get; set; }
    public int FPractical { get; set; }
    public int MIa { get; set; }
    public int FIa { get; set; }
}