using System.Collections.Generic;

namespace Corno.Data.Admin;

public sealed class AspNetRole
{
    public AspNetRole()
    {
        //AspNetUsers = new List<AspNetUser>();
    }

    public string Id { get; set; }
    public string Name { get; set; }
    //public ICollection<AspNetUser> AspNetUsers { get; set; }
}