using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_SYS_INSTMap : EntityTypeConfiguration<Tbl_SYS_INST>
{
    public Tbl_SYS_INSTMap()
    {
        // Primary Key
        HasKey(t => t.Num_PK_INST_SRNO);

        // Properties
        Property(t => t.Chr_INST_LOCK)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_INST_STATUS)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_INST_DEFA_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Var_INST_REM)
            .IsRequired()
            .HasMaxLength(50);

        Property(t => t.Var_USR_NM)
            .HasMaxLength(12);

        // Table & Column Mappings
        ToTable("Tbl_SYS_INST");
        Property(t => t.Num_PK_INST_SRNO).HasColumnName("Num_PK_INST_SRNO");
        Property(t => t.Num_INST_YEAR).HasColumnName("Num_INST_YEAR");
        Property(t => t.Num_INST_MONTH).HasColumnName("Num_INST_MONTH");
        Property(t => t.Chr_INST_LOCK).HasColumnName("Chr_INST_LOCK");
        Property(t => t.Chr_INST_STATUS).HasColumnName("Chr_INST_STATUS");
        Property(t => t.Chr_INST_DEFA_FLG).HasColumnName("Chr_INST_DEFA_FLG");
        Property(t => t.Var_INST_REM).HasColumnName("Var_INST_REM");
        Property(t => t.Var_USR_NM).HasColumnName("Var_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
        Property(t => t.Num_SEQ_NO).HasColumnName("Num_SEQ_NO");
        Property(t => t.Num_FK_PANEL_NO).HasColumnName("Num_FK_PANEL_NO");
    }
}