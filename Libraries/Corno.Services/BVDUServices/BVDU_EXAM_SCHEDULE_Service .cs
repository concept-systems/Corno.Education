using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_EXAM_SCHEDULE_Service : MasterService<Tbl_EXAM_SCHEDULE_MSTR>,IBVDU_EXAM_SCHEDULE_Service
    {
        public BVDU_EXAM_SCHEDULE_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_EXAM_SCHEDULE_MSTR> _tbl_EXAM_SCHEDULE_MSTRRepository)
            : base(unitOfWork, _tbl_EXAM_SCHEDULE_MSTRRepository)
        {
        }
    }
}
