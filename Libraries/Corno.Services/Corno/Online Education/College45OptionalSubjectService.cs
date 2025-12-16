using System.Linq;
using Corno.Data.Corno.Online_Education;
using Corno.Data.Helpers;
using Corno.Data.ViewModels;
using Corno.Services.Corno.Online_Education.Interfaces;

namespace Corno.Services.Corno.Online_Education;

public class College45OptionalSubjectService : MainService<College45OptionalSubject>, ICollege45OptionalSubjectService
{
    public void UpdateExamOptionalSubjects(ExamViewModel viewModel)
    {
        if (viewModel == null || string.IsNullOrEmpty(viewModel.PrnNo)) return;
        if (viewModel.ExamSubjectViewModels == null) return;

        var existing = Get(c => c.InstanceId == viewModel.InstanceId && c.Prn == viewModel.PrnNo,
                p => p)
            .FirstOrDefault();
        if (null == existing) return;

        foreach (var subject in viewModel.ExamSubjectViewModels.Where(s =>
                     s.OptionalIndex > 0 || (s.SubjectType != null && s.SubjectType.Contains("Optional"))))
        {
            var code = subject.SubjectCode.ToInt();
            var isElective = existing.ElectiveSubject1 == code || existing.ElectiveSubject2 == code ||
                             existing.ElectiveSubject3 == code || existing.ElectiveSubject4 == code ||
                             existing.ElectiveSubject5 == code || existing.ElectiveSubject6 == code ||
                             existing.ElectiveSubject7 == code;
            if (!isElective) 
                continue;

            subject.RowSelector = true;
            subject.IsSelected = true;
        }
    }
}
