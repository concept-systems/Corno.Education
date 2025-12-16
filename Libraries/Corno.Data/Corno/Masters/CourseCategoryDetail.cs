using Corno.Data.Common;
using Mapster;
using System;
using System.ComponentModel.DataAnnotations.Schema;

namespace Corno.Data.Corno.Masters;

[Serializable]
public class CourseCategoryDetail : BaseModel
{
    #region -- Properties --
    public int? CourseId { get; set; }
    public int CategoryId { get; set; }
    public int RootCategoryId { get; set; }

    [NotMapped]
    public string CategoryName { get; set; }
    [NotMapped]
    public string RootCategoryName { get; set; }

    [AdaptIgnore]
    protected virtual Course Course { get; set; }
    #endregion

    #region -- Public Methods --
    public void Copy(CourseCategoryDetail other)
    {
        if (null == other) return;

        SerialNo = other.SerialNo;
        Code = other.Code;

        CategoryId = other.CategoryId;
        RootCategoryId = other.RootCategoryId;

        Status = other.Status;

        ModifiedBy = other.ModifiedBy;
        ModifiedDate = other.ModifiedDate;
    }
    #endregion
}