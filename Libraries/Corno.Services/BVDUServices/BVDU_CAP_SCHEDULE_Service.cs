using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_CAP_SCHEDULE_Service : MasterService<Tbl_CAP_SCHEDULE_MSTR>, IBVDU_CAP_SCHEDULE_Service
    {
        public BVDU_CAP_SCHEDULE_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_CAP_SCHEDULE_MSTR> _tbl_CAP_SCHEDULE_MSTRRepository)
            : base(unitOfWork, _tbl_CAP_SCHEDULE_MSTRRepository)
        {
        }
    }
}
