using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_COLLEGE_MSTRMap : EntityTypeConfiguration<Tbl_COLLEGE_MSTR>
{
    public Tbl_COLLEGE_MSTRMap()
    {
        // Primary Key
        HasKey(t => new { t.Num_PK_COLLEGE_CD, t.Num_FK_DIST_CD, t.Var_CL_COLLEGE_NM1, t.Var_CL_SHRT_NM, t.Var_CL_CITY_NM });

        // Properties
        Property(t => t.Num_PK_COLLEGE_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Num_FK_DIST_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.Var_CL_COLLEGE_NM1)
            .IsRequired()
            .HasMaxLength(140);

        Property(t => t.Var_CL_COLLEGE_NM2)
            .HasMaxLength(75);

        Property(t => t.Var_CL_SHRT_NM)
            .IsRequired()
            .HasMaxLength(50);

        Property(t => t.Var_CL_COLLEGE_ADD1)
            .HasMaxLength(40);

        Property(t => t.Var_CL_COLLEGE_ADD2)
            .HasMaxLength(40);

        Property(t => t.Var_CL_COLLEGE_ADD3)
            .HasMaxLength(40);

        Property(t => t.Var_CL_CITY_NM)
            .IsRequired()
            .HasMaxLength(20);

        Property(t => t.Var_CL_PIN_CD)
            .HasMaxLength(6);

        Property(t => t.Var_CL_PH1)
            .HasMaxLength(14);

        Property(t => t.Var_CL_PH2)
            .HasMaxLength(14);

        Property(t => t.Var_CL_PH3)
            .HasMaxLength(14);

        Property(t => t.Var_CL_FAX)
            .HasMaxLength(14);

        Property(t => t.Var_CL_E_MAIL)
            .HasMaxLength(30);

        Property(t => t.Var_CL_WEB)
            .HasMaxLength(30);

        Property(t => t.Chr_DELETE_FLG)
            .IsFixedLength()
            .HasMaxLength(1);

        Property(t => t.Var_USR_NM)
            .HasMaxLength(12);

        Property(t => t.Chr_DISTANCE_EDU)
            .IsFixedLength()
            .HasMaxLength(1);

        // Table & Column Mappings
        ToTable("Tbl_COLLEGE_MSTR");
        Property(t => t.Num_PK_COLLEGE_CD).HasColumnName("Num_PK_COLLEGE_CD");
        Property(t => t.Num_FK_DIST_CD).HasColumnName("Num_FK_DIST_CD");
        Property(t => t.Var_CL_COLLEGE_NM1).HasColumnName("Var_CL_COLLEGE_NM1");
        Property(t => t.Var_CL_COLLEGE_NM2).HasColumnName("Var_CL_COLLEGE_NM2");
        Property(t => t.Var_CL_SHRT_NM).HasColumnName("Var_CL_SHRT_NM");
        Property(t => t.Var_CL_COLLEGE_ADD1).HasColumnName("Var_CL_COLLEGE_ADD1");
        Property(t => t.Var_CL_COLLEGE_ADD2).HasColumnName("Var_CL_COLLEGE_ADD2");
        Property(t => t.Var_CL_COLLEGE_ADD3).HasColumnName("Var_CL_COLLEGE_ADD3");
        Property(t => t.Var_CL_CITY_NM).HasColumnName("Var_CL_CITY_NM");
        Property(t => t.Var_CL_PIN_CD).HasColumnName("Var_CL_PIN_CD");
        Property(t => t.Var_CL_PH1).HasColumnName("Var_CL_PH1");
        Property(t => t.Var_CL_PH2).HasColumnName("Var_CL_PH2");
        Property(t => t.Var_CL_PH3).HasColumnName("Var_CL_PH3");
        Property(t => t.Var_CL_FAX).HasColumnName("Var_CL_FAX");
        Property(t => t.Var_CL_E_MAIL).HasColumnName("Var_CL_E_MAIL");
        Property(t => t.Var_CL_WEB).HasColumnName("Var_CL_WEB");
        Property(t => t.Num_CL_EXM_CAPACITY).HasColumnName("Num_CL_EXM_CAPACITY");
        Property(t => t.Chr_DELETE_FLG).HasColumnName("Chr_DELETE_FLG");
        Property(t => t.Solapur_data_Org_College_CD).HasColumnName("Solapur_data_Org_College_CD");
        Property(t => t.Var_USR_NM).HasColumnName("Var_USR_NM");
        Property(t => t.Dtm_DTE_CR).HasColumnName("Dtm_DTE_CR");
        Property(t => t.Dtm_DTE_UP).HasColumnName("Dtm_DTE_UP");
        Property(t => t.Chr_DISTANCE_EDU).HasColumnName("Chr_DISTANCE_EDU");
    }
}