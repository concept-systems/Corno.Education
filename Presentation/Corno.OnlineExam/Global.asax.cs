using Corno.Globals;
using Corno.OnlineExam.AreaLib;
using Corno.OnlineExam.Areas.Admin.Controllers;
using Corno.OnlineExam.Controllers;
using System;
using System.Configuration;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;
using Corno.Services.Bootstrapper;
using Unity;
using Unity.Injection;

namespace Corno.OnlineExam;

public class MvcApplication : HttpApplication
{
    protected void Application_Start()
    {
        Telerik.Reporting.Services.WebApi.ReportsControllerConfiguration.RegisterRoutes(System.Web.Http.GlobalConfiguration.Configuration);
        ViewEngines.Engines.Clear();
        ViewEngines.Engines.Add(new AreaViewEngine());

        AreaRegistration.RegisterAllAreas();

        //WebApiConfig.Register(GlobalConfiguration.Configuration);

        FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
        RouteConfig.RegisterRoutes(RouteTable.Routes);
        BundleConfig.RegisterBundles(BundleTable.Bundles);

        //BootstrapEditorTemplatesConfig.RegisterBundles();
        //Setup DI
        Bootstrapper.Initialise();
        AutoMapperConfig.RegisterMappings();

        // Telerik Reporting
        //ReportsControllerConfiguration.RegisterRoutes(GlobalConfiguration.Configuration);

        // Set Connection String
        GlobalVariables.ConnectionString =
            ConfigurationManager.ConnectionStrings["DefaultConnection"].ConnectionString;
        GlobalVariables.ConnectionStringExamServer =
            ConfigurationManager.ConnectionStrings["CoreContext"].ConnectionString;

        // Special case for login page. otherwise login page gets problematic.
        GlobalVariables.Container.RegisterType<AccountController>(new InjectionConstructor());
    }

    protected void Application_Error(object sender, EventArgs e)
    {
        var httpContext = ((MvcApplication) sender).Context;
        var currentController = " ";
        var currentAction = " ";
        var currentRouteData = RouteTable.Routes.GetRouteData(new HttpContextWrapper(httpContext));

        if (currentRouteData != null)
        {
            if (!string.IsNullOrEmpty(currentRouteData.Values["controller"]?.ToString()))
            {
                currentController = currentRouteData.Values["controller"].ToString();
            }

            if (!string.IsNullOrEmpty(currentRouteData.Values["action"]?.ToString()))
            {
                currentAction = currentRouteData.Values["action"].ToString();
            }
        }

        var exception = Server.GetLastError();
        //var controller = new ErrorController();
        var routeData = new RouteData();
        var action = "GenericError";

        var httpException = exception as HttpException;
        if (null != httpException)
        {
            switch (httpException.GetHttpCode())
            {
                case 404:
                    action = "NotFound";
                    break;

                // others if any
            }
        }

        httpContext.ClearError();
        httpContext.Response.Clear();
        if (null != httpException)
        {
            httpContext.Response.StatusCode = httpException.GetHttpCode();
            httpContext.Response.TrySkipIisCustomErrors = true;
        }
        else
        {
            httpContext.Response.StatusCode = 0;
            httpContext.Response.TrySkipIisCustomErrors = true;
        }

        routeData.Values["controller"] = "Error";
        routeData.Values["action"] = action;
        routeData.Values["exception"] = new HandleErrorInfo(exception, currentController, currentAction);

        IController managerController = new ErrorController();
        var wrapper = new HttpContextWrapper(httpContext);
        var rc = new RequestContext(wrapper, routeData);
        managerController.Execute(rc);

        //// At this point we have information about the error
        //var ctx = HttpContext.Current;
        //var exception = ctx.Server.GetLastError();
        //var errorInfo =
        //    "<br>Offending URL: " + ctx.Request.Url +
        //    "<br>Source: " + exception.Source +
        //    "<br>Message: " + exception.Message +
        //    "<br>Stack trace: " + exception.StackTrace;

        //ctx.Response.Write(errorInfo);
    }

    //public static void RegisterGlobalFilters(GlobalFilterCollection filters)
    //{
    //    filters.Add(new DisableCache());
    //}

    protected void Application_BeginRequest()
    {
        var culture = new System.Globalization.CultureInfo("en-GB");
        System.Threading.Thread.CurrentThread.CurrentCulture = culture;
        System.Threading.Thread.CurrentThread.CurrentUICulture = culture;
    }

    protected void Session_Start(object sender, EventArgs e)
    {
    }

    protected void Session_End(object sender, EventArgs e)
    {
        try
        {
            if (null != Session[User?.Identity?.Name])
                Session.Remove(User?.Identity?.Name);
        }
        catch
        {
            // ignored
        }
    }

    /*protected void Application_AcquireRequestState(object sender, EventArgs e)
    {
        // Check if the session has ended
        if (HttpContext.Current.Session != null && Session["UserSession"] != null) 
            return;
        if (HttpContext.Current.Request.Url.AbsolutePath != "/Account/Login")
            HttpContext.Current.Response.Redirect("~/Account/Login");
    }*/

    public class DisableCache : ActionFilterAttribute
    {
        public override void OnResultExecuting(ResultExecutingContext filterContext)
        {
            filterContext.HttpContext.Response.Cache.SetExpires(DateTime.UtcNow.AddDays(-1));
            filterContext.HttpContext.Response.Cache.SetValidUntilExpires(false);
            filterContext.HttpContext.Response.Cache.SetRevalidation(HttpCacheRevalidation.AllCaches);
            filterContext.HttpContext.Response.Cache.SetCacheability(HttpCacheability.NoCache);
            filterContext.HttpContext.Response.Cache.SetNoStore();
        }
    }
}