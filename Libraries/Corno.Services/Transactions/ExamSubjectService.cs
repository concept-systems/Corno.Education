using OnlineExam.Models;

namespace Corno.Services
{
    public class ExamSubjectService : MasterService<ExamSubject>, IExamSubjectService
    {
        public ExamSubjectService(IUnitOfWork unitOfWork, IGenericRepository<ExamSubject> examSubjectRepository)
            : base(unitOfWork, examSubjectRepository)
        {
        }
    }
}
