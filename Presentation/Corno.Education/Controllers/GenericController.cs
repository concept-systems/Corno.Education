using System.Web.Mvc;

namespace Corno.Education.Controllers;

[Authorize]
public class GenericController : BaseController
{
    protected override void HandleUnknownAction(string actionName)
    {
        View(actionName).ExecuteResult(ControllerContext);
        //base.HandleUnknownAction(actionName);
    }
}