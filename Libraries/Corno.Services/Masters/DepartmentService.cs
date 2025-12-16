using OnlineExam.Models;

namespace Corno.Services
{
    public class DepartmentService : MasterService<Department>, IDepartmentService
    {
        public DepartmentService(IUnitOfWork unitOfWork, IGenericRepository<Department> departmentRepository)
            : base(unitOfWork, departmentRepository)
        {
        }
    }
}
