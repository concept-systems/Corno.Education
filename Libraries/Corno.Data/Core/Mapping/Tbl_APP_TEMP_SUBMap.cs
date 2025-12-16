using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_APP_TEMP_SUBMap : EntityTypeConfiguration<Tbl_APP_TEMP_SUB>
{
    public Tbl_APP_TEMP_SUBMap()
    {
        // Primary Key
        this.HasKey(t => new { t.Num_FK_ENTRY_ID, t.Num_FK_INST_NO, t.Num_FK_COPRT_NO, t.Num_FK_SUB_CD, t.Chr_DELETE_FLG });

        //// Properties
        //this.Property(t => t.Num_FK_ENTRY_ID)
        //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        //this.Property(t => t.Num_FK_SUB_CD)
        //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        //this.Property(t => t.Chr_REPH_FLG)
        //    .HasMaxLength(1);

        //this.Property(t => t.Chr_SUB_FLG)
        //    .IsFixedLength()
        //    .HasMaxLength(1);

        //this.Property(t => t.Chr_DELETE_FLG)
        //    .IsRequired()
        //    .IsFixedLength()
        //    .HasMaxLength(1);

        //this.Property(t => t.Chr_USR_NM)
        //    .IsFixedLength()
        //    .HasMaxLength(12);

        //this.Property(t => t.Var_Appear_CATEGORY)
        //    .HasMaxLength(50);

        // Table & Column Mappings
        ToTable("Tbl_APP_TEMP_SUB");
        Property(t => t.Num_FK_ENTRY_ID).HasColumnName("Num_FK_ENTRY_ID");
        Property(t => t.Num_FK_SUB_CD).HasColumnName("Num_FK_SUB_CD");
        Property(t => t.Num_FK_INST_NO).HasColumnName("Num_FK_INST_NO");
        Property(t => t.Num_FK_COPRT_NO).HasColumnName("Num_FK_COPRT_NO");
        Property(t => t.Chr_REPH_FLG).HasColumnName("Chr_REPH_FLG");
        Property(t => t.Chr_SUB_FLG).HasColumnName("Chr_SUB_FLG");
        Property(t => t.Chr_DELETE_FLG).HasColumnName("Chr_DELETE_FLG");
        Property(t => t.Chr_USR_NM).HasColumnName("Chr_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
        Property(t => t.Var_Appear_CATEGORY).HasColumnName("Var_Appear_CATEGORY");
    }
}