using OnlineExam.Models;
namespace Corno.Services
{
    public class TehsilService : MasterService<Tehsil>,ITehsilService
    {
        public TehsilService(IUnitOfWork unitOfWork, IGenericRepository<Tehsil> tehsilRepository)
            : base(unitOfWork, tehsilRepository)
        {
        }
    }
}
