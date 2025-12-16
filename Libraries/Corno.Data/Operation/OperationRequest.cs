using Corno.Globals.Enums;
using System.Collections.Generic;
using Corno.Data.ViewModels;

namespace Corno.Data.Operation;

public class OperationRequest : BaseViewModel
{
    #region -- Constructors --

    public OperationRequest()
    {
        //Barcodes = new List<string>();
        //OldStatus = new List<string>();

        InputData = new Dictionary<string, object>();
    }
    #endregion

    #region -- Properties --
    public Globals.Enums.Operation Operation { get; set; }
    public ScreenAction Action { get; set; }

    //public List<string> OldStatus { get; set; }
    //public string NewStatus { get; set; }

    //public string MachineCode { get; set; }

    //public string SoNo { get; set; }
    //public string PoNo { get; set; }
    //public string ProductionOrderNo { get; set; }
    //public string WarehouseOrderNo { get; set; }
    //public string LoadNo { get; set; }
    //public string DeliveryNo { get; set; }
    //public string PalletNo { get; set; }
    //public string RackNo { get; set; }

    //public int? ProductId { get; set; }
    //public int? PackingTypeId { get; set; }
    //public int? CustomerId { get; set; }

    //public int? CartonNo { get; set; }

    //public string Barcode { get; set; }
    //public List<string> Barcodes { get; set; }
    //public string LocationBarcode { get; set; }

    //public double? Quantity { get; set; }

    //public double? TareWeight { get; set; }
    //public double? ScaleWeight { get; set; }

    // Quality
    //public string StoppageCode { get; set; }
    //public double? Interval { get; set; }
    //public string Remarks { get; set; }

    //public string UserName { get; set; }

    //public IEnumerable GridDataSource { get; set; }
    //public IEnumerable LayoutDataSource { get; set; }

    //public DateTime? DueDate { get; set; }

    //public int? SupplierId { get; set; }

    //public string ReceiptNo { get; set; }
    //public string InvoiceNo { get; set; }
    //public string BatchNo { get; set; }

    //public string InspectionNo { get; set; }
    //public string CarcassCode { get; set; }
    //public string Position { get; set; }

    //public int? ItemId { get; set; }
    //public string Group { get; set; }
    //public string ItemCode { get; set; }
    //public int? PlantId { get; set; }

    //public double? StandardWeight { get; set; }
    //public double? Tolerance { get; set; }
    //public double? GrossWeight { get; set; }
    //public double? NetWeight { get; set; }

    //public double? OrderQuantity { get; set; }
    //public double? PrintQuantity { get; set; }
    //public double? DeliverQuantity { get; set; }
    //public double? AcceptQuantity { get; set; }
    //public double? RejectQuantity { get; set; }
    //public double? PendingQuantity { get; set; }
    //public double? SetQuantity { get; set; }

    //public int? LabelCount { get; set; }
    //public int? RejectLabelCount { get; set; }

    //public string LotNo { get; set; }
    //public string GrnNo { get; set; }
    //public int? GrnId { get; set; }
    //public string IndentNo { get; set; }
    //public int WarehousePosition { get; set; }
    //public int AsnId { get; set; }

    //public bool IsImosLabel { get; set; }
    //public string ArticleNo { get; set; }

    //public string ShiftInCharge { get; set; }
    //public string PdiInCharge { get; set; }

    // Qc
    //public int ReasonId { get; set; }
    //public string InspectionInstruction { get; set; }
    //public string InspectionResult { get; set; }
    //public int DispositionId { get; set; }

    //public string LabelFormat { get; set; }

    public Dictionary<string, object> InputData { get; set; }
    #endregion

    #region -- Methods --

    public OperationResponse GetResponse()
    {
        return new OperationResponse
        {
            Operation = Operation,
            Action = Action
        };
    }

    public object GetInputData(string key)
    {
        InputData.TryGetValue(key, out var value);
        return value;
    }

    public TEntity Get<TEntity>(string key)
    {
        InputData.TryGetValue(key, out var value);
        return value is TEntity entity ? entity : default;
    }

    public void AddData(string key, object value)
    {
        if (InputData.ContainsKey(key))
            InputData[key] = value;
        else
            InputData.Add(key, value);
    }

    #endregion
}