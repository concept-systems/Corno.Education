using System;
using System.Data.Entity;
using Corno.Data.Core;
using Corno.Data.Core.Mapping;
using Corno.Data.Payment;

namespace Corno.Data.Contexts;

public class CoreContext : DbContext
{
    #region -- Constructors --
    static CoreContext()
    {
        Database.SetInitializer<CoreContext>(null);
    }

    public CoreContext()
        : base("Name=CoreContext")
    {
    }
    #endregion

    #region -- Properties --
    public DbSet<Tbl_APP_TEMP> Tbl_APP_TEMP { get; set; }

    public DbSet<Tbl_APP_TEMP_SUB> Tbl_APP_TEMP_SUB { get; set; }
    public DbSet<Tbl_BRANCH_MSTR> Tbl_BRANCH_MSTR { get; set; }
    public DbSet<Tbl_CAP_SCHEDULE_MSTR> Tbl_CAP_SCHEDULE_MSTR { get; set; }
    public DbSet<Tbl_CLASS_MSTR> Tbl_CLASS_MSTR { get; set; }
    public DbSet<Tbl_COLLEGE_COURSE_MSTR> Tbl_COLLEGE_COURSE_MSTR { get; set; }
    public DbSet<Tbl_COLLEGE_MSTR> Tbl_COLLEGE_MSTR { get; set; }
    public DbSet<Tbl_COPART_SYLLABUS> Tbl_COPART_SYLLABUS { get; set; }
    public DbSet<Tbl_COPART_SYLLABUS_TRX> Tbl_COPART_SYLLABUS_TRX { get; set; }
    public DbSet<Tbl_COURSE_CLASS_MSTR> Tbl_COURSE_CLASS_MSTR { get; set; }
    public DbSet<Tbl_COURSE_MSTR> Tbl_COURSE_MSTR { get; set; }
    public DbSet<Tbl_COURSE_PART_MSTR> Tbl_COURSE_PART_MSTR { get; set; }
    public DbSet<Tbl_COURSE_TYPE_MSTR> Tbl_COURSE_TYPE_MSTR { get; set; }
    public DbSet<TBL_DISTANCE_CENTERS> TBL_DISTANCE_CENTERS { get; set; }
    //public DbSet<Tbl_EXAM_SCHEDULE_MSTR> Tbl_EXAM_SCHEDULE_MSTR { get; set; }
    public DbSet<Tbl_FACULTY_MSTR> Tbl_FACULTY_MSTR { get; set; }
    public DbSet<Tbl_FEE_DTL> Tbl_FEE_DTL { get; set; }
    public DbSet<Tbl_FEE_MSTR> Tbl_FEE_MSTR { get; set; }
    public DbSet<Tbl_GRADE_MSTR> Tbl_GRADE_MSTR { get; set; }
    //public DbSet<TBL_STUDENT_REVALUATION> TBL_STUDENT_REVALUATION { get; set; }
    //public DbSet<TBL_STUDENT_REVAL_CHILD> TBL_STUDENT_REVAL_CHILD { get; set; }
    public DbSet<Tbl_REG_TEMP> Tbl_REG_TEMP { get; set; }
    public DbSet<Tbl_STUDENT_CAT_MARKS> Tbl_STUDENT_CAT_MARKS { get; set; }
    public DbSet<TBL_STUDENT_CGPA> TBL_STUDENT_CGPA { get; set; }
    public DbSet<Tbl_CONVO_MSTR> Tbl_CONVO_MSTR { get; set; }
    public DbSet<Tbl_STUDENT_CONVO> Tbl_STUDENT_CONVO { get; set; }
    public DbSet<Tbl_STUDENT_COURSE> Tbl_STUDENT_COURSE { get; set; }
    public DbSet<TBl_STUDENT_ENV_STUDIES> TBl_STUDENT_ENV_STUDIES { get; set; }
    public DbSet<TBL_STUDENT_EXAMS> TBL_STUDENT_EXAMS { get; set; }
    public DbSet<Tbl_STUDENT_INFO> Tbl_STUDENT_INFO { get; set; }
    //public DbSet<Tbl_STUDENT_INFO_ADR> Tbl_STUDENT_INFO_ADR { get; set; }
    public DbSet<Tbl_STUDENT_PAP_MARKS> Tbl_STUDENT_PAP_MARKS { get; set; }
    public DbSet<Tbl_STUDENT_SUBJECT> Tbl_STUDENT_SUBJECT { get; set; }
    public DbSet<Tbl_STUDENT_YR_CHNG> Tbl_STUDENT_YR_CHNG { get; set; }
    public DbSet<Tbl_SUB_CATPAP_MSTR> Tbl_SUB_CATPAP_MSTR { get; set; }
    public DbSet<Tbl_SUBJECT_CAT_MSTR> Tbl_SUBJECT_CAT_MSTR { get; set; }
    public DbSet<Tbl_SUBJECT_MSTR> Tbl_SUBJECT_MSTR { get; set; }
    public DbSet<Tbl_SYS_INST> Tbl_SYS_INST { get; set; }
    public DbSet<Tbl_MARKS_TMP> Tbl_MARKS_TMP { get; set; }
    #endregion

    #region -- Event Handlers --
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        modelBuilder.Configurations.Add(new Tbl_APP_TEMPMap());
        modelBuilder.Configurations.Add(new Tbl_APP_TEMP_SUBMap());
        modelBuilder.Configurations.Add(new Tbl_BRANCH_MSTRMap());
        modelBuilder.Configurations.Add(new Tbl_CAP_SCHEDULE_MSTRMap());
        modelBuilder.Configurations.Add(new Tbl_CLASS_MSTRMap());
        modelBuilder.Configurations.Add(new Tbl_COLLEGE_COURSE_MSTRMap());
        modelBuilder.Configurations.Add(new Tbl_COLLEGE_MSTRMap());
        modelBuilder.Configurations.Add(new Tbl_COPART_SYLLABUSMap());
        modelBuilder.Configurations.Add(new Tbl_COPART_SYLLABUS_TRXMap());
        modelBuilder.Configurations.Add(new Tbl_COURSE_CLASS_MSTRMap());
        modelBuilder.Configurations.Add(new Tbl_COURSE_MSTRMap());
        modelBuilder.Configurations.Add(new Tbl_COURSE_PART_MSTRMap());
        modelBuilder.Configurations.Add(new Tbl_COURSE_TYPE_MSTRMap());
        modelBuilder.Configurations.Add(new TBL_DISTANCE_CENTERSMap());
        //modelBuilder.Configurations.Add(new Tbl_EXAM_SCHEDULE_MSTRMap());
        modelBuilder.Configurations.Add(new Tbl_FACULTY_MSTRMap());
        modelBuilder.Configurations.Add(new Tbl_FEE_DTLMap());
        modelBuilder.Configurations.Add(new Tbl_FEE_MSTRMap());
        modelBuilder.Configurations.Add(new Tbl_GRADE_MSTRMap());
        modelBuilder.Configurations.Add(new Tbl_REG_TEMPMap());
        modelBuilder.Configurations.Add(new Tbl_STUDENT_CAT_MARKSMap());
        modelBuilder.Configurations.Add(new TBL_STUDENT_CGPAMap());
        modelBuilder.Configurations.Add(new Tbl_STUDENT_CONVOMap());
        modelBuilder.Configurations.Add(new Tbl_STUDENT_COURSEMap());
        modelBuilder.Configurations.Add(new TBl_STUDENT_ENV_STUDIESMap());
        modelBuilder.Configurations.Add(new TBL_STUDENT_EXAMSMap());
        modelBuilder.Configurations.Add(new Tbl_STUDENT_INFOMap());
        //modelBuilder.Configurations.Add(new Tbl_STUDENT_INFO_ADRMap());
        //modelBuilder.Configurations.Add(new TBL_STUDENT_REVALUATIONMap());
        //modelBuilder.Configurations.Add(new TBL_STUDENT_REVAL_CHILDMap());
        modelBuilder.Configurations.Add(new Tbl_STUDENT_PAP_MARKSMap());
        modelBuilder.Configurations.Add(new Tbl_STUDENT_SUBJECTMap());
        modelBuilder.Configurations.Add(new Tbl_STUDENT_YR_CHNGMap());
        modelBuilder.Configurations.Add(new Tbl_SUB_CATPAP_MSTRMap());
        modelBuilder.Configurations.Add(new Tbl_SUBJECT_CAT_MSTRMap());
        modelBuilder.Configurations.Add(new Tbl_SUBJECT_MSTRMap());
        modelBuilder.Configurations.Add(new Tbl_SYS_INSTMap());

        modelBuilder.Entity<Tbl_EXAM_SCHEDULE_MSTR>().ToTable(nameof(Tbl_EXAM_SCHEDULE_MSTR));
        modelBuilder.Entity<Tbl_EXAM_SCHEDULE_COURSE>().ToTable(nameof(Tbl_EXAM_SCHEDULE_COURSE));

        modelBuilder.Entity<Tbl_STUDENT_INFO_ADR>().ToTable(nameof(Tbl_STUDENT_INFO_ADR));

        modelBuilder.Entity<Tbl_EVALCAT_MSTR>().ToTable(nameof(Tbl_EVALCAT_MSTR));
        modelBuilder.Entity<Tbl_TIMETABLE_TRX>().ToTable(nameof(Tbl_TIMETABLE_TRX))
            .HasKey(t => new
            {
                t.Num_FK_COPRT_NO,
                t.Num_FK_Course_No,
                t.Num_FK_PH_CD,
                t.Num_FK_CAT_CD,
                t.Num_FK_PP_CD,
                t.NUM_FK_SUB_DIV_CD,
                t.Num_FK_INST_NO,
                t.Dtm_DTE_CR,
            });
        modelBuilder.Entity<Tbl_TimeTableINST>().ToTable(nameof(Tbl_TimeTableINST))
            .HasKey(t => new
            {
                t.Chr_FreezeTimeTable,
                t.Num_FK_INST_NO,
                t.Num_PK_CO_CD
            });
        modelBuilder.Entity<Tbl_MARKS_TMP>().ToTable(nameof(Tbl_MARKS_TMP))
            .HasKey(t => new { t.Num_PK_RECORDID });

        modelBuilder.Entity<Tbl_DEGREE_MSTR>().ToTable(nameof(Tbl_DEGREE_MSTR))
            .HasKey(t => new { t.Num_PK_DEGREE_CD });

        // Revaluation
        modelBuilder.Entity<TBL_STUDENT_REVALUATION>().ToTable(nameof(TBL_STUDENT_REVALUATION))
            .HasKey(t => new
            {
                t.Num_FK_INST_NO,
                t.Num_FK_COPRT_NO,
                t.NUM_FK_SUB_CD,
                t.NUM_FK_PRN_NO,
            });
        modelBuilder.Entity<TBL_STUDENT_REVAL_CHILD>().ToTable(nameof(Core.TBL_STUDENT_REVAL_CHILD))
            .HasKey(t => new
            {
                t.Num_FK_INST_NO,
                t.Num_FK_COPRT_NO,
                t.NUM_FK_SUB_CD,
                t.NUM_FK_PRN_NO,
            });
        modelBuilder.Entity<Tbl_ADDITIONAL_CREDITS>().ToTable(nameof(Tbl_ADDITIONAL_CREDITS));


        // Convocation
        /*modelBuilder.Entity<Tbl_STUDENT_CONVO>().ToTable(nameof(Tbl_STUDENT_CONVO))
            .HasKey(t => new { t.NUM_PK_RECORD_ID, t.Num_FK_RESULT_CD });*/
        modelBuilder.Entity<Tbl_CONVO_MSTR>().ToTable(nameof(Tbl_CONVO_MSTR));

        // Payment Gateway
        modelBuilder.Entity<Payout>().ToTable(nameof(Payout));
    }
    #endregion

    #region -- Public Methods --
    /*public void CheckDatabaseConnection()
    {
        try
        {
            if (Database.Connection != null &&
                Database.Connection.State == System.Data.C.Open)
                return;
            Database?.Connection.Close();
            Database?.Connection.Open();
        }
        catch
        {
            throw new Exception(
                "Failed to establish a connection with the database. Please verify that the database server is online");
        }
    }*/
    #endregion
}