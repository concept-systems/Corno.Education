using System.Web.Mvc;

namespace Corno.Education.Areas.Question_Bank_V2
{
    public class QuestionBankV2AreaRegistration : AreaRegistration
    {
        public override string AreaName
        {
            get
            {
                return "Question Bank V2";
            }
        }

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Question_Bank_V2_default",
                "Question Bank V2/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
