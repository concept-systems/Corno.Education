using Corno.Data.Admin;
using Corno.Data.ViewModels.Admin;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Corno.Admin.Interfaces;

public interface IRoleService : IMainService<AspNetRole>
{
    AspNetRole Create(RoleViewModel viewModel);
    AspNetRole Edit(RoleViewModel viewModel);
}