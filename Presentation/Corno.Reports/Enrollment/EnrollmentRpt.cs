using System.Linq;
using Corno.Data.Corno;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Helper;

namespace Corno.Reports.Enrollment;

public partial class EnrollmentRpt : Telerik.Reporting.Report
{
    #region -- Constructors --
    public EnrollmentRpt(Link link)
    {
        InitializeComponent();

        var coreService = Bootstrapper.Get<ICoreService>();
        var cornoService = Bootstrapper.Get<ICornoService>();

        var instanceName = coreService.Tbl_SYS_INST_Repository.FirstOrDefault(p => p.Num_PK_INST_SRNO == link.InstanceId, p => p.Var_INST_REM);
        var collegeName = ExamServerHelper.GetCollegeName(link.CollegeId, coreService);
        var courseName = ExamServerHelper.GetCourseName(link.CourseId, coreService);
        var coursePartName = ExamServerHelper.GetCoursePartName(link.CoursePartId, coreService);

        var branchIds = link.LinkDetails.Select(d => d.BranchId).Distinct();
        var branches = coreService.Tbl_BRANCH_MSTR_Repository.Get(b => branchIds.Contains(b.Num_PK_BR_CD)).ToList();
        var prnNos = link.LinkDetails.Select(d => d.Prn).Distinct();
        var studentInfos = coreService.Tbl_STUDENT_INFO_Repository.Get(s => prnNos.Contains(s.Chr_PK_PRN_NO))
            .ToList();
        var exams = cornoService.ExamRepository.Get(e => prnNos.Contains(e.PrnNo)).ToList();

        DataSource = link.LinkDetails.Select(d =>
        {
            var branch = branches.FirstOrDefault(b => b.Num_PK_BR_CD == d.BranchId);
            return new
            {
                link.InstanceId, InstanceName = instanceName,
                link.CollegeId, CollegeName = collegeName,
                link.CourseId, CourseName = courseName,
                link.CoursePartId, CoursePartName = coursePartName,
                BranchName = branch?.Num_PK_BR_CD > 0 ? $"({branch.Num_PK_BR_CD}) {branch.Var_BR_SHRT_NM}" : string.Empty,
                    
                d.Prn, 
                StudentName = studentInfos.FirstOrDefault(s => s.Chr_PK_PRN_NO == d.Prn)?.Var_ST_NM,
                d.Mobile,
                Email = d.EmailId, d.SentDate, d.SmsResponse, d.TransactionId, d.PaymentDate, 
                Amount = exams.FirstOrDefault(e => e.PrnNo == d.Prn)?.Total
            };
        });
    }
    #endregion
}