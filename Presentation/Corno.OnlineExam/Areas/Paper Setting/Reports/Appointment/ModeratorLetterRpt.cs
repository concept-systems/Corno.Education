using System;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Web;
using Corno.Data.Helpers;
using Corno.Data.ViewModels.Appointment;
using Corno.Globals.Constants;
using Corno.OnlineExam.Properties;
using Corno.Reports.Base;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Paper_Setting.Interfaces;
using Corno.Services.Corno.Question_Bank.Interfaces;
using Telerik.Reporting.Processing;

namespace Corno.OnlineExam.Areas.Paper_Setting.Reports.Appointment;

public partial class ModeratorLetterRpt : BaseReport
{
    public ModeratorLetterRpt(IEnumerable<ReportModel> dataSource)
    {
        InitializeComponent();

        var imageConverter = new ImageConverter();
        pictureBox1.Value = imageConverter.ConvertFrom(Resources.logo1_min);
        pictureBox2.Value = imageConverter.ConvertFrom(Resources.logo2_min);
        pictureBox3.Value = imageConverter.ConvertFrom(Resources.roz_sir_sign);

        if (dataSource.ToList().Count <= 0)
            return;

        DataSource = dataSource;
    }

    public ModeratorLetterRpt(Data.Corno.Paper_Setting.Models.Appointment appointment)
    {
        InitializeComponent();

        var appointmentService = Bootstrapper.Get<IAppointmentService>();

        var imageConverter = new ImageConverter();
        pictureBox1.Value = imageConverter.ConvertFrom(Resources.logo1_min);
        pictureBox2.Value = imageConverter.ConvertFrom(Resources.logo2_min);
        pictureBox3.Value = imageConverter.ConvertFrom(Resources.roz_sir_sign);

        _appointment = appointment;

        var dataSource = appointmentService.GetReportDataFromAppointment(_appointment, false)
            .ToList();

        if (dataSource.Count <= 0)
            return;

        DataSource = dataSource;
    }

    #region -- Data Members --

    private readonly Data.Corno.Paper_Setting.Models.Appointment _appointment;

    #endregion

    #region -- Methods --

    /*
    private IEnumerable GetDataSource(List<int> facultyIds, List<int> collegeIds, List<int> courseIds, List<int> coursePartIds,
        List<int> branchIds, List<int> subjectIds, bool onlyInternal)
    {
        var categoryId = 53;
        var instanceId = HttpContext.Current.Session[ModelConstants.InstanceId]?.ToInt();

        var appointments = _appointmentService.Get(p =>
                p.InstanceId == instanceId && facultyIds.Contains(p.FacultyId ?? 0) &&
                collegeIds.Contains(p.CollegeId ?? 0) && courseIds.Contains(p.CourseId ?? 0) &&
                coursePartIds.Contains(p.CoursePartId ?? 0) && subjectIds.Contains(p.SubjectId ?? 0) &&
                p.CategoryId == categoryId && p.AppointmentDetails.Any(d => d.IsPaperSetter),
            p => p).ToList();
        var appointmentDetails = appointments.SelectMany(p => p.AppointmentDetails.Where(d =>
            d.IsPaperSetter ), (_, d) => d).ToList();
        
        var schedules = _scheduleService.Get(p =>
            p.InstanceId == instanceId && facultyIds.Contains(p.FacultyId ?? 0) &&
            collegeIds.Contains(p.CollegeId ?? 0) && courseIds.Contains(p.CourseId ?? 0) &&
            coursePartIds.Contains(p.CoursePartId ?? 0) &&
            p.ScheduleDetails.Any(d => subjectIds.Contains(d.SubjectId ?? 0)) &&
            p.CategoryId == categoryId, p => p).ToList();

        var staffIds = appointmentDetails.Select(d => d.StaffId).Distinct().ToList();
        var staffs = _staffService.Get(p => staffIds.Contains(p.Id), p => p).ToList();

        var instance = _instanceService.GetById(instanceId);
        var colleges = _collegeService.Get(p => collegeIds.Contains(p.Id ?? 0), p => p).ToList();
        var courses = _courseService.Get(p => courseIds.Contains(p.Id ?? 0), p => p).ToList();
        var courseParts = _coursePartService.GetViewModelList(p => coursePartIds.Contains(p.Id ?? 0)).ToList();
        var branches = _branchService.GetViewModelList(p => branchIds.Contains(p.Id ?? 0)).ToList();
        var subjects = _subjectService.Get(p => subjectIds.Contains(p.Id ?? 0), p => p).ToList();

        var dataSource = appointmentDetails.Select(d =>
        {
            var p = appointments.FirstOrDefault(x => x.Id == d.AppointmentId);
            if (null == p) return null;
            var staff = staffs.FirstOrDefault(x => x.Id == d.StaffId);
            var college = colleges.FirstOrDefault(x => x.Id == p.CollegeId);
            var course = courses.FirstOrDefault(x => x.Id == p.CourseId);
            var coursePart = courseParts.FirstOrDefault(x => x.Id == p.CoursePartId);
            var branch = branches.FirstOrDefault(x => x.Id == p.BranchId);
            var subject = subjects.FirstOrDefault(x => x.Id == p.SubjectId);
            var schedule = schedules.FirstOrDefault(x => x.FacultyId == p.FacultyId && x.CollegeId == p.CollegeId &&
                                                         x.CourseId == p.CourseId && x.CoursePartId == p.CoursePartId &&
                                                         x.CategoryId == categoryId);
            var scheduleDetail = schedule?.ScheduleDetails.FirstOrDefault(x => x.SubjectId == p.SubjectId);
            var designationName = string.Empty;
            var departmentName = string.Empty; // $"{(string.IsNullOrEmpty(designationName) ? "" : designationName )} "
            return new
            {
                InstanceName = instance?.Name,
                //p.FacultyId,
                //p.CollegeId,
                CollegeName = college?.Name,
                CourseName = course?.Name,
                course?.LetterAddress,
                CoursePartName = coursePart?.Name,
                //p.SubjectId,
                SubjectName = $"({subject?.Id}) {subject?.Name} {((p.BranchId ?? 0) > 0 ? branch?.Name : "")}",
                StaffName = $"{staff?.Salutation} {staff?.Name} {(d.IsChairman ? " (Chairman)" : "")}",
                StaffAddress = $"{designationName}{departmentName}, {(string.IsNullOrEmpty(staff?.Address1) ? college?.Address1 : staff?.Address1)}, Mob. : {staff?.Mobile} ",
                MeetingDate = scheduleDetail?.FromDate,
                //scheduleDetail?.ToDate,
                OutWardNo = $"Ref No. : BVDU/Exam/{instance?.StartDate?.Year} - {instance?.EndDate?.Year}/{scheduleDetail?.OutwardNo}",
                SetsToBeDrawn = string.Empty,
            };
        }).ToList();

        return dataSource;
    }*/
    #endregion

    #region -- Events --
    /*private void ModeratorLetterRpt_NeedDataSource(object sender, System.EventArgs e)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;

        report.DataSource = GetDataSource(new List<int> { _appointment.FacultyId ?? 0 }, new List<int> { _appointment.CollegeId ?? 0 },
            new List<int> { _appointment.CourseId ?? 0 }, new List<int> { _appointment.CoursePartId ?? 0 },
            new List<int> { _appointment.BranchId ?? 0 }, new List<int> { _appointment.SubjectId ?? 0 },
            false);
    }

    private void detail_ItemDataBound(object sender, EventArgs e)
    {
        if (!(sender is DetailSection detailSection)) return;

        var txtMeetingDateP = (TextBox)ElementTreeHelper.GetChildByName(detailSection,
            "txtMeetingDate");
        if (null == txtMeetingDateP) return;
        var dateTime = txtMeetingDateP.Value.ToDateTime();
        if (dateTime.TimeOfDay != new TimeSpan(0, 0, 0))
            return;
        dateTime = dateTime.Date + new TimeSpan(10, 00, 0);
        txtMeetingDateP.Value = dateTime;
    }*/
    #endregion
}