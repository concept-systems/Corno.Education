namespace Corno.Data.ViewModels;

public class BaseViewModel : CornoViewModel
{
    public int? SerialNo { get; set; }
    public string Code { get; set; }
    public int Id { get; set; }
    public string Status { get; set; }
}