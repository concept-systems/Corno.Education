using OnlineExam.Models;

namespace Corno.Services
{
    public class RevaluationService : MasterService<Revalution>, IRevaluationService
    {
        public RevaluationService(IUnitOfWork unitOfWork, IGenericRepository<Revalution> revaluationRepository)
            : base(unitOfWork, revaluationRepository)
        {
        }
    }
}
