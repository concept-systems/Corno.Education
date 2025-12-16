using Corno.Data.Admin;
using System.Data.Entity;
using Corno.Data.Common;
using Corno.Data.Core;
using Corno.Data.Core.Mapping;
using Corno.Data.Corno;
using Corno.Data.Corno.Mapping;
using Corno.Data.Corno.Masters;
using Corno.Data.Corno.Online_Education;
using Corno.Data.Corno.Paper_Setting.Models;
using Corno.Data.Corno.Question_Bank;
using Corno.Data.Corno.Question_Bank.Models;
using Corno.Data.Payment;

namespace Corno.Data.Contexts;

public class CornoContext : DbContext
{
    #region -- Constructors --
    public CornoContext(string connectionString)
        : base(connectionString)
    {
    }

    public CornoContext()
    {
    }
    #endregion

    #region -- Data Members --

    /*public DbSet<AspNetRole> AspNetRoles { get; set; }
    public DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
    public DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }*/
    //public DbSet<AspNetUser> AspNetUsers { get; set; }
    public DbSet<EnvironmentStudy> EnvironmentStudies { get; set; }

    //public DbSet<Registration> Registrations { get; set; }
    public DbSet<Exam> Exams { get; set; }
    //public DbSet<Instance> Instances { get; set; }

    public DbSet<ConvocationFee> ConvocationFees { get; set; }
    public DbSet<Complaint> Complaints { get; set; }
    //public DbSet<Verification> Verifications { get; set; }
    public DbSet<Convocation> Convocations { get; set; }
    public DbSet<Revalution> Revalutions { get; set; }
    public DbSet<RevalutionSubject> RevalutionSubjects { get; set; }
    //public DbSet<ExamSchedule> ExamSchedules { get; set; }
    public DbSet<ExamSubject> ExamSubjects { get; set; }
    #endregion

    #region -- Mappings --
    protected override void OnModelCreating(DbModelBuilder modelBuilder)
    {
        /*modelBuilder.Configurations.Add(new AspNetRoleMap());
        modelBuilder.Configurations.Add(new AspNetUserClaimMap());
        modelBuilder.Configurations.Add(new AspNetUserLoginMap());*/
        //modelBuilder.Configurations.Add(new AspNetUserMap());

        modelBuilder.Entity<AspNetUser>().ToTable("AspNetUsers");
        modelBuilder.Entity<AspNetUserRole>().ToTable("AspNetUserRoles")
            .HasKey(e => new { e.UserId, e.RoleId });
        modelBuilder.Entity<AspNetUserRole>()
            .HasRequired(ur => ur.User)
            .WithMany(u => u.UserRoles)
            .HasForeignKey(ur => ur.UserId)
            .WillCascadeOnDelete(true);

        modelBuilder.Entity<AspNetUserClaim>().ToTable("AspNetUserClaims")
            .HasKey(t => t.Id);
        modelBuilder.Entity<AspNetUserLogin>().ToTable("AspNetUserLogins")
            .HasKey(t => new { t.LoginProvider, t.ProviderKey, t.UserId }); 
        modelBuilder.Entity<AspNetRole>().ToTable("AspNetRoles")
            .HasKey(t => t.Id);


        // Masters
        modelBuilder.Entity<Instance>().ToTable(nameof(Instance));
        modelBuilder.Entity<Faculty>().ToTable(nameof(Faculty));
        modelBuilder.Entity<College>().ToTable(nameof(College));
        modelBuilder.Entity<CollegeCourseDetail>().ToTable(nameof(CollegeCourseDetail));
        modelBuilder.Entity<Course>().ToTable(nameof(Course));
        modelBuilder.Entity<CourseCategoryDetail>().ToTable(nameof(CourseCategoryDetail));
        modelBuilder.Entity<CoursePart>().ToTable(nameof(CoursePart));
        modelBuilder.Entity<Branch>().ToTable(nameof(Branch));
        modelBuilder.Entity<BranchSubjectDetail>().ToTable(nameof(BranchSubjectDetail));
        modelBuilder.Entity<Category>().ToTable(nameof(Category));
        modelBuilder.Entity<Subject>().ToTable(nameof(Subject));
        modelBuilder.Entity<SubjectCategoryDetail>().ToTable(nameof(SubjectCategoryDetail));
        modelBuilder.Entity<SubjectChapterDetail>().ToTable(nameof(SubjectChapterDetail));
        modelBuilder.Entity<SubjectInstructionDetail>().ToTable(nameof(SubjectInstructionDetail));
        modelBuilder.Entity<SubjectSectionDetail>().ToTable(nameof(SubjectSectionDetail));
        modelBuilder.Entity<Staff>().ToTable(nameof(Staff));
        modelBuilder.Entity<StaffSubjectDetail>().ToTable(nameof(StaffSubjectDetail));
        modelBuilder.Entity<Student>().ToTable(nameof(Student));
        modelBuilder.Entity<StudentAddressDetail>().ToTable(nameof(StudentAddressDetail));
        modelBuilder.Entity<MiscMaster>().ToTable(nameof(MiscMaster));

        //modelBuilder.Configurations.Add(new RegistrationMap());
        modelBuilder.Configurations.Add(new EnvironmentStudyMap());
        modelBuilder.Configurations.Add(new ExamMap());
        //modelBuilder.Configurations.Add(new InstanceMap());
        modelBuilder.Configurations.Add(new ConvocationFeeMap());
        modelBuilder.Configurations.Add(new ComplaintMap());
        modelBuilder.Configurations.Add(new ConvocationMap());
        modelBuilder.Configurations.Add(new RevalutionMap());
        modelBuilder.Configurations.Add(new ExamSubjectMap());
        modelBuilder.Configurations.Add(new RevalutionSubjectMap());

        modelBuilder.Entity<Registration>().ToTable("Registration");
        //modelBuilder.Entity<TestRegistration>().ToTable("TestRegistration");

        modelBuilder.Entity<TimeTable>().ToTable("TimeTable");
        modelBuilder.Entity<TimeTableTheoryDetail>().ToTable("TimeTableTheoryDetail");
        modelBuilder.Entity<TimeTablePracticalDetail>().ToTable("TimeTablePracticalDetail");
        modelBuilder.Entity<TimeTableCoursePartDetail>().ToTable("TimeTableCoursePartDetail");
        modelBuilder.Entity<AnswerSheet>().ToTable("AnswerSheet");
        modelBuilder.Entity<AnswerSheetSubject>().ToTable("AnswerSheetSubject");

        // Enrollment
        modelBuilder.Entity<Link>().ToTable(nameof(Link));
        modelBuilder.Entity<LinkDetail>().ToTable(nameof(LinkDetail));

        modelBuilder.Entity<TransactionOtp>().ToTable("TransactionOtp");

        modelBuilder.Entity<EnvironmentSetting>().ToTable("EnvironmentSetting");

        // Paper Setting
        modelBuilder.Entity<Appointment>().ToTable(nameof(Appointment));
        modelBuilder.Entity<AppointmentDetail>().ToTable(nameof(AppointmentDetail));
        modelBuilder.Entity<AppointmentBillDetail>().ToTable(nameof(AppointmentBillDetail));
        modelBuilder.Entity<Schedule>().ToTable(nameof(Schedule));
        modelBuilder.Entity<ScheduleDetail>().ToTable(nameof(ScheduleDetail));
        modelBuilder.Entity<Remuneration>().ToTable(nameof(Remuneration));
        modelBuilder.Entity<RemunerationDetail>().ToTable(nameof(RemunerationDetail));

        // Question Bank
        modelBuilder.Entity<Structure>().ToTable(nameof(Structure));
        modelBuilder.Entity<StructureDetail>().ToTable(nameof(StructureDetail));
        modelBuilder.Entity<Question>().ToTable(nameof(Question));
        modelBuilder.Entity<QuestionAppointment>()
            .ToTable(nameof(QuestionAppointment))
            .HasMany(q => q.QuestionAppointmentDetails);
        modelBuilder.Entity<QuestionAppointmentDetail>()
            .ToTable(nameof(QuestionAppointmentDetail));
        modelBuilder.Entity<QuestionAppointmentTypeDetail>()
            .ToTable(nameof(QuestionAppointmentTypeDetail));

        modelBuilder.Entity<Paper>().ToTable(nameof(Paper));
        modelBuilder.Entity<PaperDetail>().ToTable(nameof(PaperDetail));

        // Payment Gateway
        modelBuilder.Entity<Payout>().ToTable(nameof(Payout));

        // Online Education
        modelBuilder.Entity<OnlineStudent>().ToTable(nameof(OnlineStudent));
        modelBuilder.Entity<College45OptionalSubject>().ToTable(nameof(College45OptionalSubject));
    }
    #endregion
}