using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_COPART_SYLLABUS_TRXMap : EntityTypeConfiguration<Tbl_COPART_SYLLABUS_TRX>
{
    public Tbl_COPART_SYLLABUS_TRXMap()
    {
        // Primary Key
        HasKey(t => new { t.Num_FK_SYL_NO, t.Num_FK_SUB_CD, t.Chr_SUB_CMP_OPT_FLG, t.Chr_DELETE_FLG });

        // Properties
        Property(t => t.Num_FK_SYL_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_FK_SUB_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Chr_SUB_CMP_OPT_FLG)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_DELETE_FLG)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.var_USR_NM)
            .HasMaxLength(12);

        // Table & Column Mappings
        ToTable("Tbl_COPART_SYLLABUS_TRX");
        Property(t => t.Num_FK_SYL_NO).HasColumnName("Num_FK_SYL_NO");
        Property(t => t.Num_FK_SUB_CD).HasColumnName("Num_FK_SUB_CD");
        Property(t => t.Num_FK_CAT_CD).HasColumnName("Num_FK_CAT_CD");
        Property(t => t.Chr_SUB_CMP_OPT_FLG).HasColumnName("Chr_SUB_CMP_OPT_FLG");
        Property(t => t.Chr_DELETE_FLG).HasColumnName("Chr_DELETE_FLG");
        Property(t => t.var_USR_NM).HasColumnName("var_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
    }
}