using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_STUDENT_INFOMap : EntityTypeConfiguration<Tbl_STUDENT_INFO>
{
    public Tbl_STUDENT_INFOMap()
    {
        // Primary Key
        HasKey(t => new { t.Chr_PK_PRN_NO, t.Var_ST_NM, t.Chr_ST_SEX_CD, t.Num_ST_CAST_CD, t.NUM_FK_COPART });

        // Properties
        Property(t => t.Chr_PK_PRN_NO)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(10);

        Property(t => t.Var_ST_NM)
            .IsRequired()
            .HasMaxLength(100);

        Property(t => t.Var_ST_FATHR_NM)
            .HasMaxLength(100);

        Property(t => t.VAR_MOTHR_NM)
            .HasMaxLength(30);

        Property(t => t.Chr_ST_SEX_CD)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Dtm_ST_DOB_DT)
            .IsFixedLength()
            .HasMaxLength(2);

        Property(t => t.Dtm_ST_DOB_MONTH)
            .IsFixedLength()
            .HasMaxLength(2);

        Property(t => t.Dtm_ST_DOB_YEAR)
            .IsFixedLength()
            .HasMaxLength(4);

        Property(t => t.Num_ST_CAST_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Chr_USR_NM)
            .IsFixedLength()
            .HasMaxLength(12);

        Property(t => t.NUM_FK_COPART)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Chr_STUDENT_NATIONALITY)
            .HasMaxLength(1);

        Property(t => t.Var_OLD_PRN_NO)
            .HasMaxLength(25);

        Property(t => t.fffff)
            .IsFixedLength()
            .HasMaxLength(1);

        // Table & Column Mappings
        ToTable("Tbl_STUDENT_INFO");
        Property(t => t.Chr_PK_PRN_NO).HasColumnName("Chr_PK_PRN_NO");
        Property(t => t.Var_ST_NM).HasColumnName("Var_ST_NM");
        Property(t => t.Var_ST_FATHR_NM).HasColumnName("Var_ST_FATHR_NM");
        Property(t => t.VAR_MOTHR_NM).HasColumnName("VAR_MOTHR_NM");
        Property(t => t.Chr_ST_SEX_CD).HasColumnName("Chr_ST_SEX_CD");
        Property(t => t.Dtm_ST_DOB_DT).HasColumnName("Dtm_ST_DOB_DT");
        Property(t => t.Dtm_ST_DOB_MONTH).HasColumnName("Dtm_ST_DOB_MONTH");
        Property(t => t.Dtm_ST_DOB_YEAR).HasColumnName("Dtm_ST_DOB_YEAR");
        Property(t => t.Num_ST_CAST_CD).HasColumnName("Num_ST_CAST_CD");
        Property(t => t.Num_FK_INCOME_CD).HasColumnName("Num_FK_INCOME_CD");
        Property(t => t.Chr_USR_NM).HasColumnName("Chr_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
        Property(t => t.NUM_FK_COPART).HasColumnName("NUM_FK_COPART");
        Property(t => t.Chr_STUDENT_NATIONALITY).HasColumnName("Chr_STUDENT_NATIONALITY");
        Property(t => t.Var_OLD_PRN_NO).HasColumnName("Var_OLD_PRN_NO");
        Property(t => t.fffff).HasColumnName("fffff");
    }
}