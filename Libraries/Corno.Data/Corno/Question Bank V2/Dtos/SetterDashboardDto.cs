using System.Collections.Generic;
using Corno.Data.Corno.Question_Bank_V2.Models;

namespace Corno.Data.Corno.Question_Bank_V2.Dtos
{
    public class SetterDashboardDto
    {
        public int TotalQuestions { get; set; }
        public int PendingQuestions { get; set; }
        public int SubmittedQuestions { get; set; }
        public int ApprovedQuestions { get; set; }
        public List<QB_Appointment> RecentAppointments { get; set; }
        
        public SetterDashboardDto()
        {
            RecentAppointments = new List<QB_Appointment>();
        }
    }
}
