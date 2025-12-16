using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_COURSE_PART_Service : MasterService<Tbl_COURSE_PART_MSTR>, IBVDU_COURSE_PART_Service
    {
        public BVDU_COURSE_PART_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_COURSE_PART_MSTR> _tbl_COURSE_PART_MSTRRepository)
            : base(unitOfWork, _tbl_COURSE_PART_MSTRRepository)
        {
        }
    }
}
