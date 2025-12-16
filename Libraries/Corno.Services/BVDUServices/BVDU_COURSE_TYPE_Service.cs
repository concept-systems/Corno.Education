using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_COURSE_TYPE_Service : MasterService<Tbl_COURSE_TYPE_MSTR>, IBVDU_COURSE_TYPE_Service
    {
        public BVDU_COURSE_TYPE_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_COURSE_TYPE_MSTR> _tbl_COURSE_TYPE_MSTRRepository)
            : base(unitOfWork, _tbl_COURSE_TYPE_MSTRRepository)
        {
        }
    }
}
