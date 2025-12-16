using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using Corno.Data.Core;
using Corno.Data.Corno;
using Corno.Data.Helpers;
using Corno.Data.ViewModels;
using Corno.Globals.Constants;
using Corno.Globals.Enums;
using Corno.Logger;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Email.Interfaces;
using Corno.Services.Helper;
using Corno.Services.SMS.Interfaces;
using Kendo.Mvc.UI;
using MoreLinq;

namespace Corno.Services.Corno;

public class LinkService : BaseService, ILinkService
{
    #region -- Constructors --
    public LinkService(ICornoService cornoService, ICoreService coreService, ISmsService smsService, IEmailService emailService)
    {
        _cornoService = cornoService;
        _coreService = coreService;
        _smsService = smsService;
        _emailService = emailService;

        // Initialize
        Initialize();
    }
    #endregion

    #region -- Data Members --

    private readonly ICornoService _cornoService;
    private readonly ICoreService _coreService;
    private readonly ISmsService _smsService;
    private readonly IEmailService _emailService;

    //private string _baseUrl;

    private string _smsUrl;
    //private string _smsMessage;

    private EmailSetting _emailSetting;
    #endregion

    #region -- Private Methods --
    private void Initialize()
    {
        //_baseUrl = ConfigurationManager.AppSettings[SettingConstants.BaseUrl];

        // SMS Settings
        _smsUrl = ConfigurationManager.AppSettings[SettingConstants.SmsUrl];
        if (string.IsNullOrEmpty(_smsUrl))
            throw new Exception("SMS url not found.");
        /*_smsMessage = ConfigurationManager.AppSettings[SettingConstants.ExamLinkSms];
        if (string.IsNullOrEmpty(_smsMessage))
            throw new Exception("Exam link SMS message format not found.");*/

        // Email Settings
        _emailSetting = new EmailSetting
        {
            From = ConfigurationManager.AppSettings[SettingConstants.From],
            Password = ConfigurationManager.AppSettings[SettingConstants.EmailPassword],
            SmtpServer = ConfigurationManager.AppSettings[SettingConstants.SmtpServer],
            SmtpPort = ConfigurationManager.AppSettings[SettingConstants.SmtpPort].ToInt(),
            EnableSsl = ConfigurationManager.AppSettings[SettingConstants.EnableSsl].ToBoolean(),
            Cc = ConfigurationManager.AppSettings[SettingConstants.Cc],
            Bcc = ConfigurationManager.AppSettings[SettingConstants.Bcc],
            Subject = ConfigurationManager.AppSettings[SettingConstants.ExamLinkEmailSubject],
            Body = ConfigurationManager.AppSettings[SettingConstants.EmailBody],
        };
    }

    private static string GetLinkUrl()
    {
        //return $"{_baseUrl}/ExamForm/Create?instanceId={instanceId}&prn={prn}";
        return ConfigurationManager.AppSettings["LoginUrl"];
    }

    private string SendLinkSms(string mobile, string instanceName, string linkUrl, FormType formType)
    {
        // Send message
        var linkSms = formType switch
        {
            FormType.Exam => ConfigurationManager.AppSettings[SettingConstants.ExamLinkSms],
            FormType.Environment => ConfigurationManager.AppSettings[SettingConstants.EnvironmentLinkSms],
            FormType.Convocation => ConfigurationManager.AppSettings[SettingConstants.ConvocationLinkSms],
            _ => throw new ArgumentOutOfRangeException(nameof(formType), formType, null)
        };

        if (string.IsNullOrEmpty(linkSms))
            throw new Exception("Exam link SMS message format not found.");
        var smsBody = linkSms.Replace("for {#var#}", $"for {instanceName}");
        smsBody = smsBody.Replace("link {#var#}", $"link {linkUrl}");

        var smsUrl = _smsUrl.Replace("@mobileNo", mobile);
        smsUrl = smsUrl.Replace("@message", smsBody);

        return _smsService.SendSms(smsUrl);
    }

    private void GetSem1Students(Link link, int loginInstanceId)
    {
        var studentCourses = _coreService.Tbl_STUDENT_COURSE_Repository.Get(c =>
            c.Num_FK_INST_NO == loginInstanceId && (c.Num_FK_DistCenter_ID ?? 0) == (link.CentreId ?? 0) && c.Num_ST_COLLEGE_CD == (link.CollegeId ?? 0) &&
            c.Num_FK_CO_CD == link.CourseId && c.Num_FK_COPRT_NO == link.CoursePartId /*&&
                c.Num_FK_BR_CD == (link.BranchId ?? 0)*/).ToList();

        var prnList = studentCourses.Select(y => y.Chr_FK_PRN_NO).Distinct().ToList();
        var studentInfoList = _coreService.Tbl_STUDENT_INFO_Repository
            .Get(s => prnList.Contains(s.Chr_PK_PRN_NO));
        var studentInfoAdrList = _coreService.Tbl_STUDENT_INFO_ADR_Repository.Get(s =>
            prnList.Contains(s.Chr_FK_PRN_NO));
        var coursePart = _coreService.Tbl_COURSE_PART_MSTR_Repository.Get(c => c.Num_PK_COPRT_NO == link.CoursePartId)
            .FirstOrDefault();

        // Check whether Prn available in yr change
        var existingPrnList = _coreService.Tbl_STUDENT_YR_CHNG_Repository.Get(y => prnList.Contains(y.Chr_FK_PRN_NO))
            .Select(y => y.Chr_FK_PRN_NO).Distinct();
        studentCourses.RemoveAll(s => existingPrnList.Contains(s.Chr_FK_PRN_NO));

        var serialNo = link.LinkDetails.Count + 1;
        var linkDetails = from studentCourse in studentCourses
                          join studentInfo in studentInfoList
                              on studentCourse.Chr_FK_PRN_NO equals studentInfo.Chr_PK_PRN_NO
                          join studentInfoAdr in studentInfoAdrList
                              on studentCourse.Chr_FK_PRN_NO equals studentInfoAdr.Chr_FK_PRN_NO
                          select new LinkDetail
                          {
                              SerialNo = serialNo,
                              Prn = studentCourse.Chr_FK_PRN_NO,
                              Mobile = studentInfoAdr.Num_MOBILE,
                              EmailId = studentInfoAdr.Chr_Student_Email,
                              BranchId = studentCourse.Num_FK_BR_CD,

                              NotMapped = new NotMapped
                              {
                                  Index = serialNo++,
                                  CoursePartName = coursePart?.Var_COPRT_DESC,
                                  StudentName = studentInfo.Var_ST_NM,
                                  Hide = false
                              }
                          };

        link.LinkDetails.AddRange(linkDetails);
    }

    private void GetFreshStudents(Link link, IReadOnlyCollection<Tbl_STUDENT_YR_CHNG> yrChanges)
    {
        var prnList = yrChanges.Select(y => y.Chr_FK_PRN_NO).Distinct().ToList();
        var studentInfoList = _coreService.Tbl_STUDENT_INFO_Repository
            .Get(s => prnList.Contains(s.Chr_PK_PRN_NO));
        var studentInfoAdrList = _coreService.Tbl_STUDENT_INFO_ADR_Repository.Get(s =>
            prnList.Contains(s.Chr_FK_PRN_NO));
        var coursePart =
            _coreService.Tbl_COURSE_PART_MSTR_Repository.Get(c => c.Num_PK_COPRT_NO == link.CoursePartId).FirstOrDefault();

        var serialNo = link.LinkDetails.Count + 1;
        //var index = serialNo;
        var linkDetails = from yrChange in yrChanges
                          join studentInfo in studentInfoList
                              on yrChange.Chr_FK_PRN_NO equals studentInfo.Chr_PK_PRN_NO
                          join studentInfoAdr in studentInfoAdrList
                              on yrChange.Chr_FK_PRN_NO equals studentInfoAdr.Chr_FK_PRN_NO
                          select new LinkDetail
                          {
                              SerialNo = serialNo,

                              BranchId = yrChange.Num_FK_BR_CD,

                              Prn = yrChange.Chr_FK_PRN_NO,
                              SeatNo = yrChange.Num_ST_SEAT_NO?.ToString(),
                              Mobile = studentInfoAdr.Num_MOBILE,
                              EmailId = studentInfoAdr.Chr_Student_Email,
                              Remarks = yrChange.Chr_ST_RESULT,

                              NotMapped = new NotMapped
                              {
                                  Index = serialNo++,
                                  CoursePartName = coursePart?.Var_COPRT_DESC,
                                  StudentName = studentInfo.Var_ST_NM,
                                  Hide = false
                              }
                          };

        //LogHandler.LogInfo($"Link Details Count 1 : {linkDetails.Count}");

        link.LinkDetails.AddRange(linkDetails);
    }

    private void GetFailStudents(Link link, int coursePartId, int loginInstanceId)
    {
        var failStatus = new List<string> { "D", "F", "N", "O", "R" };
        //var passStatus = new List<string> { "P" };
        var yrChanges = _coreService.Tbl_STUDENT_YR_CHNG_Repository
            .Get(y => /*y.Num_FK_INST_NO == (link.InstanceId ?? 0) && */
                y.Num_FK_INST_NO == (link.InstanceId ?? 0)/* && y.Num_FK_INST_NO < loginInstanceId*/ &&
                y.Num_FK_COL_CD == (link.CollegeId ?? 0) &&
                (y.Num_FK_DistCenter_ID ?? 0) == (link.CentreId ?? 0) && y.Num_FK_COPRT_NO == coursePartId &&
                failStatus.Contains(y.Chr_ST_RESULT)).AsEnumerable()
            .DistinctBy(p => p.Chr_FK_PRN_NO)
            .ToList();
        if (!yrChanges.Any())
            return;

        var prnList = yrChanges.Select(y => y.Chr_FK_PRN_NO).Distinct().ToList();
        var studentInfoList = _coreService.Tbl_STUDENT_INFO_Repository
            .Get(s => prnList.Contains(s.Chr_PK_PRN_NO));
        var studentInfoAdrList = _coreService.Tbl_STUDENT_INFO_ADR_Repository.Get(s =>
            prnList.Contains(s.Chr_FK_PRN_NO));
        var coursePart =
            _coreService.Tbl_COURSE_PART_MSTR_Repository.Get(c => c.Num_PK_COPRT_NO == link.CoursePartId).FirstOrDefault();

        var serialNo = link.LinkDetails.Count + 1;
        //var index = 1;
        var linkDetails = from yrChange in yrChanges
                          join studentInfo in studentInfoList
                              on yrChange.Chr_FK_PRN_NO equals studentInfo.Chr_PK_PRN_NO
                          join studentInfoAdr in studentInfoAdrList
                              on yrChange.Chr_FK_PRN_NO equals studentInfoAdr.Chr_FK_PRN_NO
                          select new LinkDetail
                          {
                              SerialNo = serialNo,

                              BranchId = yrChange.Num_FK_BR_CD,

                              Prn = yrChange.Chr_FK_PRN_NO,
                              Mobile = studentInfoAdr.Num_MOBILE,
                              EmailId = studentInfoAdr.Chr_Student_Email,
                              Remarks = yrChange.Chr_ST_RESULT,

                              NotMapped = new NotMapped
                              {
                                  Index = serialNo++,
                                  CoursePartName = coursePart?.Var_COPRT_DESC,
                                  StudentName = studentInfo.Var_ST_NM,
                                  Hide = false
                              }
                          };

        link.LinkDetails.AddRange(linkDetails);
    }

    private bool IsBranchApplicable(int coursePartId)
    {
        var sentCoursePart = _coreService.Tbl_COURSE_PART_MSTR_Repository.Get(c => c.Num_PK_COPRT_NO == coursePartId)
            .FirstOrDefault();
        return sentCoursePart?.Chr_COPRT_BRANCH_APP_FLG == "Y";
    }

    private void DeleteDuplicateRecords(Link link)
    {
        var recordsToDelete = link.LinkDetails
            .GroupBy(r => r.Prn)
            .Where(g => g.Select(r => r.Status).Distinct().Count() > 1) // Ensures both "P" and "N" exist
            .SelectMany(g => g) // Flattens the grouped records
            .ToList();

        link.LinkDetails.RemoveAll(r => recordsToDelete.Contains(r));
    }
    #endregion

    #region -- Public Methods --

    public EmailSetting GetEmailSettings()
    {
        return _emailSetting;
    }

    public bool IsLinkSent(string prn, FormType formType)
    {
        /*var linkDetails = _cornoService.LinkDetailRepository.Get(d =>
            d.Prn == prn && d.Status == StatusConstants.Sent).FirstOrDefault();

        return null != linkDetails;*/
        var linkId = _cornoService.LinkRepository.Get(l => l.FormTypeId == (int)formType && l.LinkDetails.Any(d =>
                d.Prn == prn && d.Status == StatusConstants.Sent), p => p.Id).FirstOrDefault();
        return linkId > 0;
    }

    public bool IsAdmitCardOpen(string prn)
    {
        var instanceId = _coreService.Tbl_STUDENT_YR_CHNG_Repository.Get(p =>
                p.Chr_FK_PRN_NO == prn && p.Num_FK_COL_CD == 45, p => p)
            .OrderByDescending(p => p.Num_FK_INST_NO).FirstOrDefault()?.Num_FK_INST_NO ?? 0;
        var isConvocationFrozen = _coreService.Tbl_TimeTableINST_Repository.Get(p => p.Num_FK_INST_NO == instanceId)
            .FirstOrDefault()?.Chr_FreezeConvocation;
        return !string.IsNullOrEmpty(isConvocationFrozen) && isConvocationFrozen == "Y";
    }

    public Link GetExamLink(string prn, FormType formType)
    {
        /*var link = _cornoService.LinkRepository.Get(l => l.FormTypeId == (int)formType && l.LinkDetails.Any(d => d.Prn == prn &&
               d.Status == StatusConstants.Sent))
            .FirstOrDefault();

        return link;*/
        var link = _cornoService.LinkRepository.Get(l => l.FormTypeId == (int)formType && l.LinkDetails.Any(d => d.Prn == prn &&
                d.Status == StatusConstants.Sent), l => l)
            .OrderByDescending(p => p.Id).FirstOrDefault();

        return link;
    }

    public void GetLinks(Link link, int loginInstanceId)
    {
        Enum.TryParse((link.FormTypeId ?? 0).ToString(), true, out FormType formType);
        switch (formType)
        {
            case FormType.Exam:
                GetExamLinks(link, loginInstanceId);
                break;
            case FormType.Environment:
                GetEnvironmentLinks(link);
                break;
            case FormType.Convocation:
                GetConvocationLinks(link, loginInstanceId);
                break;
        }
    }

    public void GetExamLinks(Link link, int loginInstanceId)
    {
        link.LinkDetails.Clear();

        var frozen = _coreService.Tbl_TimeTableINST_Repository
            .Get(t => t.Num_FK_INST_NO == link.InstanceId && t.Num_PK_CO_CD == link.CourseId).FirstOrDefault();
        if (frozen is { Chr_FreezeConvocation: "Y" })
            throw new Exception("The course for selected instance is frozen.");

        link.NotMapped.BranchApplicable = IsBranchApplicable(link.CoursePartId ?? 0);

        // Get Fail Students
        GetFailStudents(link, link.CoursePartId ?? 0, loginInstanceId);

        var lastCoursePart = ExamServerHelper.GetPreviousCoursePart(link.CoursePartId ?? 0, _coreService);
        // Check whether selected course part is first course part.
        if (null == lastCoursePart)
        {
            GetSem1Students(link, loginInstanceId);
            //GetSem1Students(link, link.InstanceId ?? 0);
            return;
        }

        var existingPrnList = link.LinkDetails.Select(d => d.Prn).Distinct().ToList();
        var freshStatus = new List<string> { "P", "T", "Z" };
        //var failStatus = new List<string> { "D", "F", "N", "O", "R" };
        var yrChanges = _coreService.Tbl_STUDENT_YR_CHNG_Repository.Get(y =>
            y.Num_FK_INST_NO == (link.InstanceId ?? 0) /*&& y.Num_FK_INST_NO < loginInstanceId*/ &&
            y.Num_FK_COL_CD == (link.CollegeId ?? 0) &&
            (y.Num_FK_DistCenter_ID ?? 0) == (link.CentreId ?? 0) &&
            y.Num_FK_COPRT_NO == lastCoursePart.Num_PK_COPRT_NO &&
            freshStatus.Contains(y.Chr_ST_RESULT) &&
            !existingPrnList.Contains(y.Chr_FK_PRN_NO))
            .OrderBy(y => y.Num_FK_INST_NO)
            .DistinctBy(y => y.Chr_FK_PRN_NO)
            .ToList();
        if (!yrChanges.Any())
            return;

        // Get Fresh students. Fetch from last course part.
        GetFreshStudents(link, yrChanges);

        // Delete Duplicate Prn
        DeleteDuplicateRecords(link);
    }

    public void GetBackLogStudents(Link link, int loginInstanceId)
    {
        link.LinkDetails.Clear();

        var frozen = _coreService.Tbl_TimeTableINST_Repository
            .Get(t => t.Num_FK_INST_NO == link.InstanceId && t.Num_PK_CO_CD == link.CourseId).FirstOrDefault();
        if (frozen is { Chr_FreezeConvocation: "Y" })
            throw new Exception("The course for selected instance is frozen.");

        link.NotMapped.BranchApplicable = IsBranchApplicable(link.CoursePartId ?? 0);

        //var failStatus = new List<string> { "D", "F", "N", "O", "R" };
        var yrChanges = _coreService.Tbl_STUDENT_YR_CHNG_Repository.Get(y =>
            y.Num_FK_INST_NO == (link.InstanceId ?? 0) && y.Num_FK_COL_CD == (link.CollegeId ?? 0) &&
            (y.Num_FK_DistCenter_ID ?? 0) == (link.CentreId ?? 0) && y.Num_FK_COPRT_NO == (link.CoursePartId ?? 0) &&
            y.Chr_ST_RESULT != "P").ToList();
        if (!yrChanges.Any())
            return;

        // Get Fresh students. Fetch from last course part.
        GetFreshStudents(link, yrChanges);
    }

    private void GetEnvironmentLinks(Link link)
    {
        // LogHandler.LogInfo($"In GetEnvironmentLinks");

        link.LinkDetails.Clear();

        link.NotMapped.BranchApplicable = IsBranchApplicable(link.CoursePartId ?? 0);

        /*var lastCoursePart = ExamServerHelper.GetPreviousCoursePart(link.CoursePartId ?? 0, _coreService);
        // Check whether selected course part is first course part.
        if (null == lastCoursePart)
        {
            GetSem1Students(link, loginInstanceId);
            return;
        }*/

        var yrChanges = _coreService.Tbl_STUDENT_YR_CHNG_Repository.Get(y =>
                y.Num_FK_INST_NO == (link.InstanceId ?? 0) && y.Num_FK_COL_CD == (link.CollegeId ?? 0) &&
                (y.Num_FK_DistCenter_ID ?? 0) == (link.CentreId ?? 0) && y.Num_FK_COPRT_NO == (link.CoursePartId ?? 0))
            .ToList();
        // LogHandler.LogInfo($"YR Changes List : {yrChanges.Count}, Instance : {link.InstanceId}, College : {link.CollegeId}, Dist. Center : {link.CentreId}, Course Part : {link.CoursePartId}");
        if (!yrChanges.Any())
            return;

        // Get Fresh students. Fetch from last course part.
        GetFreshStudents(link, yrChanges);

        // LogHandler.LogInfo($"Link Details Count : {link.LinkDetails.Count}");

        if (link.LinkDetails.Count <= 0) return;
        var prnList = link.LinkDetails.Select(d => d.Prn).Distinct().ToList();
        // Check whether exist in environment studies table
        var existingEnvironments = _coreService.TBl_STUDENT_ENV_STUDIES_Repository.Get(e =>
            prnList.Contains(e.Chr_FK_PRN_NO) && e.Chr_ST_SUB_RES == "P").ToList();
        // LogHandler.LogInfo($"Existing List : {existingEnvironments.Count}");
        var existingPrnList = existingEnvironments.Select(e => e.Chr_FK_PRN_NO).Distinct().ToList();
        // Now remove from link details which are available in environments
        link.LinkDetails.RemoveAll(d => existingPrnList.Contains(d.Prn));
        var serialNo = 1;
        link.LinkDetails.ForEach(d => d.SerialNo = serialNo++);
        link.LinkDetails.ForEach(d => d.NotMapped.Index = d.SerialNo ?? 0);
        // LogHandler.LogInfo($"Final Link Details Count : {link.LinkDetails.Count}");
    }

    private void GetConvocationLinks(Link link, int loginInstanceId)
    {
        link.LinkDetails.Clear();

        var frozen = _coreService.Tbl_TimeTableINST_Repository
            .Get(t => t.Num_FK_INST_NO == loginInstanceId && t.Num_PK_CO_CD == link.CourseId).FirstOrDefault();
        if (frozen is { Chr_FreezeConvocation: "Y" })
            throw new Exception("The course for instance is frozen.");

        link.NotMapped.BranchApplicable = IsBranchApplicable(link.CoursePartId ?? 0);

        var yrChanges = _coreService.Tbl_STUDENT_YR_CHNG_Repository.Get(y =>
            y.Num_FK_INST_NO == loginInstanceId && y.Num_FK_COL_CD == (link.CollegeId ?? 0) &&
            (y.Num_FK_DistCenter_ID ?? 0) == (link.CentreId ?? 0) && y.Num_FK_COPRT_NO == (link.CoursePartId ?? 0) &&
            y.Chr_ST_RESULT == "P").ToList();
        // LogHandler.LogInfo($"YR Changes List : {yrChanges.Count}, Instance : {link.InstanceId}, College : {link.CollegeId}, Dist. Center : {link.CentreId}, Course Part : {link.CoursePartId}");
        if (!yrChanges.Any())
            return;

        // Get Fresh students. Fetch from last course part.
        GetFreshStudents(link, yrChanges);

        //LogHandler.LogInfo($"Link Details Count : {link.LinkDetails.Count}");

        if (link.LinkDetails.Count <= 0) return;
        var seatNos = link.LinkDetails.Select(d => d.SeatNo.ToLong()).Distinct();
        // Check whether exist in environment studies table
        var existingConvocations = _coreService.Tbl_STUDENT_CONVO_Repository.Get(e =>
            seatNos.Contains(e.Num_ST_SEAT_NO ?? 0)).ToList();
        //LogHandler.LogInfo($"Existing List : {existingConvocations.Count}");*/
        var existingSeatNos = existingConvocations.Select(e => e.Num_ST_SEAT_NO?.ToString()).Distinct().ToList();
        // Now remove from link details which are available in environments
        link.LinkDetails.RemoveAll(d => existingSeatNos.Contains(d.SeatNo));
        var serialNo = 1;
        link.LinkDetails.ForEach(d => d.SerialNo = serialNo++);
        link.LinkDetails.ForEach(d => d.NotMapped.Index = d.SerialNo ?? 0);
        // LogHandler.LogInfo($"Final Link Details Count : {link.LinkDetails.Count}");
    }

    public Link GetExistingLink(Link link, int loginInstanceId)
    {
        var existing = _cornoService.LinkRepository.Get(e =>
                e.FormTypeId == link.FormTypeId &&
                e.InstanceId == loginInstanceId && e.CollegeId == link.CollegeId &&
                e.CentreId == link.CentreId && e.CourseTypeId == link.CourseTypeId &&
                e.CoursePartId == link.CoursePartId)
            .FirstOrDefault();

        if (null == existing) return null;

        link.NotMapped.BranchApplicable = IsBranchApplicable(link.CoursePartId ?? 0);

        var prnList = existing.LinkDetails.Select(y => y.Prn).Distinct().ToList();
        var studentInfoList = _coreService.Tbl_STUDENT_INFO_Repository
            .Get(s => prnList.Contains(s.Chr_PK_PRN_NO));

        var exams = _coreService.Tbl_APP_TEMP_Repository.Get(e => e.Num_FK_INST_NO == loginInstanceId &&
                                                                  prnList.Contains(e.Chr_APP_PRN_NO)).ToList();
        var convocations = _coreService.Tbl_STUDENT_CONVO_Repository.Get(e =>
            prnList.Contains(e.Chr_FK_PRN_NO)).ToList();
        var envStudies = _coreService.TBl_STUDENT_ENV_STUDIES_Repository.Get(e =>
            prnList.Contains(e.Chr_FK_PRN_NO)).ToList();
        var index = 1;
        const string newStatus = StatusConstants.Sent;
        existing.LinkDetails.ForEach(e =>
        {

            var transactionId = e.TransactionId;
            var paymentDate = e.PaymentDate;
            switch (link.FormTypeId)
            {
                case (int?)FormType.Exam:
                    {
                        var exam = exams.FirstOrDefault(p => p.Chr_APP_PRN_NO == e.Prn);
                        transactionId = exam?.Num_Transaction_Id;
                        paymentDate = exam?.PaymentDate;
                        break;

                    }
                case (int?)FormType.Convocation:
                    {
                        var convocation = convocations.FirstOrDefault(p => p.Chr_FK_PRN_NO == e.Prn);
                        transactionId = convocation?.Chr_Transaction_Id;
                        paymentDate = convocation?.PaymentDate;
                        break;
                    }
                case (int?)FormType.Environment:
                    {
                        var environment = envStudies.FirstOrDefault(p => p.Chr_FK_PRN_NO == e.Prn);
                        transactionId = environment?.Chr_Transaction_Id;
                        paymentDate = environment?.PaymentDate;
                        break;

                    }
            }
            e.Selected = e.Status == newStatus;
            e.TransactionId = transactionId;
            e.PaymentDate = paymentDate;
            e.NotMapped = new NotMapped
            {
                Index = index++,//e.Status == newStatus ? 0 : index++,
                StudentName = studentInfoList.FirstOrDefault(d => d.Chr_PK_PRN_NO == e.Prn)?
                    .Var_ST_NM,
                //Hide = e.Status == newStatus
            };
        });

        return existing;
    }

    public void UpdateBranches(Link link)
    {
        var branches = _coreService.Tbl_BRANCH_MSTR_Repository.Get()
            .Where(d => d.Num_FK_CO_CD == link.CourseId)
            .Select(d => new MasterViewModel
            {
                Code = d.Num_PK_BR_CD.ToString(),
                Id = d.Num_PK_BR_CD,
                Name = d.Var_BR_SHRT_NM,
                NameWithId = "(" + d.Num_PK_BR_CD + ") " + d.Var_BR_SHRT_NM
            })
            .OrderBy(b => b.Id)
            .ToList();

        link.LinkDetails.ForEach(d =>
        {
            d.BranchViewModel = branches.FirstOrDefault(b => b.Id == (d.BranchId ?? 0));
        });

        link.NotMapped.Branches = branches;
    }


    public void SendSmsAndEmail(Link link)
    {
        var selectedLinks = link.GetSelectedDetails().ToList();

        //var instanceId = selectedLinks?.FirstOrDefault()?.InstanceId?.ToInt();
        var selectedInstance = _coreService.Tbl_SYS_INST_Repository.Get(i => i.Num_PK_INST_SRNO == link.InstanceId)
            .FirstOrDefault();
        foreach (var detail in selectedLinks)
        {
            var linkUrl = GetLinkUrl();

            // Send SMS
            Enum.TryParse((link.FormTypeId ?? 0).ToString(), true, out FormType formType);
            detail.SmsResponse = SendLinkSms(detail.Mobile, selectedInstance?.Var_INST_REM, linkUrl, formType);

            // Send Email
            //detail.EmailResponse = SendEmail(detail.EmailId, linkUrl);

            detail.SentDate = DateTime.Now;
        }
    }

    public void Add(Link link)
    {
        link.ModifiedDate = DateTime.Now;

        const string oldStatus = StatusConstants.Active;
        //const string newStatus = StatusConstants.Sent;
        link.LinkDetails.ForEach(d =>
        {
            d.SentDate = d.Selected && d.Status == oldStatus ? DateTime.Now : null;
            //d.Status = d.Selected && d.Status == oldStatus ? newStatus : oldStatus;
            d.Status = StatusConstants.Active;
            d.ModifiedDate = DateTime.Now;
        });

        _cornoService.LinkRepository.Add(link);
        _cornoService.Save();
    }

    public void UpdateEnrollment(Link existing, Link newLink)
    {
        existing.UpdateUserData();

        var selectedLinks = newLink.LinkDetails;
        foreach (var detail in selectedLinks)
        {
            var existingDetail = existing.LinkDetails.FirstOrDefault(d =>
                d.Prn == detail.Prn);
            if (null == existingDetail)
            {
                existing.LinkDetails.Add(detail);
                continue;
            }

            existingDetail.SerialNo = newLink.InstanceId ?? 0;
            existingDetail.Code = detail.Code;
            existingDetail.BranchId = detail.BranchId;
            existingDetail.InstanceId = detail.InstanceId;
            existingDetail.Selected = true;

            existingDetail.UpdateUserData();
        }

        _cornoService.LinkRepository.Update(existing);
        _cornoService.Save();
    }

    public void UpdateLinks(Link existing, Link newLink)
    {
        existing.UpdateUserData();

        var selectedLinks = newLink.GetSelectedDetails();
        foreach (var detail in selectedLinks)
        {
            var existingDetail = existing.LinkDetails.FirstOrDefault(d =>
                d.Prn == detail.Prn);
            if (null == existingDetail) continue;

            existingDetail.SmsResponse = detail.SmsResponse;
            existingDetail.EmailResponse = detail.EmailResponse;
            existingDetail.SentDate = detail.SentDate;
            existingDetail.Status = StatusConstants.Sent;

            /*existingDetail.SentDate = detail.SentDate;
            existingDetail.Status = detail.Status;*/

            existingDetail.UpdateUserData();
        }

        _cornoService.LinkRepository.Update(existing);
        _cornoService.Save();
    }

    public LinkDetail UpdatePayment(string prn, string transactionId, double paidAmount, DateTime paymentDate,
        FormType formType, string ipAddress)
    {
        // Update Link
        var link = GetExamLink(prn, formType);
        if (null == link)
            throw new Exception($"No link found for PRN ({prn})");
        var linkDetail = link.LinkDetails.FirstOrDefault(d => d.Prn == prn);
        if (null == linkDetail)
            throw new Exception($"No link detail found for PRN ({prn})");

        linkDetail.TransactionId = transactionId;
        linkDetail.PaidAmount ??= 0;
        linkDetail.PaidAmount += paidAmount;
        linkDetail.PaymentDate = paymentDate;
        linkDetail.DeletedBy = ipAddress;
        linkDetail.Status = StatusConstants.Paid;

        _cornoService.LinkRepository.Update(link);
        _cornoService.Save();

        return linkDetail;
    }

    public void UpdateMobile(string prn, string mobile, string emailId)
    {
        // Update Link
        var linkDetails = _cornoService.LinkDetailRepository.Get(d => d.Prn == prn).ToList();

        linkDetails.ForEach(d =>
        {
            d.Mobile = mobile;
            d.EmailId = emailId;
        });

        foreach (var linkDetail in linkDetails)
            _cornoService.LinkDetailRepository.Update(linkDetail);
        _cornoService.Save();
    }

    public string SendPaymentSms(string mobile, string transactionId, FormType formType)
    {
        // Send message
        var paymentSms = formType switch
        {
            FormType.Exam => ConfigurationManager.AppSettings[SettingConstants.ExamPaymentSms],
            FormType.Environment => ConfigurationManager.AppSettings[SettingConstants.EnvironmentPaymentSms],
            FormType.Convocation => ConfigurationManager.AppSettings[SettingConstants.ConvocationPaymentSms],
            _ => throw new ArgumentOutOfRangeException(nameof(formType), formType, null)
        };

        if (string.IsNullOrEmpty(paymentSms))
            throw new Exception("Exam payment SMS message format not found.");
        var smsBody = paymentSms.Replace("{#var#}", $"{transactionId}");

        var smsUrl = _smsUrl.Replace("@mobileNo", mobile);
        smsUrl = smsUrl.Replace("@message", smsBody);

        LogHandler.LogInfo($"Payment SMS : {smsUrl}");

        return _smsService.SendSms(smsUrl);
    }

    public string SendEmail(string emailId, string subject, string linkUrl)
    {
        var emailSettings = new EmailSetting();
        _emailSetting.CopyPropertiesTo(emailSettings);

        emailSettings.To = emailId;
        emailSettings.Subject = subject;
        emailSettings.Body = emailSettings.Body.Replace("{#var1#}", linkUrl);
        return _emailService.SendEmail(emailSettings);
    }
    #endregion
}