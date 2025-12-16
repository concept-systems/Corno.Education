using Corno.Data.Admin;
using Corno.Data.ViewModels.Admin;
using Corno.Services.Corno.Admin.Interfaces;
using System;

namespace Corno.Services.Corno.Admin;

public class RoleService : MainService<AspNetRole>, IRoleService
{
    #region -- Public Methods --

    public AspNetRole Create(RoleViewModel viewModel)
    {
        if (string.IsNullOrEmpty(viewModel.Name))
            throw new Exception($"Invalid role name {viewModel.Name}");

        var role = new AspNetRole
        {
            Id = Guid.NewGuid().ToString(),
            Name = viewModel.Name
        };

        AddAndSave(role);

        return role;
    }

    public AspNetRole Edit(RoleViewModel viewModel)
    {
        if (string.IsNullOrEmpty(viewModel.Name))
            throw new Exception($"Invalid role name {viewModel.Name}");

        var existing = GetById(viewModel.Id);
        if (null == existing)
            throw new Exception($"Role with name {viewModel.Name} does not exist.");

        existing.Name = viewModel.Name;

        UpdateAndSave(existing);

        return existing;
    }
    
    #endregion
}