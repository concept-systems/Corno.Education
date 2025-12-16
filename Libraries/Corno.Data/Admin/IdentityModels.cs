using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Corno.Data.Admin;

public class ApplicationUser : IdentityUser
{
    [Required]
    public string FirstName { get; set; }

    [Required]
    public string LastName { get; set; }

    public int? CollegeId { get; set; }
    public int? CourseId { get; set; }
    public string Type { get; set; }
}


public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext()
        : base("DefaultConnection", throwIfV1Schema: false)
    {

    }
}

public class IdentityManager
{
    public bool RoleExists(string name)
    {
        var roleManager = new RoleManager<IdentityRole>(
            new RoleStore<IdentityRole>(new ApplicationDbContext()));
        return roleManager.RoleExists(name);
    }


    public bool CreateRole(string name)
    {
        var roleManager = new RoleManager<IdentityRole>(
            new RoleStore<IdentityRole>(new ApplicationDbContext()));
        var idResult = roleManager.Create(new IdentityRole(name));
        return idResult.Succeeded;
    }

    public bool EditRole(AspNetRole role)
    {
        var roleManager = new RoleManager<IdentityRole>(
            new RoleStore<IdentityRole>(new ApplicationDbContext()));
        IdentityRole roleToUpdate = roleManager.FindById(role.Id);
        var idResult = roleManager.Update(roleToUpdate);
        return idResult.Succeeded;
    }

    public bool DeleteRole(AspNetRole role)
    {
        var roleManager = new RoleManager<IdentityRole>(
            new RoleStore<IdentityRole>(new ApplicationDbContext()));
        IdentityRole roleToDelete = roleManager.FindById(role.Id);
        var idResult = roleManager.Delete(roleToDelete);
        return idResult.Succeeded;
    }


    public bool CreateUser(ApplicationUser user, string password)
    {
        var userManager = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(new ApplicationDbContext()));
        var idResult = userManager.Create(user, password);
        return idResult.Succeeded;
    }


    public bool AddUserToRole(string userId, string roleName)
    {
        var userManager = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(new ApplicationDbContext()));
        var idResult = userManager.AddToRole(userId, roleName);
        return idResult.Succeeded;
    }


    public void ClearUserRoles(string userId)
    {
        var userManager = new UserManager<ApplicationUser>(
            new UserStore<ApplicationUser>(new ApplicationDbContext()));
        var roleManager = new RoleManager<IdentityRole>(
            new RoleStore<IdentityRole>(new ApplicationDbContext()));
        var user = userManager.FindById(userId);
        var currentRoles = new List<IdentityUserRole>();
        currentRoles.AddRange(user.Roles);
        foreach (var role in currentRoles)
        {
            userManager.RemoveFromRole(userId, roleManager.FindById(role.RoleId).Name);
        }
    }
}