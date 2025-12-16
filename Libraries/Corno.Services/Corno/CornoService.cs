using Corno.Data.Core;
using Corno.Data.Corno;
using Corno.Data.Corno.Question_Bank;
using Corno.Data.Corno.Question_Bank.Models;
using Corno.Data.Payment;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Corno;

public class CornoService : BaseService, ICornoService
{
    #region -- Constructors --
    public CornoService(IUnitOfWork unitOfWork)
    {
        UnitOfWork = unitOfWork;
    }
    #endregion

    #region -- Data Members --
    // Email & SMS
    private IGenericRepository<TransactionOtp> _otpRepository;

    // Enrollment
    private IGenericRepository<Link> _linkRepository;
    private IGenericRepository<LinkDetail> _linkDetailRepository;

    // Registration
    private IGenericRepository<Registration> _studentRepository;
        
    // Environment Studies
    private IGenericRepository<EnvironmentStudy> _environmentStudyRepository;
    private IGenericRepository<EnvironmentSetting> _environmentSettingRepository;

    // Exam
    private IGenericRepository<ExamSubject> _examSubjectRepository;
    private IGenericRepository<Exam> _studentExamRepository;

    // Revaluation
    private IGenericRepository<Revalution> _revaluationRepository;
    private IGenericRepository<RevalutionSubject> _revaluationSubjectRepository;

    // Time Table
    private IGenericRepository<TimeTable> _timeTableRepository;
    private IGenericRepository<TimeTableTheoryDetail> _timeTableTheoryDetailRepository;
        
    // Answer sheet
    private IGenericRepository<AnswerSheet> _answerSheetRepository;

    // Convocation
    private IGenericRepository<ConvocationFee> _convocationFeeRepository;
    private IGenericRepository<Convocation> _convocationRepository;

    // Paper Setting
    /*private IGenericRepository<Appointment> _appointmentRepository;
    private IGenericRepository<AppointmentDetail> _appointmentDetailRepository;
    private IGenericRepository<Schedule> _scheduleRepository;
    private IGenericRepository<ScheduleDetail> _scheduleDetailRepository;*/

    // Question Bank
    private IGenericRepository<Structure> _structureRepository;
    private IGenericRepository<StructureDetail> _structureDetailRepository;
    //private IGenericRepository<Question> _questionRepository;
    private IGenericRepository<Paper> _paperRepository;
    private IGenericRepository<PaperDetail> _paperDetailRepository;

    // Payment Gateway
    private IGenericRepository<Payout> _payoutRepository;

    #endregion

    #region -- Properties --
    // Email & SMS
    public IGenericRepository<TransactionOtp> OtpRepository => _otpRepository ??= new GenericRepository<TransactionOtp>(UnitOfWork);

    // Enrollment
    public IGenericRepository<Link> LinkRepository => _linkRepository ??= new GenericRepository<Link>(UnitOfWork, nameof(Link.LinkDetails));
    public IGenericRepository<LinkDetail> LinkDetailRepository => _linkDetailRepository ??= new GenericRepository<LinkDetail>(UnitOfWork);

    // Registration
    public IGenericRepository<Registration> StudentRepository => _studentRepository ??= new GenericRepository<Registration>(UnitOfWork);

    // Environment Studies
    public IGenericRepository<EnvironmentStudy> EnvironmentStudyRepository => _environmentStudyRepository ??= new GenericRepository<EnvironmentStudy>(UnitOfWork);
    public IGenericRepository<EnvironmentSetting> EnvironmentSettingRepository => _environmentSettingRepository ??= new GenericRepository<EnvironmentSetting>(UnitOfWork);

    // Exam
    public IGenericRepository<Exam> ExamRepository => _studentExamRepository ??= new GenericRepository<Exam>(UnitOfWork, nameof(Exam.ExamSubjects));
    public IGenericRepository<ExamSubject> ExamSubjectRepository => _examSubjectRepository ??= new GenericRepository<ExamSubject>(UnitOfWork);

    // Revaluation
    public IGenericRepository<Revalution> RevaluationRepository => _revaluationRepository ??= new GenericRepository<Revalution>(UnitOfWork);
    public IGenericRepository<RevalutionSubject> RevaluationSubjectRepository => _revaluationSubjectRepository ??= new GenericRepository<RevalutionSubject>(UnitOfWork);

    // Time Table
    public IGenericRepository<TimeTable> TimeTableRepository => _timeTableRepository ??= new GenericRepository<TimeTable>(UnitOfWork);
    public IGenericRepository<TimeTableTheoryDetail> TimeTableTheoryDetailRepository => _timeTableTheoryDetailRepository ??= new GenericRepository<TimeTableTheoryDetail>(UnitOfWork);

    // Answer sheet
    public IGenericRepository<AnswerSheet> AnswerSheetRepository => _answerSheetRepository ??= new GenericRepository<AnswerSheet>(UnitOfWork);

    // Convocation
    public IGenericRepository<Convocation> ConvocationRepository => _convocationRepository ??= new GenericRepository<Convocation>(UnitOfWork);
    public IGenericRepository<ConvocationFee> ConvocationFeeRepository => _convocationFeeRepository ??= new GenericRepository<ConvocationFee>(UnitOfWork);

    // Paper Setting
    /*public IGenericRepository<Appointment> AppointmentRepository => _appointmentRepository ??= new GenericRepository<Appointment>(UnitOfWork, nameof(Appointment.AppointmentDetails));
    public IGenericRepository<AppointmentDetail> AppointmentDetailRepository => _appointmentDetailRepository ??= new GenericRepository<AppointmentDetail>(UnitOfWork);
    public IGenericRepository<Schedule> ScheduleRepository => _scheduleRepository ??= new GenericRepository<Schedule>(UnitOfWork, nameof(Schedule.ScheduleDetails));
    public IGenericRepository<ScheduleDetail> ScheduleDetailRepository => _scheduleDetailRepository ??= new GenericRepository<ScheduleDetail>(UnitOfWork);*/

    // Question Bank
    public IGenericRepository<Structure> StructureRepository => _structureRepository ??= new GenericRepository<Structure>(UnitOfWork, nameof(Structure.StructureDetails));
    public IGenericRepository<StructureDetail> StructureDetailRepository => _structureDetailRepository ??= new GenericRepository<StructureDetail>(UnitOfWork);
    //public IGenericRepository<Question> QuestionRepository => _questionRepository ??= new GenericRepository<Question>(UnitOfWork);
    public IGenericRepository<Paper> PaperRepository => _paperRepository ??= new GenericRepository<Paper>(UnitOfWork, nameof(Paper.PaperDetails));
    public IGenericRepository<PaperDetail> PaperDetailRepository => _paperDetailRepository ??= new GenericRepository<PaperDetail>(UnitOfWork);

    // Payment gateway
    public IGenericRepository<Payout> PayoutRepository => _payoutRepository ??= new GenericRepository<Payout>(UnitOfWork);

    #endregion
}