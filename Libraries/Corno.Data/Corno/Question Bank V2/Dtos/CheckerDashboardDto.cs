using System.Collections.Generic;
using Corno.Data.Corno.Question_Bank_V2.Models;

namespace Corno.Data.Corno.Question_Bank_V2.Dtos
{
    public class CheckerDashboardDto
    {
        public int PendingReview { get; set; }
        public int ReviewedToday { get; set; }
        public int TotalReviewed { get; set; }
        public List<QB_Appointment> RecentAppointments { get; set; }
        
        public CheckerDashboardDto()
        {
            RecentAppointments = new List<QB_Appointment>();
        }
    }
}
