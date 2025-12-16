using OnlineExam.Models;

namespace Corno.Services
{
    public class InstanceService : MasterService<Instance>,IInstanceService
    {
        public InstanceService(IUnitOfWork unitOfWork, IGenericRepository<Instance> instanceRepository)
            : base(unitOfWork, instanceRepository)
        {
        }
    }
}
