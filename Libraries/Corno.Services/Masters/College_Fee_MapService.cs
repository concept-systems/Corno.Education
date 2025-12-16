using OnlineExam.Models;

namespace Corno.Services
{
    public class College_Fee_MapService : MasterService<College_Fee_Map>, ICollege_Fee_MapService
    {
        public College_Fee_MapService(IUnitOfWork unitOfWork, IGenericRepository<College_Fee_Map> collegefeemapRepository)
            : base(unitOfWork, collegefeemapRepository)
        {
        }
    }
}
