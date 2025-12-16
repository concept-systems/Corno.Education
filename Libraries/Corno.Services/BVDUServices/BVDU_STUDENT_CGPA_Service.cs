using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_STUDENT_CGPA_Service : MasterService<TBL_STUDENT_CGPA>, IBVDU_STUDENT_CGPA_Service
    {
        public BVDU_STUDENT_CGPA_Service(IUnitOfWork unitOfWork, IGenericRepository<TBL_STUDENT_CGPA> _tbl_STUDENT_CGPARepository)
            : base(unitOfWork, _tbl_STUDENT_CGPARepository)
        {
        }
    }
}
