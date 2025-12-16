using Corno.Data.Corno.Question_Bank.Models;
using Corno.Services.Corno.Interfaces;
using DevExpress.XtraRichEdit;

namespace Corno.Services.Corno.Question_Bank.Interfaces;

public interface IRichEditDocumentService : IBaseService
{
    #region -- Methods --

    RichEditDocumentServer CreateWordFile(Paper paper);
    string ConvertByteArrayToHtml(byte[] bytes);
    byte[] ConvertHtmlToByteArray(string htmlContent);
    string ConvertByteArrayToPlainText(byte[] bytes);

    #endregion
}