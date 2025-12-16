using Microsoft.AspNet.Identity;
using Microsoft.AspNet.Identity.EntityFramework;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Corno.Data.ViewModels;

namespace Corno.Data.Admin;

public class ExternalLoginConfirmationViewModel
{
    [Required]
    [Display(Name = "User name")]
    public string UserName { get; set; }
}

public class ManageUserViewModel
{
    [Required]
    [DataType(DataType.Password)]
    [Display(Name = "Current password")]
    public string OldPassword { get; set; }

    [Required]
    [StringLength(100, ErrorMessage = "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "New password")]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm new password")]
    [Compare("NewPassword", ErrorMessage =
        "The new password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }
}


public class LoginViewModel
{
    public LoginViewModel()
    {
        InstanceId = 0;
    }

    //[Required]
    [Display(Name = "User name")]
    public string UserName { get; set; }

    //[Required]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }
    //[Required]
    [Display(Name = "Instance")]
    public int? InstanceId { get; set; }

    public string Prn { get; set; }
    //public string MobileNo { get; set; }
    public string Otp { get; set; }

    [Display(Name = "Remember me?")]
    public bool RememberMe { get; set; }

    public virtual ICollection<MasterViewModel> Instances { get; set; }
}

public class OtpViewModel
{
    [Required]
    [Display(Name = "Mobile No")]
    public string MobileNo { get; set; }


    [Display(Name = "Otp")]
    public string Otp { get; set; }
}


public class RegisterViewModel
{
    [Required]
    [Display(Name = "User name")]
    //[RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Please enter correct User Name")]
    public string UserName { get; set; }

    [Required]
    [StringLength(100, ErrorMessage =
        "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Password")]
    public string Password { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("Password", ErrorMessage =
        "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    // New Fields added to extend Application User class:

    [Required(ErrorMessage = "First Name is required")]
    [StringLength(100, MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Please enter correct First Name")]
    public string FirstName { get; set; }

    [Required(ErrorMessage = "Last Name is required")]
    [StringLength(100, MinimumLength = 3)]
    [RegularExpression(@"^[a-zA-Z]+[ a-zA-Z-_]*$", ErrorMessage = "Please enter correct Last Name")]
    public string LastName { get; set; }

    [Required(ErrorMessage = "Please enter your email address")]
    [DataType(DataType.EmailAddress)]
    [Display(Name = "Email address")]
    [MaxLength(50)]
    //[RegularExpression(@"[a-z0-9._%+-]+@[a-z0-9.-]+\.[a-z]{2,4}", ErrorMessage = "Please enter correct email")]
    //[RegularExpression(@"^[a-zA-Z0-9.!#$%&'*+/=?^_`{|}~-]+@[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?(?:\.[a-zA-Z0-9](?:[a-zA-Z0-9-]{0,61}[a-zA-Z0-9])?)*$", ErrorMessage = "Please enter correct email")]
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    [Required]
    public int? CollegeId { get; set; }
    //public Nullable<int> CourseID { get; set; }

    // Return a pre-poulated instance of AppliationUser:
    public ApplicationUser GetUser()
    {
        var user = new ApplicationUser()
        {
            UserName = UserName,
            FirstName = FirstName,
            LastName = LastName,
            Email = Email,
            PhoneNumber = PhoneNumber
        };
        return user;
    }
    //public virtual ICollection<College> Colleges { get; set; }
    //public virtual ICollection<Course> Courses { get; set; }

}


public class EditUserViewModel
{
    public EditUserViewModel() { }

    // Allow Initialization with an instance of ApplicationUser:
    public EditUserViewModel(ApplicationUser user)
    {
        UserName = user.UserName;
        FirstName = user.FirstName;
        LastName = user.LastName;
        Email = user.Email;
        PhoneNumber = user.PhoneNumber;
        CollegeId = user.CollegeId;
        //this.CourseID = user.CourseID;
    }

    [Required]
    [Display(Name = "User Name")]
    public string UserName { get; set; }

    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required]
    public string Email { get; set; }
    public string PhoneNumber { get; set; }
    [Required]
    public int? CollegeId { get; set; }
    //public Nullable<int> CourseID { get; set; }
    //public virtual ICollection<College> Colleges { get; set; }
    //public virtual ICollection<Course> Courses { get; set; }
}


public class SelectUserRolesViewModel
{
    public SelectUserRolesViewModel()
    {
        Roles = new List<SelectRoleEditorViewModel>();
    }

    // Enable initialization with an instance of ApplicationUser:
    public SelectUserRolesViewModel(ApplicationUser user)
        : this()
    {
        UserName = user.UserName;
        FirstName = user.FirstName;
        LastName = user.LastName;

        var db = new ApplicationDbContext();

        // Add all available roles to the list of EditorViewModels:
        var allRoles = db.Roles;
        foreach (var role in allRoles)
        {
            // An EditorViewModel will be used by Editor Template:
            var rvm = new SelectRoleEditorViewModel(role);
            Roles.Add(rvm);
        }

        // Set the Selected property to true for those roles for 
        // which the current user is a member:
        var roleManager = new RoleManager<IdentityRole>(
            new RoleStore<IdentityRole>(new ApplicationDbContext()));
        foreach (var userRole in user.Roles)
        {
            var checkUserRole =
                Roles.Find(r => r.RoleName == roleManager.FindById(userRole.RoleId).Name);
            checkUserRole.Selected = true;
        }
    }

    public string UserName { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public List<SelectRoleEditorViewModel> Roles { get; set; }
}

// Used to display a single role with a checkbox, within a list structure:
public class SelectRoleEditorViewModel
{
    public SelectRoleEditorViewModel() { }
    public SelectRoleEditorViewModel(IdentityRole role)
    {
        RoleName = role.Name;
    }

    public bool Selected { get; set; }

    [Required]
    public string RoleName { get; set; }
}
public class ResetPasswordViewModel
{

    //[Display(Name = "User name")]
    //public string UserName { get; set; }

    //[Required]
    [StringLength(100, ErrorMessage =
        "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "Old Password")]
    public string Password { get; set; }

    [Required]
    [StringLength(100, ErrorMessage =
        "The {0} must be at least {2} characters long.", MinimumLength = 6)]
    [DataType(DataType.Password)]
    [Display(Name = "New Password")]
    public string NewPassword { get; set; }

    [DataType(DataType.Password)]
    [Display(Name = "Confirm password")]
    [Compare("NewPassword", ErrorMessage =
        "The password and confirmation password do not match.")]
    public string ConfirmPassword { get; set; }

    // New Fields added to extend Application User class:

    //[Required]
    //[Display(Name = "First Name")]
    //public string FirstName { get; set; }

    //[Required]
    //[Display(Name = "Last Name")]
    //public string LastName { get; set; }

    //[Required]
    //public string Email { get; set; }
}
public class UserProfileViewModel
{
    [Required]
    [Display(Name = "First Name")]
    public string FirstName { get; set; }

    [Required]
    [Display(Name = "Last Name")]
    public string LastName { get; set; }

    [Required]
    public string Email { get; set; }

    // Return a pre-poulated instance of AppliationUser:
    public ApplicationUser GetUser()
    {
        var user = new ApplicationUser()
        {
            FirstName = FirstName,
            LastName = LastName,
            Email = Email,
        };
        return user;
    }
}