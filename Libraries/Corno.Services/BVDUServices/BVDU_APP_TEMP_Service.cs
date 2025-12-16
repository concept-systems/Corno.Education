using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_APP_TEMP_Service : MasterService<Tbl_APP_TEMP>, IBVDU_APP_TEMP_Service
    {
        public BVDU_APP_TEMP_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_APP_TEMP> _tbl_APP_TEMP_EXAMSRepository)
            : base(unitOfWork, _tbl_APP_TEMP_EXAMSRepository)
        {
        }
    }
}
