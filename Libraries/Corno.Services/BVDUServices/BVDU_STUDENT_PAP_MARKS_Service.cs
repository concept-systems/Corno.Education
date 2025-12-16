using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_STUDENT_PAP_MARKS_Service : MasterService<Tbl_STUDENT_PAP_MARKS>, IBVDU_STUDENT_PAP_MARKS_Service
    {
        public BVDU_STUDENT_PAP_MARKS_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_STUDENT_PAP_MARKS> _tbl_STUDENT_PAP_MARKSRepository)
            : base(unitOfWork, _tbl_STUDENT_PAP_MARKSRepository)
        {
        }
    }
}
