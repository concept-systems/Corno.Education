using Corno.Globals.Constants;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Transactions;
using System.Web.Mvc;
using Corno.Data.Core;
using Corno.Data.Corno;
using Corno.Data.Helpers;
using Corno.Data.Import;
using Corno.Globals;
using Corno.Logger;
using Corno.Services.Core.Interfaces;
using Corno.OnlineExam.Controllers;
using Corno.Reports.Marks_Entry;
using Corno.Services.File.Interfaces;
using Microsoft.AspNet.Identity;
using MoreLinq;
using System.Web;
using Corno.Globals.Enums;

namespace Corno.OnlineExam.Areas.Transactions.Controllers;

[Authorize]
public class MarksEntryController : UniversityController
{
    #region -- Constructors --
    public MarksEntryController(ICoreService coreService, IExcelFileService<MarksImportModel> excelFileService)
    {
        _coreService = coreService;
        _excelFileService = excelFileService;
    }
    #endregion

    #region -- Data Members --

    private readonly ICoreService _coreService;
    private readonly IExcelFileService<MarksImportModel> _excelFileService;

    #endregion

    #region -- Methods --

    private List<MarksEntryDetail> GetDetails(MarksEntry model)
    {
        model ??= new MarksEntry();

        var instanceId = HttpContext.Session[ModelConstants.InstanceId].ToInt();
        List<MarksEntryDetail> marksEntryDetails;
        if (model.PaperId.ToInt() <= 0)
        {
            var category = _coreService.Tbl_SUBJECT_CAT_MSTR_Repository.Get(c => c.Num_FK_COPRT_NO == model.CoursePartId &&
                    c.Num_FK_SUB_CD == model.SubjectId && c.Num_FK_CAT_CD == model.CategoryId)
                .FirstOrDefault();
            marksEntryDetails = (from exam in _coreService.Tbl_STUDENT_SUBJECT_Repository.Get(e =>
                    e.Num_FK_INST_NO == instanceId && e.Num_FK_COPRT_NO == (model.CoursePartId ?? 0) && e.Num_FK_SUB_CD == model.SubjectId &&
                    e.Chr_ST_SUB_STS == "A" && e.Chr_ST_SUB_CAN != "Y"/*&& (e.Num_FK_BR_CD ?? 0) == (model.BranchId ?? 0)*/)
                                 join categoryMarks in _coreService.Tbl_STUDENT_CAT_MARKS_Repository.Get(c =>
                                         c.Num_FK_INST_NO == instanceId && c.Num_FK_SUB_CD == model.SubjectId && c.Num_FK_CAT_CD == model.CategoryId &&
                                         c.Var_ST_PH_STS == "A")
                                     on exam.Chr_FK_PRN_NO equals categoryMarks.Var_FK_PRN_NO
                                 join yrChange in _coreService.Tbl_STUDENT_YR_CHNG_Repository.Get(y => y.Num_FK_INST_NO == model.InstanceId &&
                                         y.Num_FK_COL_CD == model.CollegeId && (y.Num_FK_DistCenter_ID ?? 0) == (model.CentreId ?? 0))
                                     on exam.Chr_FK_PRN_NO equals yrChange.Chr_FK_PRN_NO
                                 select new MarksEntryDetail
                                 {
                                     Prn = exam.Chr_FK_PRN_NO,
                                     SeatNo = yrChange.Num_ST_SEAT_NO,
                                     MaxMarks = category.Num_CAT_MAX_MRK
                                 }).ToList();
        }
        else
        {
            var paper = _coreService.Tbl_SUB_CATPAP_MSTR_Repository.Get(c => c.Num_FK_COPRT_NO == model.CoursePartId &&
                                                                             c.Num_FK_SUB_CD == model.SubjectId && c.Num_FK_CAT_CD == model.CategoryId && c.Num_PK_PAP_CD == model.PaperId).FirstOrDefault();
            marksEntryDetails = (from exam in _coreService.Tbl_STUDENT_SUBJECT_Repository.Get(e =>
                    e.Num_FK_INST_NO == instanceId && e.Num_FK_COPRT_NO == (model.CoursePartId ?? 0) && e.Num_FK_SUB_CD == model.SubjectId &&
                    e.Chr_ST_SUB_STS == "A" && e.Chr_ST_SUB_CAN != "Y")
                                 join paperMarks in _coreService.Tbl_STUDENT_PAP_MARKS_Repository.Get(c =>
                                     c.Num_FK_INST_NO == instanceId && c.Num_FK_SUB_CD == model.SubjectId &&
                                     c.Num_FK_CAT_CD == model.CategoryId && c.Num_FK_PAP_CD == model.PaperId &&
                                     c.Var_ST_PH_STS == "A") on exam.Chr_FK_PRN_NO equals paperMarks.Var_FK_PRN_NO
                                 join yrChange in _coreService.Tbl_STUDENT_YR_CHNG_Repository.Get(y => y.Num_FK_INST_NO == model.InstanceId &&
                                         y.Num_FK_COL_CD == model.CollegeId && (y.Num_FK_DistCenter_ID ?? 0) == (model.CentreId ?? 0))
                                     on exam.Chr_FK_PRN_NO equals yrChange.Chr_FK_PRN_NO
                                 select new MarksEntryDetail
                                 {
                                     Prn = exam.Chr_FK_PRN_NO,
                                     SeatNo = yrChange.Num_ST_SEAT_NO,
                                     MaxMarks = paper.Num_PAP_MAX_MRK
                                 }).ToList();
        }

        return marksEntryDetails.DistinctBy(d => d.Prn).ToList();
    }

    private void GetFormSeatNos(MarksEntry model)
    {
        var marksEntryDetails = GetDetails(model);

        var marksTemps = _coreService.Tbl_MARKS_TMP_Repository.Get(t => t.Num_FK_INST_NO == model.InstanceId &&
                                                                        t.Num_FK_COL_CD == (model.CollegeId ?? 0) && t.Chr_FK_COPRT_NO ==
                                                                        model.CoursePartId.ToString() && t.Chr_FK_SUB_CD == model.SubjectId.ToString() &&
                                                                        t.Chr_FK_CAT_CD == (model.CategoryId ?? 0).ToString() && t.Chr_FK_PAP_CD == (model.PaperId ?? 0).ToString())
            .Select(m => m.Chr_CODE_SEAT_NO).ToList();

        marksEntryDetails = marksEntryDetails.Where(m => !marksTemps.Contains(m.SeatNo ?? 0)).ToList();

        if (marksEntryDetails.Count <= 0)
            throw new Exception("No students found for marks entry.");

        model.MarksEntryDetails.AddRange(marksEntryDetails);
    }

    private void GetReportSeatNos(MarksEntry model)
    {
        var marksEntryDetails = GetDetails(model);

        var marksTemps = _coreService.Tbl_MARKS_TMP_Repository.Get(t => t.Num_FK_INST_NO == model.InstanceId &&
                                                                        t.Num_FK_COL_CD == (model.CollegeId ?? 0) && t.Chr_FK_COPRT_NO ==
                                                                        model.CoursePartId.ToString() && t.Chr_FK_SUB_CD == model.SubjectId.ToString() &&
                                                                        t.Chr_FK_CAT_CD == (model.CategoryId ?? 0).ToString() &&
                                                                        t.Chr_FK_PAP_CD == (model.PaperId ?? 0).ToString() &&
                                                                        t.Var_CHKLIST_FLG == "A" && t.Num_Application_No == 1)
            .Select(m => new { m.Chr_CODE_SEAT_NO, m.Chr_MARKS, m.Chr_STATUS_FLG }).ToList();

        foreach (var marksEntryDetail in marksEntryDetails)
        {
            var tempDetail = marksTemps.FirstOrDefault(t => t.Chr_CODE_SEAT_NO == marksEntryDetail.SeatNo);
            if (null == tempDetail) continue;

            marksEntryDetail.Status = tempDetail.Chr_STATUS_FLG;
            marksEntryDetail.Marks = tempDetail.Chr_MARKS;
        }

        model.MarksEntryDetails.AddRange(marksEntryDetails);
    }

    private void UpdateUserDetails(MarksEntry marksEntry)
    {
        var userId = User.Identity.GetUserId();
        marksEntry.ModifiedBy = userId;
        marksEntry.ModifiedDate = DateTime.Now;

        marksEntry.MarksEntryDetails.ForEach(t => t.ModifiedBy = userId);
        marksEntry.MarksEntryDetails.ForEach(t => t.ModifiedDate = DateTime.Now);
    }

    private void AddInExamServer(MarksEntry model)
    {
        if (null == model.CoursePartId)
            throw new Exception("Invalid course part.");

        var instanceId = HttpContext.Session[ModelConstants.InstanceId].ToInt();
        foreach (var detail in from detail in model.MarksEntryDetails
                               let found = _coreService.Tbl_MARKS_TMP_Repository.Get(t => t.Num_FK_INST_NO == model.InstanceId &&
                                                                                          t.Num_FK_COL_CD == (model.CollegeId ?? 0) &&
                                                                                          t.Chr_FK_COPRT_NO == model.CoursePartId.ToString() &&
                                                                                          t.Chr_FK_SUB_CD == model.SubjectId.ToString() &&
                                                                                          t.Chr_CODE_SEAT_NO == detail.SeatNo &&
                                                                                          t.Chr_FK_CAT_CD == (model.CategoryId ?? 0).ToString() &&
                                                                                          t.Chr_FK_PAP_CD == (model.PaperId ?? 0).ToString())
                                   .FirstOrDefault()
                               where null == found
                               select detail)
        {
            const string absentMarks = "999";
            var status = "P";
            if (detail.Marks == absentMarks)
            {
                status = "A";
                detail.Marks = "0";
            }
            var marksTemp = new Tbl_MARKS_TMP
            {
                Num_FK_INST_NO = (short)instanceId,
                Num_FK_COL_CD = (short)(model.CollegeId ?? 0),
                Num_FK_DISTCOL_CD = (short)(model.CentreId ?? 0),
                Chr_FK_COPRT_NO = model.CoursePartId.ToString(),
                Num_FORM_NO = 1,
                Num_FORM_SRNO = 1,
                Chr_FK_BRANCH_CD = model.BranchId,
                Chr_FK_SUB_CD = model.SubjectId.ToString(),
                Chr_FK_CAT_CD = model.CategoryId.ToString(),
                Chr_FK_PAP_CD = (model.PaperId ?? 0).ToString(),
                Chr_FK_SEC_CD = "0",

                Chr_CODE_SEAT_NO = detail.SeatNo ?? 0,
                Chr_MARKS = detail.Marks,
                Chr_MARKS2 = null,

                Chr_EXAMINER_CD = "1",
                Chr_HD_EXAMINER_CD = "1",

                Chr_STATUS_FLG = status,
                Num_ENTRY_NO = 1,
                Num_EVAL_NO = 1,
                Var_BUNDLE_NO = model.SubjectId.ToString(),
                Var_CHKLIST_FLG = "A",
                Num_Application_No = 1,

                Var_USR_NM = User.Identity.Name,
                Dtm_DTE_CR = DateTime.Now,
                Dtm_DTE_UP = DateTime.Now
            };
            _coreService.Tbl_MARKS_TMP_Repository.Add(marksTemp);
        }

        _coreService.Save();
    }

    private void Import(MarksEntry model, IEnumerable<HttpPostedFileBase> files)
    {
        /*var httpPostedFileBases = files.ToList();
        if (!httpPostedFileBases.Any())
            throw new Exception("No file to upload");*/

        model.MarksEntryDetails.Clear();
        var file = model.UploadedFile;// httpPostedFileBases.FirstOrDefault();
        try
        {
            var records = _excelFileService.Read(file, 0, 14).ToList();
            //await progressService.Report("Importing records.");

            if (!records.Any())
                throw new Exception("No records found to import.");

            // Create progress model
            //progressService.Initialize(filePath, 0, records.Count(), 1);
            int maxMarks;
            if ((model.PaperId ?? 0) > 0)
                maxMarks = _coreService.Tbl_SUB_CATPAP_MSTR_Repository.Get(c => c.Num_FK_COPRT_NO == model.CoursePartId &&
                    c.Num_FK_SUB_CD == model.SubjectId && c.Num_FK_CAT_CD == model.CategoryId &&
                    c.Num_PK_PAP_CD == model.PaperId).FirstOrDefault()?.Num_PAP_MAX_MRK ?? 0;
            else
                maxMarks = _coreService.Tbl_SUBJECT_CAT_MSTR_Repository.Get(c => c.Num_FK_COPRT_NO == model.CoursePartId &&
                        c.Num_FK_SUB_CD == model.SubjectId && c.Num_FK_CAT_CD == model.CategoryId)
                    .FirstOrDefault()?.Num_CAT_MAX_MRK ?? 0;

            // ignore existing
            var seatNos = records.Select(r => r.SeatNo).Distinct();
            var existingSeatNos = _coreService.Tbl_MARKS_TMP_Repository.Get(m =>
                (m.Num_FK_INST_NO ?? 0) == (model.InstanceId ?? 0) && (m.Num_FK_COL_CD ?? 0) == (model.CategoryId ?? 0) &&
                m.Chr_FK_COPRT_NO == model.CoursePartId.ToString() && m.Chr_FK_SUB_CD == model.SubjectId.ToString() &&
                m.Chr_FK_CAT_CD == model.CategoryId.ToString() && m.Chr_FK_PAP_CD == model.PaperId.ToString() &&
                seatNos.Contains(m.Chr_CODE_SEAT_NO))
                .Select(m => m.Chr_CODE_SEAT_NO).Distinct().ToList();
            records.RemoveAll(r => existingSeatNos.Contains(r.SerialNo));


            foreach (var detail in from record in records
                                   let detail = model.MarksEntryDetails.FirstOrDefault(d => d.SeatNo == record.SeatNo)
                                   where null == detail
                                   select new MarksEntryDetail
                                   {
                                       Prn = record.Prn,
                                       SeatNo = record.SeatNo,
                                       Marks = record.Marks,
                                       MaxMarks = maxMarks
                                   })
            {
                model.MarksEntryDetails.Add(detail);
            }

            //AddInExamServer(model);
            //await Task.Delay(0);
        }
        catch (Exception exception)
        {
            LogHandler.LogInfo(exception);
            //progressService.Report(LogHandler.GetDetailException(exception).Message);
            throw;
        }
    }

    private void ValidateFields(MarksEntry model, string submitType)
    {
        if (model.CollegeId.ToInt() <= 0)
            throw new Exception("Invalid College.");
        if (model.CourseTypeId.ToInt() <= 0)
            throw new Exception("Invalid Course Type.");
        if (model.CourseId.ToInt() <= 0)
            throw new Exception("Invalid Course.");
        if (model.CoursePartId.ToInt() <= 0)
            throw new Exception("Invalid Course Part.");
        if (model.SubjectId.ToInt() <= 0)
            throw new Exception("Invalid Subject.");
        if (model.CategoryId.ToInt() <= 0)
            throw new Exception("Invalid Category.");
        var papers = _coreService.Tbl_SUB_CATPAP_MSTR_Repository.Get(c =>
            c.Num_FK_COPRT_NO == model.CoursePartId && c.Num_FK_SUB_CD == model.SubjectId && c.Num_FK_CAT_CD == model.CategoryId).ToList();
        if (papers.Count > 0 && model.PaperId.ToInt() <= 0)
            throw new Exception("Invalid Paper.");

        switch (submitType)
        {
            case ModelConstants.Search:
                break;
            default:
                // Validations
                if (model.MarksEntryDetails.Count <= 0)
                    throw new Exception("At least one seat no should have marks filled.");
                var extraMarkEntry = model.MarksEntryDetails.FirstOrDefault(d => d.Marks?.ToDouble() > d.MaxMarks && d.Marks != "999");
                if (null != extraMarkEntry)
                    throw new Exception($"Seat No {extraMarkEntry.SeatNo} marks {extraMarkEntry.Marks} have exceeded max marks {extraMarkEntry.MaxMarks}.");
                var blankMarkEntry = model.MarksEntryDetails.FirstOrDefault(d => string.IsNullOrEmpty(d.Marks));
                if (null != blankMarkEntry)
                    throw new Exception($"Seat No {blankMarkEntry.SeatNo} marks cannot be blank.");
                break;
        }
    }

    #endregion

    #region -- Actions --
    // GET: /Reg/Create
    [Authorize]
    public ActionResult Create()
    {
        var marksEntry = new MarksEntry();
        var sessionData = Session[User.Identity.Name] as SessionData;
        marksEntry.CollegeId = sessionData?.CollegeId ?? -1;

        return View(marksEntry);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public ActionResult Create(MarksEntry model, string submitType)
    {
        try
        {
            Session[ModelConstants.Password] = "bvdu";
            // Check user type
            var sessionData = GetSession();
            if (sessionData?.UserType == UserType.College)
                model.CollegeId = sessionData.CollegeId;
            model.InstanceId = (int)HttpContext.Session[ModelConstants.InstanceId];
            switch (submitType)
            {
                case ModelConstants.Search:
                    ValidateFields(model, submitType);
                    // Get new subjects.
                    model.MarksEntryDetails?.Clear();
                    GetFormSeatNos(model);
                    model.bEnable = false;
                    return View(model);
                case ModelConstants.Report:
                    // Get new subjects.
                    model.MarksEntryDetails?.Clear();
                    GetReportSeatNos(model);
                    if (model.MarksEntryDetails?.Count <= 0)
                        throw new Exception("No entries found for marks entry.");
                    Session[ModelConstants.Report] = new MarksSlipRpt(model);
                    return RedirectToAction("Details", "Report", new { area = "Reports", reportName = nameof(MarksSlipRpt), description = string.Empty });
                case ModelConstants.Summary:
                    // Get new subjects.
                    model.MarksEntryDetails?.Clear();
                    Session[ModelConstants.Report] = new MarksSummaryRpt(GetSession()?.InstanceId ?? 0);
                    return RedirectToAction("Details", "Report", new { area = "Reports", reportName = nameof(MarksSlipRpt), description = string.Empty });
                default:
                    if (null != model.UploadedFile)
                        Import(model, null);
                    break;
            }

            if (ModelState.IsValid)
            {
                // Validate fields in model
                ValidateFields(model, submitType);

                // Check whether extra marks entries are added.
                var newModel = new MarksEntry
                {
                    InstanceId = model.InstanceId,
                    CollegeId = model.CollegeId,
                    CentreId = model.CentreId,
                    CourseTypeId = model.CourseTypeId,
                    CourseId = model.CourseId,
                    CoursePartId = model.CoursePartId,
                    SubjectId = model.SubjectId,
                    CategoryId = model.CategoryId,
                    PaperId = model.PaperId
                };
                newModel.MarksEntryDetails.Clear();
                GetFormSeatNos(newModel);
                var seatNos = newModel.MarksEntryDetails.Select(d => d.SeatNo).Distinct();
                // Remove from model
                model.MarksEntryDetails.RemoveAll(d => !seatNos.Contains(d.SeatNo));

                /*using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout });
                try
                {*/
                UpdateUserDetails(model);

                // Add Student To Exam Server
                AddInExamServer(model);

                //scope.Complete();
                TempData["Success"] = "Marks Saved Successfully";

                return RedirectToAction("Create");
                /*}
                catch
                {
                    //scope.Dispose();
                    throw;
                }*/
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
    }

    [HttpPost]
    public ActionResult Upload(IEnumerable<HttpPostedFileBase> files, MarksEntry model)
    {
        // Process the files using Telerik components or your custom logic
        Import(model, files);

        return Json(new { status = "success" });
    }

    #endregion
}