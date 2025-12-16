using Corno.OnlineExam.Properties;
using Corno.Reports;
using Corno.Reports.Base;
using Corno.Services.Corno.Question_Bank.Interfaces;
using System.Drawing;
using System.Linq;
using Corno.Services.Bootstrapper;
using Corno.Services.Corno.Paper_Setting.Interfaces;
using MoreLinq;

namespace Corno.OnlineExam.Areas.Paper_Setting.Reports.Bill
{
    public partial class TaDaRpt : BaseReport
    {
        public TaDaRpt(Data.Corno.Paper_Setting.Models.Appointment appointment)
        {
            InitializeComponent();

            var imageConverter = new ImageConverter();
            pictureBox1.Value = imageConverter.ConvertFrom(Resources.logo1_min);
            pictureBox2.Value = imageConverter.ConvertFrom(Resources.logo2_min);

            var appointmentService = Bootstrapper.Get<IAppointmentService>();

            var appointments = appointmentService.Get(p =>
                    p.InstanceId == appointment.InstanceId && p.FacultyId == appointment.FacultyId &&
                    p.CollegeId == appointment.CollegeId && p.CourseId == appointment.CourseId &&
                    p.CoursePartId == appointment.CoursePartId &&
                    p.CategoryId == appointment.CategoryId && p.SubjectId == appointment.SubjectId &&
                    p.AppointmentDetails.Any(d => (d.IsPaperSetter) &&
                                                  d.StaffId == appointment.StaffId),
                p => p).ToList();

            var scheduleService = Bootstrapper.Get<IScheduleService>();
            var schedules = scheduleService.Get(p =>
                p.InstanceId == appointment.InstanceId && p.FacultyId == appointment.FacultyId &&
                p.CollegeId == appointment.CollegeId && p.CourseId == appointment.CourseId, p => p).ToList();

            var dataSource = appointmentService.GetData(appointments, schedules, false)
                .Where(d => d.StaffId == appointment.StaffId).ToList();

            dataSource.ForEach(d => d.InWords = TelerikReportHelper.GetAmountInWords(
                (d.BillModel.Ta ?? 0) + (d.BillModel.Da ?? 0) + (d.BillModel.Travel ?? 0)));

            if (dataSource.Count <= 0)
                return;

            DataSource = dataSource;
        }

        /*public TaDaRpt(int facultyId, int collegeId, int courseId, int coursePartsId, int subjectId)
        {
            InitializeComponent();

            CreateParameters();

            ReportParameters[FieldConstants.Faculty].Value = facultyId;
            ReportParameters[FieldConstants.College].Value = collegeId;
            ReportParameters[FieldConstants.Course].Value = courseId;
            ReportParameters[FieldConstants.CoursePart].Value = coursePartsId;
            ReportParameters[FieldConstants.Subject].Value = subjectId;
        }

        private void CreateParameters()
        {
            ReportHelper.CreateFacultyParameter(this);
            ReportHelper.CreateCollegeParameter(this);
            ReportHelper.CreateCourseParameter(this);
            ReportHelper.CreateCoursePartParameter(this, true);
            ReportHelper.CreateSubjectParameter(this, true, true);

            pictureBox1.Value = Resources.logo1_min;
            pictureBox2.Value = Resources.logo2_min;

            sdsMain.SelectCommand = sdsMain.SelectCommand.Replace("@InstanceId", ApplicationGlobals.InstanceId.ToString());
        }

        public static string GetAmountInWords(double? totalAmount)
        {
            return TelerikReportHelper.GetAmountInWords(totalAmount ?? 0);
        }*/
    }
}