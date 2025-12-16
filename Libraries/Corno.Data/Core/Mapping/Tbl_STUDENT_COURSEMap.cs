using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_STUDENT_COURSEMap : EntityTypeConfiguration<Tbl_STUDENT_COURSE>
{
    public Tbl_STUDENT_COURSEMap()
    {
        // Primary Key
        HasKey(t => new { t.Chr_FK_PRN_NO, t.Num_FK_CO_CD, t.Num_FK_COPRT_NO, t.CHR_MIGRATED, t.CHR_DEGREE_ISSUED, t.CHR_PRACTICE_APPLICABLE, t.CHR_PASS_CERT_ISSUED });

        // Properties
        Property(t => t.Chr_FK_PRN_NO)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(10);

        Property(t => t.Num_FK_CO_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_FK_COPRT_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Chr_ST_COL_ADM_ROLL)
            .IsFixedLength()
            .HasMaxLength(5);

        Property(t => t.Chr_ST_REG_YEAR)
            .IsFixedLength()
            .HasMaxLength(4);

        Property(t => t.Chr_ST_REG_CARD_PRINT_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Num_REG_FORM_NO)
            .HasMaxLength(10);

        Property(t => t.Num_ST_REG_FEES)
            .IsFixedLength()
            .HasMaxLength(7);

        Property(t => t.Chr_STCO_MIG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.CHR_MIGRATED)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.CHR_CO_RESULT)
            .IsFixedLength()
            .HasMaxLength(10);

        Property(t => t.CHR_DEGREE_ISSUED)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.CHR_PRACTICE_APPLICABLE)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.CHR_PASS_CERT_ISSUED)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Var_USR_NM)
            .HasMaxLength(12);

        Property(t => t.Chr_Num_Enclose)
            .HasMaxLength(30);

        Property(t => t.Num_Enroll_No)
            .HasMaxLength(10);

        // Table & Column Mappings
        ToTable("Tbl_STUDENT_COURSE");
        Property(t => t.Chr_FK_PRN_NO).HasColumnName("Chr_FK_PRN_NO");
        Property(t => t.Num_FK_CO_CD).HasColumnName("Num_FK_CO_CD");
        Property(t => t.Num_FK_COPRT_NO).HasColumnName("Num_FK_COPRT_NO");
        Property(t => t.Num_ST_COLLEGE_CD).HasColumnName("Num_ST_COLLEGE_CD");
        Property(t => t.Chr_ST_COL_ADM_ROLL).HasColumnName("Chr_ST_COL_ADM_ROLL");
        Property(t => t.Chr_ST_REG_YEAR).HasColumnName("Chr_ST_REG_YEAR");
        Property(t => t.Num_FK_INST_NO).HasColumnName("Num_FK_INST_NO");
        Property(t => t.Chr_ST_REG_CARD_PRINT_FLG).HasColumnName("Chr_ST_REG_CARD_PRINT_FLG");
        Property(t => t.Num_REG_FORM_NO).HasColumnName("Num_REG_FORM_NO");
        Property(t => t.Num_ST_REG_FEES).HasColumnName("Num_ST_REG_FEES");
        Property(t => t.Chr_STCO_MIG).HasColumnName("Chr_STCO_MIG");
        Property(t => t.CHR_MIGRATED).HasColumnName("CHR_MIGRATED");
        Property(t => t.CHR_CO_RESULT).HasColumnName("CHR_CO_RESULT");
        Property(t => t.NUM_CO_CLASS).HasColumnName("NUM_CO_CLASS");
        Property(t => t.NUM_CO_GRADE).HasColumnName("NUM_CO_GRADE");
        Property(t => t.CHR_DEGREE_ISSUED).HasColumnName("CHR_DEGREE_ISSUED");
        Property(t => t.CHR_PRACTICE_APPLICABLE).HasColumnName("CHR_PRACTICE_APPLICABLE");
        Property(t => t.CHR_PASS_CERT_ISSUED).HasColumnName("CHR_PASS_CERT_ISSUED");
        Property(t => t.Img_Pdf).HasColumnName("Img_Pdf");
        Property(t => t.Var_USR_NM).HasColumnName("Var_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
        Property(t => t.Num_FK_BR_CD).HasColumnName("Num_FK_BR_CD");
        Property(t => t.Chr_Num_Enclose).HasColumnName("Chr_Num_Enclose");
        Property(t => t.Num_FK_DistCenter_ID).HasColumnName("Num_FK_DistCenter_ID");
        Property(t => t.Num_Enroll_No).HasColumnName("Num_Enroll_No");
    }
}