using Mapster;

namespace Corno.Data.Admin;

public class AspNetUserRole
{
    public string UserId { get; set; }
    public string RoleId { get; set; }

    [AdaptIgnore]
    public virtual AspNetUser User { get; set; }
}