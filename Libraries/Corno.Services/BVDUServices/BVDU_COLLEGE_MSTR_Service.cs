using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_COLLEGE_MSTR_Service : MasterService<Tbl_COLLEGE_MSTR>, IBVDU_COLLEGE_MSTR_Service
    {
        public BVDU_COLLEGE_MSTR_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_COLLEGE_MSTR> _tbl_COLLEGE_MSTRRepository)
            : base(unitOfWork, _tbl_COLLEGE_MSTRRepository)
        {
        }
    }
}
