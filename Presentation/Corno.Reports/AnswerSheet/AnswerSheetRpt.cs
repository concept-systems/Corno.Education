using Corno.Globals;
using Corno.Globals.Constants;
using Corno.Services.Bootstrapper;
using Corno.Services.Core;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Helper;
using System;
using System.Collections.Generic;
using System.Linq;
using Telerik.Reporting;

namespace Corno.Reports.AnswerSheet;

public partial class AnswerSheetRpt : Report
{
    #region -- Constructors --
    public AnswerSheetRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    public AnswerSheetRpt(int instanceId, int collegeId)
    {
        // Required for telerik Reporting designer support
        InitializeComponent();

        sdsCollege.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        if (collegeId > 0)
            sdsCollege.SelectCommand += " And Num_PK_COLLEGE_CD = " + collegeId;
        sdsCourse.ConnectionString = GlobalVariables.ConnectionStringExamServer;
        sdsCenter.ConnectionString = GlobalVariables.ConnectionStringExamServer;

        _instanceId = (short)instanceId;
    }
    #endregion

    #region -- Data Members --
    private readonly short _instanceId;
    #endregion

    #region -- Events --
    private void AnswerSheetRpt_NeedDataSource(object sender, EventArgs e)
    {
        if (!(sender is Telerik.Reporting.Processing.Report report)) return;

        var collegeId = (short)(null != report.Parameters[ModelConstants.College].Value ?
            Convert.ToInt16(((object[])report.Parameters[ModelConstants.College].Value)[0]) : 0);
        var courseIds = ((object[])report.Parameters[ModelConstants.Course].Value).ToList().Select(s => int.Parse(s.ToString())).ToList();
        var centerId = (short)(null != report.Parameters[ModelConstants.Center].Value ?
            Convert.ToInt16(report.Parameters[ModelConstants.Center].Value) : 0);

        var coreService = Bootstrapper.Get<ICoreService>();
        var instanceName = coreService.Tbl_SYS_INST_Repository.FirstOrDefault(p => p.Num_PK_INST_SRNO == _instanceId, p => p.Var_INST_REM);
        var collegeName = ExamServerHelper.GetCollegeName(collegeId, coreService);

        var transactionService = Bootstrapper.Get<ICornoService>();
        var answerSheets = transactionService.AnswerSheetRepository.Get(a => a.InstanceId == _instanceId &&
                                                                             a.CollegeId == collegeId &&
                                                                             courseIds.Contains(a.CourseId ?? 0) &&
                                                                             (a.CentreId ?? 0) == centerId, null,
                "AnswerSheetSubjects")
            .ToList();

        var dataSource = new List<AnswerSheetViewModel>();
        foreach (var answerSheet in answerSheets)
        {
            foreach (var subject in answerSheet.AnswerSheetSubjects)
            {
                if ((subject.Fee ?? 0) <= 0) continue;

                dataSource.Add(new AnswerSheetViewModel
                {
                    InstanceName = instanceName,
                    CollegeId = collegeId,
                    CollegeName = collegeName,
                    CenterId = answerSheet.CentreId ?? 0,
                    CenterName = ExamServerHelper.GetCentreName(answerSheet.CentreId, coreService),
                    CourseId = answerSheet.CourseId,
                    CourseName = ExamServerHelper.GetCourseName(answerSheet.CourseId, coreService),
                    CoursePartId = subject.CoursePartId,
                    CoursePartName = ExamServerHelper.GetCoursePartName(subject.CoursePartId, coreService),
                    BranchName = ExamServerHelper.GetBranchName(answerSheet.BranchId, coreService),

                    StudentName = ExamServerHelper.GetStudentName(answerSheet.Prn, coreService),
                    SeatNo = answerSheet.SeatNo,
                    Prn = answerSheet.Prn,
                    SubjectId = subject.SubjectId,
                    SubjectName = ExamServerHelper.GetSubjectName(subject.SubjectId, coreService),
                    Mobile = answerSheet.MobileNo,
                    Email = answerSheet.EmailId,
                    Fee = subject.Fee ?? 0
                });
            }
        }

        report.DataSource = dataSource;
    }
    #endregion

    #region -- Inner Classes --
    public class AnswerSheetViewModel
    {
        public int InstanceId { get; set; }
        public string InstanceName { get; set; }
        public int CollegeId { get; set; }
        public string CollegeName { get; set; }
        public int CenterId { get; set; }
        public string CenterName { get; set; }
        public int? CourseId { get; set; }
        public string CourseName { get; set; }
        public int? CoursePartId { get; set; }
        public string CoursePartName { get; set; }
        public string BranchName { get; set; }

        public string StudentName { get; set; }
        public string Prn { get; set; }
        public long? SeatNo { get; set; }
        public int? SubjectId { get; set; }
        public string SubjectName { get; set; }
        public string Mobile { get; set; }
        public string Email { get; set; }
        public double Fee { get; set; }
    }

    #endregion
}