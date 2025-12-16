using System.Data.Entity.ModelConfiguration;

namespace Corno.Data.Core.Mapping;

public class TBl_STUDENT_ENV_STUDIESMap : EntityTypeConfiguration<TBl_STUDENT_ENV_STUDIES>
{
    public TBl_STUDENT_ENV_STUDIESMap()
    {
        //// Primary Key
        HasKey(t => new { t.Num_PK_ENTRY_ID });

        // Table & Column Mappings
        ToTable("TBl_STUDENT_ENV_STUDIES");
    }
}