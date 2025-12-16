using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_FACULTY_MSTRMap : EntityTypeConfiguration<Tbl_FACULTY_MSTR>
{
    public Tbl_FACULTY_MSTRMap()
    {
        // Primary Key
        HasKey(t => t.Num_PK_FA_CD);

        // Properties
        Property(t => t.Var_FA_NM)
            .IsRequired()
            .HasMaxLength(60);

        Property(t => t.Var_FA_NM_BL)
            .HasMaxLength(30);

        Property(t => t.Chr_DELETE_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Var_USR_NM)
            .HasMaxLength(12);

        // Table & Column Mappings
        ToTable("Tbl_FACULTY_MSTR");
        Property(t => t.Num_PK_FA_CD).HasColumnName("Num_PK_FA_CD");
        Property(t => t.Var_FA_NM).HasColumnName("Var_FA_NM");
        Property(t => t.Num_FA_SEQ_NO).HasColumnName("Num_FA_SEQ_NO");
        Property(t => t.Var_FA_NM_BL).HasColumnName("Var_FA_NM_BL");
        Property(t => t.Chr_DELETE_FLG).HasColumnName("Chr_DELETE_FLG");
        Property(t => t.Var_USR_NM).HasColumnName("Var_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
    }
}