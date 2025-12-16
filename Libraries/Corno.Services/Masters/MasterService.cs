using OnlineExam.Models;

namespace OnlineExam.DAL.Classes
{
    public class MasterService : BaseService, IMasterService
    {
        //private IGenericRepository<Faculty> _facultyRepository;
        //public IGenericRepository<Faculty> FacultyRepository
        //{
        //    get
        //    {
        //        if (this._facultyRepository == null)
        //            this._facultyRepository = new GenericRepository<Faculty>(_unitOfWork);
        //        return _facultyRepository;
        //    }
        //}
        //private IGenericRepository<College> _collegeRepository;
        //public IGenericRepository<College> CollegeRepository
        //{
        //    get
        //    {
        //        if (this._collegeRepository == null)
        //            this._collegeRepository = new GenericRepository<College>(_unitOfWork);
        //        return _collegeRepository;
        //    }
        //}
        private IGenericRepository<CollegeFee> _collegeFeeRepository;
        public IGenericRepository<CollegeFee> CollegeFeeRepository
        {
            get
            {
                if (this._collegeFeeRepository == null)
                    this._collegeFeeRepository = new GenericRepository<CollegeFee>(_unitOfWork);
                return _collegeFeeRepository;
            }
        }
        //private IGenericRepository<CourseType> _courseTypeRepository;
        //public IGenericRepository<CourseType> CourseTypeRepository
        //{
        //    get
        //    {
        //        if (this._courseTypeRepository == null)
        //            this._courseTypeRepository = new GenericRepository<CourseType>(_unitOfWork);
        //        return _courseTypeRepository;
        //    }
        //}
        //private IGenericRepository<CoursePart> _coursePartRepository;
        //public IGenericRepository<CoursePart> CoursePartRepository
        //{
        //    get
        //    {
        //        if (this._coursePartRepository == null)
        //            this._coursePartRepository = new GenericRepository<CoursePart>(_unitOfWork);
        //        return _coursePartRepository;
        //    }
        //}
        //private IGenericRepository<CourseCategoryMap> _courseCategoryMapRepository;
        //public IGenericRepository<CourseCategoryMap> CourseCategoryMapRepository
        //{
        //    get
        //    {
        //        if (this._courseCategoryMapRepository == null)
        //            this._courseCategoryMapRepository = new GenericRepository<CourseCategoryMap>(_unitOfWork);
        //        return _courseCategoryMapRepository;
        //    }
        //}
        //private IGenericRepository<Branch> _branchRepository;
        //public IGenericRepository<Branch> BranchRepository
        //{
        //    get
        //    {
        //        if (this._branchRepository == null)
        //            this._branchRepository = new GenericRepository<Branch>(_unitOfWork);
        //        return _branchRepository;
        //    }
        //}
        //private IGenericRepository<Course> _courseRepository;
        //public IGenericRepository<Course> CourseRepository
        //{
        //    get
        //    {
        //        if (this._courseRepository == null)
        //            this._courseRepository = new GenericRepository<Course>(_unitOfWork);
        //        return _courseRepository;
        //    }
        //}
        //private IGenericRepository<College_Course_Map> _College_Course_MapRepository;
        //public IGenericRepository<College_Course_Map> College_Course_MapRepository
        //{
        //    get
        //    {
        //        if (this._College_Course_MapRepository == null)
        //            this._College_Course_MapRepository = new GenericRepository<College_Course_Map>(_unitOfWork);
        //        return _College_Course_MapRepository;
        //    }
        //}
        private IGenericRepository<Degree> _degreeRepository;
        public IGenericRepository<Degree> DegreeRepository
        {
            get
            {
                if (this._degreeRepository == null)
                    this._degreeRepository = new GenericRepository<Degree>(_unitOfWork);
                return _degreeRepository;
            }
        }
        private IGenericRepository<Designation> _designationRepository;
        public IGenericRepository<Designation> DesignationRepository
        {
            get
            {
                if (this._designationRepository == null)
                    this._designationRepository = new GenericRepository<Designation>(_unitOfWork);
                return _designationRepository;
            }
        }

        private IGenericRepository<Fee> _feeRepository;
        public IGenericRepository<Fee> FeeRepository
        {
            get
            {
                if (this._feeRepository == null)
                    this._feeRepository = new GenericRepository<Fee>(_unitOfWork);
                return _feeRepository;
            }
        }
        private IGenericRepository<ExamFee> _examfeeRepository;
        public IGenericRepository<ExamFee> ExamFeeRepository
        {
            get
            {
                if (this._examfeeRepository == null)
                    this._examfeeRepository = new GenericRepository<ExamFee>(_unitOfWork);
                return _examfeeRepository;
            }
        }
        //private IGenericRepository<ConvocationFee> _convocationFeeRepository;
        //public IGenericRepository<ConvocationFee> ConvocationFeeRepository
        //{
        //    get
        //    {
        //        if (this._convocationFeeRepository == null)
        //            this._convocationFeeRepository = new GenericRepository<ConvocationFee>(_unitOfWork);
        //        return _convocationFeeRepository;
        //    }
        //}

        //private IGenericRepository<Subject> _subjectRepository;
        //public IGenericRepository<Subject> SubjectRepository
        //{
        //    get
        //    {
        //        if (this._subjectRepository == null)
        //            this._subjectRepository = new GenericRepository<Subject>(_unitOfWork);
        //        return _subjectRepository;
        //    }
        //}
        //private IGenericRepository<SubjectCategory> _subjectCategoryRepository;
        //public IGenericRepository<SubjectCategory> SubjectCategoryRepository
        //{
        //    get
        //    {
        //        if (this._subjectCategoryRepository == null)
        //        {
        //            this._subjectCategoryRepository = new GenericRepository<SubjectCategory>(_unitOfWork);
        //        }
        //        return _subjectCategoryRepository;
        //    }
        //}

        private IGenericRepository<Instance> _instanceRepository;
        public IGenericRepository<Instance> InstanceRepository
        {
            get
            {
                if (this._instanceRepository == null)
                    this._instanceRepository = new GenericRepository<Instance>(_unitOfWork);
                return _instanceRepository;
            }
        }
        //private IGenericRepository<Bos> _bosRepository;
        //public IGenericRepository<Bos> BosRepository
        //{
        //    get
        //    {
        //        if (this._bosRepository == null)
        //            this._bosRepository = new GenericRepository<Bos>(_unitOfWork);
        //        return _bosRepository;
        //    }
        //}
        //private IGenericRepository<Centre> _centreRepository;
        //public IGenericRepository<Centre> CentreRepository
        //{
        //    get
        //    {
        //        if (this._centreRepository == null)
        //            this._centreRepository = new GenericRepository<Centre>(_unitOfWork);
        //        return _centreRepository;
        //    }
        //}
        //private IGenericRepository<BOSStaff> _bosStaffRepository;
        //public IGenericRepository<BOSStaff> BOSStaffRepository
        //{
        //    get
        //    {
        //        if (this._bosStaffRepository == null)
        //            this._bosStaffRepository = new GenericRepository<BOSStaff>(_unitOfWork);
        //        return _bosStaffRepository;
        //    }
        //}
        private IGenericRepository<Department> _departmentRepository;
        public IGenericRepository<Department> DepartmentRepository
        {
            get
            {
                if (this._departmentRepository == null)
                    this._departmentRepository = new GenericRepository<Department>(_unitOfWork);
                return _departmentRepository;
            }
        }
        //private IGenericRepository<Staff> _staffRepository;
        //public IGenericRepository<Staff> StaffRepository
        //{
        //    get
        //    {
        //        if (this._staffRepository == null)
        //            this._staffRepository = new GenericRepository<Staff>(_unitOfWork);
        //        return _staffRepository;
        //    }
        //}

        //City State
        private IGenericRepository<City> _cityRepository;
        public IGenericRepository<City> CityRepository
        {
            get
            {
                if (this._cityRepository == null)
                {
                    this._cityRepository = new GenericRepository<City>(_unitOfWork);
                }
                return _cityRepository;
            }
        }
        private IGenericRepository<State> _stateRepository;
        public IGenericRepository<State> StateRepository
        {
            get
            {
                if (this._stateRepository == null)
                {
                    this._stateRepository = new GenericRepository<State>(_unitOfWork);
                }
                return _stateRepository;
            }
        }
        private IGenericRepository<District> _districtRepository;
        public IGenericRepository<District> DistrictRepository
        {
            get
            {
                if (this._districtRepository == null)
                {
                    this._districtRepository = new GenericRepository<District>(_unitOfWork);
                }
                return _districtRepository;
            }
        }
        private IGenericRepository<Country> _countryRepository;
        public IGenericRepository<Country> CountryRepository
        {
            get
            {
                if (this._countryRepository == null)
                {
                    this._countryRepository = new GenericRepository<Country>(_unitOfWork);
                }
                return _countryRepository;
            }
        }
        private IGenericRepository<Complaint> _ComplaintRepository;
        public IGenericRepository<Complaint> ComplaintRepository
        {
            get
            {
                if (this._ComplaintRepository == null)
                {
                    this._ComplaintRepository = new GenericRepository<Complaint>(_unitOfWork);
                }
                return _ComplaintRepository;
            }
        }
        private IGenericRepository<Tehsil> _tehsilRepository;
        public IGenericRepository<Tehsil> TehsilRepository
        {
            get
            {
                if (this._tehsilRepository == null)
                {
                    this._tehsilRepository = new GenericRepository<Tehsil>(_unitOfWork);
                }
                return _tehsilRepository;
            }
        }
        private IGenericRepository<Verification> _VerificationRepository;
        public IGenericRepository<Verification> VerificationRepository
        {
            get
            {
                if (this._VerificationRepository == null)
                {
                    this._VerificationRepository = new GenericRepository<Verification>(_unitOfWork);
                }
                return _VerificationRepository;
            }
        }
        private IGenericRepository<College_Fee_Map> _college_Fee_MapRepository;
        public IGenericRepository<College_Fee_Map> College_Fee_MapRepository
        {
            get
            {
                if (this._college_Fee_MapRepository == null)
                {
                    this._college_Fee_MapRepository = new GenericRepository<College_Fee_Map>(_unitOfWork);
                }
                return _college_Fee_MapRepository;
            }
        }
        private IGenericRepository<ExamSchedule> _examScheduleRepository;
        public IGenericRepository<ExamSchedule> ExamScheduleRepository
        {
            get
            {
                if (this._examScheduleRepository == null)
                {
                    this._examScheduleRepository = new GenericRepository<ExamSchedule>(_unitOfWork);
                }
                return _examScheduleRepository;
            }
        }
       
        public MasterService(IUnitOfWork unitOfWork,
                        IGenericRepository<Instance> instanceRepository)
        {
            this._unitOfWork = unitOfWork;
            this._instanceRepository = instanceRepository;
        }
    }
}