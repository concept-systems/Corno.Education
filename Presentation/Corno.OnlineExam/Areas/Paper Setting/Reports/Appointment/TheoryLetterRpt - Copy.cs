using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using Corno.Data.Helpers;
using Corno.Globals.Constants;
using Corno.OnlineExam.Properties;
using Corno.Reports.Base;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;

namespace Corno.OnlineExam.Areas.Paper_Setting.Reports.Appointment;

public partial class TheoryLetterRpt : BaseReport
{
    #region -- Constructors --
    public TheoryLetterRpt(Data.Corno.Question_Bank.Appointment model)
    {
        InitializeComponent();

        _appointmentService = Bootstrapper.Get<IAppointmentService>();
        _scheduleService = Bootstrapper.Get<IScheduleService>();
        _instanceService = Bootstrapper.Get<IInstanceService>(); 
        _collegeService = Bootstrapper.Get<ICollegeService>(); 
        _courseService = Bootstrapper.Get<ICourseService>(); 
        _coursePartService = Bootstrapper.Get<ICoursePartService>(); 
        _branchService = Bootstrapper.Get<IBranchService>(); 
        _subjectService = Bootstrapper.Get<ISubjectService>(); 
        _staffService = Bootstrapper.Get<IStaffService>();

        var imageConverter = new ImageConverter();
        pictureBox1.Value = imageConverter.ConvertFrom(Resources.logo1_min);
        pictureBox2.Value = imageConverter.ConvertFrom(Resources.logo2_min);
        pictureBox3.Value = imageConverter.ConvertFrom(Resources.roz_sir_sign);

        DataSource = GetDataSource(new List<int> { model.FacultyId ?? 0 }, new List<int> { model.CollegeId ?? 0 },
            new List<int> { model.CourseId ?? 0 }, new List<int> { model.CoursePartId ?? 0 },
            new List<int> { model.BranchId ?? 0 }, new List<int> { model.SubjectId ?? 0 },
            false);
    }

    public TheoryLetterRpt(IAppointmentService appointmentService, IScheduleService scheduleService,
       IInstanceService instanceService, ICollegeService collegeService, ICourseService courseService,
        ICoursePartService coursePartService, IBranchService branchService, ISubjectService subjectService, IStaffService staffService)
    {
        InitializeComponent();

        _appointmentService = appointmentService;
        _scheduleService = scheduleService;
        _instanceService = instanceService;
        _collegeService = collegeService;
        _courseService = courseService;
        _coursePartService = coursePartService;
        _branchService = branchService;
        _subjectService = subjectService;
        _staffService = staffService;

        var imageConverter = new ImageConverter();

        pictureBox1.Value = imageConverter.ConvertFrom(Resources.logo1_min);
        pictureBox2.Value = imageConverter.ConvertFrom(Resources.logo2_min); 
        pictureBox3.Value = imageConverter.ConvertFrom(Resources.roz_sir_sign);

        //var instanceId = HttpContext.Current.Session[ModelConstants.InstanceId].ToString();
        //sdsMain.SelectCommand = sdsMain.SelectCommand.Replace("@InstanceId", instanceId);

        sdsMain.ConnectionString = Globals.GlobalVariables.ConnectionString;
        sdsFaculties.ConnectionString = Globals.GlobalVariables.ConnectionString;
        sdsColleges.ConnectionString = Globals.GlobalVariables.ConnectionString;
        sdsCourses.ConnectionString = Globals.GlobalVariables.ConnectionString;
        sdsCoursePart.ConnectionString = Globals.GlobalVariables.ConnectionString;
        sdsSubjects.ConnectionString = Globals.GlobalVariables.ConnectionString;
    }
    #endregion

    #region -- Data Members --

    private readonly IAppointmentService _appointmentService;
    private readonly IScheduleService _scheduleService;
    private readonly IInstanceService _instanceService;
    private readonly ICollegeService _collegeService;
    private readonly ICourseService _courseService;
    private readonly ICoursePartService _coursePartService;
    private readonly IBranchService _branchService;
    private readonly ISubjectService _subjectService;
    private readonly IStaffService _staffService;

    #endregion

    #region -- Methods --

    private IEnumerable GetDataSource(List<int> facultyIds, List<int> collegeIds, List<int> courseIds, List<int> coursePartIds,
        List<int> branchIds, List<int> subjectIds, bool isInternal)
    {
        var instanceId = HttpContext.Current.Session[ModelConstants.InstanceId]?.ToInt();

        var appointments = _appointmentService.Get(p =>
            p.InstanceId == instanceId && facultyIds.Contains(p.FacultyId ?? 0) &&
            collegeIds.Contains(p.CollegeId ?? 0) && courseIds.Contains(p.CourseId ?? 0) &&
            coursePartIds.Contains(p.CoursePartId ?? 0) && subjectIds.Contains(p.SubjectId ?? 0) &&
            p.CategoryId == 2 && p.AppointmentDetails.Any(d => d.IsPaperSetter && d.IsInternal == isInternal),
                p => p).ToList();
        var appointmentDetails = appointments.SelectMany(p => p.AppointmentDetails.Where(d =>
            d.IsPaperSetter && d.IsInternal == isInternal), (_, d) => d).ToList();

        var schedules = _scheduleService.Get(p =>
                p.InstanceId == instanceId && facultyIds.Contains(p.FacultyId ?? 0) &&
                collegeIds.Contains(p.CollegeId ?? 0) && courseIds.Contains(p.CourseId ?? 0) &&
                coursePartIds.Contains(p.CoursePartId ?? 0) &&
                p.ScheduleDetails.Any(d => subjectIds.Contains(d.SubjectId ?? 0)) &&
                p.CategoryId == 2, p => p).ToList();
        //var scheduleDetails = schedules.SelectMany(p => p.ScheduleDetails.Where(d => subjectIds.Contains(d.SubjectId ?? 0)),
        //    (_, d) => d).ToList();

        var staffIds = appointmentDetails.Select(d => d.StaffId).Distinct().ToList();
        var staffs = _staffService.Get(p => staffIds.Contains(p.Id), p => p).ToList();

        var instance = _instanceService.GetById(instanceId);
        var colleges = _collegeService.Get(p => collegeIds.Contains(p.Id ?? 0), p => p).ToList();
        var courses = _courseService.Get(p => collegeIds.Contains(p.Id ?? 0), p => p).ToList();
        var courseParts = _coursePartService.GetViewModelList(p => collegeIds.Contains(p.Id ?? 0)).ToList();
        var branches = _branchService.GetViewModelList(p => collegeIds.Contains(p.Id ?? 0)).ToList();
        var subjects = _subjectService.Get(p => subjectIds.Contains(p.Id ?? 0), p => p).ToList();

        var dataSource = appointments.SelectMany(p => p.AppointmentDetails, (p, d) =>
        {
            var staff = staffs.FirstOrDefault(x => x.Id == d.StaffId);
            var college = colleges.FirstOrDefault(x => x.Id == p.CollegeId);
            var course = courses.FirstOrDefault(x => x.Id == p.CourseId);
            var coursePart = courseParts.FirstOrDefault(x => x.Id == p.CoursePartId);
            var branch = branches.FirstOrDefault(x => x.Id == p.BranchId);
            var subject = subjects.FirstOrDefault(x => x.Id == p.SubjectId);
            var schedule = schedules.FirstOrDefault(x => x.FacultyId == p.FacultyId && x.CollegeId == p.CollegeId &&
                                                         x.CourseId == p.CourseId && x.CoursePartId == p.CoursePartId &&
                                                         x.CategoryId == 2);
            var scheduleDetail = schedule?.ScheduleDetails.FirstOrDefault(x => x.SubjectId == p.SubjectId);
            return new
            {
                p.InstanceId, InstanceName = instance?.Name, instance?.StartDate, instance?.EndDate,
                p.CollegeId, CollegeName = college?.Name, CollegeAddress = college?.Address1,
                p.CourseId, CourseName = course?.Name, course?.LetterAddress,
                p.CoursePartId, CoursePartName = coursePart?.Name,
                BranchId = p.BranchId ?? 0, BranchName = branch?.Name,
                p.SubjectId, SubjectName = subject?.Name, subject?.StandardSets, subject?.SubjectType, subject?.AvailableSets,
                d.StaffId, staff?.Salutation, StaffName = staff?.Name, staff?.Address1, staff?.Phone, staff?.Mobile, staff?.Address2,
                DesignationName = string.Empty,
                DepartmentName = string.Empty,
                MeetingDate = scheduleDetail?.FromDate,
                scheduleDetail?.OutwardNo,
                d.IsInternal,
                d.IsChairman,
                d.IsPaperSetter,
                SetsToBeDrawn = string.Empty,
            };
        }).ToList();

        return dataSource;
    }
#endregion

    #region -- Events --
    private void TheoryLetterRpt_NeedDataSource(object sender, System.EventArgs e)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;

        var facultyIds = ((object[])report.Parameters[ModelConstants.Course].Value)
            .Select(s => int.Parse(s.ToString())).ToList();
        var collegeIds = ((object[])report.Parameters[ModelConstants.College].Value)
            .Select(s => int.Parse(s.ToString())).ToList();
        var courseIds = ((object[])report.Parameters[ModelConstants.Course].Value)
            .Select(s => int.Parse(s.ToString())).ToList();
        var coursePartIds = ((object[])report.Parameters[ModelConstants.CoursePart].Value)
            .Select(s => int.Parse(s.ToString())).ToList();
        var branchIds = ((object[])report.Parameters[ModelConstants.Branch].Value)
            .Select(s => int.Parse(s.ToString())).ToList();
        var subjectIds = ((object[])report.Parameters[ModelConstants.Subject].Value)
            .Select(s => int.Parse(s.ToString())).ToList();
        var isInternal = report.Parameters[ModelConstants.IsInternal].Value.ToBoolean();
       

        report.DataSource = GetDataSource(facultyIds, collegeIds, courseIds, coursePartIds, 
            branchIds, subjectIds, isInternal);
    }
    #endregion
}