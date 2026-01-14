using System.Web.Mvc;

namespace Corno.Education.Areas.Transactions
{
    public class TransactionsAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Transactions";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            // Check if route already exists to avoid duplicate registration
            var routeName = "Transactions_default";
            if (context.Routes[routeName] == null)
            {
                context.MapRoute(
                    routeName,
                    "Transactions/{controller}/{action}/{id}",
                    new { action = "Index", id = UrlParameter.Optional }
                );
            }
        }
    }
}
