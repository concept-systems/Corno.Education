using OnlineExam.Models;

namespace Corno.Services
{
    public class BVDU_STUDENT_REVALUATION_Service : MasterService<TBL_STUDENT_REVALUATION>, IBVDU_STUDENT_REVALUATION_Service
    {
        public BVDU_STUDENT_REVALUATION_Service(IUnitOfWork unitOfWork, IGenericRepository<TBL_STUDENT_REVALUATION> _tbL_STUDENT_REVALUATIONRepository)
            : base(unitOfWork, _tbL_STUDENT_REVALUATIONRepository)
        {
        }
    }
}
