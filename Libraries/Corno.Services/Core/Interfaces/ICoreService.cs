using Corno.Data.Core;
using Corno.Data.Payment;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Core.Interfaces;

public interface ICoreService : IBaseCoreService
{
    IGenericRepositoryCore<TBL_DISTANCE_CENTERS> TBL_DISTANCE_CENTERS_Repository { get; }
    IGenericRepositoryCore<Tbl_COLLEGE_MSTR> TBL_COLLEGE_MSTRRepository { get; }
    IGenericRepositoryCore<Tbl_COLLEGE_COURSE_MSTR> Tbl_COLLEGE_COURSE_MSTRRepository { get; }
    IGenericRepositoryCore<Tbl_COURSE_MSTR> Tbl_COURSE_MSTR_Repository { get; }
    IGenericRepositoryCore<Tbl_COURSE_TYPE_MSTR> Tbl_COURSE_TYPE_MSTR_Repository { get; }
    IGenericRepositoryCore<Tbl_COURSE_PART_MSTR> Tbl_COURSE_PART_MSTR_Repository { get; }
    IGenericRepositoryCore<Tbl_BRANCH_MSTR> Tbl_BRANCH_MSTR_Repository { get; }
    IGenericRepositoryCore<Tbl_SUBJECT_MSTR> Tbl_SUBJECT_MSTR_Repository { get; }
    IGenericRepositoryCore<Tbl_SUBJECT_CAT_MSTR> Tbl_SUBJECT_CAT_MSTR_Repository { get; }
    IGenericRepositoryCore<Tbl_SUB_CATPAP_MSTR> Tbl_SUB_CATPAP_MSTR_Repository { get; }
    IGenericRepositoryCore<Tbl_REG_TEMP> REG_TEMP_Repository { get; }
    IGenericRepositoryCore<Tbl_APP_TEMP> Tbl_APP_TEMP_Repository { get; }
    IGenericRepositoryCore<Tbl_APP_TEMP_SUB> Tbl_APP_TEMP_SUB_Repository { get; }
    IGenericRepositoryCore<TBL_STUDENT_EXAMS> TBL_STUDENT_EXAMS_Repository { get; }
    IGenericRepositoryCore<Tbl_STUDENT_INFO> Tbl_STUDENT_INFO_Repository { get; }
    IGenericRepositoryCore<Tbl_STUDENT_INFO_ADR> Tbl_STUDENT_INFO_ADR_Repository { get; }
    IGenericRepositoryCore<Tbl_STUDENT_YR_CHNG> Tbl_STUDENT_YR_CHNG_Repository { get; }
    IGenericRepositoryCore<Tbl_STUDENT_SUBJECT> Tbl_STUDENT_SUBJECT_Repository { get; }
    IGenericRepositoryCore<Tbl_STUDENT_CAT_MARKS> Tbl_STUDENT_CAT_MARKS_Repository { get; }
    IGenericRepositoryCore<Tbl_STUDENT_PAP_MARKS> Tbl_STUDENT_PAP_MARKS_Repository { get; }
    IGenericRepositoryCore<Tbl_STUDENT_COURSE> Tbl_STUDENT_COURSE_Repository { get; }
    IGenericRepositoryCore<Tbl_COPART_SYLLABUS> Tbl_COPART_SYLLABUS_Repository { get; }
    IGenericRepositoryCore<Tbl_COPART_SYLLABUS_TRX> Tbl_COPART_SYLLABUS_TRX_Repository { get; }
    IGenericRepositoryCore<Tbl_COURSE_CLASS_MSTR> Tbl_COURSE_CLASS_MSTR_Repository { get; }
    IGenericRepositoryCore<Tbl_CLASS_MSTR> Tbl_CLASS_MSTR_Repository { get; }
    IGenericRepositoryCore<TBl_STUDENT_ENV_STUDIES> TBl_STUDENT_ENV_STUDIES_Repository { get; }
    IGenericRepositoryCore<Tbl_FEE_DTL> Tbl_FEE_DTL_Repository { get; }
    IGenericRepositoryCore<Tbl_CONVO_MSTR> Tbl_CONVO_MSTR_Repository { get; }
    IGenericRepositoryCore<Tbl_STUDENT_CONVO> Tbl_STUDENT_CONVO_Repository { get; }
    IGenericRepositoryCore<Tbl_GRADE_MSTR> Tbl_GRADE_MSTR_Repository { get; }
    IGenericRepositoryCore<Tbl_CAP_SCHEDULE_MSTR> Tbl_CAP_SCHEDULE_MSTR_Repository { get; }
    IGenericRepositoryCore<TBL_STUDENT_CGPA> TBL_STUDENT_CGPA_Repository { get; }
    IGenericRepositoryCore<Tbl_SYS_INST> Tbl_SYS_INST_Repository { get; }
    IGenericRepositoryCore<TBL_STUDENT_REVALUATION> TBL_STUDENT_REVALUATION_Repository { get; }
    IGenericRepositoryCore<TBL_STUDENT_REVAL_CHILD> TBL_STUDENT_REVAL_CHILD_Repository { get; }
    IGenericRepositoryCore<Tbl_FACULTY_MSTR> Tbl_FACULTY_MSTR_Repository { get; }
    IGenericRepositoryCore<Tbl_EXAM_SCHEDULE_MSTR> Tbl_EXAM_SCHEDULE_MSTR_Repository { get; }
    IGenericRepositoryCore<Tbl_EXAM_SCHEDULE_COURSE> Tbl_EXAM_SCHEDULE_COURSE_Repository { get; }

    IGenericRepositoryCore<Tbl_EVALCAT_MSTR> Tbl_EVALCAT_MSTR_Repository { get; }

    IGenericRepositoryCore<Tbl_TIMETABLE_TRX> Tbl_TIMETABLE_TRX_Repository { get; }
    IGenericRepositoryCore<Tbl_TimeTableINST> Tbl_TimeTableINST_Repository { get; }

    IGenericRepositoryCore<Tbl_MARKS_TMP> Tbl_MARKS_TMP_Repository { get; }
    IGenericRepositoryCore<Tbl_DEGREE_MSTR> Tbl_Degree_Repository { get; }
    IGenericRepositoryCore<Tbl_ADDITIONAL_CREDITS> Tbl_Additional_Credits_Repository { get; }

    // Payment Gateway
    IGenericRepositoryCore<Payout> PayoutRepository { get; }
}