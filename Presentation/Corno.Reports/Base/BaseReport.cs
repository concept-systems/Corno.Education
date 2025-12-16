using Corno.Logger;
using Telerik.Reporting;
using Telerik.ReportViewer.Common;

namespace Corno.Reports.Base
{
    public class BaseReport : Report
    {
        #region -- Constructors --

        public BaseReport()
        {
            Error += BaseReport_Error;

            Style.Font.Name = "Segoe UI";
            Style.Font.Size = Telerik.Reporting.Drawing.Unit.Point(8.25D);
        }
        #endregion

        #region -- Properties --
        public object Attributes { get; set; }
        #endregion

        #region -- Methods --
        // Export To CSV
        public virtual void ExportToCsv(string filePath)
        {

        }

        public virtual object GetDataSource()
        {
            return DataSource;
        }

        #endregion

        #region -- Events --
        public virtual void OnActionExecuting(object sender, InteractiveActionCancelEventArgs args)
        {

        }

        private void BaseReport_Error(object sender, ErrorEventArgs eventArgs)
        {
            LogHandler.LogInfo(eventArgs.Exception);
            LogHandler.LogError(eventArgs.Exception);
        }
        #endregion
    }
}
