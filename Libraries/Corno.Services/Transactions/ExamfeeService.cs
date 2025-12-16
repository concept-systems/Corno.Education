using OnlineExam.Models;

namespace Corno.Services
{
    public class ExamfeeService : MasterService<ExamFee>,IExamfeeService
    {
        public ExamfeeService(IUnitOfWork unitOfWork, IGenericRepository<ExamFee> examfeeRepository)
            : base(unitOfWork, examfeeRepository)
        {
        }
    }
}
