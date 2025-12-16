using System.Web;
using System.Web.Mvc;

namespace Corno.OnlineExam.Controllers;

public class ErrorController : Controller
{
    public ActionResult Index()
    {
        return RedirectToAction("GenericError", new HandleErrorInfo(new HttpException(403, "Dont allow access the error pages"), @"Error", "Index"));
    }

    public ViewResult GenericError(HandleErrorInfo exception)
    {
        return View("Error", exception);
    }

    public ViewResult NotFound(HandleErrorInfo exception)
    {
        ViewBag.Title = "Page Not Found";
        return View("Error", exception);
    }
}