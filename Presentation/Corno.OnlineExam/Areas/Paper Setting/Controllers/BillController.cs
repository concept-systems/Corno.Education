using System;
using Corno.Data.Corno.Question_Bank;
using Corno.Globals;
using Corno.Globals.Constants;
using Corno.OnlineExam.Areas.Paper_Setting.Reports.Bill;
using Corno.OnlineExam.Attributes;
using Corno.OnlineExam.Controllers;
using Corno.Reports.Enrollment;
using Corno.Services.Corno.Question_Bank.Interfaces;
using System.Linq;
using System.Web.Mvc;
using Corno.Data.Corno.Paper_Setting.Models;
using Corno.Data.Helpers;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Paper_Setting.Interfaces;

namespace Corno.OnlineExam.Areas.Paper_Setting.Controllers;

[Authorize]
public class BillController : CornoController
{
    #region -- Constructors --
    public BillController(IAppointmentService appointmentService, IStaffService staffService)
    {
        _appointmentService = appointmentService;
        _staffService = staffService;

        var path = "~/Areas/paper setting/views/Bill/";
        _indexPath = $"{path}Index.cshtml";
        _createPath = $"{path}Create.cshtml";
    }
    #endregion

    #region -- Data Members --

    private readonly IAppointmentService _appointmentService;
    private readonly IStaffService _staffService;

    private readonly string _indexPath;
    private readonly string _createPath;
    #endregion

    #region -- Actions --
    [Authorize]
    public ActionResult Index()
    {
        return View(_indexPath);
    }

    [Authorize]
    public ActionResult Create()
    {
        var sessionData = Session[User.Identity.Name] as SessionData;
        var appointment = new Appointment
        {
            InstanceId = sessionData?.InstanceId ?? 0,
            CollegeId = sessionData?.CollegeId,
        };

        //appointment.AppointmentBillDetail.BillDate = DateTime.Now;

        return View(_createPath, appointment);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    [MultipleButton(Name = "action", Argument = "Save")]
    public ActionResult Create(Appointment appointment)
    {
        if (!ModelState.IsValid)
            return View(_createPath, appointment);

        try
        {
            var staffId = appointment.StaffId.ToInt();
            if (staffId <= 0)
                throw new Exception("Please, select the staff");
            var existing = _appointmentService.GetExisting(appointment);
            if(null == existing)
                throw new Exception("Invalid appointment information.");
            var billDetail = existing.AppointmentBillDetails.FirstOrDefault(d =>
                d.StaffId == staffId);
            if (null == billDetail)
            {
                billDetail = new AppointmentBillDetail();
                billDetail.Copy(appointment.AppointmentBillDetail);
                billDetail.BillDate = DateTime.Now.Date;
                existing.AppointmentBillDetails.Add(billDetail);
            }
            else
                billDetail.Copy(appointment.AppointmentBillDetail);

            billDetail.DateOfJourney ??= billDetail.BillDate;
            billDetail.SerialNo = existing.InstanceId;
            billDetail.DeletedBy = existing.Id.ToString();
            billDetail.Code ??= appointment.SubjectId.ToString();

            billDetail.StaffId = appointment.StaffId;
            //billDetail.BillDate = DateTime.Now;

            _appointmentService.ValidateHeader(existing);
            _appointmentService.UpdateAndSave(existing);

            TempData["Success"] = "Saved successfully.";

            appointment.StaffId = null;
            appointment.AppointmentBillDetail = new AppointmentBillDetail();

            ModelState.Clear();
            //return View(_createPath, appointment);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(_createPath, appointment);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "ShowBill")]
    public ActionResult ShowBill(Appointment appointment)
    {
        if (!ModelState.IsValid || null == appointment)
            return View(_createPath, appointment);

        try
        {
            if ((appointment.StaffId ?? 0) <= 0)
                throw new Exception("Invalid Staff");

            var staffId = appointment.StaffId;
            var existing = _appointmentService.GetExisting(appointment);

            var appointmentDetail = existing.AppointmentDetails.FirstOrDefault(d =>
                d.StaffId == staffId);
            if (null == appointmentDetail)
                throw new Exception("Invalid staff");
            var billDetail = existing.AppointmentBillDetails.FirstOrDefault(d =>
                d.StaffId == staffId);
            if (null == billDetail)
            {
                var scheduleService = Bootstrapper.Get<IScheduleService>();

                var schedule = scheduleService.GetExisting(existing);
                var scheduleDetail = schedule?.ScheduleDetails.FirstOrDefault(d =>
                    d.SubjectId == existing.SubjectId);
                billDetail = new AppointmentBillDetail
                {
                    AppointmentId = existing.Id,
                    StaffId = staffId,
                    SetsDrawn = scheduleDetail?.SetsToBeDrawn,
                    BillDate = scheduleDetail?.FromDate,
                    DateOfJourney = scheduleDetail?.FromDate
                };
            }

            var remunerationService = Bootstrapper.Get<IRemunerationService>();
            var remuneration = remunerationService.GetExisting(existing);
            var remunerationDetail = remuneration?.RemunerationDetails.FirstOrDefault(d =>
                d.CoursePartId == existing.CoursePartId);
            var staffIds = existing.AppointmentDetails.Select(d => d.StaffId).Distinct().ToList();
            var staffs = _staffService.Get(p => staffIds.Contains(p.Id) &&
                                                p.Status != StatusConstants.Closed, p => p).ToList();
            var appointmentDetails = (from detail in existing.AppointmentDetails.Where(d =>
                    d.IsPaperSetter)
                join staff in staffs on detail.StaffId equals staff.Id
                select detail).ToList();
            if (appointmentDetails.Count > 0)
            {
                billDetail.CourseFee = remunerationDetail?.Fee / appointmentDetails.Count;
                billDetail.AllottedFee = remunerationDetail?.Fee;
                billDetail.RemunerationOthers = remunerationDetail?.Others;
                billDetail.SchemeOfMarking = remunerationDetail?.SchemeOfMarking;
                billDetail.ModelAnswers = remunerationDetail?.ModelAnswers;
            }

            billDetail.ChairmanAllowance = appointmentDetail.IsChairman ? 200 : 0;
            billDetail.Travel = 500;

            existing.StaffId = appointment.StaffId;
            billDetail.StaffId = appointment.StaffId;
            billDetail.Code = existing.SubjectId?.ToString();
            existing.AppointmentBillDetail = billDetail;

            //ModelState.Remove(ModelConstants.Id);
            ModelState.Clear();

            return View(_createPath, existing);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_createPath, appointment);
    }

    [HttpPost]
    [Authorize]
    [ValidateAntiForgeryToken]
    [MultipleButton(Name = "action", Argument = "ShowReport")]
    public ActionResult ShowReport(Appointment model, string type)
    {
        TempData["Success"] = null;

        if (!ModelState.IsValid)
            return View(_createPath, model);

        try
        {
            switch (type)
            {
                case ReportName.TaDa:
                    _appointmentService.ValidateReportHeader(model);
                    Session[ModelConstants.Report] = new TaDaRpt(model);
                    break;
                case ReportName.Remuneration:
                    _appointmentService.ValidateReportHeader(model);
                    Session[ModelConstants.Report] = new RemunerationRpt(model);
                    break;
                case ReportName.Certificate:
                    _appointmentService.ValidateReportHeader(model);
                    Session[ModelConstants.Report] = new CertificateRpt(model);
                    break;
                case ReportName.NeftForm:
                    _appointmentService.ValidateReportHeader(model);
                    Session[ModelConstants.Report] = new NeftRpt(model);
                    break;
                case ReportName.NonAttendanceLetter:
                    _appointmentService.ValidateReportHeader(model);
                    Session[ModelConstants.Report] = new NonAttendanceLetterRpt(model);
                    break;
                case ReportName.RemunerationPaid2:
                    Session[ModelConstants.Report] = new RemunerationPaid2Rpt(model);
                    break;
                case ReportName.RemunerationPaidFacultyWise:
                    Session[ModelConstants.Report] = new RemunerationPaidFacultyWiseRpt(model.InstanceId ?? 0);
                    break;
                case ReportName.UnattendedExaminers:
                    Session[ModelConstants.Report] = new UnattendedExaminersRpt(model);
                    break;
            }

            return RedirectToAction("Details", "Report", new { area = "Reports", reportName = nameof(EnrollmentRpt), description = string.Empty });
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(_createPath, model);
    }
    #endregion
}
