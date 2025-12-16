using Corno.Data.Admin;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Login.Interfaces;

public interface IAspNetRoleService : IBaseService
{
    IGenericRepository<AspNetRole> AspNetRoleRepository { get; }
}