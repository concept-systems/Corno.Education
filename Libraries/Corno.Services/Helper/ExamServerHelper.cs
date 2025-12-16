using System;
using System.Collections.Generic;
using System.Linq;
using Corno.Data.Core;
using Corno.Data.Corno;
using Corno.Services.Core.Interfaces;
using Corno.Services.Corno.Interfaces;

namespace Corno.Services.Helper;

public class ExamServerHelper
{
    public static string GetInstanceName(int? instanceId, ICoreService examService)
    {
        var instanceName = "-";
        try
        {
            if (null != instanceId)
            {
                // Get Course from Tbl_SYS_INST
                var instance =
                    examService.Tbl_SYS_INST_Repository.Get(
                        c => c.Num_PK_INST_SRNO == instanceId).FirstOrDefault();
                if (null != instance)
                    instanceName = instance.Var_INST_REM;
            }
        }
        catch
        {
            // ignored
        }

        return instanceName;
    }

    public static string GetStudentName(string prnNo, ICoreService examService)
    {
        var studentName = "-";
        try
        {
            if (null != prnNo)
            {
                // Get Course from TBL_COURSE_MSTR
                var studentInfo =
                    examService.Tbl_STUDENT_INFO_Repository.Get(c => c.Chr_PK_PRN_NO == prnNo).FirstOrDefault();
                if (null != studentInfo)
                    studentName = studentInfo.Var_ST_NM;
            }
        }
        catch
        {
            // ignored
        }

        return studentName;
    }

    public static string GetStudentPrn(string mobileNo, ICoreService examService)
    {
        var prn = "-";
        try
        {
            if (!string.IsNullOrEmpty(mobileNo))
            {
                // Get Course from TBL_COURSE_MSTR
                var studentInfo = examService.Tbl_STUDENT_INFO_ADR_Repository.Get(c => c.Num_MOBILE.ToString() == mobileNo)
                    .FirstOrDefault();
                prn = studentInfo?.Chr_FK_PRN_NO;
            }
        }
        catch
        {
            // ignored
        }

        return prn;
    }

    public static string GetStudentMobile(string prnNo, ICoreService examService)
    {
        return examService.Tbl_STUDENT_INFO_ADR_Repository.Get(c => c.Chr_FK_PRN_NO == prnNo)?
            .FirstOrDefault()?.Num_MOBILE;
    }

    public static string GetStudentEmail(string prnNo, ICoreService examService)
    {
        return examService.Tbl_STUDENT_INFO_ADR_Repository.Get(c => c.Chr_FK_PRN_NO == prnNo)?
            .FirstOrDefault()?.Chr_Student_Email;
    }

    public static byte[] GetStudentPhoto(string prnNo, ICoreService examService)
    {
        return examService.Tbl_STUDENT_INFO_ADR_Repository.Get(c => c.Chr_FK_PRN_NO == prnNo)?
            .FirstOrDefault()?.Ima_ST_PHOTO;
    }

    public static string GetStudentAdharNo(string prnNo, ICoreService examService)
    {
        var aadharNo = "-";
        try
        {
            if (null != prnNo)
            {
                var studentInfo = examService.Tbl_STUDENT_INFO_Repository.Get(c => c.Chr_PK_PRN_NO == prnNo).FirstOrDefault();
                if (null != studentInfo)
                    aadharNo = studentInfo.AadharNo;
            }
        }
        catch
        {
            // ignored
        }

        return aadharNo;
    }

    public static string GetStudentGender(string prnNo, ICoreService examService)
    {
        var gender = "-";
        try
        {
            if (null != prnNo)
            {
                var studentInfo =
                    examService.Tbl_STUDENT_INFO_Repository.Get(c => c.Chr_PK_PRN_NO == prnNo).FirstOrDefault();
                if (null != studentInfo)
                    gender = studentInfo.Chr_ST_SEX_CD;
            }
        }
        catch
        {
            // ignored
        }

        return gender;
    }

    public static int GetCollegeIdFromCourseId(int? courseId, ICoreService examService)
    {
        var collegeId = 0;
        try
        {
            if (null != courseId)
            {
                var collegeCourseMaster =
                    examService.Tbl_COLLEGE_COURSE_MSTRRepository.Get(
                        c => c.NUM_FK_CO_CD == courseId).FirstOrDefault();
                if (null != collegeCourseMaster)
                    collegeId = collegeCourseMaster.NUM_FK_COLLEGE_CD;
            }
        }
        catch
        {
            // ignored
        }

        return collegeId;
    }

    public static string GetCollegeName(int? collegeId, ICoreService examService)
    {
        var collegeName = "-";
        try
        {
            if (null != collegeId)
            {
                var college =
                    examService.TBL_COLLEGE_MSTRRepository.FirstOrDefault(c => 
                        c.Num_PK_COLLEGE_CD == collegeId && c.Chr_DELETE_FLG != "Y", p => p);
                if (null != college)
                    collegeName = college.Var_CL_COLLEGE_NM1;
            }
        }
        catch
        {
            // ignored
        }

        return collegeName;
    }

    public static string GetCollegeShortName(int? collegeId, ICoreService examService)
    {
        var shortName = "-";
        try
        {
            if (null != collegeId)
            {
                var college =
                    examService.TBL_COLLEGE_MSTRRepository.Get(
                        c => c.Num_PK_COLLEGE_CD == collegeId && c.Chr_DELETE_FLG != "Y").FirstOrDefault();
                if (null != college)
                    shortName = college.Var_CL_SHRT_NM;
            }
        }
        catch
        {
            // ignored
        }

        return shortName;
    }

    public static string GetCentreName(int? centreId, ICoreService examService)
    {
        var centreName = "-";
        try
        {
            if (null != centreId)
            {
                var center =
                    examService.TBL_DISTANCE_CENTERS_Repository.Get(c => c.Num_PK_DistCenter_ID == centreId)
                        .FirstOrDefault();
                if (null != center)
                    centreName = center.DIST_CENT_NAME;
            }
        }
        catch
        {
            // ignored
        }

        return centreName;
    }

    public static string GetCoursePartName(int? coursePartId, ICoreService examService)
    {
        var coursePartName = "-";
        try
        {
            if (null != coursePartId)
            {
                var coursePart =
                    examService.Tbl_COURSE_PART_MSTR_Repository.Get(
                        c => c.Num_PK_COPRT_NO == coursePartId && c.Chr_DELETE_FLG != "Y").FirstOrDefault();
                if (null != coursePart)
                    coursePartName = coursePart.Var_COPRT_DESC;
            }
        }
        catch
        {
            // ignored
        }

        return coursePartName;
    }

    public static string GetCoursePartShortName(int? coursePartId, ICoreService examService)
    {
        var coursePartShortName = "-";
        try
        {
            if (null != coursePartId)
            {
                var coursePart =
                    examService.Tbl_COURSE_PART_MSTR_Repository.Get(
                        c => c.Num_PK_COPRT_NO == coursePartId && c.Chr_DELETE_FLG != "Y").FirstOrDefault();
                if (null != coursePart)
                    coursePartShortName = coursePart.Var_COPRT_SHRT_NM;
            }
        }
        catch
        {
            // ignored
        }

        return coursePartShortName;
    }

    public static Tbl_COURSE_PART_MSTR GetCoursePart(int? coursePartId, ICoreService examService)
    {
        try
        {
            if (null != coursePartId)
            {
                var coursePart =
                    examService.Tbl_COURSE_PART_MSTR_Repository.Get(
                        c => c.Num_PK_COPRT_NO == coursePartId && c.Chr_DELETE_FLG != "Y").FirstOrDefault();
                return coursePart;
            }
        }
        catch
        {
            // ignored
        }

        return null;
    }

    public static bool HasCoursePartAdditionalCredits(int? coursePartId, ICoreService examService)
    {
        try
        {
            //TODO: It is not good logic to bo hardcoded.
            if (coursePartId == 1247)
                return true;

            //var coursePart =
            //    examService.Tbl_COURSE_PART_MSTR_Repository.Get(
            //        c => c.Num_PK_COPRT_NO == coursePartId && c.Chr_DELETE_FLG != "Y").FirstOrDefault();
            //if (coursePart != null && coursePart.Chr_AdditionalCredits == "Y")
            //    return true;
        }
        catch
        {
            // ignored
        }

        return false;
    }



    public static int GetCourseId(int? coursePartId, ICoreService examService)
    {
        var courseId = 0;
        try
        {
            if (coursePartId > 0)
            {
                var coursePart =
                    examService.Tbl_COURSE_PART_MSTR_Repository.Get(
                        c => c.Num_PK_COPRT_NO == coursePartId && c.Chr_DELETE_FLG != "Y").FirstOrDefault();
                if (null != coursePart)
                    courseId = coursePart.Num_FK_CO_CD;
            }
        }
        catch
        {
            // ignored
        }

        return courseId;
    }

    public static string GetCourseName(int? courseId, ICoreService examService)
    {
        var courseName = "-";
        try
        {
            if (null != courseId)
            {
                var course =
                    examService.Tbl_COURSE_MSTR_Repository.Get(
                        c => c.Num_PK_CO_CD == courseId && c.Chr_DELETE_FLG != "Y").FirstOrDefault();
                if (null != course)
                    courseName = course.Var_CO_NM;
            }
        }
        catch
        {
            // ignored
        }

        return courseName;
    }

    public static string GetCourseShortName(int? courseId, ICoreService examService)
    {
        var courseShortName = "-";
        try
        {
            if (null != courseId)
            {
                var course =
                    examService.Tbl_COURSE_MSTR_Repository.Get(
                        c => c.Num_PK_CO_CD == courseId && c.Chr_DELETE_FLG != "Y").FirstOrDefault();
                if (null != course)
                    courseShortName = course.Var_CO_SHRT_NM;
            }
        }
        catch
        {
            // ignored
        }

        return courseShortName;
    }

    public static string GetCourseNameFromCoursePartId(int? coursePartId, ICoreService examService)
    {
        var courseName = "-";
        try
        {
            if (null != coursePartId)
            {
                var coursePart =
                    examService.Tbl_COURSE_PART_MSTR_Repository.Get(
                        c => c.Num_PK_COPRT_NO == coursePartId && c.Chr_DELETE_FLG != "Y").FirstOrDefault();
                if (coursePart != null)
                {
                    int courseId = coursePart.Num_FK_CO_CD;
                    var course =
                        examService.Tbl_COURSE_MSTR_Repository.Get(
                            c => c.Num_PK_CO_CD == courseId && c.Chr_DELETE_FLG != "Y").FirstOrDefault();
                    if (null != course)
                        courseName = course.Var_CO_NM;
                }
            }
        }
        catch
        {
            // ignored
        }

        return courseName;
    }

    public static string GetCourseShortNameFromCoursePartId(int? coursePartId, ICoreService examService)
    {
        var courseShortName = "-";
        try
        {
            if (null != coursePartId)
            {
                var coursePart =
                    examService.Tbl_COURSE_PART_MSTR_Repository.Get(
                        c => c.Num_PK_COPRT_NO == coursePartId && c.Chr_DELETE_FLG != "Y").FirstOrDefault();
                // Get Course from TBL_COURSE_MSTR
                if (coursePart != null)
                {
                    int courseId = coursePart.Num_FK_CO_CD;
                    var course =
                        examService.Tbl_COURSE_MSTR_Repository.Get(
                            c => c.Num_PK_CO_CD == courseId && c.Chr_DELETE_FLG != "Y").FirstOrDefault();
                    if (null != course)
                        courseShortName = course.Var_CO_SHRT_NM;
                }
            }
        }
        catch
        {
            // ignored
        }

        return courseShortName;
    }

    public static string GetCourseTypeName(int? coursePartId, ICoreService examService)
    {
        var courseTypeName = "-";
        try
        {
            if (null != coursePartId)
            {
                var coursePart =
                    examService.Tbl_COURSE_PART_MSTR_Repository.Get(
                        c => c.Num_PK_COPRT_NO == coursePartId && c.Chr_DELETE_FLG != "Y").FirstOrDefault();
                var course =
                    examService.Tbl_COURSE_MSTR_Repository.Get(
                        c => c.Num_PK_CO_CD == coursePart.Num_FK_CO_CD && c.Chr_DELETE_FLG != "Y").FirstOrDefault();
                if (null == course)
                    return null;

                int courseTypeId = course.Num_FK_TYP_CD;
                // Get Course from TBL_COURSE_MSTR
                var courseType =
                    examService.Tbl_COURSE_TYPE_MSTR_Repository.Get(
                        c => c.Num_PK_TYP_CD == courseTypeId && c.Chr_DELETE_FLG != "Y").FirstOrDefault();
                if (null != courseType)
                    courseTypeName = courseType.Var_TYP_NM;
            }
        }
        catch
        {
            // ignored
        }

        return courseTypeName;
    }

    public static string GetBranchName(int? branchId, ICoreService examService)
    {
        var branchName = "-";
        try
        {
            if (null != branchId)
            {
                // Get Course from TBL_COURSE_MSTR
                var branch =
                    examService.Tbl_BRANCH_MSTR_Repository.Get(
                        b => b.Num_PK_BR_CD == branchId && b.Chr_DELETE_FLG != "Y").FirstOrDefault();
                if (null != branch)
                    branchName = branch.Var_BR_NM;
            }
        }
        catch
        {
            // ignored
        }

        return branchName;
    }

    public static string GetBranchShortName(int? branchId, ICoreService examService)
    {
        var branchShortName = "-";
        try
        {
            if (null != branchId)
            {
                // Get Course from TBL_COURSE_MSTR
                var branch =
                    examService.Tbl_BRANCH_MSTR_Repository.Get(
                        b => b.Num_PK_BR_CD == branchId && b.Chr_DELETE_FLG != "Y").FirstOrDefault();
                if (null != branch)
                    branchShortName = branch.Var_BR_SHRT_NM;
            }
        }
        catch
        {
            // ignored
        }

        return branchShortName;
    }

    public static string SubjectCommonType(int? subjectId, ICoreService examService)
    {
        var commonType = "-";
        try
        {
            if (null != subjectId)
            {
                // Get Course from TBL_COURSE_MSTR
                var subject =
                    examService.Tbl_SUBJECT_MSTR_Repository.Get(
                        s => s.Num_PK_SUB_CD == subjectId && s.Chr_DELETE_FLG != "Y").FirstOrDefault();
                if (null != subject)
                    commonType = subject.Var_CommonSubject;
            }
        }
        catch
        {
            // ignored
        }

        return commonType;
    }

    public static string GetSubjectName(int? subjectId, ICoreService examService)
    {
        var subjectName = "-";
        try
        {
            if (null != subjectId)
            {
                // Get Course from TBL_COURSE_MSTR
                var subject =
                    examService.Tbl_SUBJECT_MSTR_Repository.Get(
                        s => s.Num_PK_SUB_CD == subjectId && s.Chr_DELETE_FLG != "Y").FirstOrDefault();
                if (null != subject)
                    subjectName = subject.Var_SUBJECT_NM;
            }
        }
        catch
        {
            // ignored
        }

        return subjectName;
    }

    public static string GetSubjectShortName(int? subjectId, ICoreService examService)
    {
        var subjectName = "-";
        try
        {
            if (null != subjectId)
            {
                // Get Course from TBL_COURSE_MSTR
                var subject =
                    examService.Tbl_SUBJECT_MSTR_Repository.Get(
                        s => s.Num_PK_SUB_CD == subjectId && s.Chr_DELETE_FLG != "Y").FirstOrDefault();
                if (null != subject)
                    subjectName = subject.Var_SUBJECT_SHRT_NM;
            }
        }
        catch
        {
            // ignored
        }

        return subjectName;
    }

    public static string GetCategoryShortName(int? categoryId, ICoreService examService)
    {
        var categoryName = "-";
        try
        {
            if (null != categoryId)
            {
                // Get Course from TBL_COURSE_MSTR
                var subject =
                    examService.Tbl_EVALCAT_MSTR_Repository.Get(
                        s => s.Num_PK_CAT_CD == categoryId && s.Chr_DELETE_FLG != "Y").FirstOrDefault();
                if (null != subject)
                    categoryName = subject.Var_CAT_SHRT_NM;
            }
        }
        catch
        {
            // ignored
        }

        return categoryName;
    }

    public static decimal GetCgpa(ICoreService examService, int instanceId, int coursePartId, string prnNo)
    {
        decimal cgpaAverage = 0;
        try
        {
            if (null != prnNo)
            {
                // Get Course from TBL_COURSE_MSTR
                var studentCgpa =
                    examService.TBL_STUDENT_CGPA_Repository.Get(
                        c =>
                            c.Num_FK_INST_NO == instanceId && c.Chr_FK_PRN_NO == prnNo &&
                            c.Num_FK_COPRT_NO == coursePartId).FirstOrDefault();
                if (studentCgpa != null)
                {
                    var value = studentCgpa.Num_CGPA_AVG + (studentCgpa.Num_CGPA_GRACE ?? 0);
                    if (value != null)
                        cgpaAverage = (decimal)value;
                }
            }
        }
        catch
        {
            // ignored
        }

        return cgpaAverage;
    }

    public static string GetGender(ICoreService examService, string prnNo)
    {
        var gender = "_";
        try
        {
            if (null != prnNo)
            {
                var genderInfo =
                    examService.Tbl_STUDENT_INFO_Repository.Get(c => c.Chr_PK_PRN_NO == prnNo).FirstOrDefault();
                if (null != genderInfo)
                    gender = genderInfo.Chr_ST_SEX_CD;
            }
        }
        catch
        {
            // ignored
        }

        return gender;
    }

    public static long GetSeatNo(ICoreService examService, string prnNo)
    {
        long seatNo = 0;
        try
        {
            if (null != prnNo)
            {
                var seatInfo =
                    examService.Tbl_STUDENT_YR_CHNG_Repository.Get(c => c.Chr_FK_PRN_NO == prnNo)
                        .OrderByDescending(p => p.Dtm_DTE_CR).FirstOrDefault();
                if (null != seatInfo)
                    seatNo = seatInfo.Num_ST_SEAT_NO ?? 0;
            }
        }
        catch
        {
            // ignored
        }

        return seatNo;
    }

    public static string GetPhoto(ICoreService examService, string prnNo)
    {
        var studentPhoto = "_";
        try
        {
            if (null != prnNo)
            {
                var photo =
                    examService.Tbl_STUDENT_INFO_ADR_Repository.Get(c => c.Chr_FK_PRN_NO == prnNo).FirstOrDefault();
                if (photo?.Ima_ST_PHOTO != null)
                    studentPhoto = "data:image;base64, " + Convert.ToBase64String(photo.Ima_ST_PHOTO);
            }
        }
        catch
        {
            // ignored
        }

        return studentPhoto;
    }

    public static DateTime? GetResultDate(ICoreService examService, int instance, int coursePartId)
    {
        try
        {
            var year =
                examService.Tbl_EXAM_SCHEDULE_MSTR_Repository.Get(
                    c => c.Num_FK_INST_NO == instance && c.Num_FK_COPRT_NO == coursePartId).FirstOrDefault();
            if (null != year)
                return year.Dtm_Result_Date;
        }
        catch
        {
            // ignored
        }

        return null;
    }

    public static DateTime? GetRevalStartDate(ICoreService examService, int instance, int coursePartId)
    {
        try
        {
            var year =
                examService.Tbl_EXAM_SCHEDULE_MSTR_Repository.Get(
                    c => c.Num_FK_INST_NO == instance && c.Num_FK_COPRT_NO == coursePartId).FirstOrDefault();
            if (null != year)
                return year.Dtm_Reval_Date;
        }
        catch
        {
            // ignored
        }

        return null;
    }

    public static int GetPassingMonth(ICoreService examService, int instance)
    {
        var passingMonth = 0;
        try
        {
            if (instance > 0)
            {
                var month =
                    examService.Tbl_SYS_INST_Repository.Get(c => c.Num_PK_INST_SRNO == instance).FirstOrDefault();
                if (null != month)
                    passingMonth = month.Num_INST_MONTH;
            }
        }
        catch
        {
            // ignored
        }

        return passingMonth;
    }

    public static string GetClass(ICoreService examService, int classId)
    {
        var className = "_";
        try
        {
            if (classId > 0)
            {
                var studentClass =
                    examService.Tbl_CLASS_MSTR_Repository.Get(c => c.Num_PK_CLS_CD == classId).FirstOrDefault();
                if (null != studentClass)
                    className = studentClass.Var_CLS_NM;
            }
        }
        catch
        {
            // ignored
        }

        return className;
    }

    public static string GetGrade(ICoreService examService, int gradeId)
    {
        var gradeName = "_";
        try
        {
            if (gradeId > 0)
            {
                var studentGrade =
                    examService.Tbl_GRADE_MSTR_Repository.Get(c => c.Num_PK_GRD_CD == gradeId).FirstOrDefault();
                if (null != studentGrade)
                    gradeName = studentGrade.Var_GRD_NM;
            }
        }
        catch
        {
            // ignored
        }

        return gradeName;
    }

    public static int GetConvoNo(int? instanceId, ICoreService examService)
    {
        var convoNo = 0;
        try
        {
            if (null != instanceId)
            {
                var sysInst =
                    examService.Tbl_SYS_INST_Repository.Get(c => c.Num_PK_INST_SRNO == instanceId).FirstOrDefault();
                if (sysInst?.Num_CONVO_NO != null)
                    convoNo = (int)sysInst.Num_CONVO_NO;
            }
        }
        catch
        {
            // ignored
        }

        return convoNo;
    }

    public static Tbl_COURSE_PART_MSTR GetNextCoursePart(int lastCoursePartId, ICoreService examService)
    {
        var coursePart =
            examService.Tbl_COURSE_PART_MSTR_Repository.Get(cp => cp.Num_PK_COPRT_NO == lastCoursePartId)
                .FirstOrDefault();

        // Get Num_COPRT_SEQ_NO for last course part from Tbl_COURSE_PART_MSTR
        if (coursePart?.Num_COPRT_SEQ_NO == null) return null;

        var lastSequenceNo = (int)coursePart.Num_COPRT_SEQ_NO;
        if (lastSequenceNo <= 0)
            throw new Exception("Last Course Part Sequence No is not valid");

        // Get course part for next sequence no from Tbl_COURSE_PART_MSTR
        var nextSequenceNo = lastSequenceNo + 1;
        var nextCoursePart =
            examService.Tbl_COURSE_PART_MSTR_Repository.Get(
                    cp => cp.Num_FK_CO_CD == coursePart.Num_FK_CO_CD && cp.Num_COPRT_SEQ_NO == nextSequenceNo)
                .FirstOrDefault();

        if (nextCoursePart is { Chr_DELETE_FLG: "Y" })
            nextCoursePart = GetNextCoursePart(nextCoursePart.Num_PK_COPRT_NO, examService);

        return nextCoursePart;
    }

    public static Tbl_COURSE_PART_MSTR GetPreviousCoursePart(int coursePartId, ICoreService examService)
    {
        var coursePart =
            examService.Tbl_COURSE_PART_MSTR_Repository.Get(cp => cp.Num_PK_COPRT_NO == coursePartId)
                .FirstOrDefault();

        // Get Num_COPRT_SEQ_NO for last course part from Tbl_COURSE_PART_MSTR
        if (coursePart?.Num_COPRT_SEQ_NO == null) return null;

        var lastSequenceNo = (int)coursePart.Num_COPRT_SEQ_NO;
        if (lastSequenceNo <= 0)
            throw new Exception("Last Course Part Sequence No is not valid");

        // Get course part for next sequence no from Tbl_COURSE_PART_MSTR
        var prevSequenceNo = lastSequenceNo - 1;
        var prevCoursePart = examService.Tbl_COURSE_PART_MSTR_Repository.Get(cp => 
                cp.Num_FK_CO_CD == coursePart.Num_FK_CO_CD && cp.Num_COPRT_SEQ_NO == prevSequenceNo)
            .FirstOrDefault();
        if (prevCoursePart is { Chr_DELETE_FLG: "Y" })
            prevCoursePart = GetPreviousCoursePart(prevCoursePart.Num_PK_COPRT_NO, examService);

        return prevCoursePart;
    }

    public static double GetExamFee(int instanceId, int courseId, int coursePartId,
        ICoreService examService)
    {
        // First try to get fees for selected course part.
        var feeStructure =
            examService.Tbl_FEE_DTL_Repository.Get(f => f.Num_FK_CO_CD == courseId &&
                                                        f.Num_FK_COPRT_NO == coursePartId &&
                                                        f.Num_FK_FEE_CD == 1 &&
                                                        f.Num_FK_INST_NO == instanceId)
                .FirstOrDefault();

        if (null == feeStructure)
            return 0;
        return Convert.ToDouble(feeStructure.FEE_AMOUNT ?? 0);

    }

    public static FeeStructure GetFeeStructureForExam(int instanceId, int collegeId, int courseId, int coursePartId, bool checkSchedule,
        ICoreService examService)
    {
        var feeStructure = new FeeStructure();

        if (HasCoursePartAdditionalCredits(coursePartId, examService))
            return feeStructure;

        // First try to get fees for selected course part.
        /*var feeStructures = examService.Tbl_FEE_DTL_Repository.Get(f => f.Num_FK_COPRT_NO == coursePartId && f.Num_FK_INST_NO == instanceId).ToList();
        // If not found, check the fees for course 
        if (feeStructures.Count <= 0)
            feeStructures =
                (List<Tbl_FEE_DTL>)
                    examService.Tbl_FEE_DTL_Repository.Get(
                        f =>
                            f.Num_FK_CO_CD == courseId && f.Num_FK_COPRT_NO == 0 &&
                            f.Num_FK_INST_NO == instanceId);*/
        // First try to get fees for selected course part.
        var feeStructures = examService.Tbl_FEE_DTL_Repository.Get(f =>
            f.Num_FK_INST_NO == instanceId && f.Num_FK_COL_CD == collegeId && f.Num_FK_COPRT_NO == coursePartId).ToList();
        // If not found, check the fees for course 
        if (feeStructures.Count <= 0)
            feeStructures = examService.Tbl_FEE_DTL_Repository.Get(f =>
                f.Num_FK_INST_NO == instanceId && f.Num_FK_COL_CD == collegeId && f.Num_FK_CO_CD == courseId && f.Num_FK_COPRT_NO == 0).ToList();

        if (null == feeStructures || feeStructures.Count <= 0)
            throw new Exception("Fee for selected course(" + courseId + ") or course part (" + coursePartId + ") or instance(" + instanceId + ") is not available.");

        /*var schedule = examService.Tbl_EXAM_SCHEDULE_MSTR_Repository.Get(s => 
            s.Num_FK_INST_NO == instanceId && s.Num_FK_COPRT_NO == coursePartId).FirstOrDefault();*/
        var schedule = examService.Tbl_EXAM_SCHEDULE_COURSE_Repository.Get(s =>
            s.Num_FK_INST_NO == instanceId && s.Num_FK_COURSE_NO == courseId).FirstOrDefault();
        if (null == schedule)
            throw new Exception($"Schedule for the Student's Course Part is not available. Instance : {instanceId}, Course : {courseId}");
        if (checkSchedule)
        {
            if (schedule.Dtm_FormFilll_Date == null || DateTime.Now.Date < schedule.Dtm_FormFilll_Date.Value.Date)
                throw new Exception($"Form Filling Start Date is " + schedule.Dtm_FormFilll_Date +
                                    $". Form will not be accepted before this date. Instance : {instanceId}, Course : {courseId}");
            if (schedule.Dtm_Commencement_DateOfExam != null)
            {
                var lastFormFillingDate = schedule.Dtm_Commencement_DateOfExam.Value.AddDays(-5);
                if (DateTime.Now.Date > lastFormFillingDate.Date)
                    throw new Exception("Last Form Filling Date (" + lastFormFillingDate.ToLongDateString() +
                                        ") is over. Form will not be accepted after this date.");
            }
        }

        feeStructure.LateFeeDate = schedule.Dtm_LstDateOfForm_WithoutLateFee;
        feeStructure.SuperLateFeeDate = schedule.Dtm_LstDateOfForm_WithLateFee;

        foreach (var fee in feeStructures)
        {
            if (fee.FEE_AMOUNT == null)
                continue;

            switch (fee.Num_FK_FEE_CD)
            {
                case 1:
                    feeStructure.ExamFee = (double)fee.FEE_AMOUNT;
                    break;
                case 2:
                    feeStructure.CapFee = (double)fee.FEE_AMOUNT;
                    break;
                case 3:
                    feeStructure.StatementOfMarksFee = (double)fee.FEE_AMOUNT;
                    break;
                case 4:
                    feeStructure.CertificateOfPassingFee = 0;
                    var nextCoursePart = GetNextCoursePart(coursePartId, examService);
                    if (null == nextCoursePart)
                        feeStructure.CertificateOfPassingFee = (double)fee.FEE_AMOUNT;
                    break;
                case 5:
                    feeStructure.PracticalFee = (double)fee.FEE_AMOUNT;
                    break;
                case 6:
                    feeStructure.DissertationFee = (double)fee.FEE_AMOUNT;
                    break;
                case 7:
                    feeStructure.OthersFee = (double)fee.FEE_AMOUNT;
                    break;
                case 8:
                    feeStructure.LateFee = 0;
                    if (schedule.Dtm_LstDateOfForm_WithoutLateFee != null &&
                        (schedule.Dtm_LstDateOfForm_WithoutLateFee.Value.Date < DateTime.Now.Date))
                        feeStructure.LateFee = (double)fee.FEE_AMOUNT;
                    break;
                case 9:
                    feeStructure.SuperLateFee = 0;
                    if (schedule.Dtm_LstDateOfForm_WithLateFee != null &&
                        (schedule.Dtm_LstDateOfForm_WithLateFee.Value.Date < DateTime.Now.Date))
                        feeStructure.SuperLateFee = (double)fee.FEE_AMOUNT;
                    break;
            }
        }
        return feeStructure;
    }

    public static FeeStructure GetFeeStructureForReport(int instanceId, int collegeId, int courseId, int coursePartId, bool checkSchedule,
        ICoreService examService)
    {
        var feeStructure = new FeeStructure();

        if (HasCoursePartAdditionalCredits(coursePartId, examService))
            return feeStructure;
        
        // First try to get fees for selected course part.
        var feeStructures = examService.Tbl_FEE_DTL_Repository.Get(f =>
            f.Num_FK_INST_NO == instanceId && f.Num_FK_COL_CD == collegeId && f.Num_FK_COPRT_NO == coursePartId)
            .ToList();
        // If not found, check the fees for course 
        if (feeStructures.Count <= 0)
            feeStructures = (List<Tbl_FEE_DTL>)examService.Tbl_FEE_DTL_Repository.Get(f =>
                f.Num_FK_INST_NO == instanceId && f.Num_FK_COL_CD == collegeId && f.Num_FK_CO_CD == courseId && f.Num_FK_COPRT_NO == 0)
                .ToList();

        if (feeStructures is not { Count: > 0 })
            return feeStructure;
            //throw new Exception("Fee for selected course(" + courseId + ") or course part (" + coursePartId + ") or instance(" + instanceId + ") is not available.");

        var schedule = examService.Tbl_EXAM_SCHEDULE_MSTR_Repository.Get(s => 
            s.Num_FK_INST_NO == instanceId && s.Num_FK_COPRT_NO == coursePartId).FirstOrDefault();
        if (null == schedule)
            throw new Exception("Schedule for the Student's Course Part is not available.");
        if (checkSchedule)
        {
            if (schedule.Dtm_FormFilll_Date == null || DateTime.Now.Date < schedule.Dtm_FormFilll_Date.Value.Date)
                throw new Exception("Form Filling Start Date is " + schedule.Dtm_FormFilll_Date +
                                    ". Form will not be accepted before this date.");
            if (schedule.Dtm_Commencement_DateOfExam != null)
            {
                var lastFormFillingDate = schedule.Dtm_Commencement_DateOfExam.Value.AddDays(-5);
                if (DateTime.Now.Date > lastFormFillingDate.Date)
                    throw new Exception("Last Form Filling Date (" + lastFormFillingDate.ToLongDateString() +
                                        ") is over. Form will not be accepted after this date.");
            }
        }

        feeStructure.LateFeeDate = schedule.Dtm_LstDateOfForm_WithoutLateFee;
        feeStructure.SuperLateFeeDate = schedule.Dtm_LstDateOfForm_WithLateFee;

        foreach (var fee in feeStructures)
        {
            if (fee.FEE_AMOUNT == null)
                continue;

            switch (fee.Num_FK_FEE_CD)
            {
                case 1:
                    feeStructure.ExamFee = (double)fee.FEE_AMOUNT;
                    break;
                case 2:
                    feeStructure.CapFee = (double)fee.FEE_AMOUNT;
                    break;
                case 3:
                    feeStructure.StatementOfMarksFee = (double)fee.FEE_AMOUNT;
                    break;
                case 4:
                    feeStructure.CertificateOfPassingFee = 0;
                    var nextCoursePart = GetNextCoursePart(coursePartId, examService);
                    if (null == nextCoursePart)
                        feeStructure.CertificateOfPassingFee = (double)fee.FEE_AMOUNT;
                    break;
                case 5:
                    feeStructure.PracticalFee = (double)fee.FEE_AMOUNT;
                    break;
                case 6:
                    feeStructure.DissertationFee = (double)fee.FEE_AMOUNT;
                    break;
                case 7:
                    feeStructure.OthersFee = (double)fee.FEE_AMOUNT;
                    break;
                case 8:
                    feeStructure.LateFee = 0;
                    if (schedule.Dtm_LstDateOfForm_WithoutLateFee != null &&
                        (schedule.Dtm_LstDateOfForm_WithoutLateFee.Value.Date < DateTime.Now.Date))
                        feeStructure.LateFee = (double)fee.FEE_AMOUNT;
                    break;
                case 9:
                    feeStructure.SuperLateFee = 0;
                    if (schedule.Dtm_LstDateOfForm_WithLateFee != null &&
                        (schedule.Dtm_LstDateOfForm_WithLateFee.Value.Date < DateTime.Now.Date))
                        feeStructure.SuperLateFee = (double)fee.FEE_AMOUNT;
                    break;
            }
        }
        return feeStructure;
    }

    public static FeeStructure GetFeeStructureForEnvironment(int instanceId,
        ICornoService cornoService)
    {
        var feeStructure = new FeeStructure();

        // Get Environment Month & Year.
        var environmentSetting = cornoService.EnvironmentSettingRepository
            .Get(i => i.InstanceId == instanceId).FirstOrDefault();
        if (null == environmentSetting)
            throw new Exception("Instance No. " + instanceId + " is not created in EnvironmentSetting Table.");

        var lastFormFillingDate = environmentSetting.ExamDate?.AddDays(-5);
        if (DateTime.Today.Date > lastFormFillingDate?.Date)
            throw new Exception("Last Form Filling Date (" + lastFormFillingDate.Value.ToString("dd/MM/yyyy") + ") is over. Form will not be accepted after this date.");

        feeStructure.EnvironmentExamFee = environmentSetting.ExamFee ?? 0;
        feeStructure.EnvironmentLateFeeDate = environmentSetting.LateFeeDate;
        feeStructure.EnvironmentSuperLateFeeDate = environmentSetting.SuperLateFeeDate;
        feeStructure.OthersFee = environmentSetting.OtherFee ?? 0;

        return feeStructure;
    }

    //public static FeeStructure GetFeeStructureForEnvironment(int instanceId, int courseId, int coursePartId,
    //    ICoreService examService)
    //{
    //    var feeStructure = new FeeStructure();
    //    // First try to get fees for selected course part.
    //    var feeStructures =
    //        examService.Tbl_FEE_DTL_Repository.Get(
    //            f => f.Num_FK_COPRT_NO == coursePartId && f.Num_FK_INST_NO == instanceId).ToList();
    //    // If not found, check the fees for course 
    //    if (feeStructures.Count <= 0)
    //        feeStructures =
    //            (List<Tbl_FEE_DTL>)
    //                examService.Tbl_FEE_DTL_Repository.Get(
    //                    f =>
    //                        f.Num_FK_CO_CD == courseId && f.Num_FK_COPRT_NO == 0 &&
    //                        f.Num_FK_INST_NO == instanceId);

    //    if (null == feeStructures || feeStructures.Count <= 0)
    //        throw new Exception("Fee for selected course(" + courseId + ") or course part (0) or instance(" + instanceId + ") is not available.");

    //    var schedule =
    //        examService.Tbl_EXAM_SCHEDULE_MSTR_Repository.Get(
    //            s => s.Num_FK_INST_NO == instanceId && s.Num_FK_COPRT_NO == coursePartId).FirstOrDefault();
    //    if (null == schedule)
    //        throw new Exception("Schedule for the Student's Course Part is not available.");
    //    if (schedule.Dtm_Envi_Examdt != null)
    //    {
    //        var lastFormFillingDate = schedule.Dtm_Envi_Examdt?.AddDays(-5);
    //        if (DateTime.Today.Date > lastFormFillingDate.Value.Date)
    //            throw new Exception("Last Form Filling Date (" + lastFormFillingDate?.ToString("dd/MM/yyyy") + ") is over. Form will not be accepted after this date.");
    //    }

    //    feeStructure.EnvironmentLateFeeDate = schedule.Dtm_EnviForm_WithoutLateFee;
    //    feeStructure.EnvironmentSuperLateFeeDate = schedule.Dtm_EnviForm_WithLateFee;

    //    foreach (var fee in feeStructures)
    //    {
    //        if (fee.FEE_AMOUNT == null)
    //            continue;

    //        switch (fee.Num_FK_FEE_CD)
    //        {
    //            case 7:
    //                feeStructure.OthersFee = (double) fee.FEE_AMOUNT;
    //                break;
    //            case 10:
    //                feeStructure.EnvironmentExamFee = (double) fee.FEE_AMOUNT;
    //                break;
    //            case 11:
    //                feeStructure.EnvironmentLateFee = (double) fee.FEE_AMOUNT;
    //                break;
    //            case 12:
    //                feeStructure.EnvironmentSuperLateFee = (double) fee.FEE_AMOUNT;
    //                break;
    //        }
    //    }

    //    return feeStructure;
    //}

    public static int[] GetTheoryCategories()
    {
        return [2, 10, 26, 27, 48];
    }

    public static int[] GetPracticalCategories()
    {
        return [6, 7, 8, 15, 16, 17, 18, 19, 23, 25, 30, 34, 35, 37, 38, 41, 42, 43, 44, 46, 49, 50, 51, 54, 55, 57, 58, 59, 66, 68];
    }
}