using Corno.Data.Admin;
using Corno.Data.ViewModels.Admin;
using Corno.Services.Corno.Interfaces;
using System.Collections.Generic;

namespace Corno.Services.Corno.Admin.Interfaces;

public interface IUserService : IMainService<AspNetUser>
{
    UserCrudViewModel GetViewModelWithRoles(string id);

    AspNetUser Create(UserCrudViewModel viewModel);
    //AspNetUser CreatePaperSetter(string mobileNo);
    AspNetUser CreateUser(string mobileNo, List<string> roles);
    AspNetUser Edit(UserCrudViewModel viewModel);

    AspNetUser ChangePassword(UserCrudViewModel viewModel);
}