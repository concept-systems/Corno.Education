using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_SUB_CATPAP_Service : MasterService<Tbl_SUB_CATPAP_MSTR>, IBVDU_SUB_CATPAP_Service
    {
        public BVDU_SUB_CATPAP_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_SUB_CATPAP_MSTR> _tbl_SUB_CATPAP_MSTRRepository)
            : base(unitOfWork, _tbl_SUB_CATPAP_MSTRRepository)
        {
        }
    }
}
