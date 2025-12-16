using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_STUDENT_COURSE_Service : MasterService<Tbl_STUDENT_COURSE>, IBVDU_STUDENT_COURSE_Service
    {
        public BVDU_STUDENT_COURSE_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_STUDENT_COURSE> _tbl_STUDENT_COURSERepository)
            : base(unitOfWork, _tbl_STUDENT_COURSERepository)
        {
        }
    }
}
