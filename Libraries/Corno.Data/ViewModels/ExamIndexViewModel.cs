using System;
using Corno.Globals.Enums;

namespace Corno.Data.ViewModels;

public class ExamIndexViewModel : UniversityViewModel
{
    public string Prn { get; set; }
    public FormType FormType { get; set; }
    public DateTime? FormDate { get; set; }
    public string TransactionId { get; set; }
    public double? TotalFee { get; set; }
}