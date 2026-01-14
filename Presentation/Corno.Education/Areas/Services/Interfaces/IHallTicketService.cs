using System.Threading.Tasks;
using Corno.Data.Reports;
using CrystalDecisions.CrystalReports.Engine;

namespace Corno.Education.Areas.Services.Interfaces;

public interface IHallTicketService
{
    Task<ReportDocument> GetCrystalReport(HallTicketViewModel dto, int instanceId);
}