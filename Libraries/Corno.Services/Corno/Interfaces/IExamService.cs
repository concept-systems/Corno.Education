using Corno.Data.Corno;

namespace Corno.Services.Corno.Interfaces;

public interface IExamService : IBaseService
{
    #region -- Methods --

    void AddExamInExamServer(Exam model, string userName);

    #endregion
}