using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class ConvocationFeeMap : EntityTypeConfiguration<ConvocationFee>
{
    public ConvocationFeeMap()
    {

        HasKey(t => t.Id);

        //// Properties
        //Property(t => t.Code)
        //    .HasMaxLength(100);

        //Property(t => t.Status)
        //    .HasMaxLength(128);

        //Property(t => t.CreatedBy)
        //    .HasMaxLength(128);

        //Property(t => t.ModifiedBy)
        //    .HasMaxLength(128);

        //Property(t => t.DeletedBy)
        //    .HasMaxLength(128);

        // Table & Column Mappings
        ToTable("ConvocationFee");
        //Property(t => t.CompanyId).HasColumnName("CompanyID");
        //Property(t => t.SerialNo).HasColumnName("SerialNo");
        //Property(t => t.Code).HasColumnName("Code");
        //Property(t => t.Id).HasColumnName("ID");
        //Property(t => t.InstanceID).HasColumnName("InstanceID");
        //Property(t => t.CourseTypeID).HasColumnName("CourseTypeID");
        //Property(t => t.Fee).HasColumnName("Fee");
        //Property(t => t.Status).HasColumnName("Status");
        //Property(t => t.CreatedBy).HasColumnName("CreatedBy");
        //Property(t => t.CreatedDate).HasColumnName("CreatedDate");
        //Property(t => t.ModifiedBy).HasColumnName("ModifiedBy");
        //Property(t => t.ModifiedDate).HasColumnName("ModifiedDate");
        //Property(t => t.DeletedBy).HasColumnName("DeletedBy");
        //Property(t => t.DeletedDate).HasColumnName("DeletedDate");
    }
}