using Corno.Data.Corno.Online_Education;
using Corno.Data.ViewModels;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Corno.Online_Education.Interfaces;

public interface ICollege45OptionalSubjectService : IMainService<College45OptionalSubject>
{
    void UpdateExamOptionalSubjects(ExamViewModel viewModel);
}
