using System;
using System.Linq;
using System.Web.Mvc;
using Corno.Globals.Constants;
using Corno.Services.Core.Interfaces;
using MoreLinq;

namespace Corno.OnlineExam.Areas.Reports.Controllers;

public class ReportController : Controller
{
    #region -- Constructors --
    public ReportController(ICoreService examService)
    {
        _examService = examService;
    }
    #endregion

    #region -- Data  Members --

    private readonly ICoreService _examService;
    #endregion

    #region -- Actions --
    //
    // GET: /Reports/Report/
    public ActionResult Index()
    {
        return View();
    }

    [Authorize]
    public ActionResult Details(string reportName, string description)
    {
        ViewData["ReportName"] = string.IsNullOrEmpty(reportName) ? string.Empty : reportName;
        ViewData["ReportDescription"] = description;

        HttpContext.Session["ReportName"] = reportName;

        return View();
    }

    protected override void HandleUnknownAction(string actionName)
    {
        try
        {
            //ViewData["ReportName"] = reportName;
            View(actionName).ExecuteResult(ControllerContext);
        }
        catch
        {
            Response.Redirect("Page Not Found");
        }
    }
    #endregion

    public JsonResult GetCascadeBundles(int? coursePartId, int? collegeId)
    {
        if (null == collegeId) return null;
        if (coursePartId == null) return null;

        coursePartId = (int)coursePartId;

        int instanceId = Convert.ToInt16(HttpContext.Session[ModelConstants.InstanceId].ToString());
        var regTempRecords = _examService.Tbl_APP_TEMP_Repository.Get()
            .Where(p => p.Num_FK_INST_NO == instanceId && p.Num_FK_COPRT_NO == coursePartId &&
                        p.Num_FK_COLLEGE_CD == collegeId).AsEnumerable().DistinctBy(p => p.Chr_BUNDAL_NO).ToList();

        return Json(regTempRecords.Select(p => new { ID = p.Chr_BUNDAL_NO, Name = p.Chr_BUNDAL_NO }).ToList(), JsonRequestBehavior.AllowGet);
    }
}