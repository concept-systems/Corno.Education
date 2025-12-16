using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using Corno.Data.Common;

namespace Corno.Data.Corno.Question_Bank;

[Serializable]
public class PurchaseInvoice : UniversityBaseModel
{
    #region -- Constructors -- 
    public PurchaseInvoice()
    {
        //EnableHeader = true;

        PurchaseInvoiceDetails = new List<PurchaseInvoiceDetail>();
    }
    #endregion

    [NotMapped]
    public new int? CentreId { get; set; }
    [NotMapped]
    public new int? CourseTypeId { get; set; }

    public int? SubjectId { get; set; }
    public int? CategoryId { get; set; }
    public int? StaffId { get; set; }

    public List<PurchaseInvoiceDetail> PurchaseInvoiceDetails { get; set; }

    #region -- Public Methods --
    public override bool UpdateDetails(BaseModel newModel)
    {
        if (newModel is not PurchaseInvoice newPurchaseInvoice) return false;

        var toDelete = new List<PurchaseInvoiceDetail>();
        foreach (var purchaseInvoiceDetail in PurchaseInvoiceDetails)
        {
            var newPurchaseInvoiceDetail = newPurchaseInvoice.PurchaseInvoiceDetails.FirstOrDefault(d =>
                d.Id == purchaseInvoiceDetail.Id);
            purchaseInvoiceDetail.Copy(newPurchaseInvoiceDetail);
        }

        // Add new entries
        var newPurchaseInvoiceDetails = newPurchaseInvoice.PurchaseInvoiceDetails.Where(d => d.Id <= 0).ToList();
        PurchaseInvoiceDetails.AddRange(newPurchaseInvoiceDetails);

        // Delete existing entries
        foreach (var purchaseInvoiceDetail in toDelete)
            PurchaseInvoiceDetails.Remove(purchaseInvoiceDetail);

        return true;
    }
    #endregion
}