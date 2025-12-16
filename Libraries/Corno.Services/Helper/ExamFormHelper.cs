using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using Corno.Data.Core;
using Corno.Data.Corno;
using Corno.Data.Helpers;
using Corno.Data.ViewModels;
using Corno.Globals.Constants;
using Corno.Globals.Enums;
using Corno.Services.Common.Interfaces;
using Corno.Services.Core.Interfaces;
using Mapster;
using MoreLinq;

namespace Corno.Services.Helper;

public class ExamFormHelper
{
    public static IEnumerable<ExamSubjectViewModel> FreshSubjects(int coursePartId, int branchId)
    {
        var examService = Bootstrapper.Bootstrapper.Get<ICoreService>();

        // Get Syllabus from TBL_COPART_SYLLABUS against Course Part & Branch
        var syllabuses = examService.Tbl_COPART_SYLLABUS_Repository.Get(s =>
            s.Num_FK_COPRT_NO == coursePartId && s.Num_FK_BR_CD == branchId && s.Chr_DELETE_FLG != "Y");
        if (null == syllabuses)
            return null;

        // Get subjects from for selected syllabus
        var examSubjects = new List<ExamSubjectViewModel>();
        var optionalIndex = 0;
        foreach (var syllabus in syllabuses)
        {
            // Get Syllabus subjects from Tbl_COPART_SYLLABUS_TRX with following conditions
            // Subject count should not be greater than Num_COMPL_SUB & Num_COMPLE_OPT_SUB
            int optionalSubjectCount = syllabus.Num_COMPL_OPT_SUB;
            //int compulsorySubjectIndex = 0;
            //int optionalSubjectIndex = 0;

            if (optionalSubjectCount > 0)
                optionalIndex++;

            var syllabusSubjects =
                examService.Tbl_COPART_SYLLABUS_TRX_Repository.Get(
                    st => st.Num_FK_SYL_NO == syllabus.Num_PK_SYL_NO && st.Chr_DELETE_FLG != "Y");

            foreach (var syllabusSubject in syllabusSubjects)
            {
                string subjectType;
                if (syllabusSubject.Chr_SUB_CMP_OPT_FLG == "C")
                    subjectType = SubjectType.Compulsory.ToString();
                else
                    subjectType = "Optional " + optionalIndex + " ( Select Any " + optionalSubjectCount + " )";

                examSubjects.Add(
                    new ExamSubjectViewModel
                    {
                        RowSelector = subjectType == SubjectType.Compulsory.ToString(),
                        InstanceId = (int)HttpContext.Current.Session[ModelConstants.InstanceId],
                        CoursePartId = syllabus.Num_FK_COPRT_NO,
                        CoursePartName = ExamServerHelper.GetCoursePartName(syllabus.Num_FK_COPRT_NO, examService),
                        //examService.Tbl_COURSE_PART_MSTR_Repository.Get(c => c.Num_PK_COPRT_NO == syllabus.Num_FK_COPRT_NO && c.Chr_DELETE_FLG != "Y").FirstOrDefault().Var_COPRT_DESC,
                        SubjectCode = syllabusSubject.Num_FK_SUB_CD,
                        SubjectName = ExamServerHelper.GetSubjectName(syllabusSubject.Num_FK_SUB_CD, examService),
                        SubjectType = subjectType,
                        OptionalIndex = subjectType.Contains("Optional") ? optionalIndex : 0,
                        MaxOptionalCount = optionalSubjectCount
                    }
                );
            }
        }

        // Additional Course Part Subjects
        // Check Chr_AdditionalCredites Flag == "Y" in Tbl_COURSE_PART_MSTR
        // Get Course ID from Tbl_COURSE_MSTR , Whose Chr_AdditionalCredites Flag == "Y" 
        // Again come to Tbl_COURSE_PART_MSTR and get Course Part ID for Additional Credites Course
        // Retrieve Subjects for this Course Part & add to Failed or Fresh List 
        var coursePart =
            examService.Tbl_COURSE_PART_MSTR_Repository.Get(c => c.Num_PK_COPRT_NO == coursePartId)
                .FirstOrDefault();
        if (coursePart != null && coursePart.Chr_AdditionalCredits != "Y") return examSubjects;

        var additionalCourse =
            examService.Tbl_COURSE_MSTR_Repository.Get(
                c => c.Chr_AdditionalCredits == "Y" && c.Chr_DELETE_FLG != "Y").FirstOrDefault();

        if (null == additionalCourse) return examSubjects;

        var additionCoursePart =
            examService.Tbl_COURSE_PART_MSTR_Repository.Get(
                    c => c.Num_FK_CO_CD == additionalCourse.Num_PK_CO_CD && c.Chr_DELETE_FLG != "Y")
                .FirstOrDefault();

        if (null == additionCoursePart) return examSubjects;

        var additionalSubjects =
            examService.Tbl_SUBJECT_MSTR_Repository.Get(
                s => s.Num_FK_COPRT_NO == additionCoursePart.Num_PK_COPRT_NO && s.Chr_DELETE_FLG != "Y");
        foreach (var additionalSubject in additionalSubjects)
        {
            var tblSubjectMstr =
                examService.Tbl_SUBJECT_MSTR_Repository.Get(
                        s => s.Num_PK_SUB_CD == additionalSubject.Num_PK_SUB_CD && s.Chr_DELETE_FLG != "Y")
                    .FirstOrDefault();
            if (tblSubjectMstr != null)
            {
                examSubjects.Add(
                    new ExamSubjectViewModel
                    {
                        RowSelector = true,
                        InstanceId = (int)HttpContext.Current.Session[ModelConstants.InstanceId],
                        CoursePartId = additionalSubject.Num_FK_COPRT_NO,
                        CoursePartName =
                            ExamServerHelper.GetCoursePartName(additionalSubject.Num_FK_COPRT_NO,
                                examService),
                        //examService.Tbl_COURSE_PART_MSTR_Repository.Get(c => c.Num_PK_COPRT_NO == additionalSubject.Num_FK_COPRT_NO && c.Chr_DELETE_FLG != "Y").FirstOrDefault().Var_COPRT_DESC,
                        SubjectCode = additionalSubject.Num_PK_SUB_CD,
                        SubjectName = tblSubjectMstr.Var_SUBJECT_NM,
                        SubjectType = SubjectType.Additional.ToString()
                    });
            }
        }

        return examSubjects;
    }

    /// <summary>
    ///     Get Fail Subjects
    /// </summary>
    public static IEnumerable<ExamSubjectViewModel> FailSubjects(int instanceNo, int coursePartId, string prnNo)
    {
        var examService = Bootstrapper.Bootstrapper.Get<ICoreService>();

        var examSubjects = new List<ExamSubjectViewModel>();
        // Check Chr_Fail_All_Catgories Flag == F in Tbl_COURSE_PART_MSTR
        // Get all Subject from Tbl_STUDENT_SUBJECT where Chr_St_Sub_Res == "P" or "F" for Course Part ID from YR change

        var coursePart = examService.Tbl_COURSE_PART_MSTR_Repository.Get(c => c.Num_PK_COPRT_NO == coursePartId)
            .FirstOrDefault();
        switch (coursePart)
        {
            case { Chr_FAIL_ALL_CATEGORIES: "F" }:
                {
                    var allSubjects =
                        examService.Tbl_STUDENT_SUBJECT_Repository.Get(
                            s =>
                                s.Num_FK_INST_NO == instanceNo && s.Num_FK_COPRT_NO == coursePartId &&
                                s.Chr_FK_PRN_NO == prnNo);

                    foreach (var subject in allSubjects)
                    {
                        if (ExamServerHelper.HasCoursePartAdditionalCredits(subject.Num_FK_COPRT_NO, examService))
                            continue;

                        examSubjects.Add(
                            new ExamSubjectViewModel
                            {
                                RowSelector = true,
                                InstanceId = (int)HttpContext.Current.Session[ModelConstants.InstanceId],
                                CoursePartId = subject.Num_FK_COPRT_NO,
                                CoursePartName = ExamServerHelper.GetCoursePartName(subject.Num_FK_COPRT_NO, examService),
                                //examService.Tbl_COURSE_PART_MSTR_Repository.Get(c => c.Num_PK_COPRT_NO == subject.Num_FK_COPRT_NO && c.Chr_DELETE_FLG != "Y").FirstOrDefault().Var_COPRT_DESC,
                                SubjectCode = subject.Num_FK_SUB_CD,
                                SubjectName = ExamServerHelper.GetSubjectName(subject.Num_FK_SUB_CD, examService),
                                SubjectType = SubjectType.BackLog.ToString()
                            }
                        );
                    }

                    return examSubjects;
                }
            // 1 ==>    a. Get Num_COPRT_MAX_YRS_TO_CMPLT & Num_COPRT_MAX_ATMPTS_TO_CMPLT from course part
            //          b. Get No. of records form Tbl_STUDENT_EXAMS where Chr_FK_PRN_NO == @PRNNo && Num_FK_COPRT_NO == @CoursePartID
            //          c. check whether studentExams.Count() >= coursePart.Num_COPRT_MAX_ATMPTS_TO_CMPLT
            //          d. check whether Year (First two digits of PRN No.) >= coursePart.Num_COPRT_MAX_ATMPTS_TO_CMPLT
            case null:
                return examSubjects;
        }

        var previousAttemptCount =
            examService.TBL_STUDENT_EXAMS_Repository.Get(
                e => e.Chr_FK_PRN_NO == prnNo && e.Num_FK_COPRT_NO == coursePartId).Count();
        // TODO: Confirm ??????????????   || (DateTime.Now.Year - registrationYear) >= coursePart.Num_COPRT_MAX_YRS_TO_CMPLT)
        if (coursePart.Num_COPRT_MAX_ATMPTS_TO_CMPLT > 0 &&
            previousAttemptCount >= coursePart.Num_COPRT_MAX_ATMPTS_TO_CMPLT)
            // ? || (DateTime.Now.Year - registrationYear) >= coursePart.Num_COPRT_MAX_YRS_TO_CMPLT)
            return examSubjects;

        // 2 ==>    Get subjects from Tbl_STUDENT_SUBJECT Where
        //          a. @PRNNo && b. Chr_ST_SUB_RES == 'F' && c. Chr_ST_SUB_CAN != 'Y && SubjectID not in (a, b, c) && Num_PF_COPRT_NO != Additional Credit's Course Part ID

        // Get Passed Subject from Tbl_STUDENT_SUBJECT Where
        // a. Chr_FK_PRN_NO == @PRNNo && b. Chr_ST_SUB_RES == 'P' && c. Chr_ST_SUB_CAN != 'Y && SubjectID not in (a, b, c) 
        var passSubjects = examService.Tbl_STUDENT_SUBJECT_Repository.Get(s =>
                s.Num_FK_INST_NO == instanceNo && s.Chr_FK_PRN_NO == prnNo && s.Chr_ST_SUB_RES == "P" &&
                s.Chr_ST_SUB_CAN != "Y")
            .Distinct()
            .Select(s => s.Num_FK_SUB_CD);

        // Get Fail Subjects from Tbl_STUDENT_SUBJECT Where
        // a. Chr_FK_PRN_NO == @PRNNo && b. Chr_ST_SUB_RES == 'F' && c. Chr_ST_SUB_CAN != 'Y && SubjectID not in (a, b, c) && Num_PF_COPRT_NO != Additional Credit's Course Part ID
        var failedSubjects =
            examService.Tbl_STUDENT_SUBJECT_Repository.Get(
                s =>
                    s.Num_FK_INST_NO == instanceNo && s.Chr_FK_PRN_NO == prnNo && s.Chr_ST_SUB_RES == "F" &&
                    s.Chr_ST_SUB_CAN != "Y" &&
                    !passSubjects.Contains(s.Num_FK_SUB_CD) && s.Num_FK_COPRT_NO != s.Num_FK_COPRT_NO_AddCr);

        if (null == failedSubjects)
        {
            //var coprtPartInfo = examService.Tbl_STUDENT_YR_CHNG_Repository.Get(c => c.Num_FK_COPRT_NO == coursePartId).FirstOrDefault();
            //var aggregateFail = examService.TBL_STUDENT_EXAMS_Repository.Get(c => c.Num_FK_COPRT_NO == coprtPartInfo.Num_FK_COPRT_NO && c.Chr_PART_TOT_PASSFAIL_FLG == "F").FirstOrDefault();
            var subjectInfo = examService.Tbl_STUDENT_SUBJECT_Repository.Get();
            foreach (var subject in subjectInfo)
            {
                if (ExamServerHelper.HasCoursePartAdditionalCredits(subject.Num_FK_COPRT_NO, examService))
                    continue;

                var tblSubjectMaster = examService.Tbl_SUBJECT_MSTR_Repository.Get().FirstOrDefault();
                if (tblSubjectMaster != null)
                {
                    examSubjects.Add(
                        new ExamSubjectViewModel
                        {
                            RowSelector = true,
                            InstanceId = (int)HttpContext.Current.Session[ModelConstants.InstanceId],
                            CoursePartId = subject.Num_FK_COPRT_NO,
                            CoursePartName =
                                ExamServerHelper.GetCoursePartName(subject.Num_FK_COPRT_NO, examService),
                            //examService.Tbl_COURSE_PART_MSTR_Repository.Get(c => c.Num_PK_COPRT_NO == subject.Num_FK_COPRT_NO && c.Chr_DELETE_FLG != "Y").FirstOrDefault().Var_COPRT_DESC,
                            SubjectCode = subject.Num_FK_SUB_CD,
                            SubjectName = tblSubjectMaster.Var_SUBJECT_NM,
                            SubjectType = SubjectType.BackLog.ToString()
                        }
                    );
                }

                return examSubjects;
            }
        }

        // Get Regular Subjects
        if (failedSubjects == null)
            return examSubjects;
        foreach (var failedSubject in failedSubjects)
        {
            if (ExamServerHelper.HasCoursePartAdditionalCredits(failedSubject.Num_FK_COPRT_NO, examService))
                continue;

            var tblSubjectMstr = examService.Tbl_SUBJECT_MSTR_Repository.Get(
                    s => s.Num_PK_SUB_CD == failedSubject.Num_FK_SUB_CD && s.Chr_DELETE_FLG != "Y")
                .FirstOrDefault();
            if (tblSubjectMstr != null)
            {
                examSubjects.Add(
                    new ExamSubjectViewModel
                    {
                        RowSelector = true,
                        InstanceId = (int)HttpContext.Current.Session[ModelConstants.InstanceId],
                        CoursePartId = failedSubject.Num_FK_COPRT_NO,
                        CoursePartName =
                            ExamServerHelper.GetCoursePartName(failedSubject.Num_FK_COPRT_NO, examService),
                        //examService.Tbl_COURSE_PART_MSTR_Repository.Get(c => c.Num_PK_COPRT_NO == failedSubject.Num_FK_COPRT_NO && c.Chr_DELETE_FLG != "Y").FirstOrDefault().Var_COPRT_DESC,
                        SubjectCode = failedSubject.Num_FK_SUB_CD,
                        SubjectName =
                            tblSubjectMstr
                                .Var_SUBJECT_NM,
                        SubjectType = SubjectType.BackLog.ToString()
                    }
                );
            }
        }


        return examSubjects;
    }

    public static IEnumerable<ExamSubjectViewModel> GetExamSubjects(string prnNo, int coursePartId, int branchId)
    {
        var examService = Bootstrapper.Bootstrapper.Get<ICoreService>();
        var examSubjects = new List<ExamSubjectViewModel>();

        // Get Student info from Online Exam Database.
        var studentCourse =
            examService.Tbl_STUDENT_COURSE_Repository.Get(c => c.Chr_FK_PRN_NO == prnNo).FirstOrDefault();
        if (null == studentCourse)
            throw new Exception("Student for this PRN No is not available");

        // Get last exam details from TBL_STUDENT_EXAMS
        var lastExams = examService.TBL_STUDENT_EXAMS_Repository.Get(e => e.Chr_FK_PRN_NO == prnNo).ToList();
        if (lastExams.Count <= 0)
        {
            // TODO: Wrong implementation. This is temparory for the additional subjects.
            var instanceId = (int)HttpContext.Current.Session[ModelConstants.InstanceId];
            var failSubjects = FailSubjects(instanceId, studentCourse.Num_FK_COPRT_NO, prnNo);
            examSubjects.AddRange(failSubjects);

            if (branchId <= 0 && studentCourse.Num_FK_BR_CD != null)
                branchId = (int)studentCourse.Num_FK_BR_CD;
            var freshSubjects = FreshSubjects(studentCourse.Num_FK_COPRT_NO, branchId);
            examSubjects.AddRange(freshSubjects);

            return examSubjects;
        }

        // Check for Chr_ST_RESULT in Tbl_STUDENT_YR_CHNG Table
        var yrChangeRecord =
            examService.Tbl_STUDENT_YR_CHNG_Repository.Get(s => s.Chr_FK_PRN_NO == prnNo)
                .OrderByDescending(s => s.Num_FK_INST_NO)
                .FirstOrDefault();

        // If user has selected previous exam, then show him only failed subjects.
        if (yrChangeRecord != null && coursePartId < yrChangeRecord.Num_FK_COPRT_NO)
            throw new Exception("You are not allowed to fill the form for this course part");

        if (yrChangeRecord != null && yrChangeRecord.Num_FK_COPRT_NO == coursePartId)
            return FailSubjects(yrChangeRecord.Num_FK_INST_NO, yrChangeRecord.Num_FK_COPRT_NO, prnNo);

        if (yrChangeRecord?.Chr_ST_RESULT == null) return null;

        switch (yrChangeRecord.Chr_ST_RESULT)
        {
            case "A": // Appear   - Don't Care
                break;
            case "D": // Detained - Fail - Fail Subjects
                return FailSubjects(yrChangeRecord.Num_FK_INST_NO,
                    yrChangeRecord.Num_FK_COPRT_NO, prnNo);

            case "F": // Fail - Fail Subjects                         // TODO: (Uncomment It) Temporary arrangement for COVID 
                return FailSubjects(yrChangeRecord.Num_FK_INST_NO,    // TODO: (Uncomment It) Temporary arrangement for COVID 
                    yrChangeRecord.Num_FK_COPRT_NO, prnNo);           // TODO: (Uncomment It) Temporary arrangement for COVID 

            case "N": // Absent - Fail Subjects
                return FailSubjects(yrChangeRecord.Num_FK_INST_NO,
                    yrChangeRecord.Num_FK_COPRT_NO, prnNo);

            case "O": // Lower Exam Fail - Fail Subjects              // TODO: (Uncomment It) Temporary arrangement for COVID 
                return FailSubjects(yrChangeRecord.Num_FK_INST_NO,    // TODO: (Uncomment It) Temporary arrangement for COVID 
                    yrChangeRecord.Num_FK_COPRT_NO, prnNo);           // TODO: (Uncomment It) Temporary arrangement for COVID 

            //case "F":   // TODO: (Remove it) Temporary arrangement for COVID 
            //case "O":   // TODO: (Remove it) Temporary arrangement for COVID 
            case "P": // Pass - Fresh Subjects
                {
                    // Get Last Course Part ID from Tbl_STUDENT_YR_CHNG
                    var nextCoursePart = ExamServerHelper.GetNextCoursePart(yrChangeRecord.Num_FK_COPRT_NO,
                        examService);

                    if (null == nextCoursePart)
                        throw new Exception(
                            "Next course part is not available for this student. His / her all course parts finished.");

                    // If user has selected next exam than required, then show him only failed subjects.
                    if (coursePartId > nextCoursePart.Num_PK_COPRT_NO)
                        throw new Exception("You are not allowed to fill the form for this course part");

                    // If branch ID for last course part is 0 and Chr_COPRT_BRANCH_APP_FLG is 'Y' for next course part, then allow user to select the branch below course part in the form.
                    // Get branches for selected course of next course part and show in the combo box.
                    // TODO : branch combo.


                    // TODO: Wrong implementation. This is temporary for the additional subjects.
                    var failSubjects = FailSubjects(yrChangeRecord.Num_FK_INST_NO,
                        yrChangeRecord.Num_FK_COPRT_NO, prnNo);
                    examSubjects.AddRange(failSubjects);

                    var freshSubjects = FreshSubjects(nextCoursePart.Num_PK_COPRT_NO,
                        branchId > 0 ? branchId : yrChangeRecord.Num_FK_BR_CD);
                    examSubjects.AddRange(freshSubjects);

                    //return FreshSubjects(nextCoursePart.Num_PK_COPRT_NO, yrChangeRecord.Num_FK_BR_CD);
                    return examSubjects;
                }
            case "R": // Reserve - Don't Allow to fill the form.
                break;

            case "T": // ATKT - Fresh + Fail Subjects
                {
                    var nextCoursePart = ExamServerHelper.GetNextCoursePart(yrChangeRecord.Num_FK_COPRT_NO,
                        examService);
                    if (null == nextCoursePart)
                        throw new Exception(
                            "Next course part is not available for this student. His / her all course parts finished.");

                    // If user has selected next exam than required, then show him only failed subjects.
                    if (coursePartId > nextCoursePart.Num_PK_COPRT_NO)
                        throw new Exception("You are not allowed to fill the form for this course part");

                    //var failSubjects = FailSubjects((int)yrChangeRecord.Num_FK_INST_NO, (int)yrChangeRecord.Num_FK_COPRT_NO, (string)viewModel.PRNNo);
                    //var freshSubjects = FreshSubjects(nextCoursePart.Num_PK_COPRT_NO, yrChangeRecord.Num_FK_BR_CD);

                    //examSubjects.AddRange(failSubjects);
                    //examSubjects.AddRange(freshSubjects);

                    var failSubjects = FailSubjects(yrChangeRecord.Num_FK_INST_NO,
                        yrChangeRecord.Num_FK_COPRT_NO, prnNo);
                    examSubjects.AddRange(failSubjects);

                    if (branchId > 0)
                    {
                        var freshSubjects = FreshSubjects(nextCoursePart.Num_PK_COPRT_NO, branchId);
                        examSubjects.AddRange(freshSubjects);
                    }
                    else
                    {
                        var freshSubjects = FreshSubjects(nextCoursePart.Num_PK_COPRT_NO,
                            yrChangeRecord.Num_FK_BR_CD);
                        examSubjects.AddRange(freshSubjects);
                    }

                    return examSubjects;
                }

            case "W": // Not Improved - Fail - Will discuss later
                break;
            case "Z": // Dash - Fresh + Fail Subjects
                {
                    var nextCoursePart = ExamServerHelper.GetNextCoursePart(yrChangeRecord.Num_FK_COPRT_NO,
                        examService);
                    if (null == nextCoursePart)
                        throw new Exception(
                            "Next course part is not available for this student. His / her all course parts finished.");

                    // If user has selected next exam than required, then show him only failed subjects.
                    if (coursePartId > nextCoursePart.Num_PK_COPRT_NO)
                        throw new Exception("You are not allowed to fill the form for this course part");

                    var failSubjects = FailSubjects(yrChangeRecord.Num_FK_INST_NO,
                        yrChangeRecord.Num_FK_COPRT_NO, prnNo);
                    examSubjects.AddRange(failSubjects);

                    if (branchId > 0)
                    {
                        var freshSubjects = FreshSubjects(nextCoursePart.Num_PK_COPRT_NO, branchId);
                        examSubjects.AddRange(freshSubjects);
                    }
                    else
                    {
                        var freshSubjects = FreshSubjects(nextCoursePart.Num_PK_COPRT_NO,
                            yrChangeRecord.Num_FK_BR_CD);
                        examSubjects.AddRange(freshSubjects);
                    }

                    return examSubjects;
                }
        }

        return null;
    }

    public static void GetFeeStructure(ExamViewModel dto, int instanceId)
    {
        var examService = Bootstrapper.Bootstrapper.Get<ICoreService>();

        if (dto.CourseId == null) return;
        if (dto.CoursePartId == null) return;
        if (dto.BranchId == null) return;

        var feeStructure = ExamServerHelper.GetFeeStructureForExam(instanceId, dto.CollegeId.ToInt(), dto.CourseId.ToInt(),
            dto.CoursePartId.ToInt(), true, examService);

        //var feeStructures = examService.Tbl_FEE_DTL_Repository.Get(f => f.Num_FK_COPRT_NO == viewModel.CoursePartID);
        //FeeStructure feeStructure = ExamServerHelper.GetFeeStructure(instanceID, courseID, coursePartID, branchID, examService); ;

        // Get from TBL_STUDENT_YR_CHNG
        var yrChange = examService.Tbl_STUDENT_YR_CHNG_Repository.Get(s =>
                s.Chr_FK_PRN_NO == dto.PrnNo)
                .OrderByDescending(s => s.Num_FK_INST_NO)
                .FirstOrDefault();
        if (null != yrChange)
        {
            if (yrChange.Chr_ST_RESULT is "F" or "O" || dto.CoursePartId == yrChange.Num_FK_COPRT_NO)
            {
                feeStructure.ExamFee = 0;
                feeStructure.StatementOfMarksFee = 0;
                feeStructure.CapFee = 0;
                feeStructure.DissertationFee = 0;
                feeStructure.CertificateOfPassingFee = 0;
            }

            var subjects =
                FailSubjects(yrChange.Num_FK_INST_NO, yrChange.Num_FK_COPRT_NO, dto.PrnNo).ToList();
            foreach (var line in subjects.GroupBy(info => info.CoursePartId)
                         .Select(group => new
                         {
                             CoursePartID = group.Key,
                             Count = group.Count()
                         })
                         .OrderBy(x => x.CoursePartID))
            {
                var fee = ExamServerHelper.GetFeeStructureForExam(instanceId, dto.CollegeId.ToInt(), dto.CourseId.ToInt(),
                    line.CoursePartID.ToInt(), false, examService);
                if (null == fee) continue;

                double backLogFee = 0;
                if (line.Count <= 3)
                {
                    var coursePart = ExamServerHelper.GetCoursePart(line.CoursePartID, examService);
                    if (coursePart is { Num_COPRT_TOT_SUBJECT: <= 3 } &&
                        line.Count >= coursePart.Num_COPRT_TOT_SUBJECT)
                        backLogFee += fee.ExamFee;
                    else
                        backLogFee += fee.ExamFee / 2;
                }
                else
                    backLogFee += fee.ExamFee;

                feeStructure.BacklogFee += (int)backLogFee; //(int) Math.Ceiling(backLogFee / 10) * 10;
                feeStructure.CapFee += fee.CapFee;
                feeStructure.StatementOfMarksFee += fee.StatementOfMarksFee;
                var examSubjectViewModel = subjects.FirstOrDefault(s => s.CoursePartId == (line.CoursePartID ?? 0));
                if (examSubjectViewModel != null)
                    feeStructure.BacklogSummary += examSubjectViewModel.CoursePartName + ": " +
                                                   feeStructure.BacklogFee + ", ";
            }
        }

        if (dto.CoursePartId != null && dto.BranchId != null)
        {
            dto.ExamSubjectViewModels = GetExamSubjects(dto.PrnNo, (int)dto.CoursePartId,
                (int)dto.BranchId).ToList();

            var coursePartCount = dto.ExamSubjectViewModels.Where(e => e.CoursePartId != 1247)
                .DistinctBy(e => e.CoursePartId).Count();
            feeStructure.Total -= feeStructure.LateFee;
            feeStructure.Total -= feeStructure.SuperLateFee;
            if (feeStructure.LateFee > 0)
                feeStructure.LateFee *= coursePartCount;
            if (feeStructure.SuperLateFee > 0)
            {
                feeStructure.SuperLateFee *= coursePartCount;
                feeStructure.LateFee = 0;
            }

            feeStructure.Total += feeStructure.LateFee;
            feeStructure.Total += feeStructure.SuperLateFee;
        }

        //AutoMapperConfig.CornoMapper.Map(feeStructure, viewModel);
        // Copy fee structure to Dto
        feeStructure.Adapt(dto);

        // Calculate total.
        dto.Total = feeStructure.GetTotal();
        var amountInWordService = Bootstrapper.Bootstrapper.Get<IAmountInWordsService>();
        dto.FeeInWord = amountInWordService.GetAmountInWords(dto.Total.ToString());
    }

    public static void GetPrnInfoForPaymentGateway(ExamViewModel viewModel)
    {
        // Check for invalid PRN No
        if (string.IsNullOrEmpty(viewModel.PrnNo))
            throw new Exception("Invalid PRN No");

        var examService = Bootstrapper.Bootstrapper.Get<ICoreService>();
        var existing = examService.Tbl_APP_TEMP_Repository.Get(e =>
            e.Chr_APP_PRN_NO == viewModel.PrnNo && e.Num_FK_INST_NO == viewModel.InstanceId).FirstOrDefault();
        if (null != existing)
            throw new Exception($"Exam form for this PRN {viewModel.PrnNo} already exists(Temp).");

        var college = examService.TBL_COLLEGE_MSTRRepository.Get(c => c.Num_PK_COLLEGE_CD == viewModel.CollegeId)
            .FirstOrDefault();
        viewModel.CollegeName = $"({viewModel.CollegeId}) {college?.Var_CL_COLLEGE_NM1}";

        var center = examService.TBL_DISTANCE_CENTERS_Repository.Get(c => c.Num_PK_DistCenter_ID == viewModel.CentreId)
            .FirstOrDefault();
        viewModel.CentreName = viewModel.CentreId > 0 ? $"({viewModel.CentreId}) {center?.DIST_CENT_NAME}" : "-";

        viewModel.CourseTypeName = ExamServerHelper.GetCourseTypeName(viewModel.CoursePartId, examService);

        var course = examService.Tbl_COURSE_MSTR_Repository.Get(c => c.Num_PK_CO_CD == viewModel.CourseId)
            .FirstOrDefault();
        viewModel.CourseName = $"({viewModel.CourseId}) {course?.Var_CO_NM}";

        var coursePart = examService.Tbl_COURSE_PART_MSTR_Repository.Get(c => c.Num_PK_COPRT_NO == viewModel.CoursePartId)
            .FirstOrDefault();
        viewModel.CoursePartName = $"({viewModel.CoursePartId}) {coursePart?.Var_COPRT_DESC}";
        if (coursePart is { Chr_DEG_APL_FLG: "Y" })
            viewModel.DegreeApplicable = true;

        //Get from TBL_BRANCH_MSTR
        if (viewModel.BranchId > 0)
        {
            var branch = examService.Tbl_BRANCH_MSTR_Repository.Get(c => c.Num_PK_BR_CD == viewModel.BranchId)
                .FirstOrDefault();
            viewModel.BranchName = $"({viewModel.BranchId}) {branch?.Var_BR_NM}";
        }

        var studentInfo = examService.Tbl_STUDENT_INFO_Repository.Get(s => s.Chr_PK_PRN_NO == viewModel.PrnNo)
            .FirstOrDefault();
        viewModel.StudentName = studentInfo?.Var_ST_NM?.Trim();
        viewModel.AadharNo = studentInfo?.AadharNo?.Trim();

        var studentInfoAdr = examService.Tbl_STUDENT_INFO_ADR_Repository.Get(s => s.Chr_FK_PRN_NO == viewModel.PrnNo)
            .FirstOrDefault();
        viewModel.Mobile = studentInfoAdr?.Num_MOBILE;
        viewModel.Email = studentInfoAdr?.Chr_Student_Email;
        if (studentInfoAdr?.Ima_ST_PHOTO != null)
            viewModel.Photo = studentInfoAdr.Ima_ST_PHOTO;
        else
        {
            var photo1 = examService.REG_TEMP_Repository.Get(c => c.Chr_REG_PRN_NO == viewModel.PrnNo).FirstOrDefault();
            if (photo1 != null)
                viewModel.Photo = photo1.Ima_REG_PHOTO;
        }

        viewModel.FormNo = "1";
        viewModel.Bundle = viewModel.CoursePartId?.ToString();

        // This call is necessary.
        GetFeeStructure(viewModel, viewModel.InstanceId);
    }

    public static void GetPrnInfo(ExamViewModel viewModel)
    {
        viewModel.ShowBranchCombo = false;

        // Check for invalid PRN No
        if (string.IsNullOrEmpty(viewModel.PrnNo))
            throw new Exception("Invalid PRN No");

        // Check for existing PRN No for Current Instance
        //var instanceId = (int)HttpContext.Current.Session[ModelConstants.InstanceId];
        var instanceId = viewModel.InstanceId;

        //var transactionService = (ITransactionService) Bootstrapper.GetService(typeof(TransactionService));
        //var existingExam =
        //    transactionService.ExamRepository.Get(e => e.PrnNo == viewModel.PrnNo && e.InstanceId == instanceId)
        //        .ToList();
        //if (existingExam.Count > 0)
        //    throw new Exception("Exam form for this PRN " + viewModel.PrnNo + " already exists.");

        var examService = Bootstrapper.Bootstrapper.Get<ICoreService>();
        var existingExam =
            examService.Tbl_APP_TEMP_Repository
                .Get(e => e.Chr_APP_PRN_NO == viewModel.PrnNo && e.Num_FK_INST_NO == instanceId)
                .ToList();
        if (existingExam.Count > 0)
            throw new Exception($"Exam form for this PRN {viewModel.PrnNo} already exists.");

        // Get from TBL_STUDENT_YR_CHNG
        var yearChangeRecord =
            examService.Tbl_STUDENT_YR_CHNG_Repository.Get(s => s.Chr_FK_PRN_NO == viewModel.PrnNo)
                .OrderByDescending(s => s.Num_FK_INST_NO)
                .FirstOrDefault();
        if (null == yearChangeRecord)
        {
            // Go to TBL_STUDENT_COURSE - First Year
            var studentCourse =
                examService.Tbl_STUDENT_COURSE_Repository.Get(s => s.Chr_FK_PRN_NO == viewModel.PrnNo)
                    .FirstOrDefault();
            if (null == studentCourse)
                throw new Exception("Student course for prn " + viewModel.PrnNo + " does not exists!");

            if (null != studentCourse.Num_FK_DistCenter_ID)
                viewModel.CentreId = (int)studentCourse.Num_FK_DistCenter_ID;
            if (studentCourse.Num_ST_COLLEGE_CD != null)
                viewModel.CollegeId = (int)studentCourse.Num_ST_COLLEGE_CD;
            viewModel.CoursePartId = studentCourse.Num_FK_COPRT_NO;

            if ((null == viewModel.BranchId || viewModel.BranchId <= 0) && null != studentCourse.Num_FK_BR_CD)
                viewModel.BranchId = (int)studentCourse.Num_FK_BR_CD;

            var coursePartFirst = examService.Tbl_COURSE_PART_MSTR_Repository
                .Get(cp => cp.Num_PK_COPRT_NO == viewModel.CoursePartId).FirstOrDefault();
            if (coursePartFirst is { Chr_COPRT_BRANCH_APP_FLG: "Y" })
            {
                viewModel.ShowBranchCombo = true;
            }
        }
        else
        {
            if (null != yearChangeRecord.Num_FK_DistCenter_ID)
                viewModel.CentreId = (int)yearChangeRecord.Num_FK_DistCenter_ID;

            viewModel.CollegeId = yearChangeRecord.Num_FK_COL_CD;

            Tbl_COURSE_PART_MSTR nextCoursePart;
            // Here 1. User can entry PRN No and Course part is auto selected. 2. User selects branch or course part.
            if (viewModel.CoursePartId is > 0)
            {
                nextCoursePart =
                    examService.Tbl_COURSE_PART_MSTR_Repository.Get(cp => cp.Num_PK_COPRT_NO ==
                                                                          viewModel.CoursePartId).FirstOrDefault();
            }
            else if (yearChangeRecord.Chr_ST_RESULT == "P" ||
                     yearChangeRecord.Chr_ST_RESULT == "T" ||
                     yearChangeRecord.Chr_ST_RESULT == "Z")// ||
                                                           //yearChangeRecord.Chr_ST_RESULT == "F" || // TODO: (Remove it) Temporary arrangement for COVID 
                                                           //yearChangeRecord.Chr_ST_RESULT == "O"   // TODO: (Remove it) Temporary arrangement for COVID 
                                                           //)
            {
                nextCoursePart = ExamServerHelper.GetNextCoursePart(yearChangeRecord.Num_FK_COPRT_NO,
                    examService);

                if (null == nextCoursePart) //&&
                                            //yearChangeRecord.Chr_ST_RESULT != "F" && // TODO: (Remove it) Temporary arrangement for COVID 
                                            //yearChangeRecord.Chr_ST_RESULT != "O")   // TODO: (Remove it) Temporary arrangement for COVID 
                {
                    throw new Exception("Next course part is not available for this student. " +
                                        $"His / her all course parts finished.");
                }

                //if (null == nextCoursePart &&
                //    (yearChangeRecord.Chr_ST_RESULT == "F" || // TODO: (Remove it) Temporary arrangement for COVID 
                //    yearChangeRecord.Chr_ST_RESULT == "O"))   // TODO: (Remove it) Temporary arrangement for COVID 
                //{
                //    viewModel.CoursePartId = yearChangeRecord.Num_FK_COPRT_NO;      // TODO: (Remove it) Temporary arrangement for COVID 
                //    nextCoursePart = examService.Tbl_COURSE_PART_MSTR_Repository    // TODO: (Remove it) Temporary arrangement for COVID 
                //        .Get(cp => cp.Num_PK_COPRT_NO == viewModel.CoursePartId).FirstOrDefault();  // TODO: Temporary arrangement for COVID 
                //}
                //else
                viewModel.CoursePartId = nextCoursePart?.Num_PK_COPRT_NO;
            }
            else
            {
                viewModel.CoursePartId = yearChangeRecord.Num_FK_COPRT_NO;
                nextCoursePart = examService.Tbl_COURSE_PART_MSTR_Repository
                    .Get(cp => cp.Num_PK_COPRT_NO == viewModel.CoursePartId).FirstOrDefault();
            }

            if (nextCoursePart is { Chr_COPRT_BRANCH_APP_FLG: "Y" })
            {
                //ViewBag.Branches = examService.Tbl_BRANCH_MSTR_Repository.Get(b => b.Num_FK_CO_CD == nextCoursePart.Num_FK_CO_CD).Select(b => new { ID = b.Num_PK_BR_CD, Name = b.Var_BR_NM });
                viewModel.ShowBranchCombo = true;
                //viewModel.BranchID = 0;
            }
            else
            {
                viewModel.BranchId = 0;
                viewModel.BranchName = "-";
            }

            if (viewModel.BranchId == null)
            {
                viewModel.BranchId = yearChangeRecord.Num_FK_BR_CD;
            }
        }

        // Get Student Name from Tbl_STUDENT_INFO
        viewModel.StudentName = ExamServerHelper.GetStudentName(viewModel.PrnNo, examService);
        viewModel.AadharNo = ExamServerHelper.GetStudentAdharNo(viewModel.PrnNo, examService);

        // Get College from TBL_COLLEGE_MSTR
        viewModel.CollegeName = "(" + viewModel.CollegeId + ") " +
                                ExamServerHelper.GetCollegeName(viewModel.CollegeId, examService);

        // Get Centre from TBL_DISTANCE_CENTRES 
        if (viewModel.CentreId > 0)
            viewModel.CentreName = "(" + viewModel.CentreId + ") " +
                                   ExamServerHelper.GetCentreName(viewModel.CentreId, examService);
        else
            viewModel.CentreName = "-";

        viewModel.CourseId = ExamServerHelper.GetCourseId(viewModel.CoursePartId, examService);
        viewModel.CourseName = "(" + viewModel.CourseId + ") " +
                               ExamServerHelper.GetCourseNameFromCoursePartId(viewModel.CoursePartId, examService);

        // Get Num_FK_TYP_CD from TBL_COURSE_MSTR. Get Name from TBL_COURSE_TYPE_MSTR
        viewModel.CourseTypeName = ExamServerHelper.GetCourseTypeName(viewModel.CoursePartId, examService);

        // Get Course Part from Tbl_COURSE_PART_MSTR
        viewModel.CoursePartName = ExamServerHelper.GetCoursePartName(viewModel.CoursePartId, examService);

        //var copart = examService.Tbl_COURSE_PART_MSTR_Repository.Get(c => c.Num_PK_COPRT_NO == viewModel.CoursePartID).FirstOrDefault();
        //if (copart.Chr_DEG_APL_FLG == "Y")
        //{
        //    if (null != viewModel.UploadPhoto)
        //    {
        //        model.Photo = new byte[viewModel.UploadPhoto.ContentLength];
        //        viewModel.UploadPhoto.InputStream.Read(model.Photo, 0, model.Photo.Length);
        //    }
        //}

        var studentInfo =
            examService.Tbl_STUDENT_INFO_ADR_Repository.Get(c => c.Chr_FK_PRN_NO == viewModel.PrnNo)
                .FirstOrDefault();
        if (studentInfo?.Ima_ST_PHOTO != null)
            viewModel.Photo = studentInfo.Ima_ST_PHOTO;
        viewModel.Mobile = studentInfo?.Num_MOBILE;
        viewModel.Email = studentInfo?.Chr_Student_Email;
        //viewModel.StudentPhoto = "data:image;base64, " + System.Convert.ToBase64String(photo.Ima_ST_PHOTO);

        if (studentInfo?.Ima_ST_PHOTO == null)
        {
            var photo1 =
                examService.REG_TEMP_Repository.Get(c => c.Chr_REG_PRN_NO == viewModel.PrnNo).FirstOrDefault();
            if (photo1 != null) viewModel.Photo = photo1.Ima_REG_PHOTO;
        }

        //Get from TBL_BRANCH_MSTR
        if (false == viewModel.ShowBranchCombo && viewModel.BranchId > 0)
            viewModel.BranchName = "(" + viewModel.BranchId + ") " +
                                   ExamServerHelper.GetBranchName(viewModel.BranchId, examService);

        // This call is necessary.
        GetFeeStructure(viewModel, instanceId);

        // Check whether this course part contains degree applicable flag.
        var coursePart =
            examService.Tbl_COURSE_PART_MSTR_Repository.Get(c => c.Num_PK_COPRT_NO == viewModel.CoursePartId)
                .FirstOrDefault();
        if (coursePart is { Chr_DEG_APL_FLG: "Y" })
            viewModel.DegreeApplicable = true;
    }

    public static void ReceiveChallan(string prn)
    {
        // Check for existing PRN No for Current Instance
        var instanceId = (int)HttpContext.Current.Session[ModelConstants.InstanceId];

        var examService = Bootstrapper.Bootstrapper.Get<ICoreService>();
        var existingRecord = examService.Tbl_APP_TEMP_Repository
            .Get(e => e.Chr_APP_PRN_NO == prn && e.Num_FK_INST_NO == instanceId).FirstOrDefault();
        if (null == existingRecord)
            throw new Exception("Examination form for this PRN " + prn + " does not exists");

        if (existingRecord.Chr_Fee_Submit == "Y")
            throw new Exception("Challan for this PRN " + prn + " is already received.");

        existingRecord.Chr_Fee_Submit = "Y";
        examService.Tbl_APP_TEMP_Repository.Update(existingRecord);
        examService.Save();
    }
}