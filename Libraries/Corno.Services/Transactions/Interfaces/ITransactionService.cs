using Corno.Models;

namespace Corno.DAL.Classes
{
    public interface ITransactionService : IBaseService
    {
        IGenericRepository<Student> StudentRepository { get; }
        IGenericRepository<Exam> ExamRepository { get; }
        IGenericRepository<ExamFee> ExamFeeRepository { get; }
        IGenericRepository<Fee> FeeRepository { get; }
        IGenericRepository<Convocation> ConvocationRepository { get; }
        IGenericRepository<Revalution> RevalutionRepository { get; }
        IGenericRepository<RevalutionSubject> RevalutionSubjectRepository { get; }

        IGenericRepository<ExamSubject> ExamSubjectRepository { get; }
        IGenericRepository<EnvironmentStudy> EnvironmentStudyRepository { get; }
        IGenericRepository<ConvocationFee> ConvocationFeeRepository { get; }
    }
}