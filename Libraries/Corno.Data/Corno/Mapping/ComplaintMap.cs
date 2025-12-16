using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Corno.Mapping;

public class ComplaintMap : EntityTypeConfiguration<Complaint>
{
    public ComplaintMap()
    {
        // Primary Key
        HasKey(t => t.Id);

            
        // Table & Column Mappings
        ToTable("Complaint");
        //this.Property(t => t.CompanyID).HasColumnName("CompanyID");
        //this.Property(t => t.SerialNo).HasColumnName("SerialNo");
        //this.Property(t => t.Code).HasColumnName("Code");
        //this.Property(t => t.ID).HasColumnName("ID");
        //this.Property(t => t.StaffID).HasColumnName("StaffID");
        //this.Property(t => t.Name).HasColumnName("Name");
        //this.Property(t => t.IsApproved).HasColumnName("IsApproved");
        //this.Property(t => t.ComplaintDate).HasColumnName("ComplaintDate");
        //this.Property(t => t.ComplaintBy).HasColumnName("ComplaintBy");
        //this.Property(t => t.Description).HasColumnName("Description");
        //this.Property(t => t.AssignedTo).HasColumnName("AssignedTo");
        //this.Property(t => t.AssignedBy).HasColumnName("AssignedBy");
        //this.Property(t => t.AssignedDate).HasColumnName("AssignedDate");
        //this.Property(t => t.ResolvedBy).HasColumnName("ResolvedBy");
        //this.Property(t => t.ResolvedDate).HasColumnName("ResolvedDate");
        //this.Property(t => t.Status).HasColumnName("Status");
        //this.Property(t => t.ClosedBy).HasColumnName("ClosedBy");
        //this.Property(t => t.ClosedDate).HasColumnName("ClosedDate");
        //this.Property(t => t.CreatedBy).HasColumnName("CreatedBy");
        //this.Property(t => t.CreatedDate).HasColumnName("CreatedDate");
        //this.Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
        //this.Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        //this.Property(t => t.DeletedBy).HasColumnName("DeletedBy");
        //this.Property(t => t.DeletedDate).HasColumnName("DeletedDate");

        // Relationships
        //this.HasOptional(t => t.Staff)
        //    .WithMany(t => t.Complaints)
        //    .HasForeignKey(d => d.StaffID);

    }
}