using Ganss.Excel;

namespace Corno.Data.Dtos.Online_Education;

public class OnlineStudentImportDto
{
    [Column("PRN")]
    public string Prn { get; set; }

    [Column("College_Id")]
    public int? CollegeId { get; set; }

    [Column("Course_ID")]
    public int? CourseId { get; set; }

    [Column("Part-ID")]
    public int? CoursePartId { get; set; }

    [Column("Branch")]
    public int? BranchId { get; set; }

    [Column("Month of Admission")]
    public int? MonthOfAdmission { get; set; }

    [Column("Year of Admission")]
    public int? Batch { get; set; }

    [Column("Fee ID")]
    public int? FeeId { get; set; }

    [Column("Regular Fee")]
    public double RegularFee { get; set; }

    [Column("BacklogFee")]
    public double BacklogFee { get; set; }
}

/*public class OnlineStudentImportDto
{
    [Column("SourceName")]
    public string SourceName { get; set; }

    [Column("PRN ")]
    public string Prn { get; set; }

    [Column("STUDENT'S NAME")]
    public string Name { get; set; }

    [Column("GENDER")]
    public string Gender { get; set; }

    [Column("MOBILE")]
    public string Mobile { get; set; }

    [Column("PROGRAMME")]
    public string Course { get; set; }

    [Column("ADMISSION_SESSION")]
    public string AdmissionSession { get; set; }

    [Column("BATCH")]
    public string Batch { get; set; }

    [Column("Student type")]
    public string StudentType { get; set; }

    [Column("Regular Fee")]
    public string RegularFee { get; set; }

    [Column("Backlog_fee_Type")]
    public string BacklogFeeType { get; set; }

    [Column("BacklogFee")]
    public string BacklogFee { get; set; }

    [Column("CAP")]
    public double CapFee { get; set; }

    [Column("Statement_Fee")]
    public double StatementFee { get; set; }

    [Column("Passing_Fee")]
    public double PassingFee { get; set; }

    [Column("Late_fee")]
    public double LateFee { get; set; }

    [Column("Super_Late_Fee")]
    public double SuperLateFee { get; set; }

    [Column("Fine")]
    public double Fine { get; set; }

    [Column("Revaluation_Fee")]
    public double RevaluationFee { get; set; }

    [Column("Verification_Fee")]
    public double VerificationFee { get; set; }

    [Column("Other_Fee")]
    public double OtherFee { get; set; }

    [Column("Regular_Fee_Show")]
    public string RegularFeeShow { get; set; }

    [Column("BackLog_Fee_Show")]
    public string BackLogFeeShow { get; set; }

    [Column("Other_Fee_Show")]
    public string OtherFeeShow { get; set; }
}*/