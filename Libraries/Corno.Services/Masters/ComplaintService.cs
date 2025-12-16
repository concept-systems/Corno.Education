using OnlineExam.Models;

namespace Corno.Services
{
    public class ComplaintService : MasterService<Complaint>,IComplaintService
    {
        public ComplaintService(IUnitOfWork unitOfWork, IGenericRepository<Complaint> complaintRepository)
            : base(unitOfWork, complaintRepository)
        {
        }
    }
}
