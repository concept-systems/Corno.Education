using Corno.Data.Core;
using Corno.Data.Payment;
using Corno.Services.Core.Interfaces;

namespace Corno.Services.Core;

public class CoreService : BaseCoreService, ICoreService
{
    #region -- Constructors --
    public CoreService(IUnitOfWorkCore unitOfWorkCore)
    {
        UnitOfWorkCore = unitOfWorkCore;
    }
    #endregion

    #region -- Data Members --
    private IGenericRepositoryCore<Tbl_REG_TEMP> _regTempRepository;
    private IGenericRepositoryCore<Tbl_APP_TEMP> _tblAppTempExamsRepository;
    private IGenericRepositoryCore<Tbl_APP_TEMP_SUB> _tblAppTempSubExamsRepository;
    private IGenericRepositoryCore<Tbl_BRANCH_MSTR> _tblBranchMstrRepository;
    private IGenericRepositoryCore<Tbl_CAP_SCHEDULE_MSTR> _tblCapScheduleMstrRepository;
    private IGenericRepositoryCore<Tbl_EXAM_SCHEDULE_COURSE> _tblCapScheduleCourseRepository;
    private IGenericRepositoryCore<Tbl_CLASS_MSTR> _tblClassMstrRepository;
    private IGenericRepositoryCore<Tbl_COLLEGE_COURSE_MSTR> _tblCollegeCourseMstrRepository;
    private IGenericRepositoryCore<Tbl_COLLEGE_MSTR> _tblCollegeMstrRepository;
    private IGenericRepositoryCore<Tbl_COPART_SYLLABUS_TRX> _tblCopartSyllabusTrxRepository;
    private IGenericRepositoryCore<Tbl_COPART_SYLLABUS> _tblCopartSyllabusRepository;
    private IGenericRepositoryCore<Tbl_COURSE_CLASS_MSTR> _tblCourseClassMstrRepository;
    private IGenericRepositoryCore<Tbl_COURSE_MSTR> _tblCourseMstrRepository;
    private IGenericRepositoryCore<Tbl_COURSE_PART_MSTR> _tblCoursePartMstrRepository;
    private IGenericRepositoryCore<Tbl_COURSE_TYPE_MSTR> _tblCourseTypeMstrRepository;
    private IGenericRepositoryCore<TBL_DISTANCE_CENTERS> _tblDistanceCentersRepository;
    private IGenericRepositoryCore<Tbl_EXAM_SCHEDULE_MSTR> _tblExamScheduleMstrRepository;
    private IGenericRepositoryCore<Tbl_EXAM_SCHEDULE_COURSE> _tblExamScheduleCourseRepository;
    private IGenericRepositoryCore<Tbl_FACULTY_MSTR> _tblFacultyMstrRepository;
    private IGenericRepositoryCore<Tbl_FEE_DTL> _tblFeeDtlRepository;
    private IGenericRepositoryCore<Tbl_GRADE_MSTR> _tblGradeMstrRepository;
    private IGenericRepositoryCore<Tbl_STUDENT_CAT_MARKS> _tblStudentCatMarksRepository;
    private IGenericRepositoryCore<TBL_STUDENT_CGPA> _tblStudentCgpaRepository;
    protected IGenericRepositoryCore<Tbl_CONVO_MSTR> _tblConvoMasterRepository;
    private IGenericRepositoryCore<Tbl_STUDENT_CONVO> _tblStudentConvorRepository;
    private IGenericRepositoryCore<Tbl_STUDENT_COURSE> _tblStudentCourseRepository;
    private IGenericRepositoryCore<TBl_STUDENT_ENV_STUDIES> _tblStudentEnvStudiesRepository;
    private IGenericRepositoryCore<TBL_STUDENT_EXAMS> _tblStudentExamsRepository;
    private IGenericRepositoryCore<Tbl_STUDENT_INFO_ADR> _tblStudentInfoAdrRepository;
    private IGenericRepositoryCore<Tbl_STUDENT_INFO> _tblStudentInfoRepository;
    private IGenericRepositoryCore<Tbl_STUDENT_PAP_MARKS> _tblStudentPapMarksRepository;
    private IGenericRepositoryCore<TBL_STUDENT_REVALUATION> _tBlStudentRevaluationRepository;
    private IGenericRepositoryCore<TBL_STUDENT_REVAL_CHILD> _tBlStudentRevalChildRepository;
    private IGenericRepositoryCore<Tbl_STUDENT_SUBJECT> _tblStudentSubjectRepository;
    private IGenericRepositoryCore<Tbl_STUDENT_YR_CHNG> _tblStudentYrChngRepository;
    private IGenericRepositoryCore<Tbl_SUB_CATPAP_MSTR> _tblSubCatpapMstrRepository;
    private IGenericRepositoryCore<Tbl_SUBJECT_CAT_MSTR> _tblSubjectCatMstrRepository;
    private IGenericRepositoryCore<Tbl_SUBJECT_MSTR> _tblSubjectMstrRepository;
    private IGenericRepositoryCore<Tbl_EVALCAT_MSTR> _tblEvalcatMstr;
    private IGenericRepositoryCore<Tbl_SYS_INST> _tblSysInstRepository;
    private IGenericRepositoryCore<Tbl_TIMETABLE_TRX> _tblTimetableRepository;
    private IGenericRepositoryCore<Tbl_TimeTableINST> _tblTimeTableInstRepository;
    private IGenericRepositoryCore<Tbl_MARKS_TMP> _tblMarksTmpRepository;
    private IGenericRepositoryCore<Tbl_DEGREE_MSTR> _tblDegreeRepository;

    private IGenericRepositoryCore<Tbl_ADDITIONAL_CREDITS> _tblAdditionalCredits;

    // Payment Gateway
    private IGenericRepositoryCore<Payout> _payoutRepository;
    #endregion

    #region -- Properties --
    public IGenericRepositoryCore<Tbl_EXAM_SCHEDULE_MSTR> Tbl_EXAM_SCHEDULE_MSTR_Repository =>
        _tblExamScheduleMstrRepository ??= new GenericRepositoryCore<Tbl_EXAM_SCHEDULE_MSTR>(UnitOfWorkCore);
    public IGenericRepositoryCore<Tbl_EXAM_SCHEDULE_COURSE> Tbl_EXAM_SCHEDULE_COURSE_Repository =>
        _tblExamScheduleCourseRepository ??= new GenericRepositoryCore<Tbl_EXAM_SCHEDULE_COURSE>(UnitOfWorkCore);

    public IGenericRepositoryCore<TBL_DISTANCE_CENTERS> TBL_DISTANCE_CENTERS_Repository =>
        _tblDistanceCentersRepository ??= new GenericRepositoryCore<TBL_DISTANCE_CENTERS>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_COLLEGE_MSTR> TBL_COLLEGE_MSTRRepository =>
        _tblCollegeMstrRepository ??= new GenericRepositoryCore<Tbl_COLLEGE_MSTR>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_COLLEGE_COURSE_MSTR> Tbl_COLLEGE_COURSE_MSTRRepository =>
        _tblCollegeCourseMstrRepository ??= new GenericRepositoryCore<Tbl_COLLEGE_COURSE_MSTR>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_COURSE_MSTR> Tbl_COURSE_MSTR_Repository =>
        _tblCourseMstrRepository ??= new GenericRepositoryCore<Tbl_COURSE_MSTR>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_COURSE_TYPE_MSTR> Tbl_COURSE_TYPE_MSTR_Repository =>
        _tblCourseTypeMstrRepository ??= new GenericRepositoryCore<Tbl_COURSE_TYPE_MSTR>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_COURSE_PART_MSTR> Tbl_COURSE_PART_MSTR_Repository =>
        _tblCoursePartMstrRepository ??= new GenericRepositoryCore<Tbl_COURSE_PART_MSTR>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_BRANCH_MSTR> Tbl_BRANCH_MSTR_Repository =>
        _tblBranchMstrRepository ??= new GenericRepositoryCore<Tbl_BRANCH_MSTR>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_SUBJECT_MSTR> Tbl_SUBJECT_MSTR_Repository =>
        _tblSubjectMstrRepository ??= new GenericRepositoryCore<Tbl_SUBJECT_MSTR>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_CONVO_MSTR> Tbl_CONVO_MSTR_Repository =>
        _tblConvoMasterRepository ??= new GenericRepositoryCore<Tbl_CONVO_MSTR>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_STUDENT_CONVO> Tbl_STUDENT_CONVO_Repository =>
        _tblStudentConvorRepository ??= new GenericRepositoryCore<Tbl_STUDENT_CONVO>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_SUBJECT_CAT_MSTR> Tbl_SUBJECT_CAT_MSTR_Repository =>
        _tblSubjectCatMstrRepository ??= new GenericRepositoryCore<Tbl_SUBJECT_CAT_MSTR>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_SUB_CATPAP_MSTR> Tbl_SUB_CATPAP_MSTR_Repository =>
        _tblSubCatpapMstrRepository ??= new GenericRepositoryCore<Tbl_SUB_CATPAP_MSTR>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_REG_TEMP> REG_TEMP_Repository =>
        _regTempRepository ??= new GenericRepositoryCore<Tbl_REG_TEMP>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_APP_TEMP> Tbl_APP_TEMP_Repository =>
        _tblAppTempExamsRepository ??= new GenericRepositoryCore<Tbl_APP_TEMP>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_APP_TEMP_SUB> Tbl_APP_TEMP_SUB_Repository =>
        _tblAppTempSubExamsRepository ??= new GenericRepositoryCore<Tbl_APP_TEMP_SUB>(UnitOfWorkCore);

    public IGenericRepositoryCore<TBL_STUDENT_EXAMS> TBL_STUDENT_EXAMS_Repository =>
        _tblStudentExamsRepository ??= new GenericRepositoryCore<TBL_STUDENT_EXAMS>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_STUDENT_INFO> Tbl_STUDENT_INFO_Repository =>
        _tblStudentInfoRepository ??= new GenericRepositoryCore<Tbl_STUDENT_INFO>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_STUDENT_INFO_ADR> Tbl_STUDENT_INFO_ADR_Repository =>
        _tblStudentInfoAdrRepository ??= new GenericRepositoryCore<Tbl_STUDENT_INFO_ADR>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_STUDENT_YR_CHNG> Tbl_STUDENT_YR_CHNG_Repository =>
        _tblStudentYrChngRepository ??= new GenericRepositoryCore<Tbl_STUDENT_YR_CHNG>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_STUDENT_SUBJECT> Tbl_STUDENT_SUBJECT_Repository =>
        _tblStudentSubjectRepository ??= new GenericRepositoryCore<Tbl_STUDENT_SUBJECT>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_STUDENT_CAT_MARKS> Tbl_STUDENT_CAT_MARKS_Repository =>
        _tblStudentCatMarksRepository ??= new GenericRepositoryCore<Tbl_STUDENT_CAT_MARKS>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_STUDENT_COURSE> Tbl_STUDENT_COURSE_Repository =>
        _tblStudentCourseRepository ??= new GenericRepositoryCore<Tbl_STUDENT_COURSE>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_COPART_SYLLABUS> Tbl_COPART_SYLLABUS_Repository =>
        _tblCopartSyllabusRepository ??= new GenericRepositoryCore<Tbl_COPART_SYLLABUS>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_COPART_SYLLABUS_TRX> Tbl_COPART_SYLLABUS_TRX_Repository =>
        _tblCopartSyllabusTrxRepository ??= new GenericRepositoryCore<Tbl_COPART_SYLLABUS_TRX>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_COURSE_CLASS_MSTR> Tbl_COURSE_CLASS_MSTR_Repository =>
        _tblCourseClassMstrRepository ??= new GenericRepositoryCore<Tbl_COURSE_CLASS_MSTR>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_CLASS_MSTR> Tbl_CLASS_MSTR_Repository =>
        _tblClassMstrRepository ??= new GenericRepositoryCore<Tbl_CLASS_MSTR>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_STUDENT_PAP_MARKS> Tbl_STUDENT_PAP_MARKS_Repository =>
        _tblStudentPapMarksRepository ??= new GenericRepositoryCore<Tbl_STUDENT_PAP_MARKS>(UnitOfWorkCore);

    public IGenericRepositoryCore<TBl_STUDENT_ENV_STUDIES> TBl_STUDENT_ENV_STUDIES_Repository =>
        _tblStudentEnvStudiesRepository ??= new GenericRepositoryCore<TBl_STUDENT_ENV_STUDIES>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_FEE_DTL> Tbl_FEE_DTL_Repository =>
        _tblFeeDtlRepository ??= new GenericRepositoryCore<Tbl_FEE_DTL>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_GRADE_MSTR> Tbl_GRADE_MSTR_Repository =>
        _tblGradeMstrRepository ??= new GenericRepositoryCore<Tbl_GRADE_MSTR>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_CAP_SCHEDULE_MSTR> Tbl_CAP_SCHEDULE_MSTR_Repository =>
        _tblCapScheduleMstrRepository ??= new GenericRepositoryCore<Tbl_CAP_SCHEDULE_MSTR>(UnitOfWorkCore);

    public IGenericRepositoryCore<TBL_STUDENT_CGPA> TBL_STUDENT_CGPA_Repository =>
        _tblStudentCgpaRepository ??= new GenericRepositoryCore<TBL_STUDENT_CGPA>(UnitOfWorkCore);

    public IGenericRepositoryCore<TBL_STUDENT_REVALUATION> TBL_STUDENT_REVALUATION_Repository =>
        _tBlStudentRevaluationRepository ??= new GenericRepositoryCore<TBL_STUDENT_REVALUATION>(UnitOfWorkCore);

    public IGenericRepositoryCore<TBL_STUDENT_REVAL_CHILD> TBL_STUDENT_REVAL_CHILD_Repository =>
        _tBlStudentRevalChildRepository ??= new GenericRepositoryCore<TBL_STUDENT_REVAL_CHILD>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_SYS_INST> Tbl_SYS_INST_Repository =>
        _tblSysInstRepository ??= new GenericRepositoryCore<Tbl_SYS_INST>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_FACULTY_MSTR> Tbl_FACULTY_MSTR_Repository =>
        _tblFacultyMstrRepository ??= new GenericRepositoryCore<Tbl_FACULTY_MSTR>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_EVALCAT_MSTR> Tbl_EVALCAT_MSTR_Repository => _tblEvalcatMstr ??= new GenericRepositoryCore<Tbl_EVALCAT_MSTR>(UnitOfWorkCore);
    public IGenericRepositoryCore<Tbl_TIMETABLE_TRX> Tbl_TIMETABLE_TRX_Repository =>
        _tblTimetableRepository ??= new GenericRepositoryCore<Tbl_TIMETABLE_TRX>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_MARKS_TMP> Tbl_MARKS_TMP_Repository =>
        _tblMarksTmpRepository ??= new GenericRepositoryCore<Tbl_MARKS_TMP>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_TimeTableINST> Tbl_TimeTableINST_Repository =>
        _tblTimeTableInstRepository ??= new GenericRepositoryCore<Tbl_TimeTableINST>(UnitOfWorkCore);
    public IGenericRepositoryCore<Tbl_DEGREE_MSTR> Tbl_Degree_Repository =>
        _tblDegreeRepository ??= new GenericRepositoryCore<Tbl_DEGREE_MSTR>(UnitOfWorkCore);

    public IGenericRepositoryCore<Tbl_ADDITIONAL_CREDITS> Tbl_Additional_Credits_Repository =>
        _tblAdditionalCredits ??= new GenericRepositoryCore<Tbl_ADDITIONAL_CREDITS>(UnitOfWorkCore);


    // Payment gateway
    public IGenericRepositoryCore<Payout> PayoutRepository => _payoutRepository ??= new GenericRepositoryCore<Payout>(UnitOfWorkCore);
    #endregion

    #region -- Methods --
    public new void Save()
    {
        UnitOfWorkCore.Save();
    }
    #endregion

    #region -- Events --
    public new void Dispose(bool disposing)
    {
        if (disposing)
        {
            UnitOfWorkCore.Dispose();
        }
    }
    #endregion
}