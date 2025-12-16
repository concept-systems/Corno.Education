using System;
using System.Collections.Generic;
using System.Data.Entity;
using System.Linq;
using System.Linq.Expressions;
using System.Web;
using Corno.Data.Common;
using Corno.Data.Corno.Masters;
using Corno.Data.Corno.Paper_Setting.Dtos;
using Corno.Data.Corno.Paper_Setting.Models;
using Corno.Data.Helpers;
using Corno.Data.ViewModels;
using Corno.Data.ViewModels.Appointment;
using Corno.Globals.Constants;
using Corno.Logger;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Paper_Setting.Interfaces;
using LinqKit;
using Mapster;

namespace Corno.Services.Corno.Paper_Setting;

public class AppointmentService : MainService<Appointment>, IAppointmentService
{
    #region -- Constructors --
    public AppointmentService(IInstanceService instanceService, IFacultyService facultyService,
        ICollegeService collegeService, ICourseService courseService,
        ICoursePartService coursePartService, IBranchService branchService,
        ICategoryService categoryService, ISubjectService subjectService,
        IMiscMasterService miscMasterService, IRemunerationService remunerationService,
        IStaffService staffService)
    {
        _instanceService = instanceService;
        _facultyService = facultyService;
        _collegeService = collegeService;
        _courseService = courseService;
        _coursePartService = coursePartService;
        _branchService = branchService;
        _categoryService = categoryService;
        _subjectService = subjectService;
        _miscMasterService = miscMasterService;
        _remunerationService = remunerationService;
        _staffService = staffService;

        SetIncludes($"{nameof(Appointment.AppointmentDetails)},{nameof(Appointment.AppointmentBillDetails)}");
    }
    #endregion

    #region -- Data Members --

    private readonly IInstanceService _instanceService;
    private readonly IFacultyService _facultyService;
    private readonly ICollegeService _collegeService;
    private readonly ICourseService _courseService;
    private readonly ICoursePartService _coursePartService;
    private readonly IBranchService _branchService;
    private readonly ICategoryService _categoryService;
    private readonly ISubjectService _subjectService;
    private readonly IMiscMasterService _miscMasterService;
    private readonly IRemunerationService _remunerationService;
    private readonly IStaffService _staffService;

    private string _smsUrl;

    private static void FillReportHeader(ReportModel reportModel, Appointment appointment, MasterModel instance,
        MasterModel faculty, College college, Course course, MasterViewModel coursePart,
        MasterViewModel branch, Subject subject, MasterModel category)
    {
        instance.Adapt(reportModel.Instance);
        faculty.Adapt(reportModel.Faculty);
        college.Adapt(reportModel.College);
        course.Adapt(reportModel.Course);
        coursePart.Adapt(reportModel.CoursePart);
        branch.Adapt(reportModel.Branch);
        category.Adapt(reportModel.Category);
        subject.Adapt(reportModel.Subject);
        reportModel.PaperCategoryApplicable = subject?.PaperCategoryApplicable ?? false;

        reportModel.Subject.Name =
            $"({subject?.Code}) {subject?.Name} {((appointment.BranchId ?? 0) > 0 ? branch?.Name : "")}";

        reportModel.StandardSets = subject?.StandardSets;
        reportModel.SubjectType = subject?.SubjectType ?? string.Empty;
        reportModel.LetterAddress = course?.LetterAddress ?? string.Empty;
        reportModel.PrintSequenceNo = course?.PrintSequenceNo;
    }

    private void FillScheduleInfo(ReportModel reportModel, ScheduleDetail scheduleDetail, Instance instance)
    {
        reportModel.MeetingDate = scheduleDetail?.FromDate;
        if (null != reportModel.MeetingDate)
            reportModel.MeetingDate = reportModel.MeetingDate.Value.AddHours(scheduleDetail?.Time?.Hours ?? 0)
                .AddMinutes(scheduleDetail?.Time?.Minutes ?? 0);

        reportModel.ToDate = scheduleDetail?.ToDate;
        reportModel.Time = scheduleDetail?.Time ?? new TimeSpan(0, 0, 0);
        reportModel.SetsToBeDrawn = scheduleDetail?.SetsToBeDrawn;
        reportModel.OutWardNo =
            $"Ref No. : BVDU/Exam/{instance?.StartDate?.Year} - {instance?.EndDate?.Year}/{scheduleDetail?.OutwardNo}";
        reportModel.OutWardNoInt = scheduleDetail?.OutwardNo ?? 0;
    }

    private void FillStaffInfo(ReportModel reportModel, AppointmentDetail detail, College college, Staff staff,
        string designationName, string departmentName)
    {
        reportModel.StaffId = detail.StaffId;
        reportModel.StaffName = $"{staff?.Salutation} {staff?.Name} {(detail.IsChairman ? " (Chairman)" : "")}";
        reportModel.StaffAddress =
            $"{designationName}, " +
            $"{departmentName}, \n" +
            $"{(string.IsNullOrEmpty(staff?.Address1) && staff?.UseCollegeAddress.ToBoolean() == true ? college?.Address1 : staff?.Address1 ?? string.Empty)}, \n" +
            $"Mob. : {staff?.Mobile} ";
        //reportModel.StaffAddress = $"{staff?.Address1}";
        reportModel.DesignationName = designationName ?? string.Empty;
        reportModel.Phone = staff?.Phone ?? string.Empty;
        reportModel.MobileNo = staff?.Mobile ?? string.Empty;
        reportModel.EmailId = staff?.Email ?? string.Empty;
        reportModel.Address2 = staff?.Address2 ?? string.Empty;
        reportModel.IsChairman = detail.IsChairman;
        reportModel.IsInternal = detail.IsInternal;
        reportModel.IsModerator = detail.IsModerator;
        reportModel.IsManuscript = detail.IsManuscript;
        reportModel.NoOfAttempts = detail.NoOfAttempts;
        reportModel.OriginalId = detail.OriginalId;
        reportModel.SmsCount = detail.SmsCount.ToInt();
        reportModel.EmailCount = detail.EmailCount.ToInt();
        reportModel.EmailDate = detail.EmailDate;

        reportModel.BankModel.BankName = staff?.BankName;
        reportModel.BankModel.BankBranch = staff?.BranchName;
        reportModel.BankModel.BankAccountNo = staff?.BankAccountNo;
        reportModel.BankModel.IfscCode = staff?.IfscCode;

        reportModel.BosId = 0;                              // TODO : Fill it
        reportModel.BosName = string.Empty; // TODO : Fill it,
    }

    private void FillReportBillModel(ReportModel reportModel, AppointmentBillDetail billDetail)
    {
        if (null == billDetail)
            return;

        reportModel.BillModel.BillNo = billDetail.Id;
        reportModel.BillModel.BillDate = billDetail.BillDate;
        reportModel.BillModel.BasicPay = billDetail.BasicPay.ToDouble();
        reportModel.BillModel.Ta = billDetail.Ta.ToDouble();
        reportModel.BillModel.Da = billDetail.Da.ToDouble();
        reportModel.BillModel.Travel = billDetail.Travel.ToDouble();
        reportModel.BillModel.AllottedFee = billDetail.AllottedFee.ToDouble();
        reportModel.BillModel.CourseFee = billDetail.CourseFee.ToDouble();
        reportModel.BillModel.ChairmanAllowance = billDetail.ChairmanAllowance.ToDouble();
        reportModel.BillModel.TotalFee = billDetail.TotalFee.ToDouble();
        reportModel.BillModel.FromPlace = billDetail.FromPlace;
        reportModel.BillModel.ToPlace = billDetail.ToPlace;
        reportModel.BillModel.TotalDistance = billDetail.TotalDistance;
        reportModel.BillModel.TravelingMode = billDetail.TravelingMode;
        reportModel.BillModel.IsHindi = billDetail.IsHindi ?? false;
        reportModel.BillModel.IsMarathi = billDetail.IsMarathi ?? false;
        reportModel.BillModel.IsSanskrit = billDetail.IsSanskrit ?? false;
        reportModel.BillModel.SetsDrawn = billDetail.SetsDrawn.ToInt();
    }

    private void FillRemunerationInfo(ReportBillModel billModel, RemunerationDetail remunerationDetail,
        AppointmentBillDetail appointmentBillDetail)
    {
        if (null == billModel)
            return;

        billModel.Remuneration = remunerationDetail?.Fee.ToInt();
        billModel.RemunerationOthers = remunerationDetail?.Others.ToInt();
        if (null != appointmentBillDetail)
            billModel.ModelAnswers = appointmentBillDetail.IsModelAnswers ?? false ? remunerationDetail?.ModelAnswers.ToInt() : 0;

        var staffCount = billModel.AllottedFee / billModel.CourseFee;
        var versionFee = billModel.RemunerationOthers.ToDouble() * billModel.SetsDrawn / staffCount;
        versionFee = Math.Round(versionFee ?? 0, 0);
        billModel.HindiFee = billModel.IsHindi ? versionFee : 0;
        billModel.MarathiFee = billModel.IsMarathi ? versionFee : 0;
        billModel.SanskritFee = billModel.IsSanskrit ? versionFee : 0;
        if (null != appointmentBillDetail)
            billModel.ModelAnswersFee = appointmentBillDetail.IsModelAnswers ?? false ? billModel.ModelAnswers.ToDouble() * billModel.SetsDrawn : 0;
    }

    private IEnumerable<ReportModel> GetScheduleData(List<Schedule> schedules, List<Appointment> appointments)
    {
        var instanceId = HttpContext.Current.Session[ModelConstants.InstanceId]?.ToInt();

        var scheduleDetails = schedules.SelectMany(p => p.ScheduleDetails.Where(d =>
            null != d.FromDate), (_, d) => d).ToList();

        var facultyIds = schedules.Select(p => p.FacultyId ?? 0).Distinct().ToList();
        var collegeIds = schedules.Select(p => p.CollegeId ?? 0).Distinct().ToList();
        var courseIds = schedules.Select(p => p.CourseId ?? 0).Distinct().ToList();
        var coursePartIds = schedules.Select(p => p.CoursePartId ?? 0).Distinct().ToList();
        var branchIds = schedules.Select(p => p.BranchId ?? 0).Distinct().ToList();
        var subjectIds = schedules.SelectMany(p => p.ScheduleDetails, (_, d) => d.SubjectId ?? 0).Distinct().ToList();
        var categoryId = schedules.Select(p => p.CategoryId ?? 0).FirstOrDefault();

        var instance = _instanceService.GetById(instanceId);
        var faculties = _facultyService.Get(p => facultyIds.Contains(p.Id ?? 0), p => p).ToList();
        var colleges = _collegeService.Get(p => collegeIds.Contains(p.Id ?? 0), p => p).ToList();
        var courses = _courseService.Get(p => courseIds.Contains(p.Id ?? 0), p => p).ToList();
        var courseParts = _coursePartService.GetViewModelList(p => coursePartIds.Contains(p.Id ?? 0)).ToList();
        var branches = _branchService.GetViewModelList(p => branchIds.Contains(p.Id ?? 0)).ToList();
        var categoryIds = schedules.Select(d => d.CategoryId).Distinct().ToList();
        var categories = _categoryService.Get(p => categoryIds.Contains(p.Id), p => p).ToList();

        var subjects = _subjectService.Get(p => subjectIds.Contains(p.Id ?? 0), p => p).ToList();

        var dataSource = scheduleDetails.Select(d =>
        {
            var schedule = schedules.FirstOrDefault(x => x.Id == d.ScheduleId);
            if (null == schedule) return null;

            var faculty = faculties.FirstOrDefault(x => x.Id == schedule.FacultyId);
            var college = colleges.FirstOrDefault(x => x.Id == schedule.CollegeId);
            var course = courses.FirstOrDefault(x => x.Id == schedule.CourseId);
            var coursePart = courseParts.FirstOrDefault(x => x.Id == schedule.CoursePartId);
            var branch = branches.FirstOrDefault(x => x.Id == schedule.BranchId);
            var subject = subjects.FirstOrDefault(x => x.Id == d.SubjectId);
            var category = categories.FirstOrDefault(p => p.Id == schedule.CategoryId);

            var appointment = appointments.FirstOrDefault(x => x.InstanceId == schedule.InstanceId && x.FacultyId == schedule.FacultyId && x.CollegeId == schedule.CollegeId &&
                                                               x.CourseId == schedule.CourseId && x.CoursePartId == schedule.CoursePartId &&
                                                               x.CategoryId == categoryId &&
                                                               x.SubjectId == d.SubjectId) ?? new Appointment();

            var reportModel = new ReportModel();
            // Fill Header
            FillReportHeader(reportModel, appointment, instance, faculty, college, course,
                coursePart, branch, subject, category);
            // Fill Schedule details
            FillScheduleInfo(reportModel, d, instance);

            // Fill Details
            FillReportBillModel(reportModel, appointment.AppointmentBillDetails.FirstOrDefault());

            reportModel.SmsCount = appointment.AppointmentDetails.Max(x => x.SmsCount);
            reportModel.EmailCount = appointment.AppointmentDetails.Max(x => x.EmailCount);
            reportModel.EmailDate = appointment.AppointmentDetails.Max(x => x.EmailDate);

            reportModel.CollegeEmailId = college?.Email ?? string.Empty;

            return reportModel;
        }).ToList();

        return dataSource.Where(p => null != p);
    }

    private Expression<Func<Appointment, bool>> GetAppointmentPredicate(Appointment appointment, List<int> subjectIds = default)
    {
        Expression<Func<Appointment, bool>> predicate = p =>
            p.InstanceId == appointment.InstanceId;// && p.AppointmentDetails.Any(d => d.IsPaperSetter);
        if ((appointment.FacultyId ?? 0) > 0)
            predicate = predicate.And(s => s.FacultyId == appointment.FacultyId);
        if ((appointment.CollegeId ?? 0) > 0)
            predicate = predicate.And(s => s.CollegeId == appointment.CollegeId);
        if ((appointment.CourseId ?? 0) > 0)
            predicate = predicate.And(s => s.CourseId == appointment.CourseId);
        if ((appointment.CoursePartId ?? 0) > 0)
            predicate = predicate.And(s => s.CoursePartId == appointment.CoursePartId);
        if ((appointment.BranchId ?? 0) > 0)
            predicate = predicate.And(s => s.BranchId == appointment.BranchId);

        if (subjectIds is { Count: > 0 })
            predicate = predicate.And(s => subjectIds.Contains(s.SubjectId ?? 0));
        else if ((appointment.SubjectId ?? 0) > 0)
            predicate = predicate.And(s => s.SubjectId == appointment.SubjectId);

        if ((appointment.CategoryId ?? 0) > 0)
            predicate = predicate.And(s => s.CategoryId == appointment.CategoryId);

        return predicate;
    }

    private List<Appointment> GetAppointments(Appointment appointment, List<int> subjectIds = default)
    {
        var predicate = GetAppointmentPredicate(appointment, subjectIds);
        var appointments = Get(predicate, p => p).ToList();

        /*if (appointments.Count <= 0)
            throw new Exception("No appointments to send mail");*/

        return appointments;
    }

    public List<Schedule> GetSchedules(Appointment appointment)
    {
        var scheduleService = Bootstrapper.Bootstrapper.Get<IScheduleService>();
        Expression<Func<Schedule, bool>> schedulePredicate = p => p.InstanceId == appointment.InstanceId;
        if ((appointment.FacultyId ?? 0) > 0)
            schedulePredicate = schedulePredicate.And(s => s.FacultyId == appointment.FacultyId);
        if ((appointment.CollegeId ?? 0) > 0)
            schedulePredicate = schedulePredicate.And(s => s.CollegeId == appointment.CollegeId);
        if ((appointment.CourseId ?? 0) > 0)
            schedulePredicate = schedulePredicate.And(s => s.CourseId == appointment.CourseId);
        if ((appointment.CoursePartId ?? 0) > 0)
            schedulePredicate = schedulePredicate.And(s => s.CoursePartId == appointment.CoursePartId);
        if ((appointment.BranchId ?? 0) > 0)
            schedulePredicate = schedulePredicate.And(s => s.BranchId == appointment.BranchId);
        if ((appointment.SubjectId ?? 0) > 0)
            schedulePredicate = schedulePredicate.And(s => s.ScheduleDetails.Any(d => d.SubjectId == appointment.SubjectId));
        if ((appointment.CategoryId ?? 0) > 0)
            schedulePredicate = schedulePredicate.And(s => s.CategoryId == appointment.CategoryId);

        var schedules = scheduleService.Get(schedulePredicate, p => p).ToList();

        return schedules;
    }

    #endregion

    #region -- Public Methods --
    public void ValidateReportHeader(Appointment appointment)
    {
        if (appointment.InstanceId.ToInt() <= 0)
            throw new Exception("Invalid instance / session.");
    }

    public void ValidateHeader(Appointment appointment)
    {
        if (appointment.InstanceId.ToInt() <= 0)
            throw new Exception("Invalid instance / session.");
        if (appointment.CollegeId.ToInt() <= 0)
            throw new Exception("Invalid College.");
        if (appointment.CourseId.ToInt() <= 0)
            throw new Exception("Invalid Course.");
        if (appointment.CoursePartId.ToInt() <= 0)
            throw new Exception("Invalid Course Part.");
        if (appointment.CategoryId.ToInt() <= 0)
            throw new Exception("Invalid Subject Category.");
        if (appointment.SubjectId.ToInt() <= 0)
            throw new Exception("Invalid Subject.");
    }

    public void ValidateEmailHeader(Appointment appointment)
    {
        if (appointment.InstanceId.ToInt() <= 0)
            throw new Exception("Invalid instance / session.");
        if (appointment.CollegeId.ToInt() <= 0)
            throw new Exception("Invalid College.");
        if (appointment.CourseId.ToInt() <= 0)
            throw new Exception("Invalid Course.");
        if (appointment.CoursePartId.ToInt() <= 0)
            throw new Exception("Invalid Course Part.");
        if (appointment.CategoryId.ToInt() <= 0)
            throw new Exception("Invalid Subject Category.");
    }

    public void ValidateFields(Appointment appointment)
    {
        // Validate Header  
        ValidateHeader(appointment);

        if (appointment.AppointmentDetails.Count <= 0)
            throw new Exception("No rows in appointment.");
        if (appointment.AppointmentDetails.All(d => null == d.StaffId))
            throw new Exception("All rows are blank.");
    }

    public Appointment GetExisting(Appointment appointment)
    {
        var existing = FirstOrDefault(p =>
            (p.InstanceId ?? 0) == (appointment.InstanceId ?? 0) &&
            (p.FacultyId ?? 0) == (appointment.FacultyId ?? 0) && (p.CollegeId ?? 0) == (appointment.CollegeId ?? 0) &&
            (p.CourseId ?? 0) == (appointment.CourseId ?? 0) &&
            (p.CoursePartId ?? 0) == (appointment.CoursePartId ?? 0) &&
            (p.BranchId ?? 0) == (appointment.BranchId ?? 0) &&
            (p.CategoryId ?? 0) == (appointment.CategoryId ?? 0) &&
            (p.SubjectId ?? 0) == (appointment.SubjectId ?? 0), a => a);

        return existing;
    }

    public IEnumerable<ReportModel> GetData(Appointment appointment, bool onlyInternal)
    {
        var instanceId = appointment.InstanceId ?? 0;
        var categoryId = appointment.CategoryId ?? 0;
        var facultyIds = new List<int> { appointment.FacultyId ?? 0 };
        var collegeIds = new List<int> { appointment.CollegeId ?? 0 };
        var courseIds = new List<int> { appointment.CourseId ?? 0 };
        var coursePartIds = new List<int> { appointment.CoursePartId ?? 0 };

        var appointments = Get(p =>
            p.InstanceId == instanceId && facultyIds.Contains(p.FacultyId ?? 0) &&
            collegeIds.Contains(p.CollegeId ?? 0) && courseIds.Contains(p.CourseId ?? 0) &&
            coursePartIds.Contains(p.CoursePartId ?? 0) /*&&
            p.AppointmentDetails.Any(d => d.IsPaperSetter)*/,
                p => p).ToList();
        if ((appointment.BranchId ?? 0) > 0)
            appointments = appointments.Where(p => p.BranchId == appointment.BranchId).ToList();
        if ((appointment.CategoryId ?? 0) > 0)
            appointments = appointments.Where(p => p.CategoryId == appointment.CategoryId).ToList();

        var scheduleService = Bootstrapper.Bootstrapper.Get<IScheduleService>();
        var schedules = scheduleService.Get(p =>
                p.InstanceId == instanceId && facultyIds.Contains(p.FacultyId ?? 0) &&
                collegeIds.Contains(p.CollegeId ?? 0) && courseIds.Contains(p.CourseId ?? 0) &&
                coursePartIds.Contains(p.CoursePartId ?? 0) &&
                p.CategoryId == categoryId, p => p).ToList();
        if ((appointment.BranchId ?? 0) > 0)
            schedules = schedules.Where(p => p.BranchId == appointment.BranchId).ToList();

        return GetData(appointments, schedules, onlyInternal);
    }

    public IEnumerable<ReportModel> GetReportDataFromAppointment(Appointment appointment, bool onlyInternal)
    {
        var appointments = GetAppointments(appointment);
        var schedules = GetSchedules(appointment);

        return GetData(appointments, schedules, onlyInternal);
    }

    public IEnumerable<ReportModel> GetReportDataFromSchedule(Appointment appointment, bool onlyInternal)
    {
        var schedules = GetSchedules(appointment);

        var subjectIds = schedules.SelectMany(p => p.ScheduleDetails, (_, d) => d.SubjectId ?? 0).Distinct().ToList();
        if (subjectIds.Count <= 0)
            throw new Exception("No subjects found in schedule.");
        var categoryIds = schedules.Select(p => p.CategoryId).Distinct().ToList();
        LogHandler.LogInfo($"Schedule Count : {schedules.Count}, Subject Count : {subjectIds.Count}, " +
                           $"Categories : {string.Join(",", categoryIds)}");

        var appointments = GetAppointments(appointment, subjectIds);

        LogHandler.LogInfo($"Appointment Count : {schedules.Count}, Subject Count : {subjectIds.Count}, " +
                           $"Categories : {string.Join(",", categoryIds)}");

        return GetScheduleData(schedules, appointments);
    }

    public IEnumerable<ReportModel> GetData(List<Appointment> appointments,
        List<Schedule> schedules, bool onlyInternal)
    {
        var instanceId = HttpContext.Current.Session[ModelConstants.InstanceId]?.ToInt();

        var appointmentDetails = appointments.SelectMany(p => p.AppointmentDetails.Where(d =>
            d.IsPaperSetter), (_, d) => d).ToList();
        if (onlyInternal)
            appointmentDetails = appointmentDetails.Where(d => d.IsInternal == true).ToList();
        //var appointmentBillDetails = appointments.SelectMany(p => p.AppointmentBillDetails).ToList();

        var facultyIds = appointments.Select(p => p.FacultyId ?? 0).Distinct().ToList();
        var collegeIds = appointments.Select(p => p.CollegeId ?? 0).Distinct().ToList();
        var courseIds = appointments.Select(p => p.CourseId ?? 0).Distinct().ToList();
        var coursePartIds = appointments.Select(p => p.CoursePartId ?? 0).Distinct().ToList();
        var branchIds = appointments.Select(p => p.BranchId ?? 0).Distinct().ToList();
        var subjectIds = appointments.Select(p => p.SubjectId ?? 0).Distinct().ToList();
        //var categoryId = appointments.Select(p => p.CategoryId ?? 0).FirstOrDefault();

        var staffIds = appointmentDetails.Select(d => d.StaffId).Distinct().ToList();
        var staffs = _staffService.Get(p => staffIds.Contains(p.Id) &&
            p.Status != StatusConstants.Closed, p => p).ToList();

        var instance = _instanceService.GetById(instanceId);
        var faculties = _facultyService.Get(p => facultyIds.Contains(p.Id ?? 0), p => p).ToList();
        var colleges = _collegeService.Get(p => collegeIds.Contains(p.Id ?? 0), p => p).ToList();
        var courses = _courseService.Get(p => courseIds.Contains(p.Id ?? 0), p => p).ToList();
        var courseParts = _coursePartService.GetViewModelList(p => coursePartIds.Contains(p.Id ?? 0)).ToList();
        var branches = _branchService.GetViewModelList(p => branchIds.Contains(p.Id ?? 0)).ToList();
        var categoryIds = appointments.Select(d => d.CategoryId).Distinct().ToList();
        var categories = _categoryService.Get(p => categoryIds.Contains(p.Id), p => p).ToList();

        var designationIds = staffs.Select(d => d.DesignationId).Distinct().ToList();
        var designations = _miscMasterService.GetViewModelList(p =>
            p.MiscType == MiscConstants.Designation && designationIds.Contains(p.Id));
        var departmentIds = staffs.Select(d => d.DepartmentId).Distinct().ToList();
        var departments = _miscMasterService.GetViewModelList(p =>
            p.MiscType == MiscConstants.Department && departmentIds.Contains(p.Id));

        var remunerations = _remunerationService.Get(p =>
                p.InstanceId == instanceId && facultyIds.Contains(p.FacultyId ?? 0) &&
                collegeIds.Contains(p.CollegeId ?? 0) && courseIds.Contains(p.CourseId ?? 0), p => p)
            .ToList();

        var subjects = _subjectService.Get(p => subjectIds.Contains(p.Id ?? 0), p => p).ToList();

        var dataSource = appointmentDetails.Select(d =>
        {
            var appointment = appointments.FirstOrDefault(x => x.Id == d.AppointmentId);
            if (null == appointment) return null;

            var faculty = faculties.FirstOrDefault(x => x.Id == appointment.FacultyId);
            var college = colleges.FirstOrDefault(x => x.Id == appointment.CollegeId);
            var course = courses.FirstOrDefault(x => x.Id == appointment.CourseId);
            var coursePart = courseParts.FirstOrDefault(x => x.Id == appointment.CoursePartId);
            var branch = branches.FirstOrDefault(x => x.Id == appointment.BranchId);
            var subject = subjects.FirstOrDefault(x => x.Id == appointment.SubjectId);
            var staff = staffs.FirstOrDefault(x => x.Id == d.StaffId);
            if (null == staff) return null;
            var category = categories.FirstOrDefault(p => p.Id == appointment.CategoryId);

            var schedule = schedules.FirstOrDefault(x => x.InstanceId.ToInt() == appointment.InstanceId.ToInt() &&
                                                         x.FacultyId.ToInt() == appointment.FacultyId.ToInt() &&
                                                         x.CollegeId.ToInt() == appointment.CollegeId.ToInt() &&
                                                         x.CourseId.ToInt() == appointment.CourseId.ToInt() &&
                                                         x.CoursePartId.ToInt() == appointment.CoursePartId.ToInt() &&
                                                         x.CategoryId.ToInt() == appointment.CategoryId.ToInt());

            var scheduleDetail = schedule?.ScheduleDetails.FirstOrDefault(x => x.SubjectId == appointment.SubjectId);

            var designation = designations.FirstOrDefault(x => x.Id == staff.DesignationId);
            var department = departments.FirstOrDefault(x => x.Id == staff.DepartmentId);
            var appointmentBillDetail = appointment.AppointmentBillDetails.FirstOrDefault(x =>
                x.AppointmentId == d.AppointmentId && x.StaffId == d.StaffId);

            var remuneration = remunerations.FirstOrDefault(x =>
                x.FacultyId == appointment.FacultyId && x.CollegeId == appointment.CollegeId &&
                x.CourseId == appointment.CourseId);
            var remunerationDetail = remuneration?.RemunerationDetails.FirstOrDefault(x =>
                x.CoursePartId == appointment.CoursePartId);

            var reportModel = new ReportModel
            {
                Appointment = appointment,
                Schedule = schedule
            };

            // Fill Header
            FillReportHeader(reportModel, appointment, instance, faculty, college, course,
                coursePart, branch, subject, category);
            // Fill Schedule details
            FillScheduleInfo(reportModel, scheduleDetail, instance);
            // Fill Details
            FillStaffInfo(reportModel, d, college, staff,
                designation?.Name, department?.Name);
            // Fill Bill Details
            FillReportBillModel(reportModel, appointmentBillDetail);

            // Fill Remuneration
            FillRemunerationInfo(reportModel.BillModel, remunerationDetail, appointmentBillDetail);

            return reportModel;
        }).ToList();

        return dataSource.Where(p => null != p);
    }

    public IEnumerable<EvaluationDto> GetEvaluationReport(Appointment appointment)
    {
        var predicate = GetAppointmentPredicate(appointment);
        var query = GetQuery().Include(nameof(Appointment.AppointmentDetails)).Where(predicate) as IEnumerable<Appointment>;
        var dtos = from apt in query
                   from detail in apt.AppointmentDetails
                   where detail.IsPaperSetter || detail.IsModerator || detail.IsManuscript || detail.IsChairman
                   join faculty in _facultyService.GetQuery() on apt.FacultyId equals faculty.Id into facultyGroup
                   from faculty in facultyGroup.DefaultIfEmpty()
                   join college in _collegeService.GetQuery() on apt.CollegeId equals college.Id into collegeGroup
                   from college in collegeGroup.DefaultIfEmpty()
                   join course in _courseService.GetQuery() on apt.CourseId equals course.Id into courseGroup
                   from course in courseGroup.DefaultIfEmpty()
                   join coursePart in _coursePartService.GetQuery() on apt.CoursePartId equals coursePart.Id into coursePartGroup
                   from coursePart in coursePartGroup.DefaultIfEmpty()
                   join subject in _subjectService.GetQuery() on apt.SubjectId equals subject.Id into subjectGroup
                   from subject in subjectGroup.DefaultIfEmpty()
                   join staff in _staffService.GetQuery() on detail.StaffId equals staff.Id into staffGroup
                   from staff in staffGroup.DefaultIfEmpty()
                   select new EvaluationDto
                   {
                       FacultyName = faculty != null ? faculty.Name : string.Empty,
                       StaffId = staff.Id ?? 0,
                       StaffName = staff.Name,
                       ExaminerType = detail.IsChairman ? "Chairman" : "Paper Setter",
                       MobileNo = staff.Mobile,
                       EmailId = staff.Email,
                       CollegeName = college != null ? college.Name : string.Empty,
                       CourseCode = course.Id.ToString(),
                       CourseName = course != null ? course.Name : string.Empty,
                       CoursePartName = coursePart != null ? coursePart.Name : string.Empty,
                       SubjectCode = subject.Code,
                       SubjectName = subject != null ? subject.Name : string.Empty,
                   };

        return dtos;
    }

    public Appointment GetStaffs(Appointment appointment)
    {
        // Validate Header
        ValidateHeader(appointment);
        appointment.AppointmentDetails.Clear();

        appointment = GetExisting(appointment) ?? appointment;

        // Get staffs
        var staffs = _staffService.Get(p => p.StaffSubjectDetails.Any(m =>
                m.CollegeId == appointment.CollegeId && m.SubjectId == appointment.SubjectId &&
                p.Status != StatusConstants.Closed), p => p).ToList();

        foreach (var staff in staffs)
        {
            var staffSubjectDetail = staff.StaffSubjectDetails.FirstOrDefault(d => d.CollegeId == appointment.CollegeId &&
                d.SubjectId == appointment.SubjectId);
            if (null == staffSubjectDetail)
                continue;

            var appointmentDetail = appointment.AppointmentDetails.FirstOrDefault(a => a.StaffId == staff.Id);
            if (null != appointmentDetail)
            {
                appointmentDetail.StaffName = $"{staff.Salutation} {staff.Name}";
                appointmentDetail.IsInternal = staffSubjectDetail.IsInternal;
                appointmentDetail.IsBarred = staffSubjectDetail.IsBarred;
                appointmentDetail.EmailId = staff.Email;
                appointmentDetail.MobileNo = staff.Mobile;

                if (null == appointmentDetail.NoOfAttempts || (appointmentDetail.NoOfAttempts <= 0 &&
                                                               (appointmentDetail.IsPaperSetter)))
                    appointmentDetail.NoOfAttempts = 1;

                continue;
            }

            appointment.AppointmentDetails.Add(new AppointmentDetail
            {
                StaffId = staff.Id,
                StaffName = $"{staff.Salutation} {staff.Name}",
                IsInternal = staffSubjectDetail.IsInternal,
                IsBarred = staffSubjectDetail.IsBarred,
                EmailId = staff.Email,
                MobileNo = staff.Mobile,

                NoOfAttempts = 0
            });
        }

        var staffIds = staffs.Select(p => p.Id).ToList();
        appointment.AppointmentDetails.RemoveAll(d => !staffIds.Contains(d.StaffId));

        return appointment;
    }

    public void Save(Appointment appointment)
    {
        // Validate user input
        ValidateFields(appointment);

        var existing = GetExisting(appointment);
        if (null == existing)
        {
            // Add new appointment
            AddAndSave(appointment);
            return;
        }

        /*// Delete existing entries
        existing.AppointmentDetails.RemoveAll(x => appointment.AppointmentDetails.All(y =>
            y.Id != x.Id));

        // Update appointment
        foreach (var newDetail in appointment.AppointmentDetails)
        {
            var oldDetail = existing.AppointmentDetails.FirstOrDefault(x => x.StaffId == newDetail.StaffId);
            if (null == oldDetail)
            {
                existing.AppointmentDetails.Add(newDetail);
                continue;
            }

            oldDetail.IsInternal = newDetail?.IsInternal ?? false;
            oldDetail.IsBarred = newDetail?.IsBarred ?? false;
            oldDetail.IsChairman = newDetail?.IsChairman ?? false;
            oldDetail.IsPaperSetter = newDetail?.IsPaperSetter ?? false;
            oldDetail.IsModerator = newDetail?.IsModerator ?? false;
            oldDetail.IsManuscript = newDetail?.IsManuscript ?? false;

            oldDetail.OriginalId = newDetail?.OriginalId;
            oldDetail.NoOfAttempts = newDetail?.NoOfAttempts;
            oldDetail.EmailCount = newDetail?.EmailCount;
            oldDetail.SmsCount = newDetail?.SmsCount;
        }*/

        // Add existing bill details.
        appointment.AppointmentBillDetails.Clear();
        appointment.AppointmentBillDetails.AddRange(existing.AppointmentBillDetails);

        UpdateAndSave(appointment);
    }

    #endregion
}