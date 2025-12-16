using System;
using Corno.Data.Corno;
using Corno.Data.ViewModels;
using Corno.Globals.Enums;

namespace Corno.Services.Corno.Interfaces;

public interface ILinkService : IBaseService
{
    #region -- Methods --

    EmailSetting GetEmailSettings();
    bool IsLinkSent(string prn, FormType formType);
    bool IsAdmitCardOpen(string prn);
    void GetLinks(Link link, int loginInstanceId);
    void GetExamLinks(Link link, int loginInstanceId);
    void GetBackLogStudents(Link link, int loginInstanceId);
    Link GetExamLink(string prn, FormType formType);
    Link GetExistingLink(Link link, int loginInstanceId);

    void SendSmsAndEmail(Link link);

    void Add(Link link);
    void UpdateEnrollment(Link existing, Link newLink);
    void UpdateLinks(Link existing, Link newLink);
    void UpdateBranches(Link link);
    LinkDetail UpdatePayment(string prn, string transactionId, double paidAmount, DateTime paymentDate, FormType formType, string ipAddress);
    void UpdateMobile(string prn, string mobile, string emailId);

    string SendPaymentSms(string mobile, string transactionId, FormType formType);
    string SendEmail(string emailId, string subject, string linkUrl);

    #endregion
}