using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Corno.Data.Common;

namespace Corno.Data.Corno;

public class RegistrationCommon : UniversityBaseModel
{
    public RegistrationCommon()
    {
        BranchId = 0;
        CentreId = 0;
        IsLastDistanceEducation = false;
        IsLastMigrated = false;
        IsApproved = false;
        //InstanceId =  (int)HttpContext.Current.Session[ModelConstants.InstanceId];
    }

    [Required(ErrorMessage = "Please Enter Student Name.")]
    [StringLength(100, ErrorMessage = "The student name must not exceed 100 characters.")]
    public string StudentName { get; set; }
    [StringLength(100, ErrorMessage = "The father name must not exceed 100 characters.")]
    public string FatherName { get; set; }
    [StringLength(100, ErrorMessage = "The mother name must not exceed 100 characters.")]
    public string MotherName { get; set; }
    [Required(ErrorMessage = "Please enter email.")]
    [StringLength(100, ErrorMessage = "The email must not exceed 100 characters.")]
    public string StudentEmail { get; set; }
    public string ParentEmail { get; set; }
    public string Class { get; set; }
    public int? Pincode { get; set; }
    [Required(ErrorMessage = "Please Enter Year.")]
    public int? Year { get; set; }
    public int? Month { get; set; }
    public byte[] Photo { get; set; }
        
    public string CountryName { get; set; }
    public int? CountryId { get; set; }
    public string StateName { get; set; }
    public int? StateId { get; set; }
    public string CityName { get; set; }
    public int? CityId { get; set; }
    public string CountryName1 { get; set; }
    public int? CountryId1 { get; set; }
    public string StateName1 { get; set; }
    public int? StateId1 { get; set; }
    public string CityName1 { get; set; }
    public int? CityId1 { get; set; }
    public int? DistrictCentreId { get; set; }
    public int? FormNo { get; set; }
    public string Religion { get; set; }
    public string Caste { get; set; }
    public string IfBackward { get; set; }
    [Required(ErrorMessage = "Please Select Gender")]
    public string Gender { get; set; }
    public DateTime? Dob { get; set; }
    public string PrnNo { get; set; }
    public string EligibilityNo { get; set; }
    public int? PrnSerialNo { get; set; }
    public string OldPrnNo { get; set; }
    public string LocalAddress { get; set; }
    public string Phone { get; set; }
    public string ParentMobileNo { get; set; }
    [Required(ErrorMessage = "Please enter mobile no.")]
    [StringLength(15, ErrorMessage = "The mobile no must not exceed 15 characters.")]
    public string Mobile1 { get; set; }
    public string PermanentAddress { get; set; }
    public string GuardianName { get; set; }
    public string LocalGuardianAddress { get; set; }
    public string GuardianMobileNo { get; set; }
    public string Nationality { get; set; }
    public string PassportNo { get; set; }
    public string ExamName { get; set; }
    public int? LastPassingYear { get; set; }
    public string LastUniversity { get; set; }
    public long? LastSeatNo { get; set; }
    public bool IsLastDistanceEducation { get; set; }
    public bool IsLastMigrated { get; set; }
    public bool IsBridgeCourse { get; set; }
    public bool IsApproved { get; set; }
    public double? LastPercentage { get; set; }
    public byte[] Signature { get; set; }
    public byte[] Document1 { get; set; }
    public byte[] Document2 { get; set; }
    public byte[] Document3 { get; set; }
    public byte[] Document4 { get; set; }
    public byte[] Document5 { get; set; }
    public string BirthPlace { get; set; }
    public string BirthPlaceDistrictName { get; set; }
    public int? BirthPlaceDistrictId { get; set; }
    public int? BirthPlaceCountryId { get; set; }
    public string Minority { get; set; }
    public string DomicileDistrictName { get; set; }
    public int? DomicileDistrictId { get; set; }
    public string DomicileStateName { get; set; }
    public int? DomicileStateId { get; set; }
    public string NativePlace { get; set; }
    public string Area { get; set; }
    public string GuardianTelephone { get; set; }
    public string RelationshipWithCandidate { get; set; }
    public string ParentTelephone { get; set; }
    public string ParentQualification { get; set; }
    public string ParentOccupation { get; set; }
    public string ParentOfficeName { get; set; }
    public string ParentOfficeTelephone { get; set; }
    public string ParentOfficeMobile { get; set; }
    public string ExamSubjectList { get; set; }
    public string QualifyingExamAttempts { get; set; }
    public string QualifyingExamTotalMarks { get; set; }
    public string QualifyingExamOutOfMarks { get; set; }
    public string ExtraCurricularActivities { get; set; }
    public string CloseRelativeDetails { get; set; }
    public string AnnualIncome { get; set; }
    public string BirthPlaceStateName { get; set; }
    public string BirthPlaceCountryName { get; set; }
    public int? BirthPlaceStateId { get; set; }
    public string BloodGroup { get; set; }
    public string LocalPinCode { get; set; }
    public string GuardianEmail { get; set; }
    public string RegisterNo { get; set; }
    public string EnrollmentNo { get; set; }
    public string Pan { get; set; }
    public string AdhaarNo { get; set; }
    public DateTime? AdmissionDate { get; set; }
    public DateTime? LeavingDate { get; set; }
    public string RenewedPassportNo { get; set; }
    public DateTime? PassportIssuedOn { get; set; }
    public DateTime? RenewedPassportIssuedOn { get; set; }
    public string VisaNo { get; set; }
    public DateTime? VisaIssuedOn { get; set; }
    public string IsAdmissionCancelled { get; set; }
    public DateTime? AdmissionCancelledDate { get; set; }
    public string QualifyingExaminations { get; set; }
    public string QualifyingSubjects { get; set; }
    public string SscTotalMarks { get; set; }
    public string SscMarksOutOf { get; set; }
    public string HscTotalMarks { get; set; }
    public string HscMarksOutOf { get; set; }
    public string UgTotalMarks { get; set; }
    public string UgMarksOutOf { get; set; }

    public string ErpNo { get; set; }
    public string AbcId { get; set; }
}

public sealed class Registration : RegistrationCommon
{
    public Registration()
    {
        Exams = new List<Exam>();
    }
    public ICollection<Exam> Exams { get; set; }
    public ICollection<Convocation> Convocations { get; set; }
}

public class RegistrationViewModel : RegistrationCommon
{
    [Display(Name = "Branch Name")]
    public string BranchName { get; set; }
    public string CentreName { get; set; }
    public string CollegeName { get; set; }
    public string CourseName { get; set; }
    public string CoursePartName { get; set; }
    public string CourseTypeName { get; set; }
    public HttpPostedFileBase UploadPhoto { get; set; }
    public HttpPostedFileBase UploadSignature { get; set; }
    public HttpPostedFileBase UploadDocument1 { get; set; }
    public HttpPostedFileBase UploadDocument2 { get; set; }
    public HttpPostedFileBase UploadDocument3 { get; set; }
    public HttpPostedFileBase UploadDocument4 { get; set; }
    public HttpPostedFileBase UploadDocument5 { get; set; }

}

public class RegistrationIndex
{
    public int? Id { get; set; }
    public string Status { get; set; }
    public string StudentName { get; set; }
    public string CollegeName { get; set; }
    public string CourseName { get; set; }
    public string Gender { get; set; }
    public string PrnNo { get; set; }
    public bool IsApproved { get; set; }
    public bool? AdmissionCancelledFlag { get; set; }
    public DateTime? Dob { get; set; }
    public DateTime? ModifiedDate { get; set; }
}