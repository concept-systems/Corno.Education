using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_GRADE_MSTR_Service : MasterService<Tbl_GRADE_MSTR>, IBVDU_GRADE_MSTR_Service
    {
        public BVDU_GRADE_MSTR_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_GRADE_MSTR> _tbl_GRADE_MSTRRepository)
            : base(unitOfWork, _tbl_GRADE_MSTRRepository)
        {
        }
    }
}
