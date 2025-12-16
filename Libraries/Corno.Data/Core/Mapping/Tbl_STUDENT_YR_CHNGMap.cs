using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_STUDENT_YR_CHNGMap : EntityTypeConfiguration<Tbl_STUDENT_YR_CHNG>
{
    public Tbl_STUDENT_YR_CHNGMap()
    {
        // Primary Key
        HasKey(t => new { t.Chr_FK_PRN_NO, t.Num_FK_INST_NO, t.Num_FK_COPRT_NO, t.Num_FK_BR_CD, t.Num_ST_SEAT_NO, t.Num_FK_COL_CD, t.Num_FK_CENTER_COL_CD, t.Num_FK_DEBR_CD, t.Num_ST_MARKSHEET_NO, t.Chr_ST_APP_FORM_NO });

        // Properties
        Property(t => t.Chr_FK_PRN_NO)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(10);

        Property(t => t.Num_FK_INST_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_FK_COPRT_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_FK_BR_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_ST_SEAT_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_FK_COL_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Chr_ST_RESULT)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Num_FK_CENTER_COL_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_FK_DEBR_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Chr_ST_ADMIT_PRN)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_ST_ATTEN_PRN)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_ST_MARKSHEET_PRN)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Num_ST_MARKSHEET_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Chr_ST_APP_FORM_NO)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(10);

        Property(t => t.Chr_ST_RESERV_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_Ord_Appl_Flg)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_USR_NM)
            .IsFixedLength()
            .HasMaxLength(12);

        Property(t => t.Chr_STYC_REVAL_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_IMPROVEMENT_FLG)
            .HasMaxLength(1);

        // Table & Column Mappings
        ToTable("Tbl_STUDENT_YR_CHNG");
        Property(t => t.Chr_FK_PRN_NO).HasColumnName("Chr_FK_PRN_NO");
        Property(t => t.Num_FK_INST_NO).HasColumnName("Num_FK_INST_NO");
        Property(t => t.Num_FK_COPRT_NO).HasColumnName("Num_FK_COPRT_NO");
        Property(t => t.Num_FK_BR_CD).HasColumnName("Num_FK_BR_CD");
        Property(t => t.Num_ST_SEAT_NO).HasColumnName("Num_ST_SEAT_NO");
        Property(t => t.Num_FK_COL_CD).HasColumnName("Num_FK_COL_CD");
        Property(t => t.Chr_ST_RESULT).HasColumnName("Chr_ST_RESULT");
        Property(t => t.Num_FK_RESULT_CD).HasColumnName("Num_FK_RESULT_CD");
        Property(t => t.Num_ST_ORD_MRK).HasColumnName("Num_ST_ORD_MRK");
        Property(t => t.Num_FK_ORD_NO).HasColumnName("Num_FK_ORD_NO");
        Property(t => t.Num_FK_CENTER_COL_CD).HasColumnName("Num_FK_CENTER_COL_CD");
        Property(t => t.Num_FK_DEBR_CD).HasColumnName("Num_FK_DEBR_CD");
        Property(t => t.Chr_ST_ADMIT_PRN).HasColumnName("Chr_ST_ADMIT_PRN");
        Property(t => t.Chr_ST_ATTEN_PRN).HasColumnName("Chr_ST_ATTEN_PRN");
        Property(t => t.Chr_ST_MARKSHEET_PRN).HasColumnName("Chr_ST_MARKSHEET_PRN");
        Property(t => t.Num_ST_MARKSHEET_NO).HasColumnName("Num_ST_MARKSHEET_NO");
        Property(t => t.Chr_ST_APP_FORM_NO).HasColumnName("Chr_ST_APP_FORM_NO");
        Property(t => t.Chr_ST_RESERV_FLG).HasColumnName("Chr_ST_RESERV_FLG");
        Property(t => t.Chr_Ord_Appl_Flg).HasColumnName("Chr_Ord_Appl_Flg");
        Property(t => t.Img_PDF).HasColumnName("Img_PDF");
        Property(t => t.Chr_USR_NM).HasColumnName("Chr_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
        Property(t => t.Chr_STYC_REVAL_FLG).HasColumnName("Chr_STYC_REVAL_FLG");
        Property(t => t.Num_EXAM_GROUP).HasColumnName("Num_EXAM_GROUP");
        Property(t => t.Chr_IMPROVEMENT_FLG).HasColumnName("Chr_IMPROVEMENT_FLG");
        Property(t => t.Num_FK_DistCenter_ID).HasColumnName("Num_FK_DistCenter_ID");
    }
}