using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Net.Mail;
using Corno.Data.Common;

namespace Corno.Data.ViewModels;

public class EmailSetting : BaseModel
{
    public EmailSetting()
    {
        Attachments = new List<string>();
        StreamAttachments = new List<Attachment>();
    }

    public string TransactionType { get; set; }
    public string From { get; set; }
    public string Password { get; set; }
    public string SmtpServer { get; set; }
    public int? SmtpPort { get; set; }
    public bool? EnableSsl { get; set; }
    public string To { get; set; }
    public string Cc { get; set; }
    public string Bcc { get; set; }
    public string Subject { get; set; }
    public string Body { get; set; }

    [NotMapped]
    public List<string> Attachments { get; set; }
    [NotMapped]
    public List<Attachment> StreamAttachments { get; set; }
}