using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_COURSE_Service : MasterService<Tbl_COURSE_MSTR>, IBVDU_COURSE_Service
    {
        public BVDU_COURSE_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_COURSE_MSTR> _tbl_COURSE_MSTRRepository)
            : base(unitOfWork, _tbl_COURSE_MSTRRepository)
        {
        }
    }
}
