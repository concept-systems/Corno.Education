using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_SUBJECT_Service : MasterService<Tbl_SUBJECT_MSTR>, IBVDU_SUBJECT_Service
    {
        public BVDU_SUBJECT_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_SUBJECT_MSTR> _tbl_SUBJECT_MSTRRepository)
            : base(unitOfWork, _tbl_SUBJECT_MSTRRepository)
        {
        }
    }
}
