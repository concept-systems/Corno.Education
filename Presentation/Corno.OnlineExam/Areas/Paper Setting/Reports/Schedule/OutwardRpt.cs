using System.Linq;
using Corno.Data.Helpers;
using Corno.Globals.Constants;
using Corno.Reports.Base;

namespace Corno.OnlineExam.Areas.Paper_Setting.Reports.Schedule
{
    public partial class OutwardRpt : BaseReport
    {
        public OutwardRpt()
        {
            InitializeComponent();
        }

        private void OutwardRpt_NeedDataSource(object sender, System.EventArgs e)
        {
            var report = (Telerik.Reporting.Processing.Report)sender;

            var fromOutwardNo = report.Parameters[ModelConstants.From].Value.ToInt();
            var toOutwardNo = report.Parameters[ModelConstants.To].Value.ToInt();
        }
    }
}