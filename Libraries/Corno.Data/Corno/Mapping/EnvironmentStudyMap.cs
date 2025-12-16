using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Corno.Mapping;

public class EnvironmentStudyMap : EntityTypeConfiguration<EnvironmentStudy>
{
    public EnvironmentStudyMap()
    {
        // Primary Key
        HasKey(t => t.Id);

        // Table & Column Mappings
        ToTable("EnvironmentStudy");
        //this.Property(t => t.CompanyID).HasColumnName("CompanyID");
        //this.Property(t => t.SerialNo).HasColumnName("SerialNo");
        //this.Property(t => t.Code).HasColumnName("Code");
        //this.Property(t => t.ID).HasColumnName("ID");
        //this.Property(t => t.PRNNo).HasColumnName("PRNNo");
        //this.Property(t => t.Year).HasColumnName("Year");
        //this.Property(t => t.CourseID).HasColumnName("CourseID");
        //this.Property(t => t.CoursePartID).HasColumnName("CoursePartID");
        //this.Property(t => t.CollegeID).HasColumnName("CollegeID");
        //this.Property(t => t.SubMarks).HasColumnName("SubMarks");
        //this.Property(t => t.SubRes).HasColumnName("SubRes");
        //this.Property(t => t.SubSts).HasColumnName("SubSts");
        //this.Property(t => t.Month).HasColumnName("Month");
        //this.Property(t => t.Status).HasColumnName("Status");
        //this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
        //this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
        //this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
        //this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        //this.Property(t => t.DeletedBy).HasColumnName("DeletedBy");
        //this.Property(t => t.DeletedDate).HasColumnName("DeletedDate");
    }
}