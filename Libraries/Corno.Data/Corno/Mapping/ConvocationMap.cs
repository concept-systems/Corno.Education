using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Corno.Mapping;

public class ConvocationMap : EntityTypeConfiguration<Convocation>
{
    public ConvocationMap()
    {
        // Primary Key
        HasKey(t => t.Id);

        // Table & Column Mappings
        ToTable("Convocation");
        //this.Property(t => t.CompanyID).HasColumnName("CompanyID");
        //this.Property(t => t.SerialNo).HasColumnName("SerialNo");
        //this.Property(t => t.Code).HasColumnName("Code");
        //this.Property(t => t.ID).HasColumnName("ID");
        //this.Property(t => t.PRNNo).HasColumnName("PRNNo");
        //this.Property(t => t.Address).HasColumnName("Address");
        //this.Property(t => t.Phone).HasColumnName("Phone");
        //this.Property(t => t.StudentID).HasColumnName("StudentID");
        //this.Property(t => t.CollegeID).HasColumnName("CollegeID");
        //this.Property(t => t.CourseID).HasColumnName("CourseID");
        //this.Property(t => t.CoursePartID).HasColumnName("CoursePartID");
        //this.Property(t => t.BranchID).HasColumnName("BranchID");
        //this.Property(t => t.AppearExam).HasColumnName("AppearExam");
        //this.Property(t => t.RecordNo).HasColumnName("RecordNo");
        //this.Property(t => t.ForeignStudent).HasColumnName("ForeignStudent");
        //this.Property(t => t.IsApproved).HasColumnName("IsApproved");
        //this.Property(t => t.Pincode).HasColumnName("Pincode");
        //this.Property(t => t.Destination).HasColumnName("Destination");
        //this.Property(t => t.Email).HasColumnName("Email");
        //this.Property(t => t.PassMonth).HasColumnName("PassMonth");
        //this.Property(t => t.PassYear).HasColumnName("PassYear");
        //this.Property(t => t.CGPA).HasColumnName("CGPA");
        //this.Property(t => t.PrincipleSubject1).HasColumnName("PrincipleSubject1");
        //this.Property(t => t.PrincipleSubject2).HasColumnName("PrincipleSubject2");
        //this.Property(t => t.SeatNo).HasColumnName("SeatNo");
        //this.Property(t => t.InPerson).HasColumnName("InPerson");
        //this.Property(t => t.InAbsentia).HasColumnName("InAbsentia");
        //this.Property(t => t.ByCash).HasColumnName("ByCash");
        //this.Property(t => t.ByDD).HasColumnName("ByDD");
        //this.Property(t => t.FeesAmount).HasColumnName("FeesAmount");
        //this.Property(t => t.DDNo).HasColumnName("DDNo");
        //this.Property(t => t.DrawnOn).HasColumnName("DrawnOn");
        //this.Property(t => t.DDDate).HasColumnName("DDDate");
        //this.Property(t => t.ClassImprovementStudent).HasColumnName("ClassImprovementStudent");
        //this.Property(t => t.Status).HasColumnName("Status");
        //this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
        //this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
        //this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
        //this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        //this.Property(t => t.DeletedBy).HasColumnName("DeletedBy");
        //this.Property(t => t.DeletedDate).HasColumnName("DeletedDate");


        //// Relationships
        //this.HasRequired(t => t.Student)
        //    .WithMany(t => t.Convocations)
        //    .HasForeignKey(d => d.StudentID);
           
        //this.HasRequired(t => t.Branch)
        //    .WithMany(t => t.Convocations)
        //    .HasForeignKey(d => d.BranchID);
        //this.HasRequired(t => t.College)
        //    .WithMany(t => t.Convocations)
        //    .HasForeignKey(d => d.CollegeID);
        //this.HasRequired(t => t.Course)
        //    .WithMany(t => t.Convocations)
        //    .HasForeignKey(d => d.CourseID);
        //this.HasRequired(t => t.CoursePart)
        //    .WithMany(t => t.Convocations)
        //    .HasForeignKey(d => d.CoursePartID);
    }
}