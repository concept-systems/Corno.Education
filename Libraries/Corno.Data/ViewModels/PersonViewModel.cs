using System.ComponentModel.DataAnnotations;
using Corno.Data.Common;

namespace Corno.Data.ViewModels;

public class PersonViewModel : MasterModel
{
    #region -- Properties --
    [Required]
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }

    public string Mobile { get; set; }
    public string Email { get; set; }
    #endregion

    #region -- Public Methods --

    public string GetFullName()
    {
        var middleName = string.IsNullOrEmpty(MiddleName) ? string.Empty : $" {MiddleName}";
        var lastName = string.IsNullOrEmpty(LastName) ? string.Empty : $" {LastName}";
        Name = $"{FirstName}{middleName}{lastName}";
                     
        return Name;
    }
#endregion
}