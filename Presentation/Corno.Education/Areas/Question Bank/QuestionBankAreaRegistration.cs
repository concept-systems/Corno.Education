using System.Web.Mvc;

namespace Corno.Education.Areas.Question_Bank
{
    public class QuestionBankAreaRegistration : AreaRegistration
    {
        public override string AreaName => "Question Bank";

        public override void RegisterArea(AreaRegistrationContext context)
        {
            context.MapRoute(
                "Question Bank_default",
                "Question Bank/{controller}/{action}/{id}",
                new { action = "Index", id = UrlParameter.Optional }
            );
        }
    }
}
