using Castle.DynamicProxy;
using Corno.Data.Core;
using Corno.Data.Corno;
using Corno.Globals.Constants;
using Corno.OnlineExam.Controllers;
using Corno.Services.Common.Interfaces;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;

namespace Corno.OnlineExam.Areas.Transactions.Controllers;

[Authorize]
public class EnvironmentController : UniversityController
{
    #region -- Constructors --
    public EnvironmentController(ICornoService cornoService, ICoreService coreService,
        IAmountInWordsService amountInWordsService)
    {
        _cornoService = cornoService;
        _coreService = coreService;
        _amountInWordsService = amountInWordsService;
    }
    #endregion

    #region -- Data Members --
    private readonly ICornoService _cornoService;
    private readonly ICoreService _coreService;
    private readonly IAmountInWordsService _amountInWordsService;
    #endregion

    #region -- Protected Methods --
    protected void AddInCornoDatabase(EnvironmentStudy model, string paymentUser = null)
    {
        var instanceId = model.InstanceId ?? 0;

        // Get Environment Month & Year.
        var environmentSetting = _cornoService.EnvironmentSettingRepository
            .Get(i => i.InstanceId == instanceId).FirstOrDefault();
        if (null == environmentSetting)
            throw new Exception("Instance No. " + instanceId + " is not created in EnvironmentSetting Table.");

        // First save to Online Exam Database
        model.InstanceId = instanceId;
        model.Year = environmentSetting.Year;
        model.Month = environmentSetting.Month;
        model.CreatedBy = User.Identity.GetUserId();
        model.CreatedDate = DateTime.Now;

        _cornoService.EnvironmentStudyRepository.Add(model);
        _cornoService.Save();
    }

    protected void AddInCoreDatabase(EnvironmentStudy model, string paymentUser = null)
    {
        var instanceId = model.InstanceId ?? 0;

        // Get Environment Month & Year.
        var environmentSetting = _cornoService.EnvironmentSettingRepository
            .Get(i => i.InstanceId == instanceId).FirstOrDefault();
        if (null == environmentSetting)
            throw new Exception("Instance No. " + instanceId + " is not created in EnvironmentSetting Table.");

        // Next Save to Exam Server
        var environmentStudies = new TBl_STUDENT_ENV_STUDIES
        {
            Chr_FK_PRN_NO = model.PrnNo,

            Num_FK_CO_CD = null == model.CourseId ? (short)0 : (short)model.CourseId,
            Num_FK_COL_CD = (short?)model.CollegeId,
            Num_FK_DistCenter_ID = (short)(model.CenterId ?? 0),
            Num_FK_COPRT_NO = (null == model.CoursePartId ? 0 : (short?)model.CoursePartId),
            Chr_ST_SUB_STS = "A",
            Num_FK_INST_NO = (short)instanceId,
            Num_YEAR = environmentSetting.Year.ToString(),
            Num_MONTH_NO = (short)(environmentSetting.Month ?? 0),
            Chr_ST_SUB_RES = null,
            Num_ST_SUB_MRK = 0,
            Num_EnviFee = model.EnvironmentFee,
            Num_EnviLateFee = model.LateFee,
            Num_EnviSuperLateFee = model.SuperLateFee,
            Num_EnviOtherFee = model.OtherFee,
            Num_EnviTotalFee = model.TotalFee,
            Var_USR_NM = string.IsNullOrEmpty(paymentUser) ? User.Identity.Name : paymentUser,
            Dtm_DTE_CR = model.CreatedDate,
            Dtm_DTE_UP = model.ModifiedDate,
            Chr_Transaction_Id = model.TransactionId,
            PaidAmount = model.PaidAmount,
            PaymentDate = model.PaymentDate,
            SettlementDate = model.SettlementDate
        };

        _coreService.TBl_STUDENT_ENV_STUDIES_Repository.Add(environmentStudies);
        _coreService.Save();
    }

    protected EnvironmentStudy UpdateInCornoDatabase(string prn, string transactionId, double paidAmount, DateTime paymentDate, string ipAddress)
    {
        var environmentStudy = _cornoService.EnvironmentStudyRepository.Get(e => e.PrnNo == prn)
            .OrderByDescending(p => p.Id).FirstOrDefault();
        if (null == environmentStudy)
            throw new Exception($"No entry in environment study found for PRN {transactionId}");

        if (!environmentStudy.TotalFee.Equals(paidAmount))
        {
            throw new Exception($"You have made payment of Rs. {paidAmount} which does not match with actual amount Rs. {environmentStudy.TotalFee}");
        }

        environmentStudy.TransactionId = transactionId;
        environmentStudy.PaidAmount ??= 0;
        environmentStudy.PaidAmount += paidAmount;
        environmentStudy.PaymentDate = paymentDate;
        environmentStudy.DeletedBy = ipAddress;

        if (!environmentStudy.TotalFee.Equals(paidAmount))
        {
            _cornoService.EnvironmentStudyRepository.Update(environmentStudy);
            _cornoService.Save();
            throw new Exception($"You have made payment of Rs. {paidAmount} which does not match with actual amount Rs. {environmentStudy.TotalFee}");
        }

        environmentStudy.Status = StatusConstants.Paid;

        _cornoService.EnvironmentStudyRepository.Update(environmentStudy);
        _cornoService.Save();

        return environmentStudy;
    }
    #endregion

    #region -- Public Methods --
    public EnvironmentStudy GetPrnInfo(EnvironmentStudy model)
    {
        // Check for existing PRN No for Current Instance
        /*var existingRecord = _coreService.TBl_STUDENT_ENV_STUDIES_Repository.Get(e => e.Chr_FK_PRN_NO == model.PrnNo && 
            e.Chr_ST_SUB_RES == "P").ToList();
        if (existingRecord.Count > 0)
            throw new Exception("Environment form for this PRN " + model.PrnNo + " already exists and his result is 'Pass'");

        existingRecord = _coreService.TBl_STUDENT_ENV_STUDIES_Repository.Get(e => e.Chr_FK_PRN_NO == model.PrnNo && 
            e.Chr_ST_SUB_STS == "A").ToList();
        if (existingRecord.Count > 0)
            throw new Exception("Environment form for this PRN " + model.PrnNo + " already exists and appeared.");*/

        var existingRecords = _coreService.TBl_STUDENT_ENV_STUDIES_Repository.Get(e => e.Chr_FK_PRN_NO == model.PrnNo)
            .ToList();
        if (existingRecords.Count > 0 && existingRecords.Any(e => e.Chr_ST_SUB_RES == "P"))
            throw new Exception("Environment form for this PRN " + model.PrnNo + " already exists and his result is 'Pass'");
        var existingRecord = existingRecords.FirstOrDefault(e => e.Chr_FK_PRN_NO == model.PrnNo && 
                                                                 e.Chr_ST_SUB_STS == "A");

        //LogHandler.LogInfo($"PRN : {existingRecord?.Chr_FK_PRN_NO}, User Name : {existingRecord?.Var_USR_NM}, Transaction Id : {existingRecord?.Chr_Transaction_Id}, Status : {existingRecord?.Chr_ST_SUB_STS}");
        if (null != existingRecord)
        {
            if (existingRecord.Var_USR_NM != ModelConstants.Online || (existingRecord.Var_USR_NM == ModelConstants.Online && !string.IsNullOrEmpty(existingRecord.Chr_Transaction_Id)))
                throw new Exception("Environment form for this PRN " + model.PrnNo + " already exists and appeared.");
        }
                
        // Get from TBL_STUDENT_YR_CHNG
        var yearChangeRecord = _coreService.Tbl_STUDENT_YR_CHNG_Repository.Get(s => s.Chr_FK_PRN_NO == model.PrnNo).OrderByDescending(s => s.Num_FK_INST_NO).FirstOrDefault();
        var instanceId = model.InstanceId ?? 0;// (int)HttpContext.Session[ModelConstants.InstanceId];
        // Go to TBL_STUDENT_COURSE
        var studentCourse = _coreService.Tbl_STUDENT_COURSE_Repository.Get(s => s.Chr_FK_PRN_NO == model.PrnNo).FirstOrDefault();
        if (null == studentCourse)
            throw new Exception("PRN No " + model.PrnNo + " does not exists!");
        var studentInfoAdr = _coreService.Tbl_STUDENT_INFO_ADR_Repository.Get(c => c.Chr_FK_PRN_NO == model.PrnNo).FirstOrDefault();
        if (null == yearChangeRecord)
        {
            if (null != studentCourse.Num_FK_DistCenter_ID)
                model.CenterId = (int)studentCourse.Num_FK_DistCenter_ID;
            if (studentCourse.Num_ST_COLLEGE_CD != null)
                model.CollegeId = (int)studentCourse.Num_ST_COLLEGE_CD;
            model.CoursePartId = studentCourse.Num_FK_COPRT_NO;
        }
        else
        {
            if (instanceId == yearChangeRecord.Num_FK_INST_NO)
            {
                model.CoursePartId = yearChangeRecord.Num_FK_COPRT_NO;
            }
            else
            {
                var nextCoursePart = ExamServerHelper.GetNextCoursePart(yearChangeRecord.Num_FK_COPRT_NO, _coreService);
                //if (null == nextCoursePart)
                //    throw new Exception("Next course part is not available for this student. His / her all course parts finished.");
                model.CoursePartId = nextCoursePart?.Num_PK_COPRT_NO ?? yearChangeRecord.Num_FK_COPRT_NO;
            }
            model.CollegeId = yearChangeRecord.Num_FK_COL_CD;

            if (null != yearChangeRecord.Num_FK_DistCenter_ID)
                model.CenterId = (int)yearChangeRecord.Num_FK_DistCenter_ID;
        }

        model.Mobile = studentInfoAdr?.Num_MOBILE;
        model.Email = studentInfoAdr?.Chr_Student_Email;
        model.StudentName = ExamServerHelper.GetStudentName(model.PrnNo, _coreService);
        model.CollegeName = ExamServerHelper.GetCollegeName(model.CollegeId, _coreService);
        model.CenterName = ExamServerHelper.GetCentreName(model.CenterId, _coreService);
        model.CourseId = ExamServerHelper.GetCourseId(model.CoursePartId, _coreService);
        model.CourseName = ExamServerHelper.GetCourseNameFromCoursePartId(model.CoursePartId, _coreService);

        // Get Course Part from Tbl_COURSE_PART_MSTR
        model.CoursePartName = ExamServerHelper.GetCoursePartName(model.CoursePartId, _coreService);

        if (studentInfoAdr?.Ima_ST_PHOTO != null)
            model.Photo = studentInfoAdr.Ima_ST_PHOTO;

        var feeStructure = ExamServerHelper.GetFeeStructureForEnvironment(instanceId, _cornoService);

        model.EnvironmentFee = feeStructure.EnvironmentExamFee;
        model.LateFee = feeStructure.EnvironmentLateFeeDate != null && DateTime.Now.Date > feeStructure.EnvironmentLateFeeDate.Value.Date ?
            feeStructure.EnvironmentLateFee : 0;
        model.SuperLateFee = feeStructure.EnvironmentSuperLateFeeDate != null && DateTime.Now.Date > feeStructure.EnvironmentSuperLateFeeDate.Value.Date ?
            feeStructure.EnvironmentSuperLateFee : 0;
        if (model.SuperLateFee > 0)
            model.LateFee = 0;

        model.OtherFee = 0;
        model.TotalFee = model.EnvironmentFee + model.LateFee + model.SuperLateFee + model.OtherFee;
        model.FeeInWord = _amountInWordsService.GetAmountInWords(model.TotalFee.ToString());

        model.LateFeeDate = feeStructure.EnvironmentLateFeeDate;
        model.SuperLateFeeDate = feeStructure.EnvironmentSuperLateFeeDate;

        return model;
    }
    #endregion

    #region -- Actions --
    // GET: /Reg/Create
    [Authorize]
    public virtual ActionResult Create()
    {
        var model = new EnvironmentStudy();
        return View(model);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public virtual ActionResult Create(EnvironmentStudy model, string submitType)
    {
        try
        {
            //Check for invalid PRN No
            if (string.IsNullOrEmpty(model.PrnNo))
                throw new Exception("Error: Invalid PRN No");
            // Check for existing PRN No for Current Instance
            var existingRecord = _coreService.TBl_STUDENT_ENV_STUDIES_Repository.Get(e => e.Chr_FK_PRN_NO == model.PrnNo)
                .OrderByDescending(e => e.Num_YEAR).ThenByDescending(e => e.Num_MONTH_NO).FirstOrDefault();
            if (null != existingRecord && existingRecord.Chr_ST_SUB_RES != "F" &&
                existingRecord.Chr_ST_SUB_RES != "N")
            {
                throw existingRecord.Chr_ST_SUB_RES switch
                {
                    "P" => new Exception("Environment form for this PRN " + model.PrnNo +
                                         " already exists and his result is 'Pass'"),
                    "A" => new Exception("Environment form for this PRN " + model.PrnNo +
                                         " already exists and appeared."),
                    _ => new Exception("Environment form for this PRN " + model.PrnNo + " already exists")
                };
            }

            model.InstanceId = (int)HttpContext.Session[ModelConstants.InstanceId];
            if (submitType == "Search")
            {
                GetPrnInfo(model);

                ModelState.Clear();
                return View(model);
            }

            if (ModelState.IsValid)
            {
                AddInCornoDatabase(model);
                AddInCoreDatabase(model);

                TempData["Success"] = "Environment form submitted successfully.";
                return RedirectToAction("Create");
            }

            ViewData["Error"] = "Error message text.";
            return View(model);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(model);
    }

    #endregion
}