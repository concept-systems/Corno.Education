using Corno.Data.Corno.Question_Bank.Models;
using Corno.Services.Corno.Interfaces;
using Telerik.Windows.Documents.Flow.Model;

namespace Corno.Services.Corno.Question_Bank.Interfaces;

public interface ITelerikDocumentService : IBaseService
{
    RadFlowDocument Build(Paper paper);

    byte[] ExportDocx(RadFlowDocument doc);
    byte[] ExportPdf(RadFlowDocument doc);
}