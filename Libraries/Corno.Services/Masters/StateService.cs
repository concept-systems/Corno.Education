using OnlineExam.Models;

namespace Corno.Services
{
    public class StateService : MasterService<State>,IStateService
    {
        public StateService(IUnitOfWork unitOfWork, IGenericRepository<State> stateRepository)
            : base(unitOfWork, stateRepository)
        {
        }
    }
}
