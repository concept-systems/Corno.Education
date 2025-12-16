using Corno.Data.Corno.Question_Bank;
using Corno.Data.Corno.Question_Bank.Models;
using Corno.Services.Corno.Interfaces;
using Telerik.Windows.Documents.Flow.Model;

namespace Corno.Services.Corno.Question_Bank.Interfaces;

public interface IDocumentService : IBaseService
{
    #region -- Methods --

    RadFlowDocument CreateWordFile(Paper paper);

    #endregion
}