using OnlineExam.Models;

namespace Corno.Services
{
    public class EnvironmentStudyService : MasterService<EnvironmentStudy>, IEnvironmentStudyService
    {
        public EnvironmentStudyService(IUnitOfWork unitOfWork, IGenericRepository<EnvironmentStudy> environmentStudyRepository)
            : base(unitOfWork, environmentStudyRepository)
        {
        }
    }
}
