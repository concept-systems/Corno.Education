using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class TBL_STUDENT_CGPAMap : EntityTypeConfiguration<TBL_STUDENT_CGPA>
{
    public TBL_STUDENT_CGPAMap()
    {
        // Primary Key
        HasKey(t => t.Chr_FK_PRN_NO);

        // Properties
        Property(t => t.Chr_FK_PRN_NO)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(10);

        Property(t => t.Chr_CGPA_AVG_RES)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_CGPA_RES)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_USR_NM)
            .HasMaxLength(12);

        // Table & Column Mappings
        ToTable("TBL_STUDENT_CGPA");
        Property(t => t.Chr_FK_PRN_NO).HasColumnName("Chr_FK_PRN_NO");
        Property(t => t.Num_FK_BR_CD).HasColumnName("Num_FK_BR_CD");
        Property(t => t.Num_FK_COPRT_NO).HasColumnName("Num_FK_COPRT_NO");
        Property(t => t.Num_FK_INST_NO).HasColumnName("Num_FK_INST_NO");
        Property(t => t.Num_FK_GRADE_NO).HasColumnName("Num_FK_GRADE_NO");
        Property(t => t.Num_GRADE_POINTS).HasColumnName("Num_GRADE_POINTS");
        Property(t => t.Num_TOTAL_CREDITS).HasColumnName("Num_TOTAL_CREDITS");
        Property(t => t.Num_CGPA).HasColumnName("Num_CGPA");
        Property(t => t.Num_FK_GRADE_NO_AVG).HasColumnName("Num_FK_GRADE_NO_AVG");
        Property(t => t.Num_GRADE_POINTS_AVG).HasColumnName("Num_GRADE_POINTS_AVG");
        Property(t => t.Num_TOTAL_CREDITS_AVG).HasColumnName("Num_TOTAL_CREDITS_AVG");
        Property(t => t.Num_CGPA_AVG).HasColumnName("Num_CGPA_AVG");
        Property(t => t.Chr_CGPA_AVG_RES).HasColumnName("Chr_CGPA_AVG_RES");
        Property(t => t.Chr_CGPA_RES).HasColumnName("Chr_CGPA_RES");
        Property(t => t.Chr_USR_NM).HasColumnName("Chr_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
        Property(t => t.Num_CGPA_PER).HasColumnName("Num_CGPA_PER");
        Property(t => t.Num_CGPA_GRACE).HasColumnName("Num_CGPA_GRACE");
    }
}