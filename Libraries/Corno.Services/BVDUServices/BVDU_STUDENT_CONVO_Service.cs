using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_STUDENT_CONVO_Service : MasterService<Tbl_STUDENT_CONVO>, IBVDU_STUDENT_CONVO_Service
    {
        public BVDU_STUDENT_CONVO_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_STUDENT_CONVO> _tbl_STUDENT_CONVORRepository)
            : base(unitOfWork, _tbl_STUDENT_CONVORRepository)
        {
        }
    }
}
