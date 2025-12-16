using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_BRANCH_Service : MasterService<Tbl_BRANCH_MSTR>, IBVDU_BRANCH_Service
    {
        public BVDU_BRANCH_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_BRANCH_MSTR> _tbl_BRANCH_MSTRRepository)
            : base(unitOfWork, _tbl_BRANCH_MSTRRepository)
        {
        }
    }
}
