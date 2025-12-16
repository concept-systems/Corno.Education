using System;

namespace Corno.OnlineExam.Areas.Transactions.Dtos;

public class AddlCreditsSubjectDto
{
    public int SerialNo { get; set; }
    public int SubjectId { get; set; }
    public string SubjectName { get; set; }
    public double MaximumCredits { get; set; }
    public DateTime? CompletedDate { get; set; }
    public bool IsCompleted { get; set; }
}