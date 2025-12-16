using System;
using System.Web.Hosting;
using System.Web.Mvc;
using Corno.Globals;
using Corno.Services.Common;
using Corno.Services.Common.Interfaces;
using Corno.Services.Core;
using Corno.Services.Core.Interfaces;
using Corno.Services.Core.Marks_Entry;
using Corno.Services.Core.Marks_Entry.Interfaces;
using Corno.Services.Corno;
using Corno.Services.Corno.Admin;
using Corno.Services.Corno.Admin.Interfaces;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Corno.Masters;
using Corno.Services.Corno.Masters.Interfaces;
using Corno.Services.Corno.Online_Education;
using Corno.Services.Corno.Online_Education.Interfaces;
using Corno.Services.Corno.Paper_Setting;
using Corno.Services.Corno.Paper_Setting.Interfaces;
using Corno.Services.Corno.Question_Bank;
using Corno.Services.Corno.Question_Bank.Interfaces;
using Corno.Services.Email;
using Corno.Services.Email.Interfaces;
using Corno.Services.File;
using Corno.Services.File.Interfaces;
using Corno.Services.Login;
using Corno.Services.Login.Interfaces;
using Corno.Services.Payment;
using Corno.Services.Payment.Interfaces;
using Corno.Services.SMS;
using Corno.Services.SMS.Interfaces;
using Kendo.Mvc.Infrastructure;
using Kendo.Mvc.UI;
using Telerik.Reporting;
using Unity;
using Unity.Injection;
using Unity.Lifetime;
using Unity.Mvc5;

namespace Corno.Services.Bootstrapper;

public static class Bootstrapper
{
    public static IUnityContainer Initialise()
    {
        var container = BuildUnityContainer();

        DependencyResolver.SetResolver(new UnityDependencyResolver(container));

        GlobalVariables.Container = container;

        return container;
    }

    private static IUnityContainer BuildUnityContainer()
    {
        var container = new UnityContainer();
        container.RegisterType<IUnitOfWork, UnitOfWork>(new TransientLifetimeManager(), new InjectionConstructor(@"Name=DefaultConnection"));
        container.RegisterType(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        container.RegisterType<IUnitOfWorkCore, UnitOfWorkCore>(new TransientLifetimeManager(), new InjectionConstructor(@"Name=CoreContext"));
        container.RegisterType(typeof(IGenericRepositoryCore<>), typeof(GenericRepositoryCore<>));
        /*container.RegisterType<IUnitOfWork, UnitOfWork>(new SingletonLifetimeManager(), new InjectionConstructor(@"Name=DefaultConnection"));
        container.RegisterType(typeof(IGenericRepository<>), typeof(GenericRepository<>));

        container.RegisterType<IUnitOfWorkCore, UnitOfWorkCore>(new SingletonLifetimeManager(), new InjectionConstructor(@"Name=CoreContext"));
        container.RegisterType(typeof(IGenericRepositoryCore<>), typeof(GenericRepositoryCore<>));*/

        // Admin
        container.RegisterType(typeof(IAspNetRoleService), typeof(AspNetRoleService));
        container.RegisterType(typeof(IAspNetUserService), typeof(AspNetUserService));
        container.RegisterType(typeof(IUserService), typeof(UserService));
        container.RegisterType(typeof(IRoleService), typeof(RoleService));
        container.RegisterType(typeof(IUserRoleService), typeof(UserRoleService));

        // Masters
        container.RegisterType(typeof(IInstanceService), typeof(InstanceService));
        container.RegisterType(typeof(IFacultyService), typeof(FacultyService));
        container.RegisterType(typeof(ICollegeService), typeof(CollegeService));
        container.RegisterType(typeof(ICourseService), typeof(CourseService));
        container.RegisterType(typeof(ICoursePartService), typeof(CoursePartService));
        container.RegisterType(typeof(IBranchService), typeof(BranchService));
        container.RegisterType(typeof(ISubjectService), typeof(SubjectService));
        container.RegisterType(typeof(ICategoryService), typeof(CategoryService));
        container.RegisterType(typeof(IStaffService), typeof(StaffService));
        container.RegisterType(typeof(IStudentService), typeof(StudentService));
        container.RegisterType(typeof(IMiscMasterService), typeof(MiscMasterService));

        // Online
        container.RegisterType(typeof(IUniversityService), typeof(UniversityService));
        container.RegisterType(typeof(ICornoService), typeof(CornoService));
        container.RegisterType(typeof(ICoreService), typeof(CoreService));
        container.RegisterType(typeof(ILinkService), typeof(LinkService));
        container.RegisterType(typeof(IExcelFileService<>), typeof(ExcelFileService<>));
        //container.RegisterType(typeof(IHallTicketService), typeof(HallTicketService));

        // Marks Entry
        container.RegisterType(typeof(IMarksApiService), typeof(MarksApiService));

        // Paper Setting
        container.RegisterType(typeof(IAppointmentService), typeof(AppointmentService));
        container.RegisterType(typeof(IScheduleService), typeof(ScheduleService));
        container.RegisterType(typeof(IScheduleDetailService), typeof(ScheduleDetailService));
        container.RegisterType(typeof(IRemunerationService), typeof(RemunerationService));

        // Question Bank
        container.RegisterType(typeof(IStructureService), typeof(StructureService));
        container.RegisterType(typeof(IQuestionService), typeof(QuestionService));
        container.RegisterType(typeof(IQuestionAppointmentService), typeof(QuestionAppointmentService));
        container.RegisterType(typeof(IPaperService), typeof(PaperService));
        container.RegisterType(typeof(IDocumentService), typeof(DocumentService));
        container.RegisterType(typeof(IRichEditDocumentService), typeof(RichEditDocumentService));
        container.RegisterType(typeof(ITelerikDocumentService), typeof(TelerikDocumentService));

        // Online Education
        container.RegisterType(typeof(IOnlineEducationStudentService), typeof(OnlineEducationStudentService));
        container.RegisterType(typeof(ICollege45OptionalSubjectService), typeof(College45OptionalSubjectService));

        // Common
        container.RegisterType(typeof(IAmountInWordsService), typeof(AmountInWordsService));

        // SMS
        container.RegisterType(typeof(ISmsService), typeof(SmsService));

        // Email
        container.RegisterType(typeof(IEmailService), typeof(EmailService));

        // EaseBuzz payment gateway
        container.RegisterType(typeof(IEaseBuzzService), typeof(EaseBuzzService));

        // Telerik
        container.RegisterType(typeof(IDirectoryBrowser), typeof(DirectoryBrowser));
        container.RegisterType(typeof(IDirectoryPermission), typeof(DirectoryPermission));
        container.RegisterType(typeof(IVirtualPathProvider), typeof(VirtualPathProvider));
        container.RegisterType(typeof(IThumbnailCreator), typeof(ThumbnailCreator));

        return container;
    }

    public static void RegisterType(Type interfaceType, Type serviceType)
    {
        if (null == GlobalVariables.Container)
            GlobalVariables.Container = BuildUnityContainer();

        GlobalVariables.Container.RegisterType(interfaceType, serviceType);
    }

    public static Report GetReport(Type type)
    {
        GlobalVariables.Container ??= BuildUnityContainer();

        return (Report)GlobalVariables.Container.Resolve(type);
    }

    /*public static IBaseService GetService(Type type)
    {
        return (IBaseService) GlobalVariables.Container.Resolve(type);
    }*/

    public static TEntity Get<TEntity>()
    {
        return (TEntity)GlobalVariables.Container?.Resolve(typeof(TEntity));
    }
}