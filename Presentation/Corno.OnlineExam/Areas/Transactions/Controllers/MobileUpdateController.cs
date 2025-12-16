using Corno.OnlineExam.Controllers;
using System;
using System.Linq;
using System.Web.Mvc;
using Corno.Data.Corno;
using Corno.Logger;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;

namespace Corno.OnlineExam.Areas.Transactions.Controllers;

[Authorize]
public class MobileUpdateController : BaseController
{
    #region -- Data Members --

    private readonly ICoreService _coreService;
    private readonly ILinkService _linkService;

    #endregion

    #region -- Constructors --
    public MobileUpdateController(ICoreService examService, ILinkService linkService)
    {
        _coreService = examService;
        _linkService = linkService;
    }
    #endregion

    #region -- Methods --
    private void PrnChange(MobileUpdateViewModel viewModel)
    {
        var studentInfo = _coreService.Tbl_STUDENT_INFO_ADR_Repository.Get(s =>
            s.Chr_FK_PRN_NO == viewModel.PrnNo).FirstOrDefault();
        if (null == studentInfo)
            throw new Exception("Student with this prn no is not available.");

        viewModel.Name = ExamServerHelper.GetStudentName(viewModel.PrnNo, _coreService);
        viewModel.Mobile = studentInfo.Num_MOBILE?.Trim();
        viewModel.Email = studentInfo.Chr_Student_Email?.Trim();
        viewModel.Abc = studentInfo.Chr_Abc?.Trim();
    }
    #endregion

    #region -- Actions --
    // GET: /MobileUpdate/Create
    [Authorize]
    public ActionResult Create()
    {
        return View(new MobileUpdateViewModel());
    }

    // POST: /MobileUpdate/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public ActionResult Create(MobileUpdateViewModel viewModel, string submitType)
    {
        try
        {
            if (submitType == "Search")
            {
                if (string.IsNullOrEmpty(viewModel.PrnNo))
                    throw new Exception("Error: Invalid PRN No");
                PrnChange(viewModel);

                ModelState.Clear();
                return View(viewModel);
            }

            if (!ModelState.IsValid)
                return View(viewModel);

            /*using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
            {*/
            try
            {
                var studentInfo = _coreService.Tbl_STUDENT_INFO_ADR_Repository.FirstOrDefault(s =>
                    s.Chr_FK_PRN_NO == viewModel.PrnNo, p => p);
                if (null == studentInfo)
                    throw new Exception("Student with this prn no is not available.");

                //LogHandler.LogInfo("Student found");
                studentInfo.Num_MOBILE = viewModel.Mobile?.Trim();
                studentInfo.Chr_Student_Email = viewModel.Email?.Trim();
                studentInfo.Chr_Abc = viewModel.Abc?.Trim();

                _coreService.Tbl_STUDENT_INFO_ADR_Repository.Update(studentInfo);
                _coreService.Save();

                _linkService.UpdateMobile(studentInfo.Chr_FK_PRN_NO, viewModel.Mobile?.Trim(), viewModel.Email?.Trim());

                //scope.Complete();

                TempData["Success"] = "Mobile Update form submitted successfully.";
            }
            catch (Exception exception)
            {
                //scope.Dispose();
                LogHandler.LogInfo(exception);
                throw;
            }
            //}

            return RedirectToAction("Create");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            LogHandler.LogInfo(LogHandler.GetDetailException(exception));
            //ModelState.Clear();
        }

        PrnChange(viewModel);
        return View(viewModel);
    }

    #endregion
}