using Corno.Data.Corno.Question_Bank;
using Corno.Data.ViewModels;
using Corno.Services.Corno.Interfaces;
using System.Collections.Generic;
using Corno.Data.Corno.Question_Bank.Models;

namespace Corno.Services.Corno.Question_Bank.Interfaces;

public interface IStructureService : IMainService<Structure>
{
    #region -- Methods --

    Structure GetExisting(int facultyId, int courseId, int coursePartId, int branchId, 
        int subjectId, int paperCategoryId);
    Structure GetExisting(QuestionBankModel model);

    Structure GetQuestions(Structure structure);
    List<MasterViewModel> GetQuestionNos(int subjectId, int chapterId);

    void Save(Structure structure);
    #endregion
}