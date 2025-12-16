using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_COURSE_MSTRMap : EntityTypeConfiguration<Tbl_COURSE_MSTR>
{
    public Tbl_COURSE_MSTRMap()
    {
        // Primary Key
        //this.HasKey(t => new { t.Num_FK_FA_CD, t.Num_FK_TYP_CD, t.Num_PK_CO_CD, t.Var_CO_NM, t.Var_CO_SHRT_NM, t.Chr_CO_IMPRVO_FLG, t.Chr_CO_MIG_FLG, t.Chr_CO_AGR_FLG, t.Chr_CO_FIRST_ATT_FLG, t.Num_CO_MAX_DURATION, t.Num_ENROLL_NO });
        HasKey(t => new { t.Num_PK_CO_CD});


        // Properties
        Property(t => t.Num_FK_FA_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_FK_TYP_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_PK_CO_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Var_CO_NM)
            .IsRequired()
            .HasMaxLength(160);

        Property(t => t.Var_CO_SHRT_NM)
            .IsRequired()
            .HasMaxLength(50);

        Property(t => t.Chr_CO_IMPRVO_FLG)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_CO_MIG_FLG)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_CO_AGR_FLG)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_CO_FIRST_ATT_FLG)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_CO_GRAD_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Var_CO_NM_BL)
            .HasMaxLength(50);

        Property(t => t.Chr_SUB_DIST_APL_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_CO_VALID_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_REGISTRATION_DATA_POSTING)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_EXAMINATION_APPLICATION_FORM_POSTING)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_MARK_POSTING)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_PRN_GENERATION)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_EXAMINATION_APPLICATION_FORM_VALIDATION)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_REGISTRATION_VALIDATION)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_CENTER_ALLOCATION)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_SUB_VALIDATION)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_SEATNO_GENERATION)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_COURSE_VALIDATION)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_GROUP_VALIDATION)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_GRP_FAIL)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_RESULT_PROCESS)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Num_ENROLL_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Chr_DELETE_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.CHR_CO_MIG_DEGR_ALLOWED)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.CHR_CO_SECOND_DEGR_ALLOWED)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.CHR_CO_SECOND_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.CHR_CO_LOCK_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.CHR_MARK_VALIDATION)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Var_USR_NM)
            .HasMaxLength(12);

        Property(t => t.Chr_GEN_ORD_APL)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_Env_Studies)
            .IsFixedLength()
            .HasMaxLength(10);

        Property(t => t.Var_CO_NM_Certificate)
            .HasMaxLength(150);

        Property(t => t.Var_CO_NM_CertificateBold)
            .HasMaxLength(150);

        Property(t => t.Chr_AdditionalCredits)
            .IsFixedLength()
            .HasMaxLength(1);

        // Table & Column Mappings
        ToTable("Tbl_COURSE_MSTR");
        Property(t => t.Num_FK_FA_CD).HasColumnName("Num_FK_FA_CD");
        Property(t => t.Num_FK_TYP_CD).HasColumnName("Num_FK_TYP_CD");
        Property(t => t.Num_PK_CO_CD).HasColumnName("Num_PK_CO_CD");
        Property(t => t.Var_CO_NM).HasColumnName("Var_CO_NM");
        Property(t => t.Var_CO_SHRT_NM).HasColumnName("Var_CO_SHRT_NM");
        Property(t => t.Num_CO_SEQ_NO).HasColumnName("Num_CO_SEQ_NO");
        Property(t => t.Chr_CO_IMPRVO_FLG).HasColumnName("Chr_CO_IMPRVO_FLG");
        Property(t => t.Chr_CO_MIG_FLG).HasColumnName("Chr_CO_MIG_FLG");
        Property(t => t.Chr_CO_AGR_FLG).HasColumnName("Chr_CO_AGR_FLG");
        Property(t => t.Chr_CO_FIRST_ATT_FLG).HasColumnName("Chr_CO_FIRST_ATT_FLG");
        Property(t => t.Chr_CO_GRAD_FLG).HasColumnName("Chr_CO_GRAD_FLG");
        Property(t => t.Num_CO_MAX_DURATION).HasColumnName("Num_CO_MAX_DURATION");
        Property(t => t.Var_CO_NM_BL).HasColumnName("Var_CO_NM_BL");
        Property(t => t.Chr_SUB_DIST_APL_FLG).HasColumnName("Chr_SUB_DIST_APL_FLG");
        Property(t => t.Num_PERCENTAGE).HasColumnName("Num_PERCENTAGE");
        Property(t => t.Chr_CO_VALID_FLG).HasColumnName("Chr_CO_VALID_FLG");
        Property(t => t.Chr_REGISTRATION_DATA_POSTING).HasColumnName("Chr_REGISTRATION_DATA_POSTING");
        Property(t => t.Chr_EXAMINATION_APPLICATION_FORM_POSTING).HasColumnName("Chr_EXAMINATION_APPLICATION_FORM_POSTING");
        Property(t => t.Chr_MARK_POSTING).HasColumnName("Chr_MARK_POSTING");
        Property(t => t.Chr_PRN_GENERATION).HasColumnName("Chr_PRN_GENERATION");
        Property(t => t.Chr_EXAMINATION_APPLICATION_FORM_VALIDATION).HasColumnName("Chr_EXAMINATION_APPLICATION_FORM_VALIDATION");
        Property(t => t.Chr_REGISTRATION_VALIDATION).HasColumnName("Chr_REGISTRATION_VALIDATION");
        Property(t => t.Chr_CENTER_ALLOCATION).HasColumnName("Chr_CENTER_ALLOCATION");
        Property(t => t.Chr_SUB_VALIDATION).HasColumnName("Chr_SUB_VALIDATION");
        Property(t => t.Chr_SEATNO_GENERATION).HasColumnName("Chr_SEATNO_GENERATION");
        Property(t => t.Chr_COURSE_VALIDATION).HasColumnName("Chr_COURSE_VALIDATION");
        Property(t => t.Chr_GROUP_VALIDATION).HasColumnName("Chr_GROUP_VALIDATION");
        Property(t => t.Chr_GRP_FAIL).HasColumnName("Chr_GRP_FAIL");
        Property(t => t.Chr_RESULT_PROCESS).HasColumnName("Chr_RESULT_PROCESS");
        Property(t => t.Num_ENROLL_NO).HasColumnName("Num_ENROLL_NO");
        Property(t => t.Chr_DELETE_FLG).HasColumnName("Chr_DELETE_FLG");
        Property(t => t.CHR_CO_MIG_DEGR_ALLOWED).HasColumnName("CHR_CO_MIG_DEGR_ALLOWED");
        Property(t => t.CHR_CO_SECOND_DEGR_ALLOWED).HasColumnName("CHR_CO_SECOND_DEGR_ALLOWED");
        Property(t => t.CHR_CO_SECOND_FLG).HasColumnName("CHR_CO_SECOND_FLG");
        Property(t => t.CHR_CO_LOCK_FLG).HasColumnName("CHR_CO_LOCK_FLG");
        Property(t => t.CHR_MARK_VALIDATION).HasColumnName("CHR_MARK_VALIDATION");
        Property(t => t.Var_USR_NM).HasColumnName("Var_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
        Property(t => t.Chr_GEN_ORD_APL).HasColumnName("Chr_GEN_ORD_APL");
        Property(t => t.Chr_Env_Studies).HasColumnName("Chr_Env_Studies");
        Property(t => t.Num_Env_Studies_Maxmrk).HasColumnName("Num_Env_Studies_Maxmrk");
        Property(t => t.Num_Env_Studies_Passmrk).HasColumnName("Num_Env_Studies_Passmrk");
        Property(t => t.Num_COMMON_CO_CD).HasColumnName("Num_COMMON_CO_CD");
        Property(t => t.SUB_DIST_MARK_ADD).HasColumnName("SUB_DIST_MARK_ADD");
        Property(t => t.Var_CO_NM_Certificate).HasColumnName("Var_CO_NM_Certificate");
        Property(t => t.Var_CO_NM_CertificateBold).HasColumnName("Var_CO_NM_CertificateBold");
        Property(t => t.Num_DeanGracingApplicable).HasColumnName("Num_DeanGracingApplicable");
        Property(t => t.Chr_AdditionalCredits).HasColumnName("Chr_AdditionalCredits");
        Property(t => t.Chr_Registration_Active).HasColumnName("Chr_Registration_Active");
    }
}