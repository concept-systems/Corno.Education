using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_APP_TEMPMap : EntityTypeConfiguration<Tbl_APP_TEMP>
{
    public Tbl_APP_TEMPMap()
    {
        // Primary Key
        HasKey(t => new { t.Num_PK_ENTRY_ID })
            .Property(t => t.Num_PK_ENTRY_ID)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
            .IsRequired();

        // Properties
        //this.Property(t => t.Num_PK_ENTRY_ID)
        //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        //this.Property(t => t.Num_FORM_ID)
        //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        //this.Property(t => t.Chr_APP_VALID_FLG)
        //    .IsFixedLength()
        //    .HasMaxLength(1);

        //this.Property(t => t.Chr_APP_PRN_NO)
        //    .IsRequired()
        //    .IsFixedLength()
        //    .HasMaxLength(10);

        //this.Property(t => t.Num_FK_COPRT_NO)
        //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        //this.Property(t => t.Num_FK_COLLEGE_CD)
        //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        //this.Property(t => t.Num_FK_CENTER_CD)
        //    .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        //this.Property(t => t.Chr_BUNDAL_NO)
        //    .IsFixedLength()
        //    .HasMaxLength(15);

        //this.Property(t => t.DELETE_FLG)
        //    .IsFixedLength()
        //    .HasMaxLength(1);

        //this.Property(t => t.Var_USR_NM)
        //    .HasMaxLength(12);

        //this.Property(t => t.Chr_REPEATER_FLG)
        //    .IsFixedLength()
        //    .HasMaxLength(1);

        //this.Property(t => t.Chr_IMPROVEMENT_FLG)
        //    .HasMaxLength(1);

        //this.Property(t => t.Chr_College_Chnage)
        //    .IsFixedLength()
        //    .HasMaxLength(1);

        //this.Property(t => t.Chr_Branch_Chnage)
        //    .IsFixedLength()
        //    .HasMaxLength(1);

        // Table & Column Mappings
        ToTable("Tbl_APP_TEMP");
        Property(t => t.Num_PK_ENTRY_ID).HasColumnName("Num_PK_ENTRY_ID");
        Property(t => t.Num_FORM_ID).HasColumnName("Num_FORM_ID");
        Property(t => t.Chr_APP_VALID_FLG).HasColumnName("Chr_APP_VALID_FLG");
        Property(t => t.Chr_APP_PRN_NO).HasColumnName("Chr_APP_PRN_NO");
        Property(t => t.Num_FK_COPRT_NO).HasColumnName("Num_FK_COPRT_NO");
        Property(t => t.Num_FK_INST_NO).HasColumnName("Num_FK_INST_NO");
        Property(t => t.Num_FK_BR_CD).HasColumnName("Num_FK_BR_CD");
        Property(t => t.Num_FK_COLLEGE_CD).HasColumnName("Num_FK_COLLEGE_CD");
        Property(t => t.Num_FK_CENTER_CD).HasColumnName("Num_FK_CENTER_CD");
        Property(t => t.Chr_BUNDAL_NO).HasColumnName("Chr_BUNDAL_NO");
        Property(t => t.Num_FK_STUDCAT_CD).HasColumnName("Num_FK_STUDCAT_CD");
        Property(t => t.Num_FK_STACTV_CD).HasColumnName("Num_FK_STACTV_CD");
        Property(t => t.DELETE_FLG).HasColumnName("DELETE_FLG");
        Property(t => t.Var_USR_NM).HasColumnName("Var_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
        Property(t => t.Chr_REPEATER_FLG).HasColumnName("Chr_REPEATER_FLG");
        Property(t => t.Chr_IMPROVEMENT_FLG).HasColumnName("Chr_IMPROVEMENT_FLG");
        Property(t => t.Chr_College_Chnage).HasColumnName("Chr_College_Chnage");
        Property(t => t.Chr_Branch_Chnage).HasColumnName("Chr_Branch_Chnage");
        Property(t => t.Num_FK_DistCenter_ID).HasColumnName("Num_FK_DistCenter_ID");
    }
}