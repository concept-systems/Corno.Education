using System.Web.Mvc;

namespace Corno.Education.Areas.Paper_Setting
{
    public class PaperSettingAreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Paper Setting";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Paper Setting_default",
                "Paper Setting/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
