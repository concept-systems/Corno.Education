using Corno.Data.Corno.Online_Education;
using Corno.Data.Dtos.Online_Education;
using Corno.Data.ViewModels;
using Corno.Services.Corno.Interfaces;
using System.Collections.Generic;
using System.Web;
using Corno.Data.Core;
using Corno.Data.Corno;

namespace Corno.Services.Corno.Online_Education.Interfaces;

public interface IOnlineEducationStudentService : IMainService<OnlineStudent>
{
    void UpdateExamFees(ExamViewModel dto);
    void UpdateAppTemp(Tbl_APP_TEMP appTemp, Exam exam);
    IEnumerable<OnlineStudentImportDto> Import(HttpPostedFileBase file);
    void CancelImport();
}