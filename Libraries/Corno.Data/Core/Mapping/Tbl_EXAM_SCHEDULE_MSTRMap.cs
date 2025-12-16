using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.BHVEDPSNET.Mapping
{
    public class Tbl_EXAM_SCHEDULE_MSTRMap : EntityTypeConfiguration<Tbl_EXAM_SCHEDULE_MSTR>
    {
        public Tbl_EXAM_SCHEDULE_MSTRMap()
        {
            // Primary Key
            HasKey(t => new { t.Num_FK_COPRT_NO, t.Num_FK_BR_NO });

            // Properties
            Property(t => t.Num_FK_COPRT_NO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Num_FK_BR_NO)
                .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Var_USR_NM)
                .HasMaxLength(12);

            // Table & Column Mappings
            ToTable("Tbl_EXAM_SCHEDULE_MSTR");
            //this.Property(t => t.Num_FK_COPRT_NO).HasColumnName("Num_FK_COPRT_NO");
            //this.Property(t => t.Num_FK_BR_NO).HasColumnName("Num_FK_BR_NO");
            //this.Property(t => t.Dtm_Commencement_DateOfExam).HasColumnName("Dtm_Commencement_DateOfExam");
            //this.Property(t => t.Dtm_ConclExam).HasColumnName("Dtm_ConclExam");
            //this.Property(t => t.Dtm_ComplCAP).HasColumnName("Dtm_ComplCAP");
            //this.Property(t => t.Dtm_MarkFromCAP).HasColumnName("Dtm_MarkFromCAP");
            //this.Property(t => t.Dtm_Result_Date).HasColumnName("Dtm_Result_Date");
            //this.Property(t => t.Num_FK_INST_NO).HasColumnName("Num_FK_INST_NO");
            //this.Property(t => t.Var_USR_NM).HasColumnName("Var_USR_NM");
            //this.Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
            //this.Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
        }
    }
}
