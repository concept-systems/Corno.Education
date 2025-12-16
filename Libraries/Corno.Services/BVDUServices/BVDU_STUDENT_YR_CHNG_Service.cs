using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_STUDENT_YR_CHNG_Service : MasterService<Tbl_STUDENT_YR_CHNG>, IBVDU_STUDENT_YR_CHNG_Service
    {
        public BVDU_STUDENT_YR_CHNG_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_STUDENT_YR_CHNG> _tbl_STUDENT_YR_CHNGRepository)
            : base(unitOfWork, _tbl_STUDENT_YR_CHNGRepository)
        {
        }
    }
}
