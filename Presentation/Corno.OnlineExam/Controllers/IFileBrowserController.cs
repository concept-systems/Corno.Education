using System.Web;
using System.Web.Mvc;
using Kendo.Mvc.UI;

namespace Corno.OnlineExam.Controllers;

public interface IFileBrowserController
{
    JsonResult Read(string path);
    ActionResult Destroy(string path, FileBrowserEntry entry);
    ActionResult Create(string path, FileBrowserEntry entry);
    ActionResult Upload(string path, HttpPostedFileBase file);
}