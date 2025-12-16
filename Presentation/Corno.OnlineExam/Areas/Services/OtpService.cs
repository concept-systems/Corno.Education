using System;
using System.Linq;
using System.Web.WebPages;
using Corno.Data.Admin;
using Corno.Data.Core;
using Corno.Data.Corno;
using Corno.Data.Operation;
using Corno.Globals.Constants;
using Corno.OnlineExam.Areas.Services.Interfaces;
using Corno.Services.Bootstrapper;
using Corno.Services.Core;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno;
using Corno.Services.Corno.Interfaces;
using Corno.Services.SMS;
using Corno.Services.SMS.Interfaces;

namespace Corno.OnlineExam.Areas.Services;

public class OtpService : IOtpService
{
    #region -- Constructors --
    public OtpService()
    {
        _coreService = Bootstrapper.Get<ICoreService>();
        _cornoService = Bootstrapper.Get<ICornoService>();
        _linkService = Bootstrapper.Get<ILinkService>();
        _smsService = new SmsService();
            
        _expiryMinutes = 3;
    }
    #endregion

    #region -- Data Members --

    private readonly ICoreService _coreService;
    private readonly ICornoService _cornoService;
    private readonly ILinkService _linkService;
    private readonly ISmsService _smsService;

    private readonly int _expiryMinutes;
    #endregion

    #region -- Private Methods --

    /*private void ValidateMobile(string mobileNo)
    {
        var studentInfoAdr = _coreService.Tbl_STUDENT_INFO_ADR_Repository.Get(s => s.Num_MOBILE == mobileNo)
            .FirstOrDefault();

        if (null == studentInfoAdr)
            throw new Exception($"Mobile {mobileNo} does not exist in system.");
    }*/

    private Tbl_STUDENT_INFO_ADR ValidatePrn(string prn)
    {
        var studentInfoAdr = _coreService.Tbl_STUDENT_INFO_ADR_Repository.FirstOrDefault(s => s.Chr_FK_PRN_NO == prn,
            p => p);

        if (null == studentInfoAdr)
            throw new Exception($"PRN {prn} does not exist in system.");
        var mobile = studentInfoAdr.Num_MOBILE.Trim();
        if (string.IsNullOrEmpty(mobile))
            throw new Exception($"PRN {prn} does have mobile no assigned to it.");
        if (mobile.Length != 10)
            throw new Exception($"Invalid mobile no {mobile}.");

        return studentInfoAdr;
    }


    private void IsAlreadySent(string mobileNo)
    {
        // Check if otp is already sent.
        var otp = _cornoService.OtpRepository.Get(o => o.MobileNo == mobileNo && o.ExpiryTime >= DateTime.Now)
            .Select(o => o.Otp).OrderByDescending(p => p.IsDecimal()).FirstOrDefault();
        if (!string.IsNullOrEmpty(otp))
            throw new Exception($"OTP is already sent on your mobile.");
    }

    private void Add(string smsSendStatus, string transaction, string prn, string mobileNo, string otp)
    {
        // Save otp
        var sentTime = DateTime.Now;
        _cornoService.OtpRepository.Add(new TransactionOtp
        {
            Code = smsSendStatus,
            Transaction = transaction,
            PrnNo = prn,
            MobileNo = mobileNo,
            Otp = otp,
            SendTime = sentTime,
            ExpiryTime = sentTime.AddMinutes(3),
            Status = StatusConstants.Sent
        });
        _cornoService.Save();
    }

    private void Update(TransactionOtp transactionOtp, string status)
    {
        // Save otp
        transactionOtp.SendTime = DateTime.Now;
        transactionOtp.Status = status;
        _cornoService.OtpRepository.Update(transactionOtp);
        _cornoService.Save();
    }

    //private void CreateUser(LoginViewModel loginViewModel)
    //{
    //    var identityManager = new IdentityManager();

    //    var existingUser = 

    //    var user = new ApplicationUser()
    //    {
    //        UserName = model.UserName,
    //        FirstName = model.FirstName,
    //        LastName = model.LastName,
    //        Email = model.Email,
    //        CollegeId = model.CollegeId,
    //    };
            
    //    var result = idManager.CreateUser(user, model.Password);

    //    var existing = _cor
    //}
    #endregion

    #region -- Protected Methods --
    protected virtual string GenerateOtp()
    {
        // Generate message
        return new Random().Next(0, 999999).ToString().PadLeft(6, '0');
    }

    protected virtual string SendOtp(string smsUrl, string mobileNo, string message)
    {
        // Check if otp is already sent.
        IsAlreadySent(mobileNo);

        // Send message
        smsUrl = smsUrl.Replace("@mobileNo", mobileNo);
        smsUrl = smsUrl.Replace("@message", message);
        var smsSendStatus = _smsService.SendSms(smsUrl);

        return smsSendStatus;
    }
    #endregion

    #region -- Public Methods --

    //public virtual void UpdateSessions(OtpViewModel otpViewModel)
    //{
    //    // Check if user is college user 
    //    var staffCollegeId = _staffService.Get(s => s.Mobile1 == otpViewModel.MobileNo,
    //        s => s.CollegeId).FirstOrDefault();
    //    if (!(staffCollegeId > 0)) return;

    //    SessionGlobals.MySession.CollegeId = staffCollegeId;
    //    SessionGlobals.MySession.CollegeName = _collegeService.GetName(staffCollegeId ?? 0);
    //}

    public virtual string SendLoginOtp(LoginViewModel loginViewModel)
    {
        // Validate mobile no.
        /*var mobileNo = loginViewModel.MobileNo.Trim();// "9373333210";
        if (string.IsNullOrEmpty(mobileNo))
            throw new Exception("Your mobile no is not registered with us.");*/

        // Validate mobile
        //ValidateMobile(mobileNo);
        var studentInfoAdr = ValidatePrn(loginViewModel.Prn);
        var mobileNo = studentInfoAdr.Num_MOBILE.Trim();
        var email = studentInfoAdr.Chr_Student_Email.Trim();

        // Generate message
        // Check if otp is already sent.
        var existing = _cornoService.OtpRepository.Get(o => o.MobileNo == loginViewModel.Prn &&
                                                            o.ExpiryTime >= DateTime.Now)
            .OrderByDescending(p => p.Id).FirstOrDefault();
        var otp = null != existing ? existing.Otp : GenerateOtp();
        //throw new Exception($"OTP {loginViewModel.Otp} is already sent. It will be valid till 3 minutes.");
        // Validate message
        var operationRequest = new OperationRequest();
        _smsService.GetAppSettings(operationRequest);
        var smsUrl = operationRequest.Get<string>(ModelConstants.SmsUrl);
        var message = operationRequest.Get<string>(ModelConstants.LoginOtp);

        message = message.Replace("@formName", "Exam");
        message = message.Replace("@otp", otp);

        // Send message
        smsUrl = smsUrl.Replace("@mobileNo", mobileNo);
        smsUrl = smsUrl.Replace("@message", message);

        // Second, send SMS
        var smsSendStatus = _smsService.SendSms(smsUrl);
        if (string.IsNullOrEmpty(smsSendStatus))
            throw new Exception("Problem in sms sending SMS");

        // First, send mail
        _linkService.SendEmail(email, "Login OTP",message);

        if (!string.IsNullOrEmpty(smsSendStatus) && null != existing)
            Update(existing, StatusConstants.Resent);
        else
            Add(smsSendStatus, RoleConstants.Student, loginViewModel.Prn, mobileNo, otp);

        // Crete User if required
        //CreateUser(loginViewModel);

        return smsSendStatus;
    }

    public virtual bool ValidateOtp(LoginViewModel loginViewModel)
    {
        if (null == loginViewModel)
            throw new Exception("LoginViewModel is null.");

        //var mobileNo = loginViewModel.MobileNo;

        // Validate mobile
        //ValidateMobile(loginViewModel.Prn);
        var studentInfoAdr =  ValidatePrn(loginViewModel.Prn);
        var mobileNo = studentInfoAdr.Num_MOBILE.Trim();
        var otpEntity = _cornoService.OtpRepository.Get(o => o.MobileNo.Trim() == mobileNo &&
                                                             o.Otp == loginViewModel.Otp && (o.Status == StatusConstants.Sent || o.Status == StatusConstants.Resent ||
                                                                 o.Status == StatusConstants.Completed)).FirstOrDefault();
        if (null == otpEntity)
            throw new Exception("Invalid Otp");
        if (DateTime.Now > otpEntity.ExpiryTime)
            throw new Exception("Otp is expired");

        // Save and complete otp
        otpEntity.Status = StatusConstants.Completed;
        otpEntity.ModifiedDate = DateTime.Now;
        _cornoService.OtpRepository.Update(otpEntity);

        // Update User
        //UpdateUser(otpViewModel);

        return true;
    }
    #endregion
}