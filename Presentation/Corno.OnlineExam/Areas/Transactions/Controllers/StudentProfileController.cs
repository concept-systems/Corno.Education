using Corno.OnlineExam.Controllers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;
using Corno.Data.Corno;
using Corno.Services.Core.Interfaces;

namespace Corno.OnlineExam.Areas.Transactions.Controllers;

[Authorize]
public class StudentProfileController : BaseController
{
    #region -- Construtors --
    public StudentProfileController(ICoreService examService)
    {
        _examService = examService;
    }
    #endregion

    #region -- Data Members --

    private readonly ICoreService _examService;
    #endregion

    #region -- Methods --

    private void GetProfile(StudentProfile profile)
    {
        profile.StudentProfileDetails ??= new List<StudentProfileDetail>();

        // Fill the View Model
        FillViewModel(profile);

        var profileDetails =
            (from studentExam in _examService.TBL_STUDENT_EXAMS_Repository.Get(e => e.Chr_FK_PRN_NO.Trim() == profile.Prn.Trim())
             join yrChange in _examService.Tbl_STUDENT_YR_CHNG_Repository.Get(e => e.Chr_FK_PRN_NO.Trim() == profile.Prn.Trim())
                 on new { studentExam.Num_FK_INST_NO, studentExam.Chr_FK_PRN_NO } equals new { yrChange.Num_FK_INST_NO, yrChange.Chr_FK_PRN_NO }
             join center in _examService.TBL_DISTANCE_CENTERS_Repository.Get()
                 on yrChange.Num_FK_DistCenter_ID equals center.Num_PK_DistCenter_ID into defaultCenter
             from center in defaultCenter.DefaultIfEmpty()
             join college in _examService.TBL_COLLEGE_MSTRRepository.Get()
                 on yrChange.Num_FK_COL_CD equals college.Num_PK_COLLEGE_CD into defaultCollege
             from college in defaultCollege.DefaultIfEmpty()
             join course in _examService.Tbl_COURSE_MSTR_Repository.Get()
                 on studentExam.Num_FK_CO_CD equals course.Num_PK_CO_CD into defaultCourse
             from course in defaultCourse.DefaultIfEmpty()
             join coursePart in _examService.Tbl_COURSE_PART_MSTR_Repository.Get()
                 on studentExam.Num_FK_COPRT_NO equals coursePart.Num_PK_COPRT_NO into defaultCoursePart
             from coursePart in defaultCoursePart
             join branch in _examService.Tbl_BRANCH_MSTR_Repository.Get()
                 on studentExam.Num_FK_BR_CD equals branch.Num_PK_BR_CD into defaultBranch
             from branch in defaultBranch.DefaultIfEmpty()
             join instance in _examService.Tbl_SYS_INST_Repository.Get()
                 on studentExam.Num_FK_INST_NO equals instance.Num_PK_INST_SRNO into defaultInstance
             from instance in defaultInstance.DefaultIfEmpty()
             join cgpa in _examService.TBL_STUDENT_CGPA_Repository.Get()
                 on new { studentExam.Chr_FK_PRN_NO, Num_FK_INST_NO = (short?)studentExam.Num_FK_INST_NO, Num_FK_COPRT_NO = (short?)studentExam.Num_FK_COPRT_NO } equals
                    new { cgpa.Chr_FK_PRN_NO, cgpa.Num_FK_INST_NO, cgpa.Num_FK_COPRT_NO } into defaultCgpa
             from cgpa in defaultCgpa.DefaultIfEmpty()
             select new StudentProfileDetail
             {
                 InstanceName = instance.Var_INST_REM,
                 Exam = instance.Var_INST_REM,
                 SeatNo = studentExam.Num_ST_SEAT_NO.ToString(),
                 CoursePartName = coursePart.Var_COPRT_SHRT_NM,
                 CollegeName = college.Var_CL_COLLEGE_NM1,
                 Result = studentExam.Chr_ST_YR_RES,
                 BranchName = branch.Var_BR_NM,
                 ImpFlg = coursePart.Chr_ATKT_APL_FLG,
                 CentreName = center.DIST_CENT_NAME,
                 Cgpa = (double?)((cgpa.Num_CGPA_AVG ?? 0) + (cgpa.Num_CGPA_GRACE ?? 0)),
                 CourseName = course.Var_CO_NM,
             });

        foreach (var detail in profileDetails)
            profile.StudentProfileDetails.Add(detail);
    }
    private void FillViewModel(StudentProfile profile)
    {
        var student = _examService.Tbl_STUDENT_INFO_ADR_Repository.Get(s => s.Chr_FK_PRN_NO == profile.Prn).FirstOrDefault();
        var studentInfo = _examService.Tbl_STUDENT_INFO_Repository.Get(s => s.Chr_PK_PRN_NO == profile.Prn).FirstOrDefault();
        if (null == studentInfo)
            throw new Exception("Invalid PRN");

        profile.StudentName = studentInfo.Var_ST_NM;
        //profile.DateOfBirth = studentInfo.Dtm_ST_DOB_DT + "/" + studentInfo.Dtm_ST_DOB_MONTH + "/" + studentInfo.Dtm_ST_DOB_YEAR;
        profile.Photo = ExamServerHelper.GetPhoto(_examService, profile.Prn);
        if (student != null)
            profile.Address = student.Chr_ST_ADD1 + "" + student.Chr_ST_ADD2 + "" + student.Chr_ST_CITY + "" +
                              student.Chr_ST_DISTRICT;
    }
    #endregion

    #region -- Events --

    // GET: /StudentProfile/Create
    [Authorize]
    public ActionResult Create()
    {
        return View(new StudentProfile());
    }

    // POST: /StudentProfile/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public ActionResult Create(StudentProfile model, string submitType)
    {
        try
        {
            switch (submitType)
            {
                case "Search":
                    GetProfile(model);
                    ModelState.Clear();
                    return View(model);
            }

            if (ModelState.IsValid)
            {

            }
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(model);
    }

    #endregion
}