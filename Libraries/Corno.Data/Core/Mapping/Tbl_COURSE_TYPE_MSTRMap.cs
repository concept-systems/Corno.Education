using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_COURSE_TYPE_MSTRMap : EntityTypeConfiguration<Tbl_COURSE_TYPE_MSTR>
{
    public Tbl_COURSE_TYPE_MSTRMap()
    {
        // Primary Key
        //this.HasKey(t => new { t.Num_PK_TYP_CD, t.Var_TYP_NM, t.Var_TYP_SHRT_NM });
        HasKey(t => new { t.Num_PK_TYP_CD});

        // Properties
        Property(t => t.Num_PK_TYP_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Var_TYP_NM)
            .IsRequired()
            .HasMaxLength(40);

        Property(t => t.Var_TYP_SHRT_NM)
            .IsRequired()
            .HasMaxLength(10);

        Property(t => t.Chr_DELETE_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Var_USR_NM)
            .HasMaxLength(12);

        // Table & Column Mappings
        ToTable("Tbl_COURSE_TYPE_MSTR");
        Property(t => t.Num_PK_TYP_CD).HasColumnName("Num_PK_TYP_CD");
        Property(t => t.Var_TYP_NM).HasColumnName("Var_TYP_NM");
        Property(t => t.Var_TYP_SHRT_NM).HasColumnName("Var_TYP_SHRT_NM");
        Property(t => t.Chr_DELETE_FLG).HasColumnName("Chr_DELETE_FLG");
        Property(t => t.Var_USR_NM).HasColumnName("Var_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
    }
}