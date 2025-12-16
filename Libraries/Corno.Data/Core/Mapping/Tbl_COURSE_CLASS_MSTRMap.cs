using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_COURSE_CLASS_MSTRMap : EntityTypeConfiguration<Tbl_COURSE_CLASS_MSTR>
{
    public Tbl_COURSE_CLASS_MSTRMap()
    {
        // Primary Key
        HasKey(t => new { t.Num_FK_CO_CD, t.Num_FK_COPRT_NO, t.Num_FK_COPRT_NO_FROM });

        // Properties
        Property(t => t.Num_FK_CO_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_FK_COPRT_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_FK_COPRT_NO_FROM)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Chr_DELETE_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Var_USR_NM)
            .HasMaxLength(12);

        // Table & Column Mappings
        ToTable("Tbl_COURSE_CLASS_MSTR");
        Property(t => t.Num_FK_CO_CD).HasColumnName("Num_FK_CO_CD");
        Property(t => t.Num_FK_COPRT_NO).HasColumnName("Num_FK_COPRT_NO");
        Property(t => t.Num_FK_CLASS_CD).HasColumnName("Num_FK_CLASS_CD");
        Property(t => t.Num_CLASS_MAX_MRK).HasColumnName("Num_CLASS_MAX_MRK");
        Property(t => t.Num_CLASS_MIN_MRK).HasColumnName("Num_CLASS_MIN_MRK");
        Property(t => t.Num_GRD_POINTS).HasColumnName("Num_GRD_POINTS");
        Property(t => t.Num_FK_GRD_NO).HasColumnName("Num_FK_GRD_NO");
        Property(t => t.Num_FK_GRP_CD).HasColumnName("Num_FK_GRP_CD");
        Property(t => t.Num_FK_COPRT_NO_FROM).HasColumnName("Num_FK_COPRT_NO_FROM");
        Property(t => t.Chr_DELETE_FLG).HasColumnName("Chr_DELETE_FLG");
        Property(t => t.Var_USR_NM).HasColumnName("Var_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
    }
}