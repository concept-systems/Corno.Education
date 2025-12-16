using System.Linq;
using Corno.Data.Helpers;
using Corno.Logger;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Telerik.Reporting;
using Telerik.Reporting.Processing;
using Report = Telerik.Reporting.Report;

namespace Corno.Reports.Convocation;

public partial class ConvocationStudentsRpt : Report
{
    public ConvocationStudentsRpt()
    {
        // Required for telerik Reporting designer support
        InitializeComponent();
    }

    private void ConvocationStudentsRpt_NeedDataSource(object sender, System.EventArgs e)
    {
        var report = (Telerik.Reporting.Processing.Report)sender;

        var fromStudentConvocationNo = report.Parameters["FromStudentConvocationNo"].Value.ToInt();
        var toStudentConvocationNo = report.Parameters["ToStudentConvocationNo"].Value.ToInt();
        var convocationNo = report.Parameters["ConvocationNo"].Value.ToInt();

        var coreService = Bootstrapper.Get<ICoreService>();
        var convocations = coreService.Tbl_STUDENT_CONVO_Repository.Get(c =>
            c.Num_ST_CONVO_NO >= fromStudentConvocationNo &&
            c.Num_ST_CONVO_NO <= toStudentConvocationNo && c.Num_FK_CONVO_NO == convocationNo).ToList();

        //LogHandler.LogInfo($"Fetched convocations : {convocations.Count}");

        var collegeCodes = convocations.Select(c => c.Num_FK_COLLEGE_CD).Distinct().ToList();
        var colleges = coreService.TBL_COLLEGE_MSTRRepository.Get(c => collegeCodes.Contains(c.Num_PK_COLLEGE_CD))
            .ToList();
        var courseCodes = convocations.Select(c => c.Num_FK_CO_CD).Distinct().ToList();
        var courses = coreService.Tbl_COURSE_MSTR_Repository.Get(c => courseCodes.Contains(c.Num_PK_CO_CD))
            .ToList();

        var courseParts= coreService.Tbl_COURSE_PART_MSTR_Repository.Get(c => courseCodes.Contains(c.Num_FK_CO_CD) && 
                                                                              c.Chr_DEG_APL_FLG == "Y").ToList();
        var degreeCodes = courseParts.Select(c => c.Num_FK_Degree_CD).Distinct().ToList();
        var degrees = coreService.Tbl_Degree_Repository.Get(c => degreeCodes.Contains(c.Num_PK_DEGREE_CD))
            .ToList();

        var branchCodes = convocations.Select(c => c.Num_FK_BR_CD).Distinct().ToList();
        var branches = coreService.Tbl_BRANCH_MSTR_Repository.Get(c => branchCodes.Contains(c.Num_PK_BR_CD))
            .ToList();
        var resultCodes = convocations.Select(c => c.Num_FK_RESULT_CD).Distinct().ToList();
        var results = coreService.Tbl_CLASS_MSTR_Repository.Get(c => resultCodes.Contains(c.Num_PK_CLS_CD))
            .ToList();


        var dataSource = (from convocation in convocations
            join college in colleges on convocation.Num_FK_COLLEGE_CD equals college?.Num_PK_COLLEGE_CD into defaultCollege
            from college in defaultCollege
            join course in courses on convocation.Num_FK_CO_CD equals course?.Num_PK_CO_CD into defaultCourse
            from course in defaultCourse
            join coursePart in courseParts on convocation.Num_FK_CO_CD equals coursePart?.Num_FK_CO_CD into defaultCoursePart
            from coursePart in defaultCoursePart
            join degree in degrees on coursePart?.Num_FK_Degree_CD equals degree?.Num_PK_DEGREE_CD into defaultDegree
            from degree in defaultDegree
            /*join branch in branches on convocation.Num_FK_BR_CD ?? 0 equals branch?.Num_PK_BR_CD ?? 0 into defaultBranch
            from branch in defaultBranch*/
            join result in results on convocation.Num_FK_RESULT_CD equals result?.Num_PK_CLS_CD into defaultResult
            from result in defaultResult
            select new
            {
                Prn = convocation?.Chr_FK_PRN_NO,
                Gender = convocation?.Chr_ST_SEX_CD,
                Name = convocation?.Var_ST_NM,
                CollegeName = college?.Var_CL_COLLEGE_NM1,
                PassingMonthAndYear = $"{convocation?.Num_ST_PASS_MONTH}-{convocation?.Chr_ST_PASS_YEAR}",
                FinalResultClass = result?.Var_CLS_NM,
                InternshipMonthAndYear = $"{convocation?.Num_ST_PASS_MONTH_INT}-{convocation?.Chr_ST_PASS_YEAR_INT}",
                FatherName = convocation?.Var_FATHER_NAME,
                MotherName = convocation?.Var_MOTHER_NAME,
                BranchName = branches.FirstOrDefault(b => b.Num_PK_BR_CD == (convocation.Num_FK_BR_CD ?? 0))?.Var_BR_NM,
                StudentConvocationNo = convocation?.Num_ST_CONVO_NO,
                Photo = convocation?.Ima_ST_PHOTO,
                CourseName = degree?.Var_DEGREE_NM,
                CourseShortName = course?.Var_CO_SHRT_NM,
                Cgpa = convocation?.Num_CGPA_AVG
            }).ToList();

        //LogHandler.LogInfo($"Data source : {dataSource.Count}");

        report.DataSource = dataSource;

    }

    private void ConvocationStudentsRpt_Error(object sender, ErrorEventArgs eventArgs)
    {
        LogHandler.LogError(eventArgs.Exception);

        var procEl = (ProcessingElement)sender;
        procEl.Exception = null;
    }


}