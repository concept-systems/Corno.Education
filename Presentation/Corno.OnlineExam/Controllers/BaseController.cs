using Corno.Globals.Constants;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data.Entity.Validation;
using System.IO.Compression;
using System.Linq;
using System.Web.Mvc;
using System.Web.Routing;
using Corno.Globals;
using Corno.Logger;

namespace Corno.OnlineExam.Controllers;

//[Compress]
[CustomAuthorize]
public class BaseController : Controller
{
    #region -- Protected Methods --

    protected SessionData GetSession()
    {
        if (!(Session[User.Identity.Name] is SessionData sessionData))
            throw new Exception("Invalid Session");
        return sessionData;
    }

    protected string GetClientIpAddress()
    {
        // Check if the request is made through a proxy
        var ipAddress = Request.ServerVariables["HTTP_X_FORWARDED_FOR"];

        // If no proxy, get the user's IP address directly
        if (string.IsNullOrEmpty(ipAddress))
            ipAddress = Request.ServerVariables["REMOTE_ADDR"];

        return ipAddress;
    }
        
    protected void HandleControllerException(Exception exception)
    {
        // Clear the ModelState
        ModelState.Clear();

        if (exception.GetType() == typeof(DbEntityValidationException))
        {
            if (exception is DbEntityValidationException dbEx)
            {
                foreach (var validationErrors in dbEx.EntityValidationErrors)
                {
                    foreach (var validationError in validationErrors.ValidationErrors)
                        ModelState.AddModelError(validationError.PropertyName, validationError.ErrorMessage);
                }
            }
        }

        var errors = ModelState.Values.SelectMany(v => v.Errors).ToList();
        foreach (var error in errors)
            ModelState.AddModelError(error.ErrorMessage, error.ErrorMessage);

        ModelState.AddModelError(string.Empty, exception.Message);

        if (exception.InnerException != null)
        {
            ModelState.AddModelError(exception.InnerException.Message, exception.InnerException.Message);
            exception = exception.InnerException;

            if (exception.InnerException != null)
            {
                ModelState.AddModelError(exception.InnerException.Message, exception.InnerException.Message);
                exception = exception.InnerException;
            }
        }
            
        if (exception.Data.Count > 0)
        {
            var message = string.Empty;
            foreach (DictionaryEntry de in exception.Data)
            {
                if (de.Value is List<Exception> exceptions)
                {
                    message += "\n";
                    foreach (var value in exceptions)
                        message += $"\nError: \"{value.Message}\"";
                }
                else
                {
                    message += $"\n{de.Key} : {de.Value}";
                }
            }
            ModelState.AddModelError(string.Empty, message);
        }

        LogHandler.LogError(exception);
    }
    #endregion
}

public class CompressAttribute : ActionFilterAttribute
{
    public override void OnActionExecuting(ActionExecutingContext filterContext)
    {
        if (null == filterContext.HttpContext.Session[ModelConstants.InstanceId])
        {
            var allowAnonymous = filterContext.ActionDescriptor.GetCustomAttributes(typeof(AllowAnonymousAttribute), false);
            if (allowAnonymous.Length <= 0)
            {
                filterContext.Result =
                    new RedirectToRouteResult(
                        new RouteValueDictionary(new {area = "Admin", controller = "Account", action = "LogOff"}));
                return;
            }
        }

        var encodingsAccepted = filterContext.HttpContext.Request.Headers["Accept-Encoding"];
        if (string.IsNullOrEmpty(encodingsAccepted)) return;

        encodingsAccepted = encodingsAccepted.ToLowerInvariant();
        var response = filterContext.HttpContext.Response;

        if (encodingsAccepted.Contains("deflate"))
        {
            response.AppendHeader("Content-encoding", "deflate");
            response.Filter = new DeflateStream(response.Filter, CompressionMode.Compress);
        }
        else if (encodingsAccepted.Contains("gzip"))
        {
            response.AppendHeader("Content-encoding", "gzip");
            response.Filter = new GZipStream(response.Filter, CompressionMode.Compress);
        }
    }
}
public class CustomAuthorizeAttribute : AuthorizeAttribute
{
    protected override void HandleUnauthorizedRequest(AuthorizationContext filterContext)
    {
        filterContext.Result = new RedirectResult("~/"); // Redirect to the root
    }
}
