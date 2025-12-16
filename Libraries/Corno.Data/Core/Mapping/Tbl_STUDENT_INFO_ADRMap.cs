using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_STUDENT_INFO_ADRMap : EntityTypeConfiguration<Tbl_STUDENT_INFO_ADR>
{
    public Tbl_STUDENT_INFO_ADRMap()
    {
        // Primary Key
        HasKey(t => t.Chr_FK_PRN_NO);

        // Properties
        Property(t => t.Chr_FK_PRN_NO)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(10);

        Property(t => t.Var_ST_FATH_NM)
            .HasMaxLength(30);

        Property(t => t.Var_ST_MOTH_NM)
            .HasMaxLength(30);

        Property(t => t.Chr_ST_ADD1)
            .IsFixedLength()
            .HasMaxLength(40);

        Property(t => t.Chr_ST_ADD2)
            .IsFixedLength()
            .HasMaxLength(40);

        Property(t => t.Chr_ST_ADD3)
            .IsFixedLength()
            .HasMaxLength(40);

        Property(t => t.Chr_ST_CITY)
            .IsFixedLength()
            .HasMaxLength(20);

        Property(t => t.Chr_ST_DISTRICT)
            .IsFixedLength()
            .HasMaxLength(20);

        Property(t => t.Chr_ST_PINCODE)
            .IsFixedLength()
            .HasMaxLength(6);

        Property(t => t.Var_REG_RELIGION)
            .HasMaxLength(10);

        Property(t => t.Var_REG_CASTE)
            .HasMaxLength(10);

        Property(t => t.Chr_ST_TIFF_NO)
            .IsFixedLength()
            .HasMaxLength(15);

        Property(t => t.Ima_Flg)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_USR_NM)
            .IsFixedLength()
            .HasMaxLength(12);

        Property(t => t.Var_FOREIGN_ADD)
            .HasMaxLength(100);

        Property(t => t.Var_FOREIGN_CITY)
            .HasMaxLength(50);

        Property(t => t.Var_COUNTRY)
            .HasMaxLength(50);

        Property(t => t.Var_PASSPORT)
            .HasMaxLength(20);

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
            .HasMaxLength(10);

        Property(t => t.Chr_PREV_EXAM_PERCENT)
            .HasMaxLength(5);

        Property(t => t.Chr_PREV_EXAM_MON_YR)
            .HasMaxLength(20);

        Property(t => t.Chr_DIST_EDU)
            .HasMaxLength(1);

        Property(t => t.Chr_LOCAL_ADD)
            .HasMaxLength(100);

        Property(t => t.Chr_PERMANENT_ADD)
            .HasMaxLength(100);

        Property(t => t.Chr_GUARDIAN_ADD)
            .HasMaxLength(100);

        Property(t => t.Num_PHONE_GRDAN)
            .HasMaxLength(15);

        Property(t => t.Chr_LICENSE_NO)
            .HasMaxLength(15);

        Property(t => t.Chr_BLOOD_GROUP)
            .HasMaxLength(10);

        Property(t => t.Chr_PAN_CARD_NO)
            .HasMaxLength(10);

        // Table & Column Mappings
        ToTable("Tbl_STUDENT_INFO_ADR");
        Property(t => t.Chr_FK_PRN_NO).HasColumnName("Chr_FK_PRN_NO");
        Property(t => t.Var_ST_FATH_NM).HasColumnName("Var_ST_FATH_NM");
        Property(t => t.Var_ST_MOTH_NM).HasColumnName("Var_ST_MOTH_NM");
        Property(t => t.Chr_ST_ADD1).HasColumnName("Chr_ST_ADD1");
        Property(t => t.Chr_ST_ADD2).HasColumnName("Chr_ST_ADD2");
        Property(t => t.Chr_ST_ADD3).HasColumnName("Chr_ST_ADD3");
        Property(t => t.Chr_ST_CITY).HasColumnName("Chr_ST_CITY");
        Property(t => t.Chr_ST_DISTRICT).HasColumnName("Chr_ST_DISTRICT");
        Property(t => t.Chr_ST_PINCODE).HasColumnName("Chr_ST_PINCODE");
        Property(t => t.Var_REG_RELIGION).HasColumnName("Var_REG_RELIGION");
        Property(t => t.Var_REG_CASTE).HasColumnName("Var_REG_CASTE");
        Property(t => t.Chr_ST_TIFF_NO).HasColumnName("Chr_ST_TIFF_NO");
        Property(t => t.Ima_ST_PHOTO).HasColumnName("Ima_ST_PHOTO");
        Property(t => t.Ima_ST_ADDRESS).HasColumnName("Ima_ST_ADDRESS");
        Property(t => t.Ima_Flg).HasColumnName("Ima_Flg");
        Property(t => t.Chr_USR_NM).HasColumnName("Chr_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
        Property(t => t.Var_FOREIGN_ADD).HasColumnName("Var_FOREIGN_ADD");
        Property(t => t.Var_FOREIGN_CITY).HasColumnName("Var_FOREIGN_CITY");
        Property(t => t.Var_COUNTRY).HasColumnName("Var_COUNTRY");
        Property(t => t.Var_PASSPORT).HasColumnName("Var_PASSPORT");
        Property(t => t.Num_PHONE).HasColumnName("Num_PHONE");
        Property(t => t.Num_PHONE1).HasColumnName("Num_PHONE1");
        Property(t => t.Num_MOBILE).HasColumnName("Num_MOBILE");
        Property(t => t.Chr_GUARDIAN).HasColumnName("Chr_GUARDIAN");
        Property(t => t.Chr_PREV_EXAM).HasColumnName("Chr_PREV_EXAM");
        Property(t => t.Chr_PREV_EXAM_UNI).HasColumnName("Chr_PREV_EXAM_UNI");
        Property(t => t.Chr_PREV_EXAM_SEATNO).HasColumnName("Chr_PREV_EXAM_SEATNO");
        Property(t => t.Chr_PREV_EXAM_PERCENT).HasColumnName("Chr_PREV_EXAM_PERCENT");
        Property(t => t.Chr_PREV_EXAM_MON_YR).HasColumnName("Chr_PREV_EXAM_MON_YR");
        Property(t => t.Chr_DIST_EDU).HasColumnName("Chr_DIST_EDU");
        Property(t => t.Chr_LOCAL_ADD).HasColumnName("Chr_LOCAL_ADD");
        Property(t => t.Chr_PERMANENT_ADD).HasColumnName("Chr_PERMANENT_ADD");
        Property(t => t.Chr_GUARDIAN_ADD).HasColumnName("Chr_GUARDIAN_ADD");
        Property(t => t.Num_PHONE_GRDAN).HasColumnName("Num_PHONE_GRDAN");
        Property(t => t.Chr_LICENSE_NO).HasColumnName("Chr_LICENSE_NO");
        Property(t => t.Chr_BLOOD_GROUP).HasColumnName("Chr_BLOOD_GROUP");
        Property(t => t.Chr_PAN_CARD_NO).HasColumnName("Chr_PAN_CARD_NO");
    }
}