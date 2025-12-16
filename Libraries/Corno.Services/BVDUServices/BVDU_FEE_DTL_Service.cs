using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_FEE_DTL_Service : MasterService<Tbl_FEE_DTL>, IBVDU_FEE_DTL_Service
    {
        public BVDU_FEE_DTL_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_FEE_DTL> _tbl_FEE_DTLRepository)
            : base(unitOfWork, _tbl_FEE_DTLRepository)
        {
        }
    }
}
