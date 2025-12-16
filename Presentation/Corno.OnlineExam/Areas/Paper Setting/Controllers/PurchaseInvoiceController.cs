using System;
using System.Web.Mvc;
using Corno.Data.Corno;
using Corno.Data.Corno.Question_Bank;
using Corno.Globals;
using Corno.OnlineExam.Attributes;
using Corno.OnlineExam.Controllers;
using Corno.Reports.Enrollment;
using Corno.Services.Corno.Question_Bank.Interfaces;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;

namespace Corno.OnlineExam.Areas.Paper_Setting.Controllers;

[Authorize]
public class PurchaseInvoiceController : CornoController
{
    #region -- Constructors --
    public PurchaseInvoiceController(IPurchaseInvoiceService purchaseInvoiceService)
    {
        _purchaseInvoiceService = purchaseInvoiceService;

        _createPath = "~/Areas/paper setting/views/PurchaseInvoice/Create.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly IPurchaseInvoiceService _purchaseInvoiceService;

    private readonly string _createPath;
    #endregion

    #region -- Actions --
    [Authorize]
    public ActionResult Create()
    {
        var sessionData = Session[User.Identity.Name] as SessionData;
        var purchaseInvoice = new PurchaseInvoice
        {
            InstanceId = sessionData?.InstanceId ?? 0,
            CollegeId = sessionData?.CollegeId,
        };

        return View(purchaseInvoice);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [MultipleButton(Name = "action", Argument = "Save")]
    public ActionResult Create(PurchaseInvoice purchaseInvoice)
    {
        if (!ModelState.IsValid)
            return View(_createPath, purchaseInvoice);

        try
        {
            // Validate fields
            _purchaseInvoiceService.ValidateFields(purchaseInvoice);

            // Add or update PurchaseInvoice
            _purchaseInvoiceService.Save(purchaseInvoice);

            TempData["Success"] = "Saved successfully.";
            purchaseInvoice.PurchaseInvoiceDetails.Clear();

            /*ModelState.Clear();
            return RedirectToAction("Create", new {  });*/
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, purchaseInvoice);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "GetStaffs")]
    public ActionResult GetStaffs(PurchaseInvoice purchaseInvoice)
    {
        if (!ModelState.IsValid || null == purchaseInvoice)
            return View(_createPath, purchaseInvoice);

        try
        {
            // Get all PurchaseInvoice subjects
            purchaseInvoice = _purchaseInvoiceService.GetStaffs(purchaseInvoice);

            /*ModelState.Clear();
            return View(_createPath, purchaseInvoice);*/
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_createPath, purchaseInvoice);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "ShowReport")]
    public ActionResult ShowReport(PurchaseInvoice model, string type)
    {
        if (!ModelState.IsValid)
            return View(_createPath, model);

        try
        {
            // Validate header
            _purchaseInvoiceService.ValidateReportHeader(model);

            //Session[ModelConstants.Report] = Bootstrapper.GetReport(typeof(TheoryLetterRpt));
            /*switch (type)
            {
                case ReportName.TheoryLetter:
                    Session[ModelConstants.Report] = new TheoryLetterRpt(model);
                    break;
                case ReportName.PracticalLetter:
                    Session[ModelConstants.Report] = new PracticalLetterRpt(model);
                    break;
                case ReportName.ManuscriptLetter:
                    Session[ModelConstants.Report] = new ManuscriptLetterRpt(model);
                    break;
                case ReportName.ModeratorLetter:
                    Session[ModelConstants.Report] = new ModeratorLetterRpt(model);
                    break;
                case ReportName.AlternativeLetter:
                    Session[ModelConstants.Report] = new AlternateExaminerLetterRpt(model);
                    break;

                case ReportName.AppointedExaminers:
                    Session[ModelConstants.Report] = new AppointedExaminersRpt(model);
                    break;
                case ReportName.ManuscriptExaminers:
                    Session[ModelConstants.Report] = new ManuscriptExaminersRpt(model);
                    break;
                case ReportName.SubjectStickers:
                    Session[ModelConstants.Report] = new ManuscriptExaminersRpt(model);
                    break;
            }*/
            
            return RedirectToAction("Details", "Report", new { area = "Reports", reportName = nameof(EnrollmentRpt), description = string.Empty });
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_createPath, model);
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