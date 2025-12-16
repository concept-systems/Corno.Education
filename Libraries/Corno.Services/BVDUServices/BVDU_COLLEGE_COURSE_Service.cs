using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_COLLEGE_COURSE_Service : MasterService<Tbl_COLLEGE_COURSE_MSTR>, IBVDU_COLLEGE_COURSE_Service
    {
        public BVDU_COLLEGE_COURSE_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_COLLEGE_COURSE_MSTR> _tbl_COLLEGE_COURSE_MSTRRepository)
            : base(unitOfWork, _tbl_COLLEGE_COURSE_MSTRRepository)
        {
        }
    }
}
