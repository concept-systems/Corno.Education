using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Masters.Mapping;

public class InstanceMap : EntityTypeConfiguration<Instance>
{
    public InstanceMap()
    {
        // Primary Key
        HasKey(t => new { t.Id });

        //// Properties
        //Property(t => t.CompanyId)
        //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        //Property(t => t.SerialNo)
        //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        //Property(t => t.Code)
        //    .HasMaxLength(100);

        //Property(t => t.Description)
        //    .HasMaxLength(500);

        // Table & Column Mappings
        ToTable("Instance");
        //Property(t => t.CompanyId).HasColumnName("CompanyID");
        //Property(t => t.SerialNo).HasColumnName("SerialNo");
        //Property(t => t.Code).HasColumnName("Code");
        //Property(t => t.Id).HasColumnName("ID");
        //Property(t => t.Name).HasColumnName("Name");
        //Property(t => t.Description).HasColumnName("Description");
        //Property(t => t.StartDate).HasColumnName("StartDate");
        //Property(t => t.EndDate).HasColumnName("EndDate");
        //Property(t => t.IsActive).HasColumnName("IsActive");
        //Property(t => t.CreatedBy).HasColumnName("CreatedBy");
        //Property(t => t.CreatedDate).HasColumnName("CreatedDate");
        //Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
        //Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        //Property(t => t.DeletedBy).HasColumnName("DeletedBy");
        //Property(t => t.DeletedDate).HasColumnName("DeletedDate");
    }
}