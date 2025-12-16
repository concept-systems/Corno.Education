using OnlineExam.Models;

namespace Corno.Services
{
    public class RevaluationSubjectService : MasterService<RevalutionSubject>, IRevaluationSubjectService
    {
        public RevaluationSubjectService(IUnitOfWork unitOfWork, IGenericRepository<RevalutionSubject> revalutionSubjectRepository)
            : base(unitOfWork, revalutionSubjectRepository)
        {
        }
    }
}
