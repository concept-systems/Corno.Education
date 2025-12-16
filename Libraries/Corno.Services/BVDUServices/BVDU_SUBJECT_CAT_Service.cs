using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_SUBJECT_CAT_Service : MasterService<Tbl_SUBJECT_CAT_MSTR>, IBVDU_SUBJECT_CAT_Service
    {
        public BVDU_SUBJECT_CAT_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_SUBJECT_CAT_MSTR> _tbl_SUBJECT_CAT_MSTRRepository)
            : base(unitOfWork, _tbl_SUBJECT_CAT_MSTRRepository)
        {
        }
    }
}
