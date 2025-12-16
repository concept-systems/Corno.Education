using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class TBL_STUDENT_REVAL_CHILDMap : EntityTypeConfiguration<TBL_STUDENT_REVAL_CHILD>
{
    public TBL_STUDENT_REVAL_CHILDMap()
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

        // Table & Column Mappings
        ToTable("TBL_STUDENT_REVAL_CHILD");
        Property(t => t.NUM_FK_PRN_NO).HasColumnName("NUM_FK_PRN_NO");
        Property(t => t.Num_FK_COPRT_NO).HasColumnName("Num_FK_COPRT_NO");
        Property(t => t.Num_FK_INST_NO).HasColumnName("Num_FK_INST_NO");
        Property(t => t.NUM_FK_SUB_CD).HasColumnName("NUM_FK_SUB_CD");
        Property(t => t.Num_FK_CAT_CD).HasColumnName("Num_FK_CAT_CD");
        Property(t => t.Num_FK_PAP_CD).HasColumnName("Num_FK_PAP_CD");
        Property(t => t.Num_FK_SEC_CD).HasColumnName("Num_FK_SEC_CD");
        Property(t => t.OLD_MARK).HasColumnName("OLD_MARK");
        Property(t => t.NEW_MARK).HasColumnName("NEW_MARK");
        Property(t => t.NUM_REVALUATION_FEE).HasColumnName("NUM_REVALUATION_FEE");
        Property(t => t.NUM_VERIFICATION_FEE).HasColumnName("NUM_VERIFICATION_FEE");
        Property(t => t.NUM_TOTAL_FEE).HasColumnName("NUM_TOTAL_FEE");

    }
}