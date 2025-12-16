using Newtonsoft.Json;
using RestSharp;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Security.Cryptography;
using System.Text;

namespace Corno.Data.Payment;

public class EaseBuzz
{
    #region -- Constructors --
    public EaseBuzz(string salt, string key, string environment, string isSeamlessRequest)
    {
        Salt = salt;
        Key = key;
        Environment = environment;
        IsEnableSeamless = isSeamlessRequest ?? "false";

        TransactionId = string.Empty;
        EmptyValue = string.Empty;

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;
    }
    #endregion


    #region -- Properties --
    public string ActionUrl { get; }
    public string Hash { get; set; }
    public string TransactionId { get; set; }
    public string MerchantKey { get; set; }
    public string Salt { get; set; }
    public string Key { get; set; }
    public string Environment { get; }
    public string IsEnableIFrame { get; set; }
    public string IsEnableSeamless { get; set; }
    public string EmptyValue { get; private set; }
    #endregion


    #region -- Private Methods --
    private bool EmptyValidation(Dictionary<string, string> dictionary)
    {
        var isValid = false;

        if (dictionary != null)
        {
            if (string.IsNullOrEmpty(dictionary["key"]))
            {
                EmptyValue = "Merchant Key";
                isValid = true;
            }
            else if (string.IsNullOrEmpty(dictionary["txnid"]))
            {
                EmptyValue = "Transaction Id";
                isValid = true;
            }
            else if (string.IsNullOrEmpty(dictionary["amount"]))
            {
                EmptyValue = "Amount";
                isValid = true;
            }
            else if (string.IsNullOrEmpty(dictionary["productinfo"]))
            {
                EmptyValue = "Product Information";
                isValid = true;
            }
            else if (string.IsNullOrEmpty(dictionary["firstname"]))
            {
                EmptyValue = "First Name";
                isValid = true;
            }
            else if (string.IsNullOrEmpty(dictionary["email"]))
            {
                EmptyValue = "Email";
                isValid = true;
            }
            else if (string.IsNullOrEmpty(dictionary["phone"]))
            {
                EmptyValue = "Phone";
                isValid = true;
            }
            else if (!string.IsNullOrEmpty(dictionary["phone"]))
            {
                if (dictionary["phone"].Length == 10) return isValid;
                EmptyValue = "Phone number must be 10 digit";
                isValid = true;
            }
            else if (string.IsNullOrEmpty(dictionary["surl"]))
            {
                EmptyValue = "Success URL";
                isValid = true;
            }
            else if (string.IsNullOrEmpty(dictionary["furl"]))
            {
                EmptyValue = "Failure URL";
                isValid = true;
            }
            else if (string.IsNullOrEmpty(Salt))
            {
                EmptyValue = "Merchant Salt Key";
                isValid = true;
            }
        }
        else
        {
            isValid = false;
        }
        return isValid;
    }

        
    // hashcode generation
    private static string GenerateHash512(string text)
    {

        var message = Encoding.UTF8.GetBytes(text);

        //var ue = new UnicodeEncoding();
        var hashString = new SHA512Managed();
        var hashValue = hashString.ComputeHash(message);

        return hashValue.Aggregate("", (current, x) => current + $"{x:x2}");
    }


    //get url using env variable
    private string GetPaymentApiUrl()
    {
        var paymentUrl = Environment switch
        {
            "prod" => "https://pay.easebuzz.in",
            _ => "https://testpay.easebuzz.in"
        };
        return paymentUrl;
    }

    private string GetTransactionApiUrl()
    {
        var paymentUrl = Environment switch
        {
            "prod" => "https://dashboard.easebuzz.in/transaction/v1/retrieve",
            _ => "https://testdashboard.easebuzz.in/transaction/v1/retrieve"
        };
        return paymentUrl;
    }
    #endregion

    #region -- Public Methods --
    // this function is required to initiate payment
    [Obsolete]
    public string InitiatePaymentApi(Dictionary<string, string> dictionary)
    {
        var result = "";

        if (EmptyValidation(dictionary))
        {
            var obj = new
            {
                status = "0",
                data = $"Mandatory parameter {EmptyValue} can not empty"
            };
            return obj.ToString();

        }

        // generate hash
        var hashVarsSeq = dictionary["key"] + "|" + dictionary["txnid"] + "|" + dictionary["amount"] + "|" + dictionary["productinfo"] + "|" + dictionary["firstname"] + "|"
                          + dictionary["email"] + "|" + dictionary["udf1"] + "|" + dictionary["udf2"] + "|" + dictionary["udf3"] + "|" + dictionary["udf4"] + "|" + dictionary["udf5"] + "|" + dictionary["udf6"] + "|" + dictionary["udf7"] + "|"
                          + dictionary["udf8"] + "|" + dictionary["udf9"] + "|" + dictionary["udf10"] + "|" + Salt; // splitting hash sequence from config

        Hash = GenerateHash512(hashVarsSeq).ToLower();        //generating hash

        dictionary.Add("hash", Hash);

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        var client = new RestClient(GetPaymentApiUrl());
        var request = new RestRequest("/payment/initiateLink");

        foreach (var data in dictionary)
        {
            request.AddParameter(data.Key, data.Value);
        }
        var response = client.Post(request);

        var responseDict = JsonConvert.DeserializeObject<Dictionary<string, string>>(response.Content ?? throw new InvalidOperationException());

        IsEnableIFrame = System.Configuration.ConfigurationSettings.AppSettings["enable_iframe"] ?? "false";

        if (responseDict != null && responseDict["status"] == "1")
        {
            if (string.IsNullOrEmpty(responseDict["data"])) return result;
            if (IsEnableSeamless == "true")
                result = responseDict["data"];
            else if (IsEnableIFrame == "true")
                result = responseDict["data"];
            else
                result = $"{GetPaymentApiUrl()}/pay/{responseDict["data"]}";
        }
        else
            result = response.Content;

        return result;
    }

    //initiate refund api 
    public string RefundApi(string txnId, string refundAmount, string phone, string amount, string email)
    {
        var data = new System.Collections.Hashtable
        {
            { "txnid", txnId.Trim() },
            { "refund_amount", refundAmount.Trim() },
            { "key", Key },
            //amount = AmountForm;
            //string AmountForm = Convert.ToDecimal(amount.Trim()).ToString("g29");// eliminating trailing zeros
            { "amount", amount },
            { "email", email.Trim() },
            { "phone", phone.Trim() }
        }; // adding values in gash table for data post

        // generate hash
        var hashVarsSeq = "key|txnid|amount|refund_amount|email|phone".Split('|'); // splitting hash sequence from config
        var hashString = "";
        foreach (var hashVar in hashVarsSeq)
        {
            hashString += (data.ContainsKey(hashVar) ? data[hashVar].ToString() : "");
            hashString += '|';
        }
        hashString += Salt;// appending SALT
        Console.WriteLine(hashString);
        Hash = GenerateHash512(hashString).ToLower();        //generating hash
        data.Add("hash", Hash);

        var postData = "txnid=" + txnId;
        postData += "&refund_amount=" + refundAmount;
        postData += "&phone=" + phone;
        postData += "&key=" + Key;
        postData += "&amount=" + amount;
        postData += "&email=" + email;
        postData += "&hash=" + Hash;

        const string url = "https://dashboard.easebuzz.in/transaction/v1/refund";

        var request = (HttpWebRequest)WebRequest.Create(url);

        var nData = Encoding.ASCII.GetBytes(postData);

        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = nData.Length;

        using (var stream = request.GetRequestStream())
        {
            stream.Write(nData, 0, nData.Length);
        }

        var response = (HttpWebResponse)request.GetResponse();

        var responseString = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()).ReadToEnd();

        return responseString;
    }

    //initiates transaction api 
    public string TransactionApi(string txnId, string amount, string email, string phone)
    {
        var data = new System.Collections.Hashtable
        {
            { "key", Key },
            { "txnid", txnId },
            { "amount", amount },
            { "email", email },
            { "phone", phone }
        };

        //LogHandler.LogInfo($"Key : {Key}, Transaction Id : {txnId}, Amount : {amount}, Email: {email}, Phone : {phone}");

        // generate hash
        var hashVarsSeq = "key|txnid|amount|email|phone".Split('|'); // splitting hash sequence from config
        var hashString = "";
        foreach (var hashVar in hashVarsSeq)
        {
            hashString += (data.ContainsKey(hashVar) ? data[hashVar].ToString() : "");
            hashString += '|';
        }
        hashString += Salt;// appending SALT
        Console.WriteLine(hashString);
        Hash = GenerateHash512(hashString).ToLower();        //generating hash
        data.Add("hash", Hash);

        ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls12;

        //var url = "https://dashboard.easebuzz.in/transaction/v1/retrieve";
        //var url = "https://testdashboard.easebuzz.in/transaction/v1/retrieve";
        var url = GetTransactionApiUrl();
        var request = (HttpWebRequest)WebRequest.Create(url);

        var postData = "txnid=" + txnId;
        postData += "&amount=" + amount;
        postData += "&email=" + email;
        postData += "&phone=" + phone;
        postData += "&key=" + Key;
        postData += "&hash=" + Hash;

        var nData = Encoding.ASCII.GetBytes(postData);

        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = nData.Length;

        using (var stream = request.GetRequestStream())
            stream.Write(nData, 0, nData.Length);

        var response = (HttpWebResponse)request.GetResponse();

        var responseString = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()).ReadToEnd();
        //Response.Write(responseString);
        //string testResponse = "take it or leave it </br>";
        return responseString;
    }

    // Initiate transactionDateAPI api 
    public string TransactionDateApi(string merchantEmail, string transactionDate)
    {
        var data = new System.Collections.Hashtable
        {
            { "key", Key },
            { "merchant_email", merchantEmail },
            { "transaction_date", transactionDate }
        };
        // generate hash
        var hashVarsSeq = "key|merchant_email|transaction_date".Split('|'); // splitting hash sequence from config
        var hashString = "";
        foreach (var hashVar in hashVarsSeq)
        {
            hashString += (data.ContainsKey(hashVar) ? data[hashVar].ToString() : "");
            hashString += '|';
        }
        hashString += Salt;// appending SALT
        Hash = GenerateHash512(hashString).ToLower();        //generating hash
        data.Add("hash", Hash);

        //const string url = "https://dashboard.easebuzz.in/transaction/v1/retrieve/date";
        var url = GetTransactionApiUrl();
        var request = (HttpWebRequest)WebRequest.Create(url);

        var postData = "merchant_key=" + Key;
        postData += "&merchant_email=" + merchantEmail;
        postData += "&transaction_date=" + transactionDate;
        postData += "&hash=" + Hash;

        var nData = Encoding.ASCII.GetBytes(postData);

        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = nData.Length;

        using (var stream = request.GetRequestStream())
        {
            stream.Write(nData, 0, nData.Length);
        }

        var response = (HttpWebResponse)request.GetResponse();

        var responseString = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()).ReadToEnd();
        return responseString;
    }

    // Initiate payoutAPI api 
    public string PayoutApi(string merchantEmail, string payoutDate)
    {
        var data = new System.Collections.Hashtable
        {
            { "key", Key },
            { "merchant_email", merchantEmail },
            { "payout_date", payoutDate }
        };

        // generate hash
        var hashVarsSeq = "key|merchant_email|payout_date".Split('|'); // splitting hash sequence from config
        var hashString = "";
        foreach (var hashVar in hashVarsSeq)
        {
            hashString += (data.ContainsKey(hashVar) ? data[hashVar].ToString() : "");
            hashString += '|';
        }
        hashString += Salt;// appending SALT
        Hash = GenerateHash512(hashString).ToLower();        //generating hash
        data.Add("hash", Hash);

        var url = "https://dashboard.easebuzz.in/payout/v1/retrieve";
        var request = (HttpWebRequest)WebRequest.Create(url);

        var postData = "merchant_key=" + Key;
        postData += "&merchant_email=" + merchantEmail;
        postData += "&payout_date=" + payoutDate;
        postData += "&hash=" + Hash;
        var nData = Encoding.ASCII.GetBytes(postData);

        request.Method = "POST";
        request.ContentType = "application/x-www-form-urlencoded";
        request.ContentLength = nData.Length;

        using (var stream = request.GetRequestStream())
        {
            stream.Write(nData, 0, nData.Length);
        }

        var response = (HttpWebResponse)request.GetResponse();
            
        var responseString = new StreamReader(response.GetResponseStream() ?? throw new InvalidOperationException()).ReadToEnd();
        return responseString;
    }
    #endregion

}