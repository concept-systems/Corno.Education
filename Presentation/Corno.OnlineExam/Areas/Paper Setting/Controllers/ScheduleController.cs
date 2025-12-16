using System;
using System.Web.Mvc;
using Corno.Data.Corno.Paper_Setting.Models;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.OnlineExam.Areas.Paper_Setting.Reports.Schedule;
using Corno.OnlineExam.Attributes;
using Corno.OnlineExam.Controllers;
using Corno.Reports.Enrollment;
using Corno.Services.Corno.Paper_Setting.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Corno.OnlineExam.Areas.Paper_Setting.Controllers;

[Authorize]
public class ScheduleController : CornoController
{
    #region -- Constructors --
    public ScheduleController(IScheduleService scheduleService)
    {
        _scheduleService = scheduleService;

        _viewPath = "~/Areas/paper setting/views/Schedule/Create.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly IScheduleService _scheduleService;

    private readonly string _viewPath;
    #endregion

    #region -- Private Methods --
        
    #endregion

    #region -- Actions --
    [Authorize]
    public ActionResult Create()
    {
        var sessionData = Session[User.Identity.Name] as SessionData;
        var schedule = new Schedule
        {
            InstanceId = sessionData?.InstanceId ?? 0,
            CollegeId = sessionData?.CollegeId,
        };

        return View(schedule);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [MultipleButton(Name = "action", Argument = "Save")]
    public ActionResult Create(Schedule schedule)
    {
        if (!ModelState.IsValid)
            return View(_viewPath, schedule);

        try
        {
            // Validate fields
            _scheduleService.ValidateFields(schedule);

            // Add or update schedule
            _scheduleService.Save(schedule);

            TempData["Success"] = "Saved successfully.";

            ModelState.Clear();
            return RedirectToAction("Create", new {  });
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_viewPath, schedule);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "GetAllSubjects")]
    public ActionResult GetAllSubjects(Schedule schedule)
    {
        if (!ModelState.IsValid || null == schedule)
            return View(_viewPath, schedule);

        try
        {
            // Get all schedule subjects
            schedule = _scheduleService.GetAllSubjects(schedule);

            //schedule.EnableHeader = false;

            return View(_viewPath, schedule);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        //schedule.EnableHeader = true;
        return View(_viewPath, schedule);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "ShowReport")]
    public ActionResult ShowReport(Schedule model, string type)
    {
        if (!ModelState.IsValid)
            return View(_viewPath, model);

        try
        {
            // Validate header
            _scheduleService.ValidateReportHeader(model);

            switch (type)
            {
                case ReportName.Schedule:
                    Session[ModelConstants.Report] = new ScheduleRpt(model);
                    break;
                /*case ReportName.ScheduleForPaperSetting:
                    Session[ModelConstants.Report] = new ScheduleForPaperSettingRpt(model);
                    break;*/
                case ReportName.ScheduledExaminers:
                    Session[ModelConstants.Report] = new ScheduledExaminersRpt(model);
                    break;
            }

            return RedirectToAction("Details", "Report", new { area = "Reports", reportName = nameof(EnrollmentRpt), description = string.Empty });
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_viewPath, model);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "ClearControls")]
    public ActionResult ClearControls(Schedule model)
    {
        try
        {
            model.ScheduleDetails.Clear();

            ModelState.Clear();
            return View(_viewPath, model);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        ModelState.Clear();
        return View(_viewPath, model);
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Inline_Create_Update_Destroy([DataSourceRequest] DataSourceRequest request, ScheduleDetail model)
    {
        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    }

    /*[HttpPost]*/
    public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
    {
        var fileContents = Convert.FromBase64String(base64);

        return File(fileContents, contentType, fileName);
    }
    #endregion
}