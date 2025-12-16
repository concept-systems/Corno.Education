using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class TBL_STUDENT_REVALUATIONMap : EntityTypeConfiguration<TBL_STUDENT_REVALUATION>
{
    public TBL_STUDENT_REVALUATIONMap()
    {
        // Primary Key
        HasKey(t => new { t.NUM_FK_PRN_NO, t.Num_FK_INST_NO, t.Num_FK_COPRT_NO, t.NUM_FK_SUB_CD });

        // Properties

        Property(t => t.NUM_FK_PRN_NO)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(10);

        Property(t => t.Num_FK_COPRT_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_FK_INST_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.NUM_FK_SUB_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Chr_REVAL_VERI_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        // Table & Column Mappings
        ToTable("TBL_STUDENT_REVALUATION");
        Property(t => t.NUM_FK_PRN_NO).HasColumnName("NUM_FK_PRN_NO");
        Property(t => t.Num_FK_COPRT_NO).HasColumnName("Num_FK_COPRT_NO");
        Property(t => t.Num_FK_INST_NO).HasColumnName("Num_FK_INST_NO");
        Property(t => t.NUM_FK_SUB_CD).HasColumnName("NUM_FK_SUB_CD");
        Property(t => t.Var_ST_SUB_NOCHNG).HasColumnName("Var_ST_SUB_NOCHNG");
        Property(t => t.Chr_REVAL_VERI_FLG).HasColumnName("Chr_REVAL_VERI_FLG");
        Property(t => t.OLD_MARK).HasColumnName("OLD_MARK");
        Property(t => t.NEW_MARK).HasColumnName("NEW_MARK");
        Property(t => t.Chr_Reval_UniExmHd).HasColumnName("Chr_Reval_UniExmHd");

    }
}