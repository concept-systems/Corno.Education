using Corno.Data.Core;
using Corno.Data.Corno;
using Corno.Data.ViewModels;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Globals.Enums;
using Corno.OnlineExam.Attributes;
using Corno.OnlineExam.Controllers;
using Corno.Reports.Enrollment;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;
using Ganss.Excel;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace Corno.OnlineExam.Areas.Transactions.Controllers;

[Authorize]
public class LinkController : BaseController
{
    #region -- Constructors --
    public LinkController(ICornoService cornoService, ICoreService coreService)
    {
        _cornoService = cornoService;
        _coreService = coreService;
        _linkService = Bootstrapper.Get<ILinkService>();

        _viewPath = "~/Areas/transactions/views/Link/Create.cshtml";

        /*// Initialize variables
        Initialize();*/
    }
    #endregion

    #region -- Data Members --

    private readonly ICornoService _cornoService;
    private readonly ICoreService _coreService;
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

    private void UpdateSinglePrn(Link link, string enteredPrn)
    {
        if (string.IsNullOrEmpty(enteredPrn)) return;

        link.LinkDetails.ForEach(d => { d.NotMapped.Hide = true; });
        var prnLinkDetail = link.LinkDetails.FirstOrDefault(d => d.Prn == enteredPrn);
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
            _linkService.UpdateLinks(existing, link);
        else
            _linkService.Add(link);
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
            InstanceId = sessionData?.InstanceId,
            CollegeId = sessionData?.CollegeId,
            NotMapped = new NotMapped
            {
                CollegeName = sessionData?.CollegeName
            }
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
    [ValidateAntiForgeryToken]
    [Authorize]
    [MultipleButton(Name = "action", Argument = "Send")]
    public ActionResult Send(Link link)
    {
        if (!ModelState.IsValid)
            return View(_viewPath, link);

        try
        {
            // Validate fields
            ValidateFields(link);

            // Extra validation. This will validate selected rows.
            //link.GetSelectedDetails();

            var sessionData = GetSession();
            // Check user type
            if (sessionData?.UserType == UserType.College)
                link.CollegeId = sessionData.CollegeId;

            // Send SMS & Emails
            _linkService.SendSmsAndEmail(link);

            // Save
            Save(link);

            TempData["Success"] = "Links sent successfully.";

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
            var sessionData = GetSession();
            // Check user type
            if (sessionData?.UserType == UserType.College)
                link.CollegeId = sessionData.CollegeId;
            link.InstanceId = sessionData?.InstanceId;

            var loginInstanceId = GetSession()?.InstanceId;
            var existing = _linkService.GetExistingLink(link, loginInstanceId ?? 0);
            if (existing == null)
                throw new Exception("Enrollment is not created for selected course part.");

            ModelState.Clear();
            foreach (var detail in existing.LinkDetails.Where(detail => detail.Status == StatusConstants.Sent))
                detail.Selected = true;

            _linkService.UpdateBranches(existing);
            existing.NotMapped.BranchApplicable = link.NotMapped.BranchApplicable;

            // Specific for PRN
            UpdateSinglePrn(existing, link.NotMapped.Prn);

            return View(_viewPath, existing);
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
    [MultipleButton(Name = "action", Argument = "ShowReport")]
    public ActionResult ShowReport(Link link)
    {
        if (!ModelState.IsValid)
            return View(_viewPath, link);

        try
        {
            var existing = _cornoService.LinkRepository.Get(l => l.InstanceId == link.InstanceId &&
                                                                 l.CollegeId == link.CollegeId && l.CourseId == link.CourseId && l.CoursePartId == link.CoursePartId).FirstOrDefault();
            if (null == existing)
                throw new Exception("Enrollments are not created for selected college and course part.");
            Session[ModelConstants.Report] = new EnrollmentRpt(existing);
            return RedirectToAction("Details", "Report", new { area = "Reports", reportName = nameof(EnrollmentRpt), description = string.Empty });
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
    [MultipleButton(Name = "action", Argument = "GetResentLinks")]
    public ActionResult GetResentLinks(Link link)
    {
        if (!ModelState.IsValid)
            return View(_viewPath, link);

        try
        {
            var loginInstanceId = GetSession()?.InstanceId;
            var existing = _linkService.GetExistingLink(link, loginInstanceId ?? 0);
            if (existing != null)
            {
                ModelState.Clear();
                foreach (var detail in existing.LinkDetails.Where(detail => detail.Status == StatusConstants.Sent))
                    detail.Selected = true;
                return View(_viewPath, existing);
            }

            if (Session[User.Identity.Name] is not SessionData sessionData)
                throw new Exception("Invalid Session");
            link.CollegeId = sessionData.CollegeId;
            /*var content = _apiService.Post("GetExamStudents", link, ApiName.Core);
            var examLinkViewModels =
                (JsonConvert.DeserializeObject<List<ExamLinkViewModel>>(content.ToString()) ??
                 throw new InvalidOperationException())
                .ToList();
            link.LinkDetails.Clear();
            foreach (var viewModel in examLinkViewModels)
            {
                var examLinkDetail = new ExamLinkDetail();
                viewModel.CopyPropertiesTo(examLinkDetail);
                examLinkDetail.Selected = true;
                examLinkDetail.NotMapped.StudentName = viewModel.StudentName;
                examLinkDetail.NotMapped.CoursePartName = viewModel.CoursePartName;
                link.LinkDetails.Add(examLinkDetail);
            }*/
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_viewPath, link);
    }

    /*public ActionResult Excel_Export_Links(string contentType, string base64, string fileName, int linkId)
    {
        // Get link with details
        var link = _cornoService.LinkRepository.Get(l => l.Id == linkId)?.FirstOrDefault();
        //var prnList = link.LinkDetails.Select(y => y.Prn).Distinct().ToList();

        var index = 0;
        var prnList = link?.LinkDetails.Select(y => y.Prn).Distinct().ToList() ?? [];
        var students = _coreService.Tbl_STUDENT_INFO_Repository.GetAll()
            .Where(s => prnList.Contains(s.Chr_PK_PRN_NO)).ToList();
        var appTemps = _coreService.Tbl_APP_TEMP_Repository.Get(p =>
            p.Num_FK_INST_NO == link.InstanceId && prnList.Contains(p.Chr_APP_PRN_NO)).ToList();
        var convocations = _coreService.Tbl_STUDENT_CONVO_Repository.Get(e =>
            prnList.Contains(e.Chr_FK_PRN_NO)).ToList();
        var envStudies = _coreService.TBl_STUDENT_ENV_STUDIES_Repository.Get(e =>
            prnList.Contains(e.Chr_FK_PRN_NO)).ToList();
        var records = from linkDetail in link?.LinkDetails
                      join branch in _coreService.Tbl_BRANCH_MSTR_Repository.Get()
                          on linkDetail.BranchId equals branch.Num_PK_BR_CD into branchJoin
                      join student in students on linkDetail.Prn equals student.Chr_PK_PRN_NO
                      select new
                      {
                          SerialNo = index++,
                          PRN = linkDetail.Prn,
                          StudentName = student.Var_ST_NM,
                          Branch = linkDetail.BranchId > 0 ? $"({linkDetail.BranchId}) {branchJoin.FirstOrDefault()?.Var_BR_SHRT_NM}" : string.Empty,
                          linkDetail.Mobile,
                          Email = linkDetail.EmailId,
                          SentDate = linkDetail.SentDate.HasValue ? linkDetail.SentDate.Value.ToString("dd-MMM-yyyy") : string.Empty,
                          TransactionId = appTemps.FirstOrDefault(p => p.Chr_APP_PRN_NO == linkDetail.Prn)?.Num_Transaction_Id,
                          PaymentDate = appTemps.FirstOrDefault(p => p.Chr_APP_PRN_NO == linkDetail.Prn)?.PaymentDate?.ToString("dd-MMM-yyyy")
                      };


        using var stream = new MemoryStream();
        var excelMapper = new ExcelMapper();

        excelMapper.Save(stream, records);
        stream.Position = 0;

        return File(stream.ToArray(), contentType, fileName);
    }*/

    public ActionResult Excel_Export_Links(string contentType, string base64, string fileName, int linkId)
    {
        // Get link with details
        var link = _cornoService.LinkRepository.Get(l => l.Id == linkId)?.FirstOrDefault();

        var index = 0;
        var prnList = link?.LinkDetails.Select(y => y.Prn).Distinct().ToList() ?? new List<string>();

        var students = _coreService.Tbl_STUDENT_INFO_Repository.GetAll()
            .Where(s => prnList.Contains(s.Chr_PK_PRN_NO)).ToList();

        var appTemps = _coreService.Tbl_APP_TEMP_Repository.Get(p =>
            p.Num_FK_INST_NO == link.InstanceId && prnList.Contains(p.Chr_APP_PRN_NO)).ToList();

        var convocations = _coreService.Tbl_STUDENT_CONVO_Repository.Get(e =>
            prnList.Contains(e.Chr_FK_PRN_NO)).ToList();

        var envStudies = _coreService.TBl_STUDENT_ENV_STUDIES_Repository.Get(e =>
            prnList.Contains(e.Chr_FK_PRN_NO)).ToList();

        var exams = appTemps; // Assuming exams come from Tbl_APP_TEMP for FormType.Exam

        var records = from linkDetail in link?.LinkDetails
                      join branch in _coreService.Tbl_BRANCH_MSTR_Repository.Get()
                          on linkDetail.BranchId equals branch.Num_PK_BR_CD into branchJoin
                      join student in students on linkDetail.Prn equals student.Chr_PK_PRN_NO
                      select new
                      {
                          SerialNo = index++,
                          PRN = linkDetail.Prn,
                          StudentName = student.Var_ST_NM,
                          Branch = linkDetail.BranchId > 0 ? $"({linkDetail.BranchId}) {branchJoin.FirstOrDefault()?.Var_BR_SHRT_NM}" : string.Empty,
                          linkDetail.Mobile,
                          Email = linkDetail.EmailId,
                          SentDate = linkDetail.SentDate.HasValue ? linkDetail.SentDate.Value.ToString("dd-MMM-yyyy") : string.Empty,

                          // Dynamic TransactionId & PaymentDate based on FormTypeId
                          TransactionId = GetTransactionId(link.FormTypeId, linkDetail.Prn, exams, convocations, envStudies),
                          PaymentDate = GetPaymentDate(link.FormTypeId, linkDetail.Prn, exams, convocations, envStudies)
                      };

        using var stream = new MemoryStream();
        var excelMapper = new ExcelMapper();
        excelMapper.Save(stream, records);
        stream.Position = 0;

        return File(stream.ToArray(), contentType, fileName);
    }

    // Helper methods
    private string GetTransactionId(int? formTypeId, string prn,
        List<Tbl_APP_TEMP> exams,
        List<Tbl_STUDENT_CONVO> convocations,
        List<TBl_STUDENT_ENV_STUDIES> envStudies)
    {
        switch (formTypeId)
        {
            case (int?)FormType.Exam:
                return exams.FirstOrDefault(p => p.Chr_APP_PRN_NO == prn)?.Num_Transaction_Id?.ToString();
            case (int?)FormType.Convocation:
                return convocations.FirstOrDefault(p => p.Chr_FK_PRN_NO == prn)?.Chr_Transaction_Id;
            case (int?)FormType.Environment:
                return envStudies.FirstOrDefault(p => p.Chr_FK_PRN_NO == prn)?.Chr_Transaction_Id;
            default:
                return string.Empty;
        }
    }

    private string GetPaymentDate(int? formTypeId, string prn,
        List<Tbl_APP_TEMP> exams,
        List<Tbl_STUDENT_CONVO> convocations,
        List<TBl_STUDENT_ENV_STUDIES> envStudies)
    {
        switch (formTypeId)
        {
            case (int?)FormType.Exam:
                return exams.FirstOrDefault(p => p.Chr_APP_PRN_NO == prn)?.PaymentDate?.ToString("dd-MMM-yyyy");
            case (int?)FormType.Convocation:
                return convocations.FirstOrDefault(p => p.Chr_FK_PRN_NO == prn)?.PaymentDate?.ToString("dd-MMM-yyyy");
            case (int?)FormType.Environment:
                return envStudies.FirstOrDefault(p => p.Chr_FK_PRN_NO == prn)?.PaymentDate?.ToString("dd-MMM-yyyy");
            default:
                return string.Empty;
        }
    }

    [AcceptVerbs(HttpVerbs.Post)]
    public ActionResult Inline_Create_Update_Destroy([DataSourceRequest] DataSourceRequest request, LinkDetail model)
    {
        return Json(new[] { model }.ToDataSourceResult(request, ModelState));
    }
    #endregion
}