using System;
using System.Web.Http;
using Corno.Data.Admin;
using Corno.Logger;
using Corno.OnlineExam.Areas.Services.Interfaces;

namespace Corno.OnlineExam.Areas.Api.Controllers;

public class OtpController : ApiController
{
    #region -- Constructors --

    public OtpController(IOtpService otpService)
    {
        _otpService = otpService;
    }
    #endregion

    #region -- Data Members --
    private readonly IOtpService _otpService;
    #endregion

    // POST api/<controller>
    [HttpPost]
    public IHttpActionResult SendOtp([FromBody] LoginViewModel loginViewModel)
    {
        IHttpActionResult response;
        try
        {
            _otpService.SendLoginOtp(loginViewModel);

            response = Ok(true);
        }
        catch (Exception exception)
        {
            LogHandler.LogInfo(exception);
            response = InternalServerError(exception);
        }

        return response;
    }

    [HttpPost]
    public IHttpActionResult ValidateOtp([FromBody] LoginViewModel loginViewModel)
    {
        IHttpActionResult response;
        try
        {
            // Send Otp
            _otpService.ValidateOtp(loginViewModel);

            response = Ok(true);
        }
        catch (Exception exception)
        {
            LogHandler.LogInfo(exception);
            response = InternalServerError(exception);
        }

        return response;
    }

    /*// GET api/<controller>
    public IEnumerable<string> Get()
    {
        return new[] { "Otp Test 1", "Otp Test 2" };
    }

    // GET api/<controller>/5
    public string Get(int id)
    {
        return "Otp Umesh Kale";
    }

    // POST api/<controller>
    public void Post([FromBody] string value)
    {
    }

    // PUT api/<controller>/5
    public void Put(int id, [FromBody] string value)
    {
    }

    // DELETE api/<controller>/5
    public void Delete(int id)
    {
    }*/
}