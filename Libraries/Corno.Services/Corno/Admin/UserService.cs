using System;
using System.Collections.Generic;
using System.Linq;
using Corno.Data.Admin;
using Corno.Data.ViewModels.Admin;
using Corno.Globals.Enums;
using Corno.Services.Corno.Admin.Interfaces;
using Mapster;
using Microsoft.AspNet.Identity;
using PasswordGenerator;

namespace Corno.Services.Corno.Admin;

public class UserService : MainService<AspNetUser>, IUserService
{
    #region -- Constructors --

    public UserService(IRoleService roleService, IUserRoleService userRoleService)
    {
        _roleService = roleService;
        _userRoleService = userRoleService;

        SetIncludes($"{nameof(AspNetUser.UserRoles)}");
        /*SetIncludes($"{nameof(AspNetUser.AspNetUserRoles)},{nameof(AspNetUser.AspNetUserClaims)}," +
                    $"{nameof(AspNetUser.AspNetUserLogins)}");*/
    }
    #endregion

    #region -- Data Members --

    private readonly IRoleService _roleService;
    private readonly IUserRoleService _userRoleService;

    #endregion

    #region -- Public Methods --

    public UserCrudViewModel GetViewModelWithRoles(string id)
    {
        // Get AspNetUser
        var user = GetById(id);

        var viewModel = new UserCrudViewModel();
        user.Adapt(viewModel);

        /*var userRoleIds = _userRoleService.Get(p => p.UserId == id, 
                p => p.RoleId).ToList();*/
        var roleIds = user.UserRoles.Select(p => p.RoleId).ToList();
        var roles = _roleService.Get(null,
                p => p).ToList();

        foreach (var role in roles)
        {
            viewModel.Roles.Add(new UserRoleViewModel
            {
                IsSelected = roleIds.Contains(role.Id),
                RoleName = role.Name
            });
        }

        return viewModel;
    }

    public AspNetUser Create(UserCrudViewModel viewModel)
    {
        if (!viewModel.Password.Equals(viewModel.ConfirmPassword, StringComparison.InvariantCulture))
            throw new Exception("Password and confirm password doesn't match");

        var existing = FirstOrDefault(p =>
            p.UserName.Equals(viewModel.UserName, StringComparison.OrdinalIgnoreCase), p => p);
        if (null != existing)
            throw new Exception($"User with user name {viewModel.UserName} already exists.");

        var aspNetUser = new AspNetUser();
        viewModel.Adapt(aspNetUser);

        var passwordHasher = new PasswordHasher();
        aspNetUser.Id = Guid.NewGuid().ToString();
        aspNetUser.SecurityStamp = Guid.NewGuid().ToString();
        aspNetUser.PasswordHash = passwordHasher.HashPassword(viewModel.Password);

        AddAndSave(aspNetUser);

        return aspNetUser;
    }

    public AspNetUser CreateUser(string mobileNo, List<string> roles)
    {
        var user = FirstOrDefault(p => p.UserName == mobileNo,
            p => p);
        var password = new Password().IncludeLowercase().IncludeUppercase().IncludeSpecial()
            .LengthRequired(8);
        if (null != user)
        {
            var passwordString = password.Next();
            user.PasswordHash = new PasswordHasher().HashPassword(passwordString);
            user.Password = passwordString;

            user.Type = UserType.PaperSetter.ToString();

            UpdateAndSave(user);

            _userRoleService.AddRoles(user.Id, roles);

            if (!user.Type.Equals(UserType.PaperSetter.ToString(), StringComparison.InvariantCultureIgnoreCase))
                throw new Exception($"User {mobileNo} is already created with another type {user.Type}");

            user.Password = passwordString;
            return user;
        }

        var aspNetUser = new AspNetUser();


        var standardPassword = password.Next();// "setter@123";

        var passwordHasher = new PasswordHasher();
        aspNetUser.Id = Guid.NewGuid().ToString();
        aspNetUser.SecurityStamp = Guid.NewGuid().ToString();
        aspNetUser.UserName = mobileNo;
        aspNetUser.FirstName = mobileNo;
        aspNetUser.LastName = mobileNo;
        aspNetUser.EmailConfirmed = true;
        aspNetUser.PhoneNumberConfirmed = true;
        aspNetUser.PasswordHash = passwordHasher.HashPassword(standardPassword);
        aspNetUser.Password = standardPassword;

        AddAndSave(aspNetUser);

        // Add Paper Setter Role
        _userRoleService.AddRoles(aspNetUser.Id, roles);

        return aspNetUser;
    }

    /*public AspNetUser CreatePaperSetter(string mobileNo)
    {
        var user = FirstOrDefault(p => p.UserName == mobileNo,
            p => p);
        var password = new Password().IncludeLowercase().IncludeUppercase().IncludeSpecial()
            .LengthRequired(8);
        if (null != user)
        {
            /*Delete(user);
            Save();#1#

            var passwordString = password.Next();
            /*if (null == user.Type)
            {#1#
            user.PasswordHash = new PasswordHasher().HashPassword(passwordString);
            user.Password = passwordString;

            user.Type = UserType.PaperSetter.ToString();
            UpdateAndSave(user);
            //}
            if (!user.Type.Equals(UserType.PaperSetter.ToString(), StringComparison.InvariantCultureIgnoreCase))
                throw new Exception($"User {mobileNo} is already created with another type {user.Type}");

            user.Password = passwordString;
            return user;
        }

        var aspNetUser = new AspNetUser();


        var standardPassword = password.Next();// "setter@123";

        var passwordHasher = new PasswordHasher();
        aspNetUser.Id = Guid.NewGuid().ToString();
        aspNetUser.SecurityStamp = Guid.NewGuid().ToString();
        aspNetUser.UserName = mobileNo;
        aspNetUser.FirstName = mobileNo;
        aspNetUser.LastName = mobileNo;
        aspNetUser.EmailConfirmed = true;
        aspNetUser.PhoneNumberConfirmed = true;
        aspNetUser.PasswordHash = passwordHasher.HashPassword(standardPassword);
        aspNetUser.Password = standardPassword;

        AddAndSave(aspNetUser);

        // Add Paper Setter Role
        _userRoleService.AddRoles(aspNetUser.Id, new List<string> { RoleConstants.PaperSetter });

        return aspNetUser;
    }*/


    public AspNetUser Edit(UserCrudViewModel viewModel)
    {
        var existing = FirstOrDefault(p =>
            p.Id.Equals(viewModel.Id, StringComparison.OrdinalIgnoreCase), p => p);
        if (null == existing)
            throw new Exception($"User with user name {viewModel.UserName} does not exist.");

        existing.FirstName = viewModel.FirstName;
        existing.LastName = viewModel.LastName;
        existing.Email = viewModel.Email;
        existing.PhoneNumber = viewModel.PhoneNumber;

        UpdateAndSave(existing);

        // Add Paper Setter Role
        viewModel.Roles ??= new List<UserRoleViewModel>();
        var selectedRoles = viewModel.Roles.Where(r => r.IsSelected)
            .Select(r => r.RoleName)
            .ToList();
        _userRoleService.AddRoles(existing.Id, selectedRoles);

        return existing;
    }

    public AspNetUser ChangePassword(UserCrudViewModel viewModel)
    {
        if (!viewModel.Password.Equals(viewModel.ConfirmPassword, StringComparison.InvariantCulture))
            throw new Exception("Password and confirm password doesn't match");

        var existing = FirstOrDefault(p =>
            p.Id.Equals(viewModel.Id, StringComparison.OrdinalIgnoreCase), p => p);
        if (null == existing)
            throw new Exception($"User with user name {viewModel.UserName} does not exist.");

        var passwordHasher = new PasswordHasher();
        existing.PasswordHash = passwordHasher.HashPassword(viewModel.Password);

        UpdateAndSave(existing);

        return existing;
    }
    #endregion
}