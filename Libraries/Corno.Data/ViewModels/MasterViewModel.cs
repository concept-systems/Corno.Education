namespace Corno.Data.ViewModels;

public class MasterViewModel : BaseViewModel
{
    public string Name { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string NameWithCode { get; set; } = string.Empty;
    public string NameWithId { get; set; } = string.Empty;
}