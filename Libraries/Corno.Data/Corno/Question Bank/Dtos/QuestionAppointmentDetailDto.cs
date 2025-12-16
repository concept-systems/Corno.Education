using System;

namespace Corno.Data.Corno.Question_Bank.Dtos;

[Serializable]
public class QuestionAppointmentDetailDto
{
    public int Id { get; set; }
    public int? QuestionAppointmentId { get; set; }

    public int? StaffId { get; set; }
    public string StaffName { get; set; }
    public bool IsSetter { get; set; }
    public bool? IsChecker { get; set; }
    public string EmailId { get; set; }
    public string  MobileNo { get; set; }
    public int? EmailCount { get; set; }
    public int? SmsCount { get; set; }
}