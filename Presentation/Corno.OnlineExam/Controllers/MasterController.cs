using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using Corno.Data.Common;

namespace Corno.OnlineExam.Controllers
{
    //[Authorize]
    [Attributes.Compress]
    public class MasterController<TEntity> : BaseController<TEntity>
        where TEntity : MasterModel, new()
    {
        #region -- Constructor --

        public MasterController(IMasterService<TEntity> genericMasterService,
            IWebProgressService progressService)
        : base(genericMasterService)
        {
            _genericMasterService = genericMasterService;
            _progressService = progressService;
        }

       
        #endregion

        #region -- Data Members --

        private readonly IMasterService<TEntity> _genericMasterService;
        private readonly IWebProgressService _progressService;

        #endregion

        #region -- Protected Methods -- 
        protected override ActionResult IndexGet(int? pageNo, string type)
        {
            return View(_genericMasterService.GetViewModelList());
        }

        #endregion

        #region -- Public Methods --
        [HttpPost]
        public override ActionResult ImportMaster(IEnumerable<HttpPostedFileBase> files)
        {
            ActionResult jsonResult = Json(new {error = false }, JsonRequestBehavior.AllowGet); 
            try
            {
                var httpPostedFileBases = files.ToList();
                if (httpPostedFileBases.FirstOrDefault() == null )
                    throw new Exception("No file selected for import");

                var fileBase = httpPostedFileBases.FirstOrDefault();
                // Save file
                var filePath = Server.MapPath("~/Upload/" + fileBase?.FileName);
                fileBase?.SaveAs(filePath);

                // Import file
                _progressService.SetWebProgress();
                _genericMasterService.ImportRecords(filePath, _progressService);
            }
            catch (Exception exception)
            {
                jsonResult = Json(new
                {
                    error = true,
                    message = exception.Message
                }, JsonRequestBehavior.AllowGet);
            }
            return jsonResult;
        }
        #endregion
    }
}