using System.Collections.Generic;
using Corno.Data.Corno.Question_Bank_V2.Models;

namespace Corno.Data.Corno.Question_Bank_V2.Dtos
{
    public class AdminDashboardDto
    {
        public int TotalQuestions { get; set; }
        public int ApprovedQuestions { get; set; }
        public int TotalAppointments { get; set; }
        public int TotalPapers { get; set; }
    }
}
