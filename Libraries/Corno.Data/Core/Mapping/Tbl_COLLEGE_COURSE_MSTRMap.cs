using System.ComponentModel.DataAnnotations.Schema;
using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class Tbl_COLLEGE_COURSE_MSTRMap : EntityTypeConfiguration<Tbl_COLLEGE_COURSE_MSTR>
{
    public Tbl_COLLEGE_COURSE_MSTRMap()
    {
        // Primary Key
        HasKey(t => new { t.NUM_FK_COLLEGE_CD, t.NUM_FK_CO_CD });

        // Properties
        Property(t => t.NUM_FK_COLLEGE_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        Property(t => t.NUM_FK_CO_CD)
            .HasDatabaseGeneratedOption(DatabaseGeneratedOption.None);

        // Table & Column Mappings
        ToTable("Tbl_COLLEGE_COURSE_MSTR");
        Property(t => t.NUM_FK_COLLEGE_CD).HasColumnName("NUM_FK_COLLEGE_CD");
        Property(t => t.NUM_FK_CO_CD).HasColumnName("NUM_FK_CO_CD");
    }
}