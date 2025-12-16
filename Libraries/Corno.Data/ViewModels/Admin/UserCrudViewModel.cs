using System.Collections.Generic;

namespace Corno.Data.ViewModels.Admin;

public class UserCrudViewModel 
{
    #region -- Constructors --
    public UserCrudViewModel()
    {
        Roles = new List<UserRoleViewModel>();
    }
    #endregion

    #region -- Properties --
    public string Id { get; set; }
    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    public bool Locked { get; set; }

    public string Password { get; set; }
    public string ConfirmPassword { get; set; }

    public List<UserRoleViewModel> Roles { get; set; }
#endregion
}

public class UserRoleViewModel
{
    public bool IsSelected { get; set; }
    public string RoleName { get; set; }
}