using System;
using System.Web.Mvc;
using System.Web.Routing;

namespace Corno.OnlineExam.AreaLib;

public static class AreaRouteHelper
{
    public static void MapAreas(this RouteCollection routes, string url, string rootNamespace, string[] areas)
    {
        Array.ForEach(areas, area =>
        {
            var route = new Route("{area}/" + url, new MvcRouteHandler())
            {
                Constraints = new RouteValueDictionary(new {area})
            };
            var areaNamespace = rootNamespace + ".Areas." + area + ".Controllers";
            route.DataTokens = new RouteValueDictionary(new {namespaces = new[] {areaNamespace}});
            route.Defaults = new RouteValueDictionary(new {action = "Login", controller = "Account", id = ""});
            routes.Add(route);
        });
    }

    public static void MapRootArea(this RouteCollection routes, string url, string rootNamespace, object defaults)
    {
        var route = new Route(url, new MvcRouteHandler())
        {
            DataTokens = new RouteValueDictionary(new {namespaces = new[] {rootNamespace + ".Controllers"}}),
            Defaults =
                new RouteValueDictionary(new {area = "root", action = "Login", controller = "Account", id = ""})
        };
        routes.Add(route);
    }
}