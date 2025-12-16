using System;
using System.Web.Http;
using Corno.Data.ViewModels;
using Corno.OnlineExam.Helpers;
using Corno.Services.Core.Interfaces;

namespace Corno.OnlineExam.Areas.Api.Controllers;

public class CoreController : ApiController
{
    #region -- Constructors --

    public CoreController(ICoreService coreService)
    {
        _coreService = coreService;
    }
    #endregion

    #region -- Data Members --
    private readonly ICoreService _coreService;
    #endregion

    // POST api/<controller>
    /*[HttpPost]
    public IHttpActionResult GetExamStudents(UniversityBaseModel universityBaseModel)
    {
        IHttpActionResult response;
        try
        {
            var examLinkDetail = _yrChangeService.GetExamLinks(universityBaseModel);

            response = Ok(examLinkDetail);
        }
        catch (Exception exception)
        {
            response = InternalServerError(exception);
        }

        return response;
    }*/

    // POST api/<controller>
    [HttpPost]
    public IHttpActionResult GetExamForm(ExamViewModel viewModel)
    {
        IHttpActionResult response;
        try
        {
            ExamFormHelper.GetPrnInfoForPaymentGateway(viewModel);

            response = Ok(viewModel);
        }
        catch (Exception exception)
        {
            response = InternalServerError(exception);
        }

        return response;
    }

    /*// GET api/<controller>
    public IEnumerable<string> Get()
    {
        return new[] { "Test 1", "Test 2" };
    }

    // GET api/<controller>/5
    public string Get(int id)
    {
        return "Umesh Kale";
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