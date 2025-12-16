using System;
using System.Web.Mvc;
using Corno.OnlineExam.Areas.Question_Bank.Models;
using Corno.OnlineExam.Controllers;
using Corno.Services.Corno.Interfaces;

namespace Corno.OnlineExam.Areas.Question_Bank.Controllers
{
    public class StaffController : BaseController
    {
        #region -- Constructors --
        public StaffController(ICornoService cornoService)
        {
            //_staffService = staffService;
        }
        #endregion

        #region -- Data Mambers --
        //private readonly IStaffService _staffService;
        #endregion

        #region -- Action Methods --
        protected ActionResult Index()
        {
            return View();
        }

        protected override Staff EditPost(Staff model)
        {
            var existing = _staffService.GetById(model.Id);
            if (null == existing)
                throw new Exception("Something went wrong Product controller.");

            model.CopyPropertiesTo(existing);

            return existing;
        }

        #endregion
    }
}