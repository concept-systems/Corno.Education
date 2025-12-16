using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_BRANCH_MSTRMap : EntityTypeConfiguration<Tbl_BRANCH_MSTR>
{
    public Tbl_BRANCH_MSTRMap()
    {
        // Primary Key
        HasKey(t => new { t.Num_FK_FA_CD, t.Num_PK_BR_CD, t.Var_BR_NM, t.Var_BR_SHRT_NM, t.Num_BR_SEQ_NO });

        // Properties
        Property(t => t.Num_FK_FA_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_PK_BR_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Var_BR_NM)
            .IsRequired()
            .HasMaxLength(80);

        Property(t => t.Var_BR_SHRT_NM)
            .IsRequired()
            .HasMaxLength(10);

        Property(t => t.Num_BR_SEQ_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Chr_DELETE_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Var_BR_NM_BL)
            .HasMaxLength(50);

        Property(t => t.Var_USR_NM)
            .HasMaxLength(12);

        // Table & Column Mappings
        ToTable("Tbl_BRANCH_MSTR");
        Property(t => t.Num_FK_FA_CD).HasColumnName("Num_FK_FA_CD");
        Property(t => t.Num_PK_BR_CD).HasColumnName("Num_PK_BR_CD");
        Property(t => t.Var_BR_NM).HasColumnName("Var_BR_NM");
        Property(t => t.Var_BR_SHRT_NM).HasColumnName("Var_BR_SHRT_NM");
        Property(t => t.Num_BR_SEQ_NO).HasColumnName("Num_BR_SEQ_NO");
        Property(t => t.Chr_DELETE_FLG).HasColumnName("Chr_DELETE_FLG");
        Property(t => t.Var_BR_NM_BL).HasColumnName("Var_BR_NM_BL");
        Property(t => t.Var_USR_NM).HasColumnName("Var_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
        Property(t => t.Num_FK_CO_CD).HasColumnName("Num_FK_CO_CD");
    }
}