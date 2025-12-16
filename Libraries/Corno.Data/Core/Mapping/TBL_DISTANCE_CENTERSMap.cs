using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class TBL_DISTANCE_CENTERSMap : EntityTypeConfiguration<TBL_DISTANCE_CENTERS>
{
    public TBL_DISTANCE_CENTERSMap()
    {
        // Primary Key
        HasKey(t => t.Num_PK_DistCenter_ID);

        // Properties
        Property(t => t.Num_PK_DistCenter_ID)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.DIST_CENT_NAME)
            .HasMaxLength(100);

        Property(t => t.Chr_USR_NM)
            .IsFixedLength()
            .HasMaxLength(12);

        // Table & Column Mappings
        ToTable("TBL_DISTANCE_CENTERS");
        Property(t => t.Num_PK_DistCenter_ID).HasColumnName("Num_PK_DistCenter_ID");
        Property(t => t.DIST_CENT_NAME).HasColumnName("DIST_CENT_NAME");
        Property(t => t.Chr_USR_NM).HasColumnName("Chr_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
    }
}