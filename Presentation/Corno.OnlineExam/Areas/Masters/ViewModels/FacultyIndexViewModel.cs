using System;
using Corno.Data.ViewModels;

namespace Corno.OnlineExam.Areas.Masters.ViewModels;

public class FacultyIndexViewModel : MasterViewModel
{
    public string Dean { get; set; }
    public DateTime? RecognitionDate { get; set; }
}