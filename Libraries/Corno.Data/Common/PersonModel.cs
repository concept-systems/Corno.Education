using System.ComponentModel.DataAnnotations;

namespace Corno.Data.Common;

public class PersonModel : MasterModel
{
    #region -- Properties --
    /*[Required]*/
    public string FirstName { get; set; }
    public string MiddleName { get; set; }
    public string LastName { get; set; }
    #endregion

    #region -- Public Methods --

    public string GetFullName()
    {
        var middleName = string.IsNullOrEmpty(MiddleName) ? string.Empty : $" {MiddleName}";
        var lastName = string.IsNullOrEmpty(LastName) ? string.Empty : $" {LastName}";
        Name = $"{FirstName}{middleName}{lastName}";

        return Name;
    }

    public string SplitFullName()
    {
        var names = Name.Split(' ');
        LastName = names[0];
        MiddleName = names.Length > 1 ? names[1] : string.Empty;
        FirstName = names.Length > 2 ? names[2] : string.Empty;
        
        return Name;
    }
    #endregion
}