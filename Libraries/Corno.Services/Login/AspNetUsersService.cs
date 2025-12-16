using Corno.Data.Admin;
using Corno.Services.Corno;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Login.Interfaces;

namespace Corno.Services.Login;

public class AspNetUserService : BaseService, IAspNetUserService
{
    private IGenericRepository<AspNetUser> _aspnetuserRepository;

    public AspNetUserService(IUnitOfWork unitOfWork, IGenericRepository<AspNetUser> aspnetuserRepository)
    {
        UnitOfWork = unitOfWork;
        _aspnetuserRepository = aspnetuserRepository;
    }

    public IGenericRepository<AspNetUser> AspNetUserRepository
    {
        get
        {
            if (_aspnetuserRepository != null) return _aspnetuserRepository;

            _aspnetuserRepository = new GenericRepository<AspNetUser>(UnitOfWork);
            return _aspnetuserRepository;
        }
    }
}