using System;
using System.Linq;
using System.Web.Mvc;
using Corno.Data.Corno;
using Corno.Data.ViewModels;
using Corno.Globals;
using Corno.Globals.Enums;
using Corno.OnlineExam.Attributes;
using Corno.OnlineExam.Controllers;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno;
using Corno.Services.Corno.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Corno.OnlineExam.Areas.Transactions.Controllers;

[Authorize]
public class EnrollmentController : BaseController
{
    #region -- Constructors --
    public EnrollmentController(ICornoService cornoService, IUniversityService universityService)
    {
        _cornoService = cornoService;
        _universityService = universityService;
        _linkService = Bootstrapper.Get<ILinkService>();

        _viewPath = "~/Areas/transactions/views/Enrollment/Create.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly ICornoService _cornoService;
    private readonly IUniversityService _universityService;
    private readonly ILinkService _linkService;

    private readonly string _viewPath;
    #endregion

    #region -- Private Methods --
    private static void ValidateFields(Link link)
    {
        if (link.CollegeId <= 0)
            throw new Exception("Invalid College.");
        if (link.CourseId <= 0)
            throw new Exception("Invalid Course.");
        if (link.CoursePartId <= 0)
            throw new Exception("Invalid Course Part.");
        if (link.CollegeId <= 0)
            throw new Exception("Invalid College.");
        if (link.InstanceId <= 0)
            throw new Exception("Invalid instance / session.");

        /*var selectedCount = link.GetSelectedDetails().Count();
        if (selectedCount <= 0)
            throw new Exception("Please, select at least one row.");*/
    }

    private void UpdateSinglePrn(Link link, Link existing)
    {
        var prn = link.NotMapped.Prn;
        //if (string.IsNullOrEmpty(prn) || null == existing) return;
        if (string.IsNullOrEmpty(prn)) return;
        if (null == existing)
        {
            link.LinkDetails.ForEach(d => { d.NotMapped.Hide = true; });
            var linkDetail = link.LinkDetails.FirstOrDefault(d => d.Prn == prn);
            if (null == linkDetail)
                throw new Exception("Entered PRN is not found.");
            linkDetail.NotMapped.Hide = false;
            return;
        }

        // Update PRN
        existing.NotMapped.Prn = prn;

        var prnLinkDetail = existing.LinkDetails.FirstOrDefault(d => d.Prn == prn);
        if (null == prnLinkDetail)
        {
            // Get All Exam Students
            var loginInstanceId = GetSession()?.InstanceId;
            _linkService.GetLinks(link, loginInstanceId ?? 0);
            prnLinkDetail = link.LinkDetails.FirstOrDefault(d => d.Prn == prn);
            if (link.LinkDetails.Count <= 0 || null == prnLinkDetail)
                throw new Exception("PRN not found.");
            existing.LinkDetails.Add(prnLinkDetail);
        }

        existing.LinkDetails.ForEach(d => { d.NotMapped.Hide = true; });
        prnLinkDetail = existing.LinkDetails.FirstOrDefault(d => d.Prn == prn);
        if (null == prnLinkDetail)
            throw new Exception("Entered PRN is not found.");
        prnLinkDetail.NotMapped.Hide = false;
    }

    private void Save(Link link)
    {
        // Save Data.
        var existing = _cornoService.LinkRepository.Get(l => l.Id == link.Id,
            null, nameof(Link.LinkDetails)).FirstOrDefault();
        if (null != existing)
            _linkService.UpdateEnrollment(existing, link);
        else
            _linkService.Add(link);
    }

    private Link GetStudents(Link link, bool backlog)
    {
        var loginInstanceId = GetSession()?.InstanceId;

        var sessionData = GetSession();
        // Check user type
        if (sessionData?.UserType == UserType.College)
            link.CollegeId = sessionData.CollegeId;

        var existing = _linkService.GetExistingLink(link, loginInstanceId ?? 0);
        if (existing != null)
        {
            ModelState.Clear();

            _linkService.UpdateBranches(existing);
            existing.NotMapped.BranchApplicable = link.NotMapped.BranchApplicable;

            // Specific for PRN
            UpdateSinglePrn(link, existing);

            // Set instance Id = previously selected instance Id
            existing.InstanceId = existing.LinkDetails.FirstOrDefault(d => (d.InstanceId ?? 0) > 0)?.InstanceId;

            return existing;
        }

        // Get All Exam Students
        if (backlog)
            _linkService.GetBackLogStudents(link, loginInstanceId ?? 0);
        else
            _linkService.GetLinks(link, loginInstanceId ?? 0);

        if (link.LinkDetails.Count <= 0)
            throw new Exception("No students found");

        // Specific for PRN
        UpdateSinglePrn(link, null);

        // Update branches 
        _linkService.UpdateBranches(link);

        return link;
    }
    #endregion

    #region -- Actions --
    // GET: /Reg/Create
    [Authorize]
    public ActionResult Create(FormType formType)
    {
        var sessionData = Session[User.Identity.Name] as SessionData;
        var link = new Link
        {
            FormTypeId = (int)formType,
            CollegeId = sessionData?.CollegeId,
            NotMapped = new NotMapped
            {
                CollegeName = sessionData?.CollegeName,
            },
        };

        //// TODO: Delete after use
        //link.InstanceId = 70;
        //link.CollegeId = 6;
        //link.CourseTypeId = 4;
        //link.CourseId = 175;
        //link.CoursePartId = 1813;

        return View(link);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [MultipleButton(Name = "action", Argument = "Save")]
    public ActionResult Create(Link link)
    {
        if (!ModelState.IsValid)
            return View(_viewPath, link);

        try
        {
            // Validate fields
            ValidateFields(link);

            if (link.LinkDetails.Count <= 0)
                throw new Exception("No rows in links.");

            // Update selected instance id in link detail
            Enum.TryParse(link.FormTypeId.ToString(), true, out FormType formType);
            link.LinkDetails.ForEach(d =>
            {
                d.InstanceId = (d.InstanceId ?? 0) <= 0 ? link.InstanceId : d.InstanceId;
                d.SerialNo = link.InstanceId ?? 0;
                d.Code = formType.ToString();
            });

            // Update InstanceId as login Instance and save against that instance.
            var sessionData = GetSession();
            // Check user type
            if (sessionData?.UserType == UserType.College)
                link.CollegeId = sessionData.CollegeId;

            link.InstanceId = sessionData?.InstanceId;

            // Save
            Save(link);

            TempData["Success"] = "Saved successfully.";

            ModelState.Clear();
            return RedirectToAction("Create", new { formType = link.FormTypeId });
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_viewPath, link);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "GetAllStudents")]
    public ActionResult GetAllStudents(Link link)
    {
        if (!ModelState.IsValid)
            return View(_viewPath, link);

        try
        {
            var studentLink = GetStudents(link, false);
            if (null != studentLink)
            {
                studentLink.NotMapped ??= new NotMapped();
            }

            return View(_viewPath, studentLink);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        //link.CollegeId = GetSession()?.CollegeId;
        return View(_viewPath, link);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "GetBacklogStudents")]
    public ActionResult GetBacklogStudents(Link link)
    {
        if (!ModelState.IsValid)
            return View(_viewPath, link);

        try
        {
            var studentLink = GetStudents(link, true);
            return View(_viewPath, studentLink);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_viewPath, link);
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Inline_Create_Update_Destroy([DataSourceRequest] DataSourceRequest request, LinkDetail model)
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