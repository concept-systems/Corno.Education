using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_SUB_CATPAP_MSTRMap : EntityTypeConfiguration<Tbl_SUB_CATPAP_MSTR>
{
    public Tbl_SUB_CATPAP_MSTRMap()
    {
        // Primary Key
        HasKey(t => new { t.Num_FK_COPRT_NO, t.Num_FK_SUB_CD, t.Num_FK_CAT_CD, t.Num_PK_PAP_CD, t.Var_PAP_NM, t.Var_PAP_SHRT_NM, t.Num_PAP_MAX_MRK, t.Num_PAP_PASS_MRK, t.Num_PAP_EXMP_MRK, t.Num_PAP_GRD_MIN, t.Chr_PAP_CODE_FLG, t.CHR_PAP_SEC_APL, t.Chr_DELETE_FLG });

        // Properties
        Property(t => t.Num_FK_COPRT_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_FK_SUB_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_FK_CAT_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_PK_PAP_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Var_PAP_NM)
            .IsRequired()
            .HasMaxLength(50);

        Property(t => t.Var_PAP_SHRT_NM)
            .IsRequired()
            .HasMaxLength(10);

        Property(t => t.Num_PAP_MAX_MRK)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_PAP_EXMP_MRK)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_PAP_GRD_MIN)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Chr_PAP_CODE_FLG)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.CHR_PAP_SEC_APL)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_DELETE_FLG)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.var_USR_NM)
            .HasMaxLength(12);

        Property(t => t.CHR_WRT_UNI_APL)
            .IsFixedLength()
            .HasMaxLength(1);

        // Table & Column Mappings
        ToTable("Tbl_SUB_CATPAP_MSTR");
        Property(t => t.Num_FK_COPRT_NO).HasColumnName("Num_FK_COPRT_NO");
        Property(t => t.Num_FK_SUB_CD).HasColumnName("Num_FK_SUB_CD");
        Property(t => t.Num_FK_CAT_CD).HasColumnName("Num_FK_CAT_CD");
        Property(t => t.Num_PK_PAP_CD).HasColumnName("Num_PK_PAP_CD");
        Property(t => t.Var_PAP_NM).HasColumnName("Var_PAP_NM");
        Property(t => t.Var_PAP_SHRT_NM).HasColumnName("Var_PAP_SHRT_NM");
        Property(t => t.Num_PAP_MAX_MRK).HasColumnName("Num_PAP_MAX_MRK");
        Property(t => t.Num_PAP_PASS_MRK).HasColumnName("Num_PAP_PASS_MRK");
        Property(t => t.Num_PAP_EXMP_MRK).HasColumnName("Num_PAP_EXMP_MRK");
        Property(t => t.Num_PAP_GRD_MIN).HasColumnName("Num_PAP_GRD_MIN");
        Property(t => t.Chr_PAP_CODE_FLG).HasColumnName("Chr_PAP_CODE_FLG");
        Property(t => t.CHR_PAP_SEC_APL).HasColumnName("CHR_PAP_SEC_APL");
        Property(t => t.Chr_DELETE_FLG).HasColumnName("Chr_DELETE_FLG");
        Property(t => t.var_USR_NM).HasColumnName("var_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
        Property(t => t.CHR_WRT_UNI_APL).HasColumnName("CHR_WRT_UNI_APL");
    }
}