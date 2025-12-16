using Corno.Data.Operation;
using System;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.SMS.Interfaces;

public interface ISmsService : IBaseService
{
    #region -- Methods --

    void GetAppSettings(OperationRequest request);
    string GenerateOtp();
    string SendSms(string uri);
    string SendSms(string phoneNo, string smsBody);
    void SaveSmsLog(string phoneNo, string smsBody, string smsResult,
        DateTime transactionDate, string transactionType, int departmentId);
    #endregion
}