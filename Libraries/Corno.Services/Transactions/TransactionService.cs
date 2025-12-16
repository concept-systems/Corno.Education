using Corno.Models;

namespace Corno.DAL.Classes
{
    public class TransactionService : BaseService, ITransactionService
    {
        private IGenericRepository<Student> _studentRepository;
        public IGenericRepository<Student> StudentRepository
        {
            get
            {
                if (this._studentRepository == null)
                    this._studentRepository = new GenericRepository<Student>(_unitOfWork);
                return _studentRepository;
            }
        }

        private IGenericRepository<EnvironmentStudy> _environmentStudyRepository;
        public IGenericRepository<EnvironmentStudy> EnvironmentStudyRepository
        {
            get
            {
                if (this._environmentStudyRepository == null)
                    this._environmentStudyRepository = new GenericRepository<EnvironmentStudy>(_unitOfWork);
                return _environmentStudyRepository;
            }
        }

       
        private IGenericRepository<Exam> _studentexamRepository;
        public IGenericRepository<Exam> ExamRepository
        {
            get
            {
                if (this._studentexamRepository == null)
                    this._studentexamRepository = new GenericRepository<Exam>(_unitOfWork);
                return _studentexamRepository;
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
        private IGenericRepository<ExamFee> _examFeeRepository;
        public IGenericRepository<ExamFee> ExamFeeRepository
        {
            get
            {
                if (this._examFeeRepository == null)
                    this._examFeeRepository = new GenericRepository<ExamFee>(_unitOfWork);
                return _examFeeRepository;
            }
        }

     
        private IGenericRepository<Convocation> _ConvocationRepository;
        public IGenericRepository<Convocation> ConvocationRepository
        {
            get
            {
                if (this._ConvocationRepository == null)
                {
                    this._ConvocationRepository = new GenericRepository<Convocation>(_unitOfWork);
                }
                return _ConvocationRepository;
            }
        }
        private IGenericRepository<Revalution> _RevalutionRepository;
        public IGenericRepository<Revalution> RevalutionRepository
        {
            get
            {
                if (this._RevalutionRepository == null)
                {
                    this._RevalutionRepository = new GenericRepository<Revalution>(_unitOfWork);
                }
                return _RevalutionRepository;
            }
        }
        private IGenericRepository<RevalutionSubject> _RevalutionSubjectRepository;
        public IGenericRepository<RevalutionSubject> RevalutionSubjectRepository
        {
            get
            {
                if (this._RevalutionSubjectRepository == null)
                {
                    this._RevalutionSubjectRepository = new GenericRepository<RevalutionSubject>(_unitOfWork);
                }
                return _RevalutionSubjectRepository;
            }
        }

        private IGenericRepository<ExamSubject> _examSubjectRepository;
        public IGenericRepository<ExamSubject> ExamSubjectRepository
        {
            get
            {
                if (this._examSubjectRepository == null)
                {
                    this._examSubjectRepository = new GenericRepository<ExamSubject>(_unitOfWork);
                }
                return _examSubjectRepository;
            }
        }
        private IGenericRepository<ConvocationFee> _convocationFeeRepository;
        public IGenericRepository<ConvocationFee> ConvocationFeeRepository
        {
            get
            {
                if (this._convocationFeeRepository == null)
                    this._convocationFeeRepository = new GenericRepository<ConvocationFee>(_unitOfWork);
                return _convocationFeeRepository;
            }
        }
       
        public TransactionService(IUnitOfWork unitOfWork,
                        IGenericRepository<Student> leadRepository)
        {
            this._unitOfWork = unitOfWork;
            this._studentRepository = leadRepository;
        }
    }
}