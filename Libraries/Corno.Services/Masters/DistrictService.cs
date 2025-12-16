using OnlineExam.Models;

namespace Corno.Services
{
    public class DistrictService : MasterService<District>,IDistrictService
    {
        public DistrictService(IUnitOfWork unitOfWork, IGenericRepository<District> districtRepository)
            : base(unitOfWork, districtRepository)
        {
        }
    }
}
