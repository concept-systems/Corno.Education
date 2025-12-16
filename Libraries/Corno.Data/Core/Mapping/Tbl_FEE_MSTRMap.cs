using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_FEE_MSTRMap : EntityTypeConfiguration<Tbl_FEE_MSTR>
{
    public Tbl_FEE_MSTRMap()
    {
        // Primary Key
        HasKey(t => new { t.Num_PK_FEE_CD, t.Var_FEE_DESC });

        // Properties
        //this.Property(t => t.Num_PK_FEE_CD)
        //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        //this.Property(t => t.Var_FEE_DESC)
        //    .IsRequired()
        //    .HasMaxLength(60);

        //this.Property(t => t.Num_FEE_SEQ_NO)
        //    .HasMaxLength(40);

        //this.Property(t => t.Chr_DELETE_FLG)
        //    .IsFixedLength()
        //    .HasMaxLength(1);

        //this.Property(t => t.Var_USR_NM)
        //    .HasMaxLength(12);

        // Table & Column Mappings
        ToTable("Tbl_FEE_MSTR");
        //this.Property(t => t.Num_PK_FEE_CD).HasColumnName("Num_PK_FEE_CD");
        //this.Property(t => t.Var_FEE_DESC).HasColumnName("Var_FEE_DESC");
        //this.Property(t => t.Num_FEE_SEQ_NO).HasColumnName("Num_FEE_SEQ_NO");
        //this.Property(t => t.Chr_DELETE_FLG).HasColumnName("Chr_DELETE_FLG");
        //this.Property(t => t.Var_USR_NM).HasColumnName("Var_USR_NM");
        //this.Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        //this.Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
    }
}