using Corno.Data.Admin;
using Corno.Services.Corno;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Login.Interfaces;

namespace Corno.Services.Login;

public class AspNetRoleService : BaseService, IAspNetRoleService
{
    private IGenericRepository<AspNetRole> _aspnetroleRepository;

    public AspNetRoleService(IUnitOfWork unitOfWork, IGenericRepository<AspNetRole> aspnetroleRepository)
    {
        UnitOfWork = unitOfWork;
        _aspnetroleRepository = aspnetroleRepository;
    }

    public IGenericRepository<AspNetRole> AspNetRoleRepository
    {
        get
        {
            if (_aspnetroleRepository != null) return _aspnetroleRepository;

            _aspnetroleRepository = new GenericRepository<AspNetRole>(UnitOfWork);
            return _aspnetroleRepository;
        }
    }
}