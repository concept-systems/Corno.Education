using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_STUDENT_INFO_Service : MasterService<Tbl_STUDENT_INFO>, IBVDU_STUDENT_INFO_Service
    {
        public BVDU_STUDENT_INFO_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_STUDENT_INFO> _tbl_STUDENT_INFORepository)
            : base(unitOfWork, _tbl_STUDENT_INFORepository)
        {
        }
    }
}
