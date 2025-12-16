using System.Collections.Generic;
using System.Linq;
using Corno.Data.ViewModels;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Email.Interfaces;
using Corno.Services.SMS.Interfaces;

namespace Corno.Services.Core;

public class UniversityService :BaseService, IUniversityService
{
    #region -- Constructors --
    public UniversityService(ICornoService cornoService, ICoreService coreService, ISmsService smsService, IEmailService emailService)
    {
        _cornoService = cornoService;
        _coreService = coreService;
        _smsService = smsService;
        _emailService = emailService;
    }
    #endregion

    #region -- Data Members --

    private readonly ICornoService _cornoService;
    private readonly ICoreService _coreService;
    private readonly ISmsService _smsService;
    private readonly IEmailService _emailService;


    #endregion

    #region -- Private Methods --

    #endregion

    #region -- Public Methods --
    public List<MasterViewModel> GetBranchesByCoursePart(int? courseId, int? coursePartId)
    {
        if (null == courseId || null == coursePartId)
            return null;

        courseId = (int)courseId;
        coursePartId = (int)coursePartId;

        var coursePart = _coreService.Tbl_COURSE_PART_MSTR_Repository.Get(c => c.Num_PK_COPRT_NO == coursePartId).FirstOrDefault();
        if (coursePart == null || coursePart.Chr_COPRT_BRANCH_APP_FLG.Trim() != "Y")
            return null;

        var branches = _coreService.Tbl_BRANCH_MSTR_Repository.Get()
            .Where(p => p.Num_FK_CO_CD == courseId)
            .Select(p => new MasterViewModel{ Code = p.Num_PK_BR_CD.ToString(), Id = p.Num_PK_BR_CD, Name = p.Var_BR_SHRT_NM, NameWithId = "(" + p.Num_PK_BR_CD + ") " + p.Var_BR_SHRT_NM })
            .OrderBy(b => b.Id)
            .ToList();

        return branches;
    }

    #endregion
}