using System;
using System.Collections.Generic;

namespace Corno.Data.ViewModels;

public class ExamDetailViewModel : UniversityViewModel
{
    public ExamDetailViewModel()
    {
        Subjects = new List<KeyValuePair<string, string>>();
    }

    public string Prn { get; set; }

    public DateTime? FormDate { get; set; }
    public double? Fee { get; set; }
    public string TransactionId { get; set; }

    public List<KeyValuePair<string, string>> Subjects {get; set; }
}