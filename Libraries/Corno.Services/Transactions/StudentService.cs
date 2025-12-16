using OnlineExam.Models;

namespace Corno.Services
{
    public class StudentService : MasterService<Student>, IStudentService
    {
        public StudentService(IUnitOfWork unitOfWork, IGenericRepository<Student> studentRepository)
            : base(unitOfWork, studentRepository)
        {
        }
    }
}
