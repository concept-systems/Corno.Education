using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_STUDENT_CONVOMap : EntityTypeConfiguration<Tbl_STUDENT_CONVO>
{
    public Tbl_STUDENT_CONVOMap()
    {
        // Primary Key
        HasKey(t => new { t.NUM_PK_RECORD_ID, t.Num_FK_RESULT_CD });

        // Properties
        Property(t => t.NUM_PK_RECORD_ID)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity);

        Property(t => t.Num_ST_BUN_NO)
            .HasMaxLength(10);

        Property(t => t.Chr_ST_YEAR)
            .IsFixedLength()
            .HasMaxLength(4);

        Property(t => t.Chr_ST_PA_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_FK_PRN_NO)
            .HasMaxLength(14);

        Property(t => t.Var_ST_NM)
            .HasMaxLength(60);

        Property(t => t.Chr_ST_SEX_CD)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_ST_ADD1)
            .HasMaxLength(200);

        Property(t => t.Chr_ST_ADD2)
            .IsFixedLength()
            .HasMaxLength(40);

        Property(t => t.Chr_ST_ADD3)
            .IsFixedLength()
            .HasMaxLength(40);

        Property(t => t.Chr_ST_ADD4)
            .IsFixedLength()
            .HasMaxLength(40);

        Property(t => t.Chr_ST_PINCODE)
            .IsFixedLength()
            .HasMaxLength(15);

        Property(t => t.Var_RES_PH)
            .HasMaxLength(14);

        Property(t => t.Var_E_MAIL)
            .HasMaxLength(50);

        Property(t => t.Chr_ST_PASS_YEAR)
            .IsFixedLength()
            .HasMaxLength(4);

        Property(t => t.Num_FK_RESULT_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Chr_ST_NMCHNG_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_ST_VALID_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_FEES_STATUS)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_PRINC_SUBJECT)
            .HasMaxLength(50);

        Property(t => t.Chr_PRINC_SUBJECT1)
            .HasMaxLength(50);

        Property(t => t.Var_ST_USR_NM)
            .HasMaxLength(15);

        Property(t => t.Chr_DELETE_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_Convo_Sts)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_Foreign_Student)
            .IsFixedLength()
            .HasMaxLength(10);

        Property(t => t.Chr_Improvement)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Destination)
            .HasMaxLength(50);

        // Table & Column Mappings
        ToTable("Tbl_STUDENT_CONVO");
        Property(t => t.NUM_PK_RECORD_ID).HasColumnName("NUM_PK_RECORD_ID");
        Property(t => t.Num_ST_BUN_NO).HasColumnName("Num_ST_BUN_NO");
        Property(t => t.Num_ST_SR_NO).HasColumnName("Num_ST_SR_NO");
        Property(t => t.Num_ST_FRM_NO).HasColumnName("Num_ST_FRM_NO");
        Property(t => t.Chr_ST_YEAR).HasColumnName("Chr_ST_YEAR");
        Property(t => t.Num_FK_CO_CD).HasColumnName("Num_FK_CO_CD");
        Property(t => t.Chr_ST_PA_FLG).HasColumnName("Chr_ST_PA_FLG");
        Property(t => t.Chr_FK_PRN_NO).HasColumnName("Chr_FK_PRN_NO");
        Property(t => t.Num_ST_SEAT_NO).HasColumnName("Num_ST_SEAT_NO");
        Property(t => t.Var_ST_NM).HasColumnName("Var_ST_NM");
        Property(t => t.Chr_ST_SEX_CD).HasColumnName("Chr_ST_SEX_CD");
        Property(t => t.Chr_ST_ADD1).HasColumnName("Chr_ST_ADD1");
        Property(t => t.Chr_ST_ADD2).HasColumnName("Chr_ST_ADD2");
        Property(t => t.Chr_ST_ADD3).HasColumnName("Chr_ST_ADD3");
        Property(t => t.Chr_ST_ADD4).HasColumnName("Chr_ST_ADD4");
        Property(t => t.Chr_ST_PINCODE).HasColumnName("Chr_ST_PINCODE");
        Property(t => t.Var_RES_PH).HasColumnName("Var_RES_PH");
        Property(t => t.Var_E_MAIL).HasColumnName("Var_E_MAIL");
        Property(t => t.Num_FK_CONVO_NO).HasColumnName("Num_FK_CONVO_NO");
        Property(t => t.Num_ST_PASS_MONTH).HasColumnName("Num_ST_PASS_MONTH");
        Property(t => t.Chr_ST_PASS_YEAR).HasColumnName("Chr_ST_PASS_YEAR");
        Property(t => t.Num_FK_RESULT_CD).HasColumnName("Num_FK_RESULT_CD");
        Property(t => t.Num_ST_CONVO_NO).HasColumnName("Num_ST_CONVO_NO");
        Property(t => t.Chr_ST_NMCHNG_FLG).HasColumnName("Chr_ST_NMCHNG_FLG");
        Property(t => t.Chr_ST_VALID_FLG).HasColumnName("Chr_ST_VALID_FLG");
        Property(t => t.Num_FK_COLLEGE_CD).HasColumnName("Num_FK_COLLEGE_CD");
        Property(t => t.Num_FK_BR_CD).HasColumnName("Num_FK_BR_CD");
        Property(t => t.Chr_FEES_STATUS).HasColumnName("Chr_FEES_STATUS");
        Property(t => t.Con_ST_FEES_AMT).HasColumnName("Con_ST_FEES_AMT");
        Property(t => t.Num_FK_FA_CD).HasColumnName("Num_FK_FA_CD");
        Property(t => t.Chr_PRINC_SUBJECT).HasColumnName("Chr_PRINC_SUBJECT");
        Property(t => t.Chr_PRINC_SUBJECT1).HasColumnName("Chr_PRINC_SUBJECT1");
        Property(t => t.Var_ST_USR_NM).HasColumnName("Var_ST_USR_NM");
        Property(t => t.ST_DTE_CR).HasColumnName("ST_DTE_CR");
        Property(t => t.ST_DTE_UP).HasColumnName("ST_DTE_UP");
        Property(t => t.Chr_DELETE_FLG).HasColumnName("Chr_DELETE_FLG");
        Property(t => t.Num_FK_INST_NO).HasColumnName("Num_FK_INST_NO");
        Property(t => t.Chr_Convo_Sts).HasColumnName("Chr_Convo_Sts");
        Property(t => t.Ima_ST_PHOTO).HasColumnName("Ima_ST_PHOTO");
        Property(t => t.Chr_Foreign_Student).HasColumnName("Chr_Foreign_Student");
        Property(t => t.Chr_Improvement).HasColumnName("Chr_Improvement");
        Property(t => t.DegreePart).HasColumnName("DegreePart");
        Property(t => t.Destination).HasColumnName("Destination");
        Property(t => t.Num_CGPA_AVG).HasColumnName("Num_CGPA_AVG");
    }
}