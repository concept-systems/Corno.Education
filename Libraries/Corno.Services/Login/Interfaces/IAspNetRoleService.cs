using Corno.Data.Admin;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Login.Interfaces;

public interface IAspNetUserService : IBaseService
{
    IGenericRepository<AspNetUser> AspNetUserRepository { get; }
}