using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_STUDENT_EXAMS_Service : MasterService<TBL_STUDENT_EXAMS>, IBVDU_STUDENT_EXAMS_Service
    {
        public BVDU_STUDENT_EXAMS_Service(IUnitOfWork unitOfWork, IGenericRepository<TBL_STUDENT_EXAMS> _tbl_STUDENT_EXAMSRepository)
            : base(unitOfWork, _tbl_STUDENT_EXAMSRepository)
        {
        }
    }
}
