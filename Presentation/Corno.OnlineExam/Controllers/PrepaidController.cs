using AutoMapper;
using Corno.DAL.Classes;
using Corno.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace Corno.Controllers
{
    public class PrepaidController : Controller
    {
         IPrepaidAccountService _prepaidaccountservice;
         IDTHAccountService _dthaccountservice;
         IDataCardAccountService _datacardaccountservice;
         IOperatorService _operatorService;
         ICircleService _circleService;

         public PrepaidController(IPrepaidAccountService prepaidaccountservice, IDTHAccountService dthaccountservice, IDataCardAccountService datacardaccountservice,IOperatorService operatorService,ICircleService circleService)
         {
             _prepaidaccountservice = prepaidaccountservice;
             _dthaccountservice = dthaccountservice;
             _datacardaccountservice = datacardaccountservice;
             _operatorService = operatorService;  
             _circleService=circleService;
         }
        
        //
        // GET: /Prepaid/
        public ActionResult Index() 
        {
             PrepaidAccountViewModel prepaidaccountviewmodel = new PrepaidAccountViewModel();
            DataCardAccountViewModel dataCardAccountViewModel = new DataCardAccountViewModel();
            DTHAccount dthaccount=new DTHAccount();
           

            Tuple<PrepaidAccountViewModel, DTHAccount, DataCardAccountViewModel> abc = new Tuple<PrepaidAccountViewModel, DTHAccount, DataCardAccountViewModel>(prepaidaccountviewmodel, dthaccount, dataCardAccountViewModel);
           
            prepaidaccountviewmodel.Operators = _operatorService.OperatorRepository.Get().ToList();
            dataCardAccountViewModel.Operators = _operatorService.OperatorRepository.Get().ToList();
            return View(abc);
        }
        [HttpPost]
        public ActionResult Mobile(PrepaidAccountViewModel prepaidaccount, string retailerChanged)
        {
            //if ("true" == retailerChanged)
            //{


            //    //PrepaidAccountViewModel prepaidaccountviewmodel = new PrepaidAccountViewModel();
            //    //Tuple<PrepaidAccountViewModel> abc = new Tuple<PrepaidAccountViewModel>(prepaidaccountviewmodel);
                
            //    abc.Item1.Operators = _operatorService.OperatorRepository.Get().ToList();
            //    //ViewBag.OperatorName = new SelectList(_operatorService.OperatorRepository.Get(), "ID", "Name");

            //    abc.Item1.Circles = (_circleService.CircleRepository.Get(w => w.OperatorID == abc.Item1.OperatorID)).ToList();

            //    //Operator opt = new Operator();
            //    //ViewBag.CircleName = (_circleService.CircleRepository.Get(w => w.OperatorID == opt.ID)).ToList();

            //    return View(abc);
            //}
            Mapper.CreateMap<PrepaidAccountViewModel, PrepaidAccount>();
            var itemModel = Mapper.Map<PrepaidAccountViewModel, PrepaidAccount>(prepaidaccount);
           
            if (ModelState.IsValid)
            {
                _prepaidaccountservice.PrepaidAccountRepository.Add(itemModel);
                _prepaidaccountservice.Save();
                return RedirectToAction("Index");
            }

            return View(prepaidaccount);
        }
         [HttpPost]
        public ActionResult DTH(DTHAccount dthAccount)
        {
            if (ModelState.IsValid)
            {
                _dthaccountservice.DthAccountRepository.Add(dthAccount);
                _dthaccountservice.Save();
                return RedirectToAction("Index");
            }

            return View(dthAccount);
        }

         [HttpPost]
         public ActionResult DataCard(DataCardAccount datacardAccount)
         {
             if (ModelState.IsValid)
             {
                 _datacardaccountservice.DataCardAccountRepository.Add(datacardAccount);
                 _datacardaccountservice.Save();
                 return RedirectToAction("Index");
             }

             return View(datacardAccount);
         }

         public JsonResult GetOperatorID(int? id)
         {
             if (id == null)
             {
                 return Json(null, JsonRequestBehavior.AllowGet);
             }

             //var Circles = (from m in _circleService.CircleRepository.Get(w => w.OperatorID == id) select new SelectListItem { Text=m.CircleName, Value = m.CircleName }).ToList() ;
             var sl = new SelectList(_circleService.CircleRepository.Get(w => w.OperatorID == id).ToList(),"ID","CircleName");
             if (sl == null)
             {
                 return Json(null, JsonRequestBehavior.AllowGet);
             }
             return Json(sl, JsonRequestBehavior.AllowGet);
         }

	}
}