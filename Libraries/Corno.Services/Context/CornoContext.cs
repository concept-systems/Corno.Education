using System.Data.Entity;
using System.Data.Entity.Validation;
using OnlineExam.Models.Mapping;

namespace OnlineExam.Models
{
    public partial class CornoContext : DbContext
    {
        #region -- Constructors --
        public CornoContext(string _connectionString)
            : base(_connectionString)
        {
        }

        public CornoContext()
            : base()
        {
        }
        #endregion

        #region -- Data Members --

        public DbSet<AspNetRole> AspNetRoles { get; set; }
        public DbSet<AspNetUserClaim> AspNetUserClaims { get; set; }
        public DbSet<AspNetUserLogin> AspNetUserLogins { get; set; }
        public DbSet<AspNetUser> AspNetUsers { get; set; }
        //public DbSet<Branch> Branches { get; set; }
        //public DbSet<Bos> Boses { get; set; }
        //public DbSet<College> Colleges { get; set; }
        //public DbSet<Course> Courses { get; set; }
        //public DbSet<College_Course_Map> College_Course_Maps { get; set; }
        //public DbSet<CoursePart> CourseParts { get; set; }
        //public DbSet<CourseType> CourseTypes { get; set; }
        public DbSet<Degree> Degrees { get; set; }
        public DbSet<Department> Departments { get; set; }
        public DbSet<Designation> Designations { get; set; }
        //public DbSet<Faculty> Faculties { get; set; }
        public DbSet<Student> Students { get; set; }
        public DbSet<EnvironmentStudy> EnvironmentStudies { get; set; }
        public DbSet<Exam> Exams { get; set; }
      //  public DbSet<Student> Students { get; set; }
        //public DbSet<Subject> Subjects { get; set; }
        //public DbSet<SubjectCategory> SubjectCategorys { get; set; }
        public DbSet<Instance> Instances { get; set; }
        //public DbSet<Staff> Staffs { get; set; }
        //public DbSet<CourseCategoryMap> CourseCategoryMaps { get; set; }
        //public DbSet<BOSStaff> BoSStaffs { get; set; }
        public DbSet<City> Citys { get; set; }
        public DbSet<Fee> Fees { get; set; }
        public DbSet<District> Districts { get; set; }
        public DbSet<State> States { get; set; }
        public DbSet<Country> Countrys { get; set; }
        public DbSet<Tehsil> Tehsils { get; set; }
        //public DbSet<Centre> Centres { get; set; }
        public DbSet<ExamFee> ExamFees { get; set; }
        public DbSet<ConvocationFee> ConvocationFees { get; set; }
        public DbSet<CollegeFee> CollegeFees { get; set; }
        public DbSet<College_Fee_Map> College_Fee_Maps { get; set; }
        public DbSet<Complaint> Complaints { get; set; }
        public DbSet<Verification> Verifications { get; set; }
        public DbSet<Convocation> Convocations { get; set; }
        public DbSet<Revalution> Revalutions { get; set; }
        public DbSet<RevalutionSubject> RevalutionSubjects { get; set; }
        public DbSet<ExamSchedule> ExamSchedules { get; set; }
        public DbSet<ExamSubject> ExamSubjects { get; set; }
        #endregion

        #region -- Mappings --
        protected override void OnModelCreating(DbModelBuilder modelBuilder)
        {
            modelBuilder.Configurations.Add(new AspNetRoleMap());
            modelBuilder.Configurations.Add(new AspNetUserClaimMap());
            modelBuilder.Configurations.Add(new AspNetUserLoginMap());
            modelBuilder.Configurations.Add(new AspNetUserMap());
            //modelBuilder.Configurations.Add(new BranchMap());
            //modelBuilder.Configurations.Add(new BosMap());
            //modelBuilder.Configurations.Add(new CollegeMap());
            //modelBuilder.Configurations.Add(new CourseMap());
            //modelBuilder.Configurations.Add(new CoursePartMap());
            //modelBuilder.Configurations.Add(new CourseTypeMap());
            modelBuilder.Configurations.Add(new DegreeMap());
            modelBuilder.Configurations.Add(new DepartmentMap());
            modelBuilder.Configurations.Add(new DesignationMap());
            //modelBuilder.Configurations.Add(new FacultyMap());
            modelBuilder.Configurations.Add(new StudentMap());
            modelBuilder.Configurations.Add(new EnvironmentStudyMap());
            modelBuilder.Configurations.Add(new ExamMap());
         //   modelBuilder.Configurations.Add(new StudentMap());
            //modelBuilder.Configurations.Add(new SubjectMap());
            modelBuilder.Configurations.Add(new InstanceMap());
            //modelBuilder.Configurations.Add(new StaffMap());
            //modelBuilder.Configurations.Add(new CourseCategoryMapMap());
            //modelBuilder.Configurations.Add(new BOSStaffMap());
            //modelBuilder.Configurations.Add(new SubjectCategoryMap());
            //modelBuilder.Configurations.Add(new CentreMap());
            modelBuilder.Configurations.Add(new CityMap());
            modelBuilder.Configurations.Add(new DistrictMap());
            modelBuilder.Configurations.Add(new CountryMap());
            modelBuilder.Configurations.Add(new TehsilMap());
            modelBuilder.Configurations.Add(new StateMap());
            modelBuilder.Configurations.Add(new FeeMap());
            modelBuilder.Configurations.Add(new ExamFeeMap());
            modelBuilder.Configurations.Add(new ConvocationFeeMap());
            modelBuilder.Configurations.Add(new CollegeFeeMap());
            modelBuilder.Configurations.Add(new College_Fee_MapMap());
            modelBuilder.Configurations.Add(new ComplaintMap());
            modelBuilder.Configurations.Add(new VerificationMap());
            //modelBuilder.Configurations.Add(new College_Course_MapMap());
            modelBuilder.Configurations.Add(new ConvocationMap());
            modelBuilder.Configurations.Add(new RevalutionMap());
            modelBuilder.Configurations.Add(new ExamScheduleMap());
            modelBuilder.Configurations.Add(new ExamSubjectMap());
            modelBuilder.Configurations.Add(new RevalutionSubjectMap());
        }
        #endregion
    }

}
