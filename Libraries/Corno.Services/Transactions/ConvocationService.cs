using OnlineExam.Models;

namespace Corno.Services
{
    public class ConvocationService : MasterService<Convocation>, IConvocationService
    {
        public ConvocationService(IUnitOfWork unitOfWork, IGenericRepository<Convocation> convocationRepository)
            : base(unitOfWork, convocationRepository)
        {
        }
    }
}
