using Corno.Data.Corno.Question_Bank;
using Corno.Data.Corno.Question_Bank.Dtos;
using Corno.Data.Corno.Question_Bank.Models;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Corno.Question_Bank.Interfaces;

public interface IQuestionAppointmentService : IMainService<QuestionAppointment>
{
    void ValidateDto(QuestionAppointmentDto dto);
    QuestionAppointment GetExisting(QuestionAppointmentDto dto);
    void GetStaffs(QuestionAppointmentDto dto);
    void UpdateCompletedCount(Question question, int instanceId);
    void SendEmailSms(QuestionAppointmentDto dto, string type);
}