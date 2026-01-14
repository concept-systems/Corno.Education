using System.Web.Mvc;

namespace Corno.Education.Areas.Online_Education
{
    public class OnlineEducationAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Online Education";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Online Education_default",
                "Online Education/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
