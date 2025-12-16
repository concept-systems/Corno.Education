using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_APP_TEMP_SUB_Service : MasterService<Tbl_APP_TEMP_SUB>, IBVDU_APP_TEMP_SUB_Service
    {
        public BVDU_APP_TEMP_SUB_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_APP_TEMP_SUB> _tbl_APP_TEMP_SUB_EXAMSRepository)
            : base(unitOfWork, _tbl_APP_TEMP_SUB_EXAMSRepository)
        {
        }
    }
}
