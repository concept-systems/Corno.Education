using Corno.Data.Core;
using Corno.Globals.Constants;
using Corno.Globals.Enums;
using Corno.Logger;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;
using Corno.Services.Corno.Masters.Interfaces;
using MoreLinq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Corno.Data.ViewModels;
using Corno.Services.Bootstrapper;
using Kendo.Mvc.Extensions;

namespace Corno.OnlineExam.Controllers;

[Authorize]
public class UniversityController : BaseController
{
    #region -- Constructors --
    public UniversityController()
    {
        _universityService = Bootstrapper.Get<IUniversityService>();
        _cornoService = Bootstrapper.Get<ICornoService>();
        //_coreService = (ICoreService)Bootstrapper.GetService(typeof(CoreService));
        _coreService = Bootstrapper.Get<ICoreService>();
    }

    #endregion

    #region -- Data Members --
    private readonly ICoreService _coreService;
    private readonly ICornoService _cornoService;
    private readonly IUniversityService _universityService;
    #endregion

    #region -- Methods --
    public JsonResult GetInstances(FormType formType = FormType.None)
    {
        try
        {
            var instanceService = Bootstrapper.Get<IInstanceService>();
            var instances = instanceService.GetViewModelList(i => i.Status == StatusConstants.Active)
                .OrderByDescending(c => c.Id).ToList();
            switch (formType)
            {
                case FormType.Exam:
                    var instanceId = GetSession()?.InstanceId;
                    instances.RemoveAll(i => i.Id >= instanceId);
                    break;
            }

            return Json(instances, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public virtual JsonResult GetFaculties()
    {
        throw new Exception("This GetFaculties() method is not implemented yet.");
    }

    [AllowAnonymous]
    public virtual JsonResult GetColleges()
    {
        try
        {
            var colleges = _coreService.TBL_COLLEGE_MSTRRepository
                .Get().ToList()
                .Select(c => new
                {
                    Id = c.Num_PK_COLLEGE_CD,
                    Name = c.Var_CL_COLLEGE_NM1,
                    NameWithId = "(" + c.Num_PK_COLLEGE_CD + ") " + c.Var_CL_COLLEGE_NM1
                });
            return Json(colleges, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public virtual ActionResult GetCollegesByFaculty(int? facultyId, string filter = default)
    {
        throw new Exception("This method GetCollegeByFaculty is not implemented yet.");
    }

    [AllowAnonymous]
    public JsonResult GetCentersByCollege(int? collegeId)
    {
        try
        {
            if (!(collegeId is 28))
                return Json(null, JsonRequestBehavior.AllowGet);

            var centers = _coreService.TBL_DISTANCE_CENTERS_Repository
                .Get().AsEnumerable()
                .Select(c => new
                {
                    Id = c.Num_PK_DistCenter_ID, 
                    Name = c.DIST_CENT_NAME, 
                    NameWithId = "(" + c.Num_PK_DistCenter_ID + ") " + c.DIST_CENT_NAME
                })
                .OrderBy(b => b.Id);
            return Json(centers, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    [AllowAnonymous]
    public JsonResult GetCourseTypesByCollege(int? collegeId, FormType formType = FormType.None)
    {
        try
        {
            if (collegeId == null)
                throw new Exception("College Id cannot be null.");

            collegeId = (int)collegeId;

            var courseCodes = _coreService.Tbl_COLLEGE_COURSE_MSTRRepository.Get(c => c.NUM_FK_COLLEGE_CD == collegeId)
                .Select(c => c.NUM_FK_CO_CD).Distinct().ToList();
            var courseTypeIds = _coreService.Tbl_COURSE_MSTR_Repository.Get(c => courseCodes.Contains(c.Num_PK_CO_CD))
                .Select(c => c.Num_FK_TYP_CD).Distinct().ToList();
            switch (formType)
            {
                case FormType.Environment:
                    courseTypeIds = courseTypeIds.Where(c => c == 2).ToList();
                    break;
            }

            // Get course type details
            var courseTypes = _coreService.Tbl_COURSE_TYPE_MSTR_Repository
                .Get(ct => courseTypeIds.Contains(ct.Num_PK_TYP_CD)).AsEnumerable()
                .Select(c => new
                {
                    Id = c.Num_PK_TYP_CD,
                    Name = c.Var_TYP_NM,
                    NameWithId = "(" + c.Num_PK_TYP_CD + ") " + c.Var_TYP_NM
                });

            return Json(courseTypes, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public JsonResult GetCourseTypes()
    {
        try
        {
            // Get course type details
            var courseTypes = _coreService.Tbl_COURSE_TYPE_MSTR_Repository.Get().AsEnumerable()
                .Select(p => new
                {
                    Id = p.Num_PK_TYP_CD, 
                    Name = p.Var_TYP_NM, 
                    NameWithId = "(" + p.Num_PK_TYP_CD + ") " + p.Var_TYP_NM
                });

            return Json(courseTypes, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public JsonResult GetCoursesByCollegeInRegistration(int? collegeId)
    {
        try
        {
            if (collegeId == null)
                throw new Exception("College Id cannot be null.");

            collegeId = (int)collegeId;

            var studentEntriesInOnline = _cornoService.StudentRepository.Get(s => 
                s.CollegeId == collegeId);
            if (!studentEntriesInOnline.Any())
                return Json(null, JsonRequestBehavior.AllowGet);

            var courseIds = studentEntriesInOnline.Select(e => e.CourseId);

            // Get course of the selected college
            var studentEntriesInBvdu = (from collegeCourse in _coreService.Tbl_COLLEGE_COURSE_MSTRRepository
                        .Get(c => c.NUM_FK_COLLEGE_CD == collegeId && courseIds.Contains(c.NUM_FK_CO_CD)).AsEnumerable()
                                        join course in _coreService.Tbl_COURSE_MSTR_Repository.Get(c => c.Chr_Close_Flg != "Y")
                                            on collegeCourse.NUM_FK_CO_CD equals course.Num_PK_CO_CD
                                        select new { Id = course.Num_PK_CO_CD, Name = course.Var_CO_NM, NameWithId = "(" + course.Num_PK_CO_CD + ") " + course.Var_CO_NM })
                .OrderByDescending(c => c.Id);

            return Json(studentEntriesInBvdu, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public virtual JsonResult GetCoursesByFacultyAndCollege(int? facultyId, int? collegeId, string filter = default)
    {
        try
        {
            if (collegeId == null)
                throw new Exception("College Id cannot be null.");

            collegeId = (int)collegeId;

            // Get course of the selected college
            var courses = (from collegeCourse in _coreService.Tbl_COLLEGE_COURSE_MSTRRepository.Get(c => c.NUM_FK_COLLEGE_CD == collegeId).AsEnumerable()
                           join course in _coreService.Tbl_COURSE_MSTR_Repository.Get(c => c.Chr_Registration_Active == "Y" &&
                               c.Chr_Close_Flg != "Y") on collegeCourse.NUM_FK_CO_CD equals course.Num_PK_CO_CD
                           select new
                           {
                               Id = course.Num_PK_CO_CD,
                               Name = course.Var_CO_NM,
                               NameWithId = "(" + course.Num_PK_CO_CD + ") " + course.Var_CO_NM,
                               NameWithCode = "(" + course.Num_PK_CO_CD + ") " + course.Var_CO_NM
                           })
                .OrderByDescending(b => b.Id);

            return Json(courses, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    [AllowAnonymous]
    public JsonResult GetCoursesByCollegeAndCourseType(int? collegeId, int? courseTypeId)
    {
        try
        {
            if (collegeId == null)
                throw new Exception("College Id cannot be null.");

            collegeId = (int)collegeId;
            // Get course of the selected college
            var courses = (from collegeCourse in _coreService.Tbl_COLLEGE_COURSE_MSTRRepository
                        .Get(c => c.NUM_FK_COLLEGE_CD == collegeId).AsEnumerable()
                           join course in _coreService.Tbl_COURSE_MSTR_Repository.Get(c => c.Num_FK_TYP_CD == courseTypeId &&
                                   c.Chr_Registration_Active == "Y" &&
                                   c.Chr_Close_Flg != "Y")
                               on collegeCourse.NUM_FK_CO_CD equals course.Num_PK_CO_CD
                           select new { Id = course.Num_PK_CO_CD, Name = course.Var_CO_NM, NameWithId = "(" + course.Num_PK_CO_CD + ") " + course.Var_CO_NM })
                .OrderByDescending(b => b.Id);

            return Json(courses, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public virtual JsonResult GetAllCoursesByCollege(int? collegeId)
    {
        try
        {
            if (collegeId == null)
                throw new Exception("College Id cannot be null.");

            collegeId = (int)collegeId;

            // Get course of the selected college
            var courses = (from collegeCourse in _coreService.Tbl_COLLEGE_COURSE_MSTRRepository.Get(c => c.NUM_FK_COLLEGE_CD == collegeId).AsEnumerable()
                           join course in _coreService.Tbl_COURSE_MSTR_Repository.Get(c => c.Chr_Close_Flg != "Y") on collegeCourse.NUM_FK_CO_CD equals course.Num_PK_CO_CD
                           select new
                           {
                               Id = course.Num_PK_CO_CD,
                               Name = course.Var_CO_NM,
                               NameWithId = "(" + course.Num_PK_CO_CD + ") " + course.Var_CO_NM
                           })
                .OrderByDescending(b => b.Id);

            return Json(courses, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    [AllowAnonymous]
    public JsonResult GetAllCoursesByCollegeAndCourseType(int? collegeId, int? courseTypeId)
    {
        try
        {
            if (collegeId == null)
                throw new Exception("College Id cannot be null.");

            collegeId = (int)collegeId;
            // Get course of the selected college
            var courses = (from collegeCourse in _coreService.Tbl_COLLEGE_COURSE_MSTRRepository.Get(c => c.NUM_FK_COLLEGE_CD == collegeId).AsEnumerable()
                           join course in _coreService.Tbl_COURSE_MSTR_Repository.Get(c => c.Num_FK_TYP_CD == courseTypeId &&
                               c.Chr_Close_Flg != "Y") on collegeCourse.NUM_FK_CO_CD equals course.Num_PK_CO_CD
                           select new
                           {
                               Id = course.Num_PK_CO_CD,
                               Name = course.Var_CO_NM,
                               NameWithId = "(" + course.Num_PK_CO_CD + ") " + course.Var_CO_NM
                           })
                .OrderByDescending(c => c.Id);

            return Json(courses, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public JsonResult GetAllCoursesByCourseType(int? courseTypeId)
    {
        try
        {
            // Get course of the selected college
            var courses = _coreService.Tbl_COURSE_MSTR_Repository.Get(c => c.Num_FK_TYP_CD == courseTypeId &&
                                                                           c.Chr_Close_Flg != "Y").AsEnumerable()
                .Select(c => new
                {
                    Id = c.Num_PK_CO_CD,
                    Name = c.Var_CO_NM,
                    NameWithId = "(" + c.Num_PK_CO_CD + ") " + c.Var_CO_NM
                })
                .OrderByDescending(b => b.Id);

            return Json(courses, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public JsonResult GetTimeTableCourses(int? collegeId, int? courseTypeId)
    {
        try
        {
            if (collegeId == null)
                throw new Exception("College Id cannot be null.");

            collegeId = (int)collegeId;
            // Get course of the selected college
            var courses = (from collegeCourse in _coreService.Tbl_COLLEGE_COURSE_MSTRRepository.Get(c => c.NUM_FK_COLLEGE_CD == collegeId).AsEnumerable()
                           join course in _coreService.Tbl_COURSE_MSTR_Repository.Get(c => c.Num_FK_TYP_CD == courseTypeId
                               && c.Chr_TimeTable_Active == "Y" && c.Chr_Close_Flg != "Y").AsEnumerable()
                               on collegeCourse.NUM_FK_CO_CD equals course.Num_PK_CO_CD
                           select new
                           {
                               Id = course.Num_PK_CO_CD,
                               Name = course.Var_CO_NM,
                               NameWithId = "(" + course.Num_PK_CO_CD + ") " + course.Var_CO_NM
                           })
                .OrderByDescending(c => c.Id);

            return Json(courses, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    [AllowAnonymous]
    public virtual JsonResult GetCoursePartsByCourse(int? courseId, string filter = default, FormType formType = FormType.None)
    {
        try
        {
            if (courseId == null)
                throw new Exception("Course Id cannot be null.");

            courseId = (int)courseId;

            var courseParts = _coreService.Tbl_COURSE_PART_MSTR_Repository
                .Get(c => c.Chr_DELETE_FLG != "Y").AsEnumerable()
                .Where(p => p.Num_FK_CO_CD == courseId)
                .Select(p => new
                {
                    Id = p.Num_PK_COPRT_NO,
                    Name = p.Var_COPRT_DESC,
                    NameWithId = "(" + p.Num_PK_COPRT_NO + ") " + p.Var_COPRT_DESC,
                    p.Num_COPRT_SEMI_NO,
                    p.Chr_DEG_APL_FLG
                })
                .OrderBy(b => b.Id)
                .ToList();

            switch (formType)
            {
                case FormType.Environment:
                    if (courseParts.All(c => c.Num_COPRT_SEMI_NO > 0))
                        courseParts = courseParts.Where(c => c.Num_COPRT_SEMI_NO > 1).ToList();
                    break;
                case FormType.Convocation:
                    courseParts = courseParts.Where(c => c.Chr_DEG_APL_FLG == "Y").ToList();
                    break;
            }

            return Json(courseParts, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public virtual JsonResult GetBranchesByCourse(int? courseId)
    {
        try
        {
            if (null == courseId)
                throw new Exception("Course Id cannot be null.");

            courseId = (int)courseId;

            var branches = _coreService.Tbl_BRANCH_MSTR_Repository.Get().AsEnumerable()
                .Where(p => p.Num_FK_CO_CD == courseId)
                .Select(p => new { Id = p.Num_PK_BR_CD, Name = p.Var_BR_NM, NameWithId = "(" + p.Num_PK_BR_CD + ") " + p.Var_BR_NM })
                .OrderBy(b => b.Id);
            return Json(branches, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    [AllowAnonymous]
    public virtual JsonResult GetBranchesByCoursePart(int? courseId, int? coursePartId, string filter = default)
    {
        try
        {
            var branches = _universityService.GetBranchesByCoursePart(courseId, coursePartId);
            return Json(branches, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    [AllowAnonymous]
    public virtual JsonResult GetSubjects(int? coursePartId, int? branchId = default, string filter = default, bool onlyTheory = false)
    {
        try
        {
            if (null == coursePartId)
                throw new Exception("Course Part Id cannot be null.");

            var coursePart = _coreService.Tbl_COURSE_PART_MSTR_Repository.Get(c => c.Num_PK_COPRT_NO == coursePartId).FirstOrDefault();
            if (coursePart == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            List<Tbl_COPART_SYLLABUS> syllabuses;
            if (coursePart.Chr_COPRT_BRANCH_APP_FLG == "Y" && branchId > 0)
            {
                syllabuses = _coreService.Tbl_COPART_SYLLABUS_Repository.Get(s => s.Num_FK_COPRT_NO == coursePartId &&
                         s.Num_FK_BR_CD == branchId && s.Chr_DELETE_FLG != "Y")
                    .ToList();
            }
            else
            {
                syllabuses = _coreService.Tbl_COPART_SYLLABUS_Repository.Get(s => s.Num_FK_COPRT_NO == coursePartId && 
                        s.Chr_DELETE_FLG != "Y")
                    .ToList();
            }

            if (syllabuses.Count <= 0)
                return Json(null, JsonRequestBehavior.AllowGet);

            var subjects = new List<Tbl_SUBJECT_MSTR>();
            foreach (var syllabus in syllabuses)
            {
                var syllabusTrxs = _coreService.Tbl_COPART_SYLLABUS_TRX_Repository.Get(
                    st => st.Num_FK_SYL_NO == syllabus.Num_PK_SYL_NO &&
                          st.Chr_DELETE_FLG != "Y");

                var syllabusSubjectIds = syllabusTrxs.Select(s => s.Num_FK_SUB_CD);

                var syllabusSubjects = _coreService.Tbl_SUBJECT_MSTR_Repository.Get(s =>
                    s.Num_FK_COPRT_NO == coursePartId && syllabusSubjectIds.Contains(s.Num_PK_SUB_CD)).ToList();

                subjects.AddRange(syllabusSubjects);
            }

            return Json(subjects.Select(p => new
            {
                Id = p.Num_PK_SUB_CD,
                Name = p.Var_SUBJECT_NM,
                NameWithId = "(" + p.Num_PK_SUB_CD + ") " + p.Var_SUBJECT_NM
            }).DistinctBy(s => s.Id), JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    [AllowAnonymous]
    public virtual JsonResult GetSubjectsByCoursePart(int? coursePartId, int? branchId, bool onlyTheory = false, string filter = default)
    {
        try
        {
            if (null == coursePartId)
                throw new Exception("Course Part Id cannot be null.");

            var coursePart = _coreService.Tbl_COURSE_PART_MSTR_Repository.Get(c => c.Num_PK_COPRT_NO == coursePartId).FirstOrDefault();
            if (coursePart == null)
                return Json(null, JsonRequestBehavior.AllowGet);

            var subjects = _coreService.Tbl_SUBJECT_MSTR_Repository
                .Get(s => s.Num_FK_COPRT_NO == coursePartId).AsEnumerable()
                .DistinctBy(s => s.Num_PK_SUB_CD)
                .OrderBy(s => s.Num_PK_SUB_CD);

            if (onlyTheory)
            {
                var subjectIds = subjects.Select(s => s.Num_PK_SUB_CD);
                var categorySubjectIds = _coreService.Tbl_SUBJECT_CAT_MSTR_Repository
                    .Get(s => s.CHR_WRT_UNI_APL == "Y" && subjectIds.Contains(s.Num_FK_SUB_CD))
                    .Select(s => s.Num_FK_SUB_CD).ToList();

                subjects = (IOrderedEnumerable<Tbl_SUBJECT_MSTR>)subjects.Where(s => categorySubjectIds.Contains(s.Num_PK_SUB_CD));
            }

            return Json(subjects.Select(p => new
            {
                Id = p.Num_PK_SUB_CD,
                Name = p.Var_SUBJECT_NM,
                NameWithId = "(" + p.Num_PK_SUB_CD + ") " + p.Var_SUBJECT_NM
            }), JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public virtual JsonResult GetAllCategories()
    {
        try
        {
            var categories = _coreService.Tbl_EVALCAT_MSTR_Repository
                .Get().AsEnumerable()
                .Select(c => new
                {
                    Id = c.Num_PK_CAT_CD,
                    Name = c.Var_CAT_NM,
                    NameWithId = "(" + c.Num_PK_CAT_CD + ") " + c.Var_CAT_NM
                }).DistinctBy(s => s.Id);
            return Json(categories, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    [AllowAnonymous]
    public JsonResult GetCategories(int? coursePartId, int? subjectId)
    {
        try
        {
            if (null == coursePartId || null == subjectId)
                throw new Exception("Course Part Id / Subject Id cannot be null.");

            var categoryIds = _coreService.Tbl_SUBJECT_CAT_MSTR_Repository
                .Get(c => c.Num_FK_COPRT_NO == coursePartId && c.Num_FK_SUB_CD == subjectId)
                .Select(s => s.Num_FK_CAT_CD);

            var categories = _coreService.Tbl_EVALCAT_MSTR_Repository
                .Get(e => categoryIds.Contains(e.Num_PK_CAT_CD)).AsEnumerable()
                .Select(p => new
                {
                    Id = p.Num_PK_CAT_CD,
                    Name = p.Var_CAT_NM,
                    NameWithId = "(" + p.Num_PK_CAT_CD + ") " + p.Var_CAT_NM,
                });

            return Json(categories, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public JsonResult GetTheoryCategories(int? coursePartId, int? subjectId)
    {
        try
        {
            if (null == coursePartId || null == subjectId)
                throw new Exception("Course Part Id / Subject Id cannot be null.");

            var theoryCategoryIds = _coreService.Tbl_SUBJECT_CAT_MSTR_Repository
                .Get(c => c.Num_FK_COPRT_NO == coursePartId && c.Num_FK_SUB_CD == subjectId && c.CHR_WRT_UNI_APL == "Y")
                .Select(s => s.Num_FK_CAT_CD)
                .ToList();

            var categories = _coreService.Tbl_EVALCAT_MSTR_Repository
                .Get(e => theoryCategoryIds.Contains(e.Num_PK_CAT_CD)).AsEnumerable()
                .Select(p => new
                {
                    Id = p.Num_PK_CAT_CD,
                    Name = p.Var_CAT_NM,
                    NameWithId = "(" + p.Num_PK_CAT_CD + ") " + p.Var_CAT_NM,
                });

            return Json(categories, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    [AllowAnonymous]
    public JsonResult GetPapers(int? coursePartId, int? subjectId, int? categoryId)
    {
        try
        {
            if (null == categoryId)
                throw new Exception("Category Id cannot be null.");

            var papers = _coreService.Tbl_SUB_CATPAP_MSTR_Repository
                .Get(c => c.Num_FK_COPRT_NO == coursePartId && 
                          c.Num_FK_SUB_CD == subjectId && c.Num_FK_CAT_CD == categoryId).AsEnumerable()
                .Select(p => new
                {
                    Id = p.Num_PK_PAP_CD,
                    Name = p.Var_PAP_NM,
                    NameWithId = "(" + p.Num_PK_PAP_CD + ") " + p.Var_PAP_NM,
                    MaxMarks = p.Num_PAP_MAX_MRK
                });
            return Json(!papers.Any() ? null : papers, JsonRequestBehavior.AllowGet);
        }
        catch (Exception exception)
        {
            LogHandler.LogError(exception);
            return Json(null, JsonRequestBehavior.AllowGet);
        }
    }

    public List<MasterViewModel> GetQuestionTypeModels()
    {
        var questionTypes = new List<MasterViewModel>
        {
            new() {Id = (int)QuestionType.LongAnswer, Name = QuestionType.LongAnswer.ToString().SplitPascalCase()},
            new() {Id = (int)QuestionType.ShortAnswer, Name = QuestionType.ShortAnswer.ToString().SplitPascalCase()},
            new() {Id = (int)QuestionType.ShortNotes, Name = QuestionType.ShortNotes.ToString().SplitPascalCase()},
            new() {Id = (int)QuestionType.Mcq, Name = QuestionType.Mcq.ToString().ToUpper()},
            new() {Id = (int)QuestionType.FillInTheBlanks, Name = QuestionType.FillInTheBlanks.ToString().SplitPascalCase()},
            new() {Id = (int)QuestionType.StateTrueOrFalse, Name = QuestionType.StateTrueOrFalse.ToString().SplitPascalCase()},
            new() {Id = (int)QuestionType.MatchThePair, Name = QuestionType.MatchThePair.ToString().SplitPascalCase()},
            new() {Id = (int)QuestionType.CriticalAppreciation, Name = QuestionType.CriticalAppreciation.ToString().SplitPascalCase()},
        };

        return questionTypes;
    }

    protected string GetTransactionId()
    {
        return DateTime.Now.ToString("ddMMyyhhmmssfff");
    }

    [HttpPost]
    public ActionResult Excel_Export_Save(string contentType, string base64, string fileName)
    {
        var fileContents = Convert.FromBase64String(base64);

        return File(fileContents, contentType, fileName);
    }
}

#endregion