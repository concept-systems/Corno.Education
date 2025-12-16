using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_STUDENT_ENV_STUDIES_Service : MasterService<TBl_STUDENT_ENV_STUDIES>, IBVDU_STUDENT_ENV_STUDIES_Service
    {
        public BVDU_STUDENT_ENV_STUDIES_Service(IUnitOfWork unitOfWork, IGenericRepository<TBl_STUDENT_ENV_STUDIES> _tbl_STUDENT_ENV_STUDIESRepository)
            : base(unitOfWork, _tbl_STUDENT_ENV_STUDIESRepository)
        {
        }
    }
}
