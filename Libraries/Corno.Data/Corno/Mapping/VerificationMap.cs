using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Transactions.Mapping
{
    public class VerificationMap : EntityTypeConfiguration<Verification>
    {
        public VerificationMap()
        {
            // Primary Key
            HasKey(t => new { t.Id });

            // Properties
            //Property(t => t.CompanyId)
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            //Property(t => t.SerialNo)
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            //Property(t => t.Id)
            //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

            //Property(t => t.Status)
            //    .HasMaxLength(128);

            //Property(t => t.CreatedBy)
            //    .HasMaxLength(128);

            //Property(t => t.ModifiedBy)
            //    .HasMaxLength(128);

            //Property(t => t.DeletedBy)
            //    .HasMaxLength(128);

            // Table & Column Mappings
            ToTable("Verification");
            //Property(t => t.CompanyId).HasColumnName("CompanyID");
            //Property(t => t.SerialNo).HasColumnName("SerialNo");
            //Property(t => t.Code).HasColumnName("Code");
            //Property(t => t.Id).HasColumnName("ID");
            //Property(t => t.PRNNo).HasColumnName("PRNNo");
            //Property(t => t.Name).HasColumnName("Name");
            //Property(t => t.BranchID).HasColumnName("BranchID");
            //Property(t => t.CourseID).HasColumnName("CourseID");
            //Property(t => t.CoursePartID).HasColumnName("CoursePartID");
            //Property(t => t.Address1).HasColumnName("Address1");
            //Property(t => t.Address2).HasColumnName("Address2");
            //Property(t => t.Address3).HasColumnName("Address3");
            //Property(t => t.CountryID).HasColumnName("CountryID");
            //Property(t => t.CityID).HasColumnName("CityID");
            //Property(t => t.TehsilID).HasColumnName("TehsilID");
            //Property(t => t.DistrictID).HasColumnName("DistrictID");
            //Property(t => t.StateID).HasColumnName("StateID");
            //Property(t => t.Pincode).HasColumnName("Pincode");
            //Property(t => t.MobileNo).HasColumnName("MobileNo");
            //Property(t => t.Email).HasColumnName("Email");
            //Property(t => t.SubjectID).HasColumnName("SubjectID");
            //Property(t => t.SubjectName).HasColumnName("SubjectName");
            //Property(t => t.Fee).HasColumnName("Fee");
            //Property(t => t.ApplicationNo).HasColumnName("ApplicationNo");
            //Property(t => t.Status).HasColumnName("Status");
            //Property(t => t.CreatedBy).HasColumnName("CreatedBy");
            //Property(t => t.CreatedDate).HasColumnName("CreatedDate");
            //Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
            //Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
            //Property(t => t.DeletedBy).HasColumnName("DeletedBy");
            //Property(t => t.DeletedDate).HasColumnName("DeletedDate");

            // Relationships
            //this.HasOptional(t => t.Branch)
            //    .WithMany(t => t.Verifications)
            //    .HasForeignKey(d => d.BranchID);
            HasOptional(t => t.City)
                .WithMany(t => t.Verifications)
                .HasForeignKey(d => d.CityID);
            HasOptional(t => t.Country)
                .WithMany(t => t.Verifications)
                .HasForeignKey(d => d.CountryID);
            //this.HasOptional(t => t.Course)
            //    .WithMany(t => t.Verifications)
            //    .HasForeignKey(d => d.CourseID);
            //this.HasOptional(t => t.CoursePart)
            //    .WithMany(t => t.Verifications)
            //    .HasForeignKey(d => d.CoursePartID);
            HasOptional(t => t.District)
                .WithMany(t => t.Verifications)
                .HasForeignKey(d => d.DistrictID);
            HasOptional(t => t.State)
                .WithMany(t => t.Verifications)
                .HasForeignKey(d => d.StateID);
            //this.HasOptional(t => t.Subject)
            //    .WithMany(t => t.Verifications)
            //    .HasForeignKey(d => d.TehsilID);

        }
    }
}
