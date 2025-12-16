using System;
using System.Collections.Generic;
using System.Configuration;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web.Mvc;
using Corno.Data.Operation;
using Corno.Data.Payment;
using Corno.Globals.Constants;
using Corno.Services.Corno;
using Corno.Services.Payment.Interfaces;

namespace Corno.Services.Payment;

public class EaseBuzzService : BaseService, IEaseBuzzService
{
    #region -- Constructors --
    public EaseBuzzService()
    {
        _enableIframe = ConfigurationManager.AppSettings["enable_iframe"];

        _salt = ConfigurationManager.AppSettings["salt"];
        _key = ConfigurationManager.AppSettings["key"];
        _environment = ConfigurationManager.AppSettings["env"];

        _merchantEmail = ConfigurationManager.AppSettings["merchant_email"];
    }
    #endregion

    #region -- Data Members --
    private readonly string _enableIframe;
    private readonly string _salt;
    private readonly string _key;
    private readonly string _environment;
    private readonly string _merchantEmail;
    #endregion

    #region -- Private Methods --
    // hash code generation
    private static string GenerateHash512(string text)
    {
        var message = Encoding.UTF8.GetBytes(text);

        //var ue = new UnicodeEncoding();
        var hashString = new SHA512Managed();
        var hashValue = hashString.ComputeHash(message);
        return hashValue.Aggregate("", (current, x) => current + $"{x:x2}");

    }
    #endregion

    #region -- Public Methods --
    public string PaymentApiRequest(OperationRequest operationRequest)
    {
        var transactionId = operationRequest.Get<string>(ModelConstants.TransactionId);
        var formType = operationRequest.Get<string>(ModelConstants.FormType);
        var collegeId = operationRequest.Get<string>(ModelConstants.CollegeId);
        var courseId = operationRequest.Get<string>(ModelConstants.CourseId);
        var instanceId = operationRequest.Get<string>(ModelConstants.InstanceId);
        var prn = operationRequest.Get<string>(ModelConstants.PrnNo);
        var studentName = operationRequest.Get<string>(ModelConstants.StudentName);
        var email = operationRequest.Get<string>(ModelConstants.Email);
        var phone = operationRequest.Get<string>(ModelConstants.Mobile);
        var amount = operationRequest.Get<string>(ModelConstants.Amount);
        var productInfo = operationRequest.Get<string>(ModelConstants.ProductInfo);
            
        var successUrl = operationRequest.Get<string>(ModelConstants.SuccessUrl);
        var failureUrl = operationRequest.Get<string>(ModelConstants.FailureUrl);
            
        var showPaymentMode = operationRequest.Get<string>(ModelConstants.PaymentMode);
        var splitPayments = operationRequest.Get<string>(ModelConstants.SplitPayments);
        var subMerchantId = operationRequest.Get<string>(ModelConstants.SubMerchantId);

        /*var salt = operationRequest.Get<string>(ModelConstants.Salt);
        var key = operationRequest.Get<string>(ModelConstants.MerchantId);
        var environment = ConfigurationManager.AppSettings["env"];
        var isEnableIFrame = ConfigurationManager.AppSettings["enable_iframe"];*/

        //string show_payment_mode = Request.Form["show_payment_mode"].Trim();

        //string splitPayments = Request.Form["split_payments"].Trim();
        //string subMerchantId = Request.Form["sub_merchant_id"].Trim();

        var dict = new Dictionary<string, string>
        {
            { "txnid", transactionId },
            { "key", _key },
            { "amount", amount},
            { "firstname", studentName?.Trim() },
            { "email", email?.Trim() },
            { "phone", phone?.Trim() },
            { "productinfo", productInfo?.Trim() },
            { "surl", successUrl.Trim() },
            { "furl", failureUrl.Trim() },
            { "udf1", prn },
            { "udf2", collegeId },
            { "udf3", courseId },
            { "udf4", instanceId },
            { "udf5", formType },
            { "udf6", string.Empty },
            { "udf7", string.Empty },
            { "udf8", string.Empty },
            { "udf9", string.Empty },
            { "udf10", string.Empty },
            { "show_payment_mode", showPaymentMode?.Trim() }
        };

        if (splitPayments?.Length > 0)
            dict.Add("split_payments", splitPayments);
        if (subMerchantId?.Length > 0)
            dict.Add("sub_merchant_id", subMerchantId);

        var easeBuzz = new EaseBuzz(_salt, _key, _environment, "false");
        var result = easeBuzz.InitiatePaymentApi(dict);

        if (_enableIframe == "true")
        {
            //if (((JToken)result).Type == JTokenType.Object)
            //    return result; //Response.Write(result);
            //else
            //{
            //    accessKey = result;
            //    ClientScript.RegisterClientScriptBlock(GetType(), "Javascript", "processPayment()", true);
            //}
        }
        else
        {
            var isUri = Uri.IsWellFormedUriString(result, UriKind.RelativeOrAbsolute);
            return isUri
                ? $"<script type='text/javascript'>window.open('{result}', '_self');</script>"
                : result;
        }

        return string.Empty;

        //var EasyBuzzRequest = new EasyBuzzRequest
        //{
        //    OrderId = orderId,
        //    Amount = amount
        //};
        /*var payTmParams = payTmRequest.GetPayTmParams();

        var payTmChecksum = Checksum.generateSignature(payTmParams, payTmRequest.MerchantKey);
        bool verifySignature = Checksum.verifySignature(payTmParams, payTmRequest.MerchantKey,
            payTmChecksum);

        /* for Staging #1#
        var url = "https://securegw-stage.paytm.in/order/process";
        /* for Production #1#
        // String url = "https://securegw.paytm.in/order/process";
        /* Prepare HTML Form and Submit to Paytm #1#
        var outputHtml = "";
        outputHtml += "<html>";
        outputHtml += "<head>";
        outputHtml += "<title>Merchant Checkout Page</title>";
        outputHtml += "</head>";
        outputHtml += "<body>";
        outputHtml += "<center><h1>Please do not refresh this page...</h1></center>";
        outputHtml += "<form method='post' action='" + url + "' name='paytm_form'>";
        foreach (var key in payTmParams.Keys)
        {
            outputHtml += "<input type='hidden' name='" + key + "' value='" + payTmParams[key] + "'>";
        }
        outputHtml += "<input type='hidden' name='CHECKSUMHASH' value='" + payTmChecksum + "'>";
        outputHtml += "</form>";
        outputHtml += "<script type='text/javascript'>";
        outputHtml += "document.paytm_form.submit();";
        outputHtml += "</script>";
        outputHtml += "</body>";
        outputHtml += "</html>";
        
        //Response.Write(outputHtml);
        return outputHtml;*/
    }

    public void PaymentApiResponse(FormCollection form)
    {
        //var orderId = string.Empty;
        const string hashSeq = "key|txnid|amount|productinfo|firstname|email|udf1|udf2|udf3|udf4|udf5|udf6|udf7|udf8|udf9|udf10";
            
        var merchantHashVarSeq = hashSeq.Split('|');
        Array.Reverse(merchantHashVarSeq);
        var merchantHashString = $"{ConfigurationManager.AppSettings["salt"]}|{form["status"]}";
        foreach (var merchantHashVar in merchantHashVarSeq)
        {
            merchantHashString += "|";
            merchantHashString = merchantHashString + (form[merchantHashVar] != null ? form[merchantHashVar] : "");
        }
        var merchantHash = GenerateHash512(merchantHashString).ToLower();
        if (merchantHash != form["hash"])
            throw new Exception($"Hash value does not match. Prn : {form["udf1"]}, Hash value: {form["hash"]}");

        //orderId = form["txnid"];

        //Response.Write("value matched");+		this	{ASP.success_aspx}	easebuzz_.net.success {ASP.success_aspx}

        /*if (form["status"] == "success")
        {
            Response.Write(form);
        }
        else
        {
            Response.Write(form);
        }*/
        //Hash value did not matched
    }

    public string TransactionApi(OperationRequest operationRequest)
    {
        /*var transactionId = "161023093621066";
        var amount = "4760.0";
        var email = "vvyerge21-ece@bvucoep.edu.in";
        var phone = "9322906439";*/

        var transactionId = operationRequest.Get<string>(ModelConstants.TransactionId);
        var email = operationRequest.Get<string>(ModelConstants.Email);
        var phone = operationRequest.Get<string>(ModelConstants.Mobile);
        var amount = operationRequest.Get<string>(ModelConstants.Amount);

        //LogHandler.LogInfo($"TransId : {transactionId}, Email : {email}, phone : {phone}, amount : {amount}");

        var easeBuzz = new EaseBuzz(_salt, _key, _environment, "false");
        var result = easeBuzz.TransactionApi(transactionId, amount, email, phone);

        return result;
    }

    public string TransactionDateApi(OperationRequest operationRequest)
    {
        var transactionDate = operationRequest.Get<string>(ModelConstants.TransactionDate);
            
        var easeBuzz = new EaseBuzz(_salt, _key, _environment, "false");
        var result = easeBuzz.TransactionDateApi(_merchantEmail, transactionDate);

        return result;
    }

    public string SendPayoutRequest(OperationRequest operationRequest)
    {
        var settlementDate = operationRequest.Get<string>(ModelConstants.SettlementDate);

        var easeBuzz = new EaseBuzz(_salt, _key, _environment, "false");
        var result = easeBuzz.PayoutApi(_merchantEmail, settlementDate);

        return result;
    }

    #endregion
}