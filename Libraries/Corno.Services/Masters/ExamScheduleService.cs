using OnlineExam.Models;

namespace Corno.Services
{
    public class ExamScheduleService : MasterService<ExamSchedule>,IExamScheduleService
    {
        public ExamScheduleService(IUnitOfWork unitOfWork, IGenericRepository<ExamSchedule> examScheduleRepository)
            : base(unitOfWork, examScheduleRepository)
        {
        }
    }
}
