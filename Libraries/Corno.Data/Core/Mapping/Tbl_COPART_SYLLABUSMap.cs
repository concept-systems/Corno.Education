using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_COPART_SYLLABUSMap : EntityTypeConfiguration<Tbl_COPART_SYLLABUS>
{
    public Tbl_COPART_SYLLABUSMap()
    {
        // Primary Key
        HasKey(t => new { t.Num_PK_SYL_NO, t.Num_FK_COPRT_NO, t.Num_FK_PAT_CD, t.Num_FK_BR_CD, t.Num_FK_GRP_CD, t.Num_FK_SUB_GRP_CD, t.NUM_FK_SUB_SUB_GRP_CD, t.Num_GRP_MAX_MRK, t.Num_GRP_PASS_MRK, t.Num_GRP_EXMP_MRK, t.Num_GRP_GRD_MIN, t.Num_COMPL_SUB, t.Num_COMPL_OPT_SUB, t.Num_OPT_SUB, t.Chr_MUST_PASS_FLG, t.Chr_INCL_FLG, t.Chr_PASS_FLG, t.Chr_DELETE_FLG });

        // Properties
        Property(t => t.Num_PK_SYL_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_FK_COPRT_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_FK_PAT_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_FK_BR_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_FK_GRP_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_FK_SUB_GRP_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.NUM_FK_SUB_SUB_GRP_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_GRP_MAX_MRK)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_GRP_PASS_MRK)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_GRP_EXMP_MRK)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_GRP_GRD_MIN)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_COMPL_SUB)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_COMPL_OPT_SUB)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_OPT_SUB)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Chr_MUST_PASS_FLG)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_INCL_FLG)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_PASS_FLG)
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
        ToTable("Tbl_COPART_SYLLABUS");
        Property(t => t.Num_PK_SYL_NO).HasColumnName("Num_PK_SYL_NO");
        Property(t => t.Num_FK_COPRT_NO).HasColumnName("Num_FK_COPRT_NO");
        Property(t => t.Num_FK_PAT_CD).HasColumnName("Num_FK_PAT_CD");
        Property(t => t.Num_FK_BR_CD).HasColumnName("Num_FK_BR_CD");
        Property(t => t.Num_FK_GRP_CD).HasColumnName("Num_FK_GRP_CD");
        Property(t => t.Num_FK_SUB_GRP_CD).HasColumnName("Num_FK_SUB_GRP_CD");
        Property(t => t.NUM_FK_SUB_SUB_GRP_CD).HasColumnName("NUM_FK_SUB_SUB_GRP_CD");
        Property(t => t.Num_GRP_MAX_MRK).HasColumnName("Num_GRP_MAX_MRK");
        Property(t => t.Num_GRP_PASS_MRK).HasColumnName("Num_GRP_PASS_MRK");
        Property(t => t.Num_GRP_EXMP_MRK).HasColumnName("Num_GRP_EXMP_MRK");
        Property(t => t.Num_GRP_GRD_MIN).HasColumnName("Num_GRP_GRD_MIN");
        Property(t => t.Num_COMPL_SUB).HasColumnName("Num_COMPL_SUB");
        Property(t => t.Num_COMPL_OPT_SUB).HasColumnName("Num_COMPL_OPT_SUB");
        Property(t => t.Num_OPT_SUB).HasColumnName("Num_OPT_SUB");
        Property(t => t.Chr_MUST_PASS_FLG).HasColumnName("Chr_MUST_PASS_FLG");
        Property(t => t.Chr_INCL_FLG).HasColumnName("Chr_INCL_FLG");
        Property(t => t.Chr_PASS_FLG).HasColumnName("Chr_PASS_FLG");
        Property(t => t.Num_CO_SYB_PRINT_SEQ).HasColumnName("Num_CO_SYB_PRINT_SEQ");
        Property(t => t.Chr_DELETE_FLG).HasColumnName("Chr_DELETE_FLG");
        Property(t => t.var_USR_NM).HasColumnName("var_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
        Property(t => t.Num_Syl_Seq).HasColumnName("Num_Syl_Seq");
    }
}