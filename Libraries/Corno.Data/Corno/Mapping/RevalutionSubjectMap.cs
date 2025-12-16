using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Corno.Mapping;

public class RevalutionSubjectMap : EntityTypeConfiguration<RevalutionSubject>
{
    public RevalutionSubjectMap()
    {
        // Primary Key
        HasKey(t => t.Id);

        // Table & Column Mappings
        ToTable("RevalutionSubject");
        //this.Property(t => t.CompanyID).HasColumnName("CompanyID");
        //this.Property(t => t.SerialNo).HasColumnName("SerialNo");
        //this.Property(t => t.Code).HasColumnName("Code");
        //this.Property(t => t.ID).HasColumnName("ID");
        //this.Property(t => t.InstanceID).HasColumnName("InstanceID");
        //this.Property(t => t.ExamID).HasColumnName("ExamID");
        //this.Property(t => t.SubjectCode).HasColumnName("SubjectCode");
        //this.Property(t => t.CoursePartID).HasColumnName("CoursePartID");
        //this.Property(t => t.Status).HasColumnName("Status");
        //this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
        //this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
        //this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
        //this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        //this.Property(t => t.DeletedBy).HasColumnName("DeletedBy");
        //this.Property(t => t.DeletedDate).HasColumnName("DeletedDate");

        //// Relationships
        //this.HasOptional(t => t.Exam)
        //    .WithMany(t => t.ExamSubjects)
        //    .HasForeignKey(d => d.ExamID);

    }
}