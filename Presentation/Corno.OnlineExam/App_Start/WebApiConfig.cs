using System.Web.Http;
using Telerik.Reporting.Services.WebApi;

namespace Corno.OnlineExam;

public static class WebApiConfig
{
    public static void Register(HttpConfiguration config)
    {
        config.Routes.MapHttpRoute(
            "DefaultApi",
            "api/{controller}/{id}",
            new {id = RouteParameter.Optional}
        );

        ReportsControllerConfiguration.RegisterRoutes(config);
    }
}