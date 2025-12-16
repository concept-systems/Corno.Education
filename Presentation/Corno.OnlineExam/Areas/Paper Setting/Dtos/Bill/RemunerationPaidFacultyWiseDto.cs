namespace Corno.OnlineExam.Areas.Paper_Setting.Dtos.Bill;

public class RemunerationPaidFacultyWiseDto
{
    public int FacultyId { get; set; }
    public string FacultyName { get; set; }
    public int SubjectId { get; set; }
    public int SetsDrawn { get; set; }
    public double Ta { get; set; }
    public double Da { get; set; }
    public double CourseFee { get; set; }
    public double ChairmanAllowance { get; set; }
    public double TotalFee { get; set; }
}