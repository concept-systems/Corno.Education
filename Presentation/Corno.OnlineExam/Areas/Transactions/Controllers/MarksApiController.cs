using System;
using System.Web.Mvc;
using Corno.Data.ViewModels;
using Corno.Globals;
using Corno.Services.Core.Interfaces;
using Corno.OnlineExam.Controllers;
using Corno.Services.Core.Marks_Entry.Interfaces;
using Corno.Services.Corno.Interfaces;
using Corno.OnlineExam.Attributes;

namespace Corno.OnlineExam.Areas.Transactions.Controllers;

[Authorize]
public class MarksApiController : UniversityController
{
    #region -- Constructors --
    public MarksApiController(ICornoService cornoService, ICoreService coreService, IMarksApiService marksApiService)
    {
        _coreService = coreService;
        _marksApiService = marksApiService;

        _viewPath = "~/Areas/transactions/views/MarksApi/Create.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly ICoreService _coreService;
    private readonly IMarksApiService _marksApiService;

    private readonly string _viewPath;
    #endregion
        
    #region -- Actions --
    // GET: /Reg/Create
    [Authorize]
    public ActionResult Create()
    {
        var marksApiViewModel = new MarksApiViewModel();
        var sessionData = Session[User.Identity.Name] as SessionData;

        marksApiViewModel.InstanceId = sessionData?.InstanceId;
            
        return View(_viewPath, marksApiViewModel);
    }

    /*[HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public ActionResult Create(MarksEntry model, string submitType)
    {
        try
        {
            if (ModelState.IsValid)
            {
                _marksEntryService.Save(model);

                TempData["Success"] = "Marks Saved Successfully";

                return RedirectToAction("Create");
            }
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            model.Error = exception.Message;
        }

        if (null != model.UploadedFile)
        {
            model.MarksEntryDetails?.Clear();
            model.UploadedFile = null;
        }

        model.bEnable = false;
        return View(model);
    }*/

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "ImportFromMarksApi")]
    public ActionResult ImportFromMarksApi(MarksApiViewModel marksApiViewModel)
    {
        if (!ModelState.IsValid)
            return View(_viewPath, marksApiViewModel);

        try
        {
            // Import from Marks API
            var result = _marksApiService.ImportFromMarksApi(marksApiViewModel);
            
            // Set imported data for display
            marksApiViewModel.ImportedMarks = result.ImportedMarks;
            
            TempData["Success"] = result.Message;
            TempData["Info"] = $"Total Fetched: {result.TotalRecordsFetched}, Inserted: {result.TotalRecordsInserted}, Updated: {result.TotalRecordsUpdated}";
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            marksApiViewModel.ImportedMarks = new System.Collections.Generic.List<MarksApiDetailDto>();
        }

        return View(_viewPath, marksApiViewModel);
    }

    #endregion
}