using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Web;
using Corno.Data.Common;

namespace Corno.Data.Transactions
{
    public class TestRegistration : UniversityBaseModel
    {
        public TestRegistration()
        {
            BranchId = 0;
            CentreId = 0;
            //IsLastDistanceEducation = false;
            //IsLastMigrated = false;
            //IsApproved = false;
        }

        [Required(ErrorMessage = "Please Enter Student Name.")]
        public string StudentName { get; set; }
        public string FatherName { get; set; }
        public string MotherName { get; set; }
        public string StudentEmail { get; set; }
        public string ParentEmail { get; set; }
        public string Class { get; set; }
        public int? Pincode { get; set; }
        [Required(ErrorMessage = "Please Enter Year.")]
        public int? Year { get; set; }
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
        public int? LastSeatNo { get; set; }
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

    }
}