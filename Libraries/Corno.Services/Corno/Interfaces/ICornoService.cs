using Corno.Data.Core;
using Corno.Data.Corno;
using Corno.Data.Corno.Question_Bank;
using Corno.Data.Corno.Question_Bank.Models;
using Corno.Data.Payment;

namespace Corno.Services.Corno.Interfaces;

public interface ICornoService : IBaseService
{
    IGenericRepository<Registration> StudentRepository { get; }
    IGenericRepository<Exam> ExamRepository { get; }
    IGenericRepository<Convocation> ConvocationRepository { get; }
    IGenericRepository<Revalution> RevaluationRepository { get; }
    IGenericRepository<RevalutionSubject> RevaluationSubjectRepository { get; }
    IGenericRepository<ExamSubject> ExamSubjectRepository { get; }
    IGenericRepository<EnvironmentStudy> EnvironmentStudyRepository { get; }
    IGenericRepository<EnvironmentSetting> EnvironmentSettingRepository { get; }
    IGenericRepository<ConvocationFee> ConvocationFeeRepository { get; }

    IGenericRepository<TimeTable> TimeTableRepository { get; }
    IGenericRepository<TimeTableTheoryDetail> TimeTableTheoryDetailRepository { get; }
    IGenericRepository<AnswerSheet> AnswerSheetRepository { get; }

    IGenericRepository<TransactionOtp> OtpRepository { get; }

    // Enrollment
    IGenericRepository<Link> LinkRepository { get; }
    IGenericRepository<LinkDetail> LinkDetailRepository { get; }

    // Paper Setting & Question Bank
    /*IGenericRepository<Appointment> AppointmentRepository { get; }
    IGenericRepository<AppointmentDetail> AppointmentDetailRepository { get; }
    IGenericRepository<Schedule> ScheduleRepository { get; }
    IGenericRepository<ScheduleDetail> ScheduleDetailRepository { get; }*/

    // Question Bank
    IGenericRepository<Structure> StructureRepository { get; }
    IGenericRepository<StructureDetail> StructureDetailRepository { get; }
    /*IGenericRepository<Question> QuestionRepository { get; }*/
    IGenericRepository<Paper> PaperRepository { get; }
    IGenericRepository<PaperDetail> PaperDetailRepository { get; }

    // Payment Gateway
    IGenericRepository<Payout> PayoutRepository { get; }
}