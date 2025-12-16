using Mapster;

namespace Corno.Data.Admin;

public class AspNetUserLogin
{
    public string LoginProvider { get; set; }
    public string ProviderKey { get; set; }
    public string UserId { get; set; }

    [AdaptIgnore]
    public virtual AspNetUser AspNetUser { get; set; }
}