using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_GRADE_MSTRMap : EntityTypeConfiguration<Tbl_GRADE_MSTR>
{
    public Tbl_GRADE_MSTRMap()
    {
        // Primary Key
        HasKey(t => new { t.Num_PK_GRD_CD, t.Num_FK_CO_CD });

        // Properties
        Property(t => t.Num_PK_GRD_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_FK_CO_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Var_GRD_NM)
            .IsRequired()
            .HasMaxLength(20);

        Property(t => t.Var_GRD_SHRT_NM)
            .IsRequired()
            .HasMaxLength(10);

        Property(t => t.Chr_DELETE_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Var_USR_NM)
            .HasMaxLength(12);

        // Table & Column Mappings
        ToTable("Tbl_GRADE_MSTR");
        Property(t => t.Num_PK_GRD_CD).HasColumnName("Num_PK_GRD_CD");
        Property(t => t.Num_FK_CO_CD).HasColumnName("Num_FK_CO_CD");
        Property(t => t.Var_GRD_NM).HasColumnName("Var_GRD_NM");
        Property(t => t.Var_GRD_SHRT_NM).HasColumnName("Var_GRD_SHRT_NM");
        Property(t => t.Num_GRD_SEQ_NO).HasColumnName("Num_GRD_SEQ_NO");
        Property(t => t.Num_GRD_POINTS).HasColumnName("Num_GRD_POINTS");
        Property(t => t.Flt_MAX_MRK).HasColumnName("Flt_MAX_MRK");
        Property(t => t.Flt_MIN_MRK).HasColumnName("Flt_MIN_MRK");
        Property(t => t.Chr_DELETE_FLG).HasColumnName("Chr_DELETE_FLG");
        Property(t => t.Var_USR_NM).HasColumnName("Var_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
        Property(t => t.Flt_MAX_GPA).HasColumnName("Flt_MAX_GPA");
        Property(t => t.Flt_MIN_GPA).HasColumnName("Flt_MIN_GPA");
    }
}