using AutoMapper;
using Corno.Data.Core;
using Corno.Data.Corno;
using Corno.Data.ViewModels;

namespace Corno.OnlineExam;

public static class AutoMapperConfig
{
    #region -- Data Members --

    public static Mapper CornoMapper { get; set; }
    #endregion

    public static void RegisterMappings()
    {
        var config = new MapperConfiguration(cfg =>
        {
            cfg.CreateMap<Registration, RegistrationViewModel>();
            cfg.CreateMap<RegistrationViewModel, Registration>();

            //cfg.CreateMap<EnvironmentStudy, EnvironmentStudyViewModel>();
            //cfg.CreateMap<EnvironmentStudyViewModel, EnvironmentStudy>();

            cfg.CreateMap<Exam, ExamViewModel>();
            cfg.CreateMap<ExamViewModel, Exam>();

            cfg.CreateMap<ExamSubject, ExamSubjectViewModel>();
            cfg.CreateMap<ExamSubjectViewModel, ExamSubject>();

            //cfg.CreateMap<Fee, FeeViewModel>();
            //cfg.CreateMap<FeeViewModel, Fee>();

            //cfg.CreateMap<ExamFee, ExamFeeViewModel>();
            //cfg.CreateMap<ExamFeeViewModel, ExamFee>();

            cfg.CreateMap<ConvocationFee, ConvocationFeeViewModel>();
            cfg.CreateMap<ConvocationFeeViewModel, ConvocationFee>();

            //cfg.CreateMap<Degree, DegreeViewModel>();
            //cfg.CreateMap<DegreeViewModel, Degree>();

            //cfg.CreateMap<Designation, DesignationViewModel>();
            //cfg.CreateMap<DesignationViewModel, Designation>();

            //cfg.CreateMap<Department, DepartmentViewModel>();
            //cfg.CreateMap<DepartmentViewModel, Department>();

            //cfg.CreateMap<Instance, InstanceViewModel>();
            //cfg.CreateMap<InstanceViewModel, Instance>();

            //cfg.CreateMap<City, CityViewModel>();
            //cfg.CreateMap<CityViewModel, City>();

            //cfg.CreateMap<Country, CountryViewModel>();
            //cfg.CreateMap<CountryViewModel, Country>();

            //cfg.CreateMap<District, DistrictViewModel>();
            //cfg.CreateMap<DistrictViewModel, District>();

            //cfg.CreateMap<Tehsil, TehsilViewModel>();
            //cfg.CreateMap<TehsilViewModel, Tehsil>();

            //cfg.CreateMap<State, StateViewModel>();
            //cfg.CreateMap<StateViewModel, State>();

            //cfg.CreateMap<CollegeFee, CollegeFeeViewModel>();
            //cfg.CreateMap<CollegeFeeViewModel, CollegeFee>();

            //cfg.CreateMap<College_Fee_Map, College_Fee_MapViewModel>();
            //cfg.CreateMap<College_Fee_MapViewModel, College_Fee_Map>();

            cfg.CreateMap<Complaint, ComplaintViewModel>();
            cfg.CreateMap<ComplaintViewModel, Complaint>();

            //cfg.CreateMap<Verification, VerificationViewModel>();
            //cfg.CreateMap<VerificationViewModel, Verification>();

            cfg.CreateMap<Convocation, ConvocationViewModel>();
            cfg.CreateMap<ConvocationViewModel, Convocation>();

            cfg.CreateMap<Revalution, RevalutionViewModel>();
            cfg.CreateMap<RevalutionViewModel, Revalution>();

            cfg.CreateMap<RevalutionSubject, RevalutionSubjectViewModel>();
            cfg.CreateMap<RevalutionSubjectViewModel, RevalutionSubject>();

            //cfg.CreateMap<ExamSchedule, ExamScheduleViewModel>();
            //cfg.CreateMap<ExamScheduleViewModel, ExamSchedule>();

            cfg.CreateMap<ExamSubject, ExamSubjectViewModel>();
            cfg.CreateMap<ExamSubjectViewModel, ExamSubject>();

            cfg.CreateMap<ExamViewModel, FeeStructure>();
            cfg.CreateMap<FeeStructure, ExamViewModel>();
        });

        CornoMapper = new Mapper(config);
    }
}