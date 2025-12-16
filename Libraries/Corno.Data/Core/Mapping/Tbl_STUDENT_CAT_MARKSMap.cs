using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_STUDENT_CAT_MARKSMap : EntityTypeConfiguration<Tbl_STUDENT_CAT_MARKS>
{
    public Tbl_STUDENT_CAT_MARKSMap()
    {
        // Primary Key
        HasKey(t => new { t.Var_FK_PRN_NO, t.Num_FK_SUB_CD, t.Num_FK_CAT_CD, t.Num_FK_INST_NO });

        // Properties
        Property(t => t.Var_FK_PRN_NO)
            .IsRequired()
            .HasMaxLength(10);

        Property(t => t.Num_FK_SUB_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_FK_CAT_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Var_ST_PH_STS)
            .HasMaxLength(1);

        Property(t => t.Var_ST_PH_RES)
            .HasMaxLength(1);

        Property(t => t.Num_FK_INST_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Var_ST_PH_USR_NM)
            .HasMaxLength(12);

        // Table & Column Mappings
        ToTable("Tbl_STUDENT_CAT_MARKS");
        Property(t => t.Var_FK_PRN_NO).HasColumnName("Var_FK_PRN_NO");
        Property(t => t.Num_FK_SUB_CD).HasColumnName("Num_FK_SUB_CD");
        Property(t => t.Num_FK_CAT_CD).HasColumnName("Num_FK_CAT_CD");
        Property(t => t.Num_ST_PH_MRK).HasColumnName("Num_ST_PH_MRK");
        Property(t => t.Var_ST_PH_STS).HasColumnName("Var_ST_PH_STS");
        Property(t => t.Num_ST_GRD_NO).HasColumnName("Num_ST_GRD_NO");
        Property(t => t.Var_ST_PH_RES).HasColumnName("Var_ST_PH_RES");
        Property(t => t.Num_FK_INST_NO).HasColumnName("Num_FK_INST_NO");
        Property(t => t.Var_ST_PH_USR_NM).HasColumnName("Var_ST_PH_USR_NM");
        Property(t => t.Dtm_ST_PH_DTE_CR).HasColumnName("Dtm_ST_PH_DTE_CR");
        Property(t => t.Dtm_ST_PH_DTE_UP).HasColumnName("Dtm_ST_PH_DTE_UP");
        Property(t => t.Num_ST_PH_ACT_MRK).HasColumnName("Num_ST_PH_ACT_MRK");
    }
}