using Corno.Globals.Constants;
using Corno.OnlineExam.Controllers;
using Microsoft.AspNet.Identity;
using System;
using System.Linq;
using System.Web.Mvc;
using Corno.Data.Core;
using Corno.Data.Corno;
using Corno.Data.Helpers;
using Corno.Services.Bootstrapper;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;

namespace Corno.OnlineExam.Areas.Transactions.Controllers;

[Authorize]
public class ConvocationController : UniversityController
{
    #region -- Constructors --
    public ConvocationController()
    {
        _cornoService = Bootstrapper.Get<ICornoService>();
        _coreService = Bootstrapper.Get<ICoreService>(); ;
    }
    #endregion

    #region -- Data Members --
    private readonly ICornoService _cornoService;
    private readonly ICoreService _coreService;
    #endregion

    #region -- Private Methods --

    private void FillViewModel(ConvocationViewModel viewModel)
    {
        var yearChangeRecord = _coreService.Tbl_STUDENT_YR_CHNG_Repository.Get(s => s.Chr_FK_PRN_NO == viewModel.PrnNo).OrderByDescending(s => s.Num_FK_INST_NO).FirstOrDefault();
        if (null == yearChangeRecord)
        {
            // Go to TBL_STUDENT_COURSE
            var studentCourse = _coreService.Tbl_STUDENT_COURSE_Repository.Get(s => s.Chr_FK_PRN_NO == viewModel.PrnNo).FirstOrDefault();
            if (null == studentCourse)
                throw new Exception("PRN No " + viewModel.PrnNo + " does not exists!");

            if (null != studentCourse.Num_FK_DistCenter_ID)
                viewModel.CentreId = (int)studentCourse.Num_FK_DistCenter_ID;
            if (studentCourse.Num_ST_COLLEGE_CD != null)
                viewModel.CollegeId = (int)studentCourse.Num_ST_COLLEGE_CD;
        }
        else
        {
            if (null != yearChangeRecord.Num_FK_DistCenter_ID)
                viewModel.CentreId = (int)yearChangeRecord.Num_FK_DistCenter_ID;

            viewModel.CollegeId = yearChangeRecord.Num_FK_COL_CD;
        }

        viewModel.StudentName = ExamServerHelper.GetStudentName(viewModel.PrnNo, _coreService);
        viewModel.CollegeName = ExamServerHelper.GetCollegeName(viewModel.CollegeId, _coreService);
        viewModel.CentreName = ExamServerHelper.GetCentreName(viewModel.CentreId, _coreService);
        viewModel.CourseId = ExamServerHelper.GetCourseId(viewModel.CoursePartId, _coreService);
        viewModel.CourseName = ExamServerHelper.GetCourseNameFromCoursePartId(viewModel.CoursePartId, _coreService);
        viewModel.CoursePartName = ExamServerHelper.GetCourseNameFromCoursePartId(viewModel.CoursePartId, _coreService);
        viewModel.CourseTypeName = ExamServerHelper.GetCourseTypeName(viewModel.CoursePartId, _coreService);
        viewModel.BranchName = ExamServerHelper.GetBranchName(viewModel.BranchId, _coreService);
        viewModel.Gender = ExamServerHelper.GetGender(_coreService, viewModel.PrnNo) == "M" ? "Male" : "Female";
        viewModel.Photo = ExamServerHelper.GetPhoto(_coreService, viewModel.PrnNo);

        // Fill Fee details
        FillFeeStructure(viewModel);
    }

    private void AddInCoreDatabase(Convocation model)
    {
        var instanceId = model.InstanceId;

        var studentConvo = new Tbl_STUDENT_CONVO
        {
            Num_FK_INST_NO = (short?)instanceId,
            Num_CGPA_AVG = model.Cgpa.ToDecimal(),
            Chr_ST_YEAR = DateTime.Now.Year.ToString(),
            Num_FK_CO_CD = model.CourseId,
            Chr_ST_PA_FLG = model.InPerson == false ? "A" : "P",
            Chr_FK_PRN_NO = model.PrnNo,
            Num_ST_SEAT_NO = model.SeatNo.ToLong(),
            Var_ST_NM = ExamServerHelper.GetStudentName(model.PrnNo, _coreService),
            Chr_ST_SEX_CD = ExamServerHelper.GetGender(_coreService, model.PrnNo),
            Chr_ST_ADD1 = model.Address,
            Chr_ST_ADD2 = string.Empty,
            Chr_ST_ADD3 = string.Empty,
            Chr_ST_ADD4 = string.Empty,
            Chr_ST_PINCODE = model.PinCode.ToString(),
            Var_RES_PH = model.Phone,
            Var_E_MAIL = model.Email,
            Num_ST_PASS_MONTH = (short)model.PassMonth.ToUShort(),
            Chr_ST_PASS_YEAR = model.PassYear,
            Num_ST_PASS_MONTH1 = (short)model.PassMonth1.ToUShort(),
            Chr_ST_PASS_YEAR1 = model.PassYear1,
            AdharNo = model.AdharNo,
            Chr_ST_NMCHNG_FLG = "N",
            Chr_ST_VALID_FLG = "A",
            Chr_DELETE_FLG = "N",
            Num_FK_COLLEGE_CD = model.CollegeId,
            Num_FK_BR_CD = model.BranchId,
            Chr_FEES_STATUS = "C",
            Con_ST_FEES_AMT = model.TotalFee.ToUShort(),
            Destination = model.Destination,
            DegreePart = (short?)model.CoursePartId,
            Chr_Improvement = model.ClassImprovementStudent ? "Y" : "N",
            Chr_Foreign_Student = model.ForeignStudent ? "Y" : "N",
            Chr_PRINC_SUBJECT = model.PrincipleSubject1,
            Chr_PRINC_SUBJECT1 = model.PrincipleSubject2,
            Var_ST_USR_NM = User.Identity.Name,
            ST_DTE_CR = model.CreatedDate,
            ST_DTE_UP = model.ModifiedDate,
            Chr_Form_Submit = string.Empty,
            Num_Fk_Centre_cd = model.CentreId,
            Chr_Transaction_Id = model.TransactionId,
            PaidAmount = model.PaidAmount,
            PaymentDate = model.PaymentDate,
            SettlementDate = model.SettlementDate
        };

        var faculty = _coreService.Tbl_COURSE_MSTR_Repository.Get(s => s.Num_PK_CO_CD == model.CourseId).FirstOrDefault();
        if (faculty != null)
            studentConvo.Num_FK_FA_CD = faculty.Num_FK_FA_CD;
        var sysInstance = _coreService.Tbl_SYS_INST_Repository.Get(i => 
            i.Num_PK_INST_SRNO == instanceId).FirstOrDefault();
        if (sysInstance?.Num_CONVO_NO != null)
            studentConvo.Num_FK_CONVO_NO = (short)sysInstance.Num_CONVO_NO;
        //if (!string.IsNullOrEmpty(model.ClassCode))
        studentConvo.Num_FK_RESULT_CD = (short)model.ClassCode.ToInt();
        var studentInfo = _coreService.Tbl_STUDENT_INFO_ADR_Repository.Get(c => c.Chr_FK_PRN_NO == model.PrnNo).FirstOrDefault();
        if (null != studentInfo)
            studentConvo.Ima_ST_PHOTO = studentInfo.Ima_ST_PHOTO;

        _coreService.Tbl_STUDENT_CONVO_Repository.Add(studentConvo);
        _coreService.Save();
    }
    #endregion

    #region -- Protected Methods --
    protected void FillFeeStructure(ConvocationViewModel viewModel)
    {
        //  get Convocation Fee ;
        var course = _coreService.Tbl_COURSE_MSTR_Repository.Get(c => c.Num_PK_CO_CD == viewModel.CourseId).FirstOrDefault();
        if (null == course) return;

        var convocationFee = _cornoService.ConvocationFeeRepository.Get(c =>
            c.FacultyId == course.Num_FK_FA_CD && c.CourseTypeId == course.Num_FK_TYP_CD).FirstOrDefault();
        if (null == convocationFee)
            throw new Exception("Convocation fee is not available in system.");
        viewModel.ConvocationFee = convocationFee.Fee ?? 0;
        viewModel.PostalChargeInIndia = convocationFee.PostalIndia ?? 0;
        viewModel.PostalChargeInAbroad = convocationFee.PostalOverseas ?? 0;

        if (convocationFee.StartDate == null || convocationFee.EndDate == null)
            throw new Exception("Convocation end date is not available in system.");
        if (DateTime.Now.Date < convocationFee.StartDate.Value.Date)
            throw new Exception($"Form Filling Start Date is {convocationFee.StartDate?.ToString("dd/MM/yyyy")}. Form are not be accepted before this date.");
        if (DateTime.Now.Date > convocationFee.EndDate.Value.Date)
            throw new Exception($"Form Filling Last Date is {convocationFee.EndDate?.ToString("dd/MM/yyyy")}. Form are not be accepted after this date.");
    }

    protected void PrnChange(ConvocationViewModel viewModel)
    {
        // First Check for Result
        var studentExams = (from studentExam in _coreService.TBL_STUDENT_EXAMS_Repository.Get(
                e => e.Chr_FK_PRN_NO.Trim() == viewModel.PrnNo.Trim() && e.Chr_ST_COPRT_RES == "P")
                            join coursePart in _coreService.Tbl_COURSE_PART_MSTR_Repository.Get(c => c.Chr_DEG_APL_FLG == "Y") on
                                studentExam.Num_FK_COPRT_NO equals coursePart.Num_PK_COPRT_NO
                            select new
                            {
                                studentExam.Chr_IMPROVEMENT_FLG,
                                studentExam.Num_FK_CO_CD,
                                studentExam.Num_FK_COPRT_NO,
                                studentExam.Num_FK_BR_CD,
                                studentExam.Num_FK_INST_NO,
                                studentExam.Num_ST_SEAT_NO,
                                studentExam.Num_FK_CLASS_CD,
                                coursePart.Var_COPRT_DESC,
                                coursePart.Chr_CGPA_APL_FLG
                            })
            .OrderBy(s => s.Num_FK_INST_NO)
            .ToList();

        // Now Check for Class
        if (studentExams.Count <= 0)
        {
            studentExams = (from studentExam in _coreService.TBL_STUDENT_EXAMS_Repository.Get(e => e.Chr_FK_PRN_NO.Trim() == viewModel.PrnNo.Trim() && e.Num_FK_CLASS_CD != null)
                            join coursePart in _coreService.Tbl_COURSE_PART_MSTR_Repository.Get(c => c.Chr_DEG_APL_FLG == "Y")
                                on studentExam.Num_FK_COPRT_NO equals coursePart.Num_PK_COPRT_NO
                            select new
                            {
                                studentExam.Chr_IMPROVEMENT_FLG,
                                studentExam.Num_FK_CO_CD,
                                studentExam.Num_FK_COPRT_NO,
                                studentExam.Num_FK_BR_CD,
                                studentExam.Num_FK_INST_NO,
                                studentExam.Num_ST_SEAT_NO,
                                studentExam.Num_FK_CLASS_CD,
                                coursePart.Var_COPRT_DESC,
                                coursePart.Chr_CGPA_APL_FLG
                            })
                .OrderBy(s => s.Num_FK_INST_NO)
                .ToList();
        }

        if (studentExams.Count <= 0)
            throw new Exception("You are not allowed to fill the form. \r\nReason : Course part is not applicable for degree.");

        // Login instance ID should be >= passed instance ID
        var loginInstanceId = (int)HttpContext.Session[ModelConstants.InstanceId];
        var convocationInstanceId = studentExams.LastOrDefault()?.Num_FK_INST_NO;
        if (loginInstanceId < convocationInstanceId)
            throw new Exception("You are not allowed to fill the form. Your login session is less than pass out session.");

        // Check whether convocation is frozen.
        IsFrozen(studentExams.FirstOrDefault()?.Num_FK_CO_CD ?? 0,
            viewModel.InstanceId ?? 0);

        // Update model instance Id.
        viewModel.InstanceId = studentExams.LastOrDefault()?.Num_FK_INST_NO;

        var anyCoursePart = false;
        foreach (var studentExam in studentExams)
        {
            // Check whether course part has already filled the convocation.
            var existingRecord = _coreService.Tbl_STUDENT_CONVO_Repository.Get(s => s.Chr_FK_PRN_NO == viewModel.PrnNo && s.DegreePart == studentExam.Num_FK_COPRT_NO).FirstOrDefault();
            if (null != existingRecord && studentExam.Chr_IMPROVEMENT_FLG != "Y") continue;

            // Special case
            existingRecord = _coreService.Tbl_STUDENT_CONVO_Repository.Get(s => s.Chr_FK_PRN_NO == viewModel.PrnNo && s.DegreePart == null).FirstOrDefault();
            if (null != existingRecord) continue;

            anyCoursePart = true;

            var resultDate = ExamServerHelper.GetResultDate(_coreService, studentExam.Num_FK_INST_NO, studentExam.Num_FK_COPRT_NO);
            if (DateTime.Now.Date < resultDate?.AddDays(2).Date)
                throw new Exception("Convocation form will be accepted after this Date : ." + resultDate?.AddDays(2).ToString("dd/MM/yyyy"));

            if (null != resultDate)
            {
                viewModel.PassingYear = resultDate.Value.Year;
                viewModel.PassingMonth = resultDate.Value.Month;
            }

            viewModel.AdharNo = ExamServerHelper.GetStudentAdharNo(viewModel.PrnNo, _coreService);
            viewModel.ClassImprovementStudent = studentExam.Chr_IMPROVEMENT_FLG == "Y";
            viewModel.SeatNo = studentExam.Num_ST_SEAT_NO.ToString();
            viewModel.CourseId = studentExam.Num_FK_CO_CD;
            viewModel.CoursePartId = studentExam.Num_FK_COPRT_NO;
            viewModel.BranchId = studentExam.Num_FK_BR_CD;
            viewModel.ClassCode = studentExam.Num_FK_CLASS_CD;
            switch (studentExam.Chr_CGPA_APL_FLG)
            {
                case "Y":
                    if (studentExam.Num_FK_CLASS_CD != null)
                        viewModel.Class = ExamServerHelper.GetClass(_coreService, (int)studentExam.Num_FK_CLASS_CD);

                    viewModel.Cgpa = ExamServerHelper.GetCgpa(_coreService, studentExam.Num_FK_INST_NO, studentExam.Num_FK_COPRT_NO, viewModel.PrnNo)
                        .ToDouble();
                    break;
                case "N":
                    if (null != studentExam.Num_FK_CLASS_CD)
                    {
                        viewModel.Class = ExamServerHelper.GetClass(_coreService, (int)studentExam.Num_FK_CLASS_CD);
                        if (string.IsNullOrEmpty(viewModel.Class))
                            viewModel.Grade = ExamServerHelper.GetGrade(_coreService, (int)studentExam.Num_FK_CLASS_CD);
                        viewModel.Cgpa = 0;
                    }

                    break;
            }

            break;
        }

        if (false == anyCoursePart)
            throw new Exception("Your all convocation forms are already submitted.");

        var studentInfo =
            _coreService.Tbl_STUDENT_INFO_ADR_Repository.Get(c => c.Chr_FK_PRN_NO == viewModel.PrnNo)
                .FirstOrDefault();
        viewModel.Phone = studentInfo?.Num_MOBILE;
        viewModel.Email = studentInfo?.Chr_Student_Email;

        // Fill the View Model
        FillViewModel(viewModel);
    }

    private void IsFrozen(int courseId, int instanceId)
    {
        // Check whether time table is freeze
        var timeTableFreeze = _coreService.Tbl_TimeTableINST_Repository.Get(t =>
                t.Num_FK_INST_NO == instanceId && t.Num_PK_CO_CD == courseId)
            .FirstOrDefault();

        if (timeTableFreeze is { Chr_FreezeConvocation: "Y" })
            throw new Exception("Convocation is frozen. Not allowed to fill form.");
    }

    protected Convocation AddInCornoDatabase(ConvocationViewModel viewModel)
    {
        // Make upper case
        viewModel.Address = viewModel.Address.ToUpper();
        viewModel.Destination = viewModel.Destination.ToUpper();

        //var instanceId = (int)HttpContext.Session[ModelConstants.InstanceId];
        var instanceId = viewModel.InstanceId;
        var prnNo = viewModel.PrnNo;
        var coursePartId = viewModel.CoursePartId;
        var existingRecord = _coreService.Tbl_STUDENT_CONVO_Repository.Get(e => e.Chr_FK_PRN_NO == prnNo &&
            e.Num_FK_INST_NO == instanceId && e.DegreePart == coursePartId).ToList();
        if (existingRecord.Count > 0)
            throw new Exception("Convocation form for this PRN " + viewModel.PrnNo + " already exists");
        if (viewModel.TotalFee <= 0)
            throw new Exception("Convocation total fee cannot be zero.");
        if (viewModel.PassingMonth1 <= 0 || viewModel.PassingYear1 <= 0)
            throw new Exception("Passing Month and Passing Year cannot be zero.");

        IsFrozen(viewModel.CourseId ?? 0, instanceId ?? 0);

        var model = AutoMapperConfig.CornoMapper.Map<Convocation>(viewModel);
        var studentCourse = _coreService.Tbl_STUDENT_COURSE_Repository.Get(s => s.Chr_FK_PRN_NO == prnNo).FirstOrDefault();
        if (null != studentCourse)
        {
            model.CollegeId = studentCourse.Num_ST_COLLEGE_CD;
            model.CourseId = studentCourse.Num_FK_CO_CD;
        }

        model.PassYear = viewModel.PassingYear.ToString();
        model.PassMonth = viewModel.PassingMonth.ToString();
        model.PassYear1 = viewModel.PassingYear1.ToString();
        model.PassMonth1 = viewModel.PassingMonth1.ToString();
        model.CreatedBy = User.Identity.GetUserId();
        model.CreatedDate = DateTime.Now;

        model.TransactionId = viewModel.TransactionId;
        model.Status = viewModel.Status;

        _cornoService.ConvocationRepository.Add(model);
        _cornoService.Save();

        return model;
    }

    protected void Update(string prn, string transactionId, double paidAmount, DateTime paymentDate, string ipAddress)
    {
        var convocation = _cornoService.ConvocationRepository.Get(e => e.PrnNo == prn &&
                e.Status == StatusConstants.InProcess, p => p)
            .OrderByDescending(p => p.Id).FirstOrDefault();
        if (null == convocation)
            throw new Exception($"No entry in convocation found for PRN {transactionId}");

        if (!convocation.TotalFee.Equals(paidAmount))
        {
            throw new Exception($"You have made payment of Rs. {paidAmount} which does not match with actual amount Rs. {convocation.TotalFee}");
        }

        convocation.TransactionId = transactionId;
        convocation.PaidAmount ??= 0;
        convocation.PaidAmount += paidAmount;
        convocation.PaymentDate = paymentDate;
        convocation.Status = StatusConstants.Paid;
        convocation.DeletedBy = ipAddress;

        // Add in Core database
        AddInCoreDatabase(convocation);

        _cornoService.ConvocationRepository.Update(convocation);
        _cornoService.Save();
    }
    #endregion

    #region -- Events --
    // GET: /Convocation/Create
    [Authorize]
    public virtual ActionResult Create()
    {
        return View(new ConvocationViewModel());
    }

    // POST: /Convocation/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    [Authorize]
    public virtual ActionResult Create(ConvocationViewModel viewModel, string submitType)
    {
        try
        {
            if (submitType == "Search")
            {
                if (string.IsNullOrEmpty(viewModel.PrnNo))
                    throw new Exception("Error: Invalid PRN No");
                PrnChange(viewModel);

                ModelState.Clear();
                return View(viewModel);
            }

            if (ModelState.IsValid)
            {
                /*// Make upper case
                viewModel.Address = viewModel.Address.ToUpper();
                viewModel.Destination = viewModel.Destination.ToUpper();

                var instanceId = (int)HttpContext.Session[ModelConstants.InstanceId];
                var prnNo = viewModel.PrnNo;
                var coursePartId = viewModel.CoursePartId;
                var existingRecord = _examService.Tbl_STUDENT_CONVO_Repository.Get(e => e.Chr_FK_PRN_NO == prnNo &&
                e.Num_FK_INST_NO == instanceId && e.DegreePart == coursePartId).ToList();
                if (existingRecord.Count > 0)
                    throw new Exception("Convocation form for this PRN " + viewModel.PrnNo + " already exists");
                if (viewModel.Total <= 0)
                    throw new Exception("Convocation total fee cannot be zero.");
                if (viewModel.PassingMonth1 <= 0 || viewModel.PassingYear1 <= 0)
                    throw new Exception("Passing Month and Passing Year cannot be zero.");

                IsFrozen(viewModel.CourseId ?? 0, instanceId);

                var model = AutoMapperConfig.CornoMapper.Map<Convocation>(viewModel);
                var studentCourse = _examService.Tbl_STUDENT_COURSE_Repository.Get(s => s.Chr_FK_PRN_NO == prnNo).FirstOrDefault();
                if (null != studentCourse)
                {
                    model.CollegeId = studentCourse.Num_ST_COLLEGE_CD;
                    model.CourseId = studentCourse.Num_FK_CO_CD;
                }

                model.PassYear = viewModel.PassingYear.ToString();
                model.PassMonth = viewModel.PassingMonth.ToString();
                model.PassYear1 = viewModel.PassingYear1.ToString();
                model.PassMonth1 = viewModel.PassingMonth1.ToString();
                model.CreatedBy = User.Identity.GetUserId();
                model.CreatedDate = DateTime.Now;

                using (var scope = new TransactionScope(TransactionScopeOption.Required, new TransactionOptions { IsolationLevel = IsolationLevel.ReadCommitted, Timeout = TransactionManager.MaximumTimeout }))
                {
                    try
                    {
                        _registrationService.ConvocationRepository.Add(model);
                        _registrationService.Save();

                        // Add to exam server
                        AddInCoreDatabase(model, viewModel);

                        scope.Complete();

                        viewModel = new ConvocationViewModel();

                        TempData["Success"] = "Convocation form submitted successfully.";
                    }
                    catch (Exception)
                    {
                        scope.Dispose();
                        throw;
                    }
                }*/

                // Save
                viewModel.InstanceId = (int)HttpContext.Session[ModelConstants.InstanceId];
                var model = AddInCornoDatabase(viewModel);
                model.InstanceId = (int)HttpContext.Session[ModelConstants.InstanceId];
                AddInCoreDatabase(model);

                viewModel = new ConvocationViewModel();
                TempData["Success"] = "Convocation form submitted successfully.";

                return RedirectToAction("Create");
            }
        }
        catch (Exception exception)
        {
            viewModel.Clear();

            HandleControllerException(exception);
            //ModelState.Clear();
        }

        //PrnChange(viewModel);
        return View(viewModel);
    }

    #endregion
}

