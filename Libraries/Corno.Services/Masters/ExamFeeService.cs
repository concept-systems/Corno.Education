using OnlineExam.Models;

namespace Corno.Services
{
    public class ExamFeeService : MasterService<ExamFee>,IExamFeeService
    {
        public ExamFeeService(IUnitOfWork unitOfWork, IGenericRepository<ExamFee> examFeeRepository)
            : base(unitOfWork, examFeeRepository)
        {
        }
    }
}
