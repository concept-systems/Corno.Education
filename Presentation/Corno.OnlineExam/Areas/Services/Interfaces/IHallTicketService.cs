using System.Threading.Tasks;
using Corno.Data.Reports;
using CrystalDecisions.CrystalReports.Engine;

namespace Corno.OnlineExam.Areas.Services.Interfaces;

public interface IHallTicketService
{
    Task<ReportDocument> GetCrystalReport(HallTicketViewModel dto, int instanceId);
}