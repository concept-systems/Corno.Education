using Corno.Services.SMS.Interfaces;
using System;
using System.Net;
using Corno.Globals.Constants;
using System.Configuration;
using Corno.Data.Operation;
using Corno.Services.Corno;

namespace Corno.Services.SMS;

public class SmsService : BaseService, ISmsService
{
    #region -- Methods --

    public void GetAppSettings(OperationRequest request)
    {
        // Validate message
        var smsUrl = ConfigurationManager.AppSettings[ModelConstants.SmsUrl];
        var loginMessage = ConfigurationManager.AppSettings[ModelConstants.LoginOtp];
        var marksEntryMessage = ConfigurationManager.AppSettings[ModelConstants.MarksEntryOtp];
        if (string.IsNullOrEmpty(loginMessage))
            throw new Exception("Login SMS format not available in system.");
        if (string.IsNullOrEmpty(marksEntryMessage) || string.IsNullOrEmpty(smsUrl))
            throw new Exception("No SMS format available in system.");
        request.AddData(ModelConstants.SmsUrl, smsUrl);
        request.AddData(ModelConstants.LoginOtp, loginMessage);
        request.AddData(ModelConstants.MarksEntryOtp, marksEntryMessage);
    }

    public string GenerateOtp()
    {
        // Generate message
        return new Random().Next(0, 999999).ToString().PadLeft(6, '0');
    }

    public virtual string SendSms(string uri)
    {
        try
        {
            var req = (HttpWebRequest)WebRequest.Create(uri);
            req.UserAgent = ".NET Framework Client";
            req.ContentType = "application/x-www-form-urlencoded";
            var response = (HttpWebResponse)req.GetResponse();
            var stream = response.GetResponseStream();

            if (stream == null) return null;

            var sr = new System.IO.StreamReader(stream);
            return sr.ReadToEnd().Trim();
        }
        catch (WebException ex)
        {
            if (!(ex.Response is HttpWebResponse httpWebResponse))
                return null;

            switch (httpWebResponse.StatusCode)
            {
                case HttpStatusCode.NotFound:
                    return "404:URL not found :" + uri;
                case HttpStatusCode.BadRequest:
                    return "400:Bad Request";
                default:
                    return httpWebResponse.StatusCode.ToString();
            }
        }
    }


    public virtual string SendSms(string phoneNo, string smsBody)
    {
        throw new NotImplementedException();
    }

    public virtual void SaveSmsLog(string phoneNo, string smsBody, string smsResult,
        DateTime transactionDate, string transactionType, int departmentId)
    {
        throw new NotImplementedException();
    }
}
#endregion