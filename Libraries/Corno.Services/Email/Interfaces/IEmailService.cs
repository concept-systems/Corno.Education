using Corno.Data.ViewModels;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Email.Interfaces;

public interface IEmailService : IBaseService
{
    #region -- Methods --

    //string GetAttachmentFolderPath(string category, string settingName);

    string SendEmailAsync (string toAddress, string subject, string body);

    string SendEmailAsync(string senderId, string senderPassword,
        string smtpAddress, int smtpPort,
        string toAddress, string subject, string body, string filePath);

    string SendEmail(string senderId, string senderPassword,
        string smtpAddress, int smtpPort, string fromAddress,
        string toAddress, string cc, string subject, string body, string filePath, bool bEnableSsl);

    string SendEmail(EmailSetting emailSetting);

    #endregion
}