using Corno.Data.Admin;
using Corno.Services.Corno.Interfaces;
using System.Collections.Generic;

namespace Corno.Services.Corno.Admin.Interfaces;

public interface IUserRoleService : IMainService<AspNetUserRole>
{
    void AddRoles(string userId, List<string> roleNames);
}