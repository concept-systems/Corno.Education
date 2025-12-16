using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_CLASS_Service : MasterService<Tbl_CLASS_MSTR>, IBVDU_CLASS_Service
    {
        public BVDU_CLASS_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_CLASS_MSTR> _tbl_CLASS_MSTRRepository)
            : base(unitOfWork, _tbl_CLASS_MSTRRepository)
        {
        }
    }
}
