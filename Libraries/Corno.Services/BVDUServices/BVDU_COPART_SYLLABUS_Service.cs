using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_COPART_SYLLABUS_Service : MasterService<Tbl_COPART_SYLLABUS>, IBVDU_COPART_SYLLABUS_Service
    {
        public BVDU_COPART_SYLLABUS_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_COPART_SYLLABUS> _tbl_COPART_SYLLABUSRepository)
            : base(unitOfWork, _tbl_COPART_SYLLABUSRepository)
        {
        }
    }
}
