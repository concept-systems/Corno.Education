using Corno.Data.Corno.Question_Bank;
using Corno.Services.Corno.Interfaces;
using System.Collections.Generic;
using Corno.Data.ViewModels.Appointment;

namespace Corno.Services.Corno.Question_Bank.Interfaces;

public interface IPurchaseInvoiceService : IMainService<PurchaseInvoice>
{
    #region -- Methods --

    void ValidateHeader(PurchaseInvoice purchaseInvoice);
    void ValidateReportHeader(PurchaseInvoice purchaseInvoice);
    void ValidateFields(PurchaseInvoice purchaseInvoice);

    IEnumerable<ReportModel> GetData(PurchaseInvoice purchaseInvoice, bool onlyInternal);

    PurchaseInvoice GetStaffs(PurchaseInvoice purchaseInvoice);

    void Save(PurchaseInvoice purchaseInvoice);
    #endregion
}