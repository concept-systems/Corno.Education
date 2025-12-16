using OnlineExam.Models;

namespace Corno.Services
{
    public class DegreeService : MasterService<Degree>, IDegreeService
    {
        public DegreeService(IUnitOfWork unitOfWork, IGenericRepository<Degree> degreeRepository)
            : base(unitOfWork, degreeRepository)
        {
        }
    }
}
