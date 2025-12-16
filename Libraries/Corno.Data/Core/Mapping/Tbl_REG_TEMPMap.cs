using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_REG_TEMPMap : EntityTypeConfiguration<Tbl_REG_TEMP>
{
    public Tbl_REG_TEMPMap()
    {
        // Primary Key
        //this.HasKey(t => new { t.Num_PK_RECORD_NO, t.Num_INCREMENT_PART_INST });
        HasKey(t =>t.Num_PK_RECORD_NO)
            .Property(t => t.Num_PK_RECORD_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.Identity)
            .IsRequired();


        /*// Properties
        Property(t => t.Var_BUNDLE_NO)
            .HasMaxLength(10);

        Property(t => t.Chr_PK_FORM_NO)
            .HasMaxLength(10);

        Property(t => t.Chr_REG_TIFF_NO)
            .IsFixedLength()
            .HasMaxLength(15);

        Property(t => t.Chr_REG_VALID_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_REG_PRN_NO)
            .IsFixedLength()
            .HasMaxLength(10);

        Property(t => t.Chr_REG_COPRT_NO)
            .IsFixedLength()
            .HasMaxLength(4);

        Property(t => t.Chr_REG_EXM_PAT_CD)
            .IsFixedLength()
            .HasMaxLength(2);

        Property(t => t.Chr_REG_BR_CD)
            .IsFixedLength()
            .HasMaxLength(4);

        Property(t => t.Chr_REG_YEAR)
            .IsFixedLength()
            .HasMaxLength(4);

        Property(t => t.Chr_REG_MONTH)
            .IsFixedLength()
            .HasMaxLength(2);

        Property(t => t.Chr_REG_ST_NM)
            .HasMaxLength(100);

        Property(t => t.Chr_REG_FATH_NM)
            .HasMaxLength(30);

        Property(t => t.Chr_REG_MOTH_NM)
            .HasMaxLength(50);

        Property(t => t.Chr_REG_CAST_CD)
            .IsFixedLength()
            .HasMaxLength(4);

        Property(t => t.Var_REG_RELIGION)
            .HasMaxLength(50);

        Property(t => t.Var_REG_CASTE)
            .HasMaxLength(20);

        Property(t => t.Chr_REG_SEX_CD)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_REG_DOB_DT)
            .IsFixedLength()
            .HasMaxLength(2);

        Property(t => t.Chr_REG_DOB_MONTH)
            .IsFixedLength()
            .HasMaxLength(2);

        Property(t => t.Chr_REG_DOB_YEAR)
            .IsFixedLength()
            .HasMaxLength(4);

        Property(t => t.Chr_REG_ADD1)
            .IsFixedLength()
            .HasMaxLength(60);

        Property(t => t.Chr_REG_ADD2)
            .IsFixedLength()
            .HasMaxLength(60);

        Property(t => t.Chr_REG_ADD3)
            .IsFixedLength()
            .HasMaxLength(60);

        Property(t => t.Chr_REG_CITY)
            .IsFixedLength()
            .HasMaxLength(20);

        Property(t => t.Chr_REG_DISTRICT)
            .IsFixedLength()
            .HasMaxLength(20);

        Property(t => t.Chr_REG_PIN)
            .IsFixedLength()
            .HasMaxLength(6);

        Property(t => t.Chr_REG_ENTRY_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_REG_COLLEGE_CD)
            .IsFixedLength()
            .HasMaxLength(6);

        Property(t => t.Chr_REG_ADM_ROLL)
            .IsFixedLength()
            .HasMaxLength(5);

        Property(t => t.Chr_REG_FEES)
            .IsFixedLength()
            .HasMaxLength(7);

        Property(t => t.Chr_REG_INCM_CD)
            .IsFixedLength()
            .HasMaxLength(2);

        Property(t => t.Var_CHKLIST_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_REPEATER_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_USR_NM)
            .IsFixedLength()
            .HasMaxLength(12);

        Property(t => t.Chr_STUDENT_NATIONALITY)
            .HasMaxLength(1);

        Property(t => t.Var_OLD_PRN_NO)
            .HasMaxLength(25);

        Property(t => t.Var_FOREIGN_ADD)
            .HasMaxLength(100);

        Property(t => t.Var_FOREIGN_CITY)
            .HasMaxLength(50);

        Property(t => t.Var_COUNTRY)
            .HasMaxLength(50);

        Property(t => t.Var_PASSPORT)
            .HasMaxLength(20);

        Property(t => t.Chr_FATHER_NAME)
            .HasMaxLength(100);

        Property(t => t.Num_PHONE)
            .HasMaxLength(15);

        Property(t => t.Num_PHONE1)
            .HasMaxLength(15);

        Property(t => t.Num_MOBILE)
            .HasMaxLength(15);

        Property(t => t.Chr_GUARDIAN)
            .HasMaxLength(1);

        Property(t => t.Chr_PREV_EXAM)
            .HasMaxLength(20);

        Property(t => t.Chr_PREV_EXAM_UNI)
            .HasMaxLength(20);

        Property(t => t.Chr_PREV_EXAM_SEATNO)
            .HasMaxLength(20);

        Property(t => t.Chr_PREV_EXAM_PERCENT)
            .HasMaxLength(5);

        Property(t => t.Chr_PREV_EXAM_MON_YR)
            .HasMaxLength(20);

        Property(t => t.Chr_DIST_EDU)
            .HasMaxLength(1);

        Property(t => t.Chr_MIGRATION_FLG)
            .HasMaxLength(1);

        Property(t => t.Chr_LOCAL_ADD)
            .HasMaxLength(100);

        Property(t => t.Chr_PERMANENT_ADD)
            .HasMaxLength(100);

        Property(t => t.Chr_GUARDIAN_ADD)
            .HasMaxLength(100);

        Property(t => t.Chr_IMPROVEMENT_FLG)
            .HasMaxLength(1);

        Property(t => t.Num_INCREMENT_PART_INST)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_PHONE_GRDAN)
            .HasMaxLength(15);

        Property(t => t.Chr_LICENSE_NO)
            .HasMaxLength(15);

        Property(t => t.Chr_BLOOD_GROUP)
            .HasMaxLength(10);

        Property(t => t.Chr_PAN_CARD_NO)
            .HasMaxLength(10);

        Property(t => t.Chr_Num_Enclose)
            .HasMaxLength(30);*/

        // Table & Column Mappings
        ToTable("Tbl_REG_TEMP");
        //this.Property(t => t.Num_PK_RECORD_NO).HasColumnName("Num_PK_RECORD_NO");
        //this.Property(t => t.Var_BUNDLE_NO).HasColumnName("Var_BUNDLE_NO");
        //this.Property(t => t.Chr_PK_FORM_NO).HasColumnName("Chr_PK_FORM_NO");
        //this.Property(t => t.Chr_REG_TIFF_NO).HasColumnName("Chr_REG_TIFF_NO");
        //this.Property(t => t.Chr_REG_VALID_FLG).HasColumnName("Chr_REG_VALID_FLG");
        //this.Property(t => t.Chr_REG_PRN_NO).HasColumnName("Chr_REG_PRN_NO");
        //this.Property(t => t.Chr_REG_COPRT_NO).HasColumnName("Chr_REG_COPRT_NO");
        //this.Property(t => t.Chr_REG_EXM_PAT_CD).HasColumnName("Chr_REG_EXM_PAT_CD");
        //this.Property(t => t.Chr_REG_BR_CD).HasColumnName("Chr_REG_BR_CD");
        //this.Property(t => t.Num_FK_INST_NO).HasColumnName("Num_FK_INST_NO");
        //this.Property(t => t.Chr_REG_YEAR).HasColumnName("Chr_REG_YEAR");
        //this.Property(t => t.Chr_REG_MONTH).HasColumnName("Chr_REG_MONTH");
        //this.Property(t => t.Chr_REG_ST_NM).HasColumnName("Chr_REG_ST_NM");
        //this.Property(t => t.Chr_REG_FATH_NM).HasColumnName("Chr_REG_FATH_NM");
        //this.Property(t => t.Chr_REG_MOTH_NM).HasColumnName("Chr_REG_MOTH_NM");
        //this.Property(t => t.Chr_REG_CAST_CD).HasColumnName("Chr_REG_CAST_CD");
        //this.Property(t => t.Var_REG_RELIGION).HasColumnName("Var_REG_RELIGION");
        //this.Property(t => t.Var_REG_CASTE).HasColumnName("Var_REG_CASTE");
        //this.Property(t => t.Chr_REG_SEX_CD).HasColumnName("Chr_REG_SEX_CD");
        //this.Property(t => t.Chr_REG_DOB_DT).HasColumnName("Chr_REG_DOB_DT");
        //this.Property(t => t.Chr_REG_DOB_MONTH).HasColumnName("Chr_REG_DOB_MONTH");
        //this.Property(t => t.Chr_REG_DOB_YEAR).HasColumnName("Chr_REG_DOB_YEAR");
        //this.Property(t => t.Chr_REG_ADD1).HasColumnName("Chr_REG_ADD1");
        //this.Property(t => t.Chr_REG_ADD2).HasColumnName("Chr_REG_ADD2");
        //this.Property(t => t.Chr_REG_ADD3).HasColumnName("Chr_REG_ADD3");
        //this.Property(t => t.Chr_REG_CITY).HasColumnName("Chr_REG_CITY");
        //this.Property(t => t.Chr_REG_DISTRICT).HasColumnName("Chr_REG_DISTRICT");
        //this.Property(t => t.Chr_REG_PIN).HasColumnName("Chr_REG_PIN");
        //this.Property(t => t.Chr_REG_ENTRY_FLG).HasColumnName("Chr_REG_ENTRY_FLG");
        //this.Property(t => t.Chr_REG_COLLEGE_CD).HasColumnName("Chr_REG_COLLEGE_CD");
        //this.Property(t => t.Chr_REG_ADM_ROLL).HasColumnName("Chr_REG_ADM_ROLL");
        //this.Property(t => t.Chr_REG_FEES).HasColumnName("Chr_REG_FEES");
        //this.Property(t => t.Ima_REG_PHOTO).HasColumnName("Ima_REG_PHOTO");
        //this.Property(t => t.Ima_REG_ADDRESS).HasColumnName("Ima_REG_ADDRESS");
        //this.Property(t => t.Chr_REG_INCM_CD).HasColumnName("Chr_REG_INCM_CD");
        //this.Property(t => t.Var_CHKLIST_FLG).HasColumnName("Var_CHKLIST_FLG");
        //this.Property(t => t.Chr_REPEATER_FLG).HasColumnName("Chr_REPEATER_FLG");
        //this.Property(t => t.Chr_USR_NM).HasColumnName("Chr_USR_NM");
        //this.Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        //this.Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
        //this.Property(t => t.Chr_STUDENT_NATIONALITY).HasColumnName("Chr_STUDENT_NATIONALITY");
        //this.Property(t => t.Var_OLD_PRN_NO).HasColumnName("Var_OLD_PRN_NO");
        //this.Property(t => t.Var_FOREIGN_ADD).HasColumnName("Var_FOREIGN_ADD");
        //this.Property(t => t.Var_FOREIGN_CITY).HasColumnName("Var_FOREIGN_CITY");
        //this.Property(t => t.Var_COUNTRY).HasColumnName("Var_COUNTRY");
        //this.Property(t => t.Var_PASSPORT).HasColumnName("Var_PASSPORT");
        //this.Property(t => t.Chr_FATHER_NAME).HasColumnName("Chr_FATHER_NAME");
        //this.Property(t => t.Num_PHONE).HasColumnName("Num_PHONE");
        //this.Property(t => t.Num_PHONE1).HasColumnName("Num_PHONE1");
        //this.Property(t => t.Num_MOBILE).HasColumnName("Num_MOBILE");
        //this.Property(t => t.Chr_GUARDIAN).HasColumnName("Chr_GUARDIAN");
        //this.Property(t => t.Chr_PREV_EXAM).HasColumnName("Chr_PREV_EXAM");
        //this.Property(t => t.Chr_PREV_EXAM_UNI).HasColumnName("Chr_PREV_EXAM_UNI");
        //this.Property(t => t.Chr_PREV_EXAM_SEATNO).HasColumnName("Chr_PREV_EXAM_SEATNO");
        //this.Property(t => t.Chr_PREV_EXAM_PERCENT).HasColumnName("Chr_PREV_EXAM_PERCENT");
        //this.Property(t => t.Chr_PREV_EXAM_MON_YR).HasColumnName("Chr_PREV_EXAM_MON_YR");
        //this.Property(t => t.Chr_DIST_EDU).HasColumnName("Chr_DIST_EDU");
        //this.Property(t => t.Chr_MIGRATION_FLG).HasColumnName("Chr_MIGRATION_FLG");
        //this.Property(t => t.Chr_LOCAL_ADD).HasColumnName("Chr_LOCAL_ADD");
        //this.Property(t => t.Chr_PERMANENT_ADD).HasColumnName("Chr_PERMANENT_ADD");
        //this.Property(t => t.Chr_GUARDIAN_ADD).HasColumnName("Chr_GUARDIAN_ADD");
        //this.Property(t => t.Chr_IMPROVEMENT_FLG).HasColumnName("Chr_IMPROVEMENT_FLG");
        //this.Property(t => t.Num_INCREMENT_PART_INST).HasColumnName("Num_INCREMENT_PART_INST");
        //this.Property(t => t.Num_PHONE_GRDAN).HasColumnName("Num_PHONE_GRDAN");
        //this.Property(t => t.Chr_LICENSE_NO).HasColumnName("Chr_LICENSE_NO");
        //this.Property(t => t.Chr_BLOOD_GROUP).HasColumnName("Chr_BLOOD_GROUP");
        //this.Property(t => t.Chr_PAN_CARD_NO).HasColumnName("Chr_PAN_CARD_NO");
        //this.Property(t => t.Chr_Num_Enclose).HasColumnName("Chr_Num_Enclose");
        //this.Property(t => t.Num_FK_DistCenter_ID).HasColumnName("Num_FK_DistCenter_ID");
    }
}