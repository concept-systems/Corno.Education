using Corno.Data.Common;
using Kendo.Mvc.UI;

namespace Corno.Data.Operation;

public class MultiColumnComboBoxRequest
{
    #region -- Constructors --
    public MultiColumnComboBoxRequest()
    {
        DataSourceRequest = new DataSourceRequest();
        ViewModel = new UniversityBaseModel();
    }
    #endregion

    public DataSourceRequest DataSourceRequest { get; set; }


    public string Filter { get; set; }  // Text typed in the combobox

    public UniversityBaseModel ViewModel { get; set; }
}
