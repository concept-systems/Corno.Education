using System;
using System.Net;
using System.Net.Mail;
using Corno.Data.ViewModels;
using Corno.Logger;
using Corno.Services.Corno;
using Corno.Services.Email.Interfaces;

namespace Corno.Services.Email;

public class EmailService : BaseService, IEmailService
{
    #region -- Methods --

    //public string GetAttachmentFolderPath(string category, string settingName)
    //{
    //    var settingService = Bootstrapper.Get<ISettingService>();
    //    var emailFolderPath = settingService.Get(category, settingName);
    //    if (string.IsNullOrEmpty(emailFolderPath))
    //        return string.Empty;
    //    if (!Directory.Exists(emailFolderPath))
    //        Directory.CreateDirectory(emailFolderPath);
    //    return emailFolderPath;
    //}

    public string SendEmailAsync(string toAddress, string subject, string body)
    {
        var result = "Message Sent Successfully..!!";
        var senderId = "info@4everpayment.com";// use sender’s email id here..
        const string senderPassword = "jugad4ever"; // sender password here…
        try
        {
            var smtp = new SmtpClient
            {
                Host = "smtp.gmail.com", // smtp server address here…
                Port = 587,
                EnableSsl = true,
                DeliveryMethod = SmtpDeliveryMethod.Network,
                Credentials = new NetworkCredential(senderId, senderPassword),
                Timeout = 30000
            };
            var message = new MailMessage(senderId, toAddress, subject, body) { IsBodyHtml = true };
            smtp.Send(message);

            // Release attachments
            foreach (var attachment in message.Attachments)
                attachment.Dispose();
        }
        catch (Exception exception)
        {
            result = "Error sending email.!!! : " + exception.Message;
        }
        return result;
    }

    public string SendEmailAsync(string senderId, string senderPassword,
        string smtpAddress, int smtpPort,
        string toAddress, string subject, string body, string filePath)
    {
        const string result = "Message Sent Successfully..!!";
        var smtp = new SmtpClient
        {
            Host = smtpAddress, // smtp server address here…
            Port = smtpPort,
            EnableSsl = true,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            Credentials = new NetworkCredential(senderId, senderPassword),
            Timeout = 30000
        };
        var message = new MailMessage(senderId, toAddress, subject, body) { IsBodyHtml = true };
        if (!string.IsNullOrEmpty(filePath))
            message.Attachments.Add(new Attachment(filePath));
        smtp.SendAsync(message, null);

        // Release attachments
        foreach (var attachment in message.Attachments)
            attachment.Dispose();

        return result;
    }

    public string SendEmail(string senderId, string senderPassword,
        string smtpAddress, int smtpPort, string fromAddress,
        string toAddress, string cc, string subject, string body, string filePath, bool bEnableSsl)
    {
        const string result = "Message Sent Successfully..!!";
        var smtp = new SmtpClient
        {
            Host = smtpAddress, // smtp server address here…
            Port = smtpPort,
            EnableSsl = bEnableSsl,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(senderId, senderPassword, "MicrosoftOffice365Domain.com"),
            TargetName = "STARTTLS/smtp.office365.com",
            Timeout = Int32.MaxValue,
        };

        //LogHandler.LogInfo($"From : {fromAddress}, To : {toAddress}, Sender Id : {senderId}");

        var mailMessage = new MailMessage(fromAddress, toAddress, subject, body){ IsBodyHtml = true};
        if (!string.IsNullOrEmpty(cc))
            mailMessage.CC.Add(new MailAddress(cc));

        if (!string.IsNullOrEmpty(filePath))
            mailMessage.Attachments.Add(new Attachment(filePath));
        smtp.Send(mailMessage);

        // Release attachments
        foreach (var attachment in mailMessage.Attachments)
            attachment.Dispose();

        return result;
    }

    /*public string SendEmail(EmailSetting emailSetting)
    {
        const string result = "Message Sent Successfully..!!";

        using (var smtpClient = new SmtpClient
               {
                   Host = emailSetting.SmtpServer, // SMTP server address here…
                   Port = emailSetting.SmtpPort ?? 0,
                   EnableSsl = emailSetting.EnableSsl ?? false,
                   DeliveryMethod = SmtpDeliveryMethod.Network,
                   UseDefaultCredentials = false,
                   Credentials = new NetworkCredential(emailSetting.From, emailSetting.Password),
                   Timeout = int.MaxValue,
               })
        using (var message = new MailMessage(emailSetting.From, emailSetting.To,
                       emailSetting.Subject, emailSetting.Body)
                   { IsBodyHtml = true })
        {
            if (!string.IsNullOrEmpty(emailSetting.Cc))
                message.CC.Add(new MailAddress(emailSetting.Cc));

            foreach (var attachment in emailSetting.Attachments)
                message.Attachments.Add(new Attachment(attachment));

            foreach (var attachment in emailSetting.StreamAttachments)
            {
                // Ensure the stream is at the beginning
                if (attachment.ContentStream.CanSeek)
                {
                    attachment.ContentStream.Position = 0;
                }
                message.Attachments.Add(attachment);
            }

            smtpClient.Send(message);

            LogHandler.LogInfo("Message sent.");

            /#1#/ Explicitly dispose of attachments
            foreach (var attachment in message.Attachments)
                attachment.Dispose();#1#
        }

        LogHandler.LogInfo($"Returning to caller : {result}");

        return result;
    }*/



    public string SendEmail(EmailSetting emailSetting)
    {
        const string result = "Message Sent Successfully..!!";
        var smtpClient = new SmtpClient
        {
            Host = emailSetting.SmtpServer, // smtp server address here…
            Port = emailSetting.SmtpPort ?? 0,
            EnableSsl = emailSetting.EnableSsl ?? false,
            DeliveryMethod = SmtpDeliveryMethod.Network,
            UseDefaultCredentials = false,
            Credentials = new NetworkCredential(emailSetting.From, emailSetting.Password),
            Timeout = int.MaxValue,
        };
        var message = new MailMessage(emailSetting.From, emailSetting.To,
                emailSetting.Subject, emailSetting.Body)
            { IsBodyHtml = true };
        if (!string.IsNullOrEmpty(emailSetting.Cc))
            message.CC.Add(new MailAddress(emailSetting.Cc));
        foreach (var attachment in emailSetting.Attachments)
            message.Attachments.Add(new Attachment(attachment));
        foreach (var attachment in emailSetting.StreamAttachments)
            message.Attachments.Add(attachment);
        smtpClient.Send(message);

        /*// Release attachments
        foreach (var attachment in message.Attachments)
            attachment.Dispose();

        message.Attachments.Clear();

        smtpClient.Dispose();*/

        return result;
    }

    #endregion
}