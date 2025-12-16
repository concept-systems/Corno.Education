using Corno.Data.Admin;
using Corno.Data.Corno;
using Corno.Data.ViewModels;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Core.Marks_Entry.Interfaces;

public interface IMarksApiService : IBaseService
{
    #region -- Methods --

    /*void GetFormSeatNos(MarksEntry model);
    void GetReportSeatNos(MarksEntry model);

    void Save(MarksEntry model, string submitType, AspNetUser user);*/

    MarksApiDto ImportFromMarksApi(MarksApiViewModel marksApiViewModel);

    #endregion
}