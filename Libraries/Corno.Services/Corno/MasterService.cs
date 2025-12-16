using Corno.Data.Corno;
using Corno.Data.Corno.Masters;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Corno;

public class MasterService : BaseService, IMasterService
{
    #region -- Constructors --
    public MasterService(IUnitOfWork unitOfWork)
    {
        UnitOfWork = unitOfWork;
        //_instanceRepository = instanceRepository;
    }
    #endregion

    #region -- Data Members --
    /*private IGenericRepository<Instance> _instanceRepository;
    private IGenericRepository<Faculty> _facultyRepository;
    private IGenericRepository<College> _collegeRepository;
    private IGenericRepository<CollegeCourseDetail> _collegeCourseDetailRepository;
    private IGenericRepository<Course> _courseRepository;
    private IGenericRepository<CourseCategoryDetail> _courseCategoryRepository;
    private IGenericRepository<Branch> _branchRepository;
    private IGenericRepository<BranchSubjectDetail> _branchSubjectRepository;
    private IGenericRepository<CoursePart> _coursePartRepository;
    private IGenericRepository<Category> _categoryRepository;
    private IGenericRepository<Subject> _subjectRepository;
    private IGenericRepository<SubjectChapterDetail> _subjectChapterDetailRepository;
    private IGenericRepository<SubjectCategoryDetail> _subjectCategoryDetailRepository;
    private IGenericRepository<SubjectInstructionDetail> _subjectInstructionDetailRepository;
    private IGenericRepository<Staff> _staffRepository;
    private IGenericRepository<StaffSubjectDetail> _staffSubjectDetailRepository;

    private IGenericRepository<Complaint> _complaintRepository;*/
    #endregion

    #region -- Properties --
    /*public IGenericRepository<Instance> InstanceRepository => _instanceRepository ??= new GenericRepository<Instance>(UnitOfWork);
    public IGenericRepository<Faculty> FacultyRepository => _facultyRepository ??= new GenericRepository<Faculty>(UnitOfWork);
    public IGenericRepository<College> CollegeRepository => _collegeRepository ??= new GenericRepository<College>(UnitOfWork, nameof(College.CollegeCourseDetails));
    public IGenericRepository<CollegeCourseDetail> CollegeCourseDetailRepository => _collegeCourseDetailRepository ??= new GenericRepository<CollegeCourseDetail>(UnitOfWork);
    public IGenericRepository<Course> CourseRepository => _courseRepository ??= new GenericRepository<Course>(UnitOfWork, nameof(Course.CourseCategoryDetails));
    public IGenericRepository<CourseCategoryDetail> CourseCategoryDetailRepository => _courseCategoryRepository ??= new GenericRepository<CourseCategoryDetail>(UnitOfWork);
    public IGenericRepository<Branch> BranchRepository => _branchRepository ??= new GenericRepository<Branch>(UnitOfWork, nameof(Branch.BranchSubjectDetails));
    public IGenericRepository<BranchSubjectDetail> BranchSubjectDetailRepository => _branchSubjectRepository ??= new GenericRepository<BranchSubjectDetail>(UnitOfWork);
    public IGenericRepository<CoursePart> CoursePartRepository => _coursePartRepository ??= new GenericRepository<CoursePart>(UnitOfWork);
    public IGenericRepository<Category> CategoryRepository => _categoryRepository ??= new GenericRepository<Category>(UnitOfWork);
    public IGenericRepository<Subject> SubjectRepository => _subjectRepository ??= new GenericRepository<Subject>(UnitOfWork, $"{nameof(Subject.SubjectCategoryDetails)},{nameof(Subject.SubjectChapterDetails)},{nameof(Subject.SubjectInstructionDetails)}");
    public IGenericRepository<SubjectChapterDetail> SubjectChapterDetailRepository => _subjectChapterDetailRepository ??= new GenericRepository<SubjectChapterDetail>(UnitOfWork);
    public IGenericRepository<SubjectCategoryDetail> SubjectCategoryDetailRepository => _subjectCategoryDetailRepository ??= new GenericRepository<SubjectCategoryDetail>(UnitOfWork);
    public IGenericRepository<SubjectInstructionDetail> SubjectInstructionDetailRepository => _subjectInstructionDetailRepository ??= new GenericRepository<SubjectInstructionDetail>(UnitOfWork);
    public IGenericRepository<Staff> StaffRepository => _staffRepository ??= new GenericRepository<Staff>(UnitOfWork, $"{nameof(Staff.StaffSubjectDetails)}");
    public IGenericRepository<StaffSubjectDetail> StaffSubjectDetailRepository => _staffSubjectDetailRepository ??= new GenericRepository<StaffSubjectDetail>(UnitOfWork);

    public IGenericRepository<Complaint> ComplaintRepository => _complaintRepository ??= new GenericRepository<Complaint>(UnitOfWork);*/
    #endregion
}