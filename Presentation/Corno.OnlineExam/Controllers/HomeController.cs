using System.Web.Mvc;

namespace Corno.OnlineExam.Controllers;

[Authorize]
public class HomeController : BaseController
{
    public ActionResult Index()
    {
        //if (User.IsInRole(ModelConstants.College) && (int) HttpContext.Session[ModelConstants.CollegeId] > 0)
        //{
        //    //var user = UserManager.FindById(User.Identity.GetUserId());
        //    // TO DO - Umesh - Change it to Exam Server.
        //    HttpContext.Session[ModelConstants.CollegeName] =
        //        ExamServerHelper.GetCollegeName(Convert.ToInt32(HttpContext.Session[ModelConstants.CollegeId]),
        //            _examService);
        //}
        //else
        //    HttpContext.Session[ModelConstants.CollegeName] = string.Empty;

        //HttpContext.Session[ModelConstants.InstanceName] =
        //    _masterService.InstanceRepository.GetById(HttpContext.Session[ModelConstants.InstanceId]).Name;

        //if (this.ControllerContext.HttpContext.Request.Cookies[".AspNet.ApplicationCookie"] != null)
        //{
        //    HttpCookie cookie = this.ControllerContext.HttpContext.Request.Cookies[".AspNet.ApplicationCookie"];
        //    //Here, we are setting the time to a previous time.
        //    //When the browser detect it next time, it will be deleted automatically.
        //    cookie.Expires = DateTime.Now.AddDays(-1);
        //    this.ControllerContext.HttpContext.Response.Cookies.Add(cookie);
        //}

        return View();
    }

    public ActionResult About()
    {
        ViewBag.Message = "The complete business portal";

        return View();
    }

    public ActionResult Contact()
    {
        return View();
    }
}