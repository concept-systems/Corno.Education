using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_STUDENT_SUBJECTMap : EntityTypeConfiguration<Tbl_STUDENT_SUBJECT>
{
    public Tbl_STUDENT_SUBJECTMap()
    {
        // Primary Key
        HasKey(t => new { t.Num_FK_COPRT_NO, t.Chr_FK_PRN_NO, t.Num_FK_SUB_CD, t.Num_ST_GRD_NO, t.Num_FK_INST_NO, t.Num_ST_SUB_ORG_MRK });

        // Properties
        Property(t => t.Num_FK_COPRT_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Chr_FK_PRN_NO)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(10);

        Property(t => t.Num_FK_SUB_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Chr_ST_SUB_STS)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Num_ST_GRD_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Chr_ST_SUB_RES)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Num_FK_INST_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Chr_ST_SUB_CAN)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Num_ST_SUB_ORG_MRK)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Chr_ST_SUB_ORDINANCE)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.CHR_ST_ORD_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Var_USR_NM)
            .HasMaxLength(12);

        Property(t => t.Chr_IMPROVEMENT_FLG)
            .HasMaxLength(1);

        // Table & Column Mappings
        ToTable("Tbl_STUDENT_SUBJECT");
        Property(t => t.Num_FK_COPRT_NO).HasColumnName("Num_FK_COPRT_NO");
        Property(t => t.Chr_FK_PRN_NO).HasColumnName("Chr_FK_PRN_NO");
        Property(t => t.Num_FK_SUB_CD).HasColumnName("Num_FK_SUB_CD");
        Property(t => t.Num_ST_SUB_MRK).HasColumnName("Num_ST_SUB_MRK");
        Property(t => t.Chr_ST_SUB_STS).HasColumnName("Chr_ST_SUB_STS");
        Property(t => t.Num_ST_GRD_NO).HasColumnName("Num_ST_GRD_NO");
        Property(t => t.Chr_ST_SUB_RES).HasColumnName("Chr_ST_SUB_RES");
        Property(t => t.Num_FK_INST_NO).HasColumnName("Num_FK_INST_NO");
        Property(t => t.Chr_ST_SUB_CAN).HasColumnName("Chr_ST_SUB_CAN");
        Property(t => t.Num_ST_SUB_ORG_MRK).HasColumnName("Num_ST_SUB_ORG_MRK");
        Property(t => t.Chr_ST_SUB_ORDINANCE).HasColumnName("Chr_ST_SUB_ORDINANCE");
        Property(t => t.CHR_ST_ORD_FLG).HasColumnName("CHR_ST_ORD_FLG");
        Property(t => t.Var_USR_NM).HasColumnName("Var_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
        Property(t => t.Chr_IMPROVEMENT_FLG).HasColumnName("Chr_IMPROVEMENT_FLG");
        Property(t => t.Chr_ST_SUB_GPA).HasColumnName("Chr_ST_SUB_GPA");
        Property(t => t.Num_FK_COPRT_NO_AddCr).HasColumnName("Num_FK_COPRT_NO_AddCr");
    }
}