using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Corno.Mapping;

public class RevalutionMap : EntityTypeConfiguration<Revalution>
{
    public RevalutionMap()
    {
        // Primary Key
        HasKey(t => t.Id);

        // Table & Column Mappings
        ToTable("Revalution");
        //this.Property(t => t.CompanyID).HasColumnName("CompanyID");
        //this.Property(t => t.SerialNo).HasColumnName("SerialNo");
        //this.Property(t => t.Code).HasColumnName("Code");
        //this.Property(t => t.ID).HasColumnName("ID");
        //this.Property(t => t.PRNNo).HasColumnName("PRNNo"); 
        //this.Property(t => t.SeatNo).HasColumnName("SeatNo");
        //this.Property(t => t.CourseID).HasColumnName("CourseID");
        //this.Property(t => t.CoursePartID).HasColumnName("CoursePartID");
        //this.Property(t => t.CollegeID).HasColumnName("CollegeID");
        //this.Property(t => t.SubjectID).HasColumnName("SubjectID");
        //this.Property(t => t.RevalutionAndVerification).HasColumnName("RevalutionAndVerification");
        //this.Property(t => t.Verification).HasColumnName("Verification");
        //this.Property(t => t.IsApproved).HasColumnName("IsApproved");
        //this.Property(t => t.Status).HasColumnName("Status");
        //this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
        //this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
        //this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
        //this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        //this.Property(t => t.DeletedBy).HasColumnName("DeletedBy");
        //this.Property(t => t.DeletedDate).HasColumnName("DeletedDate");

        // Relationships
        //this.HasRequired(t => t.College)
        //    .WithMany(t => t.Revalutions)
        //    .HasForeignKey(d => d.CollegeID);
        //this.HasRequired(t => t.Course)
        //    .WithMany(t => t.Revalutions)
        //    .HasForeignKey(d => d.CourseID);
        //this.HasRequired(t => t.CoursePart)
        //    .WithMany(t => t.Revalutions)
        //    .HasForeignKey(d => d.CoursePartID);
        //this.HasRequired(t => t.Subject)
        //    .WithMany(t => t.Revalutions)
        //    .HasForeignKey(d => d.SubjectID);

    }
}