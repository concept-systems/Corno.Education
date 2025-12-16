
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Corno.Data.Corno.Question_Bank;
using Corno.Data.Helpers;
using Corno.Data.ViewModels.Appointment;
using Corno.Globals.Constants;
using Corno.Logger;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;

namespace Corno.Services.Corno.Question_Bank;

public class PurchaseInvoiceService : MainService<PurchaseInvoice>, IPurchaseInvoiceService
{
    #region -- Constructors --
    public PurchaseInvoiceService(IStaffService staffService)
    {
        _staffService = staffService;

        SetIncludes(nameof(PurchaseInvoice.PurchaseInvoiceDetails));
    }
    #endregion

    #region -- Data Members --

    private readonly IStaffService _staffService;

    private string _smsUrl;
    //private EmailSetting _emailSetting;

    #endregion

    #region -- Private Methods --
    private PurchaseInvoice GetExisting(PurchaseInvoice purchaseInvoice)
    {
        var existing = FirstOrDefault(p =>
                (p.InstanceId ?? 0) == (purchaseInvoice.InstanceId ?? 0) &&
                (p.FacultyId ?? 0) == (purchaseInvoice.FacultyId ?? 0) && (p.CollegeId ?? 0) == (purchaseInvoice.CollegeId ?? 0) &&
                (p.CourseId ?? 0) == (purchaseInvoice.CourseId ?? 0) &&
                (p.CoursePartId ?? 0) == (purchaseInvoice.CoursePartId ?? 0) &&
                (p.BranchId ?? 0) == (purchaseInvoice.BranchId ?? 0) &&
                (p.CategoryId ?? 0) == (purchaseInvoice.CategoryId ?? 0) &&
                (p.SubjectId ?? 0) == (purchaseInvoice.SubjectId ?? 0), a => a);

        return existing;
    }

    private IEnumerable<ReportModel> GetData(List<int> facultyIds, List<int> collegeIds, List<int> courseIds, List<int> coursePartIds,
        List<int> branchIds, List<int> subjectIds, int categoryId, bool onlyInternal)
    {
        var scheduleService = Bootstrapper.Bootstrapper.Get<IScheduleService>();
        var facultyService = Bootstrapper.Bootstrapper.Get<IFacultyService>();
        var instanceService = Bootstrapper.Bootstrapper.Get<IInstanceService>();
        var collegeService = Bootstrapper.Bootstrapper.Get<ICollegeService>();
        var courseService = Bootstrapper.Bootstrapper.Get<ICourseService>();
        var coursePartService = Bootstrapper.Bootstrapper.Get<ICoursePartService>();
        var branchService = Bootstrapper.Bootstrapper.Get<IBranchService>();
        var categoryService = Bootstrapper.Bootstrapper.Get<ICategoryService>();
        var subjectService = Bootstrapper.Bootstrapper.Get<ISubjectService>();

        //const int categoryId = 2;
        var instanceId = HttpContext.Current.Session[ModelConstants.InstanceId]?.ToInt();

        var purchaseInvoices = Get(p =>
            p.InstanceId == instanceId && facultyIds.Contains(p.FacultyId ?? 0) &&
            collegeIds.Contains(p.CollegeId ?? 0) && courseIds.Contains(p.CourseId ?? 0) &&
            coursePartIds.Contains(p.CoursePartId ?? 0) && /*subjectIds.Contains(p.SubjectId ?? 0) &&
            p.CategoryId == categoryId &&*/ p.PurchaseInvoiceDetails.Any(d => d.IsPaperSetter),
                p => p).ToList();
        var purchaseInvoiceDetails = purchaseInvoices.SelectMany(p => p.PurchaseInvoiceDetails.Where(d =>
            d.IsPaperSetter), (_, d) => d).ToList();
        if (onlyInternal)
            purchaseInvoiceDetails = purchaseInvoiceDetails.Where(d => d.IsInternal == true).ToList();

        var schedules = scheduleService.Get(p =>
                p.InstanceId == instanceId && facultyIds.Contains(p.FacultyId ?? 0) &&
                collegeIds.Contains(p.CollegeId ?? 0) && courseIds.Contains(p.CourseId ?? 0) &&
                coursePartIds.Contains(p.CoursePartId ?? 0) &&
                //p.ScheduleDetails.Any(d => subjectIds.Contains(d.SubjectId ?? 0)) &&
                p.CategoryId == categoryId, p => p).ToList();

        var staffIds = purchaseInvoiceDetails.Select(d => d.StaffId).Distinct().ToList();
        var staffs = _staffService.Get(p => staffIds.Contains(p.Id), p => p).ToList();

        var instance = instanceService.GetById(instanceId);
        var faculties = facultyService.Get(p => facultyIds.Contains(p.Id ?? 0), p => p).ToList();
        var colleges = collegeService.Get(p => collegeIds.Contains(p.Id ?? 0), p => p).ToList();
        var courses = courseService.Get(p => courseIds.Contains(p.Id ?? 0), p => p).ToList();
        var courseParts = coursePartService.GetViewModelList(p => coursePartIds.Contains(p.Id ?? 0)).ToList();
        var branches = branchService.GetViewModelList(p => branchIds.Contains(p.Id ?? 0)).ToList();
        var category = categoryService.GetById(categoryId);

        subjectIds = purchaseInvoices.Select(p => p.SubjectId ?? 0).Distinct().ToList();
        var subjects = subjectService.Get(p => subjectIds.Contains(p.Id ?? 0), p => p).ToList();

        var dataSource = purchaseInvoiceDetails.Select(d =>
        {
            var p = purchaseInvoices.FirstOrDefault(x => x.Id == d.PurchaseInvoiceId);
            if (null == p) return null;

            var faculty = faculties.FirstOrDefault(x => x.Id == p.FacultyId);
            var college = colleges.FirstOrDefault(x => x.Id == p.CollegeId);
            var course = courses.FirstOrDefault(x => x.Id == p.CourseId);
            var coursePart = courseParts.FirstOrDefault(x => x.Id == p.CoursePartId);
            var branch = branches.FirstOrDefault(x => x.Id == p.BranchId);
            var subject = subjects.FirstOrDefault(x => x.Id == p.SubjectId);
            var staff = staffs.FirstOrDefault(x => x.Id == d.StaffId);

            var schedule = schedules.FirstOrDefault(x => x.FacultyId == p.FacultyId && x.CollegeId == p.CollegeId &&
                                                         x.CourseId == p.CourseId && x.CoursePartId == p.CoursePartId &&
                                                         x.CategoryId == categoryId);
            var scheduleDetail = schedule?.ScheduleDetails.FirstOrDefault(x => x.SubjectId == p.SubjectId);
            var designationName = string.Empty;         // TODO : Fill it
            var departmentName = string.Empty;          // TODO : Fill it
            return new ReportModel
            {
                ////InstanceName = instance?.Name,
                ////FacultyId = p.FacultyId,
                ////FacultyName = faculty?.Name,
                ////CollegeId = p.CollegeId,
                ////CollegeName = college?.Name,
                ////CourseId = p.CourseId,
                ////CourseName = course?.Name,
                LetterAddress = course?.LetterAddress,
                PrintSequenceNo = course?.PrintSequenceNo,
                //CoursePartId = p.CoursePartId,
                //CoursePartName = coursePart?.Name,
                //BranchId = branch?.Id,
                //BranchName = branch?.Name,
                //SubjectId = p.SubjectId,
                //SubjectName = $"({subject?.Id}) {subject?.Name} {((p.BranchId ?? 0) > 0 ? branch?.Name : "")}",
                StandardSets = subject?.StandardSets,
                SubjectType = subject?.SubjectType,
                //CategoryId = p.CategoryId,
                //CategoryName = category?.Name,
                StaffId = d.StaffId,
                StaffName = $"{staff?.Salutation} {staff?.Name} {(d.IsChairman ? " (Chairman)" : "")}",
                StaffAddress = $"{designationName}{departmentName}, \n {(string.IsNullOrEmpty(staff?.Address1) ? college?.Address1 : staff.Address1)}, \n Mob. : {staff?.Mobile} ",
                //StaffAddress = $"{staff?.Address1}",
                Phone = staff?.Phone,
                MobileNo = staff?.Mobile,
                EmailId = staff?.Email,
                Address2 = staff?.Address2,
                MeetingDate = scheduleDetail?.FromDate,
                OutWardNo = $"Ref No. : BVDU/Exam/{instance?.StartDate?.Year} - {instance?.EndDate?.Year}/{scheduleDetail?.OutwardNo}",
                IsChairman = d.IsChairman,
                IsInternal = d.IsInternal,
                IsModerator = d.IsModerator,
                IsManuscript = d.IsManuscript,
                SetsToBeDrawn = 0,                      // TODO : Fill it
                NoOfAttempts = d.NoOfAttempts,
                OriginalId = d.OriginalId,
                BosId = 0,                              // TODO : Fill it
                BosName = string.Empty                  // TODO : Fill it
            };
        }).ToList();

        foreach(var detail in dataSource)
            LogHandler.LogInfo($"StaffId : {detail.StaffId}, StaffName: {detail.StaffName}, Address 1 : {detail.StaffAddress}, EmailId: {detail.EmailId}, Address2: {detail.Address2}");

        return dataSource;
    }

    #endregion

    #region -- Public Methods --
    public void ValidateReportHeader(PurchaseInvoice purchaseInvoice)
    {
        if (purchaseInvoice.InstanceId.ToInt() <= 0)
            throw new Exception("Invalid instance / session.");
        if (purchaseInvoice.CollegeId.ToInt() <= 0)
            throw new Exception("Invalid College.");
        if (purchaseInvoice.CourseId.ToInt() <= 0)
            throw new Exception("Invalid Course.");
        //if (purchaseInvoice.CoursePartId.ToInt() <= 0)
        //    throw new Exception("Invalid Course Part.");
    }

    public void ValidateHeader(PurchaseInvoice purchaseInvoice)
    {
        if (purchaseInvoice.InstanceId.ToInt() <= 0)
            throw new Exception("Invalid instance / session.");
        if (purchaseInvoice.CollegeId.ToInt() <= 0)
            throw new Exception("Invalid College.");
        if (purchaseInvoice.CourseId.ToInt() <= 0)
            throw new Exception("Invalid Course.");
        if (purchaseInvoice.CoursePartId.ToInt() <= 0)
            throw new Exception("Invalid Course Part.");
        if (purchaseInvoice.CategoryId.ToInt() <= 0)
            throw new Exception("Invalid Subject Category.");
        if (purchaseInvoice.SubjectId.ToInt() <= 0)
            throw new Exception("Invalid Subject.");
    }

    public void ValidateFields(PurchaseInvoice purchaseInvoice)
    {
        // Validate Header
        ValidateHeader(purchaseInvoice);

        if (purchaseInvoice.PurchaseInvoiceDetails.Count <= 0)
            throw new Exception("No rows in bill.");
        if (purchaseInvoice.PurchaseInvoiceDetails.All(d => null == d.StaffId))
            throw new Exception("All rows are blank.");
    }

    public IEnumerable<ReportModel> GetData(PurchaseInvoice purchaseInvoice, bool onlyInternal)
    {
        return GetData(new List<int> { purchaseInvoice.FacultyId ?? 0 }, new List<int> { purchaseInvoice.CollegeId ?? 0 },
            new List<int> { purchaseInvoice.CourseId ?? 0 }, new List<int> { purchaseInvoice.CoursePartId ?? 0 },
            new List<int> { purchaseInvoice.BranchId ?? 0 }, new List<int> { purchaseInvoice.SubjectId ?? 0 }, purchaseInvoice.CategoryId ?? 0,
            onlyInternal);
    }

    public PurchaseInvoice GetStaffs(PurchaseInvoice purchaseInvoice)
    {
        // Validate Header
        ValidateHeader(purchaseInvoice);
        purchaseInvoice.PurchaseInvoiceDetails.Clear();

        purchaseInvoice = GetExisting(purchaseInvoice) ?? purchaseInvoice;

        // Get staffs
        var subjectStaff = _staffService.Get(p => p.StaffSubjectDetails.Any(m =>
                m.CollegeId == purchaseInvoice.CollegeId && m.SubjectId == purchaseInvoice.SubjectId), p => p).ToList();

        foreach (var staff in subjectStaff)
        {
            var staffSubjectDetail = staff.StaffSubjectDetails.FirstOrDefault(d => d.CollegeId == purchaseInvoice.CollegeId &&
                d.SubjectId == purchaseInvoice.SubjectId);
            if (null == staffSubjectDetail)
                continue;

            var purchaseInvoiceDetail = purchaseInvoice.PurchaseInvoiceDetails.FirstOrDefault(a => a.StaffId == staff.Id);
            if (null != purchaseInvoiceDetail)
            {
                purchaseInvoiceDetail.StaffName = $"{staff.Salutation} {staff.Name}";
                purchaseInvoiceDetail.IsInternal = staffSubjectDetail.IsInternal;
                purchaseInvoiceDetail.IsBarred = staffSubjectDetail.IsBarred;
                purchaseInvoiceDetail.EmailId = staff.Email;
                purchaseInvoiceDetail.MobileNo = staff.Mobile;
                continue;
            }

            purchaseInvoice.PurchaseInvoiceDetails.Add(new PurchaseInvoiceDetail
            {
                StaffId = staff.Id,
                StaffName = $"{staff.Salutation} {staff.Name}",
                IsInternal = staffSubjectDetail.IsInternal,
                IsBarred = staffSubjectDetail.IsBarred,
                EmailId = staff.Email,
                MobileNo = staff.Mobile
            });
        }

        return purchaseInvoice;
    }

    public void Save(PurchaseInvoice purchaseInvoice)
    {
        // Validate user input
        ValidateFields(purchaseInvoice);

        var existing = GetExisting(purchaseInvoice);
        if (null == existing)
        {
            // Add new PurchaseInvoice
            AddAndSave(purchaseInvoice);
            return;
        }

        // Update Purchase Invoice
        foreach (var newDetail in purchaseInvoice.PurchaseInvoiceDetails)
        {
            var oldDetail = existing.PurchaseInvoiceDetails.FirstOrDefault(x => x.StaffId == newDetail.StaffId);
            if (null == oldDetail)
            {
                existing.PurchaseInvoiceDetails.Add(newDetail);
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
        }

        UpdateAndSave(existing);
    }

    

    #endregion
}