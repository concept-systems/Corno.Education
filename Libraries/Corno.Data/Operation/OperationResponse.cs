using System.Collections;
using System.Collections.Generic;
using Corno.Data.ViewModels;
using Corno.Globals.Enums;

namespace Corno.Data.Operation;

public class OperationResponse : BaseViewModel
{
    #region -- Constructors --

    public OperationResponse()
    {
        OutputData = new Dictionary<string, object>();
    }
    #endregion

    #region -- Properties --
    public Globals.Enums.Operation Operation { get; set; }
    public ScreenAction Action { get; set; }

    public double? Quantity { get; set; }

    public object Result { get; set; }

    public Dictionary<string, object> OutputData { get; set; }

    //spublic IEnumerable<BaseReport> Reports { get; set; }
    public IEnumerable LayoutDataSource { get; set; }
    public IEnumerable GridDataSource { get; set; }
    #endregion

    #region -- Methods --
    public object GetOutputData(string key)
    {
        OutputData.TryGetValue(key, out var value);
        return value;
    }
    #endregion
}