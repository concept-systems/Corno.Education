using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_COURSE_PART_MSTRMap : EntityTypeConfiguration<Tbl_COURSE_PART_MSTR>
{
    public Tbl_COURSE_PART_MSTRMap()
    {
        // Primary Key
        //this.HasKey(t => new { t.Num_FK_CO_CD, t.Num_PK_COPRT_NO, t.Num_COPRT_PART_NO, t.Num_COPRT_SEMI_NO, t.Var_COPRT_DESC, t.Var_COPRT_SHRT_NM, t.Chr_COPRT_RESULT_FLG, t.Chr_COPRT_ORD_FLG, t.Chr_COPRT_CLASS_FLG, t.Chr_COPRT_SCALE_FLG, t.Chr_COPRT_BRANCH_APP_FLG, t.Chr_COPRT_GRP_APP_FLG, t.Chr_COPRT_GRP_PRINT_APP_FLG, t.Chr_COPRT_GRP_PREEXAM_APL_FLG, t.Chr_COPRT_GRP_RES_APP_FLG, t.Chr_COPRT_CODING_FLG, t.Chr_COPRT_QPAPER_CODING_FLG, t.Chr_COPRT_EXMP_FLG });
        HasKey(t => new { t.Num_PK_COPRT_NO});


        // Properties
        Property(t => t.Num_FK_CO_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_PK_COPRT_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_COPRT_PART_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_COPRT_SEMI_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Var_COPRT_DESC)
            .IsRequired()
            .HasMaxLength(60);

        Property(t => t.Var_COPRT_SHRT_NM)
            .IsRequired()
            .HasMaxLength(50);

        Property(t => t.Chr_COPRT_RESULT_FLG)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_COPRT_ORD_FLG)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_COPRT_CLASS_FLG)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_COPRT_SCALE_FLG)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_PHY_EDU_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_COPRT_BRANCH_APP_FLG)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_COPRT_GRP_APP_FLG)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_COPRT_GRP_PRINT_APP_FLG)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_COPRT_GRP_PREEXAM_APL_FLG)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_COPRT_GRP_RES_APP_FLG)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_COPRT_CODING_FLG)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_COPRT_QPAPER_CODING_FLG)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_COPRT_EXMP_FLG)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_SUB_CODE_DECODE_APL_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_COPRT_EVAL_TYPE)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_COPRT_REJECT_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Var_COPRT_DESC_BL)
            .HasMaxLength(60);

        Property(t => t.Chr_CGPA_APL_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_MRK_GRD_CONV_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_COTOT_PH_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_COTOT_ATKT_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_ATKT_APL_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_ATKT_APL_TYPE)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_FAILHD_TYPE_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_ELIGIBLITY_APL_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_COPRT_VALID_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.CHR_SUB_MRK_PRINT)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.CHR_CAT_MRK_PRINT)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.CHR_COPRT_CLASS_YEARLY)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.CHR_OTH_ACTIVITY_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.CHR_COPRT_YRLY_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.CHR_PAP_MRK_PRINT)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.CHR_PRACTICE_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.CHR_PRACTICE_NAME)
            .HasMaxLength(60);

        Property(t => t.Chr_DELETE_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Var_USR_NM)
            .HasMaxLength(12);

        Property(t => t.Chr_DEG_APL_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_NotMARK_GRADE_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_YEARLY_RES_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_YEARLY_AGGR_RES_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_FAIL_ALL_CATEGORIES)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_AdditionalCredits)
            .IsFixedLength()
            .HasMaxLength(1);

        // Table & Column Mappings
        ToTable("Tbl_COURSE_PART_MSTR");
        Property(t => t.Num_FK_CO_CD).HasColumnName("Num_FK_CO_CD");
        Property(t => t.Num_PK_COPRT_NO).HasColumnName("Num_PK_COPRT_NO");
        Property(t => t.Num_COPRT_PART_NO).HasColumnName("Num_COPRT_PART_NO");
        Property(t => t.Num_COPRT_SEMI_NO).HasColumnName("Num_COPRT_SEMI_NO");
        Property(t => t.Var_COPRT_DESC).HasColumnName("Var_COPRT_DESC");
        Property(t => t.Var_COPRT_SHRT_NM).HasColumnName("Var_COPRT_SHRT_NM");
        Property(t => t.Chr_COPRT_RESULT_FLG).HasColumnName("Chr_COPRT_RESULT_FLG");
        Property(t => t.Chr_COPRT_ORD_FLG).HasColumnName("Chr_COPRT_ORD_FLG");
        Property(t => t.Chr_COPRT_CLASS_FLG).HasColumnName("Chr_COPRT_CLASS_FLG");
        Property(t => t.Chr_COPRT_SCALE_FLG).HasColumnName("Chr_COPRT_SCALE_FLG");
        Property(t => t.CHR_SCALINGDWN_PERCENTAGE).HasColumnName("CHR_SCALINGDWN_PERCENTAGE");
        Property(t => t.Num_COPRT_MAX_YRS_TO_CMPLT).HasColumnName("Num_COPRT_MAX_YRS_TO_CMPLT");
        Property(t => t.Num_COPRT_MAX_ATMPTS_TO_CMPLT).HasColumnName("Num_COPRT_MAX_ATMPTS_TO_CMPLT");
        Property(t => t.Chr_PHY_EDU_FLG).HasColumnName("Chr_PHY_EDU_FLG");
        Property(t => t.Chr_PHY_EDU_MRK).HasColumnName("Chr_PHY_EDU_MRK");
        Property(t => t.Chr_COPRT_BRANCH_APP_FLG).HasColumnName("Chr_COPRT_BRANCH_APP_FLG");
        Property(t => t.Chr_COPRT_GRP_APP_FLG).HasColumnName("Chr_COPRT_GRP_APP_FLG");
        Property(t => t.Chr_COPRT_GRP_PRINT_APP_FLG).HasColumnName("Chr_COPRT_GRP_PRINT_APP_FLG");
        Property(t => t.Chr_COPRT_GRP_PREEXAM_APL_FLG).HasColumnName("Chr_COPRT_GRP_PREEXAM_APL_FLG");
        Property(t => t.Chr_COPRT_GRP_RES_APP_FLG).HasColumnName("Chr_COPRT_GRP_RES_APP_FLG");
        Property(t => t.Chr_COPRT_CODING_FLG).HasColumnName("Chr_COPRT_CODING_FLG");
        Property(t => t.Chr_COPRT_QPAPER_CODING_FLG).HasColumnName("Chr_COPRT_QPAPER_CODING_FLG");
        Property(t => t.Chr_COPRT_EXMP_FLG).HasColumnName("Chr_COPRT_EXMP_FLG");
        Property(t => t.Chr_SUB_CODE_DECODE_APL_FLG).HasColumnName("Chr_SUB_CODE_DECODE_APL_FLG");
        Property(t => t.Num_COPRT_TOT_SUBJECT).HasColumnName("Num_COPRT_TOT_SUBJECT");
        Property(t => t.Num_COPRT_EVAL_NO).HasColumnName("Num_COPRT_EVAL_NO");
        Property(t => t.Chr_COPRT_EVAL_TYPE).HasColumnName("Chr_COPRT_EVAL_TYPE");
        Property(t => t.Chr_COPRT_REJECT_FLG).HasColumnName("Chr_COPRT_REJECT_FLG");
        Property(t => t.Var_COPRT_DESC_BL).HasColumnName("Var_COPRT_DESC_BL");
        Property(t => t.Chr_CGPA_APL_FLG).HasColumnName("Chr_CGPA_APL_FLG");
        Property(t => t.Chr_MRK_GRD_CONV_FLG).HasColumnName("Chr_MRK_GRD_CONV_FLG");
        Property(t => t.Flt_DIFF_EVAL_PER).HasColumnName("Flt_DIFF_EVAL_PER");
        Property(t => t.Chr_COTOT_PH_FLG).HasColumnName("Chr_COTOT_PH_FLG");
        Property(t => t.Chr_COTOT_ATKT_FLG).HasColumnName("Chr_COTOT_ATKT_FLG");
        Property(t => t.Chr_ATKT_APL_FLG).HasColumnName("Chr_ATKT_APL_FLG");
        Property(t => t.Chr_ATKT_APL_TYPE).HasColumnName("Chr_ATKT_APL_TYPE");
        Property(t => t.Chr_ATKT_SUBNO).HasColumnName("Chr_ATKT_SUBNO");
        Property(t => t.Chr_FAILHD_TYPE_FLG).HasColumnName("Chr_FAILHD_TYPE_FLG");
        Property(t => t.Chr_ELIGIBLITY_APL_FLG).HasColumnName("Chr_ELIGIBLITY_APL_FLG");
        Property(t => t.Chr_COPRT_VALID_FLG).HasColumnName("Chr_COPRT_VALID_FLG");
        Property(t => t.NUM_ADD_SUB).HasColumnName("NUM_ADD_SUB");
        Property(t => t.CHR_SUB_MRK_PRINT).HasColumnName("CHR_SUB_MRK_PRINT");
        Property(t => t.CHR_CAT_MRK_PRINT).HasColumnName("CHR_CAT_MRK_PRINT");
        Property(t => t.CHR_COPRT_CLASS_YEARLY).HasColumnName("CHR_COPRT_CLASS_YEARLY");
        Property(t => t.CHR_OTH_ACTIVITY_FLG).HasColumnName("CHR_OTH_ACTIVITY_FLG");
        Property(t => t.CHR_OTH_ACTIVITY_MRK).HasColumnName("CHR_OTH_ACTIVITY_MRK");
        Property(t => t.CHR_COPRT_YRLY_FLG).HasColumnName("CHR_COPRT_YRLY_FLG");
        Property(t => t.CHR_COTOT_PASS_PERCENTAGE).HasColumnName("CHR_COTOT_PASS_PERCENTAGE");
        Property(t => t.CHR_COTOT_PH_FAIL_NO_OF_HEADS_APPEAR).HasColumnName("CHR_COTOT_PH_FAIL_NO_OF_HEADS_APPEAR");
        Property(t => t.CHR_PAP_MRK_PRINT).HasColumnName("CHR_PAP_MRK_PRINT");
        Property(t => t.CHR_PRACTICE_FLG).HasColumnName("CHR_PRACTICE_FLG");
        Property(t => t.CHR_PRACTICE_NAME).HasColumnName("CHR_PRACTICE_NAME");
        Property(t => t.Chr_DELETE_FLG).HasColumnName("Chr_DELETE_FLG");
        Property(t => t.Var_USR_NM).HasColumnName("Var_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
        Property(t => t.Chr_DEG_APL_FLG).HasColumnName("Chr_DEG_APL_FLG");
        Property(t => t.NUM_CAT_HD_PASS).HasColumnName("NUM_CAT_HD_PASS");
        Property(t => t.Num_COPRT_SEQ_NO).HasColumnName("Num_COPRT_SEQ_NO");
        Property(t => t.Chr_NotMARK_GRADE_FLG).HasColumnName("Chr_NotMARK_GRADE_FLG");
        Property(t => t.Chr_YEARLY_RES_FLG).HasColumnName("Chr_YEARLY_RES_FLG");
        Property(t => t.Chr_YEARLY_AGGR_RES_FLG).HasColumnName("Chr_YEARLY_AGGR_RES_FLG");
        Property(t => t.Chr_FAIL_ALL_CATEGORIES).HasColumnName("Chr_FAIL_ALL_CATEGORIES");
        Property(t => t.Chr_AdditionalCredits).HasColumnName("Chr_AdditionalCredits");
        Property(t => t.Num_ClassMinPercentage).HasColumnName("Num_ClassMinPercentage");
        Property(t => t.Num_FK_Degree_CD).HasColumnName("Num_FK_Degree_CD");
    }
}