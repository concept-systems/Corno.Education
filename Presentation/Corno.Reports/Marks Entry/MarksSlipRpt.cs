using System.Globalization;
using System.Linq;
using Corno.Data.Corno;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Helper;
using Report = Telerik.Reporting.Report;

namespace Corno.Reports.Marks_Entry;

public partial class MarksSlipRpt : Report
{
    #region -- Constructors --
    public MarksSlipRpt(MarksEntry marksEntry)
    {
        InitializeComponent();

        var coreService = Bootstrapper.Get<ICoreService>();
        var instanceName = coreService.Tbl_SYS_INST_Repository.FirstOrDefault(p => p.Num_PK_INST_SRNO == marksEntry.InstanceId, p => p.Var_INST_REM);
        var collegeName = ExamServerHelper.GetCollegeName(marksEntry.CollegeId, coreService);
        var courseName = ExamServerHelper.GetCourseName(marksEntry.CourseId, coreService);
        var coursePartName = ExamServerHelper.GetCoursePartName(marksEntry.CoursePartId, coreService);
        var subjectName = ExamServerHelper.GetSubjectName(marksEntry.SubjectId, coreService);
        var categoryName = coreService.Tbl_EVALCAT_MSTR_Repository.Get(s => s.Num_PK_CAT_CD == marksEntry.CategoryId && s.Chr_DELETE_FLG != "Y")
            .FirstOrDefault()?.Var_CAT_NM;
        var paperName = coreService.Tbl_SUB_CATPAP_MSTR_Repository.Get(c => c.Num_FK_COPRT_NO == marksEntry.CoursePartId && 
                                                                            c.Num_FK_SUB_CD == marksEntry.SubjectId && c.Num_FK_CAT_CD == marksEntry.CategoryId &&
                                                                            c.Num_PK_PAP_CD == marksEntry.PaperId).FirstOrDefault()?.Var_PAP_NM;

        DataSource = marksEntry.MarksEntryDetails.Select(d => new
        {
            marksEntry.InstanceId, InstanceName = instanceName,
            marksEntry.CollegeId, CollegeName = collegeName,
            marksEntry.CourseId, CourseName = courseName,
            marksEntry.CoursePartId, CoursePartName = coursePartName,
            marksEntry.SubjectId, SubjectName = subjectName,
            marksEntry.CategoryId, CategoryName = categoryName,
            marksEntry.PaperId, PaperName = paperName,
            d.MaxMarks, d.Prn, d.SeatNo, 
            Marks = d.Status == "A" ? "Absent" : d.Marks?.ToString(CultureInfo.InvariantCulture)
        });
    }
    #endregion
}