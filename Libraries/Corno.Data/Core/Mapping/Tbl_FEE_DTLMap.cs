using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_FEE_DTLMap : EntityTypeConfiguration<Tbl_FEE_DTL>
{
    public Tbl_FEE_DTLMap()
    {
        // Primary Key
        HasKey(t => new { t.Num_FK_CO_CD, t.Num_FK_COPRT_NO, t.Num_FK_BR_CD, t.Num_FK_INST_NO, t.Num_FK_FEE_CD });

        // Properties
        Property(t => t.Num_FK_COPRT_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        //this.Property(t => t.Var_USR_NM)
        //    .HasMaxLength(12);

        // Table & Column Mappings
        ToTable("Tbl_FEE_DTL");
        //this.Property(t => t.Num_FK_CO_CD).HasColumnName("Num_FK_CO_CD");
        //this.Property(t => t.Num_FK_COPRT_NO).HasColumnName("Num_FK_COPRT_NO");
        //this.Property(t => t.Num_FK_BR_CD).HasColumnName("Num_FK_BR_CD");
        //this.Property(t => t.Num_FK_INST_NO).HasColumnName("Num_FK_INST_NO");
        //this.Property(t => t.Num_FK_FEE_CD).HasColumnName("Num_FK_FEE_CD");
        //this.Property(t => t.FEE_AMOUNT).HasColumnName("FEE_AMOUNT");
        //this.Property(t => t.Var_USR_NM).HasColumnName("Var_USR_NM");
        //this.Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        //this.Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
    }
}