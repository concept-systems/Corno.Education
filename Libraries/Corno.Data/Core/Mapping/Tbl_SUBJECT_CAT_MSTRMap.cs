using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_SUBJECT_CAT_MSTRMap : EntityTypeConfiguration<Tbl_SUBJECT_CAT_MSTR>
{
    public Tbl_SUBJECT_CAT_MSTRMap()
    {
        // Primary Key
        HasKey(t => new { t.Num_FK_COPRT_NO, t.Num_FK_SUB_CD, t.Num_FK_CAT_CD, t.Num_CAT_MAX_MRK, t.Num_CAT_PASS_MRK, t.Num_CAT_EXMP_MRK, t.Num_CAT_GRD_MIN });

        // Properties
        Property(t => t.Num_FK_COPRT_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_FK_SUB_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_FK_CAT_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_CAT_MAX_MRK)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_CAT_EXMP_MRK)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_CAT_GRD_MIN)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Chr_CAT_CODE_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.chr_CAT_SUBCAT_APL)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_DELETE_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.var_USR_NM)
            .HasMaxLength(12);

        Property(t => t.CHR_WRT_UNI_APL)
            .IsFixedLength()
            .HasMaxLength(1);

        // Table & Column Mappings
        ToTable("Tbl_SUBJECT_CAT_MSTR");
        Property(t => t.Num_FK_COPRT_NO).HasColumnName("Num_FK_COPRT_NO");
        Property(t => t.Num_FK_SUB_CD).HasColumnName("Num_FK_SUB_CD");
        Property(t => t.Num_FK_CAT_CD).HasColumnName("Num_FK_CAT_CD");
        Property(t => t.Num_CAT_MAX_MRK).HasColumnName("Num_CAT_MAX_MRK");
        Property(t => t.Num_CAT_PASS_MRK).HasColumnName("Num_CAT_PASS_MRK");
        Property(t => t.Num_CAT_EXMP_MRK).HasColumnName("Num_CAT_EXMP_MRK");
        Property(t => t.Num_CAT_GRD_MIN).HasColumnName("Num_CAT_GRD_MIN");
        Property(t => t.Num_CAT_PRINT_SEQ).HasColumnName("Num_CAT_PRINT_SEQ");
        Property(t => t.Chr_CAT_CODE_FLG).HasColumnName("Chr_CAT_CODE_FLG");
        Property(t => t.chr_CAT_SUBCAT_APL).HasColumnName("chr_CAT_SUBCAT_APL");
        Property(t => t.Chr_DELETE_FLG).HasColumnName("Chr_DELETE_FLG");
        Property(t => t.var_USR_NM).HasColumnName("var_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
        Property(t => t.CHR_WRT_UNI_APL).HasColumnName("CHR_WRT_UNI_APL");
        Property(t => t.WEIGHTAGE).HasColumnName("WEIGHTAGE");
    }
}