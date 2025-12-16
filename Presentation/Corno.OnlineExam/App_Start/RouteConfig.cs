using System.Web.Mvc;
using System.Web.Routing;
using Corno.OnlineExam.AreaLib;

namespace Corno.OnlineExam;

public static class RouteConfig
{
    public static void RegisterRoutes(RouteCollection routes)
    {
        //var settings = new FriendlyUrlSettings();
        //settings.AutoRedirectMode = RedirectMode.Permanent;
        //routes.EnableFriendlyUrls(settings);
        routes.IgnoreRoute("{resource}.axd/{*pathInfo}");
        routes.IgnoreRoute("{resource}.aspx/{*pathInfo}");
        routes.IgnoreRoute("{*allaspx}", new { allaspx = @".*(CrystalImageHandler).*" });
        routes.MapAreas("{controller}/{action}/{id}",
            "Areas",
            new[] {"Admin", "Account", "Masters", "Paper Setting", "Question Bank", "Online Education", "PhD", "Bos", "Transactions", "Reports", "Api"});

        routes.MapRootArea("{controller}/{action}/{id}",
            "Areas",
            new {controller = "Home", action = "Index", id = ""});

        //routes.MapRoute(
        //        name: "Default",
        //        url: "{controller}/{action}/{id}",
        //        defaults: new { action = "Index", id = UrlParameter.Optional }

        //);
    }
}