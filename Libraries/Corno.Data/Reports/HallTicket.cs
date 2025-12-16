using System.ComponentModel.DataAnnotations;
using Corno.Data.Common;

namespace Corno.Data.Reports;

public class HallTicketViewModel : UniversityBaseModel
{
    [Required]
    public string SortOrder { get; set; }

    public string FromSeatNo { get; set; }
    public string ToSeatNo { get; set; }

    public string PrnNo { get; set; }
}