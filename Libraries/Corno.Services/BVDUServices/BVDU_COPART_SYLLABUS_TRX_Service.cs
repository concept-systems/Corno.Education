using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_COPART_SYLLABUS_TRX_Service : MasterService<Tbl_COPART_SYLLABUS_TRX>, IBVDU_COPART_SYLLABUS_TRX_Service
    {
        public BVDU_COPART_SYLLABUS_TRX_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_COPART_SYLLABUS_TRX> _tbl_COPART_SYLLABUS_TRXRepository)
            : base(unitOfWork, _tbl_COPART_SYLLABUS_TRXRepository)
        {
        }
    }
}
