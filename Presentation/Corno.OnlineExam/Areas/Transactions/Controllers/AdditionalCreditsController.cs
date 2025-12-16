using Corno.Logger;
using Corno.OnlineExam.Areas.Transactions.Dtos;
using Corno.OnlineExam.Controllers;
using Corno.Services.Core.Interfaces;
using System;
using System.Linq;
using System.Web.Mvc;
using Corno.Data.Helpers;

namespace Corno.OnlineExam.Areas.Transactions.Controllers;

[Authorize]
public class AdditionalCreditsController : BaseController
{
    #region -- Constructors --
    public AdditionalCreditsController(ICoreService coreService)
    {
        _coreService = coreService;
    }
    #endregion

    #region -- Data Members --
    private readonly ICoreService _coreService;
    #endregion

    #region -- Private Methods --
    private AddlCreditsDto CreateDto(string prn)
    {
        // Fetch student info from Tbl_STUDENT_INFO and related tables
        var studentCourse = _coreService.Tbl_STUDENT_COURSE_Repository
            .FirstOrDefault(s => s.Chr_FK_PRN_NO == prn, p => p);
        if (studentCourse == null)
            throw new Exception($"Prn '{prn}' not found");
        var studentInfo = _coreService.Tbl_STUDENT_INFO_Repository
            .FirstOrDefault(s => s.Chr_PK_PRN_NO == prn, p => p);

        // Fetch college, course, branch names
        var college = _coreService.TBL_COLLEGE_MSTRRepository
            .FirstOrDefault(s => s.Num_PK_COLLEGE_CD == studentCourse.Num_ST_COLLEGE_CD, p => p);
        var course = _coreService.Tbl_COURSE_MSTR_Repository
            .FirstOrDefault(s => s.Num_PK_CO_CD == studentCourse.Num_FK_CO_CD, p => p);
        var branch = _coreService.Tbl_BRANCH_MSTR_Repository
            .FirstOrDefault(s => s.Num_PK_BR_CD == studentCourse.Num_FK_BR_CD, p => p);

        // Fetch subjects (Num_FK_COPRT_NO = 1247)
        var serialNo = 1;
        var subjects = _coreService.Tbl_SUBJECT_MSTR_Repository.Get(s => s.Num_FK_COPRT_NO == 1247 &&
                                                                         s.NUM_CAT_SCALEDOWN_FROM_T1 == studentCourse.Num_FK_CO_CD).ToList()
            .Select(p => new AddlCreditsSubjectDto
            {
                SerialNo = serialNo++,
                SubjectId = p.Num_PK_SUB_CD,
                SubjectName = p.Var_SUBJECT_NM,
                MaximumCredits = p.Num_MIN_GRD_POINT ?? 0,
                IsCompleted = false,
                //CompletedDate = DateTime.Now
            }).ToList();

        // Build DTO
        var dto = new AddlCreditsDto
        {
            Prn = studentCourse.Chr_FK_PRN_NO,
            StudentName = studentInfo.Var_ST_NM,
            CollegeId = college?.Num_PK_COLLEGE_CD ?? 0,
            CollegeName = $"({college?.Num_PK_COLLEGE_CD ?? 0}) {college?.Var_CL_COLLEGE_NM1}, {college?.Var_CL_CITY_NM}",
            CourseId = course?.Num_PK_CO_CD ?? 0,
            CourseName = $"({course?.Num_PK_CO_CD ?? 0}) {course?.Var_CO_NM}",
            BranchId = branch?.Num_PK_BR_CD ?? 0,
            BranchName = $"({branch?.Num_PK_BR_CD ?? 0}) {branch?.Var_BR_NM}",

            Subjects = subjects
        };
        return dto;
    }
    #endregion

    #region -- Actions --
    // GET: /MobileUpdate/Create
    [Authorize]
    public ActionResult Create()
    {
        return View(new AddlCreditsDto());
    }

    // POST: /MobileUpdate/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public ActionResult Create(AddlCreditsDto dto)
    {
        try
        {
            if (!ModelState.IsValid)
                View(dto);

            foreach (var detail in dto.Subjects)
            {
                var existing = _coreService.Tbl_Additional_Credits_Repository.FirstOrDefault(p =>
                    p.Chr_ADD_PRN_NO == dto.Prn && p.Num_FK_SUB_CD == detail.SubjectId, p => p);
                if (null != existing)
                {
                    existing.Chr_IS_COMPLETED = detail.IsCompleted? "Y" : "N";
                    existing.Dtm_COMPLETED = detail.CompletedDate;
                    //existing.Dtm_DTE_UP = DateTime.Now;
                    existing.Var_USR_NM = User.Identity.Name;
                    _coreService.Tbl_Additional_Credits_Repository.Update(existing);
                }
                else
                {
                    var newCredit = new Corno.Data.Core.Tbl_ADDITIONAL_CREDITS
                    {
                        Chr_ADD_PRN_NO = dto.Prn,
                        Num_FK_COLLEGE_CD = (short)dto.CollegeId,
                        Num_FK_COURCE_CD = (short)dto.CourseId,
                        Num_FK_BR_CD = (short?)dto.BranchId,
                        Num_FK_SUB_CD = (short)detail.SubjectId,
                        Num_FK_INST_NO = (short)dto.InstanceId,
                        Num_MAX_CREDITS = (short)detail.MaximumCredits,
                        Chr_IS_COMPLETED = detail.IsCompleted ? "Y" : "N",
                        Dtm_COMPLETED = detail.CompletedDate,
                        DELETE_FLG = "N",
                        Var_USR_NM = User.Identity.Name,
                        //Dtm_DTE_CR = DateTime.Now
                    };

                    _coreService.Tbl_Additional_Credits_Repository.Add(newCredit);
                }
            }

            _coreService.Save();

            return RedirectToAction("Create");
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
            LogHandler.LogInfo(LogHandler.GetDetailException(exception));
        }

        return View(dto);
    }

    [HttpGet]
    public JsonResult GetStudentDetails(string prn)
    {
        try
        {
            // Create new
            var dto = CreateDto(prn);

            // Update completed subjects based on existing credits
            var existing = _coreService.Tbl_Additional_Credits_Repository.Get(p =>
                p.Chr_ADD_PRN_NO == prn, p => p).ToList();
            if (existing.Count <= 0) 
                return Json(new { success = true, student = dto }, JsonRequestBehavior.AllowGet);

            foreach (var credit in existing)
            {
                var subjectDto = dto.Subjects.FirstOrDefault(s => s.SubjectId == credit.Num_FK_SUB_CD);
                if (subjectDto != null)
                {
                    subjectDto.IsCompleted = credit.Chr_IS_COMPLETED == "Y";
                    subjectDto.CompletedDate = credit.Dtm_COMPLETED;
                }
            }

            return Json(new { success = true, student = dto }, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            return Json(new { success = false, Message = exception.Message }, JsonRequestBehavior.AllowGet);
        }
    }

    #endregion
}

