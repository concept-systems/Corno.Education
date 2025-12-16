using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Transactions.Mapping
{
    public class RegistrationMap : EntityTypeConfiguration<Registration>
    {
        public RegistrationMap()
        {
            // Primary Key
            HasKey(t => new { t.Id, t.PrnNo});

            // Table & Column Mappings
            ToTable("Registration");
            //Property(t => t.CompanyId).HasColumnName("CompanyID");
            //Property(t => t.SerialNo).HasColumnName("SerialNo");
            //Property(t => t.Code).HasColumnName("Code");
            //Property(t => t.Id).HasColumnName("ID");
            //Property(t => t.StudentName).HasColumnName("StudentName");
            //Property(t => t.FatherName).HasColumnName("FatherName");
            //Property(t => t.MotherName).HasColumnName("MotherName");
            //Property(t => t.StudentEmail).HasColumnName("StudentEmail");
            //Property(t => t.ParentEmail).HasColumnName("ParentEmail");
            //Property(t => t.Year).HasColumnName("Year");
            //Property(t => t.Pincode).HasColumnName("Pincode");
            //Property(t => t.Class).HasColumnName("Class");
            //Property(t => t.Photo).HasColumnName("Photo");
            //Property(t => t.InstanceID).HasColumnName("InstanceID");
            //Property(t => t.FacultyID).HasColumnName("FacultyID");
            //Property(t => t.CentreID).HasColumnName("CentreID");
            //Property(t => t.CourseID).HasColumnName("CourseID");
            //Property(t => t.CoursePartID).HasColumnName("CoursePartID");
            //Property(t => t.CourseTypeID).HasColumnName("CourseTypeID");
            //Property(t => t.CollegeID).HasColumnName("CollegeID");
            //Property(t => t.BranchID).HasColumnName("BranchID");
            //Property(t => t.CountryName).HasColumnName("CountryName");
            //Property(t => t.CountryID).HasColumnName("CountryID");
            //Property(t => t.StateName).HasColumnName("StateName");
            //Property(t => t.StateID).HasColumnName("StateID");
            //Property(t => t.CityName).HasColumnName("CityName");
            //Property(t => t.CityID).HasColumnName("CityID");
            //Property(t => t.CountryName1).HasColumnName("CountryName1");
            //Property(t => t.CountryID1).HasColumnName("CountryID1");
            //Property(t => t.StateName1).HasColumnName("StateName1");
            //Property(t => t.StateID1).HasColumnName("StateID1");
            //Property(t => t.CityName1).HasColumnName("CityName1");
            //Property(t => t.CityID1).HasColumnName("CityID1");
            //Property(t => t.DistrictCentreID).HasColumnName("DistrictCentreID");
            //Property(t => t.FormNo).HasColumnName("FormNo");
            //Property(t => t.Religion).HasColumnName("Religion");
            //Property(t => t.Caste).HasColumnName("Caste");
            //Property(t => t.IfBackward).HasColumnName("IfBackward");
            //Property(t => t.Gender).HasColumnName("Gender");
            //Property(t => t.DOB).HasColumnName("DOB");
            //Property(t => t.PRNNo).HasColumnName("PRNNo");
            //Property(t => t.EligibilityNo).HasColumnName("EligibilityNo");
            //Property(t => t.PRNSerialNo).HasColumnName("PRNSerialNo");
            //Property(t => t.OldPRNNo).HasColumnName("OldPRNNo");
            //Property(t => t.LocalAddress).HasColumnName("LocalAddress");
            //Property(t => t.Phone).HasColumnName("Phone");
            //Property(t => t.Mobile).HasColumnName("Mobile");
            //Property(t => t.Mobile1).HasColumnName("Mobile1");
            //Property(t => t.PermanentAddress).HasColumnName("PermanentAddress");
            //Property(t => t.GuardianName).HasColumnName("GuardianName");
            //Property(t => t.LocalGuardianAddress).HasColumnName("LocalGuardianAddress");
            //Property(t => t.Phone1).HasColumnName("Phone1");
            //Property(t => t.Nationality).HasColumnName("Nationality");
            //Property(t => t.PassportNo).HasColumnName("PassportNo");
            //Property(t => t.ExamName).HasColumnName("ExamName");
            //Property(t => t.LastPassingYear).HasColumnName("LastPassingYear");
            //Property(t => t.LastUniversity).HasColumnName("LastUniversity");
            //Property(t => t.LastSeatNo).HasColumnName("LastSeatNo");
            //Property(t => t.IsLastDistanceEducation).HasColumnName("IsLastDistanceEducation");
            //Property(t => t.IsLastMigrated).HasColumnName("IsLastMigrated");
            //Property(t => t.IsBridgeCourse).HasColumnName("IsBridgeCourse");
            //Property(t => t.IsApproved).HasColumnName("IsApproved");
            //Property(t => t.LastPercentage).HasColumnName("LastPercentage");
            //Property(t => t.Signature).HasColumnName("Signature");
            //Property(t => t.Document1).HasColumnName("Document1");
            //Property(t => t.Document2).HasColumnName("Document2");
            //Property(t => t.Document3).HasColumnName("Document3");
            //Property(t => t.Document4).HasColumnName("Document4");
            //Property(t => t.Document5).HasColumnName("Document5");

            //Property(t => t.BirthPlace).HasColumnName("BirthPlace");
            //Property(t => t.BirthPlaceDistrictName).HasColumnName("BirthPlaceDistrictName");
            //Property(t => t.BirthPlaceDistrictID).HasColumnName("BirthPlaceDistrictID");
            //Property(t => t.Minority).HasColumnName("Minority");
            //Property(t => t.DomicileDistrictName).HasColumnName("DomicileDistrictName");
            //Property(t => t.DomicileDistrictID).HasColumnName("DomicileDistrictID");
            //Property(t => t.DomicileStateName).HasColumnName("DomicileStateName");
            //Property(t => t.DomicileStateID).HasColumnName("DomicileStateID");
            //Property(t => t.NativePlace).HasColumnName("NativePlace");
            //Property(t => t.Area).HasColumnName("Area");
            //Property(t => t.GuardianTelephone).HasColumnName("GuardianTelephone");
            //Property(t => t.RelationshipWithCandidate).HasColumnName("RelationshipWithCandidate");
            //Property(t => t.ParentTelephone).HasColumnName("ParentTelephone");
            //Property(t => t.ParentQualification).HasColumnName("ParentQualification");
            //Property(t => t.ParentOccupation).HasColumnName("ParentOccupation");
            //Property(t => t.ParentOfficeName).HasColumnName("ParentOfficeName");
            //Property(t => t.ParentOfficeTelephone).HasColumnName("ParentOfficeTelephone");
            //Property(t => t.ParentOfficeMobile).HasColumnName("ParentOfficeMobile");
            //Property(t => t.ExamSubjectList).HasColumnName("ExamSubjectList");
            //Property(t => t.QualifyingExamAttempts).HasColumnName("QualifyingExamAttempts");
            //Property(t => t.QualifyingExamTotalMarks).HasColumnName("QualifyingExamTotalMarks");
            //Property(t => t.QualifyingExamOutOfMarks).HasColumnName("QualifyingExamOutOfMarks");
            //Property(t => t.ExtraCurricularActivities).HasColumnName("ExtraCurricularActivities");
            //Property(t => t.CloseRelativeDetails).HasColumnName("CloseRelativeDetails");
            //Property(t => t.AnnualIncome).HasColumnName("AnnualIncome");
            //Property(t => t.BirthPlaceStateName).HasColumnName("BirthPlaceStateName");
            //Property(t => t.BirthPlaceStateID).HasColumnName("BirthPlaceStateID");
            //Property(t => t.BloodGroup).HasColumnName("BloodGroup");
            //Property(t => t.LocalPinCode).HasColumnName("LocalPinCode");
            //Property(t => t.GuardianEmail).HasColumnName("GuardianEmail");
            //Property(t => t.Status).HasColumnName("Status");
            //Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            //Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            //Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            //Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            //Property(t => t.DeletedBy).HasColumnName("DeletedBy");
            //Property(t => t.DeletedDate).HasColumnName("DeletedDate");

            // Relationships
            //this.HasOptional(t => t.Faculty)
            //    .WithMany(t => t.Students)
            //    .HasForeignKey(d => d.FacultyID);
            //this.HasOptional(t => t.Branch)
            //    .WithMany(t => t.Students)
            //    .HasForeignKey(d => d.BranchID);
            //this.HasOptional(t => t.College)
            //    .WithMany(t => t.Students)
            //    .HasForeignKey(d => d.CollegeID);
            //this.HasOptional(t => t.Course)
            //    .WithMany(t => t.Students)
            //    .HasForeignKey(d => d.CourseID);
            //this.HasOptional(t => t.CoursePart)
            //    .WithMany(t => t.Students)
            //    .HasForeignKey(d => d.CoursePartID);
            //this.HasOptional(t => t.Centre)
            //   .WithMany(t => t.Students)
            //   .HasForeignKey(d => d.CentreID);
            //this.HasOptional(t => t.CourseType)
            //   .WithMany(t => t.Students)
            //   .HasForeignKey(d => d.CourseTypeID);
            //this.HasOptional(t => t.City)
            //  .WithMany(t => t.Students)
            //  .HasForeignKey(d => d.CityID);
            //this.HasOptional(t => t.State)
            //   .WithMany(t => t.Students)
            //   .HasForeignKey(d => d.StateID);
            //this.HasOptional(t => t.City)
            // .WithMany(t => t.Students)
            // .HasForeignKey(d => d.CityID1);
            //this.HasOptional(t => t.State)
            //   .WithMany(t => t.Students)
            //   .HasForeignKey(d => d.StateID1);
            //this.HasOptional(t => t.Country)
            //  .WithMany(t => t.Students)
            //  .HasForeignKey(d => d.CountryID);
            // this.HasOptional(t => t.Country)
            //   .WithMany(t => t.Students)
            //   .HasForeignKey(d => d.CountryID1);

            // this.HasOptional(t => t.District)
            //    .WithMany(t => t.Students)
            //    .HasForeignKey(d => d.BirthPlaceDistrictID);
            // this.HasOptional(t => t.District)
            //    .WithMany(t => t.Students)
            //    .HasForeignKey(d => d.DomicileDistrictID);
            // this.HasOptional(t => t.State)
            //    .WithMany(t => t.Students)
            //    .HasForeignKey(d => d.DomicileStateID);
            // this.HasOptional(t => t.State)
            //     .WithMany(t => t.Students)
            //     .HasForeignKey(d => d.BirthPlaceStateID);
        }
    }
}
