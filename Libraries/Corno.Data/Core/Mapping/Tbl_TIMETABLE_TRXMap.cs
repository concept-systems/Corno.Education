using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.BHVEDPSNET.Mapping
{
    public class Tbl_TIMETABLE_TRXMap : EntityTypeConfiguration<Tbl_TIMETABLE_TRX>
    {
        public Tbl_TIMETABLE_TRXMap()
        {
            //Primary Key
            HasKey(t => new { t.Num_FK_INST_NO });

            // Properties

            Property(t => t.Num_FK_COPRT_NO)
               .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            Property(t => t.Num_FK_INST_NO)
               .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

            // Table & Column Mappings
            ToTable("Tbl_TIMETABLE_TRX");
            Property(t => t.Num_FK_COPRT_NO).HasColumnName("Num_FK_COPRT_NO");
            Property(t => t.Num_FK_PH_CD).HasColumnName("Num_FK_PH_CD");
            Property(t => t.Num_FK_CAT_CD).HasColumnName("Num_FK_CAT_CD");
            Property(t => t.Num_FK_PP_CD).HasColumnName("Num_FK_PP_CD");
            Property(t => t.NUM_FK_SUB_DIV_CD).HasColumnName("NUM_FK_SUB_DIV_CD");
            Property(t => t.Dtm_TBM_FROM_TIME).HasColumnName("Dtm_TBM_FROM_TIME");
            Property(t => t.Dtm_TBM_TO_TIME).HasColumnName("Dtm_TBM_TO_TIME");
            Property(t => t.Num_FK_INST_NO).HasColumnName("Num_FK_INST_NO");
            Property(t => t.Var_USR_NM).HasColumnName("Var_USR_NM");
            Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
            Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
            Property(t => t.VAR_START_TIME).HasColumnName("VAR_START_TIME");
            Property(t => t.VAR_TO_TIME).HasColumnName("VAR_TO_TIME");
        }
    }
}
