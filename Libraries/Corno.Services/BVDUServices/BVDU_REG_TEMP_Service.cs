using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_REG_TEMP_Service : MasterService<Tbl_REG_TEMP>, IBVDU_REG_TEMP_Service
    {
        public BVDU_REG_TEMP_Service(IUnitOfWork unitOfWork, IGenericRepository<Tbl_REG_TEMP> _reg_TEMP_Repository)
            : base(unitOfWork, _reg_TEMP_Repository)
        {
        }
    }
}
