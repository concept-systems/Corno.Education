using System.Collections.Generic;
using System.Linq;
using Corno.Data.Admin;
using Corno.Services.Corno.Admin.Interfaces;

namespace Corno.Services.Corno.Admin;

public class UserRoleService : MainService<AspNetUserRole>, IUserRoleService
{
    #region -- Constructors --

    public UserRoleService(IRoleService roleService)
    {
        _roleService = roleService;
    }
    #endregion

    #region -- Data Members --
    private readonly IRoleService _roleService;
    #endregion

    #region -- Public Methods --

    public void AddRoles(string userId, List<string> roleNames)
    {
        var roleIds = _roleService.Get(p => roleNames.Contains(p.Name), p => p.Id);

        var userRoles = Get(p => p.UserId == userId, 
            p => p).ToList();
        foreach (var roleId in roleIds)
        {
            if(null == userRoles.FirstOrDefault(p => p.RoleId == roleId))
                Add(new AspNetUserRole{UserId = userId, RoleId = roleId});
        }

        Save();
    }
    #endregion
}