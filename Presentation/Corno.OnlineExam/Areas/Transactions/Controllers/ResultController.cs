using Corno.OnlineExam.Controllers;
using Corno.OnlineExam.Helpers;
using PdfSharp.Pdf;
using PdfSharp.Pdf.IO;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.Linq;
using System.Web.Mvc;
using Corno.Data.Corno;
using Corno.Data.Helpers;
using Corno.Globals.Constants;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Payment.Interfaces;
using Corno.Services.SMS.Interfaces;
using Corno.Data.Operation;
using Corno.Data.Payment;
using Corno.Globals.Enums;
using Corno.Logger;
using Newtonsoft.Json;

namespace Corno.OnlineExam.Areas.Transactions.Controllers;

[Authorize]
[AllowAnonymous]
public class ResultController : BaseController
{
    #region -- Constructors --
    public ResultController(ICoreService coreService, ICornoService cornoService, ISmsService smsService,
        IEaseBuzzService easeBuzzService)
    {
        _coreService = coreService;
        _cornoService = cornoService;
        _smsService = smsService;
        _easeBuzzService = easeBuzzService;
    }
    #endregion

    #region -- Data Members --

    private readonly ICoreService _coreService;
    private readonly ICornoService _cornoService;
    private readonly ISmsService _smsService;
    private readonly IEaseBuzzService _easeBuzzService;

    #endregion

    #region -- Methods --
    private void GeneratePdf(int collegeId, int courseId, MarkSheet markSheet)
    {
        if (markSheet == null) throw new ArgumentNullException(nameof(markSheet));

        try
        {
            markSheet.FileName = null;

            //var folderPath = ConfigurationManager.AppSettings["MarksheetPath"];//Server.MapPath(@"~/Content/MarkSheets/");
            var folderPath = Server.MapPath(@"~/Content/MarkSheets/");

            var collegeDirectoryInfo = new DirectoryInfo(folderPath);
            var collegeDirectorInfos = collegeDirectoryInfo.GetDirectories("*" + collegeId + " " + "*.*");
            if (collegeDirectorInfos.Length <= 0) return;

            var collegeFolderPath = collegeDirectorInfos[0].FullName;

            var courseDirectoryInfo = new DirectoryInfo(collegeFolderPath);
            var courseDirectoryInfos = courseDirectoryInfo.GetDirectories("*" + courseId + " " + "*.*");
            if (courseDirectoryInfos.Length <= 0) return;

            //var courseFolderPath = courseDirectoryInfos[0].FullName;

            var fileInfos = courseDirectoryInfos[0].GetFiles("*.pdf");
            foreach (var fileInfo in fileInfos)
            {
                var outputDocument = GetPdfPage(fileInfo.FullName, markSheet.PrnNo);

                if (null == outputDocument) continue;

                markSheet.FileName = markSheet.PrnNo + ".pdf";
                outputDocument.Save(folderPath + markSheet.FileName);

                //markSheet.FileName = folderPath + markSheet.FileName;

                //var xGraphics = XGraphics.FromPdfPage(outputDocument.Pages[0]);
                //Bitmap b = new Bitmap((int)pdfp.Width.Point, (int)pdfp.Height.Point, xGraphics.Graphics);

                break;
            }

            //string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"500px\" height=\"300px\">";
            //embed += "If you are unable to view file, you can download from <a href = \"{0}\">here</a>";
            //embed += " or download <a target = \"_blank\" href = \"http://get.adobe.com/reader/\">Adobe PDF Reader</a> to view the file.";
            //embed += "</object>";
            //TempData["Embed"] = string.Format(embed, VirtualPathUtility.ToAbsolute($@"{folderPath}/{markSheet.FileName}"));
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
    }

    public PdfDocument GetPdfPage(string filePath, string prnNo)
    {
        try
        {
            var inputDocument = PdfReader.Open(filePath, PdfDocumentOpenMode.Import);
            var index = 0;
            var outputDocument = new PdfDocument { Version = inputDocument.Version };
            outputDocument.Info.Title = $"Page {index} of {inputDocument.Info.Title}";
            outputDocument.Info.Creator = inputDocument.Info.Creator;
            foreach (var page in inputDocument.Pages)
            {
                var outputText = PdfTextExtractor.GetPageText(page);
                index++;

                if (!outputText.Contains(prnNo)) continue;

                // Add the page and save it
                outputDocument.AddPage(page);
            }

            if (outputDocument.PageCount > 0)
                return outputDocument;
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return null;
    }

    private void HandleCreatePostAction(MarkSheet markSheet)
    {
        var collegeId = 0;
        var courseId = 0;

        // Get student's College ID and Course ID from TBL_STUDENT_YR_CHNG
        var yearChangeRecord = _coreService.Tbl_STUDENT_YR_CHNG_Repository.Get(s => s.Chr_FK_PRN_NO == markSheet.PrnNo)
            .OrderByDescending(s => s.Num_FK_INST_NO).FirstOrDefault();
        if (null != yearChangeRecord)
        {
            collegeId = yearChangeRecord.Num_FK_COL_CD;
            courseId = ExamServerHelper.GetCourseId(yearChangeRecord.Num_FK_COPRT_NO, _coreService);
        }

        GeneratePdf(collegeId, courseId, markSheet);

        if (string.IsNullOrEmpty(markSheet.FileName))
            ModelState.AddModelError("Error : ", @"No match found for this PRN");
    }
    #endregion

    #region -- Actions --
    // GET: /Transactions/Result/Create
    public ActionResult Create()
    {
        return View(new MarkSheet());
    }

    // GET: /Transactions/Result/Create
    [AllowAnonymous]
    public ActionResult CreateDirect()
    {
        //return RedirectToAction("NotFound", "Error");
        return View(new MarkSheet());
    }

    // POST: /Transactions/Result/Create
    [HttpPost]
    public ActionResult Create(MarkSheet markSheet)
    {
        try
        {
            HandleCreatePostAction(markSheet);

            //string embed = "<object data=\"{0}\" type=\"application/pdf\" width=\"500px\" height=\"300px\">";
            //embed += "If you are unable to view file, you can download from <a href = \"{0}\">here</a>";
            //embed += " or download <a target = \"_blank\" href = \"http://get.adobe.com/reader/\">Adobe PDF Reader</a> to view the file.";
            //embed += "</object>";
            //TempData["Embed"] = string.Format(embed, VirtualPathUtility.ToAbsolute($@"~/Content/Marksheets/{markSheet.FileName}"));

            return View(markSheet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(markSheet);
    }

    [AllowAnonymous]
    [HttpPost]
    public ActionResult CreateDirect(MarkSheet markSheet)
    {
        //return RedirectToAction("NotFound", "Error");
        try
        {
            // Code for validating the CAPTCHA   
            //if (!this.IsCaptchaValid("Captcha is not valid"))
            //    throw new Exception("Invalid Captcha");

            // Validate Otp
            var otp = _cornoService.OtpRepository.Get(o => o.Otp == markSheet.Otp &&
                                                           o.PrnNo == markSheet.PrnNo).FirstOrDefault();
            if (null == otp)
                throw new Exception("Invalid OTP");
            if (otp.ExpiryTime <= DateTime.Now)
                throw new Exception("OTP expired");

            HandleCreatePostAction(markSheet);

            return View(markSheet);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(markSheet);
    }

    [AllowAnonymous]
    [HttpPost]
    public virtual ActionResult SendOtp(MarkSheet markSheet)
    {
        ActionResult jsonResult;
        try
        {
            if (!ModelState.IsValid)
            {
                throw new Exception(ModelState.Keys.SelectMany(k => ModelState[k].Errors)
                    .Select(m => m.ErrorMessage).FirstOrDefault());
            }

            // Validate PRN
            if (string.IsNullOrEmpty(markSheet.PrnNo))
                throw new Exception("Invalid PRN No.");
            var studentAddress = _coreService.Tbl_STUDENT_INFO_ADR_Repository
                .Get(s => s.Chr_FK_PRN_NO == markSheet.PrnNo).FirstOrDefault();
            if (null == studentAddress)
                throw new Exception("PRN not exist in system.");

            // Validate message
            var smsUrl = ConfigurationManager.AppSettings[ModelConstants.SmsUrl];
            var message = ConfigurationManager.AppSettings[ModelConstants.ResultOtp];
            if (string.IsNullOrEmpty(message) || string.IsNullOrEmpty(smsUrl))
                throw new Exception("No SMS format available in system.");
            // Validate mobile no.
            var mobileNo = studentAddress.Num_MOBILE;
            if (string.IsNullOrEmpty(mobileNo))
                throw new Exception("Your mobile no is not registered with us.");

            // Generate message
            var otp = new Random().Next(0, 999999).ToString().PadLeft(6, '0');
            message = message.Replace("@otp", otp);

            // Check if otp is already sent.
            var existing = _cornoService.OtpRepository.Get(o => o.PrnNo == markSheet.PrnNo &&
                                                                o.ExpiryTime >= DateTime.Now)
                .OrderByDescending(p => p.Id).FirstOrDefault();
            if (null != existing)
                throw new Exception($"OTP {markSheet.Otp} is already sent. It will be valid till 3 mins.");

            // Send message
            smsUrl = smsUrl.Replace("@mobileNo", mobileNo);
            smsUrl = smsUrl.Replace("@message", message);
            var smsSendStatus = _smsService.SendSms(smsUrl);

            // Save otp
            var sentTime = DateTime.Now;
            _cornoService.OtpRepository.Add(new TransactionOtp
            {
                Code = smsSendStatus,
                Transaction = ModelConstants.ResultOtp,
                PrnNo = markSheet.PrnNo,
                MobileNo = mobileNo,
                Otp = otp,
                SendTime = sentTime,
                ExpiryTime = sentTime.AddMinutes(3)
            });
            _cornoService.Save();

            // Reply success
            jsonResult = Json(new
            {
                error = false,
                message = "OTP is sent to your registered mobile. Please enter OTP to view the result."
            }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            jsonResult = Json(new
            {
                error = true,
                message = exception.Message
            }, JsonRequestBehavior.AllowGet);
        }

        return jsonResult;
    }

    private bool UpdateFromExam(Payout payout)
    {
        var transactionId = payout.TransactionId;
        //var exam = _cornoService.ExamRepository.Get(e => e.TransactionId == transactionId).FirstOrDefault();
        //if (null != exam)
        //{
        //    LogHandler.LogInfo($"Found Transaction Id {transactionId} in exam table");

        //    payout.InstanceId = exam.InstanceId;
        //    payout.CollegeId = exam.CollegeId;
        //    payout.CourseId = exam.CourseId;
        //    payout.CoursePartId = exam.CoursePartId;
        //    payout.Prn = exam.PrnNo;
        //    payout.FormType = FormType.Exam.ToString();
        //    return true;
        //}

        var appTemp = _coreService.Tbl_APP_TEMP_Repository.Get(e => e.Num_Transaction_Id == transactionId).FirstOrDefault();
        if (null == appTemp) return false;

        LogHandler.LogInfo($"Found Transaction Id {transactionId} in APP_TEMP table");

        payout.InstanceId = appTemp.Num_FK_INST_NO;
        payout.CollegeId = appTemp.Num_FK_COLLEGE_CD;
        payout.CoursePartId = appTemp.Num_FK_COPRT_NO;
        payout.Prn ??= appTemp.Chr_APP_PRN_NO;
        payout.FormType = FormType.Exam.ToString();
        return true;
    }

    private bool UpdateFromEnvironmentStudy(Payout payout)
    {
        var transactionId = payout.TransactionId;
        //var environmentStudy = _cornoService.EnvironmentStudyRepository.Get(e => e.TransactionId == transactionId).FirstOrDefault();
        //if (null != environmentStudy)
        //{
        //    payout.InstanceId = environmentStudy.InstanceId;
        //    payout.CollegeId = environmentStudy.CollegeId;
        //    payout.CourseId = environmentStudy.CourseId;
        //    payout.CoursePartId = environmentStudy.CoursePartId;
        //    payout.Prn = environmentStudy.PrnNo;
        //    payout.FormType = FormType.Environment.ToString();
        //    return true;
        //}

        var envStudies = _coreService.TBl_STUDENT_ENV_STUDIES_Repository.Get(e => e.Chr_Transaction_Id == transactionId).FirstOrDefault();
        if (null == envStudies) return false;

        payout.InstanceId = envStudies.Num_FK_INST_NO;
        payout.CollegeId = envStudies.Num_FK_COL_CD;
        payout.CourseId = envStudies.Num_FK_CO_CD;
        payout.CoursePartId = envStudies.Num_FK_COPRT_NO;
        payout.Prn = envStudies.Chr_FK_PRN_NO;
        payout.FormType = FormType.Environment.ToString();
        return true;
    }

    private bool UpdateFromConvocation(Payout payout)
    {
        var transactionId = payout.TransactionId;
        //var convocation = _cornoService.EnvironmentStudyRepository.Get(e => e.TransactionId == transactionId).FirstOrDefault();
        //if (null != convocation)
        //{
        //    payout.InstanceId = convocation.InstanceId;
        //    payout.CollegeId = convocation.CollegeId;
        //    payout.CourseId = convocation.CourseId;
        //    payout.CoursePartId = convocation.CoursePartId;
        //    payout.Prn = convocation.PrnNo;
        //    payout.FormType = FormType.Environment.ToString();
        //    return true;
        //}

        var envStudies = _coreService.TBl_STUDENT_ENV_STUDIES_Repository.Get(e => e.Chr_Transaction_Id == transactionId).FirstOrDefault();
        if (null == envStudies) return false;

        payout.InstanceId = envStudies.Num_FK_INST_NO;
        payout.CollegeId = envStudies.Num_FK_COL_CD;
        payout.CourseId = envStudies.Num_FK_CO_CD;
        payout.CoursePartId = envStudies.Num_FK_COPRT_NO;
        payout.Prn = envStudies.Chr_FK_PRN_NO;
        payout.FormType = FormType.Environment.ToString();
        return true;
    }

    private void UpdateOtherInfoIfNotAvailable(List<Payout> payouts)
    {
        var problemPayouts = payouts.Where(p =>
            string.IsNullOrEmpty(p.Prn) || null == p.InstanceId || (p.CollegeId ?? 0) <= 0 ||
            (p.CourseId ?? 0) <= 0 || string.IsNullOrEmpty(p.FormType)).ToList();


        foreach (var payout in problemPayouts)
        {
            if (UpdateFromExam(payout)) continue;

            if (UpdateFromEnvironmentStudy(payout)) continue;

            UpdateFromConvocation(payout);
        }
    }

    private void UpdatePayouts(DateTime dtmSettlementDate)
    {
        var operationRequest = new OperationRequest
        {
            InputData = new Dictionary<string, object>
            {
                [ModelConstants.SettlementDate] = dtmSettlementDate.ToString("dd-MM-yyyy")
            }
        };
        var result = _easeBuzzService.SendPayoutRequest(operationRequest);
        if (string.IsNullOrEmpty(result)) return;

        var root = JsonConvert.DeserializeObject<PayoutRoot>(result);

        if (root.payouts_history_data.Count <= 0)
            return;

        var payouts = new List<Payout>();
        foreach (var historyData in root.payouts_history_data)
        {
            var historyDataPayouts = historyData.peb_transactions.Select(p =>
                new Payout(p, dtmSettlementDate)).ToList();
            payouts.AddRange(historyDataPayouts);

            // Update other data for old payments before Feb 2024. Update from exam, convocation, and environment studes tables
            UpdateOtherInfoIfNotAvailable(payouts);
        }

        //if (payouts == null) return;

        var transactionIds = payouts.Select(p => p.TransactionId.Trim()).Distinct().ToList();
        var existingPayouts = _coreService.PayoutRepository
            .Get(p => transactionIds.Contains(p.TransactionId)).ToList();
        var existingTransactionIds = existingPayouts.Select(p => p.TransactionId.Trim()).ToList();
        var newTransactionIds = transactionIds.Except(existingTransactionIds).ToList();

        var newPayouts = payouts.Where(p => newTransactionIds.Contains(p.TransactionId.Trim()))
            .ToList();
        _cornoService.PayoutRepository.AddRange(newPayouts);
        _cornoService.Save();

        _coreService.PayoutRepository.AddRange(newPayouts);
        _coreService.Save();
    }

    [HttpPost]
    public virtual ActionResult UploadSettlement(string settlementDate)
    {
        //LogHandler.LogInfo(settlementDate);
        ActionResult jsonResult;
        try
        {
            var data = settlementDate.Split('-');
            if (data.Length <= 0)
                throw new Exception("Invalid settlement date");
            //var dtmSettlementDate = settlementDate.ToDateTime();
            var dtmSettlementDate = new DateTime(data[2].ToInt(), data[1].ToInt(), data[0].ToInt());
            // Find the last day of the month
            var endDate = new DateTime(dtmSettlementDate.Year, dtmSettlementDate.Month, DateTime.DaysInMonth(dtmSettlementDate.Year, dtmSettlementDate.Month));

            // Loop from start date until end of the month
            //for (var date = dtmSettlementDate; date <= endDate; date = date.AddDays(1))
            UpdatePayouts(dtmSettlementDate);

            //LogHandler.LogInfo($"Result : {result}");

            // Reply success
            jsonResult = Json(new
            {
                error = false,
                message = "Upload successfully."
            }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            jsonResult = Json(new
            {
                error = true,
                message = exception.Message
            }, JsonRequestBehavior.AllowGet);
        }

        return jsonResult;
    }
    #endregion
}