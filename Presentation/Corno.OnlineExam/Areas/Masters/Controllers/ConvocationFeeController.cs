using Corno.Data.Masters;
using System.Net;
using System.Web.Mvc;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;

namespace Corno.OnlineExam.Areas.Masters.Controllers;

public class ConvocationFeeController : Controller
{
    #region -- Constructors --
    public ConvocationFeeController(ICornoService registrationService, IMasterService masterService, ICoreService examService)
    {
        _registrationService = registrationService;
        _masterService = masterService;
        _examService = examService;
    }
    #endregion

    #region -- Data Members --
    private readonly ICornoService _registrationService;
    private readonly IMasterService _masterService;
    private readonly ICoreService _examService;
    #endregion

    ////
    //// GET: /ConvocationFee/
    //[Authorize]
    //public ActionResult Index(int? page)
    //{
    //    var viewModel = _registrationService.ConvocationFeeRepository.Get().ToList().ToMappedList<ConvocationFee, ConvocationFeeViewModel>();

    //    return View(viewModel);
    //}

    ////
    //// GET: /ConvocationFee/Details/5

    //public ActionResult Details(int? id)
    //{
    //    if (id == null)
    //    {
    //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //    }
    //    var convocationFee = _registrationService.ConvocationFeeRepository.GetById(id);
    //    if (convocationFee == null)
    //    {
    //        return HttpNotFound();
    //    }
    //    return View(convocationFee);
    //}

    //
    // GET: /ConvocationFee/Create
    public ActionResult Create()
    {

        //ConvocationFeeViewModel viewModel = new ConvocationFeeViewModel();
        //ConvocationFee model = AutoMapperConfig.CornoMapper.Map<ConvocationFee>(viewModel);
        return View(new ConvocationFeeViewModel());
    }



    [HttpPost]
    [ValidateAntiForgeryToken]
    public ActionResult Create(ConvocationFeeViewModel viewModel)
    {
        if (!ModelState.IsValid) 
            return View(viewModel);

        // Under Graduate Diploma Fee
        InsertFee(1, viewModel.UndergraduateDiplomaFee);
        InsertFee(2, viewModel.GraduateDegreeFee);
        InsertFee(3, viewModel.PostGraduateDiplomaFee);
        InsertFee(4, viewModel.PostGraduateDegreeFee);
        InsertFee(5, viewModel.MPhilFee);
        InsertFee(6, viewModel.PhDFee);
        InsertFee(7, viewModel.ForStudentsInIndiaFee);
        InsertFee(7, viewModel.ForStudentsInAbroadFee);

        return RedirectToAction("Create");

    }

    //
    // GET: /ConvocationFee/Edit/5

    public ActionResult Edit(int? id)
    {
        if (id == null)
        {
            return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
        }
        ConvocationFee model = _registrationService.ConvocationFeeRepository.GetById(id);
        ConvocationFeeViewModel viewModel = AutoMapperConfig.CornoMapper.Map<ConvocationFeeViewModel>(model);
        if (model == null)
        {
            return HttpNotFound();
        }

        return View(viewModel);
    }


    ////
    //// POST: /ConvocationFee/Edit/5

    //[HttpPost]
    //[ValidateAntiForgeryToken]
    //public ActionResult Edit(ConvocationFeeViewModel viewModel)
    //{
    //    if (ModelState.IsValid)
    //    {
    //        // ConvocationFee model = <ConvocationFee>(viewModel);
    //        //viewModel.ModifiedBy = User.Identity.GetUserId();
    //        // viewModel.ModifiedDate = DateTime.Now;
    //        // _masterService.ConvocationFeeRepository.Update(model);
    //        _masterService.Save();
    //        return RedirectToAction("Index");
    //    }

    //    return View(viewModel);
    //}

    ////
    //// GET: /ConvocationFee/Delete/5

    //public ActionResult Delete(int? id)
    //{
    //    if (id == null)
    //    {
    //        return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
    //    }
    //    ConvocationFee model = _registrationService.ConvocationFeeRepository.GetById(id);
    //    ConvocationFeeViewModel viewModel = AutoMapperConfig.CornoMapper.Map<ConvocationFee, ConvocationFeeViewModel>(model);

    //    return View(viewModel);
    //}

    ////
    //// POST: /ConvocationFee/Delete/5

    //[HttpPost, ActionName("Delete")]
    //[ValidateAntiForgeryToken]
    //public ActionResult DeleteConfirmed(int id)
    //{
    //    ConvocationFee ConvocationFee = _registrationService.ConvocationFeeRepository.GetById(id);
    //    ConvocationFee.DeletedBy = User.Identity.GetUserId();
    //    ConvocationFee.DeletedDate = DateTime.Now;

    //    ConvocationFee.Status = ValueConstants.Deleted;
    //    _registrationService.ConvocationFeeRepository.Update(ConvocationFee);
    //    //_masterService.DistrictRepository.Delete(District);
    //    _examService.Save();
    //    return RedirectToAction("Index");
    //}

    #region -- Methods --
    private void InsertFee(int courseTypeId, double fee)
    {
        var convocationFee = new ConvocationFee
        {
            CourseTypeId = courseTypeId,
            Fee = fee
        };

        _registrationService.ConvocationFeeRepository.Add(convocationFee);
        _examService.Save();
    }

    //private void CalculateFee(int courseTypeId)
    //{
    //    var record = _registrationService.ConvocationFeeRepository.Get().ToList().Where(t => t.CourseTypeId == courseTypeId);


    //}

    #endregion
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            _masterService.Dispose(true);
        }
        base.Dispose(disposing);
    }
}