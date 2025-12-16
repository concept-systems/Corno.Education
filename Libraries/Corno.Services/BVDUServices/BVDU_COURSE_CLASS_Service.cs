using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_COURSE_CLASS_Service : MasterService<Tbl_COURSE_CLASS_MSTR>, IBVDU_COURSE_CLASS_Service
    {
        public BVDU_COURSE_CLASS_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_COURSE_CLASS_MSTR> _tbl_COURSE_CLASS_MSTRRepository)
            : base(unitOfWork, _tbl_COURSE_CLASS_MSTRRepository)
        {
        }
    }
}
