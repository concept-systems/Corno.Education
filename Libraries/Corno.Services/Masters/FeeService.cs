using OnlineExam.Models;

namespace Corno.Services
{
    public class FeeService : MasterService<Fee>,IFeeService
    {
        public FeeService(IUnitOfWork unitOfWork, IGenericRepository<Fee> feeRepository)
            : base(unitOfWork, feeRepository)
        {
        }
    }
}
