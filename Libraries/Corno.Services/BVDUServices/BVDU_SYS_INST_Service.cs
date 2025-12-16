using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_SYS_INST_Service : MasterService<Tbl_SYS_INST>, IBVDU_SYS_INST_Service
    {
        public BVDU_SYS_INST_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_SYS_INST> _tbl_SYS_INSTRepository)
            : base(unitOfWork, _tbl_SYS_INSTRepository)
        {
        }
    }
}
