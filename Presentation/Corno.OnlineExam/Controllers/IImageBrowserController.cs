using System.Web.Mvc;

namespace Corno.OnlineExam.Controllers;

public interface IImageBrowserController : IFileBrowserController
{
    ActionResult Thumbnail(string path);
}