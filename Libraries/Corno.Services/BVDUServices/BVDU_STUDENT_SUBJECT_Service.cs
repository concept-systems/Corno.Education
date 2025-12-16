using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_STUDENT_SUBJECT_Service : MasterService<Tbl_STUDENT_SUBJECT>, IBVDU_STUDENT_SUBJECT_Service
    {
        public BVDU_STUDENT_SUBJECT_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_STUDENT_SUBJECT> _tbl_STUDENT_SUBJECTRepository)
            : base(unitOfWork, _tbl_STUDENT_SUBJECTRepository)
        {
        }
    }
}
