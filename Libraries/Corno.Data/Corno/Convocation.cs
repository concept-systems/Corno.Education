using System;
using System.ComponentModel.DataAnnotations;
using Corno.Data.Common;

namespace Corno.Data.Corno;

public class ConvocationCommon : BaseModel
{
    public ConvocationCommon()
    {
        IsApproved = false;
        ClassImprovementStudent = false;
        ForeignStudent = false;
    }

    public int? InstanceId { get; set; }
    public string PrnNo { get; set; }
    public int? StudentId { get; set; }
    public int? CollegeId { get; set; }
    public int? CentreId { get; set; }
    public int? CourseId { get; set; }
    public int? CoursePartId { get; set; }
    public int? BranchId { get; set; }
    public string AppearExam { get; set; }
    public string RecordNo { get; set; }
    public bool ForeignStudent { get; set; }
    public bool IsApproved { get; set; }
    [Required]
    /*[StringLength(6, ErrorMessage = "The pin code not exceed 6 characters.")]*/
    public int? PinCode { get; set; }
    [Required]
    [StringLength(20, ErrorMessage = "The city must not exceed 20 characters.")]
    public string Destination { get; set; }
    [StringLength(50, ErrorMessage = "The email must not exceed 50 characters.")]
    public string Email { get; set; }
    public string PassMonth { get; set; }
    public string PassYear { get; set; }
    public string PassMonth1 { get; set; }
    public string PassYear1 { get; set; }
    public double? Cgpa { get; set; }
    public string ClassCode { get; set; }
    public string PrincipleSubject1 { get; set; }
    public string PrincipleSubject2 { get; set; }
    public string SeatNo { get; set; }
    public bool? InPerson { get; set; }
    public bool? InAbsentia { get; set; }
    public bool? ByCash { get; set; }
    public bool? ByDd { get; set; }
    //public string FeesAmount { get; set; }
    public double TotalFee { get; set; }
    public string DrawnOn { get; set; }
    public int? DdNo { get; set; }
    public DateTime? DdDate { get; set; }
    public bool ClassImprovementStudent { get; set; }
    [Required]
    [StringLength(200, ErrorMessage = "The address must not exceed 200 characters.")]
    public string Address { get; set; }
    [Required]
    [StringLength(14, ErrorMessage = "The mobile must not exceed 14 characters.")]
    public string Phone { get; set; }
    [StringLength(50, ErrorMessage = "The aadhar no must not exceed 50 characters.")]
    public string AdharNo { get; set; }
    public int? ConvocationNo { get; set; }
    public string TransactionId { get; set; }
    public DateTime? PaymentDate { get; set; }
    public DateTime? SettlementDate { get; set; }
    public double? PaidAmount { get; set; }
}

public class Convocation : ConvocationCommon
{
    public virtual Registration Student { get; set; }
}

public class ConvocationViewModel : ConvocationCommon
{
    public double ConvocationFee { get; set; }
    public double PostalChargeInIndia { get; set; }
    public double PostalChargeInAbroad { get; set; }
    public bool InIndia { get; set; }
    public bool InAbroad { get; set; }
    public string AppearExamName { get; set; }
    public string StudentName { get; set; }
    public string CollegeName { get; set; }
    public string CourseName { get; set; }
    public string Gender { get; set; }
    public string CentreName { get; set; }
    public string CoursePartName { get; set; }
    public string CourseTypeName { get; set; }
    public string BranchName { get; set; }
    public int PassingMonth { get; set; }
    public int PassingYear { get; set; }
    public int PassingMonth1 { get; set; }
    public int PassingYear1 { get; set; }
    public short? ClassCode { get; set; }
    public string Class { get; set; }
    public string Grade { get; set; }
    public string Photo { get; set; }
    public string MonthName { get; set; }
    public string ConvocationFeeName { get; set; }


    public void Clear()
    {
        PrnNo = string.Empty;
        StudentName = string.Empty;
        CollegeName = string.Empty;
        CourseName = string.Empty;
        CoursePartName = string.Empty;
        CourseTypeName = string.Empty;
        BranchName = string.Empty;
        Gender = string.Empty;
        SeatNo = string.Empty;
        CentreName = string.Empty;

        ConvocationFee = 0;
        ConvocationFeeName = string.Empty;
        Cgpa = 0;
        Grade = string.Empty;

        Photo = null;
        PostalChargeInAbroad = 0;
        PostalChargeInIndia = 0;
        TotalFee = 0;

        ClassCode = 0;
        Grade = string.Empty;
        Cgpa = 0;
    }
}

public class ConvocationFeeStructure
{
    public ConvocationFeeStructure()
    {
        ConvocationFee = 0;
        Total = 0;
    }
    public double ConvocationFee { get; set; }
    public double Total { get; set; }
}

public class ConvocationIndex
{
    public int? Id { get; set; }
    public string PrnNo { get; set; }
    public string CollegeName { get; set; }
    public string CourseName { get; set; }
    public string Status { get; set; }
    public bool IsApproved { get; set; }
}