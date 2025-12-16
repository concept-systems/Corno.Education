namespace Corno.Data.Corno;

public class MobileUpdateViewModel
{
    public string PrnNo { get; set; }
    public string Name { get; set; }
    /*[Required]
    [StringLength(10, MinimumLength = 10, ErrorMessage = "Mobile must be 10 digits.")]
    [RegularExpression(@"^\d{10}$", ErrorMessage = "Mobile must be a 10-digit number.")]*/
    public string Mobile { get; set; }
    public string Email { get; set; }

    /*[Required]
    [StringLength(12, MinimumLength = 12, ErrorMessage = "ABC ID must be 12 digits.")]
    [RegularExpression(@"^\d{12}$", ErrorMessage = "ABC ID must be a 12-digit number.")]*/
    public string Abc { get; set; }
}