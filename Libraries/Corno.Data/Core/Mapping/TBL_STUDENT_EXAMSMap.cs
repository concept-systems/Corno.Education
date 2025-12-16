using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class TBL_STUDENT_EXAMSMap : EntityTypeConfiguration<TBL_STUDENT_EXAMS>
{
    public TBL_STUDENT_EXAMSMap()
    {
        // Primary Key
        HasKey(t => new { t.Chr_FK_PRN_NO, t.Num_FK_COPRT_NO, t.Num_FK_INST_NO });

        // Properties
        Property(t => t.Chr_FK_PRN_NO)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(10);

        Property(t => t.Num_FK_COPRT_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_FK_INST_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Chr_ST_COPRT_RES)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_PART_TOT_PASSFAIL_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_ST_YR_RES)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_USR_NM)
            .HasMaxLength(12);

        Property(t => t.Chr_ST_REV)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_IMPROVEMENT_FLG)
            .HasMaxLength(1);

        Property(t => t.Chr_ST_BOP_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        // Table & Column Mappings
        ToTable("TBL_STUDENT_EXAMS");
        Property(t => t.Chr_FK_PRN_NO).HasColumnName("Chr_FK_PRN_NO");
        Property(t => t.Num_FK_BR_CD).HasColumnName("Num_FK_BR_CD");
        Property(t => t.Num_FK_COPRT_NO).HasColumnName("Num_FK_COPRT_NO");
        Property(t => t.Num_ST_SEAT_NO).HasColumnName("Num_ST_SEAT_NO");
        Property(t => t.Num_FK_INST_NO).HasColumnName("Num_FK_INST_NO");
        Property(t => t.Num_FK_CO_CD).HasColumnName("Num_FK_CO_CD");
        Property(t => t.Num_COPRT_SEMI_NO).HasColumnName("Num_COPRT_SEMI_NO");
        Property(t => t.Num_COPRT_PART_NO).HasColumnName("Num_COPRT_PART_NO");
        Property(t => t.Chr_ST_COPRT_RES).HasColumnName("Chr_ST_COPRT_RES");
        Property(t => t.Num_FK_CLASS_CD).HasColumnName("Num_FK_CLASS_CD");
        Property(t => t.Chr_PART_TOT_PASSFAIL_FLG).HasColumnName("Chr_PART_TOT_PASSFAIL_FLG");
        Property(t => t.Num_ST_ORD_MRK).HasColumnName("Num_ST_ORD_MRK");
        Property(t => t.Num_FK_ORD_NO).HasColumnName("Num_FK_ORD_NO");
        Property(t => t.Chr_ST_YR_RES).HasColumnName("Chr_ST_YR_RES");
        Property(t => t.Chr_USR_NM).HasColumnName("Chr_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
        Property(t => t.Chr_ST_REV).HasColumnName("Chr_ST_REV");
        Property(t => t.Chr_IMPROVEMENT_FLG).HasColumnName("Chr_IMPROVEMENT_FLG");
        Property(t => t.Chr_ST_BOP_FLG).HasColumnName("Chr_ST_BOP_FLG");
    }
}