using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_STUDENT_INFO_ADR_Service : MasterService<Tbl_STUDENT_INFO_ADR>, IBVDU_STUDENT_INFO_ADR_Service
    {
        public BVDU_STUDENT_INFO_ADR_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_STUDENT_INFO_ADR> _tbl_STUDENT_INFO_ADRRepository)
            : base(unitOfWork, _tbl_STUDENT_INFO_ADRRepository)
        {
        }
    }
}
