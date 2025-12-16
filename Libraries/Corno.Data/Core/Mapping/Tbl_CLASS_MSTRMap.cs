using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_CLASS_MSTRMap : EntityTypeConfiguration<Tbl_CLASS_MSTR>
{
    public Tbl_CLASS_MSTRMap()
    {
        // Primary Key
        HasKey(t => new { t.Num_PK_CLS_CD, t.Var_CLS_NM, t.Var_CLS_SHRT_NM, t.Num_CLS_SEQ_NO });

        // Properties
        Property(t => t.Num_PK_CLS_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Var_CLS_NM)
            .IsRequired()
            .HasMaxLength(30);

        Property(t => t.Var_CLS_SHRT_NM)
            .IsRequired()
            .HasMaxLength(10);

        Property(t => t.Num_CLS_SEQ_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Var_CLASS_NM_BL)
            .HasMaxLength(30);

        Property(t => t.Chr_DELETE_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Var_USR_NM)
            .HasMaxLength(12);

        // Table & Column Mappings
        ToTable("Tbl_CLASS_MSTR");
        Property(t => t.Num_PK_CLS_CD).HasColumnName("Num_PK_CLS_CD");
        Property(t => t.Var_CLS_NM).HasColumnName("Var_CLS_NM");
        Property(t => t.Var_CLS_SHRT_NM).HasColumnName("Var_CLS_SHRT_NM");
        Property(t => t.Num_CLS_SEQ_NO).HasColumnName("Num_CLS_SEQ_NO");
        Property(t => t.Var_CLASS_NM_BL).HasColumnName("Var_CLASS_NM_BL");
        Property(t => t.Chr_DELETE_FLG).HasColumnName("Chr_DELETE_FLG");
        Property(t => t.Var_USR_NM).HasColumnName("Var_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
    }
}