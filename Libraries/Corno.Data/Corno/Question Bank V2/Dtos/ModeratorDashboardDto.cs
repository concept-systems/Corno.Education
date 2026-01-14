using System.Collections.Generic;
using Corno.Data.Corno.Question_Bank_V2.Models;

namespace Corno.Data.Corno.Question_Bank_V2.Dtos
{
    public class ModeratorDashboardDto
    {
        public int TotalPapers { get; set; }
        public int GeneratedPapers { get; set; }
        public int DrawnPapers { get; set; }
        public List<QB_Appointment> RecentAppointments { get; set; }
        
        public ModeratorDashboardDto()
        {
            RecentAppointments = new List<QB_Appointment>();
        }
    }
}
