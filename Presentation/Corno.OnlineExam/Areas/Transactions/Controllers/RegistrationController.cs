using Corno.Globals.Constants;
using Corno.OnlineExam.Controllers;
using Microsoft.AspNet.Identity;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Transactions;
using System.Web.Mvc;
using Corno.Data.Core;
using Corno.Data.Corno;
using Corno.Logger;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;

namespace Corno.OnlineExam.Areas.Transactions.Controllers;

[Authorize]
public class RegistrationController : UniversityController
{
    #region -- Constructors --
    public RegistrationController(ICornoService cornoService, ICoreService coreService)
    {
        _cornoService = cornoService;
        _coreService = coreService;
    }
    #endregion

    #region -- Data Members --
    private readonly ICornoService _cornoService;
    private readonly ICoreService _coreService;
    #endregion


    [Authorize]
    public ActionResult Index(int collegeId = 0, int courseId = 0)
    {
        IEnumerable<Registration> students;

        var instanceId = Convert.ToInt16(HttpContext.Session[ModelConstants.InstanceId].ToString());
        if (User.IsInRole(ModelConstants.College))
        {
            collegeId = (int)HttpContext.Session[ModelConstants.CollegeId];
            students = _cornoService.StudentRepository.Get(t => t.CollegeId == collegeId && t.CourseId == courseId && t.InstanceId == instanceId).ToList();
        }
        else
        {
            students = _cornoService.StudentRepository.Get(t => t.CollegeId == collegeId && t.CourseId == courseId && t.InstanceId == instanceId).ToList();
        }

        var exams = new List<RegistrationIndex>();
        foreach (var student in students)
        {
            var college = _coreService.TBL_COLLEGE_MSTRRepository.Get(c => c.Num_PK_COLLEGE_CD == student.CollegeId).FirstOrDefault();
            var course = _coreService.Tbl_COURSE_MSTR_Repository.Get(c => c.Num_PK_CO_CD == student.CourseId).FirstOrDefault();

            if (student.Id == null) continue;
            if (college == null) continue;
            if (course == null) continue;

            var studentIndex = new RegistrationIndex
            {
                Id = (int)student.Id,
                PrnNo = student.PrnNo,
                Status = student.Status,
                CollegeName = college.Var_CL_COLLEGE_NM1,
                StudentName = student.StudentName,
                CourseName = course.Var_CO_NM,
                Gender = student.Gender,
                IsApproved = student.IsApproved,
                ModifiedDate = student.ModifiedDate,
                Dob = student.Dob
            };

            exams.Add(studentIndex);
        }
        return View(exams);
    }

    // GET: /Reg/Details/5
    [Authorize]
    public ActionResult Details(int? id, string prn)
    {
        try
        {
            return View(GetDetailViewModel(id, prn));
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        return View(new RegistrationViewModel());
    }

    [AllowAnonymous]
    public ActionResult DetailsDirect(int? id, string prn)
    {
        return RedirectToAction("NotFound", "Error");
    }

    // GET: /Reg/Create
    [Authorize]
    public ActionResult Create()
    {
        var viewModel = new RegistrationViewModel();
        return View(viewModel);
    }

    // POST: /Reg/Create
    [HttpPost]
    public ActionResult Create(RegistrationViewModel viewModel)
    {
        try
        {
            var instanceId = Convert.ToInt16(HttpContext.Session[ModelConstants.InstanceId].ToString());
            viewModel.InstanceId = instanceId;
            if (ModelState.IsValid && ValidateViewModel(viewModel))
            {
                var model = AutoMapperConfig.CornoMapper.Map<Registration>(viewModel);
                var studentCourse = _coreService.Tbl_STUDENT_COURSE_Repository.Get(s => s.Chr_FK_PRN_NO == model.PrnNo).FirstOrDefault();
                if (null != studentCourse)
                {
                    model.CollegeId = studentCourse.Num_ST_COLLEGE_CD;
                    model.CourseId = studentCourse.Num_FK_CO_CD;
                }

                if (model.CollegeId != 28)
                    model.CentreId = 0;

                //Photo
                if (null != viewModel.UploadPhoto)
                {
                    model.Photo = new byte[viewModel.UploadPhoto.ContentLength];
                    viewModel.UploadPhoto.InputStream.Read(model.Photo, 0, model.Photo.Length);
                }
                //Document1
                if (null != viewModel.UploadDocument1)
                {
                    model.Document1 = new byte[viewModel.UploadDocument1.ContentLength];
                    viewModel.UploadDocument1.InputStream.Read(model.Document1, 0, model.Document1.Length);
                }
                //Document2
                if (null != viewModel.UploadDocument2)
                {
                    model.Document2 = new byte[viewModel.UploadDocument2.ContentLength];
                    viewModel.UploadDocument2.InputStream.Read(model.Document2, 0, model.Document2.Length);
                }
                //Document3
                if (null != viewModel.UploadDocument3)
                {
                    model.Document3 = new byte[viewModel.UploadDocument3.ContentLength];
                    viewModel.UploadDocument3.InputStream.Read(model.Document3, 0, model.Document3.Length);
                }
                //Document4
                if (null != viewModel.UploadDocument4)
                {
                    model.Document4 = new byte[viewModel.UploadDocument4.ContentLength];
                    viewModel.UploadDocument4.InputStream.Read(model.Document4, 0, model.Document4.Length);
                }
                //Document5
                if (null != viewModel.UploadDocument5)
                {
                    model.Document5 = new byte[viewModel.UploadDocument5.ContentLength];
                    viewModel.UploadDocument5.InputStream.Read(model.Document5, 0, model.Document5.Length);
                }

                //Student model = Mapper.Map<Student>(viewModel);
                model.CreatedBy = User.Identity.GetUserId();
                model.CreatedDate = DateTime.Now;
                //string prnNo = GeneratePRN(ref model);

                // Generate PRN
                // PRN : Year = 2 digits, College = 2 digits, Faculty = 2 digits, Serial No = 4 digits
                // Add 2 digits year 
                var prnNo = model.Year.ToString().Substring(2, 2);
                // Add 2 digits college
                prnNo += model.CollegeId.ToString().PadLeft(2, '0');
                // Add 2 digits faculty
                //model.FacultyId = _coreService.Tbl_COURSE_MSTR_Repository.GetById(model.CourseId).Num_FK_FA_CD;
                model.FacultyId = _coreService.Tbl_COURSE_MSTR_Repository.FirstOrDefault(p =>
                    p.Num_PK_CO_CD == model.CourseId, p => p).Num_FK_FA_CD;
                prnNo += model.FacultyId.ToString().PadLeft(2, '0');
                if (model.Year != null)
                {
                    var year = (int)model.Year;
                    var facultyId = (int)model.FacultyId;
                    if (model.CollegeId != null)
                    {
                        var collegeId = (int)model.CollegeId;
                        var lastPrnSerialNo = _cornoService.StudentRepository
                            .Get(p => p.Year == year && p.FacultyId == facultyId &&
                                      p.CollegeId == collegeId)
                            .Max(p => p.PrnSerialNo) ?? 0;
                        if (lastPrnSerialNo <= 0)
                            model.PrnSerialNo = 1;
                        else
                            model.PrnSerialNo = lastPrnSerialNo + 1;
                    }
                }
                // Add PRN Serial No.
                prnNo += model.PrnSerialNo.ToString().PadLeft(4, '0');
                model.PrnNo = prnNo;

                model.StudentName = model.StudentName.ToUpper();
                if (null != model.FatherName)
                    model.FatherName = model.FatherName.ToUpper();
                if (null != model.MotherName)
                    model.MotherName = model.MotherName.ToUpper();
                if (null != model.GuardianName)
                    model.GuardianName = model.GuardianName.ToUpper();
                if (null != model.CityName)
                    model.CityName = model.CityName.ToUpper();
                if (null != model.StateName)
                    model.StateName = model.StateName.ToUpper();
                if (null != model.CountryName)
                    model.CountryName = model.CountryName.ToUpper();
                if (null != model.CityName1)
                    model.CityName1 = model.CityName1.ToUpper();
                if (null != model.CountryName1)
                    model.CountryName1 = model.CountryName1.ToUpper();
                if (null != model.StateName1)
                    model.StateName1 = model.StateName1.ToUpper();
                if (null != model.EnrollmentNo)
                    model.EnrollmentNo = model.EnrollmentNo;

                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
                {
                    try
                    {
                        model.InstanceId = instanceId;
                        _cornoService.StudentRepository.Add(model);
                        _cornoService.Save();

                        // Add Student To Exam Server
                        AddStudentInExamServer(model);

                        scope.Complete();
                        TempData["Success"] = " Registration Successful and your PRN is " + model.PrnNo;
                    }
                    catch (Exception exception)
                    {
                        scope.Dispose();
                        LogHandler.LogError(exception);
                        throw;
                    }
                }
                return RedirectToAction("Create");
            }
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(viewModel);
    }

    // GET: /Reg/Edit/5
    [Authorize]
    public ActionResult Edit(int? id, string prn = null)
    {
        try
        {
            var viewModel = EditStudent(id, prn);
            return View(viewModel);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(new RegistrationViewModel());
    }

    [AllowAnonymous]
    public ActionResult EditDirect(int? id, string prn = null)
    {
        return RedirectToAction("NotFound", "Error");
        //try
        //{
        //    if (!string.IsNullOrEmpty(prn))
        //    {
        //        var viewModel = EditStudent(id, prn);
        //        return View(viewModel);
        //    }
        //}
        //catch (Exception exception)
        //{
        //    HandleControllerException(exception);
        //}
        //return View(new RegistrationViewModel());
    }

    // POST: /Reg/Edit/5
    [HttpPost]
    public ActionResult Edit(RegistrationViewModel viewModel)
    {
        try
        {
            if (ModelState.IsValid)
            {
                // Edit student
                EditStudent(viewModel);

                return RedirectToAction(@"Create");
            }
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }
        return View(viewModel);
    }

    [AllowAnonymous]
    [HttpPost]
    public ActionResult EditDirect(RegistrationViewModel viewModel)
    {
        return RedirectToAction("NotFound", "Error");
        //try
        //{
        //    if (ModelState.IsValid)
        //    {
        //        // Edit student
        //        EditStudent(viewModel);

        //        return RedirectToAction(@"EditDirect");
        //    }
        //}
        //catch (Exception exception)
        //{
        //    HandleControllerException(exception);
        //}
        //return View(viewModel);
    }

    //
    // GET: /Reg/Delete/5
    [Authorize]
    public ActionResult Delete(int? id)
    {
        RegistrationViewModel viewModel = null;
        try
        {
            if (id == null)
            {
                return new HttpStatusCodeResult(HttpStatusCode.BadRequest);
            }
            var model = _cornoService.StudentRepository.GetById(id);
            viewModel = AutoMapperConfig.CornoMapper.Map<RegistrationViewModel>(model);
        }
        catch (Exception exception)
        {
            HandleControllerException(exception);
        }

        if (viewModel == null)
            return null;

        if (viewModel.CollegeId != null)
            viewModel.CollegeName = ExamServerHelper.GetCollegeName((int)viewModel.CollegeId, _coreService);
        viewModel.CentreName = null != viewModel.CentreId ? ExamServerHelper.GetCentreName((int)viewModel.CentreId, _coreService) : "-";
        if (viewModel.CoursePartId != null)
        {
            viewModel.CourseTypeName = ExamServerHelper.GetCourseTypeName((int)viewModel.CoursePartId, _coreService);
            viewModel.CourseName = ExamServerHelper.GetCourseNameFromCoursePartId((int)viewModel.CoursePartId, _coreService);
            viewModel.CoursePartName = ExamServerHelper.GetCoursePartName((int)viewModel.CoursePartId, _coreService);
        }
        if (viewModel.BranchId != null && viewModel.BranchId > 0)
            viewModel.BranchName = ExamServerHelper.GetBranchName((int)viewModel.BranchId, _coreService);

        return View(viewModel);
    }

    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    [Authorize]
    public ActionResult DeleteConfirmed(int id)
    {
        var form = _cornoService.StudentRepository.GetById(id);
        form.DeletedBy = User.Identity.GetUserId();
        form.DeletedDate = DateTime.Now;

        form.Status = StatusConstants.Deleted;
        _cornoService.StudentRepository.Update(form);
        _cornoService.Save();
        return RedirectToAction("Index");

    }

    #region Methods
    // Get Method
    private RegistrationViewModel EditStudent(int? id, string prn)
    {
        if (id == null && string.IsNullOrEmpty(prn))
            throw new Exception("Invalid Id or PRN 1");

        Registration model = null;
        if (null != id)
            model = _cornoService.StudentRepository.GetById(id);
        if (!string.IsNullOrEmpty(prn))
            model = _cornoService.StudentRepository.Get(r => r.PrnNo == prn).FirstOrDefault();

        if (null == model)
            throw new Exception("Invalid Id or PRN 2");

        var viewModel = AutoMapperConfig.CornoMapper.Map<RegistrationViewModel>(model);
        if (viewModel == null)
            throw new Exception("Invalid Id or PRN 3");

        model.StudentName = model.StudentName.ToUpper();
        if (null != model.FatherName)
            model.FatherName = model.FatherName.ToUpper();
        if (null != model.MotherName)
            model.MotherName = model.MotherName.ToUpper();
        if (null != model.GuardianName)
            model.GuardianName = model.GuardianName.ToUpper();
        if (null != model.CityName)
            model.CityName = model.CityName.ToUpper();
        if (null != model.StateName)
            model.StateName = model.StateName.ToUpper();
        if (null != model.CountryName)
            model.CountryName = model.CountryName.ToUpper();
        if (null != model.CityName1)
            model.CityName1 = model.CityName1.ToUpper();
        if (null != model.CountryName1)
            model.CountryName1 = model.CountryName1.ToUpper();
        if (null != model.StateName1)
            model.StateName1 = model.StateName1.ToUpper();

        return viewModel;
    }

    // Post Method
    private void EditStudent(RegistrationViewModel viewModel)
    {
        var model = _cornoService.StudentRepository.Get(r =>
            r.PrnNo == viewModel.PrnNo).FirstOrDefault();
        if (null == model)
            throw new Exception("Invalid PRN");

        var id = model.Id;
        var instanceId = model.InstanceId;
        var collegeId = model.CollegeId;
        var centerId = model.CentreId;
        var courseTypeId = model.CourseTypeId;
        var courseId = model.CourseId;
        var coursePartId = model.CoursePartId;
        var branchId = model.BranchId;
        model = AutoMapperConfig.CornoMapper.Map<Registration>(viewModel);
        model.Id = id;
        model.InstanceId = instanceId;
        model.CollegeId = collegeId;
        model.CentreId = centerId;
        model.CourseTypeId = courseTypeId;
        model.CourseId = courseId;
        model.CoursePartId = coursePartId;
        model.BranchId = branchId;

        var studentCourse = _coreService.Tbl_STUDENT_COURSE_Repository.Get(s => s.Chr_FK_PRN_NO == model.PrnNo).FirstOrDefault();
        if (null != studentCourse)
        {
            model.CollegeId = studentCourse.Num_ST_COLLEGE_CD;
            model.CourseId = studentCourse.Num_FK_CO_CD;
        }

        if (model.CollegeId != 28)
            model.CentreId = 0;

        //Photo
        if (null != viewModel.UploadPhoto)
        {
            model.Photo = new byte[viewModel.UploadPhoto.ContentLength];
            viewModel.UploadPhoto.InputStream.Read(model.Photo, 0, model.Photo.Length);
        }

        //Document1
        if (null != viewModel.UploadDocument1)
        {
            model.Document1 = new byte[viewModel.UploadDocument1.ContentLength];
            viewModel.UploadDocument1.InputStream.Read(model.Document1, 0, model.Document1.Length);
        }

        //Document2
        if (null != viewModel.UploadDocument2)
        {
            model.Document2 = new byte[viewModel.UploadDocument2.ContentLength];
            viewModel.UploadDocument2.InputStream.Read(model.Document2, 0, model.Document2.Length);
        }

        //Document3
        if (null != viewModel.UploadDocument3)
        {
            model.Document3 = new byte[viewModel.UploadDocument3.ContentLength];
            viewModel.UploadDocument3.InputStream.Read(model.Document3, 0, model.Document3.Length);
        }
        //Document4
        if (null != viewModel.UploadDocument4)
        {
            model.Document4 = new byte[viewModel.UploadDocument4.ContentLength];
            viewModel.UploadDocument4.InputStream.Read(model.Document4, 0, model.Document4.Length);
        }
        //Document5
        if (null != viewModel.UploadDocument5)
        {
            model.Document5 = new byte[viewModel.UploadDocument5.ContentLength];
            viewModel.UploadDocument5.InputStream.Read(model.Document5, 0, model.Document5.Length);
        }

        model.ModifiedBy = User.Identity.GetUserId();
        model.ModifiedDate = DateTime.Now;

        using var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout });
        try
        {
            _cornoService.StudentRepository.Update(model);
            _cornoService.Save();

            // Edit in Core database
            EditStudentInRegTemp(model);
            EditStudentInStudentInfo(model);

            scope.Complete();
            TempData["Success"] = @"Update Successful for PRN : " + model.PrnNo;
        }
        catch (Exception exception)
        {
            scope.Dispose();
            LogHandler.LogError(exception);
            throw;
        }
    }

    private RegistrationViewModel GetDetailViewModel(int? id, string prn)
    {
        if (id == null && string.IsNullOrEmpty(prn))
            throw new Exception("Invalid Id or PRN 4");
        Registration model = null;
        if (null != id)
            model = _cornoService.StudentRepository.GetById(id);
        if (!string.IsNullOrEmpty(prn))
            model = _cornoService.StudentRepository.Get(r => r.PrnNo == prn).FirstOrDefault();

        var viewModel = AutoMapperConfig.CornoMapper.Map<RegistrationViewModel>(model);

        if (viewModel == null)
            return null;

        if (viewModel.CollegeId != null)
            viewModel.CollegeName = ExamServerHelper.GetCollegeName((int)viewModel.CollegeId, _coreService);
        viewModel.CentreName = null != viewModel.CentreId ? ExamServerHelper.GetCentreName((int)viewModel.CentreId, _coreService) : "-";
        if (viewModel.CoursePartId != null)
        {
            viewModel.CourseTypeName = ExamServerHelper.GetCourseTypeName((int)viewModel.CoursePartId, _coreService);
            viewModel.CourseName = ExamServerHelper.GetCourseNameFromCoursePartId((int)viewModel.CoursePartId, _coreService);
            viewModel.CoursePartName = ExamServerHelper.GetCoursePartName((int)viewModel.CoursePartId, _coreService);
        }
        if (viewModel.BranchId != null && viewModel.BranchId > 0)
            viewModel.BranchName = ExamServerHelper.GetBranchName((int)viewModel.BranchId, _coreService);

        return viewModel;
    }

    public JsonResult GetIndexColleges()
    {
        if (User.IsInRole(ModelConstants.College))
        {
            var collegeId = (int)HttpContext.Session[ModelConstants.CollegeId];
            var singleCollege = _coreService.TBL_COLLEGE_MSTRRepository.Get(c => c.Num_PK_COLLEGE_CD == collegeId)
                .Select(c => new
                {
                    Id = c.Num_PK_COLLEGE_CD,
                    Name = c.Var_CL_COLLEGE_NM1,
                    NameWithId = "(" + c.Num_PK_COLLEGE_CD + ") " + c.Var_CL_COLLEGE_NM1
                })
                .ToList()
                .Distinct()
                .OrderBy(c => c.Id);
            return Json(singleCollege, JsonRequestBehavior.AllowGet);
        }

        var allColleges = (from exam in _cornoService.StudentRepository.Get().ToList()
                           join college in _coreService.TBL_COLLEGE_MSTRRepository.Get().ToList()
                               on exam.CollegeId equals college.Num_PK_COLLEGE_CD
                           select new
                           {
                               Id = college.Num_PK_COLLEGE_CD,
                               Name = college.Var_CL_COLLEGE_NM1,
                               NameWithId = "(" + college.Num_PK_COLLEGE_CD + ") " + college.Var_CL_COLLEGE_NM1
                           })
            .ToList()
            .Distinct()
            .OrderBy(c => c.Id);
        return Json(allColleges, JsonRequestBehavior.AllowGet);
    }

    #endregion

    private void AddStudentInExamServer(RegistrationCommon model)
    {
        var registration = new Tbl_REG_TEMP
        {
            Num_FK_INST_NO = (short)(model.InstanceId ?? 0),
            Num_FK_DistCenter_ID = (short?)model.CentreId ?? 0,
            Chr_REG_COLLEGE_CD = model.CollegeId?.ToString(),
            Chr_REG_COPRT_NO = model.CoursePartId?.ToString(),
            Chr_REG_BR_CD = null == model.BranchId ? "0   " : model.BranchId?.ToString(),
            Chr_REG_ST_NM = model.StudentName,
            Chr_REG_FATH_NM = model.FatherName,
            Chr_REG_MOTH_NM = model.MotherName,
            Chr_REG_SEX_CD = model.Gender,
            Chr_REG_PRN_NO = model.PrnNo,
            Chr_REG_ADD1 = model.LocalAddress,
            Chr_REG_DISTRICT = model.DomicileDistrictName,
            Chr_USR_NM = User.Identity.Name,
            Dtm_DTE_CR = DateTime.Now,
            Chr_REG_VALID_FLG = "V",
            Chr_PK_FORM_NO = model.FormNo?.ToString(),
            Var_PASSPORT = model.PassportNo,
            Chr_REG_YEAR = model.Year?.ToString(),
            Chr_REG_MONTH = model.Month?.ToString(),
            Ima_REG_PHOTO = model.Photo,
            Num_MOBILE = model.Mobile1,
            Chr_Student_Email = model.StudentEmail,
            Num_PHONE = model.ParentMobileNo,
            Var_OLD_PRN_NO = model.OldPrnNo,
            Var_REG_CASTE = model.Caste,
            Var_REG_RELIGION = model.Religion,
            Chr_REG_DOB_DT = model.Dob?.Day.ToString() ?? "0",
            Chr_REG_DOB_MONTH = model.Dob?.Month.ToString() ?? "0",
            Chr_REG_DOB_YEAR = model.Dob?.Year.ToString() ?? "0",
            Chr_REG_CITY = model.CityName,
            Chr_GUARDIAN = model.GuardianName,
            Chr_GUARDIAN_ADD = model.LocalGuardianAddress,
            Num_PHONE_GRDAN = model.GuardianMobileNo,
            Chr_PERMANENT_ADD = model.PermanentAddress,
            Chr_PREV_EXAM = model.ExamName,
            Chr_PREV_EXAM_MON_YR = model.LastPassingYear?.ToString(),
            Chr_PREV_EXAM_PERCENT = model.LastPercentage?.ToString(),
            Chr_PREV_EXAM_SEATNO = model.LastSeatNo?.ToString(),
            Chr_PREV_EXAM_UNI = model.LastUniversity,
            REGISTER_NO = model.RegisterNo,
            ENROLLMENT_NO = model.EnrollmentNo,
            PAN = model.Pan,
            AadharNo = model.AdhaarNo,
            ADMISSION_DATE = model.AdmissionDate,
            LEAVING_DATE = model.LeavingDate,
            RENEWED_PASSPORT_NO = model.RenewedPassportNo,
            PASSPORT_ISSUED_ON = model.PassportIssuedOn,
            RENEWED_PASSPORT_ISSUED_ON = model.RenewedPassportIssuedOn,
            VISA_NO = model.VisaNo,
            VISA_ISSUED_ON = model.VisaIssuedOn,
            IS_ADMISSION_CANCELLED = model.IsAdmissionCancelled,
            ADMISSION_CANCELLED_DATE = model.AdmissionCancelledDate,
            SSC_TOTAL_MARKS = model.SscTotalMarks,
            HSC_TOTAL_MARKS = model.HscTotalMarks,
            UG_TOTAL_MARKS = model.UgTotalMarks,
            SSC_MARKS_OUT_OF = model.SscMarksOutOf,
            HSC_MARKS_OUT_OF = model.HscMarksOutOf,
            UG_MARKS_OUT_OF = model.UgMarksOutOf,
            Chr_REG_CAST_CD = "0",
            Chr_MIGRATION_FLG = "N",
            Chr_DIST_EDU = "N",
            Chr_Erp_No = model.ErpNo,
            Chr_Abc_Id = model.AbcId,
        };

        if (null == model.Nationality || model.Nationality == "Indian")
            registration.Chr_STUDENT_NATIONALITY = "N";
        else
        {
            registration.Chr_STUDENT_NATIONALITY = "Y";
            registration.Var_FOREIGN_ADD = model.Nationality;
        }

        _coreService.REG_TEMP_Repository.Add(registration);
        _coreService.Save();
    }

    private void EditStudentInRegTemp(Registration model)
    {
        var instanceId = Convert.ToInt16(HttpContext.Session[ModelConstants.InstanceId].ToString());
        var regTemp = _coreService.REG_TEMP_Repository.Get(r =>
            r.Num_FK_INST_NO == instanceId &&
            r.Chr_REG_PRN_NO.Trim() == model.PrnNo, p => p).FirstOrDefault();
        if (regTemp == null)
            throw new Exception("No student found in exam server for InstanceId: " + instanceId + " and PRN No: " + model.PrnNo);

        /*var regTemp = _coreService.REG_TEMP_Repository.GetById(tblRegTemp.Num_PK_RECORD_NO);
        if (null == regTemp)
            throw new Exception($"No student found in exam server for Record No: {tblRegTemp.Num_PK_RECORD_NO}");*/

        //registration.Num_FK_INST_NO = Convert.ToInt16(HttpContext.Session[ModelConstants.InstanceId].ToString());
        if (model.CentreId != null)
            regTemp.Num_FK_DistCenter_ID = (short)model.CentreId;
        regTemp.Chr_REG_COLLEGE_CD = model.CollegeId?.ToString();
        regTemp.Chr_REG_COPRT_NO = model.CoursePartId?.ToString();
        regTemp.Chr_REG_BR_CD = null == model.BranchId ? "0   " : model.BranchId?.ToString();

        regTemp.Chr_REG_ST_NM = model.StudentName;
        regTemp.Chr_REG_FATH_NM = model.FatherName;
        regTemp.Chr_REG_MOTH_NM = model.MotherName;
        regTemp.Chr_REG_SEX_CD = model.Gender;
        regTemp.Chr_REG_PRN_NO = model.PrnNo;

        regTemp.Chr_REG_ADD1 = model.LocalAddress;
        regTemp.Chr_REG_DISTRICT = model.DomicileDistrictName;
        regTemp.Chr_USR_NM = User.Identity.Name;
        regTemp.Dtm_DTE_UP = DateTime.Now;
        regTemp.Chr_REG_VALID_FLG = "V";

        regTemp.Chr_PK_FORM_NO = model.FormNo?.ToString();
        if (model.Nationality == "Indian")
            regTemp.Chr_STUDENT_NATIONALITY = "N";
        else
        {
            regTemp.Chr_STUDENT_NATIONALITY = "Y";
            regTemp.Var_FOREIGN_ADD = model.Nationality;
        }
        regTemp.Var_PASSPORT = model.PassportNo;
        regTemp.Chr_REG_YEAR = model.Year?.ToString();
        regTemp.Chr_REG_MONTH = model.Month?.ToString();
        regTemp.Ima_REG_PHOTO = model.Photo;
        regTemp.Num_MOBILE = model.Mobile1;
        regTemp.Chr_Student_Email = model.StudentEmail;
        regTemp.Var_OLD_PRN_NO = model.OldPrnNo;
        regTemp.Var_REG_CASTE = model.Caste;
        regTemp.Var_REG_RELIGION = model.Religion;
        if (model.Dob != null)
        {
            regTemp.Chr_REG_DOB_DT = ((DateTime)model.Dob).Day.ToString();
            regTemp.Chr_REG_DOB_MONTH = ((DateTime)model.Dob).Month.ToString();
            regTemp.Chr_REG_DOB_YEAR = ((DateTime)model.Dob).Year.ToString();
        }
        regTemp.Chr_REG_CITY = model.CityName;
        regTemp.Chr_GUARDIAN = model.GuardianName;
        regTemp.Chr_GUARDIAN_ADD = model.LocalGuardianAddress;
        regTemp.Num_PHONE_GRDAN = model.GuardianMobileNo;
        regTemp.Chr_PERMANENT_ADD = model.PermanentAddress;
        regTemp.Chr_PREV_EXAM = model.ExamName;
        regTemp.Chr_PREV_EXAM_MON_YR = model.LastPassingYear?.ToString();
        regTemp.Chr_PREV_EXAM_PERCENT = model.LastPercentage?.ToString();
        regTemp.Chr_PREV_EXAM_SEATNO = model.LastSeatNo?.ToString();
        regTemp.Chr_PREV_EXAM_UNI = model.LastUniversity;

        regTemp.REGISTER_NO = model.RegisterNo;
        regTemp.ENROLLMENT_NO = model.EnrollmentNo;
        regTemp.PAN = model.Pan;
        regTemp.AadharNo = model.AdhaarNo;
        regTemp.ADMISSION_DATE = model.AdmissionDate;
        regTemp.LEAVING_DATE = model.LeavingDate;
        regTemp.RENEWED_PASSPORT_NO = model.RenewedPassportNo;
        regTemp.PASSPORT_ISSUED_ON = model.PassportIssuedOn;
        regTemp.RENEWED_PASSPORT_ISSUED_ON = model.RenewedPassportIssuedOn;
        regTemp.VISA_NO = model.VisaNo;
        regTemp.VISA_ISSUED_ON = model.VisaIssuedOn;
        regTemp.IS_ADMISSION_CANCELLED = model.IsAdmissionCancelled;
        regTemp.ADMISSION_CANCELLED_DATE = model.AdmissionCancelledDate;
        regTemp.SSC_TOTAL_MARKS = model.SscTotalMarks;
        regTemp.HSC_TOTAL_MARKS = model.HscTotalMarks;
        regTemp.UG_TOTAL_MARKS = model.UgTotalMarks;
        regTemp.SSC_MARKS_OUT_OF = model.SscMarksOutOf;
        regTemp.HSC_MARKS_OUT_OF = model.HscMarksOutOf;
        regTemp.UG_MARKS_OUT_OF = model.UgMarksOutOf;

        regTemp.Chr_REG_CAST_CD = "0";
        regTemp.Chr_MIGRATION_FLG = "N";
        regTemp.Chr_DIST_EDU = "N";

        _coreService.REG_TEMP_Repository.Update(regTemp);
        _coreService.Save();
    }

    private void EditStudentInStudentInfo(RegistrationCommon model)
    {
        var tblStudentInfo = _coreService.Tbl_STUDENT_INFO_ADR_Repository
            .Get(r => r.Chr_FK_PRN_NO.Trim() == model.PrnNo)
            .FirstOrDefault();
        if (tblStudentInfo == null)
            return;

        tblStudentInfo.Ima_ST_PHOTO = model.Photo;

        _coreService.Tbl_STUDENT_INFO_ADR_Repository.Update(tblStudentInfo);
        _coreService.Save();
    }

    private bool ValidateViewModel(RegistrationCommon viewModel)
    {
        if (null == viewModel.CourseTypeId)
        {
            ModelState.AddModelError("Error", @"Course Type is required");
            return false;
        }
        if (null == viewModel.CourseId)
        {
            ModelState.AddModelError("Error", @"Course is required");
            return false;
        }
        if (null == viewModel.CoursePartId)
        {
            ModelState.AddModelError("Error", @"Course Part is required");
            return false;
        }
        if (viewModel.Year > DateTime.Now.Year)
        {
            ModelState.AddModelError("Error", @"Admission year cannot be greater than current year ");
            return false;
        }

        var exists = _coreService.REG_TEMP_Repository.Get(s =>
                s.Num_FK_INST_NO == viewModel.InstanceId && s.Num_MOBILE == viewModel.Mobile1 &&
                s.Chr_REG_ST_NM == viewModel.StudentName)
            .FirstOrDefault();
        if (null != exists)
        {
            ModelState.AddModelError("Error", $@"This student is already registered. (Instance: {viewModel.InstanceId}, Mobile: {viewModel.Mobile1}, Name: {viewModel.StudentName})");
            return false;
        }

        return true;
    }

    //protected override void Dispose(bool disposing)
    //{
    //    _registrationService.Dispose(disposing);
    //    base.Dispose(disposing);
    //}
}