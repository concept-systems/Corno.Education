using Corno.Data.Common;
using Corno.Data.Corno.Question_Bank;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;

namespace Corno.Data.Admin;

public sealed class AspNetUser
{
    #region -- Construtors --
    public AspNetUser()
    {
        UserRoles = new List<AspNetUserRole>();
        /*AspNetUserClaims = new List<AspNetUserClaim>();
        AspNetUserLogins = new List<AspNetUserLogin>();*/
    }
    #endregion

    #region -- Properties --
    public string Id { get; set; }
    public string FirstName { get; set; }
    public string LastName { get; set; }
    public string Email { get; set; }
    public bool EmailConfirmed { get; set; }
    public string PasswordHash { get; set; }
    public string SecurityStamp { get; set; }
    public string PhoneNumber { get; set; }
    public bool PhoneNumberConfirmed { get; set; }
    public bool TwoFactorEnabled { get; set; }
    public DateTime? LockoutEndDateUtc { get; set; }
    public bool LockoutEnabled { get; set; }
    public int AccessFailedCount { get; set; }
    public string UserName { get; set; }

    public int? CollegeId { get; set; }
    public string Type { get; set; }

    [NotMapped]
    public string Password { get; set; }

    public ICollection<AspNetUserRole> UserRoles { get; set; }
    /*public ICollection<AspNetUserClaim> AspNetUserClaims { get; set; }
    public ICollection<AspNetUserLogin> AspNetUserLogins { get; set; }*/
    #endregion

    /*#region -- Private Methods --

    private void UpdateUserRoles(AspNetUser newUser)
    {
        var toDelete = new List<AspNetUserRole>();
        foreach (var userRole in UserRoles)
        {
            var newDetail = newUser.UserRoles.FirstOrDefault(d =>
                d.UserId == userRole.UserId);
            userRole.Copy(newDetail);
        }

        // Add new entries
        var newAppointmentDetails = newAppointment.AppointmentDetails.Where(d => d.Id <= 0).ToList();
        AppointmentDetails.AddRange(newAppointmentDetails);

        // Delete existing entries
        foreach (var detail in toDelete)
            AppointmentDetails.Remove(detail);
    }

    
    #endregion

    #region -- Public Methods --
    public bool UpdateDetails(AspNetUser newUser)
    {
        // Update appointment details
        UpdateUserRoles(newUser);

        /#1#/ Update User logins
        UpdateUserLogins(newUser);

        // Update User claims
        UpdateUserClaims(newUser);#1#

        return true;
    }
    #endregion*/
}