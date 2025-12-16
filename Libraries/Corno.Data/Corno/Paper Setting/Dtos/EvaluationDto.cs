using Ganss.Excel;

namespace Corno.Data.Corno.Paper_Setting.Dtos
{
    public class EvaluationDto
    {
        [Column("Sr.")]
        public int SerialNo { get; set; }

        [Column("Faculty")]
        public string FacultyName { get; set; }

        [Column("User_LoginID")]
        public int StaffId { get; set; }

        [Column("User_Name")]
        public string StaffName { get; set; }

        [Column("Examiner")]
        public string ExaminerType { get; set; }

        [Column("User_MobileNo")]
        public string MobileNo { get; set; }

        [Column("User_Email")]
        public string EmailId { get; set; }

        [Column("Course_Code")]
        public string CourseCode { get; set; }

        [Column("Course_Name")]
        public string CourseName { get; set; }

        [Column("Year")]
        public string CoursePartName { get; set; }

        [Column("UID_Subject_Code")]
        public string SubjectCode { get; set; }

        [Column("Subject_Name")]
        public string SubjectName { get; set; }

        [Column("ExamDate")]
        public string ExamDate { get; set; }

        [Column("User_City")]
        public string CollegeName { get; set; }

        [Column("MinAllocated_AB")]
        public string MinAllocatedAb { get; set; }

        [Column("Evaluated_AB")]
        public string EvaluatedAb { get; set; }

        [Column("NoOfDaysTaken")]
        public string NoOfDaysTaken { get; set; }

        [Column("Remaining")]
        public string Remaining { get; set; }

        [Column("Re Allocated User_Name")]
        public string ReAllocatedUserName { get; set; }

        [Column("Re Allocated User_MobileNo")]
        public string ReAllocatedUserMobileNo { get; set; }

        [Column("Re Allocated User_City")]
        public string ReAllocatedUserCity { get; set; }

        [Column("Re Allocated_User_Email")]
        public string ReAllocatedUserEmail { get; set; }

        [Column("Re Allocated Count")]
        public string ReAllocatedCount { get; set; }
    }
}
