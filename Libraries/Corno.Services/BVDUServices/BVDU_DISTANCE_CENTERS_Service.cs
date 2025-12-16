using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_DISTANCE_CENTERS_Service : MasterService<TBL_DISTANCE_CENTERS>, IBVDU_DISTANCE_CENTERS_Service
    {
        public BVDU_DISTANCE_CENTERS_Service(IUnitOfWork unitOfWork, IGenericRepository<TBL_DISTANCE_CENTERS> _tbl_DISTANCE_CENTERSRepository)
            : base(unitOfWork, _tbl_DISTANCE_CENTERSRepository)
        {
        }
    }
}
