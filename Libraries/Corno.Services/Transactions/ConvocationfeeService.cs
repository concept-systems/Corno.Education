using OnlineExam.Models;

namespace Corno.Services
{
    public class ConvocationfeeService : MasterService<ConvocationFee>,IConvocationfeeService
    {
        public ConvocationfeeService(IUnitOfWork unitOfWork, IGenericRepository<ConvocationFee> convocationfeeRepository)
            : base(unitOfWork, convocationfeeRepository)
        {
        }
    }
}
