using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_FACULTY_Service : MasterService<Tbl_FACULTY_MSTR>, IBVDU_FACULTY_Service
    {
        public BVDU_FACULTY_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_FACULTY_MSTR> _tbl_FACULTY_MSTRRepository)
            : base(unitOfWork, _tbl_FACULTY_MSTRRepository)
        {
        }
    }
}
