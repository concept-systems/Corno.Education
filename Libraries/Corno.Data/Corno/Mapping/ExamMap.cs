using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Corno.Mapping;

public class ExamMap : EntityTypeConfiguration<Exam>
{
    public ExamMap()
    {
        // Primary Key
        HasKey(t => t.Id)
            .Property(t => t.Id)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
            .IsRequired();

        // Table & Column Mappings
        ToTable("Exam");
        //this.Property(t => t.CompanyID).HasColumnName("CompanyID");
        //this.Property(t => t.SerialNo).HasColumnName("SerialNo");
        //this.Property(t => t.Code).HasColumnName("Code");
        //this.Property(t => t.BundleNo).HasColumnName("BundleNo");
        //this.Property(t => t.ID).HasColumnName("ID");
        //this.Property(t => t.InstanceID).HasColumnName("InstanceID");
        //this.Property(t => t.StudentID).HasColumnName("StudentID");
        //this.Property(t => t.CollegeID).HasColumnName("CollegeID");
        //this.Property(t => t.CourseID).HasColumnName("CourseID");
        //this.Property(t => t.CoursePartID).HasColumnName("CoursePartID");
        //this.Property(t => t.PRNNo).HasColumnName("PRNNo");
        //this.Property(t => t.SeatNo).HasColumnName("SeatNo");
        //this.Property(t => t.Pattern).HasColumnName("Pattern");
        //this.Property(t => t.Part).HasColumnName("Part");
        //this.Property(t => t.Date).HasColumnName("Date");
        //this.Property(t => t.Activity).HasColumnName("Activity");
        //this.Property(t => t.ApplyForImprovement).HasColumnName("ApplyForImprovement");
        //this.Property(t => t.IsApproved).HasColumnName("IsApproved");
        //this.Property(t => t.AttendanceFullfilled).HasColumnName("AttendanceFullfilled");
        //this.Property(t => t.ApprovalDate).HasColumnName("ApprovalDate");
        //this.Property(t => t.Remark).HasColumnName("Remark");
        //this.Property(t => t.FormNo).HasColumnName("FormNo");
        //this.Property(t => t.ExamFee).HasColumnName("ExamFee");
        //this.Property(t => t.BacklogFee).HasColumnName("BacklogFee");
        //this.Property(t => t.StatementMarksFee).HasColumnName("StatementMarksFee");
        //this.Property(t => t.CAPFee).HasColumnName("CAPFee");
        //this.Property(t => t.PassingCertificateFee).HasColumnName("PassingCertificateFee");
        //this.Property(t => t.LateFee).HasColumnName("LateFee");
        //this.Property(t => t.SuperLateFee).HasColumnName("SuperLateFee");
        //this.Property(t => t.EnvironmentalStudiesFee).HasColumnName("EnvironmentalStudiesFee");
        //this.Property(t => t.Total).HasColumnName("Total");
        //this.Property(t => t.Status).HasColumnName("Status");
        //this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
        //this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
        //this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
        //this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        //this.Property(t => t.DeletedBy).HasColumnName("DeletedBy");
        //this.Property(t => t.DeletedDate).HasColumnName("DeletedDate");

        // Relationships

        //HasRequired(t => t.Student)
        //    .WithMany(t => t.Exams)
        //    .HasForeignKey(d => d.StudentId);

        //this.HasRequired(t => t.CoursePart)
        //    .WithMany(t => t.Exams)
        //    .HasForeignKey(d => d.CoursePartID);
        //this.HasRequired(t => t.College)
        //    .WithMany(t => t.Exams)
        //    .HasForeignKey(d => d.CollegeID);
        //this.HasRequired(t => t.Course)
        //    .WithMany(t => t.Exams)
        //    .HasForeignKey(d => d.CourseID);
        //this.HasRequired(t => t.CoursePart)
        //    .WithMany(t => t.Exams)
        //    .HasForeignKey(d => d.CoursePartID);
        //this.HasRequired(t => t.Centre)
        //    .WithMany(t => t.Exams)
        //    .HasForeignKey(d => d.CentreID);
        //this.HasRequired(t => t.CourseType)
        //    .WithMany(t => t.Exams)
        //    .HasForeignKey(d => d.CourseTypeID);

    }
}