using System.Web.Mvc;

namespace Corno.Education.Controllers;

public interface IImageBrowserController : IFileBrowserController
{
    ActionResult Thumbnail(string path);
}