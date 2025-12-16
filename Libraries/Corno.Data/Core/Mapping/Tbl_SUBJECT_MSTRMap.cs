using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_SUBJECT_MSTRMap : EntityTypeConfiguration<Tbl_SUBJECT_MSTR>
{
    public Tbl_SUBJECT_MSTRMap()
    {
        // Primary Key
        HasKey(t => new { t.Num_FK_COPRT_NO, t.Num_PK_SUB_CD, t.Var_SUBJECT_NM, t.Var_SUBJECT_SHRT_NM, t.Num_SUBJECT_MAX_MRK, t.Num_SUBJECT_PASS_MRK, t.Num_SUBJECT_EXMP_MRK, t.Num_SUBJECT_GRD_MIN, t.Num_SUBJECT_PRINT_SEQ, t.Num_SUBJECT_SR_NO, t.Chr_SUBJECT_STS_FLG });

        // Properties
        Property(t => t.Num_FK_COPRT_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_PK_SUB_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Var_SUBJECT_NM)
            .IsRequired()
            .HasMaxLength(100);

        Property(t => t.Var_SUBJECT_SHRT_NM)
            .IsRequired()
            .HasMaxLength(10);

        Property(t => t.Num_SUBJECT_MAX_MRK)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_SUBJECT_EXMP_MRK)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_SUBJECT_GRD_MIN)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_SUBJECT_PRINT_SEQ)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.CHR_WRT_UNI_APL)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_SUBJECT_ORD_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_SUBJECT_REVAL_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_SUBJECT_PRINT_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Var_SUBJECT_CODE_FLG)
            .HasMaxLength(1);

        Property(t => t.Var_SUBJECT_TEMP_CODE_FLG)
            .HasMaxLength(8);

        Property(t => t.Chr_SUBJECT_INCLUDE_CLASS)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Var_SUBJECT_TOTAL_FLG)
            .HasMaxLength(1);

        Property(t => t.Chr_NOTINCL_COPART_TOTAL)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Num_SUBJECT_SR_NO)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Var_SUBJECT_NM_BL)
            .HasMaxLength(50);

        Property(t => t.Chr_SUB_VALID_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_SUBJECT_ATKT_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Chr_SUBJECT_STS_FLG)
            .IsRequired()
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Var_SUBJ_CANCEL_FROM_MON)
            .HasMaxLength(4);

        Property(t => t.Var_SUBJ_CANCEL_FROM_YR)
            .HasMaxLength(4);

        Property(t => t.Chr_DELETE_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.var_USR_NM)
            .HasMaxLength(12);

        Property(t => t.Chr_SUBMRK_GRD_CONV_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Var_Subject_Sts)
            .HasMaxLength(1);

        Property(t => t.Chr_SCALEDOWN_APL)
            .IsFixedLength()
            .HasMaxLength(1);

        // Table & Column Mappings
        ToTable("Tbl_SUBJECT_MSTR");
        Property(t => t.Num_FK_COPRT_NO).HasColumnName("Num_FK_COPRT_NO");
        Property(t => t.Num_PK_SUB_CD).HasColumnName("Num_PK_SUB_CD");
        Property(t => t.Var_SUBJECT_NM).HasColumnName("Var_SUBJECT_NM");
        Property(t => t.Var_SUBJECT_SHRT_NM).HasColumnName("Var_SUBJECT_SHRT_NM");
        Property(t => t.Num_SUBJECT_MAX_MRK).HasColumnName("Num_SUBJECT_MAX_MRK");
        Property(t => t.Num_SUBJECT_PASS_MRK).HasColumnName("Num_SUBJECT_PASS_MRK");
        Property(t => t.Num_SUBJECT_EXMP_MRK).HasColumnName("Num_SUBJECT_EXMP_MRK");
        Property(t => t.Num_SUBJECT_GRD_MIN).HasColumnName("Num_SUBJECT_GRD_MIN");
        Property(t => t.Num_SUBJECT_PRINT_SEQ).HasColumnName("Num_SUBJECT_PRINT_SEQ");
        Property(t => t.Num_FK_BOS_CD).HasColumnName("Num_FK_BOS_CD");
        Property(t => t.CHR_WRT_UNI_APL).HasColumnName("CHR_WRT_UNI_APL");
        Property(t => t.Chr_SUBJECT_ORD_FLG).HasColumnName("Chr_SUBJECT_ORD_FLG");
        Property(t => t.Chr_SUBJECT_REVAL_FLG).HasColumnName("Chr_SUBJECT_REVAL_FLG");
        Property(t => t.Chr_SUBJECT_PRINT_FLG).HasColumnName("Chr_SUBJECT_PRINT_FLG");
        Property(t => t.Var_SUBJECT_CODE_FLG).HasColumnName("Var_SUBJECT_CODE_FLG");
        Property(t => t.Var_SUBJECT_TEMP_CODE_FLG).HasColumnName("Var_SUBJECT_TEMP_CODE_FLG");
        Property(t => t.Chr_SUBJECT_INCLUDE_CLASS).HasColumnName("Chr_SUBJECT_INCLUDE_CLASS");
        Property(t => t.Var_SUBJECT_TOTAL_FLG).HasColumnName("Var_SUBJECT_TOTAL_FLG");
        Property(t => t.Chr_NOTINCL_COPART_TOTAL).HasColumnName("Chr_NOTINCL_COPART_TOTAL");
        Property(t => t.Num_SUBJECT_SR_NO).HasColumnName("Num_SUBJECT_SR_NO");
        Property(t => t.Var_SUBJECT_NM_BL).HasColumnName("Var_SUBJECT_NM_BL");
        Property(t => t.Chr_SUB_VALID_FLG).HasColumnName("Chr_SUB_VALID_FLG");
        Property(t => t.Chr_SUBJECT_ATKT_FLG).HasColumnName("Chr_SUBJECT_ATKT_FLG");
        Property(t => t.Chr_SUBJECT_STS_FLG).HasColumnName("Chr_SUBJECT_STS_FLG");
        Property(t => t.Num_EQUI_SUB_CD).HasColumnName("Num_EQUI_SUB_CD");
        Property(t => t.Var_SUBJ_CANCEL_FROM_MON).HasColumnName("Var_SUBJ_CANCEL_FROM_MON");
        Property(t => t.Var_SUBJ_CANCEL_FROM_YR).HasColumnName("Var_SUBJ_CANCEL_FROM_YR");
        Property(t => t.Chr_DELETE_FLG).HasColumnName("Chr_DELETE_FLG");
        Property(t => t.var_USR_NM).HasColumnName("var_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
        Property(t => t.Chr_SUBMRK_GRD_CONV_FLG).HasColumnName("Chr_SUBMRK_GRD_CONV_FLG");
        Property(t => t.Var_Subject_Sts).HasColumnName("Var_Subject_Sts");
        Property(t => t.Chr_SCALEDOWN_APL).HasColumnName("Chr_SCALEDOWN_APL");
        Property(t => t.NUM_CAT_SCALEDOWN_FROM_T1).HasColumnName("NUM_CAT_SCALEDOWN_FROM_T1");
        Property(t => t.NUM_CAT_SCALEDOWN_TO_T1).HasColumnName("NUM_CAT_SCALEDOWN_TO_T1");
        Property(t => t.NUM_CAT_SCALEDOWN_FROM_T2).HasColumnName("NUM_CAT_SCALEDOWN_FROM_T2");
        Property(t => t.NUM_CAT_SCALEDOWN_TO_T2).HasColumnName("NUM_CAT_SCALEDOWN_TO_T2");
        Property(t => t.NUM_PAP_SCALEDOWN_FROM_T3).HasColumnName("NUM_PAP_SCALEDOWN_FROM_T3");
        Property(t => t.NUM_PAP_SCALEDOWN_TO_T3).HasColumnName("NUM_PAP_SCALEDOWN_TO_T3");
        Property(t => t.Num_SUBJECT_CREDIT).HasColumnName("Num_SUBJECT_CREDIT");
        Property(t => t.Num_MIN_GRD_POINT).HasColumnName("Num_MIN_GRD_POINT");
    }
}