using System.Data.Entity;
using OnlineExam.Models.Mapping;

namespace OnlineExam.Models
{
    public partial class BHVEDPSNETContext : DbContext
    {
        static BHVEDPSNETContext()
        {
            Database.SetInitializer<BHVEDPSNETContext>(null);
        }

        public BHVEDPSNETContext()
            : base("Name=BHVEDPSNETContext")
        {
        }

        //public DbSet<dtproperty> dtproperties { get; set; }
        //public DbSet<HIST_Tbl_CODE_DECODE> HIST_Tbl_CODE_DECODE { get; set; }
        //public DbSet<HIST_Tbl_GRADE_TMP> HIST_Tbl_GRADE_TMP { get; set; }
        //public DbSet<HIST_Tbl_MARKS_TMP> HIST_Tbl_MARKS_TMP { get; set; }
        //public DbSet<HIST_TBL_STUDENT_EXAMS> HIST_TBL_STUDENT_EXAMS { get; set; }
        //public DbSet<HIST_TBL_STUDENT_GROUP> HIST_TBL_STUDENT_GROUP { get; set; }
        //public DbSet<HIST_Tbl_STUDENT_INFO> HIST_Tbl_STUDENT_INFO { get; set; }
        //public DbSet<HIST_Tbl_STUDENT_PAP_MARKS> HIST_Tbl_STUDENT_PAP_MARKS { get; set; }
        //public DbSet<HIST_Tbl_STUDENT_SEC_MARKS> HIST_Tbl_STUDENT_SEC_MARKS { get; set; }
        //public DbSet<HIST_Tbl_STUDENT_SUBJECT> HIST_Tbl_STUDENT_SUBJECT { get; set; }
        public DbSet<Tbl_APP_TEMP> Tbl_APP_TEMP { get; set; }
       
        public DbSet<Tbl_APP_TEMP_SUB> Tbl_APP_TEMP_SUB { get; set; }
        //public DbSet<Tbl_BOS_MSTR> Tbl_BOS_MSTR { get; set; }
        //public DbSet<Tbl_BOS_SUBJECT_MSTR> Tbl_BOS_SUBJECT_MSTR { get; set; }
        public DbSet<Tbl_BRANCH_MSTR> Tbl_BRANCH_MSTR { get; set; }
        //public DbSet<Tbl_BRANCH_SUBJECT_MSTR> Tbl_BRANCH_SUBJECT_MSTR { get; set; }
        //public DbSet<TBL_CALCULATE_GP> TBL_CALCULATE_GP { get; set; }
        //public DbSet<Tbl_CAP_MSTR> Tbl_CAP_MSTR { get; set; }
        public DbSet<Tbl_CAP_SCHEDULE_MSTR> Tbl_CAP_SCHEDULE_MSTR { get; set; }
        //public DbSet<Tbl_CAP_TRX> Tbl_CAP_TRX { get; set; }
        //public DbSet<Tbl_CAST_MSTR> Tbl_CAST_MSTR { get; set; }
        //public DbSet<Tbl_CAT_EXAM_PATTERN> Tbl_CAT_EXAM_PATTERN { get; set; }
        //public DbSet<Tbl_CENTER_COL_TRX> Tbl_CENTER_COL_TRX { get; set; }
        //public DbSet<TBL_CENTER_SEAT_NOS> TBL_CENTER_SEAT_NOS { get; set; }
        //public DbSet<Tbl_CENTER_TRX> Tbl_CENTER_TRX { get; set; }
        //public DbSet<TBL_CGPA_PERCENTAGE> TBL_CGPA_PERCENTAGE { get; set; }
        public DbSet<Tbl_CLASS_MSTR> Tbl_CLASS_MSTR { get; set; }
        //public DbSet<Tbl_CODE_DECODE> Tbl_CODE_DECODE { get; set; }
        public DbSet<Tbl_COLLEGE_COURSE_MSTR> Tbl_COLLEGE_COURSE_MSTR { get; set; }
        //public DbSet<Tbl_COLLEGE_LAB_DTL> Tbl_COLLEGE_LAB_DTL { get; set; }
        //public DbSet<Tbl_COLLEGE_LAB_MSTR> Tbl_COLLEGE_LAB_MSTR { get; set; }
        public DbSet<Tbl_COLLEGE_MSTR> Tbl_COLLEGE_MSTR { get; set; }
        //public DbSet<Tbl_CONVO_MSTR> Tbl_CONVO_MSTR { get; set; }
        //public DbSet<TBL_COPART_GROUPS> TBL_COPART_GROUPS { get; set; }
        //public DbSet<TBL_COPART_GRP_SYLLABUS> TBL_COPART_GRP_SYLLABUS { get; set; }
        //public DbSet<Tbl_COPART_MAIN_GROUPS> Tbl_COPART_MAIN_GROUPS { get; set; }
        //public DbSet<TBL_COPART_SUB_GROUPS> TBL_COPART_SUB_GROUPS { get; set; }
        public DbSet<Tbl_COPART_SYLLABUS> Tbl_COPART_SYLLABUS { get; set; }
        public DbSet<Tbl_COPART_SYLLABUS_TRX> Tbl_COPART_SYLLABUS_TRX { get; set; }
        //public DbSet<TBL_COURSE_CAT_SEQ> TBL_COURSE_CAT_SEQ { get; set; }
        public DbSet<Tbl_COURSE_CLASS_MSTR> Tbl_COURSE_CLASS_MSTR { get; set; }
        //public DbSet<Tbl_COURSE_ELIGIBILITY_DTL_MSTR> Tbl_COURSE_ELIGIBILITY_DTL_MSTR { get; set; }
        //public DbSet<Tbl_COURSE_ELIGIBILITY_DTL_MSTR1> Tbl_COURSE_ELIGIBILITY_DTL_MSTR1 { get; set; }
        //public DbSet<Tbl_COURSE_ELIGIBILITY_MSTR> Tbl_COURSE_ELIGIBILITY_MSTR { get; set; }
        //public DbSet<TBl_COURSE_LOCK_MSTR> TBl_COURSE_LOCK_MSTR { get; set; }
        public DbSet<Tbl_COURSE_MSTR> Tbl_COURSE_MSTR { get; set; }
        //public DbSet<Tbl_COURSE_PART_INST_TRX> Tbl_COURSE_PART_INST_TRX { get; set; }
        public DbSet<Tbl_COURSE_PART_MSTR> Tbl_COURSE_PART_MSTR { get; set; }
        public DbSet<Tbl_COURSE_TYPE_MSTR> Tbl_COURSE_TYPE_MSTR { get; set; }
        //public DbSet<Tbl_DEBR_MSTR> Tbl_DEBR_MSTR { get; set; }
        //public DbSet<Tbl_DEGREE_MSTR> Tbl_DEGREE_MSTR { get; set; }
        //public DbSet<Tbl_DIST_MSTR> Tbl_DIST_MSTR { get; set; }
        public DbSet<TBL_DISTANCE_CENTERS> TBL_DISTANCE_CENTERS { get; set; }
        //public DbSet<Tbl_DIVISION_MSTR> Tbl_DIVISION_MSTR { get; set; }
        //public DbSet<TBL_ELIG_DOC> TBL_ELIG_DOC { get; set; }
        //public DbSet<TBL_ELIG_MST> TBL_ELIG_MST { get; set; }
        //public DbSet<Tbl_ENROLL_TEMP> Tbl_ENROLL_TEMP { get; set; }
        //public DbSet<Tbl_ENROLL_TEMP_ADR> Tbl_ENROLL_TEMP_ADR { get; set; }
        //public DbSet<Tbl_EVALCAT_MSTR> Tbl_EVALCAT_MSTR { get; set; }
        public DbSet<Tbl_EXAM_SCHEDULE_MSTR> Tbl_EXAM_SCHEDULE_MSTR { get; set; }
        //public DbSet<Tbl_EXAM_SCHEDULE_TRX> Tbl_EXAM_SCHEDULE_TRX { get; set; }
        //public DbSet<TBL_EXAM_TYPE_MSTR> TBL_EXAM_TYPE_MSTR { get; set; }
        //public DbSet<Tbl_EXAMINER_DEG_MSTR> Tbl_EXAMINER_DEG_MSTR { get; set; }
        //public DbSet<Tbl_EXAMINER_MSTR> Tbl_EXAMINER_MSTR { get; set; }
        //public DbSet<Tbl_EXAMINER_PAIR> Tbl_EXAMINER_PAIR { get; set; }
        //public DbSet<Tbl_EXAMINER_PANNEL> Tbl_EXAMINER_PANNEL { get; set; }
        //public DbSet<Tbl_EXAMINER_SUB_SPECIALIZATION> Tbl_EXAMINER_SUB_SPECIALIZATION { get; set; }
        //public DbSet<Tbl_EXAMINER_SUB_SPECIALIZATION_TEMP> Tbl_EXAMINER_SUB_SPECIALIZATION_TEMP { get; set; }
        //public DbSet<Tbl_EXAMINER_SUB_TAUGHT> Tbl_EXAMINER_SUB_TAUGHT { get; set; }
        //public DbSet<Tbl_EXAMINER_SUB_TAUGHT_TEMP> Tbl_EXAMINER_SUB_TAUGHT_TEMP { get; set; }
        //public DbSet<TBL_EXAMS_GROUP> TBL_EXAMS_GROUP { get; set; }
        //public DbSet<Tbl_EXM_PATT_MSTR> Tbl_EXM_PATT_MSTR { get; set; }
        //public DbSet<Tbl_EXMR_TYPE_MSTR> Tbl_EXMR_TYPE_MSTR { get; set; }
        public DbSet<Tbl_FACULTY_MSTR> Tbl_FACULTY_MSTR { get; set; }
        public DbSet<Tbl_FEE_DTL> Tbl_FEE_DTL { get; set; }
        public DbSet<Tbl_FEE_MSTR> Tbl_FEE_MSTR { get; set; }
        public DbSet<Tbl_GRADE_MSTR> Tbl_GRADE_MSTR { get; set; }
        //public DbSet<TBL_GRADE_POINT_MSTR> TBL_GRADE_POINT_MSTR { get; set; }
        //public DbSet<Tbl_GRADE_TMP> Tbl_GRADE_TMP { get; set; }
        //public DbSet<Tbl_INCOMEGRP_MSTR> Tbl_INCOMEGRP_MSTR { get; set; }
        //public DbSet<TBl_INST_APPOINTMENT> TBl_INST_APPOINTMENT { get; set; }
        //public DbSet<Tbl_INVALID_ERROR> Tbl_INVALID_ERROR { get; set; }
        //public DbSet<Tbl_LAB_BATCH> Tbl_LAB_BATCH { get; set; }
        //public DbSet<TBL_LPS_MSTR> TBL_LPS_MSTR { get; set; }
        //public DbSet<Tbl_MARKS_TMP> Tbl_MARKS_TMP { get; set; }
        //public DbSet<Tbl_MARKS_TMP_CAP> Tbl_MARKS_TMP_CAP { get; set; }
        //public DbSet<Tbl_ORDINANCE_MSTR> Tbl_ORDINANCE_MSTR { get; set; }
        //public DbSet<TBL_PAPER_DIV_MASTER> TBL_PAPER_DIV_MASTER { get; set; }
        //public DbSet<TBL_PRE_REQUISITES> TBL_PRE_REQUISITES { get; set; }
        //public DbSet<Tbl_PREV_EXAM_DTL> Tbl_PREV_EXAM_DTL { get; set; }
       public DbSet<TBL_STUDENT_REVALUATION> TBL_STUDENT_REVALUATION { get; set; }
        public DbSet<Tbl_REG_TEMP> Tbl_REG_TEMP { get; set; }
        //public DbSet<TBL_REG_TEMP_COLLEGE> TBL_REG_TEMP_COLLEGE { get; set; }
        //public DbSet<Tbl_REG_TEMP_SUB> Tbl_REG_TEMP_SUB { get; set; }
        //public DbSet<TBL_SEAT_ALLOCATION> TBL_SEAT_ALLOCATION { get; set; }
        //public DbSet<Tbl_STATE_MSTR> Tbl_STATE_MSTR { get; set; }
        //public DbSet<Tbl_STD_BATCH> Tbl_STD_BATCH { get; set; }
        //public DbSet<Tbl_STREAM_MSTR> Tbl_STREAM_MSTR { get; set; }
        //public DbSet<Tbl_STUD_ACTIVITY_MSTR> Tbl_STUD_ACTIVITY_MSTR { get; set; }
        //public DbSet<Tbl_STUD_CATEGORY_MSTR> Tbl_STUD_CATEGORY_MSTR { get; set; }
        public DbSet<Tbl_STUDENT_CAT_MARKS> Tbl_STUDENT_CAT_MARKS { get; set; }
        //public DbSet<Tbl_STUDENT_CAT_MARKS_BP> Tbl_STUDENT_CAT_MARKS_BP { get; set; }
        //public DbSet<Tbl_STUDENT_CAT_MARKS_RA> Tbl_STUDENT_CAT_MARKS_RA { get; set; }
        public DbSet<TBL_STUDENT_CGPA> TBL_STUDENT_CGPA { get; set; }
        public DbSet<Tbl_STUDENT_CONVO> Tbl_STUDENT_CONVO { get; set; }
        public DbSet<Tbl_STUDENT_COURSE> Tbl_STUDENT_COURSE { get; set; }
        //public DbSet<Tbl_STUDENT_COURSE_PART> Tbl_STUDENT_COURSE_PART { get; set; }
        //public DbSet<Tbl_STUDENT_DEBAR> Tbl_STUDENT_DEBAR { get; set; }
        //public DbSet<Tbl_STUDENT_DEBARRED> Tbl_STUDENT_DEBARRED { get; set; }
        public DbSet<TBl_STUDENT_ENV_STUDIES> TBl_STUDENT_ENV_STUDIES { get; set; }
        public DbSet<TBL_STUDENT_EXAMS> TBL_STUDENT_EXAMS { get; set; }
        //public DbSet<TBL_STUDENT_GROUP> TBL_STUDENT_GROUP { get; set; }
        public DbSet<Tbl_STUDENT_INFO> Tbl_STUDENT_INFO { get; set; }
        public DbSet<Tbl_STUDENT_INFO_ADR> Tbl_STUDENT_INFO_ADR { get; set; }
        //public DbSet<TBL_STUDENT_LAPSES> TBL_STUDENT_LAPSES { get; set; }
        //public DbSet<TBL_STUDENT_LPS_TRN> TBL_STUDENT_LPS_TRN { get; set; }
        //public DbSet<Tbl_STUDENT_MIG_COURSE> Tbl_STUDENT_MIG_COURSE { get; set; }
        public DbSet<Tbl_STUDENT_PAP_MARKS> Tbl_STUDENT_PAP_MARKS { get; set; }
        //public DbSet<Tbl_STUDENT_PAP_MARKS_BP> Tbl_STUDENT_PAP_MARKS_BP { get; set; }
        //public DbSet<Tbl_STUDENT_PAP_MARKS_RA> Tbl_STUDENT_PAP_MARKS_RA { get; set; }
        //public DbSet<Tbl_STUDENT_PRV_STATUS> Tbl_STUDENT_PRV_STATUS { get; set; }
        //public DbSet<Tbl_STUDENT_SEC_MARKS> Tbl_STUDENT_SEC_MARKS { get; set; }
        //public DbSet<Tbl_STUDENT_SEC_MARKS_BP> Tbl_STUDENT_SEC_MARKS_BP { get; set; }
        //public DbSet<Tbl_STUDENT_SEC_MARKS_RA> Tbl_STUDENT_SEC_MARKS_RA { get; set; }
        public DbSet<Tbl_STUDENT_SUBJECT> Tbl_STUDENT_SUBJECT { get; set; }
        //public DbSet<Tbl_STUDENT_SUBJECT_BP> Tbl_STUDENT_SUBJECT_BP { get; set; }
        //public DbSet<Tbl_STUDENT_SUBJECT_RA> Tbl_STUDENT_SUBJECT_RA { get; set; }
        //public DbSet<Tbl_Student_Tmp> Tbl_Student_Tmp { get; set; }
        public DbSet<Tbl_STUDENT_YR_CHNG> Tbl_STUDENT_YR_CHNG { get; set; }
        //public DbSet<Tbl_SUB_CASTE_MSTR> Tbl_SUB_CASTE_MSTR { get; set; }
        public DbSet<Tbl_SUB_CATPAP_MSTR> Tbl_SUB_CATPAP_MSTR { get; set; }
        //public DbSet<Tbl_SUB_CATPAPSEC_MSTR> Tbl_SUB_CATPAPSEC_MSTR { get; set; }
        //public DbSet<Tbl_SUB_EXAM_PATTERN> Tbl_SUB_EXAM_PATTERN { get; set; }
        public DbSet<Tbl_SUBJECT_CAT_MSTR> Tbl_SUBJECT_CAT_MSTR { get; set; }
        //public DbSet<TBL_SUBJECT_EXAMINER> TBL_SUBJECT_EXAMINER { get; set; }
        public DbSet<Tbl_SUBJECT_MSTR> Tbl_SUBJECT_MSTR { get; set; }
        //public DbSet<Tbl_SYS_ERROR_MSTR> Tbl_SYS_ERROR_MSTR { get; set; }
        //public DbSet<Tbl_SYS_GRP_MSTR> Tbl_SYS_GRP_MSTR { get; set; }
        public DbSet<Tbl_SYS_INST> Tbl_SYS_INST { get; set; }
        //public DbSet<Tbl_SYS_MNU_TYP> Tbl_SYS_MNU_TYP { get; set; }
        //public DbSet<Tbl_SYS_MOD_MSTR> Tbl_SYS_MOD_MSTR { get; set; }
        //public DbSet<Tbl_SYS_MSTR> Tbl_SYS_MSTR { get; set; }
        //public DbSet<TBL_SYS_PANEL> TBL_SYS_PANEL { get; set; }
        //public DbSet<Tbl_SYS_REPORTS> Tbl_SYS_REPORTS { get; set; }
        //public DbSet<Tbl_SYS_SETTING> Tbl_SYS_SETTING { get; set; }
        //public DbSet<Tbl_SYS_USR_MSTR> Tbl_SYS_USR_MSTR { get; set; }
        //public DbSet<Tbl_TIMETABLE_PRACTICAL> Tbl_TIMETABLE_PRACTICAL { get; set; }
        //public DbSet<Tbl_TIMETABLE_PRACTICAL_COLLEGE> Tbl_TIMETABLE_PRACTICAL_COLLEGE { get; set; }
        //public DbSet<Tbl_TIMETABLE_TRX> Tbl_TIMETABLE_TRX { get; set; }
        //public DbSet<Tbl_ZONE_MSTR> Tbl_ZONE_MSTR { get; set; }
        //public DbSet<Hall_Ticket> Hall_Ticket { get; set; }
        //public DbSet<VIEW1> VIEW1 { get; set; }
        //public DbSet<VIEW5> VIEW5 { get; set; }
        //public DbSet<VW_Exam_Schedule> VW_Exam_Schedule { get; set; }
        //public DbSet<VW_Examiner_Mst> VW_Examiner_Mst { get; set; }
        //public DbSet<VW_ExaminerLetter> VW_ExaminerLetter { get; set; }
        //public DbSet<VW_HALLTICKET> VW_HALLTICKET { get; set; }
        //public DbSet<VW_Marks_Temp_ENTRYNO1> VW_Marks_Temp_ENTRYNO1 { get; set; }
        //public DbSet<VW_Marks_Temp_ENTRYNO2> VW_Marks_Temp_ENTRYNO2 { get; set; }
        //public DbSet<VW_SEATNO> VW_SEATNO { get; set; }
        //public DbSet<VWHallTicket> VWHallTickets { get; set; }
       // public DbSet<ConvocationFee> ConvocationFee { get; set; }
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            //modelBuilder.Configurations.Add(new dtpropertyMap());
            //modelBuilder.Configurations.Add(new HIST_Tbl_CODE_DECODEMap());
            //modelBuilder.Configurations.Add(new HIST_Tbl_GRADE_TMPMap());
            //modelBuilder.Configurations.Add(new HIST_Tbl_MARKS_TMPMap());
            //modelBuilder.Configurations.Add(new HIST_TBL_STUDENT_EXAMSMap());
            //modelBuilder.Configurations.Add(new HIST_TBL_STUDENT_GROUPMap());
            //modelBuilder.Configurations.Add(new HIST_Tbl_STUDENT_INFOMap());
            //modelBuilder.Configurations.Add(new HIST_Tbl_STUDENT_PAP_MARKSMap());
            //modelBuilder.Configurations.Add(new HIST_Tbl_STUDENT_SEC_MARKSMap());
            //modelBuilder.Configurations.Add(new HIST_Tbl_STUDENT_SUBJECTMap());
            modelBuilder.Configurations.Add(new Tbl_APP_TEMPMap());
            modelBuilder.Configurations.Add(new Tbl_APP_TEMP_SUBMap());
            //modelBuilder.Configurations.Add(new Tbl_BOS_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_BOS_SUBJECT_MSTRMap());
            modelBuilder.Configurations.Add(new Tbl_BRANCH_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_BRANCH_SUBJECT_MSTRMap());
            //modelBuilder.Configurations.Add(new TBL_CALCULATE_GPMap());
            //modelBuilder.Configurations.Add(new Tbl_CAP_MSTRMap());
            modelBuilder.Configurations.Add(new Tbl_CAP_SCHEDULE_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_CAP_TRXMap());
            //modelBuilder.Configurations.Add(new Tbl_CAST_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_CAT_EXAM_PATTERNMap());
            //modelBuilder.Configurations.Add(new Tbl_CENTER_COL_TRXMap());
            //modelBuilder.Configurations.Add(new TBL_CENTER_SEAT_NOSMap());
            //modelBuilder.Configurations.Add(new Tbl_CENTER_TRXMap());
            //modelBuilder.Configurations.Add(new TBL_CGPA_PERCENTAGEMap());
            modelBuilder.Configurations.Add(new Tbl_CLASS_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_CODE_DECODEMap());
            modelBuilder.Configurations.Add(new Tbl_COLLEGE_COURSE_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_COLLEGE_LAB_DTLMap());
            //modelBuilder.Configurations.Add(new Tbl_COLLEGE_LAB_MSTRMap());
            modelBuilder.Configurations.Add(new Tbl_COLLEGE_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_CONVO_MSTRMap());
            //modelBuilder.Configurations.Add(new TBL_COPART_GROUPSMap());
            //modelBuilder.Configurations.Add(new TBL_COPART_GRP_SYLLABUSMap());
            //modelBuilder.Configurations.Add(new Tbl_COPART_MAIN_GROUPSMap());
            //modelBuilder.Configurations.Add(new TBL_COPART_SUB_GROUPSMap());
            modelBuilder.Configurations.Add(new Tbl_COPART_SYLLABUSMap());
            modelBuilder.Configurations.Add(new Tbl_COPART_SYLLABUS_TRXMap());
            //modelBuilder.Configurations.Add(new TBL_COURSE_CAT_SEQMap());
            modelBuilder.Configurations.Add(new Tbl_COURSE_CLASS_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_COURSE_ELIGIBILITY_DTL_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_COURSE_ELIGIBILITY_DTL_MSTR1Map());
            //modelBuilder.Configurations.Add(new Tbl_COURSE_ELIGIBILITY_MSTRMap());
            //modelBuilder.Configurations.Add(new TBl_COURSE_LOCK_MSTRMap());
            modelBuilder.Configurations.Add(new Tbl_COURSE_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_COURSE_PART_INST_TRXMap());
            modelBuilder.Configurations.Add(new Tbl_COURSE_PART_MSTRMap());
            modelBuilder.Configurations.Add(new Tbl_COURSE_TYPE_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_DEBR_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_DEGREE_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_DIST_MSTRMap());
            modelBuilder.Configurations.Add(new TBL_DISTANCE_CENTERSMap());
            //modelBuilder.Configurations.Add(new Tbl_DIVISION_MSTRMap());
            //modelBuilder.Configurations.Add(new TBL_ELIG_DOCMap());
            //modelBuilder.Configurations.Add(new TBL_ELIG_MSTMap());
            //modelBuilder.Configurations.Add(new Tbl_ENROLL_TEMPMap());
            //modelBuilder.Configurations.Add(new Tbl_ENROLL_TEMP_ADRMap());
            //modelBuilder.Configurations.Add(new Tbl_EVALCAT_MSTRMap());
            modelBuilder.Configurations.Add(new Tbl_EXAM_SCHEDULE_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_EXAM_SCHEDULE_TRXMap());
            //modelBuilder.Configurations.Add(new TBL_EXAM_TYPE_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_EXAMINER_DEG_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_EXAMINER_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_EXAMINER_PAIRMap());
            //modelBuilder.Configurations.Add(new Tbl_EXAMINER_PANNELMap());
            //modelBuilder.Configurations.Add(new Tbl_EXAMINER_SUB_SPECIALIZATIONMap());
            //modelBuilder.Configurations.Add(new Tbl_EXAMINER_SUB_SPECIALIZATION_TEMPMap());
            //modelBuilder.Configurations.Add(new Tbl_EXAMINER_SUB_TAUGHTMap());
            //modelBuilder.Configurations.Add(new Tbl_EXAMINER_SUB_TAUGHT_TEMPMap());
            //modelBuilder.Configurations.Add(new TBL_EXAMS_GROUPMap());
            //modelBuilder.Configurations.Add(new Tbl_EXM_PATT_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_EXMR_TYPE_MSTRMap());
            modelBuilder.Configurations.Add(new Tbl_FACULTY_MSTRMap());
            modelBuilder.Configurations.Add(new Tbl_FEE_DTLMap());
            modelBuilder.Configurations.Add(new Tbl_FEE_MSTRMap());
            modelBuilder.Configurations.Add(new Tbl_GRADE_MSTRMap());
            //modelBuilder.Configurations.Add(new TBL_GRADE_POINT_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_GRADE_TMPMap());
            //modelBuilder.Configurations.Add(new Tbl_INCOMEGRP_MSTRMap());
            //modelBuilder.Configurations.Add(new TBl_INST_APPOINTMENTMap());
            //modelBuilder.Configurations.Add(new Tbl_INVALID_ERRORMap());
            //modelBuilder.Configurations.Add(new Tbl_LAB_BATCHMap());
            //modelBuilder.Configurations.Add(new TBL_LPS_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_MARKS_TMPMap());
            //modelBuilder.Configurations.Add(new Tbl_MARKS_TMP_CAPMap());
            //modelBuilder.Configurations.Add(new Tbl_ORDINANCE_MSTRMap());
            //modelBuilder.Configurations.Add(new TBL_PAPER_DIV_MASTERMap());
            //modelBuilder.Configurations.Add(new TBL_PRE_REQUISITESMap());
            //modelBuilder.Configurations.Add(new Tbl_PREV_EXAM_DTLMap());
            //modelBuilder.Configurations.Add(new Tbl_QUALIFIED_EXAM_MSTRMap());
            modelBuilder.Configurations.Add(new Tbl_REG_TEMPMap());
            //modelBuilder.Configurations.Add(new TBL_REG_TEMP_COLLEGEMap());
            //modelBuilder.Configurations.Add(new Tbl_REG_TEMP_SUBMap());
            //modelBuilder.Configurations.Add(new TBL_SEAT_ALLOCATIONMap());
            //modelBuilder.Configurations.Add(new Tbl_STATE_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_STD_BATCHMap());
            //modelBuilder.Configurations.Add(new Tbl_STREAM_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_STUD_ACTIVITY_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_STUD_CATEGORY_MSTRMap());
            modelBuilder.Configurations.Add(new Tbl_STUDENT_CAT_MARKSMap());
            //modelBuilder.Configurations.Add(new Tbl_STUDENT_CAT_MARKS_BPMap());
            //modelBuilder.Configurations.Add(new Tbl_STUDENT_CAT_MARKS_RAMap());
            modelBuilder.Configurations.Add(new TBL_STUDENT_CGPAMap());
            modelBuilder.Configurations.Add(new Tbl_STUDENT_CONVOMap());
            modelBuilder.Configurations.Add(new Tbl_STUDENT_COURSEMap());
            //modelBuilder.Configurations.Add(new Tbl_STUDENT_COURSE_PARTMap());
            //modelBuilder.Configurations.Add(new Tbl_STUDENT_DEBARMap());
            //modelBuilder.Configurations.Add(new Tbl_STUDENT_DEBARREDMap());
            modelBuilder.Configurations.Add(new TBl_STUDENT_ENV_STUDIESMap());
            modelBuilder.Configurations.Add(new TBL_STUDENT_EXAMSMap());
            //modelBuilder.Configurations.Add(new TBL_STUDENT_GROUPMap());
            modelBuilder.Configurations.Add(new Tbl_STUDENT_INFOMap());
            modelBuilder.Configurations.Add(new Tbl_STUDENT_INFO_ADRMap());
             modelBuilder.Configurations.Add(new TBL_STUDENT_REVALUATIONMap());
            //modelBuilder.Configurations.Add(new TBL_STUDENT_LPS_TRNMap());
            //modelBuilder.Configurations.Add(new Tbl_STUDENT_MIG_COURSEMap());
            modelBuilder.Configurations.Add(new Tbl_STUDENT_PAP_MARKSMap());
            //modelBuilder.Configurations.Add(new Tbl_STUDENT_PAP_MARKS_BPMap());
            //modelBuilder.Configurations.Add(new Tbl_STUDENT_PAP_MARKS_RAMap());
            //modelBuilder.Configurations.Add(new Tbl_STUDENT_PRV_STATUSMap());
            //modelBuilder.Configurations.Add(new Tbl_STUDENT_SEC_MARKSMap());
            //modelBuilder.Configurations.Add(new Tbl_STUDENT_SEC_MARKS_BPMap());
            //modelBuilder.Configurations.Add(new Tbl_STUDENT_SEC_MARKS_RAMap());
            modelBuilder.Configurations.Add(new Tbl_STUDENT_SUBJECTMap());
            //modelBuilder.Configurations.Add(new Tbl_STUDENT_SUBJECT_BPMap());
            //modelBuilder.Configurations.Add(new Tbl_STUDENT_SUBJECT_RAMap());
            //modelBuilder.Configurations.Add(new Tbl_Student_TmpMap());
            modelBuilder.Configurations.Add(new Tbl_STUDENT_YR_CHNGMap());
            //modelBuilder.Configurations.Add(new Tbl_SUB_CASTE_MSTRMap());
            modelBuilder.Configurations.Add(new Tbl_SUB_CATPAP_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_SUB_CATPAPSEC_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_SUB_EXAM_PATTERNMap());
            modelBuilder.Configurations.Add(new Tbl_SUBJECT_CAT_MSTRMap());
            //modelBuilder.Configurations.Add(new TBL_SUBJECT_EXAMINERMap());
            modelBuilder.Configurations.Add(new Tbl_SUBJECT_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_SYS_ERROR_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_SYS_GRP_MSTRMap());
            modelBuilder.Configurations.Add(new Tbl_SYS_INSTMap());
            //modelBuilder.Configurations.Add(new Tbl_SYS_MNU_TYPMap());
            //modelBuilder.Configurations.Add(new Tbl_SYS_MOD_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_SYS_MSTRMap());
            //modelBuilder.Configurations.Add(new TBL_SYS_PANELMap());
            //modelBuilder.Configurations.Add(new Tbl_SYS_REPORTSMap());
            //modelBuilder.Configurations.Add(new Tbl_SYS_SETTINGMap());
            //modelBuilder.Configurations.Add(new Tbl_SYS_USR_MSTRMap());
            //modelBuilder.Configurations.Add(new Tbl_TIMETABLE_PRACTICALMap());
            //modelBuilder.Configurations.Add(new Tbl_TIMETABLE_PRACTICAL_COLLEGEMap());
            //modelBuilder.Configurations.Add(new Tbl_TIMETABLE_TRXMap());
            //modelBuilder.Configurations.Add(new Tbl_ZONE_MSTRMap());
            //modelBuilder.Configurations.Add(new Hall_TicketMap());
            //modelBuilder.Configurations.Add(new VIEW1Map());
            //modelBuilder.Configurations.Add(new VIEW5Map());
            //modelBuilder.Configurations.Add(new VW_Exam_ScheduleMap());
            //modelBuilder.Configurations.Add(new VW_Examiner_MstMap());
            //modelBuilder.Configurations.Add(new VW_ExaminerLetterMap());
            //modelBuilder.Configurations.Add(new VW_HALLTICKETMap());
            //modelBuilder.Configurations.Add(new VW_Marks_Temp_ENTRYNO1Map());
            //modelBuilder.Configurations.Add(new VW_Marks_Temp_ENTRYNO2Map());
            //modelBuilder.Configurations.Add(new VW_SEATNOMap());
            //modelBuilder.Configurations.Add(new VWHallTicketMap());
            //modelBuilder.Configurations.Add(new ConvocationFeeMap());
        }
    }
}
