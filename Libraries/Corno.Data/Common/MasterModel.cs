using System.ComponentModel.DataAnnotations;

namespace Corno.Data.Common;

public class MasterModel : BaseModel
{
    [Required]
    public string Name { get; set; }
    public string Description { get; set; }
}